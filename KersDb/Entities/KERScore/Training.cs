namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    public class Training : ExtensionEvent
    {
        public int classicInServiceTrainingId { get; set; }
        public KersUser submittedBy {get;set;}
        public int? submittedById {get;set;}
        public KersUser approvedBy {get;set;}
        public DateTime? approvedDate {get;set;}

        [StringLength(25)]
        public string tID { get; set; }

        [StringLength(25)]
        public string tStatus { get; set; }

        public DateTime? sessionCancelledDate { get; set; }

        [StringLength(8)]
        public string TrainDateBegin { get; set; }

        [StringLength(8)]
        public string TrainDateEnd { get; set; }

        public int? RegisterCutoffDaysId { get; set; }
        public TainingRegisterWindow RegisterCutoffDays { get; set; }

        public int? CancelCutoffDaysId { get; set; }
        public TrainingCancelEnrollmentWindow CancelCutoffDays { get; set; }

        public int? iHourId { get; set; }
        public TainingInstructionalHour iHour { get; set; }

        public int? seatLimit { get; set; }

        [StringLength(300)]
        public string tTime { get; set; }

        [StringLength(300)]
        public string day1 { get; set; }

        [StringLength(300)]
        public string day2 { get; set; }

        [StringLength(300)]
        public string day3 { get; set; }

        [StringLength(300)]
        public string day4 { get; set; }

        public string tAudience { get; set; }
        public List<TrainingEnrollment> Enrollment {get; set; }
        public List<TrainingSession> TrainingSession {get;set;}

        [StringLength(200)]
        public string qualtricsSurveyID { get; set; }

        public string evaluationLink { get; set; }
        //public List<TrainingSurveyResult> SurveyResults {get;set;}


    }
}