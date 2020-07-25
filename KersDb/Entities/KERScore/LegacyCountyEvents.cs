namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

 

    public partial class LegacyCountyEvents
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int rID { get; set; }
        public string catpaws_calID { get; set; }
        public string rDt { get; set; }
        public int rBy { get; set; }
        public string seriesID { get; set; }
        public string eventStatus { get; set; }
        public int planningUnitID { get; set; }
        public string eventDateBegin { get; set; }
        public string eventTimeBegin { get; set; }
        public string eventDateEnd { get; set; }
        public string eventTimeEnd { get; set; }
        public string progANRcp { get; set; }
        public int progANR { get; set; }
        public string progHORTcp { get; set; }
        public int progHORT { get; set; }
        public string progFCScp { get; set; }
        public int progFCS { get; set; }
        public string prog4HYDcp { get; set; }
        public int prog4HYD { get; set; }
        public string progFAcp { get; set; }
        public int progFA { get; set; }
        public string progOthercp { get; set; }
        public int progOther { get; set; }
        public string eventTitle { get; set; }
        public string eventBldgName { get; set; }
        public string eventAddress { get; set; }
        public string eventCity { get; set; }
        public string eventState { get; set; }
        public string eventZip { get; set; }
        public string eventUrl { get; set; }
        public string eventCounties { get; set; }
        public string eventDescription { get; set; }
        
        
    }

}