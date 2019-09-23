namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zCesEvents")]
    public partial class zCesEvent
    {
        [Key]
        public int rID { get; set; }

        public DateTime? rDt { get; set; }

        [StringLength(8)]
        public string rBy { get; set; }

        [StringLength(8)]
        public string eventStatus { get; set; }

        [StringLength(50)]
        public string eventDateBeginWork { get; set; }

        [StringLength(50)]
        public string eventDateEndWork { get; set; }

        [StringLength(10)]
        public string eventDateBegin { get; set; }

        [StringLength(25)]
        public string eventTimeBegin { get; set; }

        [StringLength(10)]
        public string eventDateEnd { get; set; }

        [StringLength(25)]
        public string eventTimeEnd { get; set; }

        [StringLength(300)]
        public string eventTimeTmp { get; set; }

        [StringLength(300)]
        public string eventTitle { get; set; }

        [StringLength(300)]
        public string eventLocation { get; set; }

        [StringLength(300)]
        public string eventContact { get; set; }

        public string eventDescription { get; set; }
    }
}
