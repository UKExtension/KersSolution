namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zInServiceTrainingCatalog")]
    public partial class zInServiceTrainingCatalog
    {
        [Key]
        public int rID { get; set; }

        public DateTime? rDT { get; set; }

        public DateTime? submittedDate { get; set; }

        [StringLength(8)]
        public string submittedByPersonID { get; set; }

        [StringLength(125)]
        public string submittedByPersonName { get; set; }

        public DateTime? approvedDate { get; set; }

        [StringLength(8)]
        public string approvedByPersonID { get; set; }

        [StringLength(125)]
        public string approvedByPersonName { get; set; }

        [StringLength(25)]
        public string tID { get; set; }

        [StringLength(25)]
        public string tStatus { get; set; }

        public DateTime? sessionCancelledDate { get; set; }

        [StringLength(8)]
        public string TrainDateBegin { get; set; }

        [StringLength(8)]
        public string TrainDateEnd { get; set; }

        public int? RegisterCutoffDays { get; set; }

        public int? CancelCutoffDays { get; set; }

        public int? iHours { get; set; }

        public int? seatLimit { get; set; }

        [StringLength(125)]
        public string tTitle { get; set; }

        [StringLength(300)]
        public string tLocation { get; set; }

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

        [StringLength(200)]
        public string tContact { get; set; }

        public string tDescription { get; set; }

        public string tAudience { get; set; }

        [StringLength(200)]
        public string qualtricsSurveyID { get; set; }

        public string evaluationLink { get; set; }
    }
}
