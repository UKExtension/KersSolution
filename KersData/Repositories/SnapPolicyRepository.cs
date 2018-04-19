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

namespace Kers.Models.Repositories
{
    public class SnapPolicyRepository : SnapBaseRepository, ISnapPolicyRepository
    {

        private KERScoreContext context;
        private IDistributedCache _cache;
        public SnapPolicyRepository(KERScoreContext context, IDistributedCache _cache)
            : base(context, _cache)
        { 
            this.context = context;
            this._cache = _cache;
        }


        public string AimedTowardsImprovement(FiscalYear fiscalYear, bool refreshCache = false){


            string result;
            var cacheKey = CacheKeys.AimedTowardsImprovement + fiscalYear.Name;
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
                var partners = this.context.SnapPolicyAimed.Where( p => p.Active && p.FiscalYear == fiscalYear).ToList();
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
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 4 )
                    });
            
            
            }
            
            return result;
        }

        public string PartnerCategory(FiscalYear fiscalYear, bool refreshCache = false){
            
            
            string result;
            var cacheKey = CacheKeys.SnapPartnerCategory + fiscalYear.Name;
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
                var partners = this.context.SnapPolicyPartner.Where( p => p.Active && p.FiscalYear == fiscalYear).ToList();
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
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
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
                keys.Add("PlanningUnit");
                keys.Add("District");
                keys.Add("Program(s)");
                keys.Add("EventDate");
                keys.Add("Hours");


                var types = context.SnapPolicyAimed.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
                foreach( var met in types){
                    keys.Add(string.Concat( "\"", met.Name, "\""));
                }
                keys.Add("PurposeGoal");
                keys.Add("ResultImpact");
                result = string.Join(",", keys.ToArray()) + "\n";
                var activitiesThisFiscalYear = context.Activity.Where( a => 
                                                                            a.ActivityDate > fiscalYear.Start 
                                                                            && 
                                                                            a.ActivityDate < fiscalYear.End
                                                                            &&
                                                                            a.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                                        );
                var activitiesWithPolicy = activitiesThisFiscalYear.Where( r => r.Revisions.Last().SnapPolicy != null).OrderBy( a => a.ActivityDate.Year).ThenBy( a => a.ActivityDate.Month).ThenBy(a => a.KersUser.PersonalProfile.FirstName);
                var policyMeetings = activitiesWithPolicy.Select(
                                        a => new {
                                            //SnapPolicy = a.Revisions.OrderBy( r => r.Created).Last().SnapPolicy,
                                            ActivityDate = a.ActivityDate,
                                            PersonalProfile = a.KersUser.PersonalProfile,
                                            PlanningUnit = a.KersUser.RprtngProfile.PlanningUnit,
                                            Hours = a.Hours,
                                            Programs = a.KersUser.Specialties,
                                            Revisions = a.Revisions
                                        }
                                    )
                                    .ToList();
                var specialties = context.Specialty.ToList();
                foreach( var meeting in policyMeetings){
                    var LastRevision = meeting.Revisions.OrderBy(r => r.Created).Last();
                    if(LastRevision.SnapPolicyId != null){
                        var row = meeting.ActivityDate.ToString("yyyyMM") + ",";
                        row += meeting.ActivityDate.ToString("yyyy-MMM") + ",";
                        row += meeting.PersonalProfile.FirstName + meeting.PersonalProfile.LastName + ",";
                        row += meeting.PlanningUnit.Name + ",";
                        row += meeting.PlanningUnit.DistrictId + ",";
                        var prgrms = "";
                        foreach( var program in meeting.Programs){
                            prgrms += specialties.Where( s => s.Id == program.SpecialtyId).FirstOrDefault() ?.Code + " "??"";
                        }
                        row += prgrms + ",";
                        row += meeting.ActivityDate.ToString("mm/dd/yyy") + ",";
                        row += meeting.Hours.ToString() + ",";
                        var aimed = context.SnapPolicy.Where( p => p.Id == LastRevision.SnapPolicyId).Include( s => s.SnapPolicyAimedSelections).FirstOrDefault();
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
                        row += string.Concat( "\"", StripHTML(aimed.Purpose), "\"") + ",";
                        row += string.Concat( "\"", StripHTML(aimed.Result), "\"") ;
                        
                        result += row + "\n";
                    }
                    
                }

                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }



    }


}