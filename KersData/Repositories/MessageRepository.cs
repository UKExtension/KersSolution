using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using Microsoft.Extensions.Configuration;


using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Microsoft.AspNetCore.Hosting;

namespace Kers.Models.Repositories
{
    public class MessageRepository : EntityBaseRepository<Message>, IMessageRepository
    {
        private KERScoreContext context;

        public MessageRepository(KERScoreContext context)
            : base(context)
        {
            this.context = context;
        }


        public async void ProcessMessageQueue(IConfiguration configuration, IHostingEnvironment environment){
            var messages = this.context.Message.Where( m => m.IsItSent == false && m.ScheduledFor <= DateTime.Now );
            foreach( var message in messages ) this.sendMessage( message, configuration, environment );
            await this.context.SaveChangesAsync();
        }

        private void sendMessage(Message message, IConfiguration _configuration, IHostingEnvironment environment){
            try{


                using (var client = new SmtpClient ()) {

                    if(environment.IsDevelopment() || environment.IsStaging() ){
                        client.Connect (_configuration["Email:MailServerAddress"], Convert.ToInt32(_configuration["Email:MailServerPort"]), false);
                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove ("XOAUTH2");
                        // Note: only needed if the SMTP server requires authentication
                        client.Authenticate (_configuration["Email:UserId"], _configuration["Email:UserPassword"]);
                    }else if( environment.IsProduction() ){

                    }
                }




                /* 
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

                 */
                //return new OkObjectResult("Email sent successfully!");
            } catch (Exception ex) {
                //return new OkObjectResult(ex.Message);
            }
        }
        //https://github.com/jstedfast/MailKit/issues/126
        void OnMessageSent (object sender, MessageSentEventArgs e)
        {
            //Console.WriteLine ("The message was sent!");
        }




    }
}