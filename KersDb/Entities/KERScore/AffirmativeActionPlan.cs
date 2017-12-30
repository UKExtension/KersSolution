using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class AffirmativeActionPlan : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public PlanningUnit PlanningUnit { get; set; }
        public FiscalYear FiscalYear {get; set;}
        public List<AffirmativeActionPlanRevision> Revisions {get;set;}
        
    }
}