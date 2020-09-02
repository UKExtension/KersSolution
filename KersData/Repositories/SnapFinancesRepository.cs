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
using Microsoft.Extensions.Caching.Memory;

namespace Kers.Models.Repositories
{
    public class SnapFinancesRepository : SnapBaseRepository, ISnapFinancesRepository
    {

        private KERScoreContext context;
        private IDistributedCache _cache;
        public SnapFinancesRepository(KERScoreContext context, IDistributedCache _cache, IMemoryCache _memoryCache)
            : base(context, _cache, _memoryCache)
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
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
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
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
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
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
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
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                        });
            }
            return result;
        }


        public string ReimbursementNepAssistants(FiscalYear fiscalYear, bool refreshCache = false){

            string result;
            var cacheKey = CacheKeys.ReimbursementNepAssistants + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{


                var keys = new List<string>();

                keys.Add("FY");
                keys.Add("AssistantName");
                keys.Add("PlanningUnit");
                keys.Add("ReimbursementsYearToDateTotal");
                keys.Add("BudgetRemaining");


                result = string.Join(",", keys.ToArray()) + "\n";

                //List<KersUser> assistants;
                var assistants = this.context.KersUser.
                                Where(c=> (
                                    c.Specialties.Where(s => s.Specialty.Name == "Expanded Food and Nutrition Education Program").Count() != 0 
                                    ||
                                    c.Specialties.Where(s => s.Specialty.Name == "Supplemental Nutrition Assistance Program Education").Count() != 0
                                    )
                                    &&
                                    c.RprtngProfile.enabled
                                ).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                                Include(u=>u.PersonalProfile).
                                OrderBy(c => c.RprtngProfile.Name);

                var allowance = context.SnapBudgetAllowance.Where( a => a.FiscalYear == fiscalYear && a.BudgetDescription == "SNAP Ed NEP Assistant Budget").First().AnnualBudget;
                foreach( var assistant in assistants){
                    var row = fiscalYear.Name + ",";
                    row += string.Concat( "\"", assistant.RprtngProfile.Name, "\"") + ",";
                    row += string.Concat( "\"", assistant.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                    var reimbursement = context.SnapBudgetReimbursementsNepAssistant.Where( r => r.FiscalYear == fiscalYear && r.To == assistant).Sum( r => r.Amount);
                    row += reimbursement.ToString() + ",";
                    row += (allowance - reimbursement).ToString();
                    result += row + "\n";
                }

                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                        });



            }    
            return result;
        }


        public string ReimbursementCounty(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            
            var cacheKey = CacheKeys.SnapReimbursementCounty + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{

                var keys = new List<string>();
                
                keys.Add("PlanningUnit");
                keys.Add("ReimbursementsYearToDateTotal");
                keys.Add("BudgetRemaining");


                result = string.Join(",", keys.ToArray()) + "\n";


                List<PlanningUnit> counties;

            
                
                var cacheKeyList = "CountiesList";
                var cached = _cache.GetString(cacheKeyList);

                if (!string.IsNullOrEmpty(cached)){
                    counties = JsonConvert.DeserializeObject<List<PlanningUnit>>(cached);
                }else{
                
                
                    counties = this.context.PlanningUnit.
                                    Where(c=>c.District != null && c.Name.Substring(c.Name.Count() - 3) == "CES").
                                    OrderBy(c => c.Name).ToList();
                    

                    var serializedCounties = JsonConvert.SerializeObject(counties);
                    _cache.SetString(cacheKeyList, serializedCounties, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                            });
                }
                var allowance = context.SnapBudgetAllowance.Where( a => a.FiscalYear == fiscalYear && a.BudgetDescription == "SNAP Ed County Budget (separate from NEP Assistant Budget)").First().AnnualBudget;
                foreach( var county in counties){
                    
                    var row = string.Concat( "\"", county.Name.Substring(0, county.Name.Count() - 11), "\"") + ",";
                    var reimbursement = context.SnapBudgetReimbursementsCounty.Where( r => r.FiscalYear == fiscalYear && r.PlanningUnitId == county.Id).Sum( r => r.Amount);
                    row += reimbursement.ToString() + ",";
                    var countyAllowance = context.SnapCountyBudget.Where( b => b.PlanningUnitId == county.Id && b.FiscalYear == fiscalYear).FirstOrDefault();
                    var thisAllowance = allowance;
                    if( countyAllowance != null){
                        thisAllowance = countyAllowance.AnnualBudget;
                    }
                    row += (thisAllowance - reimbursement).ToString();
                    result += row + "\n";
                }

                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
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
            keys.Add("NumberOfColorCopies");
            keys.Add("NumberOfBWCopies");
            var result = string.Join(",", keys.ToArray()) + "\n";
            var activitiesWithCopies = RevisionsWithSnapData(fiscalYear).Where( a => a.SnapCopies != 0 || a.SnapCopiesBW != 0);
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
                                        Copies = g.Select( r => r.Revision).Sum( s => s.SnapCopies),
                                        BwCopies = g.Select( r => r.Revision).Sum( s => s.SnapCopiesBW)
                                    }).OrderBy( o => o.Unit.Name).ToList();

                var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                foreach( var unit in grouppedByUnit){
                    var row = dt.ToString("yyyyMM") + ",";
                    row += dt.ToString("yyyy-MMM") + ",";
                    row += string.Concat( "\"", unit.Unit.Name, "\"") + ",";
                    row += unit.Copies.ToString() + ",";
                    row += unit.BwCopies.ToString();
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
            keys.Add("NumberOfColorCopies");
            keys.Add("NumberOfBWCopies");
            keys.Add("EntryDate");
            keys.Add("Mode");
            keys.Add("Title");
            keys.Add("Program(s)");


            var result = string.Join(",", keys.ToArray()) + "\n";
            var activitiesWithCopies = RevisionsWithSnapData(fiscalYear).Where( a => a.SnapCopies != 0 || a.SnapCopiesBW != 0);
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
                row += rev.Revision.SnapCopiesBW.ToString() + ",";
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