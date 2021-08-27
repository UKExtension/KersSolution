using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
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
using System.Net.Http;
using Kers.Models.Entities.UKCAReporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class MeetingsController : ExtensionEventController
    {
        KERScoreContext _context;
        KERSmainContext _mainContext;
        KERSreportingContext _reportingContext;
        IKersUserRepository _userRepo;
        IMessageRepository messageRepo;
        ITrainingRepository trainingRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IWebHostEnvironment environment;
        private string DefaultTime = "12:34:56.1000000";
        public MeetingsController( 
                    KERSmainContext mainContext,
                    KERSreportingContext _reportingContext,
                    KERScoreContext context,
                    IMessageRepository messageRepo,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    ITrainingRepository trainingRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IWebHostEnvironment env
            ):base(mainContext, context, userRepo){
           this._context = context;
           this._mainContext = mainContext;
           this._reportingContext = _reportingContext;
           this.messageRepo = messageRepo;
           this._userRepo = userRepo;
           this.logRepo = logRepo;
           this.trainingRepo = trainingRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           this.environment = env;
        }


        [HttpPost("addmeeting")]
        [Authorize]
        public IActionResult AddMeeting( [FromBody] MeetingWithTime meeting){
            if(meeting != null){

                Meeting m = new Meeting();
                var user = this.CurrentUser();
                m.Organizer = user;
                m.CreatedDateTime = DateTime.Now;
                m.BodyPreview = m.Body = meeting.Body;
                m.LastModifiedDateTime = meeting.CreatedDateTime;
                var timezone = meeting.etimezone ? " -04:00":" -05:00";
                var starttime = this.DefaultTime;
                if(meeting.IsAllDay == false){
                    var endtime = this.DefaultTime;
                    if(meeting.Endtime != ""){
                        endtime = meeting.Endtime+":00.1000000";
                    }
                    if(meeting.End.HasValue){
                        m.End = DateTimeOffset.Parse(meeting.End.Value.ToString("yyyy-MM-dd ") + endtime + timezone);
                    }
                    if(meeting.Starttime != ""){
                        starttime = meeting.Starttime+":00.1000000";
                    }
                }else{
                    m.End = null; 
                }
                m.Start = DateTimeOffset.Parse(meeting.Start.ToString("yyyy-MM-dd ") + starttime + timezone);
                m.Subject = meeting.Subject;
                m.IsAllDay = meeting.IsAllDay;
                m.IsCancelled = meeting.IsCancelled;
                m.tContact = meeting.tContact;
                m.tLocation = meeting.tLocation;
                context.Add(m); 
                context.SaveChanges();
                this.Log(meeting,"Meeting", "Meeting Added.");
                return new OkObjectResult(meeting);
            }else{
                this.Log( meeting ,"Meeting", "Error in adding meeting attempt.", "Meeting", "Error");
                return new StatusCodeResult(500);
            }
        }


        
        [HttpPut("updatemeeting/{id}")]
        [Authorize]
        public IActionResult UpdateMeeting( int id, [FromBody] MeetingWithTime meeting){
            var mtng = context.Meeting.Where( t => t.Id == id).FirstOrDefault();
            if(meeting != null && mtng != null ){
                
                var timezone = meeting.etimezone ? " -04:00":" -05:00";
                var starttime = this.DefaultTime;
                if(meeting.IsAllDay == false){
                    var endtime = this.DefaultTime;
                    if(meeting.Endtime != ""){
                        endtime = meeting.Endtime+":00.1000000";
                    }
                    if(meeting.End.HasValue){
                        mtng.End = DateTimeOffset.Parse(meeting.End.Value.ToString("yyyy-MM-dd ") + endtime + timezone);
                    }
                    if(meeting.Starttime != ""){
                        starttime = meeting.Starttime+":00.1000000";
                    }
                }else{
                    mtng.End = null;
                }
                mtng.Start = DateTimeOffset.Parse(meeting.Start.ToString("yyyy-MM-dd ") + starttime + timezone);
                mtng.IsAllDay = meeting.IsAllDay;
                mtng.Body = mtng.BodyPreview = meeting.Body;
                mtng.tContact = meeting.tContact;
                mtng.tLocation = meeting.tLocation;
                mtng.Subject = meeting.Subject;
                mtng.LastModifiedDateTime = DateTime.Now;
                mtng.IsCancelled = meeting.IsCancelled;
                context.SaveChanges();
                this.Log(meeting,"Meeting", "Meeting Updated.");
                return new OkObjectResult(meeting);
            }else{
                this.Log( meeting ,"Meeting", "Not Found Meeting in an update attempt.", "Meeting", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("deletemeeting/{id}")]
        public IActionResult DeleteMeeting( int id ){
            var entity = context.Meeting.Find(id);
            
            if(entity != null){
                
                context.Meeting.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"Meeting", "Meeting Deleted.");

                return new OkResult();
            }else{
                this.Log( id ,"Meeting", "Not Found Meeting in a delete attempt.", "Meeting", "Error");
                return new StatusCodeResult(500);
            }
        }




        [HttpGet("MeetingsPerPeriod/{start}/{end?}/{order?}")]
        [Authorize]
        public async Task<IActionResult> MeetingsPerPeriod(DateTime start, DateTime? end = null, string order = "start"){
            IQueryable<Meeting> query = _context.Meeting.Where( t => t.Start > start);
            if(end != null){
                query = query.Where( t => t.Start < end);
            }
            if(order == "end"){
                query = query.OrderBy(t => t.End);
            }else if( order == "created"){
                query = query.OrderBy(t => t.CreatedDateTime);
            }else{
                query = query.OrderBy(t => t.Start);
            }
            

            List<Meeting> list = await query.ToListAsync();

            return new OkObjectResult(list);
        }



        [HttpGet("GetCustom")]
        public IActionResult GetCustom( [FromQuery] string search, 
                                        [FromQuery] DateTime start,
                                        [FromQuery] DateTime end,
                                        [FromQuery] string status,
                                        [FromQuery] string contacts,
                                        [FromQuery] int? day,
                                        [FromQuery] string order,
                                        [FromQuery] bool withseats,
                                        [FromQuery] bool attendance
                                        ){
            start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);;
            List<Meeting> trainings;
            if(environment.IsDevelopment()){
                trainings = _context.Meeting.ToList();
                trainings = trainings.Where( i => i.Start.UtcDateTime >= start && i.Start <= end ).ToList();
            }else{
                trainings = _context.Meeting.Where( i => i.Start >= start && i.Start <= end ).ToList();
            }
            
            if(search != null && search != ""){
                if( this.environment.IsDevelopment()  ){
                    trainings = trainings.Where( i => i.Subject.Contains(search)).ToList();
                }else{
                    trainings = trainings.Where( i => EF.Functions.FreeText(i.Subject, search)).ToList();
                }
                
            }
            if(contacts != null && contacts != ""){
                trainings = trainings.Where( i => i.tContact.Contains(contacts)).ToList();
            }
            List<Meeting> result = trainings;
            
            if(day != null){
                result = result
                            .Where( i => 
                                        (i.End == null && (int)i.Start.DayOfWeek == day)
                                        ||
                                        (i.End.HasValue == true && 
                                            (i.Start.DayOfWeek < i.End.Value.DayOfWeek ? 
                                                (int)i.Start.DayOfWeek <= day && (int)i.End.Value.DayOfWeek >= day
                                                : 
                                                (int)i.Start.DayOfWeek >= day && (int)i.End.Value.DayOfWeek <= day )
                                        )
                                    ).ToList();
            }                            
            
            if(order == "asc"){
                result = result.OrderByDescending(t => t.Start).ToList();
            }else if( order == "alph"){
                result = result.OrderBy(t => t.Subject).ToList();
            }else{
                result = result.OrderBy(t => t.Start).ToList();
            }
            var mtngs = new List<MeetingWithTime>();
            foreach( var mtng in result){
                var m = new MeetingWithTime(mtng);
                mtngs.Add(m);
            } 
            return new OkObjectResult(mtngs);
        }





        [HttpGet("migrate")]
        public IActionResult MigrateInServiceTrainings(){
            var meetings = new List<Meeting>();
            var services = this._reportingContext.zCesEvent.OrderByDescending(m => m.rID);//.Skip(10).Take(10);
            foreach( var service in services){
               // var meeting = CES2Meeting( service );
               // meetings.Add( meeting );
            }
            this.context.AddRange(meetings);
            //await this.context.SaveChangesAsync();  
            return new OkObjectResult(meetings);
        }




        private Meeting CES2Meeting(zCesEvent service){
            try{
                /* if( !(await context.Meeting.Where( t => t.mClassicId == id).AnyAsync()) ){
                    var service = await this._reportingContext.zCesEvent.Where( s => s.rID == id).FirstOrDefaultAsync();
                    if( service != null){ */
                        //if( !(await this.context.Meeting.Where( t => true).AnyAsync()) ){
                                var meeting = new Meeting();
                                meeting.Subject = service.eventTitle;
                                meeting.Body = (service.eventDescription == "NULL"?"":service.eventDescription);
                                meeting.tContact =  service.eventContact == "NULL" ? "" : service.eventContact;
                                meeting.tLocation = service.eventLocation == "NULL" ? "" : service.eventLocation;
                                var tm = "12:34:56.1000000 -04:00";
                                var endTm = tm;
                                if( service.eventTimeBegin != null & service.eventTimeBegin != "NULL"){
                                    tm = this.ProcessTime( service.eventTimeBegin);      
                                    meeting.IsAllDay = false;
                                }else{
                                    meeting.IsAllDay = true;
                                }
                                if( service.eventTimeEnd != null & service.eventTimeEnd != "NULL"){
                                    endTm = this.ProcessTime( service.eventTimeEnd);
                                }
                                string dateString = ToDateString(service.eventDateBegin) + " " + tm;
                                meeting.Start = DateTimeOffset.Parse(dateString);
                                string endDateString = ToDateString(service.eventDateEnd) + " " + endTm;
                                meeting.End = DateTimeOffset.Parse(endDateString);
                                meeting.OrganizerId = 747;
                                //this.context.Add(meeting);
                                //await this.context.SaveChangesAsync();
                                return meeting;
                            
                            
                        /* }
                    } 
                } */
            }catch( Exception e ){
                this.Log( e.Message ,"Meeting", "Migration Error.", "Meeting", "Error");
            }
            return null;
        }

        private string ProcessTime( string time){
            var hours = "12";
            var minutes = "34";
            var seconds = "56";
            var timeshift = "-04:00";
            var tmArr = time.Split(' ');
            if( tmArr.Count() == 1 ) return tmArr[0]+":56.1000000 -04:00";
            if( tmArr.Count() > 1 ){
                if(tmArr.Count() > 2){
                    if( tmArr[2] == "CST" || tmArr[2] == "CT" || tmArr[2] == "(CDT)" ){
                        timeshift = "-05:00";
                    }
                }
                var hrArr = tmArr[0].Split( ':' );
                if (Int32.TryParse(hrArr[0], out int hourPart)){
                    if( hrArr.Count() > 1 ){
                        if( tmArr[1] == "pm" || tmArr[1]=="PM"){
                            hours = hourPart < 12 ? (hourPart + 12).ToString("D2") : "12";
                        }else{
                            hours = hourPart.ToString("D2");
                        }
                    }
                }
                minutes = hrArr[1].Substring(0,2);
            }
            return hours + ":" + minutes + ":" + seconds + ".1000000 " + timeshift;
        }
        private string ToDateString( string oldDate){
            var parts = oldDate.Split('/');
            if(parts.Count() > 2){
                return parts[2] + "-" + parts[0] + "-" + parts[1];
            }
            return null;
        }





    }


    public class MeetingWithTime: Meeting{

        public MeetingWithTime(){}

        public MeetingWithTime(Meeting m){
            this.Id = m.Id;
            this.Body = m.Body;
            this.BodyPreview = m.BodyPreview;
            this.Start = m.Start;
            this.End = m.End;
            this.IsAllDay = m.IsAllDay;
            this.CreatedDateTime = m.CreatedDateTime;
            this.LastModifiedDateTime = m.LastModifiedDateTime;
            this.Subject = m.Subject;
            this.IsCancelled = m.IsCancelled;
            this.tLocation = m.tLocation;
            this.tContact = m.tContact;
            

            if(this.IsAllDay == false){
                TimeSpan tmzn = m.Start.Offset;
                var hrs = tmzn.TotalHours;
                this.etimezone = hrs == -4;
                if(this.End.HasValue ){
                    this.Endtime = this.End.Value.Hour.ToString("D2")+ ":" + this.End.Value.Minute.ToString("D2");
                }
                
            }
            this.Starttime = this.Start.Hour.ToString("D2") + ":" + this.Start.Minute.ToString("D2");

        }
        public String Starttime;
        public String Endtime;
        public Boolean etimezone;
    }

}