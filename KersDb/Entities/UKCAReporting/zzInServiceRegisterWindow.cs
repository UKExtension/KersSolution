namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zzInServiceRegisterWindow")]
    public partial class zzInServiceRegisterWindow
    {
        [Key]
        public int rID { get; set; }

        [StringLength(50)]
        public string registerDaysVal { get; set; }

        
        [StringLength(75)]
        public string registerDaysTxt { get; set; }
    }
}
