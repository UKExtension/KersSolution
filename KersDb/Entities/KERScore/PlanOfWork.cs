using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class PlanOfWork : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public PlanningUnit PlanningUnit {get; set;}
        public FiscalYear FiscalYear {get; set;}
        public List<PlanOfWorkRevision> Revisions {get; set;}
    }
}