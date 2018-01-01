using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Kers.Controllers
{
    public class HomeController : Controller
    {

        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnv;
        public HomeController(
            IConfiguration _configuration,
            IHostingEnvironment _hostingEnv
        ){
            this._configuration = _configuration;
            this._hostingEnv = _hostingEnv;
        }
        public IActionResult Index()
        {
            ViewData["isCompatible"] = !IsInternetExplorer(Request.Headers["User-Agent"].ToString());
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("token")]
        public IActionResult Post([FromBody]LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                //This method returns user id from username and password.
                var userId = GetUserIdFromCredentials(loginViewModel); 
                if (userId == "-1")
                {
                    return Unauthorized();
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, loginViewModel.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken
                (
                    issuer: "KERSSystem",
                    audience: "KersUsers",
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(60),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Secret:JWTKey"])),
                            SecurityAlgorithms.HmacSha256)
                );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return BadRequest();
        }

        private string GetUserIdFromCredentials(LoginViewModel loginViewModel){
            // Don't do this in production, obviously!
            
            if( _hostingEnv.EnvironmentName == "Development" ){
                if (loginViewModel.Username == "random"){
                    return loginViewModel.Username;
                }
            }
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            String uri = "https://kers.ca.uky.edu/kers_mobile/Handler.ashx";

            Dictionary<string, string> pairs = new Dictionary<string,string>();
            pairs.Add("username", loginViewModel.Username);
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
    }

    public class LoginViewModel{
        public string Username;
        public string Password;
    }
}
