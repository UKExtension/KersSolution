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
using Microsoft.EntityFrameworkCore;

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


        public List<Message> ProcessMessageQueue(IConfiguration configuration, IHostingEnvironment environment){
            var messages = this.context.Message
                .Where( m => m.IsItSent == false && m.ScheduledFor <= DateTime.Now )
                .Include( m => m.From ).ThenInclude( u => u.PersonalProfile)
                .Include( m => m.To).ThenInclude( u => u.PersonalProfile)
                .Include( m => m.From).ThenInclude( u => u.RprtngProfile)
                .Include( m => m.To).ThenInclude( u => u.RprtngProfile)
                .ToList();
            foreach( var message in messages ) this.sendMessage( message, configuration, environment );
            this.context.SaveChanges();
            return messages;
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
                        client.Connect (_configuration["Email:MailServerAddress"], Convert.ToInt32(_configuration["Email:MailServerPort"]), false);
                        client.AuthenticationMechanisms.Remove ("XOAUTH2");
                    }
                    var m = new MimeMessage ();
                    if( message.From != null ){
                        m.From.Add (
                            new MailboxAddress (
                                message.From.PersonalProfile.FirstName + 
                                " " +
                                message.From.PersonalProfile.LastName
                                ,
                                message.From.RprtngProfile.Email));
                    }else{
                        m.From.Add (
                            new MailboxAddress ( "Program and Staff Development", "agpsd@uky.edu")
                        );
                    }
                    
                    m.To.Add (new MailboxAddress (
                        message.To.PersonalProfile.FirstName +
                        " " +
                        message.To.PersonalProfile.LastName
                        ,
                        message.To.RprtngProfile.Email));
                    m.Subject = message.Subject;
                    var alternative = new MultipartAlternative ();
                    alternative.Add (new TextPart ("plain") {
                        Text = message.BodyText
                    });
                    alternative.Add (new TextPart ("html") {
                        Text = message.BodyHtml
                    });
                    m.Body = alternative;
                    //https://github.com/jstedfast/MailKit/issues/126
                    client.MessageSent += OnMessageSent;
                    client.Send (m);
                    client.Disconnect (true);
                    message.IsItSent = true;
                    //context.SaveChanges();



                }




                
                
/* 
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
                var log = new Log();
                log.Type = "Error";
                log.Time = DateTime.Now;
                log.ObjectType = "Exception";
                log.Type = "Send Message";
                log.Object = ex.Message;
                this.context.Add( log );
                this.context.SaveChanges();
                //return new OkObjectResult(ex.Message);
            }
        }



        public async Task<bool> ScheduleTrainingMessage(string type, Training training, KersUser To, DateTimeOffset? ScheduledFor = null){
            var template = await context.MessageTemplate.Where( t => t.Code == type).FirstOrDefaultAsync();
            if( template != null){
                var message = new Message();
                message.FromId = training.OrganizerId;
                message.ToId = To.Id;
                message.Subject = String.Format( template.Subject, training.Subject);
                var trainingArray = this.TrainingToMessageArray(training);
                message.BodyText = String.Format( template.BodyText, trainingArray);
                message.BodyHtml = String.Format( template.BodyHtml, trainingArray);
                message.Created = DateTimeOffset.Now;
                if( ScheduledFor != null) message.ScheduledFor = ScheduledFor??DateTimeOffset.Now;
                message.IsItSent = false;
                context.Add(message);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        private string[] TrainingToMessageArray(Training training){
            var returnArray = new string[]{
                training.Subject,
                training.Subject,
                training.Start.ToString("MM/dd/yyyy") + (training.End != null? " - " + training.End?.ToString("MM/dd/yyyy") : ""),
                training.tLocation,
                training.tTime,
                training.day1,
                training.day2,
                training.day3,
                training.day4,
                training.tContact
            };
            return returnArray;
        }





        //https://github.com/jstedfast/MailKit/issues/126
        void OnMessageSent (object sender, MessageSentEventArgs e)
        {
            //Console.WriteLine ("The message was sent!");
        }




    }
}