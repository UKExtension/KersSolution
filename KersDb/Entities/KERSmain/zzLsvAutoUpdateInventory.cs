using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    [Table("zzLsvAutoUpdateInventory")]
    public partial class zzLsvAutoUpdateInventory
    {
        [Key]
        public int rID { get; set; }

        public bool? processOnOff { get; set; }

        [StringLength(150)]
        public string lsvName { get; set; }

        [StringLength(200)]
        public string lsvTitle { get; set; }

        [StringLength(300)]
        public string viewUsedToPopulate { get; set; }
    }
}
