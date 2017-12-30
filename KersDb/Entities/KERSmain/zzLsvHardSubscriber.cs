using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzLsvHardSubscriber
    {
        [Key]
        public int rID { get; set; }

        public DateTime? rDT { get; set; }

        [StringLength(200)]
        public string lsvName { get; set; }

        [StringLength(200)]
        public string linkBlueID { get; set; }

        [StringLength(200)]
        public string personName { get; set; }

        [StringLength(200)]
        public string personEmailAddress { get; set; }

        public string entryNotes { get; set; }
    }
}
