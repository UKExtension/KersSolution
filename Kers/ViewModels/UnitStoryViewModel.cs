using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.ViewModels
{

    public class UnitStoryViewModel
    {
        public PlanningUnit PlanningUnit { get; set; }
        public List<PlanningUnit> PlanningUnits { get; set; }
        public List<StoryViewModel> Stories { get; set; }

    }
}