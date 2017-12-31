using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.ViewModels
{

    public class PlanOfWorkViewModel
    {
        public int Id {get;set;}
        public FiscalYear FiscalYear { get; set; }
        public PlanningUnit PlanningUnit { get; set; }
        public PlanOfWorkRevision LastRevision { get; set; }

    }
}