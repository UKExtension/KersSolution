using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.RegularExpressions;
using Kers.Models.ViewModels;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class StoryController : Controller
    {
        KERScoreContext context;
        public StoryController( 
                    KERScoreContext context
            ){
           this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        
        [HttpGet]
        [Route("{id}", Name="ReportsFullStory")]
        public async Task<IActionResult> GetAction(int id){
            var story = await this.context.Story.Where( s => s.Id == id)
                                            .Include(s => s.Revisions).ThenInclude( r => r.StoryImages).ThenInclude( i => i.UploadImage).ThenInclude( m => m.UploadFile)
                                            .Include(s => s.KersUser).ThenInclude( u => u.PersonalProfile)
                                            .Include(s => s.KersUser).ThenInclude( u => u.RprtngProfile).ThenInclude(u => u.PlanningUnit)
                                            //.Include( s => s.Revisions).ThenInclude( r => r.PlanOfWork).ThenInclude( p => p.Revisions)
                                            .Include( s => s.Revisions ).ThenInclude( r => r.StoryOutcome)
                                            .Include( s => s.Revisions).ThenInclude( r => r.MajorProgram)
                                            .FirstOrDefaultAsync();

            if(story == null){
                return StatusCode(500);
            }
            var strViewModel = new StoryViewModel();
            var lastRevision = story.Revisions.OrderBy( r => r.Created).Last();
            strViewModel.Title = lastRevision.Title;
            strViewModel.Story = lastRevision.Story;
            strViewModel.KersUser = story.KersUser;
            strViewModel.MajorProgram = lastRevision.MajorProgram;
            strViewModel.PlanningUnit = story.KersUser.RprtngProfile.PlanningUnit;
            strViewModel.StoryOutcome = lastRevision.StoryOutcome;
            strViewModel.StoryId = story.Id;
            strViewModel.Updated = lastRevision.Created;
            if(lastRevision.PlanOfWork != null){
                strViewModel.PlanOfWork = lastRevision.PlanOfWork.Revisions.OrderBy( p => p.Created ).Last();
            }
            var firstImage = lastRevision.StoryImages.OrderBy( i => i.Created).FirstOrDefault();
            if(firstImage != null){
                strViewModel.ImageName = firstImage.UploadImage.UploadFile.Name;
            }else{
                strViewModel.ImageName = "";
            }
            return View(strViewModel);
        }
        


    }
}