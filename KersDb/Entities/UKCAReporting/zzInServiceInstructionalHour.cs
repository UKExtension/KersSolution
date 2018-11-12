namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class zzInServiceInstructionalHour
    {
        [Key]
        public int rID { get; set; }

        [StringLength(25)]
        public string iHoursTxt { get; set; }
    }
}
