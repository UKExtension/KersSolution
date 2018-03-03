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
    public class SnapFinancesRepository : SnapBaseRepository, ISnapFinancesRepository
    {

        private KERScoreContext context;
        private IDistributedCache _cache;
        public SnapFinancesRepository(KERScoreContext context, IDistributedCache _cache)
            : base(context, _cache)
        { 
            this.context = context;
            this._cache = _cache;
        }






        public string CopiesSummarybyCountyAgents(FiscalYear fiscalYear, bool refreshCache = false ){
            string result;
            var cacheKey = CacheKeys.CopiesSummarybyCountyAgents + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                result = CopiesReportPerCounty( fiscalYear, 1);
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                        });
            }
            return result;
        }


        public string CopiesSummarybyCountyNotAgents(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.CopiesSummarybyCountyNotAgents + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                result = CopiesReportPerCounty( fiscalYear, 2);
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                        });
            }
            return result;
        }

        public string CopiesDetailAgents(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.CopiesDetailAgents + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                result = CopiesReportDetails( fiscalYear, 1);
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                        });
            }
            return result ;
        }   
        public string CopiesDetailNotAgents(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.CopiesDetailANotAgents + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                result = CopiesReportDetails( fiscalYear, 2);
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                        });
            }
            return result;
        }

        // type: 1 agents, 2 non agents
        private string CopiesReportPerCounty(FiscalYear fiscalYear, int type){

            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("PlanningUnit");
            keys.Add("NumberOfCopies");
            var result = string.Join(",", keys.ToArray()) + "\n";
            var activitiesWithCopies = RevisionsWithSnapData(fiscalYear).Where( a => a.SnapCopies != 0);
            var groupedByMonth = activitiesWithCopies.GroupBy(
                                                        p => new {
                                                            Year = p.ActivityDate.Year,
                                                            Month = p.ActivityDate.Month
                                                        }
                                                )
                                                .Select(
                                                        k => new {
                                                            Month = k.Key.Month,
                                                            Year = k.Key.Year,
                                                            Revisions = k.Select( a => a)
                                                        }
                                                );
            foreach( var byMonth in groupedByMonth){
                var revisionIds = byMonth.Revisions.Select( a => a.Id);

                var activities = context.ActivityRevision.Where( r => revisionIds.Contains(r.Id))
                                    .Select(
                                        r => new UserRevisionData {
                                            Revision = r,
                                            User = context.Activity.Where( a => a.Id == r.ActivityId).FirstOrDefault().KersUser
                                        }
                                    )
                                    .ToList();
                
                var fullRevisions = new List<UserRevisionData>();
                foreach( var actvt in activities){
                    var usr = context.KersUser
                                        .Where( u => u.Id == actvt.User.Id )
                                        .Include( u => u.ExtensionPosition)
                                        .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit)
                                        .FirstOrDefault();
                    fullRevisions.Add(
                        new UserRevisionData{
                            Revision = actvt.Revision,
                            User = usr
                        }
                    );
                }

                IEnumerable<UserRevisionData> byUnit;

                if( type == 1){
                    byUnit = fullRevisions.Where( r => r.User.ExtensionPosition.Code == "AGENT");
                }else{
                    byUnit = fullRevisions.Where( r => r.User.ExtensionPosition.Code != "AGENT");
                }
                var grouppedByUnit = byUnit.GroupBy( r => r.User.RprtngProfile.PlanningUnit)
                                    .Select( g => new {
                                        Unit = g.Key,
                                        Copies = g.Select( r => r.Revision).Sum( s => s.SnapCopies)
                                    }).OrderBy( o => o.Unit.Name).ToList();

                var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                foreach( var unit in grouppedByUnit){
                    var row = dt.ToString("yyyyMM") + ",";
                    row += dt.ToString("yyyy-MMM") + ",";
                    row += string.Concat( "\"", unit.Unit.Name, "\"") + ",";
                    row += unit.Copies.ToString();
                    result += row + "\n";
                }
               
            }
            return result;
        }


        private string CopiesReportDetails(FiscalYear fiscalYear, int type){
	

            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("PlanningUnit");
            keys.Add("PersonName");
            keys.Add("EventDate");
            keys.Add("NumberOfCopies");
            keys.Add("EntryDate");
            keys.Add("Mode");
            keys.Add("Title");
            keys.Add("Program(s)");


            var result = string.Join(",", keys.ToArray()) + "\n";
            var activitiesWithCopies = RevisionsWithSnapData(fiscalYear).Where( a => a.SnapCopies != 0);
            var revisionIds = activitiesWithCopies.Select( a => a.Id);
            var activities = context.ActivityRevision.Where( r => revisionIds.Contains(r.Id))
                                    .Select(
                                        r => new UserRevisionData {
                                            Revision = r,
                                            User = context.Activity.Where( a => a.Id == r.ActivityId).FirstOrDefault().KersUser
                                        }
                                    )
                                    .ToList();
            var fullRevisions = new List<UserRevisionData>();
            foreach( var actvt in activities){
                var usr = context.KersUser
                                    .Where( u => u.Id == actvt.User.Id )
                                    .Include( u => u.ExtensionPosition)
                                    .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit)
                                    .Include( u => u.Specialties).ThenInclude( s => s.Specialty)
                                    .FirstOrDefault();
                fullRevisions.Add(
                    new UserRevisionData{
                        Revision = actvt.Revision,
                        User = usr
                    }
                );
            }
            List<UserRevisionData> filteredRevisions;
            if( type == 1){
                filteredRevisions = fullRevisions.Where( r => r.User.ExtensionPosition.Code == "AGENT").ToList();
            }else{
                filteredRevisions = fullRevisions.Where( r => r.User.ExtensionPosition.Code != "AGENT").ToList();
            }
            var orderedRevisions = filteredRevisions.OrderBy(r => r.Revision.ActivityDate.Year).ThenBy( r => r.Revision.ActivityDate.Month ).ThenBy( r => r.User.RprtngProfile.PlanningUnit.Name ).ThenBy( r => r.User.RprtngProfile.Name);
            foreach( var rev in orderedRevisions){
                var dt = new DateTime( rev.Revision.ActivityDate.Year, rev.Revision.ActivityDate.Month, 15);
                var row = dt.ToString("yyyyMM") + ",";
                row += dt.ToString("yyyy-MMM") + ",";
                row += string.Concat( "\"", rev.User.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                row += string.Concat( "\"", rev.User.RprtngProfile.Name, "\"") + ",";
                row += rev.Revision.ActivityDate.ToString( "MM/dd/yyyy") + ",";
                row += rev.Revision.SnapCopies.ToString() + ",";
                row += string.Concat( "\"", rev.Revision.Created.ToString(), "\"" ) + ",";
                var mode = "";
                if(rev.Revision.SnapAdmin){
                    mode = "Admin";
                }else{
                    if( rev.Revision.SnapDirectId != null){
                        mode = "Direct";
                    }else if( rev.Revision.SnapPolicyId != null){
                        mode = "Commmunity";
                    }
                    if( rev.Revision.SnapIndirectId != null){
                        mode += " Indirect";
                    }
                }
                row += string.Concat( "\"", mode, "\"" ) + ",";
                row += rev.User.ExtensionPosition.Code + ",";
                var spclt = "";
                foreach( var sp in rev.User.Specialties){
                    spclt += " " + sp.Specialty.Code;
                }
                row += spclt;
                result += row + "\n";
            }

            return result;
        }









    }


}