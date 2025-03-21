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
using Microsoft.Extensions.Caching.Memory;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class DistrictController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        IMemoryCache memoryCache;
        IFiscalYearRepository fiscalYearRepo;
        public DistrictController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IMemoryCache memoryCache
            ){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           this.memoryCache = memoryCache;
        }

 


        [HttpGet("{id?}")]
        public IActionResult Get(int id = 0){
            if(id == 0){
                var unit = CurrentPlanningUnit();
                id = unit.District.Id;
            }
            var district = this.context.District.
                                Where(c=>c.Id == id).
                                FirstOrDefault();
            return new OkObjectResult(district);
        }

        [HttpGet("counties/{id}")]
        public IActionResult GetCounties(int id){
            var counties = this.context.PlanningUnit.
                                Where(c=>c.District.Id == id && c.Code.StartsWith("21")).
                                Include( d => d.Geography).
                                OrderBy(d => d.Name);
            return new OkObjectResult(counties);
        }


        [HttpGet("employeeactivity/{id}/{month}/{year}/{order?}/{type?}/{skip?}/{take?}/{filter?}/{includePairings?}")]
        public async Task<IActionResult> EployeeActivity(int id, int month, int year, string order = "asc", string type = "activity", int skip = 0, int take = 20, string filter = "district", bool includePairings = true){
            List<KersUser> districtEmployees;
            if( filter == "area" ){
                if( includePairings){
                    var areaController = new ExtensionAreaController(mainContext,context, userRepo, memoryCache);
                    var area = this.context.ExtensionArea.Find(id);
                    var pairings = areaController.FindContainingPair(area.Name);
                    districtEmployees = await context.KersUser
                                            .Where( u => pairings.Contains(u.RprtngProfile.PlanningUnit.ExtensionArea.Name)
                                                            &&
                                                            u.ExtensionPosition.Title == "Extension Agent"
                                                            &&
                                                            u.RprtngProfile.PlanningUnit.Name != "Wildcat County CES (demo only)"
                                                            && 
                                                            u.RprtngProfile.enabled)
                                            .Include( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                                            .Include( u => u.PersonalProfile).ThenInclude( r => r.UploadImage)
                                            .ToListAsync();

                }else{
                    districtEmployees = await context.KersUser
                                            .Where( u => u.RprtngProfile.PlanningUnit.ExtensionAreaId == id
                                                            &&
                                                            u.ExtensionPosition.Title == "Extension Agent"
                                                            &&
                                                            u.RprtngProfile.PlanningUnit.Name != "Wildcat County CES (demo only)"
                                                            && 
                                                            u.RprtngProfile.enabled)
                                            .Include( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                                            .Include( u => u.PersonalProfile).ThenInclude( r => r.UploadImage)
                                            .ToListAsync();
                } 
            }else if( filter == "region" ){
                districtEmployees = await context.KersUser
                                            .Where( u => u.RprtngProfile.PlanningUnit.ExtensionArea.ExtensionRegionId == id
                                                            &&
                                                            u.ExtensionPosition.Title == "Extension Agent"
                                                            &&
                                                            u.RprtngProfile.PlanningUnit.Name != "Wildcat County CES (demo only)"
                                                            && 
                                                            u.RprtngProfile.enabled)
                                            .Include( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                                            .Include( u => u.PersonalProfile).ThenInclude( r => r.UploadImage)
                                            .ToListAsync();

            
            }else if( filter == "county" ){
                districtEmployees = await context.KersUser
                                            .Where( u => u.RprtngProfile.PlanningUnitId == id
                                                            &&
                                                            u.ExtensionPosition.Title == "Extension Agent"
                                                            &&
                                                            u.RprtngProfile.PlanningUnit.Name != "Wildcat County CES (demo only)"
                                                            && 
                                                            u.RprtngProfile.enabled)
                                            .Include( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                                            .Include( u => u.PersonalProfile).ThenInclude( r => r.UploadImage)
                                            .ToListAsync();

            
            }else{
                districtEmployees = await context.KersUser
                                            .Where( u => u.RprtngProfile.PlanningUnit.DistrictId == id
                                                            &&
                                                            u.ExtensionPosition.Title == "Extension Agent"
                                                            &&
                                                            u.RprtngProfile.PlanningUnit.Name != "Wildcat County CES (demo only)"
                                                            && 
                                                            u.RprtngProfile.enabled)
                                            .Include( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                                            .Include( u => u.PersonalProfile).ThenInclude( r => r.UploadImage)
                                            .ToListAsync();
            }
            

            var numActivities = new List<EmployeeNumActivities>();

            foreach( var employee in districtEmployees ){
                if( employee.PersonalProfile.UploadImage != null ){
                    if(employee.PersonalProfile.UploadImage.Name == null && employee.PersonalProfile.UploadImage.UploadFileId != null ){
                        var file = await context.UploadFile.Where(u => u.Id == employee.PersonalProfile.UploadImage.UploadFileId ).FirstOrDefaultAsync();
                        employee.PersonalProfile.UploadImage.Name = file.Name;
                        await context.SaveChangesAsync();
                    }
                }
                int num;
                if( type == "activity"){
                    num = await context.Activity.Where( a => a.KersUser == employee && a.ActivityDate.Month == (month + 1) && a.ActivityDate.Year == year ).CountAsync();
                }else if( type == "indicator"){
                    var submissions = await context.ProgramIndicatorValue
                                .Where( a => a.KersUser == employee).ToListAsync();
                    num = submissions
                                .Where( a => a.LastModifiedDateTime.Value.UtcDateTime.Month == (month + 1) && a.LastModifiedDateTime.Value.UtcDateTime.Year == year)
                                .Count();
                }else{
                    num = await context.Expense.Where( a => a.KersUser == employee && a.ExpenseDate.Month == (month + 1) && a.ExpenseDate.Year == year ).CountAsync();
                }
                var empData = new EmployeeNumActivities{
                    User = employee,
                    NumActivities = num
                };
                numActivities.Add(empData);
            }

            

            if(order == "asc"){
                numActivities = numActivities.OrderBy( o => o.NumActivities).ToList();
            }else{
                numActivities = numActivities.OrderByDescending( o => o.NumActivities).ToList();
            }
            numActivities = numActivities.Skip(skip).Take(take).ToList();

            return new OkObjectResult(numActivities);
        }



        [HttpGet("mycounties")]
        [Authorize]
        public IActionResult MyCounties(){
            var unit = CurrentPlanningUnit();
            var counties = this.context.PlanningUnit.
                                Where(c=>c.District.Id == unit.District.Id && c.Code.StartsWith("21")).
                                OrderBy(d => d.Name);
            return new OkObjectResult(counties);
        }

        [HttpGet("countiesnoaa/{id}")]
        public IActionResult GetCountiesNoAa(int id){
            var counties = this.context.PlanningUnit.
                                Where(      c=>c.District.Id == id 
                                        && 
                                            c.Code.StartsWith("21")
                                        ).
                                OrderBy(d => d.Name);
            
            var noAa = new List<PlanningUnit>();
            foreach(var county in counties){
                var aa = this.context.AffirmativeActionPlan.Where(a => a.PlanningUnit == county).FirstOrDefault();
                if(aa == null){
                    noAa.Add(county);
                }
            }
            return new OkObjectResult(noAa);
        }


        [HttpGet("countiesnopl/{id}")]
        public IActionResult GetCountiesNoPl(int id){
            var counties = this.context.PlanningUnit.
                                Where(      c=>c.District.Id == id 
                                        && 
                                            c.Code.StartsWith("21")
                                        ).
                                OrderBy(d => d.Name);
            
            var noPl = new List<PlanningUnit>();
            foreach(var county in counties){
                var plans = this.context.PlanOfWork.
                                    Where(a => a.PlanningUnit == county).
                                    Include(p => p.Revisions);
                if(plans == null){
                    noPl.Add(county);
                }else{
                    var hasPlan = false;
                    foreach(var plan in plans){
                        if(plan.Revisions.Count > 1){
                            hasPlan = true;
                            break;
                        }
                    }
                    if(!hasPlan){
                        noPl.Add(county);
                    }
                }
            }
            return new OkObjectResult(noPl);
        }



        private void Log(   object obj, 
                            string objectType = "AffirmativeActionPlanRevision",
                            string description = "Submitted Affirmative Action Plan Revision", 
                            string type = "Affirmative Action Plan",
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
                user = userRepo.findByProfileID(profile.Id);
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }

        private PlanningUnit CurrentPlanningUnit(){
            var u = this.CurrentUserId();
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == u).
                            FirstOrDefault();
            return  this.context.PlanningUnit.
                    Where( p=>p.Code == profile.planningUnitID).
                    Include(l => l.District).
                    FirstOrDefault();
        }

        private zEmpRptProfile profileByUser(KersUser user){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.Id == user.classicReportingProfileId).
                            FirstOrDefault();
            
            return profile;
        }

        private KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }

        private zEmpRptProfile CurrentProfile(){
            var u = this.CurrentUserId();
            return mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == u).
                            FirstOrDefault();
        }

        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }



    }



    public class LocalizationRecord
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public string LocalizationCulture { get; set; }
        public string ResourceKey { get; set; }
    }

    public class EmployeeNumActivities{
        public KersUser User {get;set;}
        public int NumActivities {get;set;}
    }




}