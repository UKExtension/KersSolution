using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    [Table("zzCESdistrict")]
    public partial class zzCESdistrict
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dID { get; set; }

        [StringLength(200)]
        public string dName { get; set; }

        [StringLength(75)]
        public string dAreaName { get; set; }

        [StringLength(8)]
        public string dAdminID { get; set; }

        [StringLength(8)]
        public string dAsstID { get; set; }
    }
}
