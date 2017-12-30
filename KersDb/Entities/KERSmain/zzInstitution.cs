using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    [Table("zzInstitution")]
    public partial class zzInstitution
    {
        [Key]
        [StringLength(10)]
        public string iID { get; set; }

        [Column(Order = 1)]
        public string iName { get; set; }
    }
}
