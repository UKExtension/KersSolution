namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zzInServiceCancelEnrollmentWindow")]
    public partial class zzInServiceCancelEnrollmentWindow
    {
        [Key]
        [Column(Order = 0)]
        public int rID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string cancelDaysVal { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(75)]
        public string cancelDaysTxt { get; set; }
    }
}
