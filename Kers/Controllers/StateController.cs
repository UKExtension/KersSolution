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
using System.Net.Http;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class StateController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        public StateController( 
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


        [HttpGet("districts")]
        public IActionResult GetDistricts(){
            var distrcts = this.context.District.OrderBy(d => d.Description);
            return new OkObjectResult(distrcts);
        }

        [HttpGet("counties")]
        public IActionResult GetCounties(){
            var counties = this.context.PlanningUnit.Where(u => u.District != null).OrderBy(d => d.Name);
            return new OkObjectResult(counties);
        }

        [HttpGet("notcounties")]
        public IActionResult GetNotCounties(){
            var counties = this.context.PlanningUnit.Where(u => u.District == null).OrderBy(d => d.Name);
            return new OkObjectResult(counties);
        }

        [HttpGet("populationByCounty")]
        public IActionResult PopulationByCounty(){


            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            String uri = "http://api.census.gov/data/2016/pep/population?get=POP,GEONAME&for=county:*&in=state:21&DATE=9";



            var result = client.GetAsync(uri).Result;
            var data = result.Content.ReadAsStringAsync().Result;

            return new OkObjectResult(data);
        }

        
        [HttpPost("addGeoFearure/{id}")]
        [Authorize]
        public IActionResult AddGeoFeature( int id, [FromBody] Object feature){
            

            var strng = JsonConvert.SerializeObject(feature,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });

            var unit = this.context.PlanningUnit.Where( u => u.Id == id).FirstOrDefault();
            if(unit != null){
                unit.GeoFeature = strng;
                this.context.SaveChanges();
                return new OkObjectResult(unit);
            }
            
            return new StatusCodeResult(500);
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