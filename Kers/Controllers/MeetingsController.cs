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
        IHostingEnvironment environment;
        public MeetingsController( 
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


        [HttpPost()]
        [Authorize]
        public IActionResult AddMeeting( [FromBody] Meeting meeting){
            if(meeting != null){
                var user = this.CurrentUser();
                meeting.Organizer = user;
                meeting.CreatedDateTime = DateTime.Now;
                meeting.BodyPreview = meeting.Body;
                meeting.LastModifiedDateTime = meeting.CreatedDateTime;
                meeting.IsCancelled = false;
                context.Add(meeting); 
                context.SaveChanges();
                this.Log(meeting,"Meeting", "Meeting Added.");
                return new OkObjectResult(meeting);
            }else{
                this.Log( meeting ,"Meeting", "Error in adding meeting attempt.", "Meeting", "Error");
                return new StatusCodeResult(500);
            }
        }


        
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateMeeting( int id, [FromBody] Meeting meeting){
            var mtng = context.Meeting.Where( t => t.Id == id).FirstOrDefault();
            if(meeting != null && mtng != null ){
                mtng.Start = meeting.Start;
                mtng.End = meeting.End;
                mtng.IsAllDay = meeting.IsAllDay;
                mtng.Body = mtng.BodyPreview = meeting.Body;
                mtng.mContact = meeting.mContact;
                mtng.mLocation = meeting.mLocation;
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

        [HttpDelete("{id}")]
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





        [HttpGet("migrate")]
        public async Task<IActionResult> MigrateInServiceTrainings(){
            var meetings = new List<Meeting>();
            var services = this._reportingContext.zCesEvent.OrderByDescending(m => m.rID);//.Skip(10).Take(10);
            foreach( var service in services){
                var meeting = await CES2Meeting( service );
                meetings.Add( meeting );
            }
            this.context.AddRange(meetings);
            await this.context.SaveChangesAsync();  
            return new OkObjectResult(meetings);
        }




        private async Task<Meeting> CES2Meeting(zCesEvent service){
            try{
                /* if( !(await context.Meeting.Where( t => t.mClassicId == id).AnyAsync()) ){
                    var service = await this._reportingContext.zCesEvent.Where( s => s.rID == id).FirstOrDefaultAsync();
                    if( service != null){ */
                        if( !(await this.context.Meeting.Where( t => t.mClassicId == service.rID).AnyAsync()) ){
                                var meeting = new Meeting();
                                meeting.mClassicId = service.rID;
                                meeting.Subject = service.eventTitle;
                                meeting.Body = (service.eventDescription == "NULL"?"":service.eventDescription);
                                meeting.mContact = service.eventContact;
                                meeting.mLocation = service.eventLocation;
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
                                //this.context.Add(meeting);
                                //await this.context.SaveChangesAsync();
                                return meeting;
                            
                            
                        }/* 
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






/*
        




        [HttpPut("postattendance/{id}")]
        [Authorize]
        public IActionResult PostAttendance( int id, [FromBody] Training training){
           

            var trn = context.Training
                                .Where( t => t.Id == id)
                                .Include( t => t.Enrollment)
                                .FirstOrDefault();
            if(training != null && trn != null ){
                foreach( var enr in trn.Enrollment ){
                    var eSt = training.Enrollment.Where( e => e.Id == enr.Id).FirstOrDefault();
                    if( eSt != null){
                        enr.eStatus = eSt.eStatus;
                        enr.attended = eSt.attended;
                    } 
                }
                context.SaveChanges();
                this.Log(training,"Training", "Posted Attendance.", "Training"
                );
                return new OkObjectResult(training);
            }else{
                this.Log( training ,"Training", "Not Found Training in an posting attendance attempt.", "Training", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpGet("getservices/{limit}/{notConverted?}/{order?}")]
        public async Task<IActionResult> GetInServiceTrainings(int limit, Boolean notConverted = true, string order = "ASC"){
            IOrderedQueryable<zInServiceTrainingCatalog> services;
            if( notConverted ){
                List<string> converted = await context.Training.Where( t => t.tID != null).Select( t => t.tID).ToListAsync();
                if( order == "ASC"){
                    services = _reportingContext.zInServiceTrainingCatalog.Where( s => !converted.Contains( s.tID )).OrderBy(a => a.TrainDateBegin);
                }else{
                    services = _reportingContext.zInServiceTrainingCatalog.Where( s => !converted.Contains( s.tID )).OrderByDescending(a => a.TrainDateBegin);
                }
            }else{
                if( order == "ASC"){
                    services = _reportingContext.zInServiceTrainingCatalog.OrderBy(a => a.TrainDateBegin);
                }else{
                    services = _reportingContext.zInServiceTrainingCatalog.OrderByDescending(a => a.TrainDateBegin);
                }
            }
            var sc = await services.Take(limit).ToListAsync();
            return new OkObjectResult(sc);
        }
        

        [HttpGet("proposalsawaiting")]
        public async Task<IActionResult> ProposalsAwaiting(){
            var proposals = await context
                                .Training.Where( t => t.tStatus == "P")
                                .Include( t => t.submittedBy)
                                    .ThenInclude( s => s.PersonalProfile)
                                .ToListAsync();
            return new OkObjectResult(proposals);
        }

        [HttpGet("trainingsbystatus/{year}/{status}")]
        public async Task<IActionResult> TrainingsByStatus(int year, string status="A"){
            var trainings = await context
                                .Training.Where( t => t.tStatus == status && t.Start.Year == year )
                                .Include( t => t.submittedBy)
                                    .ThenInclude( s => s.PersonalProfile)
                                .Include( t => t.iHour)
                                .Include( t => t.Enrollment)
                                .ToListAsync();
            foreach( var tr in trainings){
                tr.Organizer = null;
                tr.approvedBy = null;
                if(tr.submittedBy != null){
                   // tr.submittedBy.ApprovedTrainings = null;
                    //tr.submittedBy.SubmittedTrainins = null;
                }
                foreach( var enr in tr.Enrollment){
                    enr.Training = null;
                }
            }
            return new OkObjectResult(trainings);
        }

        [HttpGet("userswithtrainings/{year}")]
        public async Task<IActionResult> UsersWithTrainings(int year){
            var users = await context
                                .TrainingEnrollment
                                .Where( t => t.Training.Start.Year == year && t.Training.tStatus=="A")
                                .GroupBy( e => e.Attendie)
                                .Select( s => s.Key)
                                .ToListAsync();
            var fullUsers = new List<KersUser>();
            foreach( var user in users){
                var fullUser = await context.KersUser
                                    .Where( u => u.Id == user.Id)
                                    .Include( u => u.PersonalProfile)
                                    .FirstOrDefaultAsync();
                if( fullUser != null ) fullUsers.Add(fullUser);
            }
            return new OkObjectResult(fullUsers.OrderBy( u => u.PersonalProfile.FirstName));
        }


        [HttpGet("RegisterWindows")]
        public async Task<IActionResult> RegisterWindows(){
            var winds = await context.TainingRegisterWindow.ToListAsync();
            return new OkObjectResult(winds);
        }
        [HttpGet("InstructionalHours")]
        public async Task<IActionResult> InstructionalHours(){
            var hours = await context.TainingInstructionalHour.ToListAsync();
            return new OkObjectResult(hours);
        }
        [HttpGet("CancelEnrollmentWindows")]
        public async Task<IActionResult> CancelEnrollmentWindows(){
            var winds = await context.TrainingCancelEnrollmentWindow.ToListAsync();
            return new OkObjectResult(winds);
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
            
            var trainings = from i in _context.Training select i;
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
            if( status == "published"){
                trainings = trainings.Where( i => i.tStatus == "A");
            }
            if(withseats){
                trainings = trainings.Where( i => i.seatLimit == null || i.seatLimit > i.Enrollment.Where(e => e.eStatus == "E").Count());
            }
            trainings = trainings
                            .Include( t => t.submittedBy).ThenInclude( u => u.PersonalProfile);
            if(attendance){
                trainings = trainings.Include(t => t.Enrollment).ThenInclude( e => e.Attendie).ThenInclude( a => a.RprtngProfile).ThenInclude( r => r.PlanningUnit);
            }
                            
            IOrderedQueryable result;
            if(order == "asc"){
                result = trainings.OrderByDescending(t => t.Start);
            }else if( order == "alph"){
                result = trainings.OrderBy(t => t.Subject);
            }else{
                result = trainings.OrderBy(t => t.Start);
            }

            return new OkObjectResult(result);
        }

        [HttpGet("byuser/{id?}/{year?}")]
        public async Task<IActionResult> TrainingsByUser( int id = 0, int year = 0 ){
            KersUser user;
            if( id == 0 ){
                user = CurrentUser();
                id = user.Id;
            }
            if( year == 0){
                year = DateTime.Now.Year;
            }
            var trainings = from training in context.Training
                from enfolment in training.Enrollment
                where enfolment.AttendieId == id
                where training.Start.Year == year
                select training;
            trainings = trainings.Include( t => t.Enrollment).Include(t => t.iHour);
            var tnngs = await trainings.ToListAsync();
            return new OkObjectResult(tnngs);
        }


 */
    }
}