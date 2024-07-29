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

using System.IO;
using CsvHelper;
using Kers.Models.Entities.UKCAReporting;
using Microsoft.EntityFrameworkCore;

namespace Kers.Models.Repositories
{
    public class TrainingRepository : ITrainingRepository
    {

        KERScoreContext context, _coreContext;
        KERSmainContext mainContext;
        KERSreportingContext reportingContext;
        IMessageRepository messageRepo;
        public TrainingRepository(
            KERScoreContext context,
            KERSreportingContext reportingContext,
            KERSmainContext mainContext,
            IMessageRepository messageRepo
            )
            
        { 
            this.reportingContext = reportingContext;
            this.context = this._coreContext = context;
            this.mainContext = mainContext;
            this.messageRepo = messageRepo;
        }



        /***********************************************/
        // Reminders
        /***********************************************/

        public List<Training> Set3DaysReminders(){
            List<Training> trainings = this.context.Training.Where( t =>  t.tStatus == "A" && t.Start.Year > 2017)
                                                    .Include( t => t.TrainingSession)
                                                    .Include(t => t.Enrollment).ThenInclude( e => e.Attendie).ToList();
            trainings = trainings.Where(  t => t.Start.AddDays(-3).ToString("MMddyyyy") == DateTimeOffset.Now.ToString("MMddyyyy")).ToList();
            this.ScheduleReminders("3DAYSREMINDER", trainings);
            return trainings;
        }

        public List<Training> Set7DaysReminders(){
            List<Training> trainings = this.context.Training.Where( t =>  t.tStatus == "A"  && t.Start.Year > 2017)
                                                    .Include( t => t.TrainingSession)
                                                    .Include(t => t.Enrollment).ThenInclude( e => e.Attendie).ToList();
            trainings = trainings.Where( t => t.Start.AddDays(-7).ToString("MMddyyyy") == DateTimeOffset.Now.ToString("MMddyyyy")).ToList();
            this.ScheduleReminders("7DAYSREMINDER", trainings);
            return trainings;
        }

        public List<Training> AwaitingActionReminders(){
            List<Training> trainings = this.context.Training
                                            .Where( t => t.tStatus == "P")
                                            .ToList();
            if(trainings.Count() > 0){

                var template = this.context.MessageTemplate.Where( t => t.Code == "AWAITINGACTION").FirstOrDefault();
                if(template != null){
                    var message = new Message();
                    message.Subject = string.Format(template.Subject, trainings.Count());
                    message.BodyHtml = string.Format(template.BodyHtml,  trainings.Count());
                    message.BodyText = string.Format(template.BodyText,  trainings.Count());
                    message.FromEmail = "agpsd@lsv.uky.edu";
                    message.ToEmail = "agpsd@lsv.uky.edu";
                    this.context.Message.Add(message);
                    this.context.SaveChanges();
                }


            }
            
            return trainings;
        }

        public List<Training> PostAttendanceReminders(){
            List<Training> trainings = this.context.Training
                                            .Where( t => 
                                                        t.tStatus == "A" 
                                                    )
                                            .Include( t => t.TrainingSession)
                                            .ToList();
            trainings = trainings.Where( t => t.Start.AddDays( 1 ).ToString("yyyyMMdd") == DateTimeOffset.Now.ToString("yyyyMMdd")).ToList();
            if(trainings.Count() > 0){

                var template = this.context.MessageTemplate.Where( t => t.Code == "POSTATTENDANCE").FirstOrDefault();
                
                if(template != null){
                    foreach( var training in trainings){

                        var valArray = this.valsToArray(training);
                        var message = new Message();
                        message.Subject = string.Format(template.Subject, valArray);
                        message.BodyHtml = string.Format(template.BodyHtml, valArray);
                        message.BodyText = string.Format(template.BodyText, valArray);
                        message.FromEmail = "agpsd@lsv.uky.edu";
                        message.ToId = training.submittedById;
                        this.context.Message.Add(message);


                    }
                    
                    this.context.SaveChanges();
                }


            }
            
            return trainings;
        }

        public List<TrainingEnrollment> SetEvaluationReminders(){
            List<TrainingEnrollment> trainings = this.context.TrainingEnrollment
                        .Where( t => t.evaluationMessageSent == false && t.attended == true)
                        .Include( t => t.Training)
                        .Include(t => t.Attendie).ThenInclude( e => e.RprtngProfile)
                        .ToList();
            var template = this.context.MessageTemplate.Where( t => t.Code == "SURVEY").FirstOrDefault();
            if( template != null && trainings.Count() > 0 ){
                foreach( var enr in trainings){
                    var message = new Message();
                    message.Subject = string.Format(template.Subject, enr.Training.Subject);
                    message.BodyHtml = string.Format(template.BodyHtml, enr.Training.Subject, enr.Training.Start.ToString( "MM/dd/yyyy" ), enr.Training.Id);
                    message.BodyText = string.Format(template.BodyText, enr.Training.Subject, enr.Training.Start.ToString( "MM/dd/yyyy" ), enr.Training.Id);
                    message.FromId = enr.Training.submittedById;
                    message.ToId = enr.AttendieId;
                    this.context.Message.Add(message);
                    enr.evaluationMessageSent = true;
                }
                this.context.SaveChanges();
            }
            return trainings;
        }

        public List<Training> RoosterReminders(){
            List<Training> trainings = this.context.Training
                        .Where( t => 
                                t.tStatus == "A"  && t.Start.Year > 2017
                            )
                        .Include( t => t.Enrollment)
                                .ThenInclude( e => e.Attendie)
                                .ThenInclude( a => a.RprtngProfile)
                                .ThenInclude(r => r.PlanningUnit)
                        .Include( t => t.TrainingSession)
                        .ToList();
            trainings = trainings.Where( t =>  t.Start.AddDays( - (t.CancelCutoffDays == null ? 1 : t.CancelCutoffDays.cancelDaysVal) ).ToString("yyyyMMdd") 
                                == 
                                DateTimeOffset.Now.ToString("yyyyMMdd")
                            ).ToList();
            var template = this.context.MessageTemplate.Where( t => t.Code == "ROSTER").FirstOrDefault();
            if( template != null && trainings.Count() > 0 ){
                foreach( var training in trainings){
                    var valArray = valsToArray( training );
                    var message = new Message();
                    message.Subject = string.Format(template.Subject, training.Subject);
                    message.BodyHtml = string.Format(template.BodyHtml, valArray);
                    message.BodyText = string.Format(template.BodyText, valArray);
                    message.FromEmail = "agpsd@lsv.uky.edu";
                    message.ToId = training.submittedById;
                    this.context.Message.Add(message);
                }
                this.context.SaveChanges();
            }
            return trainings;
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
        /***********************************************/
        private string[] valsToArray(Training training){
            var rstr = "";
            if(training.Enrollment != null && training.Enrollment.Count() > 0){
                if( training.Enrollment.First().Attendie != null && training.Enrollment.First().Attendie.RprtngProfile != null){
                    foreach( var enr in training.Enrollment.Where(e => e.eStatus == "E").OrderBy( f => f.Attendie.RprtngProfile.Name)){
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
                                    ": </td><td>" + session.Start.ToString("MM/dd/yy") + OffsetToTimeString(session.Start) + " - " + 
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


        public void ScheduleReminders(string type, List<Training> trainings){
            foreach( var training in trainings){
                foreach( var enrolment in training.Enrollment.Where(e => e.eStatus == "E")){
                    this.messageRepo.ScheduleTrainingMessage(type, training, enrolment.Attendie);
                }
            }
        }

        /***********************************************/
        // Reports
        /***********************************************/

        public List<TrainingEnrollment> trainingsPerPersonPerYear( int userId, int year){
            var trainings = context.TrainingEnrollment
                            .Where( e => e.Training.Start.Year == year && e.Training.tStatus == "A" && e.AttendieId == userId)
                            .Include( e => e.Training).ThenInclude( t => t.iHour)
                            .OrderBy( e => e.Training.Start)
                            .ToList();
            return trainings;
        }

        /***********************************************/
        // Import Trainings from the reporting repo
        /***********************************************/

        public List<zInServiceTrainingCatalog>  csv2list(string fileUrl = "database/trainingsData.csv"){
            //ViewData["trainings"] = this.reportingContext.zInServiceTrainingCatalog.Take(10).ToList();
            var records = new List<zInServiceTrainingCatalog>();
            using (var reader = new StreamReader(fileUrl))
            using (var csv = new CsvReader(reader))
            {    
                var newRecords = new List<Training>();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    DateTime date;
                    DateTime.TryParse(csv.GetField("rDT"), out date);
                    
                    DateTime submittedDate;
                    DateTime.TryParse(csv.GetField("submittedDate"), out submittedDate);

                    DateTime approvedDate;
                    DateTime.TryParse(csv.GetField("approvedDate"), out approvedDate);

                    DateTime sessionCancelledDate;
                    DateTime.TryParse(csv.GetField("approvedDate"), out sessionCancelledDate);
                    

                    var record = new zInServiceTrainingCatalog
                    {
                        rID = csv.GetField<int>("rID"),
                        rDT = date,
                        submittedDate = submittedDate,
                        submittedByPersonID = csv.GetField("submittedByPersonID"),
                        submittedByPersonName = csv.GetField("submittedByPersonName"),
                        approvedDate = approvedDate,
                        approvedByPersonID = csv.GetField("approvedByPersonID"),
                        approvedByPersonName = csv.GetField("approvedByPersonName"),
                        tID = csv.GetField("tID"),
                        tStatus = csv.GetField("tStatus"),
                        sessionCancelledDate = sessionCancelledDate,
                        TrainDateBegin = csv.GetField("TrainDateBegin"),
                        TrainDateEnd = csv.GetField("TrainDateEnd"),
                        RegisterCutoffDays = csv.GetField<int>("RegisterCutoffDays"),
                        CancelCutoffDays = csv.GetField<int>("CancelCutoffDays"),
                        iHours = csv.GetField("iHours") == "NULL" ? 0 : csv.GetField<int>("iHours"),
                        seatLimit = csv.GetField("seatLimit") == "NULL" ? 0 : csv.GetField<int>("seatLimit"),
                        tTitle = csv.GetField("tTitle"),
                        tLocation = csv.GetField("tLocation"),
                        tTime = csv.GetField("tTime"),
                        day1 = csv.GetField("day1"),
                        day2 = csv.GetField("day2"),
                        day3 = csv.GetField("day3"),
                        day4 = csv.GetField("day4"),
                        tContact = csv.GetField("tContact"),
                        tDescription = csv.GetField("tDescription"),
                        tAudience = csv.GetField("tAudience"),
                        qualtricsSurveyID = csv.GetField("qualtricsSurveyID"),
                        evaluationLink = csv.GetField("evaluationLink"),
                    };
                    records.Add(record);
                }
            }
            return records;
        }


        public List<Training> InServicesToTrainings(List<zInServiceTrainingCatalog> services){
            var trainings = new List<Training>();

            foreach( var service in services ){
                trainings.Add(ServiceToTraining(service));
            }
            return trainings;
        }

        public Training ServiceToTraining( zInServiceTrainingCatalog service){
            var training = new Training();
            
            training.classicInServiceTrainingId = service.rID;
            training.submittedBy = training.Organizer = this.userByPersonId(service.submittedByPersonID);
            training.approvedBy = this.userByPersonId( service.approvedByPersonID);
            if(training.submittedBy == null){
                training.submittedById = training.OrganizerId = 4;
            }
            training.approvedDate = service.approvedDate;
            training.tID = service.tID;
            training.tStatus = service.tStatus;
            training.IsCancelled = ( training.tStatus == "C" ? true : false);
            training.IsAllDay = false;
            training.sessionCancelledDate = service.sessionCancelledDate;
            training.Body = service.tDescription;
            training.BodyPreview = service.tDescription;
            training.Subject = service.tTitle;
            training.TrainDateBegin = service.TrainDateBegin;
            if(service.TrainDateBegin != null && service.TrainDateBegin != "NULL" && service.TrainDateBegin != "null"){
                training.Start = offsetFromString(service.TrainDateBegin);
            }else{
                training.Start = DateTimeOffset.Now;
            }
            if(service.TrainDateEnd != null && service.TrainDateEnd != "NULL" && service.TrainDateEnd != "null"){
                var end = offsetFromString( service.TrainDateEnd );
                if( end > DateTimeOffset.Now.AddYears(-100)){
                    training.TrainDateEnd = service.TrainDateEnd;
                    training.End = end;
                }else{
                    training.End = null;
                }      
            }else{
                training.End = null;
            }
            if(service.RegisterCutoffDays != null){
               training.RegisterCutoffDays = context.TainingRegisterWindow.Where( a => a.registerDaysVal == service.RegisterCutoffDays.ToString() ).FirstOrDefault(); 
            }
            if( service.CancelCutoffDays != null){
                training.CancelCutoffDays = context.TrainingCancelEnrollmentWindow.Where( a => a.cancelDaysVal == service.CancelCutoffDays).FirstOrDefault();
            }
            if( service.iHours != null ){
                training.iHour = context.TainingInstructionalHour.Where( a => a.iHourValue == service.iHours).FirstOrDefault();
            }
            if( service.seatLimit != null){
                training.seatLimit = service.seatLimit;
            }
            training.Type = ExtensionEventType.SingleInstance;
            training.tTime = service.tTime;
            training.day1 = service.day1;
            if( service.day2 != null && service.day2 != "NULL") training.day2 = service.day2;
            if( service.day3 != null && service.day3 != "NULL") training.day3 = service.day3;
            if( service.day4 != null && service.day4 != "NULL") training.day4 = service.day4;
            training.tContact = service.tContact;
            training.tAudience = service.tAudience;
            training.tLocation = service.tLocation;
            training.CreatedDateTime = service.rDT;
            training.LastModifiedDateTime = service.rDT;
            training.Enrollment = this.GetEnrollments( service.tID);
            training.Attendees = new List<KersUser>();
            foreach( var enrlm in training.Enrollment){
                if(enrlm.Attendie != null){
                    training.Attendees.Add( enrlm.Attendie);
                }
            }
            training.qualtricsSurveyID = service.qualtricsSurveyID;
            training.evaluationLink = service.evaluationLink;

            
            return training;

        }

        public KersUser userByPersonId( string id ){
            var user = this.context.KersUser.Where( u => u.RprtngProfile.PersonId == id)
                                .Include( r => r.RprtngProfile)
                                .FirstOrDefault();
            if(user == null){
                zEmpRptProfile profile = this.mainContext.zEmpRptProfiles.Where( u => u.personID == id).FirstOrDefault();
                if(profile != null && profile.linkBlueID != null) user = syncUserFromProfile(profile);
            }
            return user;
        }

        private DateTimeOffset offsetFromString(string dt){
            var year = "1900";
            var month = "1";
            var day = "1";
            if(dt != null && dt.Length > 7){
                year = dt.Substring(0, 4);
                month = dt.Substring(4, 2);
                day = dt.Substring(6, 2);
            }
            var offset = new DateTimeOffset (Int32.Parse(year), Int32.Parse(month), Int32.Parse(day), 8, 0, 0, new TimeSpan(-4, 0, 0));
            return offset;
        }

        private List<TrainingEnrollment> GetEnrollments( string tId){
            var enrlmnt = new List<TrainingEnrollment>();

            var old = this.reportingContext.zInServiceTrainingEnrollment.Where( a => a.tID == tId );
            foreach( var enr in old ){
                var newEnr = old2newEnrolment( enr );
                if(newEnr.Attendie != null) enrlmnt.Add(newEnr);
            }

            return enrlmnt;
        }

        private TrainingEnrollment old2newEnrolment( zInServiceTrainingEnrollment old ){
            var enrolment = new TrainingEnrollment();

            enrolment.rDT = old.rDT;
            enrolment.puid = old.puid;
            enrolment.Attendie = userByPersonId( old.personID);
            if(enrolment.Attendie != null && enrolment.Attendie.RprtngProfile != null ){
                enrolment.PlanningUnitId = enrolment.Attendie.RprtngProfile.PlanningUnitId;
            }
            enrolment.TrainingId = old.tID;
            enrolment.eStatus = old.eStatus;
            enrolment.enrolledDate = old.enrolledDate;
            enrolment.cancelledDate = old.cancelledDate;
            enrolment.attended = old.attended;
            enrolment.evaluationMessageSent = old.evaluationMessageSent;

            return enrolment;
        }







        /*****************************************/
        // Copied from KersUserService for training migration purpose


        public KersUser syncUserFromProfile(zEmpRptProfile profile){

            var user = _coreContext.
                                KersUser.
                                Where(u => u.classicReportingProfileId == profile.Id).
                                Include(u => u.Roles).
                                Include(u => u.PersonalProfile).
                                Include(u => u.RprtngProfile).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.GeneralLocation).
                                Include(u => u.ExtensionPosition).
                                Include(u=> u.Specialties).ThenInclude(s=>s.Specialty).
                                FirstOrDefault();
            if(user == null){
                user = new KersUser();
                user.Created = DateTime.Now;
                _coreContext.Add(user);
            }
            user.classicReportingProfileId = profile.Id;
            if(user.PersonalProfile == null){
                user.PersonalProfile = new PersonalProfile();
                this.populatePersonalProfileName(user.PersonalProfile, profile);
            }
            this.populatePosition(user, profile);
            this.addRoles(user, profile);
            this.populateSpecialties(user, profile);
            this.populateReportingProfile(user, profile);

            _coreContext.SaveChanges();

            return user;
        }

        public void syncSpecialties(KersUser user, zEmpRptProfile reporting){
            populateSpecialties(user, reporting);
        }

        private void populateReportingProfile(KersUser user, zEmpRptProfile reporting){
            if(user.RprtngProfile == null){
                user.RprtngProfile = new ReportingProfile();
            }
            user.RprtngProfile.LinkBlueId = reporting.linkBlueID;
            user.RprtngProfile.PersonId = reporting.personID;
            user.RprtngProfile.Name = reporting.personName;
            user.RprtngProfile.Email = reporting.emailDeliveryAddress;
            user.RprtngProfile.EmailAlias = reporting.emailUEA;
            user.RprtngProfile.enabled = reporting.enabled??false;
            this.populateGeneralLocation(user.RprtngProfile, reporting);
            this.populatePlanningUnit(user.RprtngProfile, reporting);
            this.populateInstitution(user.RprtngProfile, reporting);
            
        }

        private void populateSpecialties(KersUser user, zEmpRptProfile profile){
            if(user.Specialties == null){
                user.Specialties = new List<KersUserSpecialty>();
            }
            if(profile.prog4HYD??false){
                this.AddToSpecialty(user.Specialties, profile, user, "prog4HYD");
            }
            if(profile.progANR??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progANR");
            }
            if(profile.progFCS??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progFCS");
            }
            if(profile.progHORT??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progHORT");
            }
            if(profile.progNEP??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progNEP");
            }
            if(profile.progOther??false){
                this.AddToSpecialty(user.Specialties, profile, user, "progOther");
            }
        }

        private void AddToSpecialty(List<KersUserSpecialty> specialties, zEmpRptProfile profile, KersUser user, string code){
            var specialty = this._coreContext.Specialty.Where(r=>r.Code == code).FirstOrDefault();
            var isPresent = specialties.Where(s=>s.Specialty == specialty).FirstOrDefault();
            if(specialty != null && isPresent == null){
                var r = new KersUserSpecialty();
                r.Specialty = specialty;
                r.KersUserId = user.Id;
                specialties.Add(r);
            }
        }

        private void populateGeneralLocation(ReportingProfile rprtng, zEmpRptProfile profile){
            var loctn = _coreContext.GeneralLocation.Where(l=>l.Code == profile.locationID).FirstOrDefault();
            rprtng.GeneralLocation = loctn;
        }

        private void populatePlanningUnit(ReportingProfile rprtng, zEmpRptProfile profile){
            var unit = _coreContext.PlanningUnit.
                        Where(p=>p.Code == profile.planningUnitID).
                        FirstOrDefault();
            rprtng.PlanningUnit = unit;
        }
        private void populateInstitution(ReportingProfile rprtng, zEmpRptProfile profile){
            var inst = _coreContext.Institution.
                        Where(p=>p.Code == profile.instID).
                        FirstOrDefault();
            rprtng.Institution = inst;
        }

        private void populatePersonalProfileName(PersonalProfile personal, zEmpRptProfile reporting){
            var name = reporting.personName;

            char[] delimiterChars = { ',', ' ' };
            var splitName = name.Split(delimiterChars);
            if(splitName.Count() > 1){
                personal.FirstName = splitName[2];
                personal.LastName = splitName[0];
            }
        }
        private void populatePosition(KersUser user, zEmpRptProfile reporting){
            var position = _coreContext.ExtensionPosition.Where(p => p.Code == reporting.positionID).FirstOrDefault();
            if(position != null){
                user.ExtensionPosition = position;
            }else{
                if(user.ExtensionPosition == null){
                    position = _coreContext.ExtensionPosition.FirstOrDefault();
                    user.ExtensionPosition = position;
                }
            }
        }

        private void addRoles(KersUser user, zEmpRptProfile profile){
            List<zEmpProfileRole> roles;
            if(user.Roles == null){
                roles = new List<zEmpProfileRole>();
            }else{
                roles = user.Roles;
            }
            if((bool)profile.isCesInServiceAdmin){
                AddToRoles(roles, profile, user, "CESADM");
            }
            if((bool)profile.isCesInServiceTrainer){
                AddToRoles(roles, profile, user, "SRVCTRNR");
            }
            if((bool)profile.isCesInServiceAdmin){
                AddToRoles(roles, profile, user, "SRVCADM");
            }
            if((bool)profile.isDD){
                AddToRoles(roles, profile, user, "DD");
            }
            user.Roles = roles;
        }

        private void AddToRoles(List<zEmpProfileRole> roles, zEmpRptProfile profile, KersUser user, string code){
            var role = this._coreContext.zEmpRptRoleType.Where(r=>r.shortTitle == code).FirstOrDefault();
            var exists = roles.Where(r => r.zEmpRoleType == role).FirstOrDefault();
            if(role != null && exists == null){
                var r = new zEmpProfileRole();
                r.zEmpRoleType = role;
                r.User = user;
                roles.Add(r);
            }
        }











    }
}