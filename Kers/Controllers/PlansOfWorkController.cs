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
    public class PlansOfWorkController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        public PlansOfWorkController( 
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

        [HttpGet("All")]
        [Authorize]
        public IActionResult AllPlans(){
            var unit = this.CurrentPlanningUnit();
            var lastRevisions = new List<PlanOfWorkRevision>();
            var plans = this.context.
                            PlanOfWork.Where( m=>m.PlanningUnit == unit && m.FiscalYear == this.fiscalYearRepo.currentFiscalYear("serviceLog")).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Map).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp1).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp2).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp3).
                            Include(p=>p.Revisions).ThenInclude(r=>r.Mp4)
                            ;
            foreach(var plan in plans){
                lastRevisions.Add(plan.Revisions.OrderBy( r=>r.Created ).Last());
            }
            return new OkObjectResult(lastRevisions);
        }

        [HttpGet("AllDetails/{id?}")]
        [Authorize]
        public IActionResult AllPlansWithDetails(int? id){
            if(id == null){
                var unit = this.CurrentPlanningUnit();
                id = unit.Id;
            }
            var lastRevisions = new List<PlanOfWorkRevision>();
            var plans = this.context.
                            PlanOfWork.Where( m=>m.PlanningUnit.Id == id && m.FiscalYear == this.fiscalYearRepo.currentFiscalYear("serviceLog")).
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

        [HttpGet("mapsall")]
        [Authorize]
        public IActionResult AllMaps(){
            var unit = this.CurrentPlanningUnit();
            var fiscalYear = this.fiscalYearRepo.nextFiscalYear("serviceLog");
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
                FiscalYear fiscalYear;
                if(fyId != "0"){
                    fiscalYear = fiscalYearRepo.byName(fyId, FiscalYearType.ServiceLog);
                }else{
                    fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.ServiceLog);
                }
                map.FiscalYear = fiscalYear;
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

        



        [HttpGet("importplans/{fyFrom?}/{fyTo?}")]
        public async Task<IActionResult> ImportPlans(string fyFrom = "0", string fyTo = "0"){


            FiscalYear fiscalYearFrom;
            if(fyFrom != "0"){
                fiscalYearFrom = fiscalYearRepo.byName(fyFrom, FiscalYearType.ServiceLog);
            }else{
                fiscalYearFrom = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
            }

            FiscalYear fiscalYearTo;
            if(fyFrom != "0"){
                fiscalYearTo = fiscalYearRepo.byName(fyTo, FiscalYearType.ServiceLog);
            }else{
                fiscalYearTo = fiscalYearRepo.nextFiscalYear(FiscalYearType.ServiceLog);
            }

            //Do not import if already something is imported
            if( !this.context.PlanOfWork.Where( p => p.FiscalYear == fiscalYearTo).Any()){
                if( !this.context.Map.Where( m => m.FiscalYear == fiscalYearTo).Any()){

                    var newPlans = new List<PlanOfWork>();
                    var newMaps = new List<Map>();
                    
                    var maps = context.Map.AsNoTracking().Where(
                                    m =>
                                        m.FiscalYear == fiscalYearFrom
                            );

                    List<PlanOfWorkRevision>  lastPlanRevisions = await context.PlanOfWork.AsNoTracking()
                                            .Where( p => p.FiscalYear == fiscalYearFrom)
                                            .Select(p => p.Revisions.OrderBy( c => c.Created).Last() ).ToListAsync();

                    foreach( var map in maps ){
                        var revs = lastPlanRevisions.Where( r => r.MapId == map.Id).ToList();
                        if( revs.Count() != 0){
                            map.FiscalYear = fiscalYearTo;
                            map.Id = 0;
                            newMaps.Add(map);
                            
                            foreach( var revision in revs ){
                                var plan = new PlanOfWork();
                                plan.FiscalYear = fiscalYearTo;
                                plan.PlanningUnitId = context.PlanOfWork.Find( revision.PlanOfWorkId ).PlanningUnitId;
                                plan.Revisions = new List<PlanOfWorkRevision>();
                                revision.Id = 0;
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
                    context.Map.AddRange(newMaps);
                    context.PlanOfWork.AddRange(newPlans);
                    context.SaveChanges();
                    return new OkObjectResult( newPlans );
                }
            }

            /*
            FiscalYear year = fiscalYearRepo.nextFiscalYear("serviceLog");;
            var mapsCore = context.Map;
            if(mapsCore.Count() == 0){
                var mapsKers = c2017Context.zMaps;
                
                foreach( var map in mapsKers){
                    var mapCore = new Map();
                    mapCore.Code = map.mapID;
                    mapCore.Updated = map.rDT??new DateTime();
                    mapCore.Title = map.mapTitle;
                    mapCore.By = this.userByProfileId(map.rBY);
                    mapCore.PlanningUnit = context.PlanningUnit.Where(p => p.Code == map.planningUnitID).FirstOrDefault();
                    mapsCore.Add(mapCore);
                }
                context.SaveChanges();
            }

            if(context.PlanOfWork.Count() == 0){
                var plansKers = c2017Context.zProgramPlans;
                var plansCore = context.PlanOfWork;
                
                
                foreach(var plan in plansKers){
                    var planCore = new PlanOfWorkRevision();
                    
                    planCore.Map = mapsCore.Where( c => c.Code == plan.mapID ).FirstOrDefault();

                    planCore.By = this.userByProfileId(plan.rBY);

                    planCore.Title = plan.programPlanTitle;
                    planCore.AgentsInvolved = plan.agentsInvolved;
                    planCore.Situation = Regex.Replace(plan.situation??"", @"\r\n?|\n", "<br />");
                    planCore.LongTermOutcomes = Regex.Replace(plan.longTermOutcomes??"", @"\r\n?|\n", "<br />");
                    planCore.IntermediateOutcomes = Regex.Replace(plan.intermediateOutcomes??"", @"\r\n?|\n", "<br />");
                    planCore.InitialOutcomes = Regex.Replace(plan.initialOutcomes??"", @"\r\n?|\n", "<br />");
                    planCore.Learning = Regex.Replace(plan.learning??"", @"\r\n?|\n", "<br />");
                    planCore.Evaluation = Regex.Replace(plan.evaluation??"", @"\r\n?|\n", "<br />");
                    planCore.Created = plan.rDT??new DateTime();

                    var PacCode1 = c2017Context.zzPacs.
                                        Where(c => c.pacID == plan.pacID1).
                                        FirstOrDefault();
                    if(PacCode1 != null){
                        planCore.Mp1 = context.MajorProgram.
                                Where( p => p.PacCode == PacCode1.pacCodeID).FirstOrDefault();
                    }

                    var PacCode2 = c2017Context.zzPacs.
                                        Where(c => c.pacID == plan.pacID2).
                                        FirstOrDefault();
                    if(PacCode2 != null){
                        planCore.Mp2 = context.MajorProgram.
                                Where( p => p.PacCode == PacCode2.pacCodeID).FirstOrDefault();
                    }

                    var PacCode3 = c2017Context.zzPacs.
                                        Where(c => c.pacID == plan.pacID3).
                                        FirstOrDefault();
                    if(PacCode3 != null){
                        planCore.Mp3 = context.MajorProgram.
                                Where( p => p.PacCode == PacCode3.pacCodeID).FirstOrDefault();
                    }
                    var PacCode4 = c2017Context.zzPacs.
                                        Where(c => c.pacID == plan.pacID4).
                                        FirstOrDefault();
                    if(PacCode4 != null){
                        planCore.Mp4 = context.MajorProgram.
                                Where( p => p.PacCode == PacCode4.pacCodeID).FirstOrDefault();
                    }
                    
                    //context.PlanOfWorkRevision.Add(planCore);
                    var thePlan = new PlanOfWork();
                    thePlan.Revisions = new List<PlanOfWorkRevision>();
                    thePlan.Revisions.Add(planCore);
                    thePlan.FiscalYear = year;
                    thePlan.PlanningUnit = context.PlanningUnit.Where(u=>u.Code == plan.planningUnitID).FirstOrDefault();

                    context.PlanOfWork.Add(thePlan);


                }
                context.SaveChanges();
            }
            return new OkObjectResult(context.PlanOfWork.Take(1));
            //;
             */
            return NotFound(new {Error = "no need to import plans"});
        }
/* 
        [HttpGet("importregions")]
        public IActionResult ImportRegions(){

            
            var regionsCore = context.Region;
            if(regionsCore.Count() == 0){
                var regionsMain = mainContext.zzCESregions;
                foreach(var regionMain in regionsMain){
                    var regionCore = new Region();
                    //regionCore.Id = regionMain.rID??1;
                    regionCore.Name = regionMain.rName;
                    regionCore.Admin = this.userByProfileId(regionMain.rAdminID);
                    regionsCore.Add(regionCore);
                }
                context.SaveChanges();
                var districtsMain = mainContext.zzCESdistricts;
                var districtsCore = context.District;

                foreach(var districtMain in districtsMain){
                    var districtCore = new District();
                    //districtCore.Id = districtMain.dID;
                    districtCore.Name = districtMain.dName;
                    districtCore.AreaName = districtMain.dAreaName;
                    districtCore.Admin = this.userByProfileId(districtMain.dAdminID);
                    districtCore.Assistant = this.userByProfileId(districtMain.dAsstID);
                    districtsCore.Add(districtCore);
                }
                context.SaveChanges();
                var planningUnitsMain = mainContext.zzPlanningUnits;
                var planningUnitsCore = context.PlanningUnit;
                foreach(var unit in planningUnitsMain){
                    var unitCore = new PlanningUnit();
                    unitCore.order = unit.orderID;
                    unitCore.Name = unit.planningUnitName;
                    unitCore.Code = unit.planningUnitID;
                    unitCore.ReportsExtension = unit.reportsExtension??false;
                    unitCore.District = districtsCore.Where(d => d.Id == unit.distID).FirstOrDefault();
                    unitCore.Region = regionsCore.Where(r => r.Id  == unit.regID).FirstOrDefault();
                    planningUnitsCore.Add(unitCore);
                }
                context.SaveChanges();
                return new OkObjectResult(planningUnitsCore);
            }
            return NotFound(new {Error = "not found"});
        }

 */
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