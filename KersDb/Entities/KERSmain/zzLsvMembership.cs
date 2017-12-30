using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    [Table("zzLsvMembership")]
    public partial class zzLsvMembership
    {
        [Key]
        public int rID { get; set; }

        public DateTime? rDT { get; set; }

        [StringLength(50)]
        public string subType { get; set; }

        [StringLength(200)]
        public string lsvName { get; set; }

        [StringLength(200)]
        public string personEmailAddress { get; set; }

        [StringLength(200)]
        public string personName { get; set; }
    }
}
