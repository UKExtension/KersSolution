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
            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("AimedTowardImprovementInName");
            keys.Add("NumberOfAgentsReporting");
            keys.Add("TotalHoursReported");


            var result = string.Join(",", keys.ToArray()) + "\n";
            //var lastActivityRevs = this.activityRepo.LastActivityRevisionIds(fiscalYear, _cache);
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
            var partners = this.context.SnapPolicyAimed.Where( p => p.Active && p.FiscalYear == fiscalYear).ToList();
            foreach( var byMonth in groupedByMonth){
                var revisionIds = byMonth.Revisions.Select( a => a.Id);
                var byAimed = context.ActivityRevision
                                                    .Where( r => revisionIds.Contains( r.Id ) )
                                                    .Select( r => new {
                                                                Hours = r.Hours,
                                                                Aimed = r.SnapPolicy.SnapPolicyAimedSelections
                                                            }
                                                    ).ToList();
                
                
                
                var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                foreach( var partner in partners){
                    
                    var row = dt.ToString("yyyyMM") + ",";
                    row += dt.ToString("yyyy-MMM") + ",";

                    float totalHours = 0;
                    var totalMeetings = 0;
                    foreach( var revs in byAimed){
                        if( revs.Aimed != null){
                            var rv = revs.Aimed.Where( r => r.SnapPolicyAimedId == partner.Id).FirstOrDefault();
                            if(rv != null){
                                totalHours += revs.Hours;
                                totalMeetings++;
                            }
                        }
                    }

                    row += string.Concat( "\"", partner.Name, "\"") + ",";
                    row += totalMeetings.ToString() + ",";
                    row += totalHours.ToString();
                    result += row + "\n";
                }
            }
            
            return result;
        }

        public string PartnerCategory(FiscalYear fiscalYear, bool refreshCache = false){
            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("PartnerName");
            keys.Add("NumberOfAgentsReporting");
            var result = string.Join(",", keys.ToArray()) + "\n";
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
                                                    .Select( r => r.SnapPolicy.SnapPolicyPartnerValue)
                                                    .ToList();
                var partnerValues = new List<SnapPolicyPartnerValue>();
                foreach( var byPartnr in byPartner){
                    partnerValues.AddRange( byPartnr);
                }
                var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                foreach( var partner in partners){
                    
                    var row = dt.ToString("yyyyMM") + ",";
                    row += dt.ToString("yyyy-MMM") + ",";
                    row += string.Concat( "\"", partner.Name, "\"") + ",";
                    row += partnerValues.Where( p => p.SnapPolicyPartnerId == partner.Id && p.Value != 0).Count().ToString();
                    result += row + "\n";
                }
            }
            return result;
        }


    }


}