namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zInServiceTrainingEnrollment")]
    public partial class zInServiceTrainingEnrollment
    {
        [Key]
        public int rID { get; set; }

        public DateTime? rDT { get; set; }

        [StringLength(8)]
        public string rPersonID { get; set; }

        [StringLength(3)]
        public string puid { get; set; }

        [StringLength(50)]
        public string planningUnitID { get; set; }

        [StringLength(8)]
        public string personID { get; set; }

        [StringLength(300)]
        public string personName { get; set; }

        [StringLength(25)]
        public string tID { get; set; }

        [StringLength(25)]
        public string eStatus { get; set; }

        public DateTime? enrolledDate { get; set; }

        public DateTime? cancelledDate { get; set; }

        public bool? attended { get; set; }

        public bool? evaluationMessageSent { get; set; }
    }
}
