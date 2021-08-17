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
            var fiscalYear = this.fiscalYearRepo.currentFiscalYear("snapEd");
            var hours = context.Activity.
                                Where(e=>e.KersUser.Id == userId 
                                        &&
                                        e.ActivityDate < fiscalYear.End
                                        &&
                                        e.ActivityDate > fiscalYear.Start )
                                .Include( a => a.Revisions)
                                .ToList();
            var hrs = hours.Where( e => e.Revisions.OrderBy(r => r.Created.ToString("s")).Last().isSnap).Sum( h => Math.Floor(h.Hours));
            return new OkObjectResult(hrs);
            
        }
        [HttpGet("copies/{userId?}")]
        [Authorize]
        public IActionResult Copies(int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var hours = context.Activity.
                                Where(e=>e.KersUser.Id == userId ).
                                Include( s => s.Revisions).ToList();
            var hrs = hours.Where( e => e.Revisions.OrderBy(r => r.Created).Last().isSnap);
            var sum = 0;
            foreach( var sn in hrs){
                sum += sn.Revisions.OrderBy( r => r.Created).Last().SnapCopies;
            }
                                //Sum( h => h.Revisions.OrderBy(r => r.Created).Last().SnapCopies);
            return new OkObjectResult(sum);
            
        }

        [HttpGet("committed/{userId?}/{fy?}")]
        [Authorize]
        public IActionResult Committed(int userId = 0, string fy = "0"){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = context.KersUser.Find(userId);
            }
            var fiscalYear = this.GetFYByName(fy, FiscalYearType.SnapEd);
            var committed = context.SnapEd_Commitment.
                                Where(e =>  e.KersUserId == user.classicReportingProfileId 
                                            &&
                                            e.SnapEd_ActivityType.Measurement == "Hour"
                                            &&
                                            e.FiscalYear == fiscalYear
                                        ).
                                Sum( h => h.Amount);
            if(committed == null) committed = 0;
            return new OkObjectResult(committed);
        }

        [HttpGet("commitments/{userId?}/{fy?}")]
        [Authorize]
        public IActionResult Commitments(int userId = 0,  string fy = "0"){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = context.KersUser.Find(userId);
            }
            var fiscalYear = this.GetFYByName(fy, FiscalYearType.SnapEd);
            var committed = context.SnapEd_Commitment.
                                Where(e=>   e.KersUserId == user.classicReportingProfileId 
                                            &&
                                            e.SnapEd_ActivityType.Measurement == "Hour"
                                            &&
                                            e.FiscalYear == fiscalYear
                                            );
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
                                Where(e=>e.KersUser.Id == userId && e.ActivityDate > new DateTime(2017, 10, 1) ).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionSelections).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionNumbers).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).ThenInclude(r => r.Race).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).ThenInclude(r => r.Ethnicity).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapDirect).ThenInclude(r => r.SnapDirectAgesAudienceValues).ThenInclude(r => r.SnapDirectAudience).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapDirect).ThenInclude(r => r.SnapDirectAgesAudienceValues).ThenInclude(r => r.SnapDirectAges).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapIndirect).ThenInclude( r => r.SnapIndirectMethodSelections).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapIndirect).ThenInclude( r => r.SnapIndirectReachedValues).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapPolicy).ThenInclude( r => r.SnapPolicyAimedSelections).
                                Include(e=>e.Revisions).ThenInclude(r => r.SnapPolicy).ThenInclude( r => r.SnapPolicyPartnerValue).ThenInclude( r => r.SnapPolicyPartner)
                                .ToList();
            var lst = lastActivities.Where( e => e.Revisions.OrderBy(r => r.Created).Last().isSnap).
                                OrderByDescending(e=>e.Revisions.OrderBy( a => a.Created.ToString("s")).Last().ActivityDate).
                                Skip(skip).
                                Take(amount);
            var revs = new List<ActivityRevision>();
            if(lst != null){
                foreach(var activity in lst){
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
                                Where(e=>e.KersUser.Id == userId && e.ActivityDate > new DateTime(2017, 10, 1) )
                                .ToList();
            
            var withSnap = lastActivities.Where(e => e.Revisions.OrderBy(r => r.Created.ToString("s")).Last().isSnap);
            return new OkObjectResult(lastActivities.Count());
        }





        [HttpGet("userstats/{userId}/{fy?}")]
        [Authorize]
        public IActionResult Userstats(int userId, string fy="0"){

            var fiscalYear = this.GetFYByName(fy, FiscalYearType.SnapEd);



            var filteredActivities = context.Activity.
                                Where( e=>
                                        e.KersUser.Id == userId 
                                        //&&  e.Revisions.OrderBy(r => r.Created).Last().isSnap 
                                        &&
                                        e.ActivityDate < fiscalYear.End
                                        &&
                                        e.ActivityDate > fiscalYear.Start
                                ).ToList();



            var numPerMonth = filteredActivities.
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

        
        [HttpGet("activitytypes/{fiscalyearid?}")]
        public IActionResult ActivityTypes(int fiscalyearId = 0){
            FiscalYear fiscalYear;
            if(fiscalyearId == 0){
                fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.SnapEd);
            }else{
                fiscalYear = context.FiscalYear.Find(fiscalyearId);
            }

            var committed = context.SnapEd_ActivityType.Where(a => a.FiscalYear == fiscalYear);
            return new OkObjectResult(committed);
        }
        [HttpGet("projecttypes/{fiscalyearid?}")]
        public IActionResult ProjectTypes(int fiscalyearId = 0){
            FiscalYear fiscalYear;
            if(fiscalyearId == 0){
                fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.SnapEd);
            }else{
                fiscalYear = context.FiscalYear.Find(fiscalyearId);
            }


            var committed = context.SnapEd_ProjectType.Where(a => a.FiscalYear == fiscalYear);
            return new OkObjectResult(committed);
        }
        [HttpGet("reinforcementitems/{fiscalyearid?}")]
        public IActionResult ReinforcementItems(int fiscalyearId = 0){
            FiscalYear fiscalYear;
            if(fiscalyearId == 0){
                fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.SnapEd);
            }else{
                fiscalYear = context.FiscalYear.Find(fiscalyearId);
            }


            var committed = context.SnapEd_ReinforcementItem.Where(a => a.FiscalYear == fiscalYear);
            return new OkObjectResult(committed);
        }


        /****************************** */
        // State Stats
        /****************************** */
        

        [HttpGet("committedhourscounty/{id}")]
        public IActionResult CommittedHoursCounty(int id){
            var currentFiscalYear = this.fiscalYearRepo.currentFiscalYear("snapEd");
            var committed = context.SnapEd_Commitment.
                                Where(e=>   e.FiscalYear == currentFiscalYear
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
            var currentFiscalYear = this.fiscalYearRepo.currentFiscalYear("snapEd");
            var committed = context.SnapEd_Commitment.
                                Where( e=> 
                                            e.FiscalYear ==  currentFiscalYear
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