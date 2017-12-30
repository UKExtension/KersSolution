using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{
    public partial class SnapEd_ReinforcementItemSuggestion
    {
        public int Id { get; set; }
        public string Suggestion { get; set; }
        public int? zEmpProfileId { get; set; }
        public int? KersUserId {get;set;}
        public KersUser KersUser {get;set;}
        public int? Common_FiscalYearId { get; set; }
        [ForeignKey("Common_FiscalYearId")]
        public FiscalYear FiscalYear {get;set;}
    }
}
