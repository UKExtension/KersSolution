namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class zzCesEventTime
    {
        [Key]
        public int rID { get; set; }

        [StringLength(50)]
        public string timeValue { get; set; }

        [StringLength(50)]
        public string timeName { get; set; }
    }
}
