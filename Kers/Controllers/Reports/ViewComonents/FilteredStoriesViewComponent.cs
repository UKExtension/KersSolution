using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Kers.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kers.Controllers.Reports.ViewComponents
{
    public class FilteredStoriesViewComponent : ViewComponent
    {
        private readonly IStoryRepository storyRepo;

        public FilteredStoriesViewComponent(IStoryRepository storyRepo)
        {
            this.storyRepo = storyRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync( FiscalYear fiscalYear = null, int filter = 4, int id = 0, int amount = 6 )
        {
            var stories = new List<StoryViewModel>();

            var withImages = await storyRepo.LastStoriesWithImages(fiscalYear, filter, id, amount);

            if( withImages.Count > 0 ){
                stories.AddRange( withImages );
                if(withImages.Count < amount){
                    stories.AddRange(await storyRepo.LastStoriesWithoutImages(fiscalYear, filter, id, amount - withImages.Count));
                }
            }else{
                stories.AddRange(await storyRepo.LastStoriesWithoutImages(fiscalYear, filter, id, amount));
            }
            ViewData["Title"] = "Stories";
            if(stories.Count > 0){
                if( filter == FilterKeys.Employee ){
                    ViewData["Title"] += " by " + stories.First().KersUser.PersonalProfile.FirstName + " " + stories.First().KersUser.PersonalProfile.LastName;
                }else if( filter == FilterKeys.MajorProgram){
                    ViewData["Title"] += " by " + stories.First().MajorProgram.Name;
                }else if( filter == FilterKeys.PlanningUnit){
                    ViewData["Title"] += " by " + stories.First().PlanningUnit.Name;
                }
            }
            

            return View(stories);
        }
        
    }
}