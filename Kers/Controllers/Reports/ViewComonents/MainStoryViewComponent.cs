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
    public class MainStoryViewComponent : ViewComponent
    {
        private readonly IStoryRepository storyRepo;

        public MainStoryViewComponent(IStoryRepository storyRepo)
        {
            this.storyRepo = storyRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync( FiscalYear FiscalYear = null,
        int PlanningUnitId = 0, int MajorProgramId = 0, int NumStories = 6)
        {

            List<StoryViewModel> stories;
            if( PlanningUnitId != 0 ){
                stories = await storyRepo.LastStoriesWithImages( FiscalYear, FilterKeys.PlanningUnit, PlanningUnitId, NumStories);
            }else if( MajorProgramId != 0 ){
                stories = await storyRepo.LastStoriesWithImages( FiscalYear, FilterKeys.MajorProgram, MajorProgramId, NumStories);
            }else{
                stories = await storyRepo.LastStoriesWithImages( FiscalYear, FilterKeys.All, 0, NumStories);
            }


            StoryViewModel story = null;


            if(stories.Count > 0 ){
                int n = stories.Count;
                int[] array = new int[n];
                for (int i = 0; i < n; i++)
                {
                    array[i] = i;
                }
                Shuffle(array);
                story = stories[array[0]];
                var str = Kers.HtmlHelpers.StripHtmlHelper.StripHtml(story.Story);
                ViewData["Extract"] = str.Substring(0, Math.Min(str.Length, 500));
                if( array.Count() > 3 ){
                    var More = new List<StoryViewModel>();
                    for( int j = 1; j < 4; j++ ){
                        More.Add(stories[array[j]]);
                    }
                    ViewData["More"] = More;
                }
            }
            return View(story);
        }
      
        private void Shuffle(int[] array)
        {
            Random random = new Random();
            int n = array.Count();
            while (n > 1)
            {
                n--;
                int i = random.Next(n + 1);
                int temp = array[i];
                array[i] = array[n];
                array[n] = temp;
            }
        }
        
    }
}