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
    public class EmailController : BaseController
    {

        KERScoreContext _context;
        IConfiguration _configuration;
        public EmailController(
            KERSmainContext mainContext,
            IKersUserRepository userRepo,
            KERScoreContext _context,
            IConfiguration _configuration
        ):base(mainContext, _context, userRepo)
        {
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
                // https://github.com/jstedfast/MailKit
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
                    //https://github.com/jstedfast/MailKit/issues/126
                    client.MessageSent += OnMessageSent;
                    client.Send (message);
                    client.Disconnect (true);
                }

                
                return new OkObjectResult("Email sent successfully!");
            } catch (Exception ex) {
                return new OkObjectResult(ex.Message);
            }   
            
        }

        //https://github.com/jstedfast/MailKit/issues/126
        void OnMessageSent (object sender, MessageSentEventArgs e)
        {
            //Console.WriteLine ("The message was sent!");
        }





        /******************************** */
        // Email Templates CRUD Operations
        /******************************** */




        [HttpPost("addtemplate")]
        [Authorize]
        public IActionResult AddTemplate( [FromBody] MessageTemplate template){
            if(template != null){
                var user = this.CurrentUser();
                template.CreatedBy = user;
                template.UpdatedBy = user;
                template.Updated = template.Created = DateTimeOffset.Now;
                context.Add(template); 
                context.SaveChanges();
                this.Log(template,"MessageTemplate", "Message Template Added.");
                return new OkObjectResult(template);
            }else{
                this.Log( template ,"MessageTemplate", "Error in adding Message Template attempt.", "MessageTemplate", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("updatetemplate/{id}")]
        [Authorize]
        public IActionResult UpdateTemplate( int id, [FromBody] MessageTemplate template){
           
            var entity = context.MessageTemplate.Find(id);

            if(template != null && entity != null){
                var user = this.CurrentUser();
                entity.Code = template.Code;
                entity.Subject = template.Subject;
                entity.BodyHtml = template.BodyHtml;
                entity.BodyText = template.BodyText;
                entity.UpdatedBy = user;
                entity.Updated = DateTimeOffset.Now;
                context.SaveChanges();
                this.Log(entity,"MessageTemplate", "Message Template Updated.");

                return new OkObjectResult(entity);
            }else{
                this.Log( entity ,"MessageTemplate", "Not Found MessageTemplate in an update attempt.", "MessageTemplate", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("deletetemplate/{id}")]
        [Authorize]
        public IActionResult DeleteTemplate( int id ){
            var entity = context.MessageTemplate.Find(id);
            
            
            if(entity != null){
                
                context.MessageTemplate.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"MessageTemplate", "MessageTemplate Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"MessageTemplate", "Not Found MessageTemplate in a delete attempt.", "MessageTemplate", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpGet("gettemplates")]
        [Authorize]
        public async Task<IActionResult> GetTemplates(){
            return new OkObjectResult(await _context.MessageTemplate.ToListAsync());
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
