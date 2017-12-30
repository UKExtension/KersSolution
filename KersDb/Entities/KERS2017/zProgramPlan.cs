using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERS2017
{
    public partial class zProgramPlan
    {
        [Key]
        public int planID { get; set; }

        public DateTime? rDT { get; set; }

        [StringLength(8)]
        public string rBY { get; set; }

        [StringLength(200)]
        public string rByName { get; set; }

        public int? FY { get; set; }

        [StringLength(10)]
        public string instID { get; set; }

        [StringLength(50)]
        public string planningUnitID { get; set; }

        public int? mapID { get; set; }

        [StringLength(300)]
        public string programPlanTitle { get; set; }

        [StringLength(300)]
        public string agentsInvolved { get; set; }

        public int? pacID1 { get; set; }

        public int? pacID2 { get; set; }

        public int? pacID3 { get; set; }

        public int? pacID4 { get; set; }

        public string situation { get; set; }

        public string longTermOutcomes { get; set; }

        public string intermediateOutcomes { get; set; }

        public string initialOutcomes { get; set; }

        public string evaluation { get; set; }

        public string learning { get; set; }
    }
}
