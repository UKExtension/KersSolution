using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapBudgetReimbursementsCounty : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public int PlanningUnitId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public FiscalYear FiscalYear {get;set;}
        public int ById {get;set;}
        public KersUser By {get;set;}
        public DateTime ReimbursmentTime {get;set;}
        public DateTime Updated {get;set;}
        public int UpdatedById {get;set;}
        public KersUser UpdatedBy {get;set;}
        public float Amount {get;set;}
        public string Notes {get;set;}

    }
}
