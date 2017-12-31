using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERS_SNAPED2017;
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
    public class SnapClassicController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        KERS_SNAPED2017Context snapContext;
        public SnapClassicController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    KERS_SNAPED2017Context snapContext,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo
            ){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.logRepo = logRepo;
           this.snapContext = snapContext;
        }


        

        [HttpGet("{id}")]
        public IActionResult Get(int id){
            var snp = this.snapContext.zSnapEdActivities.Find(id);
            
            return new OkObjectResult(snp);
        }

        [HttpPost()]
        [Authorize]
        public IActionResult AddSnap( [FromBody] zSnapEdActivity activity){
            if(activity != null){
                
                var user = this.CurrentUser();
                

                activity.rDT = DateTime.Now;
                activity.FY = 2017;
                activity.instID = user.RprtngProfile.Institution.Code;
                activity.planningUnitID = user.RprtngProfile.PlanningUnit.Code;
                activity.planningUnitName = user.RprtngProfile.PlanningUnit.Name;
                activity.personID = user.RprtngProfile.PersonId;
                activity.personName = user.RprtngProfile.Name;


                snapContext.Add(activity); 
                
                //this.Log(activity,"ActivityRevision", "Activity Added.");
                snapContext.SaveChanges();
                
                return new OkObjectResult(activity);
            }else{
                this.Log( activity ,"ActivityRevision", "Error in adding Activity attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateActivity( int id, [FromBody] zSnapEdActivity activity){
           
            var entity = snapContext.zSnapEdActivities.Find(id);

            if(activity != null && entity != null){
                entity.rDT = DateTime.Now;
                entity.snapDate = activity.snapDate;
                entity.snapHours = activity.snapHours;
                entity.snapCopies = activity.snapCopies;

                //Direct Contact
                if(entity.snapModeID == 1){
                    entity.snapDirectDeliverySiteID = activity.snapDirectDeliverySiteID;
                    entity.snapDirectSpecificSiteName = activity.snapDirectSpecificSiteName;
                    entity.snapDirectSessionTypeID = activity.snapDirectSessionTypeID;
                    entity.snapDirectAudience_00_04_FarmersMarket = activity.snapDirectAudience_00_04_FarmersMarket;
                    entity.snapDirectAudience_05_17_FarmersMarket = activity.snapDirectAudience_05_17_FarmersMarket;
                    entity.snapDirectAudience_18_59_FarmersMarket = activity.snapDirectAudience_18_59_FarmersMarket;
                    entity.snapDirectAudience_60_pl_FarmersMarket = activity.snapDirectAudience_60_pl_FarmersMarket;
                    entity.snapDirectAudience_00_04_PreSchool = activity.snapDirectAudience_00_04_PreSchool;
                    entity.snapDirectAudience_05_17_PreSchool = activity.snapDirectAudience_05_17_PreSchool;
                    entity.snapDirectAudience_18_59_PreSchool = activity.snapDirectAudience_18_59_PreSchool;
                    entity.snapDirectAudience_60_pl_PreSchool = activity.snapDirectAudience_60_pl_PreSchool;
                    entity.snapDirectAudience_00_04_Family = activity.snapDirectAudience_00_04_Family;
                    entity.snapDirectAudience_05_17_Family = activity.snapDirectAudience_05_17_Family;
                    entity.snapDirectAudience_18_59_Family = activity.snapDirectAudience_18_59_Family;
                    entity.snapDirectAudience_60_pl_Family = activity.snapDirectAudience_60_pl_Family;
                    entity.snapDirectAudience_00_04_SchoolAge = activity.snapDirectAudience_00_04_SchoolAge;
                    entity.snapDirectAudience_05_17_SchoolAge = activity.snapDirectAudience_05_17_SchoolAge;
                    entity.snapDirectAudience_18_59_SchoolAge = activity.snapDirectAudience_18_59_SchoolAge;
                    entity.snapDirectAudience_60_pl_SchoolAge = activity.snapDirectAudience_60_pl_SchoolAge;
                    entity.snapDirectAudience_00_04_LimitedEnglish = activity.snapDirectAudience_00_04_LimitedEnglish;
                    entity.snapDirectAudience_05_17_LimitedEnglish = activity.snapDirectAudience_05_17_LimitedEnglish;
                    entity.snapDirectAudience_18_59_LimitedEnglish = activity.snapDirectAudience_18_59_LimitedEnglish;
                    entity.snapDirectAudience_60_pl_LimitedEnglish = activity.snapDirectAudience_60_pl_LimitedEnglish;
                    entity.snapDirectAudience_00_04_Seniors = activity.snapDirectAudience_00_04_Seniors;
                    entity.snapDirectAudience_05_17_Seniors = activity.snapDirectAudience_05_17_Seniors;
                    entity.snapDirectAudience_18_59_Seniors = activity.snapDirectAudience_18_59_Seniors;
                    entity.snapDirectAudience_60_pl_Seniors = activity.snapDirectAudience_60_pl_Seniors;
                    entity.snapDirectGenderMale = activity.snapDirectGenderMale;
                    entity.snapDirectGenderFemale = activity.snapDirectGenderFemale;
                    entity.snapDirectRaceWhiteNonHispanic = activity.snapDirectRaceWhiteNonHispanic;
                    entity.snapDirectRaceWhiteHispanic = activity.snapDirectRaceWhiteHispanic;
                    entity.snapDirectRaceBlackNonHispanic = activity.snapDirectRaceBlackNonHispanic;
                    entity.snapDirectRaceBlackHispanic = activity.snapDirectRaceBlackHispanic;
                    entity.snapDirectRaceAsianNonHispanic = activity.snapDirectRaceAsianNonHispanic;
                    entity.snapDirectRaceAsianHispanic = activity.snapDirectRaceAsianHispanic;
                    entity.snapDirectRaceAmericanIndianNonHispanic = activity.snapDirectRaceAmericanIndianNonHispanic;
                    entity.snapDirectRaceAmericanIndianHispanic = activity.snapDirectRaceAmericanIndianHispanic;
                    entity.snapDirectRaceHawaiianNonHispanic = activity.snapDirectRaceHawaiianNonHispanic;
                    entity.snapDirectRaceHawaiianHispanic = activity.snapDirectRaceHawaiianHispanic;
                    entity.snapDirectRaceOtherNonHispanic = activity.snapDirectRaceOtherNonHispanic;
                    entity.snapDirectRaceOtherHispanic = activity.snapDirectRaceOtherHispanic;
                    
                //Indirect Contact
                }else{
                    entity.snapIndirectEstNumbReachedPsaRadio = activity.snapIndirectEstNumbReachedPsaRadio;
                    entity.snapIndirectEstNumbReachedPsaTv = activity.snapIndirectEstNumbReachedPsaTv;
                    entity.snapIndirectEstNumbReachedArticles = activity.snapIndirectEstNumbReachedArticles;
                    entity.snapIndirectEstNumbReachedGroceryStore = activity.snapIndirectEstNumbReachedGroceryStore;
                    entity.snapIndirectEstNumbReachedFairsParticipated = activity.snapIndirectEstNumbReachedFairsParticipated;
                    entity.snapIndirectEstNumbReachedFairsSponsored = activity.snapIndirectEstNumbReachedFairsSponsored;
                    entity.snapIndirectEstNumbReachedNewsletter = activity.snapIndirectEstNumbReachedNewsletter;
                    entity.snapIndirectEstNumbReachedSocialMedia = activity.snapIndirectEstNumbReachedSocialMedia;
                    entity.snapIndirectEstNumbReachedOther = activity.snapIndirectEstNumbReachedOther;
                    entity.snapIndirectMethodFactSheets = activity.snapIndirectMethodFactSheets;
                    entity.snapIndirectMethodPosters = activity.snapIndirectMethodPosters;
                    entity.snapIndirectMethodCalendars = activity.snapIndirectMethodCalendars;
                    entity.snapIndirectMethodPromoMaterial = activity.snapIndirectMethodPromoMaterial;
                    entity.snapIndirectMethodWebsite = activity.snapIndirectMethodWebsite;
                    entity.snapIndirectMethodEmail = activity.snapIndirectMethodEmail;
                    entity.snapIndirectMethodVideo = activity.snapIndirectMethodVideo;
                    entity.snapIndirectMethodOther = activity.snapIndirectMethodOther;
                }



                snapContext.SaveChanges();
                this.Log(entity,"Snap Ed Activity", "Snap Ed Activity Updated.");
                return new OkObjectResult(entity);

            }else{
                this.Log( activity ,"ActivityRevision", "Not Found Activity in an update attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            } 
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteActivity( int id ){
            var entity = context.ActivityRevision.Find(id);
            var acEntity = context.Activity.Where(a => a.Id == entity.ActivityId).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionSelections).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionNumbers).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).
                                FirstOrDefault();
            
            if(acEntity != null){
                
                context.Activity.Remove(acEntity);
                context.SaveChanges();
                
                this.Log(entity,"ActivityRevision", "Activity Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ActivityRevision", "Not Found Activity in a delete attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpGet("hours/{month}/{userId?}")]
        [Authorize]
        public IActionResult HoursForUser(int month, int userId = 0){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = this.context.KersUser.Where(u => u.Id == userId).Include( u => u.RprtngProfile).FirstOrDefault();
            }
            var snapActivities = this.snapContext.
                                zSnapEdActivities.Where(a => a.personID == user.RprtngProfile.PersonId && a.snapDate.Substring(4,2) == month.ToString("D2")).
                                GroupBy(a => a.personID).
                                Select( g => new
                                            {
                                                TotalHours = g.Sum(x => Convert.ToDouble( x.snapHours ))
                                            }).
                                FirstOrDefault();
            return new OkObjectResult(snapActivities);
        }


        [HttpGet("site")]
        public IActionResult Sites(){
            var site = this.snapContext.zzSnapEdDeliverySites.OrderBy(o => o.orderID);
            return new OkObjectResult(site);
        }

        [HttpGet("session")]
        public IActionResult Session(){
            var sess = this.snapContext.zzSnapEdSessionTypes.OrderBy(o => o.orderID);
            return new OkObjectResult(sess);
        }


        private void Log(   object obj, 
                            string objectType = "ActivityRevision",
                            string description = "Submitted Activity Revision", 
                            string type = "Activity",
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


        private PlanningUnit CurrentPlanningUnit(){
            var u = this.CurrentUserId();
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == u).
                            FirstOrDefault();
            return  this.context.PlanningUnit.
                    Where( p=>p.Code == profile.planningUnitID).
                    FirstOrDefault();
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


        private KersUser CurrentUser(){
            var u = this.CurrentUserId();
            
            var user = this.context.KersUser.
                        Where( r => r.RprtngProfile.LinkBlueId == this.CurrentUserId()).
                        Include(p => p.RprtngProfile).ThenInclude(f=>f.Institution).
                        Include(p => p.RprtngProfile).ThenInclude(f=>f.PlanningUnit).
                        FirstOrDefault();
            return user;
        }



        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }



    }
}