using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kers.Controllers.Reports.ViewComponents
{
    public class LatestStoriesViewComponent : ViewComponent
    {
        private readonly IStoryRepository storyRepo;

        public LatestStoriesViewComponent(IStoryRepository storyRepo)
        {
            this.storyRepo = storyRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync( int amount = 4, int PlanningUnitId = 0, int MajorProgramId = 0 )
        {
            var stories = await storyRepo.LastStories( amount, PlanningUnitId, MajorProgramId );
           
            return View(stories);
        }
        
    }
}