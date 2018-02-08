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
    public class SnapedController : BaseController
    {
        private IFiscalYearRepository fiscalYearRepo;

        public SnapedController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IFiscalYearRepository fiscalYearRepo
            ):base(mainContext, context, userRepo){
                this.fiscalYearRepo = fiscalYearRepo;
        }

        [HttpGet("reportedHours/{userId?}")]
        [Authorize]
        public IActionResult ReportedHours(int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var hours = context.Activity.
                                Where(e=>e.KersUser.Id == userId && e.Revisions.OrderBy(r => r.Created.ToString("s")).Last().isSnap ).
                                Sum( h => Math.Floor(h.Hours));
            return new OkObjectResult(hours);
            
        }
        [HttpGet("copies/{userId?}")]
        [Authorize]
        public IActionResult Copies(int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var hours = context.Activity.
                                Where(e=>e.KersUser.Id == userId && e.Revisions.OrderBy(r => r.Created).Last().isSnap ).
                                Include( s => s.Revisions);

            var sum = 0;
            foreach( var sn in hours){
                sum += sn.Revisions.OrderBy( r => r.Created).Last().SnapCopies;
            }
                                //Sum( h => h.Revisions.OrderBy(r => r.Created).Last().SnapCopies);
            return new OkObjectResult(sum);
            
        }

        [HttpGet("committed/{userId?}")]
        [Authorize]
        public IActionResult Committed(int userId = 0){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = context.KersUser.Find(userId);
            }
            var committed = context.SnapEd_Commitment.
                                Where(e =>  e.KersUserId == user.classicReportingProfileId 
                                            &&
                                            e.SnapEd_ActivityType.Measurement == "Hour"
                                        ).
                                Sum( h => h.Amount);
            if(committed == null) committed = 0;
            return new OkObjectResult(committed);
        }

        [HttpGet("commitments/{userId?}")]
        [Authorize]
        public IActionResult Commitments(int userId = 0){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = context.KersUser.Find(userId);
            }
            var committed = context.SnapEd_Commitment.
                                Where(e=>   e.KersUserId == user.classicReportingProfileId 
                                            &&
                                            e.SnapEd_ActivityType.Measurement == "Hour");
            return new OkObjectResult(committed);
        }








        [HttpGet("latest/{skip?}/{amount?}/{userId?}")]
        [Authorize]
        public IActionResult Get(int skip = 0, int amount = 10, int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var lastActivities = context.Activity.
                                Where(e=>e.KersUser.Id == userId && e.Revisions.OrderBy(r => r.Created).Last().isSnap && e.ActivityDate > new DateTime(2017, 10, 1) ).
                                OrderByDescending(e=>e.Revisions.OrderBy( a => a.Created.ToString("s")).Last().ActivityDate).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionSelections).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionNumbers).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).ThenInclude(r => r.Race).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).ThenInclude(r => r.Ethnicity).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapDirect).ThenInclude(r => r.SnapDirectAgesAudienceValues).ThenInclude(r => r.SnapDirectAudience).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapDirect).ThenInclude(r => r.SnapDirectAgesAudienceValues).ThenInclude(r => r.SnapDirectAges).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapIndirect).ThenInclude( r => r.SnapIndirectMethodSelections).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapIndirect).ThenInclude( r => r.SnapIndirectReachedValues).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapPolicy).ThenInclude( r => r.SnapPolicyAimedSelections).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapPolicy).ThenInclude( r => r.SnapPolicyPartnerValue).ThenInclude( r => r.SnapPolicyPartner).
                                Skip(skip).
                                Take(amount);
            
            var revs = new List<ActivityRevision>();
            if(lastActivities != null){
                foreach(var activity in lastActivities){
                    revs.Add( activity.Revisions.OrderBy(r=>r.Created).Last() );
                }
                foreach( var a in revs){
                    a.RaceEthnicityValues = a.RaceEthnicityValues.
                                                OrderBy(r => r.Race.Order).
                                                ThenBy(e => e.Ethnicity.Order).
                                                ToList();
                    if(a.SnapDirect != null){
                        a.SnapDirect.SnapDirectAgesAudienceValues = a.SnapDirect.SnapDirectAgesAudienceValues.
                                                OrderBy(r => r.SnapDirectAudience.order).
                                                ThenBy( r => r.SnapDirectAges.order).
                                                ToList();
                    }
                    if(a.SnapPolicy != null){
                        a.SnapPolicy.SnapPolicyPartnerValue = a.SnapPolicy.SnapPolicyPartnerValue.
                                                OrderBy(r => r.SnapPolicyPartner.order).
                                                ToList();
                    }
                }
            }
            return new OkObjectResult(revs);
        }


        [HttpGet("numb/{userId?}")]
        [Authorize]
        public IActionResult Numb(int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var lastActivities = context.Activity.
                                Where(e=>e.KersUser.Id == userId && e.Revisions.OrderBy(r => r.Created.ToString("s")).Last().isSnap && e.ActivityDate > new DateTime(2017, 10, 1) );
            
            
            return new OkObjectResult(lastActivities.Count());
        }





        [HttpGet("userstats/{userId}")]
        [Authorize]
        public IActionResult Userstats(int userId){

            var fiscalYear = this.fiscalYearRepo.currentFiscalYear("snapEd");
            
            var numPerMonth = context.Activity.
                                Where( e=>
                                        e.KersUser.Id == userId 
                                        //&&  e.Revisions.OrderBy(r => r.Created).Last().isSnap 
                                        &&
                                        e.ActivityDate < fiscalYear.End
                                        &&
                                        e.ActivityDate > fiscalYear.Start
                                ).
                                GroupBy(e => new {
                                    Month = e.ActivityDate.Month,
                                    Year = e.ActivityDate.Year
                                }).
                                Select(c => new {
                                    Ids = c.Select(
                                        s => s.Id
                                    ),
                                    Hours = c.Sum(s => s.Hours),
                                    Audience = c.Sum(s => s.Audience),
                                    Month = c.Key.Month,
                                    Year = c.Key.Year
                                }).
                                OrderByDescending(e => e.Year).ThenByDescending(e => e.Month).ToList();
            var result = new List<PerMonthActivities>();

            foreach(var mnth in numPerMonth){
                var MntRevs = new List<ActivityRevision>();
                float hours = 0;
                var audience = 0;
                foreach(var rev in mnth.Ids){
                    var lstrvsn = context.ActivityRevision.
                            Where(r => r.ActivityId == rev).
                            Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                            Include(a => a.SnapDirect).
                            Include(a => a.SnapPolicy).
                            Include(a => a.SnapIndirect).
                            OrderBy(a => a.Created).Last();
                    if( lstrvsn.isSnap ){
                        hours += lstrvsn.Hours;
                        audience += lstrvsn.Male + lstrvsn.Female;
                        MntRevs.Add(lstrvsn);
                    }
                }


                var actvts = new PerMonthActivities();
                actvts.Revisions = MntRevs;
                actvts.Hours = hours;
                actvts.Audience = audience;
                actvts.Month = mnth.Month;
                actvts.Year = mnth.Year;
                result.Add(actvts);
                
            }
            return new OkObjectResult(result);
        }

        
        [HttpGet("activitytypes")]
        public IActionResult ActivityTypes(){

            var committed = context.SnapEd_ActivityType;
            return new OkObjectResult(committed);
        }
        [HttpGet("projecttypes")]
        public IActionResult ProjectTypes(){

            var committed = context.SnapEd_ProjectType;
            return new OkObjectResult(committed);
        }


        /****************************** */
        // State Stats
        /****************************** */
        

        [HttpGet("committedhourscounty/{id}")]
        public IActionResult CommittedHoursCounty(int id){

            var committed = context.SnapEd_Commitment.
                                Where(e=>   e.FiscalYear == this.fiscalYearRepo.currentFiscalYear("snapEd") 
                                            && 
                                            e.KersUser.RprtngProfile.PlanningUnitId == id
                                            &&
                                            e.SnapEd_ActivityType.Measurement == "Hour"    
                                        ).
                                Sum( h => h.Amount);
            if(committed == null) committed = 0;
            return new OkObjectResult(committed);
        }

        [HttpGet("reportedhourscounty/{id}")]
        public IActionResult ReportedHoursCounty(int id){
            var fiscalYear = this.fiscalYearRepo.currentFiscalYear("snapEd");
            var hours = context.Activity.
                                Where( e=> e.ActivityDate > fiscalYear.Start && e.ActivityDate < fiscalYear.End  && e.Revisions.OrderBy(r => r.Created).Last().isSnap && e.KersUser.RprtngProfile.PlanningUnitId == id).
                                Sum( h => Math.Floor(h.Hours));
            return new OkObjectResult(hours);
        }

        [HttpGet("committedcounty/{id}")]
        public IActionResult CommittedCounty(int id){

            var committed = context.SnapEd_Commitment.
                                Where( e=> 
                                            e.FiscalYear == this.fiscalYearRepo.currentFiscalYear("snapEd") 
                                            &&
                                            e.KersUser.RprtngProfile.PlanningUnitId == id
                                            &&
                                            e.SnapEd_ActivityType.Measurement == "Hour"
                                        );
            return new OkObjectResult(committed);
        }

        [HttpGet("reportedcounty/{id}")]
        public IActionResult ReportedCounty(int id){
            var fiscalYear = this.fiscalYearRepo.currentFiscalYear("snapEd");
            
            var numPerMonth = context.Activity.
                                Where( e=>
                                        e.KersUser.RprtngProfile.PlanningUnitId == id 
                                        //&&  e.Revisions.OrderBy(r => r.Created).Last().isSnap 
                                        &&
                                        e.ActivityDate < fiscalYear.End
                                        &&
                                        e.ActivityDate > fiscalYear.Start
                                ).
                                GroupBy(e => new {
                                    Month = e.ActivityDate.Month,
                                    Year = e.ActivityDate.Year
                                }).
                                Select(c => new {
                                    Ids = c.Select(
                                        s => s.Id
                                    ),
                                    //Hours = c.Sum(s => s.Hours),
                                    //Audience = c.Sum(s => s.Audience),
                                    Month = c.Key.Month,
                                    Year = c.Key.Year
                                }).
                                OrderByDescending(e => e.Year).ThenByDescending(e => e.Month).ToList();
            var result = new List<PerMonthActivities>();

            foreach(var mnth in numPerMonth){
                float hours = 0;
                var audience = 0;
                var MntRevs = new List<ActivityRevision>();
                foreach(var rev in mnth.Ids){
                    var lstrvsn = context.ActivityRevision.
                            Where(r => r.ActivityId == rev).
                            Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                            Include(a => a.SnapDirect).
                            Include(a => a.SnapPolicy).
                            Include(a => a.SnapIndirect).
                            OrderBy(a => a.Created).Last();
                    if(lstrvsn.isSnap){
                        hours += lstrvsn.Hours;
                        audience += lstrvsn.Male + lstrvsn.Female;
                        MntRevs.Add(lstrvsn);
                    }
                }


                var actvts = new PerMonthActivities();
                actvts.Revisions = MntRevs;
                actvts.Hours = hours;
                actvts.Audience = audience;
                actvts.Month = mnth.Month;
                actvts.Year = mnth.Year;
                result.Add(actvts);
                
            }
            return new OkObjectResult(result);
        }


        

    }
}