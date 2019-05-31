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

        KERScoreContext context;
        KERSmainContext mainContext;
        KERSreportingContext reportingContext;
        public TrainingRepository(
            KERScoreContext context,
            KERSreportingContext reportingContext,
            KERSmainContext mainContext 
            )
            
        { 
            this.reportingContext = reportingContext;
            this.context = context;
            this.mainContext = mainContext;
        }

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
            var user = this.context.KersUser.Where( u => u.RprtngProfile.PersonId == id).FirstOrDefault();
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
                enrlmnt.Add( old2newEnrolment( enr ) );
            }

            return enrlmnt;
        }

        private TrainingEnrollment old2newEnrolment( zInServiceTrainingEnrollment old ){
            var enrolment = new TrainingEnrollment();

            enrolment.rDT = old.rDT;
            enrolment.puid = old.puid;
            enrolment.Attendie = context.KersUser
                                    .Where( r => r.RprtngProfile.PersonId == old.personID)
                                    .Include( u => u.RprtngProfile)
                                    .FirstOrDefault();
            if(enrolment.Attendie != null ){
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





    }
}