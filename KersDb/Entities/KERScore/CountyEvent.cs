namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CountyEvent:ExtensionEvent
    {
        public int classicCountyEventId { get; set; }
        public DateTime? rDt { get; set; }
        public KersUser By { get; set; }
        public ICollection<CountyEventPlanningUnit> Units { get; set; }
        public ICollection<CountyEventProgramCategory> ProgramCategories { get; set; }
        [StringLength(300)]
        public string eventBldgName { get; set; }
        public PhysicalAddress PhysicalAddress {get;set;}
    }
}