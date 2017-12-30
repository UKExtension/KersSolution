using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapBudgetReimbursementsNepAssistant : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public FiscalYear FiscalYear {get;set;}
        public int ById {get;set;}
        public KersUser By {get;set;}
        public int ToId {get;set;}
        public KersUser To {get;set;}
        public DateTime ReimbursmentTime {get;set;}
        public DateTime Updated {get;set;}
        public int UpdatedById {get;set;}
        public KersUser UpdatedBy {get;set;}
        public float Amount {get;set;}
        public string Notes {get;set;}
    }
}
