using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzCESregion_district
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int rID { get; set; }

        public int dID { get; set; }
    }
}
