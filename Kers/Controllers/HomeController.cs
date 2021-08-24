using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Kers.Services.Abstract;
using Kers.Models.Entities.KERScore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace Kers.Controllers
{
    public class HomeController : Controller
    {

        private IConfiguration _configuration;
        private IWebHostEnvironment _hostingEnv;
        private readonly KERSmainContext mContext;
        private readonly IKersUserRepository userRepo;
        private readonly KERScoreContext coreContext;
        private readonly IMembershipService membershipService;
        private readonly IHttpContextAccessor httpContext;
        public HomeController(
            IConfiguration _configuration,
            IWebHostEnvironment _hostingEnv,
            KERSmainContext mContext,
            KERScoreContext coreContext,
            IKersUserRepository userRepo,
            IMembershipService membershipService,
            IHttpContextAccessor httpContextAccessor
        ){
            this._configuration = _configuration;
            this._hostingEnv = _hostingEnv;
            this.mContext = mContext;
            this.coreContext = coreContext;
            this.userRepo = userRepo;
            this.membershipService = membershipService;
            this.httpContext = httpContextAccessor;
        }
        public IActionResult Index()
        {
            ViewData["isCompatible"] = !IsInternetExplorer(Request.Headers["User-Agent"].ToString());
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/token")]
        public IActionResult Post( [FromBody]LoginViewModel loginViewModel)
        {

            if (ModelState.IsValid)
            {
                //This method returns user id from username and password.
                var userId = GetUserIdFromCredentials(loginViewModel); 
                if (userId == "-1")
                {
                    var errorMessage = "Username and Password Missmatch";
                    return Ok(new {error = errorMessage});
                }


                zEmpRptProfile usr;
                SAP_HR_ACTIVE noProfileUser = null;
                if(loginViewModel.Username == "random"){
                    //var cntx = new KERSmainContext();
                    usr =  this.mContext.zEmpRptProfiles.AsEnumerable().OrderBy( i => Guid.NewGuid() ).FirstOrDefault();
                    loginViewModel.Username = usr.linkBlueID;
                }else{
                    var length = loginViewModel.Username.Length;
                    if( length > 8 && loginViewModel.Username.Substring( length - 8, 8) == "@uky.edu"){
                        loginViewModel.Username = loginViewModel.Username.Substring( 0, length - 8 );
                    }
                    usr = mContext.zEmpRptProfiles.Where(p => p.linkBlueID == loginViewModel.Username).FirstOrDefault();
                    if(usr == null){
                        noProfileUser = mContext.SAP_HR_ACTIVE.Where(u=>u.Userid == loginViewModel.Username).FirstOrDefault();
                        if(noProfileUser == null){
                            var errorMessage = "Non UK Extension Emoployee.";
                            return Ok(new {error = errorMessage});
                        }                    
                    }else{
                        
                        if( usr.enabled == false ){
                            var errorMessage = "Your Account is Disabled. Please Contact your District Director for Providing you Access.";
                            return Ok(new {error = errorMessage});
                        }
                        
                    }

                }

                List<Claim> claims = new List<Claim>();
                claims.Add( new Claim(JwtRegisteredClaimNames.Sub, loginViewModel.Username) );
                claims.Add( new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
                if( noProfileUser == null){
                    
                    var user = coreContext.
                                KersUser.
                                Where(u => u.classicReportingProfileId == usr.Id).
                                Include(u => u.Roles).
                                Include(u => u.PersonalProfile).
                                Include(u => u.RprtngProfile).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.GeneralLocation).
                                Include(u => u.ExtensionPosition).
                                Include(u=> u.Specialties).ThenInclude(s=>s.Specialty).
                                FirstOrDefault();
                    if( user == null ){
                        user = membershipService.RefreshKersUser(usr);
                    }
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

                    this.Log(user);
                    
                }
                var claimsArray = claims.ToArray();

                var token = new JwtSecurityToken
                (
                    issuer: "KERSSystem",
                    audience: "KersUsers",
                    claims: claimsArray,
                    expires: DateTime.UtcNow.AddDays(60),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Secret:JWTKey"])),
                            SecurityAlgorithms.HmacSha256)
                );
                var response = new
                {
                    newUser = noProfileUser,
                    access_token = new JwtSecurityTokenHandler().WriteToken(token)
                };

                return Ok(response);
            }

            return BadRequest();
        }

        private string GetUserIdFromCredentials(LoginViewModel loginViewModel){
            // Don't do this in production, obviously!
            
            if( _hostingEnv.EnvironmentName == "Development" || _hostingEnv.EnvironmentName == "Staging"){
                //if (loginViewModel.Username == "random"){
                    return loginViewModel.Username;
                //}
            }
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            String uri = "https://kers.ca.uky.edu/kers_mobile/Handler.ashx";

            var length = loginViewModel.Username.Length;
            var username = loginViewModel.Username;
            if( length > 8 && loginViewModel.Username.Substring( length - 8, 8) == "@uky.edu"){
                username = username.Substring( 0, length - 8 );
            }



            Dictionary<string, string> pairs = new Dictionary<string,string>();
            pairs.Add("username", username);
            pairs.Add("password", loginViewModel.Password);
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(pairs);

            var result = client.PostAsync(uri, formContent).Result;
            var data = result.Content.ReadAsStringAsync().Result;

            if( data == "{\"valid\":true}"){
                return loginViewModel.Username;
            }
            return "-1";
        }



        public static bool IsInternetExplorer(string userAgent) {
            if(userAgent.Contains("MSIE")) {
                return true;
            } else {
                return false;
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

    public class LoginViewModel{
        public string Username;
        public string Password;

    }
}
