using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    [Table("zzCESregion")]
    public partial class zzCESregion
    {
        [Key]
        public int? rID { get; set; }

        [StringLength(50)]
        public string rName { get; set; }

        [StringLength(8)]
        public string rAdminID { get; set; }
    }
}
