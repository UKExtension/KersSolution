using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.ViewModels;
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
using Kers.Models.Abstract;
using Kers.Models.Repositories;
using System.Text;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class DataController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepo;

        public DataController( 
                    KERScoreContext context,
                    IFiscalYearRepository fiscalYearRepo
            ){
           this.context = context;
           this.fiscalYearRepo = fiscalYearRepo;
        }
        [HttpGet]
        [Route("{fy?}")]
        public IActionResult Index(string fy="0")
        {
            ViewData["fy"] = fy;
            return View();
        }
        
        [HttpGet]
        [Route("qltrx")]
        public IActionResult Qualtrics(string title = "", string id = "", string dates = "")
        {
            var contents = "[[AdvancedFormat]]\n\n[[Question:Text]]\n" + 
                            "Cooperative Extension In-Service Training Evaluation <br /><br />" + title +
                            "[" + id + "] <br /><br />" +
                            dates + "\n\n" +
                            "[[Question:Matrix]]\n"+
                            "The Content:\n"+
                            "[[Choices]]\n"+
                            "Was relevant to my needs.\n"+
                            "Was well organized.\n"+
                            "Was adequately related to the topic.\n"+
                            "Was easy to understand.\n"+
                            "[[AdvancedAnswers]]\n"+
                            "[[Answer]]\n"+
                            "Strongly Disagree\n"+
                            "[[Answer]]\n"+
                            "Disagree\n"+
                            "[[Answer]]\n"+
                            "Neutral\n"+
                            "[[Answer]]\n"+
                            "Agree\n"+
                            "[[Answer]]\n"+
                            "Strongly Agree\n"+
                            "\n"+
                            "[[Question:Matrix]]\n"+
                            "The Instructor(s):\n"+
                            "[[Choices]]\n"+
                            "Were well-prepared.\n"+
                            "Used teaching methods appropriate for the content/audience.\n"+
                            "Was knowledgeable of the subject matter.\n"+
                            "Engaged the participants in learning.\n"+
                            "Related program content to practical situations.\n"+
                            "Answered questions clearly and accurately.\n"+
                            "[[AdvancedAnswers]]\n"+
                            "[[Answer]]\n"+
                            "Strongly Disagree\n"+
                            "[[Answer]]\n"+
                            "Disagree\n"+
                            "[[Answer]]\n"+
                            "Neutral\n"+
                            "[[Answer]]\n"+
                            "Agree\n"+
                            "[[Answer]]\n"+
                            "Strongly Agree\n"+
                            "\n"+
                            "[[Question:Matrix]]\n"+
                            "Outcomes:\n"+
                            "[[Choices]]\n"+
                            "I gained knowledge/skills about the topics presented.\n"+
                            "I will use what I learned in my county program.\n"+
                            "This information will help my program move to the next level.\n"+
                            "Based on the in-service, I am now able to teach this topic to others.\n"+
                            "[[AdvancedAnswers]]\n"+
                            "[[Answer]]\n"+
                            "Strongly Disagree\n"+
                            "[[Answer]]\n"+
                            "Disagree\n"+
                            "[[Answer]]\n"+
                            "Neutral\n"+
                            "[[Answer]]\n"+
                            "Agree\n"+
                            "[[Answer]]\n"+
                            "Strongly Agree\n"+
                            "\n"+
                            "[[Question:TE]]\n"+
                            "Based on this in-service, what are two things that you are encouraged to do within the next month?\n"+
                            "\n"+
                            "[[Question:TE]]\n"+
                            "Based on this in-service, what are two things that you are encouraged to do within the next six (6) months?\n"+
                            "\n"+
                            "[[Question:TE]]\n"+
                            "If you have a program related to this topic, what do you think will help take it to the next level (i.e., achieve higher level impact)?\n"+
                            "\n"+
                            "[[Question:TE]]\n"+
                            "Please provide any additional comments about this training.\n"+
                            "\n"+
                            "[[Question:TE]]\n"+
                            "Please provide any comments about the instructor or any additional instructors/presenters.\n"+
                            "\n";


            return Content(contents);

        }

        [HttpGet]
        [Route("storiescounties/{fy?}/{id?}", Name = "StoryCounty")]
        public async Task<IActionResult> StoriesCounty(int id = 0, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                //this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", "Reports", "Error");
                return new StatusCodeResult(500);
            }
            var units = await this.context.PlanningUnit.OrderBy(l => l.order).ToListAsync();
            var model = new UnitStoryViewModel();
            model.PlanningUnits = units;
            if(id != 0){
                var unit = await this.context.PlanningUnit.Where( u => u.Id == id).FirstOrDefaultAsync();
                if(unit != null){
                    model.PlanningUnit = unit;
                    var stories = this.context.Story.
                                            Where( s => 
                                                        s.KersUser.RprtngProfile.PlanningUnit == unit
                                                        )
                                            .Include(s => s.Revisions).ThenInclude( r => r.StoryImages).ThenInclude( i => i.UploadImage).ThenInclude( m => m.UploadFile)
                                            .Include(s => s.KersUser).ThenInclude( u => u.PersonalProfile)
                                            .Include(s => s.KersUser).ThenInclude( u => u.RprtngProfile).ThenInclude(u => u.PlanningUnit)
                                            //.Include( s => s.Revisions).ThenInclude( r => r.PlanOfWork).ThenInclude( p => p.Revisions)
                                            .Include( s => s.Revisions ).ThenInclude( r => r.StoryOutcome)
                                            .Include( s => s.Revisions).ThenInclude( r => r.MajorProgram).ThenInclude( p => p.StrategicInitiative).ThenInclude( i => i.FiscalYear)
                                            .ToList();
                    var fyStories = new List<Story>();
                    foreach( var story in stories ){
                        var lastRev = story.Revisions.OrderBy( r => r.Created).Last();
                        if( lastRev.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear ){
                            fyStories.Add( story );
                        }
                    }
                    model.Stories = this.storyViewModelList(fyStories);
                }
            }
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            return View(model);
        }



        [HttpGet]
        [Route("storiesprogram/{id?}/{fy?}", Name = "StoryProgram")]
        public async Task<IActionResult> StoriesProgram(int id = 0, string fy = "0")
        {

            
            FiscalYear fiscalYear;
            var model = new ProgramStoryViewModel();
            
            if(id != 0){
                var program = await this.context.MajorProgram.Where( u => u.Id == id)
                                        .Include( p => p.StrategicInitiative).ThenInclude( i => i .FiscalYear)
                                        .FirstOrDefaultAsync();
                if(program != null){
                    model.MajorProgram = program;

                    var str = from stry in context.Story
                            let l = (from lim in stry.Revisions
                                    orderby lim.Created descending
                                    select lim).FirstOrDefault()
                            where l.MajorProgramId == id select stry;


                    var stories = await str
                                            .Include(s => s.Revisions).ThenInclude( r => r.StoryImages).ThenInclude( i => i.UploadImage).ThenInclude( m => m.UploadFile)
                                            .Include(s => s.KersUser).ThenInclude( u => u.PersonalProfile)
                                            .Include(s => s.KersUser).ThenInclude( u => u.RprtngProfile).ThenInclude(u => u.PlanningUnit)
                                            //.Include( s => s.Revisions).ThenInclude( r => r.PlanOfWork).ThenInclude( p => p.Revisions)
                                            .Include( s => s.Revisions ).ThenInclude( r => r.StoryOutcome)
                                            .Include( s => s.Revisions).ThenInclude( r => r.MajorProgram)
                                            .ToListAsync();
                    model.Stories = this.storyViewModelList(stories);
                    
                }
                fiscalYear = program.StrategicInitiative.FiscalYear;
            }else{
                fiscalYear = GetFYByName(fy);

                if(fiscalYear == null){
                    
                    return new StatusCodeResult(500);
                }
            }
            var programs = await this.context.MajorProgram.Where( p => p.StrategicInitiative.FiscalYear == fiscalYear).OrderBy(l => l.order).ToListAsync();
            model.MajorPrograms = programs;
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            return View(model);
        }


        private List<StoryViewModel> storyViewModelList(List<Story> stories){
            List<StoryViewModel> modelStories = new List<StoryViewModel>();
            foreach( var story in stories ){
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
                modelStories.Add(strViewModel);
            }
            return modelStories;
        }

/*

        [HttpPost]
        [Route("storiescounties")]
        public IActionResult StoriesCounty([FromForm] int id)
        {
            return View();
        }

 */
        private FiscalYear GetFYByName(string fy, string type = "serviceLog"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalYearRepo.currentFiscalYear(type);
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == type).FirstOrDefault();
            }
            return fiscalYear;
        }
        


    }
}