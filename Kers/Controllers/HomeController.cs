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
using Kers.Services;

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
        [Route("auth")]
        public IActionResult Auth( [FromForm]LoginViewModel loginViewModel)
        {

            // 1. TODO: specify the certificate that your SAML provider gave you
            string samlCertificate = @"-----BEGIN CERTIFICATE-----
MIIC2jCCAcKgAwIBAgIQZbcjAuiuwKVHMP+ILffqRTANBgkqhkiG9w0BAQsFADApMScwJQYDVQQDEx5BREZTIEVuY3J5cHRpb24gLSBhZGZzLnVreS5lZHUwHhcNMTcxMjE1MTUyMzMyWhcNMjcxMjEzMTUyMzMyWjApMScwJQYDVQQDEx5BREZTIEVuY3J5cHRpb24gLSBhZGZzLnVreS5lZHUwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDRRm18Dju8YhsugHh6y8w0Pqu+x+xzhlTtUo00hRel1a/hq30Sz0RnUWn686gEM4C6J502YZ4hqTbDnidhGrv3yI5F/F859vNBKL1aNWTUYpGOK45uSEgXLULiv4aXlSwNPJZ2FmHZfS5JP6ekhccW8fNQXSHpeEl949P5rxafyn/dw0nZAPo1zyvFB4gLGdwti6GtU0WBQQrcT7MPBMpEiILTdhEptdgBIdv9gIg9Ryow55cgtH4odqpoB6vcofPpdVf6EIfPaDiXjhHAH/gwgkIXV2AFtmHFfSLv9JfNZFmruuVxXy1ZntPZbaZLx8WdZPqKvRePivJH9Sytbq7PAgMBAAEwDQYJKoZIhvcNAQELBQADggEBAA5NNArgCIHHMfk9tMFS8n01IyuJYo60QGGIm2K4WAw2G6btP+F+4iesv1qWwOFrQhe/wx0siD+Qyag8aGtoro+sJIh0oHPWZH1+eh26S0rssn+GYOQjNjjsznbj6ZywKVX2TiPkMvYvoniRlDlC9kNBc+OzYCUIy+7SzkZfl0IrILWhtpN/XUuCOTv2xVyLlgyXicQK2mIahdufr0PcvVGeJmKcY9PBk8B4YZGC6LMAzeroop4DTHC8FtQrZwJzyA3bNEUjFB12zG2MUceHop3px4zTZ35sp0xekj3jQ+6E1javqwVU3xTxfKws33Vi/P2RMqt0b1zCfM/oSWRkras=
-----END CERTIFICATE-----";
            // 2. Let's read the data - SAML providers usually POST it into the "SAMLResponse" var
            ViewData["rawresponse"] = Request.Form["SAMLResponse"];
            var samlResponse = new Response(samlCertificate, Request.Form["SAMLResponse"]);
            ViewData["response"] = samlResponse;
            // 3. We're done!
            if (samlResponse.IsValid())
            {
                //WOOHOO!!! user is logged in

                //Some more optional stuff for you
                //let's extract username/firstname etc
                
                string username, email, firstname, lastname;
                try
                {
                    username = samlResponse.GetNameID();
                    email = samlResponse.GetEmail();
                    firstname = samlResponse.GetFirstName();
                    lastname = samlResponse.GetLastName();
                }
                catch(Exception ex)
                {
                    //insert error handling code
                    //no, really, please do
                    return null;
                }

                //user has been authenticated, put your code here, like set a cookie or something...
                //or call FormsAuthentication.SetAuthCookie()
                //or call context.SignInAsync() in ASP.NET Core
                //or do something else
            }
            return View();


        }

        [AllowAnonymous]
        [HttpGet]
        [Route("federationmetadata/federationmetadata.xml")]
        public IActionResult Federationmetadata( )
        {
            var today = DateTime.Now;

            string xmlString = @"<?xml version=""1.0""?>
<md:EntityDescriptor xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata""
                     validUntil="""+ today.AddDays(12).ToString("yyyy-MM-ddTHH:mm:ssK") + @"""
                     cacheDuration=""PT604800S""
                     entityID=""https://kers.ca.uky.edu"">
    <md:SPSSODescriptor AuthnRequestsSigned=""false"" WantAssertionsSigned=""false"" protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
        <md:SingleLogoutService Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
                                Location=""https://kers.ca.uky.edu/core/logout"" />
        <md:NameIDFormat>urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified</md:NameIDFormat>
        <md:AssertionConsumerService Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
                                     Location=""https://kers.ca.uky.edu/core/auth""
                                     index=""1"" />
        
    </md:SPSSODescriptor>
    <md:Organization>
       <md:OrganizationName xml:lang=""en-US"">College of Agriculture Food and Environment</md:OrganizationName>
       <md:OrganizationDisplayName xml:lang=""en-US"">CAFE</md:OrganizationDisplayName>
       <md:OrganizationURL xml:lang=""en-US"">https://www.ca.uky.edu</md:OrganizationURL>
    </md:Organization>
    <md:ContactPerson contactType=""technical"">
        <md:GivenName>Ivelin Denev</md:GivenName>
        <md:EmailAddress>ivelin.denev@uky.edu</md:EmailAddress>
    </md:ContactPerson>
    <md:ContactPerson contactType=""support"">
        <md:GivenName>Ivelin Denev</md:GivenName>
        <md:EmailAddress>ivelin.denev@uky.edu</md:EmailAddress>
    </md:ContactPerson>
</md:EntityDescriptor>";
            return this.Content(xmlString, "text/xml");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("loginsso")]
        public ActionResult LoginSSO()
        {
            //TODO: specify the SAML provider url here, aka "Endpoint"
            var samlEndpoint = "https://adfs.uky.edu/adfs/ls/";

            var request = new AuthRequest(
                "https://kers.ca.uky.edu", //TODO: put your app's "entity ID" here
                "https://kers.ca.uky.edu/core/auth" //TODO: put Assertion Consumer URL (where the provider should redirect users after authenticating)
                );


            var red = request.GetRedirectUrl(samlEndpoint);
            //redirect the user to the SAML provider
            return Redirect(red);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logintest")]
        public IActionResult LoginTestPage( )
        {
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
