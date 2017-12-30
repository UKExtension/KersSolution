using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{
    public partial class SnapEd_ReinforcementItemChoice
    {
        public int Id { get; set; }
        public int? SnapEd_ReinforcementItemId { get; set; }
        public SnapEd_ReinforcementItem SnapEd_ReinforcementItem {get;set;}
        public int? Common_FiscalYearId { get; set; }
        [ForeignKey("Common_FiscalYearId")]
        public FiscalYear FiscalYear {get;set;}
        public int? zEmpProfileId { get; set; }
        public int? KersUserId {get;set;}
        public KersUser KersUser {get;set;}
    }
}
