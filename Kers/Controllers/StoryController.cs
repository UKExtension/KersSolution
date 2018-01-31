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

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class StoryController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;

        private int DefaultNumberOfItems = 10;
        public StoryController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    IFiscalYearRepository fiscalYearRepo
            ){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
        }


        [HttpGet("numb")]
        [Authorize]
        public IActionResult GetNumb(){
            
            var numStories = context.Story.
                                Where(e=>e.KersUser == this.CurrentUser());
            
            return new OkObjectResult(numStories.Count());
        }

        [HttpGet("id/{id}")]
        [Authorize]
        public IActionResult ById(int id){
            
            var story = context.StoryRevision.
                                Include(s => s.MajorProgram).
                                Where(e=>e.Id == id).
                                FirstOrDefault();
            return new OkObjectResult(story);
            
            
        }

        [HttpGet("latest/{skip?}/{amount?}")]
        [Authorize]
        public IActionResult Get(int skip = 0, int amount = 10){
            
            var lastStories = context.Story.
                                Where(e=>e.KersUser == this.CurrentUser()).
                                Include(s => s.Revisions).ThenInclude(r => r.StoryImages).
                                OrderByDescending(e=>e.Updated).
                                Skip(skip).
                                Take(amount);
            
            var revs = new List<StoryRevision>();
            if(lastStories != null){
                foreach(var story in lastStories){
                    revs.Add( story.Revisions.OrderBy(r=>r.Created).Last() );
                }
            }
            return new OkObjectResult(revs);
        }


        [HttpGet("author/{storyid}")]
        [Authorize]
        public IActionResult Author(int storyid){
            
            var rev = context.StoryRevision.
                                Where(e=>e.Id == storyid).
                                FirstOrDefault();
            
            if(rev==null){
                return new OkObjectResult(null);
            }else{
                var story = context.Story.
                            Where(s => s.Id == rev.StoryId).
                            Include(s => s.KersUser).ThenInclude(u => u.PersonalProfile).
                            FirstOrDefault();
                if(story == null){
                    return null;
                }else{
                    return new OkObjectResult(story.KersUser);
                }

            }
            
        }

        [HttpGet("latestbyuser/{userid}/{amount?}")]
        [Authorize]
        public IActionResult LatestByUser(int userid, int amount = 10){
            
            var lastStories = context.Story.
                                Where(e=>e.KersUser.Id == userid).
                                Include(s => s.Revisions).ThenInclude(r => r.StoryImages).ThenInclude(i => i.UploadImage).ThenInclude(f => f.UploadFile).
                                OrderByDescending(e=>e.Updated).
                                Take(amount);
            
            var revs = new List<StoryRevision>();
            if(lastStories != null){
                foreach(var story in lastStories){
                    revs.Add( story.Revisions.OrderBy(r=>r.Created).Last() );
                }
            }
            foreach(var rev in revs){
                foreach( var img in rev.StoryImages){
                    img.UploadImage.UploadFile.Content = null;
                }
            }
            return new OkObjectResult(revs);
        }



        [HttpGet("perDay/{userid}/{start}/{end}")]
        [Authorize]
        public IActionResult PerDay(int userid, DateTime start, DateTime end ){
            
            end = end.AddDays(1);
            var numPerDay = context.Story.
                                Where(a=>a.KersUser.Id == userid & a.Updated > start & a.Updated < end).
                                GroupBy(e => new {
                                    Date = e.Updated.Day
                                }).
                                Select(c => new {
                                    Day = c.FirstOrDefault().Updated.ToString("yyyy-MM-dd"),
                                    Count = c.Count()
                                }).
                                OrderByDescending(e => e.Day);
            return new OkObjectResult(numPerDay);
        }


        [HttpGet("GetCustom")]
        public IActionResult GetCustom( [FromQuery] string search, 
                                        [FromQuery] string unit = "0", 
                                        [FromQuery] string amount = "0",
                                        [FromQuery] string program = "0",
                                        [FromQuery] string snap = "0",
                                        [FromQuery] string withImage = "0"
                                        ){
            var theAmount = Convert.ToInt32(amount);
            theAmount =  theAmount <= 0 ? DefaultNumberOfItems : theAmount ;

            var stories = from i in context.Story select i;
            if(search != null){
                stories = stories.Where( i => i.Revisions.OrderBy(r => r.Created).Last().Title.Contains(search) || i.Revisions.OrderBy(r => r.Created).Last().Story.Contains(search));
            }
            if( unit != "0" ){
                stories = stories.Where( i => i.KersUser.RprtngProfile.PlanningUnitId == Convert.ToInt32(unit) );
            }
            if( program != "0"){
                stories = stories.Where( i => i.Revisions.OrderBy(r => r.Created).Last().MajorProgramId == Convert.ToInt32(program) );
            }
            if( snap != "0"){
                stories = stories.Where( i => i.Revisions.OrderBy(r => r.Created).Last().IsSnap );
            }
            if( withImage != "0"){
                stories = stories.Where( i => i.Revisions.OrderBy(r => r.Created).Last().StoryImages.Count() > 0 );
            }
            
            stories = stories.Include(s => s.Revisions).ThenInclude(r => r.StoryImages).ThenInclude(i => i.UploadImage).ThenInclude(f => f.UploadFile).
                        Include( s => s.Revisions).ThenInclude(u => u.MajorProgram);
            stories = stories.OrderByDescending(s => s.Updated);
            stories = stories.Take(theAmount);
            var revs = new List<StoryRevision>();
            if(stories != null){
                foreach(var story in stories){
                    revs.Add( story.Revisions.OrderBy(r=>r.Created).Last() );
                }
            }
            foreach(var rev in revs){
                foreach( var img in rev.StoryImages){
                    img.UploadImage.UploadFile.Content = null;
                }
            }
            return new OkObjectResult(revs);
        }

        [HttpGet("GetCustomCount")]
        public IActionResult GetCustomCount(
                                                [FromQuery] string search, 
                                                [FromQuery] string unit = "0", 
                                                [FromQuery] string program = "0",
                                                [FromQuery] string snap = "0",
                                                [FromQuery] string withImage = "0"
                                            ){
            var stories = from i in context.Story select i;
            if(search != null){
                stories = stories.Where( i => i.Revisions.OrderBy(r => r.Created).Last().Title.Contains(search));
            }
            if( unit != "0" ){
                stories = stories.Where( i => i.KersUser.RprtngProfile.PlanningUnitId == Convert.ToInt32(unit) );
            }
            if( program != "0"){
                stories = stories.Where( i => i.Revisions.OrderBy(r => r.Created).Last().MajorProgramId == Convert.ToInt32(program) );
            }
            if( snap != "0"){
                stories = stories.Where( i => i.Revisions.OrderBy(r => r.Created).Last().IsSnap );
            }
            if( withImage != "0"){
                stories = stories.Where( i => i.Revisions.OrderBy(r => r.Created).Last().StoryImages.Count() > 0 );
            }
            return new OkObjectResult(stories.Count());
        }





        [HttpPost()]
        [Authorize]
        public IActionResult AddStory( [FromBody] StoryRevision story){
            
            if(story != null){

                
                var user = this.CurrentUser();
                var str = new Story();
                str.KersUser = user;
                str.Created = DateTime.Now;
                str.Updated = DateTime.Now;
                str.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                story.Created = DateTime.Now;
                str.Revisions = new List<StoryRevision>();
                str.Revisions.Add(story);
                context.Add(str); 
                this.Log(str,"Story", "Success Story Added.");
                context.SaveChanges();
                return new OkObjectResult(story);
            }else{
                this.Log( story ,"StoryRevision", "Not Found Success Story in an adding attempt.", "Success Story", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateStory( int id, [FromBody] StoryRevision story){
           
            
            var entity = context.StoryRevision.Find(id);
            var stEntity = context.Story.Find(entity.StoryId);

            if(story != null && stEntity != null){
                story.Created = DateTime.Now;
                stEntity.Revisions.Add(story);
                stEntity.Updated = DateTime.Now;
                context.SaveChanges();
                this.Log( story ,"StoryRevision", "Success Story Updated.");
                return new OkObjectResult(story);
            }else{
                this.Log( story ,"StoryRevision", "Not Found Success Story in an update attempt.", "Success Story", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStory( int id ){
            var entity = context.StoryRevision.Find(id);
            var acEntity = context.Story.Where(a => a.Id == entity.StoryId).
                                Include(e=>e.Revisions).
                                FirstOrDefault();
            
            if(acEntity != null){
                
                context.Story.Remove(acEntity);
                context.SaveChanges();
                
                this.Log(acEntity,"SuccessStory", "Success Story Deleted.");

                return new OkResult();
            }else{
                this.Log( id ,"StoryRevision", "Not Found Success Story in a delete attempt.", "Success Story", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpGet("outcome")]
        public IActionResult Outcome(){
            var outs = this.context.StoryOutcome.OrderBy(o => o.Order);
            return new OkObjectResult(outs);
        }

        private void Log(   object obj, 
                            string objectType = "SuccessStory",
                            string description = "Submitted Success Story", 
                            string type = "Success Story",
                            string level = "Information"
                        ){
                             
            var log = new Log();
            log.Level = level;
            log.Time = DateTime.Now;
            log.User = this.CurrentUser();
            log.ObjectType = objectType;
            log.Object = JsonConvert.SerializeObject(obj,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
            log.Agent = Request.Headers["User-Agent"].ToString();
            log.Ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            log.Description = description;
            log.Type = type;
            this.context.Log.Add(log);
            context.SaveChanges();

        }

        private KersUser userByProfileId(string profileId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.personID == profileId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = userRepo.findByProfileID(profile.Id);
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }


        private KersUser userByLinkBlueId(string linkBlueId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = this.context.KersUser.
                            Where( u => u.classicReportingProfileId == profile.Id).
                            Include(u => u.RprtngProfile).
                            Include(u => u.ExtensionPosition).
                            FirstOrDefault();
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }


        private KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }



        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }



    }
}