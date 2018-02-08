using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.RegularExpressions;


using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Kers.Controllers
{
    [Route("api/[controller]")]
    public class EmailController : Controller
    {

        KERScoreContext _context;
        IConfiguration _configuration;
        public EmailController(
            KERScoreContext _context,
            IConfiguration _configuration
        ){
            this._context = _context;
            this._configuration = _configuration;
        }


        [HttpPost("")]
        [Authorize]
        public IActionResult Send([FromBody] Email Email){

            try{
                var message = new MimeMessage ();
                message.From.Add (new MailboxAddress ("Kers System", Email.From));
                message.To.Add (new MailboxAddress ("Kers Recipient", Email.To));
                message.Subject = Email.Subject;

                message.Body = new TextPart ("plain") {
                    Text = Email.Body
                };

                using (var client = new SmtpClient ()) {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s,c,h,e) => true;


                    /*
                    Pressets:

                    1: Ivelin's email
                    2: IIS config (maybe not possible to be used, skipped for now)
                    3: appsettings.json
                    4: custom

                    */

                    if(Email.Pressets == 1){
                        client.Connect ("outlook.office365.com", 587, false);
                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove ("XOAUTH2");
                        // Note: only needed if the SMTP server requires authentication
                        client.Authenticate (_configuration["Email:UserId"], _configuration["Email:UserPassword"]);
                    }else if( Email.Pressets == 3){
                        client.Connect (_configuration["Email:MailServerAddress"], Convert.ToInt32(_configuration["Email:MailServerPort"]), false);
                        client.AuthenticationMechanisms.Remove ("XOAUTH2");
                        client.Authenticate (_configuration["Email:UserId"], _configuration["Email:UserPassword"]);        
                    }else if(Email.Pressets == 4){
                        client.Connect (Email.Server, Email.Port, false);
                        client.AuthenticationMechanisms.Remove ("XOAUTH2");
                        client.Authenticate (Email.Username, Email.Password);
                    }else if(Email.Pressets == 5){
                        client.Connect (Email.Server, Email.Port, false);
                        client.AuthenticationMechanisms.Remove ("XOAUTH2");
                        //client.Authenticate (Email.Username, Email.Password);
                    }
                    client.Send (message);
                    client.Disconnect (true);
                }

                
                return new OkObjectResult("Email sent successfully!");
            } catch (Exception ex) {
                return new OkObjectResult(ex.Message);
            }   
            
        }

        
        public IActionResult Error()
        {
            return View();
        }


    }
    public class Email{
        public int Pressets;
        public string Server;
        public int Port;
        public string Username;
        public string Password;
        public string From;
        public string To;
        public string Subject;
        public string Body;
    }
}
