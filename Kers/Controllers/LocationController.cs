using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Contexts;
using Kers.Models.Entities.UKCAReporting;
using Microsoft.AspNetCore.Hosting;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class LocationController : BaseController
    {
        KERScoreContext _context;
        KERSmainContext _mainContext;
        KERSreportingContext _reportingContext;
        IKersUserRepository _userRepo;
        IMessageRepository messageRepo;
        ITrainingRepository trainingRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IHostingEnvironment environment;
        public LocationController( 
                    KERSmainContext mainContext,
                    KERSreportingContext _reportingContext,
                    KERScoreContext context,
                    IMessageRepository messageRepo,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    ITrainingRepository trainingRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IHostingEnvironment env
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

/* 
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
            
            var trainings = from i in _context.Meeting select i;
            if(search != null && search != ""){
                if( environment.IsDevelopment()){
                    trainings = trainings.Where( i => i.Subject.Contains(search));
                }else{
                    trainings = trainings.Where( i => EF.Functions.FreeText(i.Subject, search));
                }
                
            }
            if(contacts != null && contacts != ""){
                trainings = trainings.Where( i => i.tContact.Contains(contacts));
            }
            if(start != null){
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                trainings = trainings.Where( i => i.Start >= start);
            }
            if( end != null){
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
                trainings = trainings.Where( i => i.Start <= end);
            }
            if(day != null){
                trainings = trainings
                            .Where( i => 
                                        (i.End == null && (int)i.Start.DayOfWeek == day)
                                        ||
                                        (i.End.HasValue == true && 
                                            (i.Start.DayOfWeek < i.End.Value.DayOfWeek ? 
                                                (int)i.Start.DayOfWeek <= day && (int)i.End.Value.DayOfWeek >= day
                                                : 
                                                (int)i.Start.DayOfWeek >= day && (int)i.End.Value.DayOfWeek <= day )
                                        )
                                    );
            }                            
            IOrderedQueryable<Meeting> result;
            if(order == "asc"){
                result = trainings.OrderByDescending(t => t.Start);
            }else if( order == "alph"){
                result = trainings.OrderBy(t => t.Subject);
            }else{
                result = trainings.OrderBy(t => t.Start);
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

*/

    }
 
}