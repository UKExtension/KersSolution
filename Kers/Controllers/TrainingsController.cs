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
    public class TrainingsController : ExtensionEventController
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
        public TrainingsController( 
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



    

        [HttpDelete("deletetraining/{id}")]
        [Authorize]
        public IActionResult DeleteTraining( int id ){
            var entity = context.Training
                            .Where(t => t.Id == id)
                            .Include( t => t.Enrollment)
                            .Include( t => t.TrainingSession)
                            .FirstOrDefault();
            
            
            if(entity != null){
                
                context.TrainingEnrollment.RemoveRange( entity.Enrollment );
                context.RemoveRange( entity.TrainingSession );

                context.Training.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"ExtensionEvent", "ExtensionEvent Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ExtensionEvent", "Not Found ExtensionEvent in a delete attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var training = await context.Training
                                    .Where( t => t.Id == id)
                                    .Include( t => t.Enrollment)
                                            .ThenInclude( e => e.Attendie)
                                            .ThenInclude( a => a.RprtngProfile)
                                                .ThenInclude( r => r.PlanningUnit)
                                    .Include( t => t.Enrollment)
                                            .ThenInclude( e => e.Attendie)
                                            .ThenInclude( a => a.Specialties)
                                                .ThenInclude( s => s.Specialty)
                                    .Include( t => t.iHour)
                                    .Include( t => t.RegisterCutoffDays)
                                    .Include( t => t.CancelCutoffDays)
                                    .Include( t => t.TrainingSession)
                                    .Include( t => t.SurveyResults)
                                    .FirstOrDefaultAsync();
            foreach( var enr in  training.Enrollment){
                enr.Attendie.RprtngProfile.PlanningUnit.GeoFeature = null;
            }
            if( training != null){
                return new OkObjectResult(this.ToTimezone( training ));     
            }else{
                this.Log( id ,"Training", "Not Found Training with this id.", "Training", "Error");
                return new StatusCodeResult(500);
            }           
        }

        [HttpGet]
        [Route("getByClassicId/{id}")]
        public async Task<IActionResult> GetByClassicId(int id)
        {
            var training = await context.Training
                                    .Where( t => t.tID == id.ToString())
                                    .Include( t => t.Enrollment)
                                    .Include( t => t.iHour)
                                    .Include( t => t.RegisterCutoffDays)
                                    .Include( t => t.CancelCutoffDays)
                                    .FirstOrDefaultAsync();
            if( training != null){
                return new OkObjectResult(training);
            }else{
                this.Log( id ,"Training", "Not Found Training with this id.", "Training", "Error");
                return new StatusCodeResult(500);
            }           
        }



        [HttpGet]
        [Route("rangetrainings/{skip?}/{take?}/{order?}/{type?}")]
        public override IActionResult GetRange(int skip = 0, int take = 10, string order = "start", string type = "Training")
        {
            IQueryable<Training> query = _context.Training.Where( t => t.End != null );
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             
            query = query.Skip(skip).Take(take);
            query = query
                        .Include( e => e.submittedBy)
                        .ThenInclude( o => o.PersonalProfile);
            var list = query.ToList();
            return new OkObjectResult(list);
        }

        [HttpGet("perPeriod/{start}/{end}/{order?}/{type?}")]
        [Authorize]
        public override IActionResult PerPeriod(DateTime start, DateTime end, string order = "start", string type = "Training" ){
            IQueryable<Training> query = _context.Training.Where( t => t.Start > start && t.Start < end);
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
            query = query
                        .Include( e => e.submittedBy)
                        .ThenInclude( o => o.PersonalProfile);

            List<Training> list = query.ToList();

            return new OkObjectResult(list);
        }
        

        [HttpPost("addsurvey")]
        [Authorize]
        public IActionResult AddSurvey( [FromBody] TrainingSurveyResult survey){
            if(survey != null){
                if(survey.UserId == null){
                    var user = this.CurrentUser();
                    survey.UserId = user.Id;
                }
                
                survey.Created = DateTime.Now;
                context.Add(survey); 
                context.SaveChanges();
                this.Log(survey,"TrainingSurveyResult", "Training Survey Results saved.");

                return new OkObjectResult(survey);
            }else{
                this.Log( survey ,"TrainingSurveyResult", "Error in adding training survey attempt.", "TrainingSurveyResult", "Error");
                return new StatusCodeResult(500);
            }
        }

  


        [HttpPost("addtraining")]
        [Authorize]
        public IActionResult AddTraining( [FromBody] TrainingWithTimezone training){
            if(training != null){
                var user = this.CurrentUser();
                Training trn = new Training();
                trn.TrainingSession = new List<TrainingSession>();
                foreach( var s in training.TrainingSessionWithTimes){
                    trn.TrainingSession.Add( s.GetSession(training.Etimezone));
                }
                var sortedSessions = trn.TrainingSession.OrderBy( s => s.Start);
                trn.Start = sortedSessions.First().Start;
                trn.End = sortedSessions.Last().End;
                trn.Subject = training.Subject;
                trn.Body = training.Body;
                trn.tLocation = training.tLocation;
                trn.tContact = training.tContact;
                trn.day1 = training.day1;
                trn.day2 = training.day2;
                trn.day3 = training.day3;
                trn.day4 = training.day4;
                trn.tAudience = training.tAudience;
                trn.TrainDateBegin = training.Start.ToString("yyyyMMdd");
                trn.TrainDateEnd = training.End?.ToString("yyyyMMdd");
                trn.LastModifiedDateTime = DateTime.Now;
                trn.tStatus = "P";
                trn.LastModifiedDateTime = DateTime.Now;
                trn.iHourId = training.iHourId;
                trn.CancelCutoffDaysId = training.CancelCutoffDaysId;
                trn.RegisterCutoffDaysId = training.RegisterCutoffDaysId;
                trn.seatLimit = training.seatLimit;
                trn.submittedBy = user;
                trn.CreatedDateTime = DateTime.Now;
                trn.BodyPreview = training.Body;
                trn.Organizer = trn.submittedBy;

                context.Add(trn); 
                context.SaveChanges();
                this.Log(trn,"Training", "Training Proposed.");
                return new OkObjectResult(trn);
            }else{
                this.Log( training ,"Training", "Error in adding training attempt.", "Training", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("enroll/{trainingId}")]
        [Authorize]
        public async Task<IActionResult> EnrollInTraining( int trainingId ){
            var training = await this.context.Training.Where( t => t.Id == trainingId)
                                        .Include( t => t.Enrollment)
                                        .Include( t => t.TrainingSession)
                                        .FirstOrDefaultAsync();
            if(training != null){
                var user = this.CurrentUser();
                if( !training.Enrollment.Where(e => e.AttendieId == user.Id).Any()){
                    var enrollment = new TrainingEnrollment();
                    enrollment.rDT = DateTime.Now;
                    enrollment.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                    enrollment.Attendie = user;
                    enrollment.TrainingId = trainingId.ToString();
                    if(training.seatLimit != null && training.seatLimit <= training.Enrollment.Count){
                        enrollment.eStatus = "W";
                    }else{
                        enrollment.eStatus = "E";
                        messageRepo.ScheduleTrainingMessage("ENROLLMENT", training, user);
                    }
                    enrollment.enrolledDate = enrollment.rDT;
                    training.Enrollment.Add(enrollment);
                    await context.SaveChangesAsync();
                    
                    this.Log(enrollment,"TrainingEnrollment", "Enrolled In Training.", "TrainingEnrollment");
                }else{
                    var enr = training.Enrollment.Where(e => e.AttendieId == user.Id).FirstOrDefault();
                    if(enr.eStatus == "W"){
                        enr.eStatus = "E";
                        messageRepo.ScheduleTrainingMessage("ENROLLMENT", training, user);
                        await context.SaveChangesAsync();
                        this.Log(enr,"TrainingEnrollment", "Enrolled In Training from the Waiting List.", "TrainingEnrollment");
                    }
                }
                
                return new OkObjectResult(training.Enrollment.Where(e => e.AttendieId == user.Id).FirstOrDefault());
            }else{
                this.Log( trainingId ,"TrainingEnrollment", "Error in training enrolment attempt.", "TrainingEnrollment", "Error");
                return new StatusCodeResult(500);
            }
        }
        [HttpPost("unenroll/{trainingId}")]
        [Authorize]
        public async Task<IActionResult> UnenrollFromTraining( int trainingId ){
            var training = await this.context.Training.Where( t => t.Id == trainingId)
                                        .Include( t => t.Enrollment)
                                        .Include( t => t.TrainingSession)
                                        .FirstOrDefaultAsync();
            if(training != null){
                var user = this.CurrentUser();
                var enrollment = training.Enrollment.Where(e => e.Attendie == user).FirstOrDefault();

                if( enrollment != null){
                    training.Enrollment.Remove(enrollment);
                    context.TrainingEnrollment.Remove(enrollment);
                    await context.SaveChangesAsync();
                    CheckTheWaitingList(training);
                    messageRepo.ScheduleTrainingMessage("CANCELENROLLMENT", training, user);
                    this.Log(enrollment,"TrainingEnrollment", "Cancelled Enrollment in Training.", "TrainingEnrollment");
                }
                
                return new OkObjectResult(training);
            }else{
                this.Log( trainingId ,"TrainingEnrollment", "Error in training enrolment cancelling attempt.", "TrainingEnrollment", "Error");
                return new StatusCodeResult(500);
            }
        }
        private async void CheckTheWaitingList(Training training){
            if(training.Enrollment.Where( a => a.eStatus == "W").Any()){
                if( training.Enrollment.Count < training.seatLimit){
                    var first = training.Enrollment.Where( a => a.eStatus == "W").OrderBy( a => a.enrolledDate).FirstOrDefault();
                    first.eStatus = "E";
                    await context.SaveChangesAsync();
                    messageRepo.ScheduleTrainingMessage("TOENROLLED", training, first.Attendie);
                }
            }
        }


        [HttpPut("updatetraining/{id}")]
        [Authorize]
        public IActionResult UpdateTraoining( int id, [FromBody] Training training){
            var trn = context.Training.Where( t => t.Id == id)
                            .Include(t => t.submittedBy)
                            .Include( t => t.Enrollment)
                            .FirstOrDefault();
            if(training != null && trn != null ){
                var isMovedToCatalog = false;
                trn.Start = new DateTime(training.Start.Year, training.Start.Month, training.Start.Day, 14, 5, 6);
                if(training.End.HasValue){
                    trn.End = new DateTime(training.End.Value.Year, training.End.Value.Month, training.End.Value.Day, 14, 5, 6);
                }else{
                    trn.End = null;
                }
                
                trn.Subject = training.Subject;
                trn.Body = training.Body;
                trn.tLocation = training.tLocation;
                trn.tContact = training.tContact;
                trn.day1 = training.day1;
                trn.day2 = training.day2;
                trn.day3 = training.day3;
                trn.day4 = training.day4;
                trn.tAudience = training.tAudience;
                trn.IsCore = training.IsCore;
                trn.TrainDateBegin = training.Start.ToString("yyyyMMdd");
                trn.TrainDateEnd = training.End?.ToString("yyyyMMdd");
                trn.LastModifiedDateTime = DateTime.Now;
                if(trn.tStatus != "A" && training.tStatus == "A"){
                    var user = CurrentUser();
                    trn.approvedBy = user;
                    trn.approvedDate = DateTime.Now;
                }
                if(trn.tStatus == "P" && training.tStatus=="A"){
                    //Send email to the Organizer that the training is approved
                    isMovedToCatalog = true;
                }
                trn.tStatus = training.tStatus;
                trn.LastModifiedDateTime = DateTime.Now;
                trn.iHourId = training.iHourId;
                trn.CancelCutoffDaysId = training.CancelCutoffDaysId;
                trn.RegisterCutoffDaysId = training.RegisterCutoffDaysId;
                trn.seatLimit = training.seatLimit;
                context.SaveChanges();
                if(isMovedToCatalog){
                    messageRepo.ScheduleTrainingMessage("PROPOSALCOMFIRMED",trn,trn.submittedBy);
                }
                
                this.Log(trn,"Training", "Training Updated.");
                return new OkObjectResult(trn);
            }else{
                this.Log( training ,"Training", "Not Found Training in an update attempt.", "Training", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpPut("updatesessionstraining/{id}")]
        [Authorize]
        public IActionResult UpdateTraining( int id, [FromBody] TrainingWithTimezone training){
            var trn = context.Training.Where( t => t.Id == id)
                            .Include(t => t.submittedBy)
                            .Include( t => t.Enrollment)
                            .Include( t => t.TrainingSession)
                            .FirstOrDefault();
            if(training != null && trn != null ){
                var isMovedToCatalog = false;
                trn.TrainingSession = new List<TrainingSession>();
                foreach( var s in training.TrainingSessionWithTimes){
                    trn.TrainingSession.Add( s.GetSession(training.Etimezone));
                }
                var sortedSessions = trn.TrainingSession.OrderBy( s => s.Start);
                trn.Start = sortedSessions.First().Start;
                trn.End = sortedSessions.Last().End;
                trn.Subject = training.Subject;
                trn.Body = training.Body;
                trn.tLocation = training.tLocation;
                trn.tContact = training.tContact;
                trn.tAudience = training.tAudience;
                trn.TrainDateBegin = training.Start.ToString("yyyyMMdd");
                trn.TrainDateEnd = training.End?.ToString("yyyyMMdd");
                trn.LastModifiedDateTime = DateTime.Now;
                if(trn.tStatus != "A" && training.tStatus == "A"){
                    var user = CurrentUser();
                    trn.approvedBy = user;
                    trn.approvedDate = DateTime.Now;
                }
                if(trn.tStatus == "P" && training.tStatus=="A"){
                    //Send email to the Organizer that the training is approved
                    isMovedToCatalog = true;
                }
                trn.tStatus = training.tStatus;
                trn.iHourId = training.iHourId;
                trn.CancelCutoffDaysId = training.CancelCutoffDaysId;
                trn.RegisterCutoffDaysId = training.RegisterCutoffDaysId;
                trn.seatLimit = training.seatLimit;
                trn.BodyPreview = training.Body;
                trn.IsCore = training.IsCore;
                context.SaveChanges();
                if(isMovedToCatalog){
                    messageRepo.ScheduleTrainingMessage("PROPOSALCOMFIRMED",trn,trn.submittedBy);
                }
                this.Log(trn,"Training", "Training Updated.");
                return new OkObjectResult(this.ToTimezone(trn));
            }else{
                this.Log( training ,"Training", "Not Found Training in an update attempt.", "Training", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("postattendance/{id}")]
        [Authorize]
        public IActionResult PostAttendance( int id, [FromBody] Training training){
           

            var trn = context.Training
                                .Where( t => t.Id == id)
                                .Include( t => t.Enrollment)
                                .Include( t => t.SurveyResults)
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
        [HttpGet("migrate/{id}")]
        public async Task<IActionResult> MigrateInServiceTrainings(int id){
            try{
                if( !(await context.Training.Where( t => t.tID == id.ToString()).AnyAsync()) ){
                    var service = await this._reportingContext.zInServiceTrainingCatalog.Where( s => s.rID == id).FirstOrDefaultAsync();
                    if( service != null){
                        if( !this.context.Training.Where( t => t.tID == service.rID.ToString()).Any() ){
                            
                                var training = trainingRepo.ServiceToTraining(service);
                                this.context.Add(training);
                                await this.context.SaveChangesAsync();
                                return new OkObjectResult(training);
                            
                            
                        }
                    } 
                }
            }catch( Exception e ){
                this.Log( e.Message ,"Training", "Migration Error.", "Training", "Error");
                return new StatusCodeResult(500);
            }
            return new StatusCodeResult(500);
        }

        [HttpGet("proposalsawaiting")]
        public IActionResult ProposalsAwaiting(){
            var proposals = context
                                .Training.Where( t => t.tStatus == "P")
                                .Include( t => t.submittedBy)
                                    .ThenInclude( s => s.PersonalProfile)
                                .Include( t => t.TrainingSession)
                                .Include(t => t.TrainingSession)
                                .OrderBy(t => t.Start);
            
            return new OkObjectResult( ToWithTimezone(proposals) );
        }

        [HttpGet("trainingsbystatus/{year}/{status}")]
        public async Task<IActionResult> TrainingsByStatus(int year, string status="A"){
            var trainings = await context
                                .Training.Where( t => t.tStatus == status && t.Start.Year == year )
                                .Include( t => t.submittedBy)
                                    .ThenInclude( s => s.PersonalProfile)
                                .Include( t => t.iHour)
                                .Include( t => t.Enrollment)
                                .Include( t => t.TrainingSession)
                                .ToListAsync();
            List<TrainingWithTimezone> withTimezone = new List<TrainingWithTimezone>();
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
                withTimezone.Add( ToTimezone(tr));
            }
            return new OkObjectResult(withTimezone);
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
                                        [FromQuery] bool attendance,
                                        [FromQuery] bool admin
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
                            .Include( t => t.submittedBy).ThenInclude( u => u.PersonalProfile)
                            .Include( t => t.TrainingSession)
                            .Include( t => t.iHour);
            if(attendance){
                trainings = trainings
                        .Where(t => t.tStatus == "A" && t.Enrollment.Count() > 0 )
                        .Include( t => t.SurveyResults)
                        .Include(t => t.Enrollment)
                            .ThenInclude( e => e.Attendie)
                            .ThenInclude( a => a.RprtngProfile)
                            .ThenInclude( r => r.PlanningUnit);
            }else if( admin ){
                trainings = trainings.Include(t => t.Enrollment);
            }
                            
            IOrderedQueryable<Training> result;
            if(order == "asc"){
                result = trainings.OrderByDescending(t => t.Start);
            }else if( order == "alph"){
                result = trainings.OrderBy(t => t.Subject);
            }else{
                result = trainings.OrderBy(t => t.Start);
            }

            if(!attendance){
                return new OkObjectResult(this.ToWithTimezone(result));
            }

            return new OkObjectResult(result);
        }

        private List<TrainingWithTimezone> ToWithTimezone(IOrderedQueryable<Training>  trainings){
            var withTimezone = new List<TrainingWithTimezone>();
            
            foreach( var training in trainings){
                /* 
                var with = new TrainingWithTimezone( training );
                with.TrainingSessionWithTimes = new List<TrainingSessionWithTimes>();
                foreach( var sess in with.TrainingSession){
                    var sWithTime = new TrainingSessionWithTimes();
                    if(sess.Start.Offset.Hours == -5){
                        with.Etimezone = false;
                    }else{
                        with.Etimezone = true;
                    }
                    sWithTime.Starttime = sess.Start.Hour.ToString("D2") + ":" + sess.Start.Minute.ToString("D2");
                    sWithTime.Endtime = sess.End.Hour.ToString("D2") + ":" + sess.End.Minute.ToString("D2");
                    sWithTime.Date = sess.Start.DateTime;
                    sWithTime.Index = sess.Index;
                    sWithTime.Note = sess.Note;
                    with.TrainingSessionWithTimes.Add(sWithTime);
                } */
                withTimezone.Add(this.ToTimezone(training));
            }
            return withTimezone;
        }

        private TrainingWithTimezone ToTimezone(Training training){
            var with = new TrainingWithTimezone( training );
            with.TrainingSessionWithTimes = new List<TrainingSessionWithTimes>();
            foreach( var sess in with.TrainingSession){
                var sWithTime = new TrainingSessionWithTimes();
                if(sess.Start.Offset.Hours == -5){
                    with.Etimezone = false;
                }else{
                    with.Etimezone = true;
                }
                sWithTime.Starttime = sess.Start.Hour.ToString("D2") + ":" + sess.Start.Minute.ToString("D2");
                sWithTime.Endtime = sess.End.Hour.ToString("D2") + ":" + sess.End.Minute.ToString("D2");
                sWithTime.Date = sess.Start.DateTime;
                sWithTime.Index = sess.Index;
                sWithTime.Note = sess.Note;
                with.TrainingSessionWithTimes.Add(sWithTime);
            }
            return with;
        }

        [HttpGet("byuser/{id?}/{year?}/{start?}/{end?}")]
        public async Task<IActionResult> TrainingsByUser( int id = 0, int? year = null,  DateTime? start = null, DateTime? end = null){
            KersUser user;
            if( id == 0 ){
                user = CurrentUser();
                id = user.Id;
            }
            var trainings = from training in context.Training
                from enfolment in training.Enrollment
                where enfolment.AttendieId == id
                //where training.Start.Year == year
                select training;
            
            if(start == null){
                if( year == 0){
                    year = DateTime.Now.Year;
                }
                trainings = trainings.Where( t => t.Start.Year == year);
            }else{
                trainings = trainings.Where( t => t.Start > start && t.Start < end);
            }
            

            trainings = trainings.Include( t => t.Enrollment).Include(t => t.iHour)
            .Include( t => t.SurveyResults);
            var tnngs = await trainings.OrderBy(t => t.Start).ToListAsync();
            return new OkObjectResult(tnngs);
        }


        [HttpGet("userhours/{id}/{start}/{end}")]
        public async Task<IActionResult> HourssByUser( int id, DateTime start, DateTime end){
            KersUser user;
            if( id == 0 ){
                user = CurrentUser();
                id = user.Id;
            }
            var trainings = from training in context.Training
            from enfolment in training.Enrollment
            where enfolment.AttendieId == id && enfolment.attended == true
            select training;
        

            trainings = trainings.Where( t => t.Start > start && t.Start < end);
            

            trainings = trainings.Include(t => t.iHour);
            var hours = await trainings.SumAsync( t => t.iHour == null ? 0 : t.iHour.iHourValue );
            return new OkObjectResult(hours);
        }





        [HttpGet("proposedbyuser/{id?}/{year?}")]
        public async Task<IActionResult> ProposedTrainingsByUser( int id = 0, int year = 0 ){
            KersUser user;
            if( id == 0 ){
                user = CurrentUser();
                id = user.Id;
            }
            if( year == 0){
                year = DateTime.Now.Year;
            }
            var trainings = from training in context.Training
                
                where training.submittedById == id
                where training.Start.Year == year
                select training;
            trainings = trainings
                            .Include( t => t.Enrollment)
                                .ThenInclude( e => e.Attendie)
                                    .ThenInclude( a => a.RprtngProfile)
                                        .ThenInclude( u => u.PlanningUnit)
                            .Include(t => t.iHour)
                            .Include( t => t.SurveyResults);
            var tnngs = await trainings.ToListAsync();
            foreach( var tn in tnngs ){
                foreach( var enr in tn.Enrollment){
                    enr.PlanningUnit.GeoFeature = "";
                }
            }
            return new OkObjectResult(tnngs);
        }

        [HttpGet("upcomming/{id?}")]
        public async Task<IActionResult> UpcommingTrainings( int id = 0 ){
            KersUser user;
            if( id == 0 ){
                user = CurrentUser();
                id = user.Id;
            }
            var trainings = from training in context.Training
                from enfolment in training.Enrollment
                where enfolment.AttendieId == id
                where training.Start > DateTime.Now
                select training;
            trainings = trainings.Include( t => t.Enrollment).Include(t => t.iHour);
            var tnngs = await trainings.ToListAsync();
            return new OkObjectResult(tnngs);
        }

    }


    public class TrainingWithTimezone : Training{
        public bool Etimezone {get;set;}
        public List<TrainingSessionWithTimes> TrainingSessionWithTimes {get;set;}


        public TrainingWithTimezone(){
            
        }
        public TrainingWithTimezone( Training training){
            var props = typeof(Training).GetProperties().Where(p => !p.GetIndexParameters().Any());
            foreach (var prop in props)
            {
                if (prop.CanWrite)
                    prop.SetValue(this, prop.GetValue(training));
            }

        }
    }
    public class TrainingSessionWithTimes : TrainingSession{
        public DateTime Date {get;set;}
        public string Starttime {get;set;}
        public string Endtime {get;set;}

        public TrainingSession GetSession(Boolean IsItEastern = true){
            TrainingSession Session = new TrainingSession();
            Session.Index = this.Index;
            Session.IsCancelled = this.IsCancelled;
            Session.Note = this.Note;
            Session.Start = OffsetFromDate(this.Date, this.Starttime, IsItEastern);
            Session.End = OffsetFromDate(this.Date, this.Endtime, IsItEastern);
            return Session;
        }

        private DateTimeOffset OffsetFromDate( DateTime date, string time, Boolean IsItEastern = true){
            var hour = 8;
            var minutes = 30;
            var tm = time.Split(':');
            if(tm.Length > 1){
                hour = Int32.Parse(tm[0]);
                minutes = Int32.Parse(tm[1]);
            }
            var offset = new DateTimeOffset (date.Year, Date.Month, Date.Day, hour, minutes, 0, new TimeSpan(IsItEastern ? -4 : -5, 0, 0));
            return offset;
        }
    }
}