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
    public class ServicelogController : BaseController
    {

        IFiscalYearRepository fiscalYearRepo;
        private IMemoryCache _cache;
        public ServicelogController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IMemoryCache memoryCache
            ):base(mainContext, context, userRepo){

           this.fiscalYearRepo = fiscalYearRepo;
           _cache = memoryCache;
        }


        [HttpGet("latest/{skip?}/{amount?}/{userId?}")]
        [Authorize]
        public IActionResult Get(int skip = 0, int amount = 10, int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var lastActivities = context.Activity.
                                Where(e=>e.KersUser.Id == userId).
                                OrderByDescending(e=>e.ActivityDate).
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



        [HttpGet("numb")]
        [Authorize]
        public IActionResult GetNumb(){
            
            var numActivities = context.Activity.
                                Where(e=>e.KersUser == this.CurrentUser());
            
            return new OkObjectResult(numActivities.Count());
        }

        [HttpPost()]
        [Authorize]
        public IActionResult AddServiceLog( [FromBody] ActivityRevision activity){
            if(activity != null){

                var user = this.CurrentUser();
                var act = new Activity();
                act.KersUser = user;
                act.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                act.Created = DateTime.Now;
                act.Updated = DateTime.Now;
                act.ActivityDate = activity.ActivityDate;
                activity.Created = DateTime.Now;

                if(!activity.isSnap){
                    activity.SnapDirect = null;
                    activity.SnapIndirect = null;
                    activity.SnapPolicy = null;
                }else{
                    if(activity.IsPolicy){
                        activity.SnapDirect = null;
                    }else{
                        activity.SnapPolicy = null;
                    }
                }


                act.Title = activity.Title;
                act.Hours = activity.Hours;
                act.Audience = activity.Male + activity.Female;
                act.MajorProgramId = activity.MajorProgramId;
                act.Revisions = new List<ActivityRevision>();
                act.Revisions.Add(activity);
                context.Add(act); 
                this.Log(activity,"ActivityRevision", "Activity Added.");
                context.SaveChanges();
                return new OkObjectResult(activity);
            }else{
                this.Log( activity ,"ActivityRevision", "Error in adding Activity attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateActivity( int id, [FromBody] ActivityRevision activity){
           
            
            var entity = context.ActivityRevision.Find(id);
            var acEntity = context.Activity.Find(entity.ActivityId);

            if(activity != null && acEntity != null){
                activity.Created = DateTime.Now;
                acEntity.ActivityDate = activity.ActivityDate;



                if(!activity.isSnap){
                    activity.SnapDirect = null;
                    activity.SnapIndirect = null;
                    activity.SnapPolicy = null;
                }else{
                    if(activity.IsPolicy){
                        activity.SnapDirect = null;
                    }else{
                        activity.SnapPolicy = null;
                    }
                }


                acEntity.Title = activity.Title;
                acEntity.Hours = activity.Hours;
                acEntity.Audience = activity.Male + activity.Female;
                acEntity.MajorProgramId = activity.MajorProgramId;




                acEntity.Revisions.Add(activity);
                context.SaveChanges();
                this.Log(entity,"ActivityRevision", "Activity Updated.");
                return new OkObjectResult(activity);
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



        [HttpGet("options")]
        public IActionResult Options(){
            var cacheKey = "ServiceLogOptions";
            List<ActivityOption> ops;

            if (!_cache.TryGetValue(cacheKey, out ops))
            {
                // Key not in cache, so get data.
                ops = this.context.ActivityOption.Where(o => o.Active ).OrderBy(o => o.Order).ToList();
            
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(cacheKey, ops, cacheEntryOptions);
            }

            
            return new OkObjectResult(ops);
        }

        [HttpGet("optionnumbers")]
        public IActionResult OptionNumbers(){

            var cacheKey = "ServiceLogOptionNumbers";
            List<ActivityOptionNumber> ops;
            
            if (!_cache.TryGetValue(cacheKey, out ops))
            {
                // Key not in cache, so get data.
                ops = this.context.ActivityOptionNumber.OrderBy(o => o.Order).ToList();
            
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(cacheKey, ops, cacheEntryOptions);
            }
            
            return new OkObjectResult(ops);
        }

        [HttpGet("races")]
        public IActionResult Races(){

            var cacheKey = "races";
            List<Race> rcs;
            


            if (!_cache.TryGetValue(cacheKey, out rcs))
            {
                // Key not in cache, so get data.
                rcs = this.context.Race.OrderBy(o => o.Order).ToList();
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(cacheKey, rcs, cacheEntryOptions);
            }
            

            return new OkObjectResult(rcs);
        }

        [HttpGet("ethnicities")]
        public IActionResult Ethnicities(){

            var cacheKey = "ethnicities";
            List<Ethnicity> rcs;
            
            if (!_cache.TryGetValue(cacheKey, out rcs))
            {
                // Key not in cache, so get data.
                rcs = this.context.Ethnicity.OrderBy(o => o.Order).ToList();
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(cacheKey, rcs, cacheEntryOptions);
            }

            return new OkObjectResult(rcs);
        }

        /**********************************/
        // Snap Ed Direct
        /**********************************/
        [HttpGet("sessiontypes")]
        public IActionResult Sessiontypes(){
            var sst = this.context.SnapDirectSessionType.Where(o => o.Active).OrderBy(o => o.order);
            return new OkObjectResult(sst);
        }
        [HttpGet("snapdirectages")]
        public IActionResult SnapDirectAges(){
            var sst = this.context.SnapDirectAges.Where(o => o.Active).OrderBy(o => o.order);
            return new OkObjectResult(sst);
        }
        //SnapDirectAudience
        [HttpGet("snapdirectaudience")]
        public IActionResult SnapDirectAudience(){
            var sst = this.context.SnapDirectAudience.Where(o => o.Active).OrderBy(o => o.order);
            return new OkObjectResult(sst);
        }
        [HttpGet("snapdirectdeliverysite")]
        public IActionResult SnapDirectDeliverySite(){
            var sst = this.context.SnapDirectDeliverySite.Where(o => o.Active).OrderBy(o => o.order);
            return new OkObjectResult(sst);
        }

        /**********************************/
        // Snap Ed InDirect
        /**********************************/

        [HttpGet("snapindirectmethod")]
        public IActionResult SnapIndirectMethod(){
            var sst = this.context.SnapIndirectMethod.Where(o => o.Active).OrderBy(o => o.order);
            return new OkObjectResult(sst);
        }
        [HttpGet("snapindirectreached")]
        public IActionResult SnapIndirectReached(){
            var sst = this.context.SnapIndirectReached.Where(o => o.Active).OrderBy(o => o.order);
            return new OkObjectResult(sst);
        }

        /**********************************/
        // Snap Ed Plicy
        /**********************************/

        [HttpGet("snappolicyaimed")]
        public IActionResult SnapPolicyAimed(){
            var sst = this.context.SnapPolicyAimed.Where(o => o.Active).OrderBy(o => o.order);
            return new OkObjectResult(sst);
        }

        [HttpGet("snappolicypartner")]
        public IActionResult SnapPolicyPartner(){
            var sst = this.context.SnapPolicyPartner.Where(o => o.Active).OrderBy(o => o.order);
            return new OkObjectResult(sst);
        }

    }
}