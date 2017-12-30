using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzPac
    {
        [Key]
        public int pacID { get; set; }

        public int? FY { get; set; }

        public int? cesProgCategory { get; set; }

        [StringLength(100)]
        public string cesProgCategoryNameShort { get; set; }

        public int? rptCodeType { get; set; }

        public int? rptOrderID { get; set; }

        public int? pacCodeID { get; set; }

        [StringLength(100)]
        public string pacTitle { get; set; }
    }
}
