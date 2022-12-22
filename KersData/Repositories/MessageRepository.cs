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
using Microsoft.Extensions.Hosting;


using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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


        public List<Message> ProcessMessageQueue(IConfiguration configuration, IWebHostEnvironment environment){
            
            var messages = this.context.Message
                .Where( m => m.IsItSent == false && m.ScheduledFor <= DateTimeOffset.Now )
                .Include( u => u.To).ThenInclude( t => t.PersonalProfile)
                .Include( u => u.To).ThenInclude( t => t.RprtngProfile)
                .ToList();
            foreach( var message in messages ){
                this.sendMessage( message, configuration, environment );
            } 
            this.context.SaveChanges();
            return messages;
        }

        private void sendMessage(Message message, IConfiguration _configuration, IWebHostEnvironment environment){
            


                using (var client = new SmtpClient ()) {
                    try{
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
                        if( message.FromId != null && message.FromId != 0){
                            var FromUser = this.context.KersUser.Where( r => r.Id == message.FromId)
                                    .Include(u => u.PersonalProfile)
                                    .Include(u => u.RprtngProfile)
                                    .FirstOrDefault();
                            m.From.Add (
                                new MailboxAddress (
                                    FromUser.PersonalProfile.FirstName + 
                                    " " + 
                                    FromUser.PersonalProfile.LastName
                                    ,
                                    FromUser.RprtngProfile.Email));
                        }else if(message.FromEmail != null && message.FromEmail != ""){
                            m.From.Add (
                                new MailboxAddress ( message.FromEmail, message.FromEmail)
                            );
                        }else{
                            m.From.Add (
                                new MailboxAddress ( "Program and Staff Development", "agpsd@uky.edu")
                            );
                        }
                        if( message.ToId != null && message.ToId != 0){
                            m.To.Add (new MailboxAddress (
                                message.To.PersonalProfile.FirstName +
                                " " +
                                message.To.PersonalProfile.LastName
                                ,
                                message.To.RprtngProfile.Email));
                        }else if(message.ToEmail != null && message.ToEmail != ""){
                            m.To.Add (
                                new MailboxAddress ( message.ToEmail, message.ToEmail)
                            );
                        }else{
                            m.To.Add (
                                new MailboxAddress ( "Program and Staff Development", "agpsd@uky.edu")
                            );
                        }
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

                        if(environment.IsDevelopment() || environment.IsStaging() ){
                            if(message.To.RprtngProfile.Email == "idene3@uky.edu"){
                                client.MessageSent += OnMessageSent;
                                client.Send (m);
                                message.IsItSent = true;
                                message.SentAt = DateTimeOffset.Now;
                            }
                        }else{
                            client.MessageSent += OnMessageSent;
                            client.Send (m);
                            message.IsItSent = true;
                            message.SentAt = DateTimeOffset.Now;
                        }
                        client.Disconnect (true);
                        
                    } catch (Exception ex) {
                        var log = new Log();
                        log.Type = "Error";
                        log.Time = DateTime.Now;
                        log.ObjectType = "Exception";
                        log.Type = "Send Message Error";
                        log.Object = JsonConvert.SerializeObject(ex.Message,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
                        this.context.Add( log );
                        this.context.SaveChanges();
                        //return new OkObjectResult(ex.Message);
                    }
                }
        }



        public bool ScheduleTrainingMessage(string type, Training training, KersUser To, DateTimeOffset? ScheduledFor = null){
            var template = context.MessageTemplate.Where( t => t.Code == type).FirstOrDefault();
            if( template != null){
                var message = new Message();
                message.FromId = training.OrganizerId;
                message.ToId = To.Id;
                message.Subject = String.Format( template.Subject, training.Subject);
                var trainingArray = this.TrainingToMessageArray(training);
                message.BodyText = String.Format( template.BodyText, trainingArray);
                message.BodyHtml = String.Format( template.BodyHtml, trainingArray);
                message.Created = DateTimeOffset.Now;
                message.ScheduledFor = ScheduledFor??DateTimeOffset.Now;
                message.IsItSent = false;
                context.Add(message);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ScheduleLadderMessage(string type, LadderApplication application, KersUser To){
            var template = context.MessageTemplate.Where( t => t.Code == type).FirstOrDefault();
            if( template != null){
                var message = new Message();
                message.FromId = application.KersUserId;
                message.ToId = To.Id;
                message.Subject = String.Format( template.Subject, application.KersUser.RprtngProfile.Name);
                string[] applicationArray = { application.KersUser.RprtngProfile.Name, application.LadderLevel.Name, application.LastStageId.ToString() };
                message.BodyText = String.Format( template.BodyText, applicationArray);
                message.BodyHtml = String.Format( template.BodyHtml, applicationArray);
                message.Created = DateTimeOffset.Now;
                message.ScheduledFor = DateTimeOffset.Now;
                message.IsItSent = false;
                context.Add(message);
                context.SaveChanges();
                return true;
            }
            return false;
        }


        /***********************************************/
        // Returns array of strings for replacements in
        // the templates.
        //
        // Index Content
        // 0 - Subject
        // 1 - Subject
        // 2 - Start and End dates
        // 3 - Location
        // 4 - Time(s)
        // 5 - Day 1
        // 6 - Day 2
        // 7 - Day 3
        // 8 - Day 4
        // 9 - Contact
        // 10 - Roster as table rows
        // 11 - Times as table rows
        // 12 - Times as text lines
        // 13 - Training Id
        /***********************************************/
        private string[] TrainingToMessageArray(Training training){
            var rstr = "";
            if(training.Enrollment != null && training.Enrollment.Count() > 0){
                if( training.Enrollment.First().Attendie != null && training.Enrollment.First().Attendie.RprtngProfile != null){
                    foreach( var enr in training.Enrollment.OrderBy( f => f.Attendie.RprtngProfile.Name)){
                        rstr += "<tr><td>" + enr.Attendie.RprtngProfile.Name + 
                                "</td><td></td><td>"+enr.Attendie.RprtngProfile.PlanningUnit.Name+"</td></tr>";
                    }
                }
                
            }
            var time = "";
            var TableRows = "";
            var TextLines = "";
            var rowIndex = 1;
            if( training.TrainingSession != null && training.TrainingSession.Count() > 0){
                foreach( var session in training.TrainingSession){
                    time += session.Start.ToString("t") + " - " + session.End.ToString("t") + "<br>";
                    TableRows += "<tr><td class='TblR'>Session " + rowIndex.ToString() + 
                                    ": </td><td>" + OffsetToTimeString(session.Start) + " - " + 
                                    OffsetToTimeString(session.End);
                    if(session.Note != null && session.Note != ""){
                        TableRows += "<br>" + session.Note;
                    }
                    TableRows += "</td></tr>";
                    TextLines += "Session " + rowIndex.ToString() + 
                                    ": " + OffsetToTimeString(session.Start) + " - " + 
                                    OffsetToTimeString(session.End);
                    if(session.Note != null && session.Note != ""){
                        TextLines += "\n" + session.Note;
                    }
                    TextLines += "\n";
                    rowIndex++;
                }
            }else{
                time = training.tTime;
                TableRows = "<tr><td class='TblR'>DAY 1 TIME: </td><td>" + training.day1 +
                             "</td></tr><tr><td class='TblR'>DAY 2 TIME: </td><td>" + training.day2 +
                             "</td></tr><tr><td class='TblR'>DAY 3 TIME: </td><td>" + training.day3 +
                             "</td></tr><tr><td class='TblR'>DAY 4 TIME: </td><td>" + training.day4 +
                             "</td></tr>";
                TextLines = "DAY 1 TIME: " + training.day1 +
                            "\nDAY 2 TIME: " + training.day2 +
                            "\nDAY 3 TIME: " + training.day3 +
                            "\nDAY 4 TIME: " + training.day4 +
                            "\n";
            }
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
                training.tContact,
                rstr,
                TableRows,
                TextLines,
                training.Id.ToString()
            };
            return returnArray;
        }

        private string OffsetToTimeString(DateTimeOffset offset){
            string result = offset.ToString("hh:mm tt");
            if(offset.ToString("%K") == "-05:00"){
                result += " CT";
            }else{
                result += " ET";
            }
            return result;
        }





        //https://github.com/jstedfast/MailKit/issues/126
        void OnMessageSent (object sender, MessageSentEventArgs e)
        {
            var log = new Log();
                log.Type = "Info";
                log.Time = DateTime.Now;
                log.Description = "Successfully Sent Email Message";
                log.ObjectType = "String";
                log.Type = "Sent Message";
                log.Object = JsonConvert.SerializeObject(e.Response + "/n" + e.Message,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,

                                                });
                this.context.Add( log );
                //this.context.SaveChanges();
        }




    }
}