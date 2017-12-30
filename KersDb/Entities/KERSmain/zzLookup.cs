using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzLookup
    {
        [Key]
        public int rID { get; set; }

        public DateTime? rDT { get; set; }

        public int? ddGrpID { get; set; }

        [StringLength(200)]
        public string ddGrpName { get; set; }

        [StringLength(200)]
        public string ddVal { get; set; }

        [StringLength(200)]
        public string ddTxt { get; set; }
    }
}
