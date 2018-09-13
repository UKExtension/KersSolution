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
    public class MainStoryViewComponent : ViewComponent
    {
        private readonly IStoryRepository storyRepo;

        public MainStoryViewComponent(IStoryRepository storyRepo)
        {
            this.storyRepo = storyRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync( FiscalYear FiscalYear = null,
        int PlanningUnitId = 0, int MajorProgramId = 0)
        {
            var story = await storyRepo.LastStoryWithImages( FiscalYear, PlanningUnitId, MajorProgramId );
            if( story != null && story.Story != null ){
                var str = System.Text.RegularExpressions.Regex.Replace(story.Story, "<[^>]*>", string.Empty);
                ViewData["Extract"] = str.Substring(0, Math.Min(str.Length, 500));
            }else{
                story = null;
            }
            return View(story);
        }
        
    }
}