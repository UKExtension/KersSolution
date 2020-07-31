namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zCesCountyEvents")]
    public partial class zCesCountyEvent
    {
        [Key]
        public int rID { get; set; }

        public int? catpaws_calID { get; set; }

        public DateTime? rDt { get; set; }

        [StringLength(8)]
        public string rBy { get; set; }

        public int? seriesID { get; set; }

        [StringLength(8)]
        public string eventStatus { get; set; }

        [StringLength(20)]
        public string planningUnitID { get; set; }

        [StringLength(50)]
        public string eventDateBegin { get; set; }

        [StringLength(4)]
        public string eventTimeBegin { get; set; }

        [StringLength(50)]
        public string eventDateEnd { get; set; }

        [StringLength(4)]
        public string eventTimeEnd { get; set; }

        [StringLength(3)]
        public string progANRcp { get; set; }

        public bool? progANR { get; set; }

        [StringLength(3)]
        public string progHORTcp { get; set; }

        public bool? progHORT { get; set; }

        [StringLength(3)]
        public string progFCScp { get; set; }

        public bool? progFCS { get; set; }

        [StringLength(3)]
        public string prog4HYDcp { get; set; }

        public bool? prog4HYD { get; set; }

        [StringLength(3)]
        public string progFAcp { get; set; }

        public bool? progFA { get; set; }

        [StringLength(3)]
        public string progOthercp { get; set; }

        public bool? progOther { get; set; }

        [StringLength(300)]
        public string eventTitle { get; set; }

        [StringLength(300)]
        public string eventBldgName { get; set; }

        [StringLength(300)]
        public string eventAddress { get; set; }

        [StringLength(300)]
        public string eventCity { get; set; }

        [StringLength(2)]
        public string eventState { get; set; }

        [StringLength(10)]
        public string eventZip { get; set; }

        [StringLength(300)]
        public string eventUrl { get; set; }

        [StringLength(2000)]
        public string eventCounties { get; set; }

        public string eventDescription { get; set; }
    }
}
