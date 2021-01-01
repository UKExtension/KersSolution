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
    public class ProgramIndicatorController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        public ProgramIndicatorController( 
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


        [HttpGet("noindicatorscounties/{id?}/{fy?}/{type?}")]
        [Authorize]
        public IActionResult CountiesWithoutIndicators(int id = 0, string fy = "0", string type = "district"){
            FiscalYear fiscalYear;
            if(fy != "0"){
                fiscalYear = fiscalYearRepo.byName(fy, FiscalYearType.ServiceLog);
            }else{
                fiscalYear = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
            }

            List<PlanningUnit> counties;

            if(id != 0){
                if(type == "area"){
                    counties = context.PlanningUnit.Where( c => c.ExtensionAreaId == id && c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
                }else if( type == "region"){
                    counties = context.PlanningUnit.Where( c => c.ExtensionArea.ExtensionRegionId == id && c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
                }else{
                    counties = context.PlanningUnit.Where( c => c.DistrictId == id && c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
                }
            }else{
                counties = context.PlanningUnit.Where( u => u.District != null && u.Name.Substring(u.Name.Count() - 3) == "CES").ToList();
            }

            var countiesWithoutIndicators = new List<PlanningUnit>();


            foreach( var county in counties ){
                if(!this.context.ProgramIndicatorValue.Where( p => p.KersUser.RprtngProfile.PlanningUnit == county && p.ProgramIndicator.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear).Any()){
                    countiesWithoutIndicators.Add(county);
                }
            }
            return new OkObjectResult( countiesWithoutIndicators.OrderBy(c => c.Name) );
        }





        [HttpGet("indicatorsforprogram/{id}")]
        public IActionResult GetIndicatorsForProgram(int id){
            var program = this.context.MajorProgram.Find(id);
            if(program==null){
                return NotFound(new {Error = "Major Program not Found"});
            }
            var indicators = this.context.ProgramIndicator.
                            Where(p=>p.MajorProgram == program).OrderBy(o=>o.order);
            return new OkObjectResult(indicators);
            
        }


        [HttpPost("{programId}")]
        [Authorize]
        public IActionResult AddIndicator(int programId, [FromBody] ProgramIndicator indicator){

            if(indicator != null){
                indicator.MajorProgramId = programId;
                this.context.ProgramIndicator.Add(indicator);
                context.SaveChanges();
                this.Log( indicator );
                return new OkObjectResult(indicator);
            }else{
                this.Log( indicator ,"ProgramIndicator", "Not Found Program Indicator in an insert attempt.", "Program Indicator", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateIndicator( int id, [FromBody] ProgramIndicator indicator){
            var entity = context.ProgramIndicator.Find(id);
            
            if(entity != null && indicator != null){

                entity.order = indicator.order;
                entity.Question = indicator.Question;

                context.SaveChanges();



                this.Log( entity );
                return new OkObjectResult(entity);
            }else{
                this.Log( indicator ,"ProgramIndicator", "Not Found Program Indicator in an update attempt.", "Program Indicator", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteIndicator( int id){
            var entity = context.ProgramIndicator.Find(id);
            
            if(entity != null){
                var values = context.ProgramIndicatorValue.Where(v=>v.ProgramIndicator == entity);
                foreach(var val in values){
                    this.context.ProgramIndicatorValue.Remove(val);
                }

                this.Log( entity, "ProgramIndicator", "Program Indicator deleted." );
                
                context.ProgramIndicator.Remove(entity);
                context.SaveChanges();
                

                return new OkResult();
            }else{
                this.Log( id ,"ProgramIndicator", "Not Found Program Indicator in an delete attempt.", "Program Indicator", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("indicatorvalues/{id}")]
        public IActionResult GetIndicatorValuesPerProgramId(int id){
            var program = this.context.MajorProgram.Find(id);
            if(program==null){
                return NotFound(new {Error = "Major Program not Found"});
            }
            var user = CurrentUser();
            user = context.KersUser.Where( u => u.Id == user.Id).Include( u => u.RprtngProfile).FirstOrDefault();
            var indicatorValues = this.context.ProgramIndicatorValue.
                            Where(p=>p.ProgramIndicator.MajorProgram == program && p.KersUser == user && p.PlanningUnitId == user.RprtngProfile.PlanningUnitId).
                            OrderBy(o=>o.ProgramIndicator.order);
            return new OkObjectResult(indicatorValues);
            
        }
        [HttpPut("valuesupdate/{id}")]
        [Authorize]
        public IActionResult ValuesUpdate(int id, [FromBody] List<ProgramIndicatorValue> indicatoValues){
            var user = CurrentUser();
            user = context.KersUser.Where( u => u.Id == user.Id).Include( u => u.RprtngProfile).FirstOrDefault();
            foreach(var val in indicatoValues){
                var v = this.context.
                            ProgramIndicatorValue.Where(l=>l.KersUser == user && l.PlanningUnitId == user.RprtngProfile.PlanningUnitId && l.ProgramIndicatorId == val.ProgramIndicatorId).
                            FirstOrDefault();
                if(v==null){
                    val.KersUser = user;
                    val.PlanningUnitId = val.KersUser.RprtngProfile.PlanningUnitId;
                    val.CreatedDateTime = DateTimeOffset.Now;
                    val.LastModifiedDateTime = DateTimeOffset.Now;
                    this.context.ProgramIndicatorValue.Add(val);
                }else{
                    v.LastModifiedDateTime = DateTimeOffset.Now;
                    v.Value = val.Value;
                }
            }
            this.context.SaveChanges();
            this.Log( indicatoValues, "ProgramIndicator", "Program Indicators Numbers Submitted." );
            return new OkObjectResult(indicatoValues);
            
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
                            string objectType = "ProgramIndicator",
                            string description = "Program Indicator Added/Updated", 
                            string type = "Program Indicator",
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