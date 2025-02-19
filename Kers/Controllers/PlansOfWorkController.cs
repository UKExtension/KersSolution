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
    public class PlansOfWorkController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IMemoryCache memoryCache;
        public PlansOfWorkController( 
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

        [HttpGet()]
        public IActionResult Get(){
            return NotFound(new {Error = "not found"});
        }

        [HttpGet("planforrevision/{id}")]
        public IActionResult GetPlanForRevision(int id){
            var plan = this.context.PlanOfWorkRevision.Find(id);
            if(plan==null){
                return NotFound(new {Error = "Plan of Work Revision not Found"});
            }
            var parent = this.context.PlanOfWork.
                            Where(p=>p.Id == plan.PlanOfWorkId).
                            Include(w=>w.FiscalYear).
                            Include(w=>w.PlanningUnit).
                            Include(w=>w.Revisions).ThenInclude(r=>r.By).ThenInclude(p=>p.PersonalProfile).
                            FirstOrDefault();
            return new OkObjectResult(parent);
            
        }




        [HttpGet("noplanscounties/{Id?}/{fy?}/{type?}")]
        [Authorize]
        public IActionResult CountiesWithoutPlans(int Id = 0, string fy = "0", string type = "district"){
            FiscalYear fiscalYear;
            if(fy != "0"){
                fiscalYear = fiscalYearRepo.byName(fy, FiscalYearType.ServiceLog);
            }else{
                fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.ServiceLog);
            }

            var plansOfWorkPerFiscalYear = this.context.PlanOfWork.Where(p => p.FiscalYear == fiscalYear);
            if( Id != 0 ){
                if(type == "area"){
                    // Find the area
                    var area = this.context.ExtensionArea.Where( a => a.Id == Id ).FirstOrDefault();
                    var AreaController = new ExtensionAreaController(mainContext,context,userRepo, memoryCache);
                    var pairing = AreaController.FindContainingPair( area.Name );
                    plansOfWorkPerFiscalYear = plansOfWorkPerFiscalYear.Where( p => pairing.Contains(p.PlanningUnit.ExtensionArea.Name));
                }else if(type == "region"){
                    plansOfWorkPerFiscalYear = plansOfWorkPerFiscalYear.Where( p => p.PlanningUnit.ExtensionArea.ExtensionRegionId == Id);
                }else{
                    plansOfWorkPerFiscalYear = plansOfWorkPerFiscalYear.Where( p => p.PlanningUnit.DistrictId == Id);
                }
            }

            var plansPerCounty = plansOfWorkPerFiscalYear
                                    .Include(p => p.PlanningUnit)
                                    .ToList()
                                    .GroupBy( p => p.PlanningUnit )
                                    .Select( p =>
                                        new {
                                            county = p.Key,
                                            plans = p.Select( a => a )
                                        }                
                                    ).ToList();
            var countiesWithoutPlan = new List<PlanningUnit>();
            foreach( var cnt in plansPerCounty){
                var editedCount = 0;
                foreach( var plan in cnt.plans ){
                    if( this.context.PlanOfWorkRevision.Where(r => r.PlanOfWorkId == plan.Id ).Count() > 1 ){
                        editedCount++;
                    }
                }
                if(editedCount == 0){
                    countiesWithoutPlan.Add(cnt.county);
                }
            }
            
            return new OkObjectResult(countiesWithoutPlan.OrderBy( c => c.Name));
        }


        [HttpGet("All/{fy?}")]
        [Authorize]
        public IActionResult AllPlans(string fy = "0"){
            FiscalYear fiscalYear;
            if(fy != "0"){
                fiscalYear = fiscalYearRepo.byName(fy, FiscalYearType.ServiceLog);
            }else{
                fiscalYear = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
            }
            var unit = this.CurrentPlanningUnit();
            var lastRevisions = new List<PlanOfWorkRevision>();
            var plans = this.context.
                            PlanOfWork.Where( m=>m.PlanningUnit == unit && m.FiscalYear == fiscalYear).
                            //Include(p=>p.Revisions).ThenInclude(r=>r.Map).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp1).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp2).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp3).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp4).
                            Include(p=>p.Revisions).ThenInclude( r => r.PlanOfWorkDataSourceSelections).ThenInclude(r=>r.PlanOfWorkDataSource)
                            ;
            foreach(var plan in plans){
                lastRevisions.Add(plan.Revisions.OrderBy( r=>r.Created ).Last());
            }
            return new OkObjectResult(lastRevisions);
        }

        [HttpGet("AllDetails/{id?}/{fy?}")]
        [Authorize]
        public IActionResult AllPlansWithDetails(int? id, string fy = "0"){
            FiscalYear fiscalYear;
            if(fy != "0"){
                fiscalYear = fiscalYearRepo.byName(fy, FiscalYearType.ServiceLog);
            }else{
                fiscalYear = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
            }
            if(id == null || id == 0){
                var unit = this.CurrentPlanningUnit();
                id = unit.Id;
            }
            var lastRevisions = new List<PlanOfWorkRevision>();
            var plans = this.context.
                            PlanOfWork.Where( m=>m.PlanningUnit.Id == id && m.FiscalYear == fiscalYear).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Map).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp1).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp2).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp3).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp4).
                            Include(p=>p.Revisions).ThenInclude(r=>r.By)
                            ;
            foreach(var plan in plans){
                lastRevisions.Add(plan.Revisions.OrderBy( r=>r.Created ).Last());
            }
            return new OkObjectResult(lastRevisions);
        }

        [HttpGet("mapsall/{fy?}")]
        [Authorize]
        public IActionResult AllMaps(string fy = "0"){
            var unit = this.CurrentPlanningUnit();
            FiscalYear fiscalYear;
            if(fy != "0"){
                fiscalYear = fiscalYearRepo.byName(fy, FiscalYearType.ServiceLog);
            }else{
                fiscalYear = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
            }
            var maps = this.context.Map.Where(m=>m.PlanningUnit == unit && m.FiscalYear == fiscalYear);
            return new OkObjectResult(maps);
        }


        [HttpGet("maphasplan/{id}")]
        [Authorize]
        public IActionResult MapHasPlan(int id){
            var has = true;
            var revs = context.PlanOfWorkRevision.
                        Where(r => r.Map.Id == id ).
                        FirstOrDefault();
            if(revs == null){
                has = false;
            }
            return new OkObjectResult(has);
        }

        [HttpGet("datasources")]
        [Authorize]
        public IActionResult DataSources(){

            var sources = context.PlanOfWorkDataSource.
                        Where(r => r.Active ).OrderBy(r => r.Order);

            return new OkObjectResult(sources);
        }

        /***************************************************/
        /*               Plan CRUD operations               */
        /***************************************************/

        [HttpPost("{fyId?}")]
        [Authorize]
        public IActionResult AddPlan([FromBody] PlanOfWorkRevision plan, string fyId = "0"){

            if(plan != null){
                var thePlan = new PlanOfWork();
                FiscalYear fiscalYear;
                if(fyId != "0"){
                    fiscalYear = fiscalYearRepo.byName(fyId, FiscalYearType.ServiceLog);
                }else{
                    fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.ServiceLog);
                }
                thePlan.FiscalYear = fiscalYear;
                thePlan.PlanningUnit = this.CurrentPlanningUnit();
                thePlan.Revisions = new List<PlanOfWorkRevision>();

                plan.Created = DateTime.Now;
                plan.By = this.CurrentUser();
                //plan.Map = this.context.Map.Find(plan.Map.Id);
                if(plan.Mp1 != null){
                    var mp1 = this.context.MajorProgram.Find(plan.Mp1.Id);
                    plan.Mp1 = mp1;
                }
                if(plan.Mp2 != null){
                    var mp2 = this.context.MajorProgram.Find(plan.Mp2.Id);
                    plan.Mp2 = mp2;
                }
                if(plan.Mp3 != null){
                    var mp3 = this.context.MajorProgram.Find(plan.Mp3.Id);
                    plan.Mp3 = mp3;
                }
                if(plan.Mp4 != null){
                    var mp4 = this.context.MajorProgram.Find(plan.Mp4.Id); 
                    plan.Mp4 = mp4;
                }
                thePlan.Revisions.Add(plan);

                this.context.PlanOfWork.Add(thePlan);
                context.SaveChanges();

                this.Log(plan,"PlanOfWorkRevision", "Plan of Work Revision Added.");

                return new OkObjectResult(plan);
            }else{
                this.Log( plan ,"PlanOfWorkRevision", "Error in adding a  Plan of Work attempt.", "Plan of Work", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeletePlan( int id){
            var entity = context.PlanOfWorkRevision.Find(id);
            var thePlan = context.PlanOfWork.Find(entity.PlanOfWorkId);
            if(thePlan != null){
                
                context.PlanOfWork.Remove(thePlan);
                context.SaveChanges();
                
                this.Log(entity,"PlanOfWorkRevision", "Plan of Work Removed.");

                return new OkResult();
            }else{
                this.Log( entity ,"PlanOfWorkRevision", "Not Found Plan of Work in an delete attempt.", "Plan of Work", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdatePlan( int id, [FromBody] PlanOfWorkRevision plan){
            var entity = context.PlanOfWorkRevision.Find(id);
            var thePlan = context.PlanOfWork.Find(entity.PlanOfWorkId);

            if(plan != null && thePlan != null){


                plan.Created = DateTime.Now;
                plan.By = this.CurrentUser();
                plan.Map = this.context.Map.Find(plan.Map.Id);
                if(plan.Mp1 != null){
                    var mp1 = this.context.MajorProgram.Find(plan.Mp1.Id);
                    plan.Mp1 = mp1;
                }
                if(plan.Mp2 != null){
                    var mp2 = this.context.MajorProgram.Find(plan.Mp2.Id);
                    plan.Mp2 = mp2;
                }
                if(plan.Mp3 != null){
                    var mp3 = this.context.MajorProgram.Find(plan.Mp3.Id);
                    plan.Mp3 = mp3;
                }
                if(plan.Mp4 != null){
                    var mp4 = this.context.MajorProgram.Find(plan.Mp4.Id); 
                    plan.Mp4 = mp4;
                }
                thePlan.Revisions.Add(plan);

                context.SaveChanges();



                this.Log( plan ,"PlanOfWorkRevision", "Plan of Work Map Updated.");
                return new OkObjectResult(plan);
            }else{
                this.Log( plan ,"PlanOfWorkRevision", "Not Found Plan of Work in an update attempt.", "Plan of Work", "Error");
                return new StatusCodeResult(500);
            }
        }



        /***************************************************/
        /*               MAP CRUD operations               */
        /***************************************************/

        [HttpPost("map/{fyId?}")]
        [Authorize]
        public IActionResult AddMap( [FromBody] Map map, string fyId = "0"){

            if(map != null){

                var user = this.CurrentUser();
                var profile = this.CurrentProfile();

                var planningUnt = this.context.PlanningUnit.
                    Where( p=>p.Code == profile.planningUnitID).
                    FirstOrDefault();
                map.PlanningUnit = planningUnt;
                /* 
                FiscalYear fiscalYear;
                if(fyId != "0"){
                    fiscalYear = fiscalYearRepo.byName(fyId, FiscalYearType.ServiceLog);
                }else{
                    fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.ServiceLog);
                }
                map.FiscalYear = fiscalYear; */
                map.By = user;
                map.Updated = DateTime.Now;
                context.Map.Add(map);
                context.SaveChanges();

                this.Log(map,"Map", "Plan of Work Map Added.");

                return new OkObjectResult(map);
            }else{
                return new StatusCodeResult(500);
            }
        }


        [HttpDelete("map/{id}")]
        public IActionResult DeleteMap( int id){
            var entity = context.Map.Find(id);
            
            if(entity != null){
                
                context.Map.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"Map", "Plan of Work Map Removed.");

                return new OkResult();
            }else{
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("map/{id}")]
        public IActionResult UpdateMap( int id, [FromBody] Map map){
            var entity = context.Map.Find(id);

            if(map != null && entity != null){
                entity.Title = map.Title;
                context.SaveChanges();
                this.Log(entity,"Map", "Plan of Work Map Updated.");
                return new OkObjectResult(entity);
            }else{
                return new StatusCodeResult(500);
            }
        }

        private Map MatchMap( Map toMatch, List<Map> available, int UnitId, int FiscalYearId){
            var isAvailable = available.Where( a => a.Title == toMatch.Title).FirstOrDefault();
            if( isAvailable != null) return isAvailable;
            Map newMap = new Map();
            newMap.FiscalYearId = FiscalYearId;
            newMap.PlanningUnitId = UnitId;
            newMap.Title = toMatch.Title;
            newMap.Updated = DateTime.Now;
            newMap.Code = toMatch.Code;
            newMap.ById = toMatch.ById;
            available.Add( newMap );
            return newMap;
        }

        private MajorProgram MatchProgram( int? toMatchId, FiscalYear FiscalYearTo){
            if( toMatchId != null){
                var to = context.MajorProgram.Where( p => p.Id == toMatchId).FirstOrDefault();
                if( to != null){
                    return context.MajorProgram.Where( p => p.PacCode == to.PacCode && p.StrategicInitiative.FiscalYear == FiscalYearTo).FirstOrDefault();
                }
            }
            return null;
        }



        [HttpGet("importplans/{fyFrom?}/{fyTo?}")]
        public async Task<IActionResult> ImportPlans(string fyFrom = "0", string fyTo = "0"){
            FiscalYear fiscalYearFrom;
            if(fyFrom != "0"){
                fiscalYearFrom = fiscalYearRepo.byName(fyFrom, FiscalYearType.ServiceLog);
            }else{
                fiscalYearFrom = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
            }
            FiscalYear fiscalYearTo;
            if(fyTo != "0"){
                fiscalYearTo = fiscalYearRepo.byName(fyTo, FiscalYearType.ServiceLog);
            }else{
                fiscalYearTo = fiscalYearRepo.nextFiscalYear(FiscalYearType.ServiceLog);
            }
            if( this.context.PlanOfWork.Where( p => p.FiscalYear == fiscalYearTo).Any()){
                return NotFound(new {Error = "There are already plans of work present for the target fiscal year"});
            }
            var newPlans = new List<PlanOfWork>();
            var counties = context.PlanningUnit.Where( u => u.DistrictId != null);
            foreach( var county in counties){
                var newMaps = new List<Map>();
                var plans = this.context.PlanOfWork
                                    .Where( p => p.FiscalYear == fiscalYearFrom && p.PlanningUnit == county)
                                    .Include( p => p.Revisions);
                foreach( var plan in await plans.ToListAsync()){
                    var newPlan = new PlanOfWork();
                    newPlan.PlanningUnitId = plan.PlanningUnitId;
                    newPlan.FiscalYear = fiscalYearTo;
                    newPlan.Revisions = new List<PlanOfWorkRevision>();
                    var oldLastRevision = await context.PlanOfWorkRevision
                                            .Where( r => r.PlanOfWork == plan)
                                            .OrderByDescending( r => r.Created)
                                            .Include( r => r.Map)
                                            .FirstOrDefaultAsync();
                    var newRevision = new PlanOfWorkRevision();
                    newRevision.Map = this.MatchMap(oldLastRevision.Map, newMaps, county.Id, fiscalYearTo.Id);
                    newRevision.Mp1 = MatchProgram( oldLastRevision.Mp1Id, fiscalYearTo);
                    newRevision.Mp2 = MatchProgram( oldLastRevision.Mp2Id, fiscalYearTo);
                    newRevision.Mp3 = MatchProgram( oldLastRevision.Mp3Id, fiscalYearTo);
                    newRevision.Mp4 = MatchProgram( oldLastRevision.Mp4Id, fiscalYearTo);
                    newRevision.Title = oldLastRevision.Title;
                    newRevision.AgentsInvolved = oldLastRevision.AgentsInvolved;
                    newRevision.Situation = oldLastRevision.Situation;
                    newRevision.LongTermOutcomes = oldLastRevision.LongTermOutcomes;
                    newRevision.InitialOutcomes = oldLastRevision.InitialOutcomes;
                    newRevision.IntermediateOutcomes = oldLastRevision.IntermediateOutcomes;
                    newRevision.Learning = oldLastRevision.Learning;
                    newRevision.Evaluation = oldLastRevision.Evaluation;
                    newRevision.Created = DateTime.Now;
                    newRevision.ById = oldLastRevision.ById;
                    newPlan.Revisions.Add(newRevision);
                    newPlans.Add(newPlan);
                }
            }

/*
            var counties = context.PlanningUnit.Where( u => u.DistrictId != null);
            foreach( var county in counties){
                //var county = this.context.PlanningUnit.Where( u => u.Id == 173).FirstOrDefault();
                //Do not import if already something is imported
                if( !this.context.PlanOfWork.Where( p => p.FiscalYear == fiscalYearTo && p.PlanningUnitId == county.Id).Any()){
                    if( !this.context.Map.Where( m => m.FiscalYear == fiscalYearTo && m.PlanningUnitId == county.Id).Any()){
                        
                         
                        var maps = context.Map.AsNoTracking().Where(
                                        m =>
                                            m.FiscalYear.Id == fiscalYearFrom.Id
                                            &&
                                            m.PlanningUnitId == county.Id
                                );

                        List<PlanOfWorkRevision>  lastPlanRevisions = await context.PlanOfWork.AsNoTracking()
                                                .Where( p => p.FiscalYear == fiscalYearFrom
                                                            &&
                                                            p.PlanningUnitId == county.Id
                                                )
                                                .Select(p => p.Revisions.OrderBy( c => c.Created).Last() ).ToListAsync();

                        foreach( var map in maps ){
                            var revs = lastPlanRevisions.Where( r => r.MapId == map.Id).ToList();
                            if( revs.Count() != 0){
                                map.FiscalYear = fiscalYearTo;
                                map.FiscalYearId = map.FiscalYear.Id;
                                map.Id = 0;


                                var nmp = new Map();
                                nmp.PlanningUnitId = map.PlanningUnitId;
                                nmp.Code = map.Code;
                                nmp.ById = map.ById;
                                nmp.FiscalYearId = fiscalYearTo.Id;
                                nmp.Title = map.Title;
                                nmp.Updated = DateTime.Now;



                                newMaps.Add(nmp);
                                
                                foreach( var revision in revs ){
                                    var plan = new PlanOfWork();
                                    plan.FiscalYear = fiscalYearTo;
                                    plan.PlanningUnitId = context.PlanOfWork.Find( revision.PlanOfWorkId ).PlanningUnitId;
                                    plan.Revisions = new List<PlanOfWorkRevision>();
                                    revision.Id = 0;
                                    revision.Map = nmp;
                                    revision.Created = DateTime.Now;
                                    if( revision.Mp1Id != null){
                                        var pacCode = context.MajorProgram.Find(revision.Mp1Id).PacCode;
                                        revision.Mp1 = await context.MajorProgram.Where( 
                                                                    m => 
                                                                        m.PacCode == pacCode 
                                                                        &&
                                                                        m.StrategicInitiative.FiscalYear == fiscalYearTo
                                                                    ).FirstOrDefaultAsync();
                                        if(revision.Mp1 != null){
                                            revision.Mp1Id = revision.Mp1.Id;
                                        }else{
                                            revision.Mp1Id = null;
                                        }
                                            
                                    }
                                    if( revision.Mp2Id != null){
                                        var pacCode = context.MajorProgram.Find(revision.Mp2Id).PacCode;
                                        revision.Mp2 = await context.MajorProgram.Where( 
                                                                    m => 
                                                                        m.PacCode == pacCode 
                                                                        &&
                                                                        m.StrategicInitiative.FiscalYear == fiscalYearTo
                                                                    ).FirstOrDefaultAsync();
                                        if(revision.Mp2 != null){
                                            revision.Mp2Id = revision.Mp2.Id;
                                        }else{
                                            revision.Mp2Id = null;
                                        }
                                        
                                    }
                                    if( revision.Mp3Id != null){
                                        var pacCode = context.MajorProgram.Find(revision.Mp3Id).PacCode;
                                        revision.Mp3 = await context.MajorProgram.Where( 
                                                                    m => 
                                                                        m.PacCode == pacCode 
                                                                        &&
                                                                        m.StrategicInitiative.FiscalYear == fiscalYearTo
                                                                    ).FirstOrDefaultAsync();
                                        if(revision.Mp3 != null){
                                            revision.Mp3Id = revision.Mp3.Id;
                                        }else{
                                            revision.Mp3Id = null;
                                        }
                                        
                                    }
                                    if( revision.Mp4Id != null){
                                        var pacCode = context.MajorProgram.Find(revision.Mp4Id).PacCode;
                                        revision.Mp4 = await context.MajorProgram.Where( 
                                                                    m => 
                                                                        m.PacCode == pacCode 
                                                                        &&
                                                                        m.StrategicInitiative.FiscalYear == fiscalYearTo
                                                                    ).FirstOrDefaultAsync();
                                        if(revision.Mp4 != null){
                                            revision.Mp4Id = revision.Mp4.Id;
                                        }else{
                                            revision.Mp4Id = null;
                                        }
                                        
                                    }
                                    plan.Revisions.Add( revision );
                                    newPlans.Add(plan);
                                }
                            }
                            
                            
                        }

                    }
                }

                //context.Map.AddRange(newMaps);
                
                    
            }*/
            context.PlanOfWork.AddRange(newPlans);
            //context.SaveChanges();
            if( newPlans.Count() > 0)  return new OkObjectResult( newPlans );
            return NotFound(new {Error = "no need to import plans"});
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

        private void Log(   object obj, 
                            string objectType = "PlanOfWorkRevision",
                            string description = "Plan Of Work", 
                            string type = "Plan Of Work",
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
            var profile = this.context.ReportingProfile.
                            Where(p=> p.LinkBlueId == u).
                            FirstOrDefault();
            return  this.context.PlanningUnit.
                    Where( p => p.Id == profile.PlanningUnitId).
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