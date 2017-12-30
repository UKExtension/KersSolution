using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapCountyBudget : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public float AnnualBudget {get;set;}
        public int PlanningUnitId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public DateTime Updated {get;set;}
        public int ById {get;set;}
        public KersUser By {get;set;}
        public int FiscalYearId {get;set;}
        public FiscalYear FiscalYear {get;set;}
    }
}
