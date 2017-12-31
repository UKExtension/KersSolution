using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Kers.Models.Contexts;
using Kers.Models.Abstract;
using Kers.Services.Abstract;
using Newtonsoft.Json;
using System.Linq;
using Kers.Models.Entities.KERSmain;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;
using Microsoft.AspNetCore.Http.Features;

namespace Kers.Services.Midleware
{
    /// <summary>
    /// Token generator middleware component which is added to an HTTP pipeline.
    /// This class is not created by application code directly,
    /// instead it is added by calling the <see cref="TokenProviderAppBuilderExtensions.UseSimpleTokenProvider(Microsoft.AspNetCore.Builder.IApplicationBuilder, TokenProviderOptions)"/>
    /// extension method.
    /// </summary>
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly KERSmainContext mContext;
        private readonly IKersUserRepository userRepo;
        private readonly KERScoreContext coreContext;
        private readonly IHttpContextAccessor httpContext;
        private readonly IMembershipService membershipService;

        public TokenProviderMiddleware(
            RequestDelegate next,
            KERSmainContext mContext,
            KERScoreContext coreContext,
            IKersUserRepository userRepo,
            IMembershipService membershipService,
            IOptions<TokenProviderOptions> options,
            ILoggerFactory loggerFactory
            , IHttpContextAccessor httpContextAccessor
      )
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<TokenProviderMiddleware>();
            this.mContext = mContext;
            this.coreContext = coreContext;
            this.userRepo = userRepo;
            _options = options.Value;
            this.httpContext = httpContextAccessor;
            this.membershipService = membershipService;
            ThrowIfInvalidOptions(_options);

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            _logger.LogInformation("Handling request: " + context.Request.Path);

            return GenerateToken(context);
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"].ToString();
            var password = context.Request.Form["password"].ToString();

            var identity = await _options.IdentityResolver(username, password);
            if (identity == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            zEmpRptProfile usr;
            SAP_HR_ACTIVE noProfileUser = null;
            if(username == "random"){
                //var cntx = new KERSmainContext();
                usr =  this.mContext.zEmpRptProfiles.OrderBy( i => Guid.NewGuid() ).FirstOrDefault();
                username = usr.linkBlueID;
            }else{
                usr = mContext.zEmpRptProfiles.Where(p => p.linkBlueID == username).FirstOrDefault();
                if(usr == null){
                    noProfileUser = mContext.SAP_HR_ACTIVE.Where(u=>u.Userid == username).FirstOrDefault();
                    if(noProfileUser == null){
                        await context.Response.WriteAsync("Non UK Extension Emoployee.");
                        return;
                    }                    
                }else{
                    /*
                    if( usr.enabled == false ){
                        await context.Response.WriteAsync("User is not enabled.");
                        return;
                    }
                     */
                }

            }
            var now = DateTime.UtcNow;
            List<Claim> claims = new List<Claim>();
            claims.Add( new Claim(JwtRegisteredClaimNames.Sub, username) );
            claims.Add( new Claim(JwtRegisteredClaimNames.Jti, await _options.NonceGenerator()));
            claims.Add( new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64));
            if( noProfileUser == null){
                var user = membershipService.RefreshKersUser(usr);
                //var rpt = membershipService.RefreshRptProfile(user);
                user.LastLogin = DateTime.Now;
                // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
                // You can add other claims here, if you want:
                
                if(user.ExtensionPosition != null){
                    claims.Add( new Claim("ExtensionPosition", user.ExtensionPosition.Id.ToString()));
                }else{
                    claims.Add( new Claim("ExtensionPosition", "10"));
                }
                
                var roles = userRepo.roles(user.Id);

                foreach(var role in roles){
                    var roleClaim = new Claim(ClaimTypes.Role, role.Id.ToString());
                    claims.Add(roleClaim);
                }


                /*********************************************************************/
                // The idea is to set cookies needed
                // for authentication and user data persistanccy
                // in the legacy web form app. Implementing something like a single login.
                // It turned challenging as it is unclear
                // how to set cookies not complaying with the latest http standrds.
                // They should include spaces and special characters in order to work :(  
                /*********************************************************************/
                //this.SetCookies(user, context);
                
                this.Log(user);
                
                
            }
            var claimsArray = claims.ToArray();
            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claimsArray,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);


            var response = new
            {
                newUser = noProfileUser,
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };
            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));
        }


        private void SetCookies( KersUser user, HttpContext context){


            var opts = new CookieOptions();
            opts.Expires = DateTime.Now.Add(_options.Expiration);
            opts.Path = "/";
            

            

            context.Response.Cookies.Append("COAgKersPersonID", user.RprtngProfile.PersonId, opts);
            context.Response.Cookies.Append("COAgKersInstID", user.RprtngProfile.Institution.Code, opts);
            context.Response.Cookies.Append("COAgKersPersonName", user.PersonalProfile.FirstName, opts);
            context.Response.Cookies.Append("COAgKersCntyID", user.RprtngProfile.PlanningUnit.Code, opts);
            context.Response.Cookies.Append("COAgKersCntyName", user.RprtngProfile.PlanningUnit.Name, opts);
            context.Response.Cookies.Append("COAgKersEmailDeliveryAddress", user.RprtngProfile.Email, opts);
            context.Response.Cookies.Append("COAgKers", "ldapDN="+user.PersonalProfile.FirstName+" &ldapID="+user.RprtngProfile.LinkBlueId, opts);
            context.Response.Cookies.Append("COAgKersIsAgent", (user.ExtensionPositionId == 1 ? "1" : "0"), opts);
            context.Response.Cookies.Append("COAgKersIsDD", (hasRole(user, "DD")?"1":"0"), opts);
            context.Response.Cookies.Append("COAgKersPlanningUnitID", user.RprtngProfile.PlanningUnit.Code, opts);
            context.Response.Cookies.Append("COAgKersPlanningUnitName", user.RprtngProfile.PlanningUnit.Name, opts);
            context.Response.Cookies.Append("COAgKersPositionID", user.ExtensionPosition.Code, opts);
            context.Response.Cookies.Append("COAgKersIsCesInServiceTrainer", (hasRole(user, "SRVCTRNR")?"1":"0"), opts);
            context.Response.Cookies.Append("COAgKersIsCesInServiceAdmin", (hasRole(user, "SRVCADM")?"1":"0"), opts);


        }

        private bool hasRole(KersUser user, string role){
            if(user.Roles != null){
                var r = user.Roles.Where( rl => rl.zEmpRoleType.shortTitle == role).FirstOrDefault();
                if(r != null){
                    return true;
                }
            }
            return false;
        }
        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Path));
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            if (options.IdentityResolver == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.IdentityResolver));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }
        }

        private void Log(   KersUser user, 
                            string objectType = "KersUser",
                            string description = "User Logged In", 
                            string type = "Authorization"
                            
                        ){
                            
            var log = new Log();
            log.User = user;
            log.Level = "Information";
            log.Time = DateTime.Now;
            log.ObjectType = objectType;
            log.Object = JsonConvert.SerializeObject(
                                            user,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                }
                                );
            
            if(this.httpContext != null){
                log.Agent = this.httpContext.HttpContext.Request.Headers["User-Agent"].ToString();
                log.Ip = this.httpContext.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            }
            
            log.Description = description;
            log.Type = type;
            this.coreContext.Log.Add(log);
            coreContext.SaveChanges();

        }

        /// <summary>
        /// Get this datetime as a Unix epoch timestamp (seconds since Jan 1, 1970, midnight UTC).
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>Seconds since Unix epoch.</returns>
        public static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}