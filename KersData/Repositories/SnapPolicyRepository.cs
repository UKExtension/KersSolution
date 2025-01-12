using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Data;
using Kers.Models.Contexts;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;

namespace Kers.Models.Repositories
{
    public class SnapPolicyRepository : SnapBaseRepository, ISnapPolicyRepository
    {

        private KERScoreContext context;
        private IDistributedCache _cache;
        public SnapPolicyRepository(KERScoreContext context, IDistributedCache _cache, IMemoryCache _memoryCache)
            : base(context, _cache, _memoryCache)
        { 
            this.context = context;
            this._cache = _cache;
        }


        public string AimedTowardsImprovement(FiscalYear fiscalYear, bool refreshCache = false){


            string result;
            var cacheKey = CacheKeys.AimedTowardsImprovement + fiscalYear.Name + fiscalYear.Type;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{


                var keys = new List<string>();
                keys.Add("YearMonth");
                keys.Add("YearMonthName");
                keys.Add("AimedTowardImprovementInName");
                keys.Add("NumberOfAgentsReporting");
                keys.Add("TotalHoursReported");


                result = string.Join(",", keys.ToArray()) + "\n";
                //var lastActivityRevs = this.activityRepo.LastActivityRevisionIds(fiscalYear, _cache);
                var revis = SnapData(fiscalYear);
                var activitiesWithPolicy = revis.Where( r => r.Revision.SnapPolicy != null);
                var groupedByMonth = activitiesWithPolicy.GroupBy(
                                                            p => new {
                                                                Year = p.Revision.ActivityDate.Year,
                                                                Month = p.Revision.ActivityDate.Month
                                                            }
                                                    )
                                                    .Select(
                                                            k => new {
                                                                Month = k.Key.Month,
                                                                Year = k.Key.Year,
                                                                Revisions = k.Select( a => a.Revision),
                                                                User = k.Select( a => a.User)
                                                            }
                                                    );
                var partners = this.context.SnapPolicyAimed.Where( p => p.Active).ToList();
                foreach( var byMonth in groupedByMonth){
                    var revisionIds = byMonth.Revisions.Select( a => a.Id);
                    //List of revisions per this month with policy data
                    var byAimed = context.ActivityRevision
                                                        .Where( r => revisionIds.Contains( r.Id ) )
                                                        .Select( r => new {
                                                                    ActivityId = r.ActivityId,
                                                                    Hours = r.Hours,
                                                                    Aimed = r.SnapPolicy.SnapPolicyAimedSelections
                                                                }
                                                        ).ToList();
                    var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                    // cycle through partners
                    foreach( var partner in partners){
                        
                        var row = dt.ToString("yyyyMM") + ",";
                        row += dt.ToString("yyyy-MMM") + ",";

                        float totalHours = 0;
                        var totalMeetings = 0;
                        var revIdsPerPartner = new List<int>();
                        // Cycle through revisions
                        foreach( var revs in byAimed){
                        
                            if( revs.Aimed != null){
                                // if there is selection in the revision from this partner
                                var rv = revs.Aimed.Where( r => r.SnapPolicyAimedId == partner.Id).FirstOrDefault();
                                if(rv != null){
                                    revIdsPerPartner.Add( revs.ActivityId );
                                    totalHours += revs.Hours;
                                    totalMeetings++;
                                }
                            }
                        }
                        var agentsReporting = context.Activity.Where( a => revIdsPerPartner.Contains( a.Id ) ).GroupBy( c => c.KersUserId).Count();
                        row += string.Concat( "\"", partner.Name, "\"") + ",";
                        row += agentsReporting.ToString() + ",";
                        row += totalHours.ToString();
                        result += row + "\n";
                    }
                }
            
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                    });
            
            
            }
            
            return result;
        }

        public string PartnerCategory(FiscalYear fiscalYear, bool refreshCache = false){
            
            
            string result;
            var cacheKey = CacheKeys.SnapPartnerCategory + fiscalYear.Name + fiscalYear.Type;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{


                
                var keys = new List<string>();
                keys.Add("YearMonth");
                keys.Add("YearMonthName");
                keys.Add("PartnerName");
                keys.Add("NumberOfAgentsReporting");
                result = string.Join(",", keys.ToArray()) + "\n";
                var revis = SnapData(fiscalYear);
                var activitiesWithPolicy = revis.Where( r => r.Revision.SnapPolicy != null).OrderBy( a => a.Revision.ActivityDate.Year).ThenBy( a => a.Revision.ActivityDate.Month);
                var groupedByMonth = activitiesWithPolicy.GroupBy(
                                                            p => new {
                                                                Year = p.Revision.ActivityDate.Year,
                                                                Month = p.Revision.ActivityDate.Month
                                                            }
                                                    )
                                                    .Select(
                                                            k => new {
                                                                Month = k.Key.Month,
                                                                Year = k.Key.Year,
                                                                Revisions = k.Select( a => a.Revision)
                                                            }
                                                    );
                var partners = this.context.SnapPolicyPartner.Where( p => p.Active).ToList();
                foreach( var byMonth in groupedByMonth){
                    var revisionIds = byMonth.Revisions.Select( a => a.Id);
                    var byPartner = context.ActivityRevision
                                                        .Where( r => revisionIds.Contains( r.Id ) )
                                                        .Select( r => new {
                                                                            activityId = r.ActivityId,
                                                                            partnerValues = r.SnapPolicy.SnapPolicyPartnerValue
                                                                        }
                                                                    )
                                                        .ToList();
                    var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                    foreach( var partner in partners){
                        var activityIdsPerPartner = new List<int>();
                        
                        foreach( var prtnr in byPartner ){
                            var rv = prtnr.partnerValues.Where( p => p.SnapPolicyPartnerId == partner.Id && p.Value != 0).FirstOrDefault();
                            if(rv != null){
                                activityIdsPerPartner.Add(prtnr.activityId);
                            }
                        }
                        var row = dt.ToString("yyyyMM") + ",";
                        row += dt.ToString("yyyy-MMM") + ",";
                        row += string.Concat( "\"", partner.Name, "\"") + ",";
                        row += context.Activity.Where( a => activityIdsPerPartner.Contains(a.Id)).GroupBy( g => g.KersUserId).Count().ToString();
                        result += row + "\n";
                    }
                }

                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                    });

            }
            return result;
        }

        public string PartnersOfACounty(int countyId, FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.PartnersOfACounty + "_" + countyId.ToString() + "_" + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();
                keys.Add("DateOfEVENT");
                keys.Add("DateSubmitted");
                keys.Add("SubmittedBy");

                var partnters = this.context.SnapPolicyPartner.Where( p => p.Active).ToList();

                foreach( var partner in partnters){
                    keys.Add(string.Concat( "\"", partner.Name, "\""));
                }

                result = string.Join(",", keys.ToArray()) + "\n";
                var revis = SnapData(fiscalYear);
                var activitiesWithPolicy = revis.Where( r => r.Revision.SnapPolicy != null && r.User.RprtngProfile.PlanningUnitId == countyId).OrderBy( a => a.Revision.ActivityDate.Year).ThenBy( a => a.Revision.ActivityDate.Month);
                
                foreach( var activity in activitiesWithPolicy ){
                    var fullDetails = context.ActivityRevision.Where( r => r.Id == activity.Revision.Id)
                                            .Include( r => r.SnapPolicy).ThenInclude( p => p.SnapPolicyPartnerValue)
                                            .FirstOrDefault();
                    var row = fullDetails.ActivityDate.ToString("MM/dd/yyyy") + ",";
                    row += fullDetails.Created.ToString("MM/dd/yyyy") + ",";
                    row += string.Concat( "\"",activity.User.RprtngProfile.Name, "\"") + ",";
                    foreach( var partner in partnters){
                        
                        var partnrVal = fullDetails.SnapPolicy.SnapPolicyPartnerValue.Where( v => v.SnapPolicyPartnerId == partner.Id).FirstOrDefault();
                        if(partnrVal == null){
                            row += "0" + ",";
                        }else{
                            row += partnrVal.Value.ToString() + ",";
                        }



                    }
                    result += row + "\n";
                }

                


                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                    });

            }
            return result;
        }



        public string AgentCommunityEventDetail(FiscalYear fiscalYear, bool refreshCache = false){


            string result;
            var cacheKey = CacheKeys.SnapAgentCommunityEventDetail + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{


                var keys = new List<string>();
                keys.Add("YearMonth");
                keys.Add("YearMonthName");
                keys.Add("PersonName");
                keys.Add("Title");
                keys.Add("PlanningUnit");
                keys.Add("Area");
                keys.Add("Program(s)");
                keys.Add("EventDate");
                keys.Add("Hours");

                keys.Add("PurposeGoal");
                keys.Add("ResultImpact");
                var types = context.SnapPolicyAimed.Where(m => m.Active).OrderBy( m => m.order);
                foreach( var met in types){
                    keys.Add(string.Concat( "\"Aimed: ", met.Name, "\""));
                }
                


                var partners = context.SnapPolicyPartner.Where(m => m.Active).OrderBy( m => m.order);
                foreach( var par in partners){
                    keys.Add(string.Concat( "\"Partners: ", par.Name, "\""));
                }

                result = string.Join(",", keys.ToArray()) + "\n";








                var activitiesThisFiscalYear = context.Activity.Where( a => 
                                                                            a.ActivityDate > fiscalYear.Start 
                                                                            && 
                                                                            a.ActivityDate < fiscalYear.End
                                                                            &&
                                                                            a.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                                        );
                var meetings = activitiesThisFiscalYear.Select(
                                        a => new {
                                            //SnapPolicy = a.Revisions.OrderBy( r => r.Created).Last().SnapPolicy,
                                            ActivityDate = a.ActivityDate,
                                            User = a.KersUser,
                                            Position = a.KersUser.ExtensionPosition.Code,
                                            PersonalProfile = a.KersUser.PersonalProfile,
                                            PlanningUnit = a.KersUser.RprtngProfile.PlanningUnit,
                                            Area = a.KersUser.RprtngProfile.PlanningUnit.ExtensionArea,
                                            Hours = a.Hours,
                                            Programs = a.KersUser.Specialties,
                                            Revisions = a.Revisions
                                        }
                                    );
                var activitiesWithPolicy = meetings
                         .OrderBy( a => a.ActivityDate)
                            .ThenBy(a => a.PersonalProfile.FirstName);
                
                var specialties = context.Specialty.ToList();
                foreach( var meeting in activitiesWithPolicy){
                    var LastRevision = meeting.Revisions.OrderBy(r => r.Created).Last();
                    if(LastRevision.SnapPolicyId != null){
                        var row = meeting.ActivityDate.ToString("yyyyMM") + ",";
                        row += meeting.ActivityDate.ToString("yyyy-MMM") + ",";
                        row += meeting.PersonalProfile.FirstName + meeting.PersonalProfile.LastName + ",";
                        if(this.context.zEmpProfileRole.Where( r => r.User.Id == meeting.User.Id && r.zEmpRoleType.shortTitle == "CNTMNGR" ).Any()){
                            row += string.Concat( "\"", meeting.Position, ", CNTMNGR\"") + ",";
                        }else{
                            row += string.Concat( "\"", meeting.Position, "\"") + ",";
                        }
                        row += meeting.PlanningUnit.Name + ",";
                        row += meeting.PlanningUnit.ExtensionArea == null ? "" : meeting.PlanningUnit.ExtensionArea.Name + ",";
                        var prgrms = "";
                        foreach( var program in meeting.Programs){
                            prgrms += specialties.Where( s => s.Id == program.SpecialtyId).FirstOrDefault() ?.Code + " "??"";
                        }
                        row += prgrms + ",";
                        row += meeting.ActivityDate.ToString("MM/dd/yyy") + ",";
                        row += meeting.Hours.ToString() + ",";
                        var aimed = context.SnapPolicy.Where( p => p.Id == LastRevision.SnapPolicyId)
                                        .Include( s => s.SnapPolicyAimedSelections)
                                        .Include( s => s.SnapPolicyPartnerValue)
                                        .FirstOrDefault();
                        row += string.Concat( "\"", StripHTML(aimed.Purpose), "\"") + ",";
                        row += string.Concat( "\"", StripHTML(aimed.Result), "\",") ;
                        foreach( var tp in types){
                            if( aimed.SnapPolicyAimedSelections == null){
                                row += ",";
                            }else{
                                var sels = aimed.SnapPolicyAimedSelections.Where( a => a.SnapPolicyAimedId == tp.Id).FirstOrDefault();
                                if( sels != null ){
                                    row += "X,";
                                }else{
                                    row += ",";
                                }
                            }
                        }

                        foreach( var par in partners){
                            if( aimed.SnapPolicyPartnerValue == null){
                                row += "0,";
                            }else{
                                var sels = aimed.SnapPolicyPartnerValue.Where( a => a.SnapPolicyPartnerId == par.Id).FirstOrDefault();
                                if( sels != null ){
                                    row += sels.Value.ToString() + ",";
                                }else{
                                    row += ",";
                                }
                            }
                        }
                        
                        
                        result += row + "\n";
                    }
                    
                }

                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                    });
            }
            return result;
        }



    }


}