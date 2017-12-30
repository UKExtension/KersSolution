using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzHour
    {
        [Key]
        public int rID { get; set; }

        public int? hourValue { get; set; }

        [StringLength(100)]
        public string hourText { get; set; }
    }
}
