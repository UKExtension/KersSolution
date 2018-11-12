namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zInServiceTrainingEnrollmentWaitingList")]
    public partial class zInServiceTrainingEnrollmentWaitingList
    {
        [Key]
        public int rID { get; set; }

        [StringLength(25)]
        public string tID { get; set; }

        [StringLength(8)]
        public string personID { get; set; }

        [StringLength(300)]
        public string personName { get; set; }

        [StringLength(25)]
        public string wStatus { get; set; }

        public DateTime? waitingListDT { get; set; }

        public DateTime? enrolledDT { get; set; }

        public DateTime? enrolledEmailSentDT { get; set; }
    }
}
