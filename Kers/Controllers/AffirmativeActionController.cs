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
    public class AffirmativeActionController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IAffirmativeActionPlanRevisionRepository repo;
        public AffirmativeActionController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IAffirmativeActionPlanRevisionRepository repo
            ){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           this.repo = repo;
        }

        [HttpGet("{unit?}/{fy?}")]
        [Authorize]
        public IActionResult Get(int unit = 0, string fy = "0"){
            if(unit == 0){
                unit = this.CurrentPlanningUnit().Id;
            }
            FiscalYear FiscalYear;
            if(fy == "0"){
                FiscalYear = fiscalYearRepo.nextFiscalYear( FiscalYearType.ServiceLog );
            }else{
                FiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == FiscalYearType.ServiceLog ).FirstOrDefault();
            }
            var plan = context.AffirmativeActionPlan.
                                    Where(p=>p.PlanningUnitId == unit && p.FiscalYearId == FiscalYear.Id).
                                    Include(a=>a.Revisions).ThenInclude(r=>r.MakeupValues).
                                    Include(a=>a.Revisions).ThenInclude(r=>r.SummaryValues).
                                    FirstOrDefault();
            if(plan != null){
                var lastRevision = plan.Revisions.OrderBy( r => r.Created).Last();
                if(lastRevision != null){
                    lastRevision.MakeupValues = lastRevision.MakeupValues.
                                                    OrderBy(m=>m.GroupTypeId).
                                                    ThenBy(m=>m.DiversityTypeId).
                                                    ToList();
                    lastRevision.SummaryValues = lastRevision.SummaryValues.
                                                OrderBy(m=>m.GroupTypeId).
                                                ThenBy(m=>m.DiversityTypeId).
                                                ToList();
                }
                return new OkObjectResult(lastRevision);
            }                    
                                    
            return new OkObjectResult( plan );
        }





        [HttpGet("countieswithoutreport/{id?}/{fy?}/{type?}")]
        [Authorize]
        public IActionResult CountiesWithoutReport(int id = 0, string fy = "0", string type = "district"){
            FiscalYear FiscalYear;
            if(fy == "0"){
                FiscalYear = fiscalYearRepo.currentFiscalYear( FiscalYearType.ServiceLog );
            }else{
                FiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == FiscalYearType.ServiceLog ).FirstOrDefault();
            }
            var plansPerFiscalYear = this.context.AffirmativeActionPlan
                                            .Where(p => p.FiscalYearId == FiscalYear.Id && p.PlanningUnit.Name.Substring(p.PlanningUnit.Name.Count() - 3) == "CES")
                                            .Include( a => a.PlanningUnit).ThenInclude( u => u.ExtensionArea)
                                            .Include( a => a.Revisions ).ToList();
            if(id != 0){
                if(type == "area"){
                    // Find the area
                    var area = this.context.ExtensionArea.Where( a => a.Id == id ).FirstOrDefault();
                    var AreaController = new ExtensionAreaController(mainContext,context,userRepo);
                    var pairing = AreaController.FindContainingPair( area.Name );
                    plansPerFiscalYear = plansPerFiscalYear
                                            .Where( p => pairing.Contains(p.PlanningUnit.ExtensionArea.Name)).ToList();
                }else if( type == "region"){
                    plansPerFiscalYear = plansPerFiscalYear
                                            .Where( p => p.PlanningUnit.ExtensionArea.ExtensionRegionId == id).ToList();
                }else{
                    plansPerFiscalYear = plansPerFiscalYear
                                            .Where( p => p.PlanningUnit.DistrictId == id).ToList();
                }
            }
            



            var plansPerCounty = plansPerFiscalYear.GroupBy( p => p.PlanningUnit )
                                    .Select( p =>
                                        new {
                                            county = p.Key,
                                            report = p.Select( a => a ).Last()
                                        }
                                    
                                    );
            var countiesWithoutPlan = new List<PlanningUnit>();
            foreach( var cnt in plansPerCounty){
                var lastRev = cnt.report.Revisions.OrderBy(r => r.Created ).Last();
                if(lastRev.Efforts == "" || lastRev.Success == "" || lastRev.Efforts == null || lastRev.Success == null){
                    countiesWithoutPlan.Add(cnt.county);
                }
            }
                                    
            return new OkObjectResult( countiesWithoutPlan.OrderBy(c => c.Name) );
        }





        [HttpGet("countieswithoutplan/{id?}/{fy?}/{type?}")]
        [Authorize]
        public IActionResult CountiesWithoutPlan(int id = 0, string fy = "0", string type = "district"){
            FiscalYear FiscalYear;
            if(fy == "0"){
                FiscalYear = fiscalYearRepo.nextFiscalYear( FiscalYearType.ServiceLog );
            }else{
                FiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == FiscalYearType.ServiceLog ).FirstOrDefault();
            }

            List<PlanningUnit> counties = new List<PlanningUnit>();
            if(id != 0){
                if(type == "area"){
                    // Find the area
                    var area = this.context.ExtensionArea.Where( a => a.Id == id ).FirstOrDefault();
                    if( area != null ){
                        var AreaController = new ExtensionAreaController(mainContext,context,userRepo);
                        var pairing = AreaController.FindContainingPair( area.Name );
                        counties = context.PlanningUnit.Where( c => pairing.Contains(c.ExtensionArea.Name) && c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
                    }
                    

                }else if( type == "region"){
                    counties = context.PlanningUnit.Where( c => c.ExtensionArea.ExtensionRegionId == id && c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
                }else{
                    counties = context.PlanningUnit.Where( c => c.DistrictId == id && c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
                }
            }else{
                counties = context.PlanningUnit.Where( u => u.District != null && u.Name.Substring(u.Name.Count() - 3) == "CES").ToList();
            }

            var countiesWithoutPlan = new List<PlanningUnit>();

            foreach( var county in counties ){
                if(!this.context.AffirmativeActionPlan.Where( p => p.PlanningUnit == county && p.FiscalYear == FiscalYear).Any()){
                    countiesWithoutPlan.Add(county);
                }
            }
            return new OkObjectResult( countiesWithoutPlan.OrderBy(c => c.Name) );
        }


        [HttpPost]
        [Authorize]
        public IActionResult Add( [FromBody] AffirmativeActionPlanRevision plan){

            if(plan != null){
                var p = this.context.
                            AffirmativeActionPlan.
                            
                            //Where(a => a.PlanningUnit == this.CurrentPlanningUnit() && a.FiscalYear == this.fiscalYearRepo.nextFiscalYear("serviceLog")).
                            Where(a => plan.AffirmativeActionPlanId == a.Id).
                            Include(a=>a.Revisions).
                            FirstOrDefault();
                if( p== null){
                    p = new AffirmativeActionPlan();
                    p.PlanningUnit = this.CurrentPlanningUnit();
                    p.FiscalYear = this.fiscalYearRepo.nextFiscalYear("serviceLog");
                    p.Revisions = new List<AffirmativeActionPlanRevision>();
                    this.context.AffirmativeActionPlan.Add(p);
                }
                plan.Created = DateTime.Now;
                plan.CreatedBy = this.CurrentUser();
                p.Revisions.Add(plan);
                context.SaveChanges();
                this.Log(plan);
                return new OkObjectResult(plan);
            }else{
                this.Log( plan ,"AffirmativeActionPlanRevision", "Error in adding an Affirmative Action Plan Revision attempt.", "Affirmative Action Plan", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpGet("MakeupDiversityGroups")]
        public IActionResult MakeupDiversityGroups(){
            var groups = context.
                AffirmativeMakeupDiversityTypeGroup.
                Include( g=> g.Types).OrderBy(g=>g.Id);
            foreach(var group in groups){
                group.Types = group.Types.OrderBy(g=>g.Id).ToList();
            }
            return new OkObjectResult(groups);
        }

        [HttpGet("AdvisoryGroups")]
        public IActionResult AdvisoryGroups(){
            var groups = context.
                            AffirmativeAdvisoryGroupType.OrderBy(g=>g.Id).
                            ToList();
            return new OkObjectResult(groups);
        }

        [HttpGet("SummaryDiversity")]
        public IActionResult SummaryDiversity(){
            var groups = context.
                            AffirmativeSummaryDiversityType.OrderBy(g=>g.Id).
                            ToList();
            return new OkObjectResult(groups);
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
}