using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    [Table("zzSpeedSort")]
    public partial class zzSpeedSort
    {
        [Key]
        [StringLength(4)]
        public string ssID { get; set; }


        [StringLength(75)]
        public string Location { get; set; }
    }
}
