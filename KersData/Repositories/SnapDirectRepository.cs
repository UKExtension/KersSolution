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
    public class SnapDirectRepository : SnapBaseRepository, ISnapDirectRepository
    {

        private KERScoreContext context;
        private IDistributedCache _cache;
        public SnapDirectRepository(KERScoreContext context, IDistributedCache _cache)
            : base(context, _cache)
        { 
            this.context = context;
            this._cache = _cache;
        }

        public string TotalByMonth(FiscalYear fiscalYear, Boolean refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapEdTotalByMonth + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                
                /*********************************/

                // Build result from data source 

                /*********************************/
                var keys = new List<string>();
                keys.Add("YearMonth");
                keys.Add("YearMonthName");
                keys.Add("HoursReported");
                keys.Add("DirectContacts");

                var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order).ToList();
                
                foreach( var audnc in snapDirectAudience){
                    keys.Add(audnc.Name);
                }
                var snapDirectAges = this.context.SnapDirectAges.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order).ToList();
                foreach( var ags in snapDirectAges){
                    keys.Add(ags.Name);
                }
                keys.Add("Male");
                keys.Add("Female");
                var races = this.context.Race.ToList();
                var ethnicities = this.context.Ethnicity;

                foreach(var race in races){
                    foreach( var ethn in ethnicities){
                        keys.Add( race.Name + ethn.Name);
                    }
                }
                keys.Add("IndirectContacts");
                result = string.Join(", ", keys.ToArray()) + "\n";
                var SnapData = this.SnapData( fiscalYear);
                var byMonth = SnapData.GroupBy( s => new {
                                            Year = s.Revision.ActivityDate.Year,
                                            Month = s.Revision.ActivityDate.Month

                                        }
                
                                    ).Select( 
                                            d => new {
                                                Year = d.Key.Year,
                                                Month = d.Key.Month,
                                                Revisions = d.Select( s => s.Revision )
                                            }
                                        )
                                        .OrderBy( d => d.Year).ThenBy( d => d.Month);
                foreach( var monthData in byMonth ){
                    var dt = new DateTime( monthData.Year, monthData.Month, 15);
                    var row = dt.ToString("yyyyMM") + ",";
                    row += dt.ToString("yyyy-MMM") + ",";
                    row += monthData.Revisions.Sum( s => s.Hours).ToString() + ",";

                    var male = monthData.Revisions.Sum( s => s.Male);
                    var female = monthData.Revisions.Sum( s => s.Female);

                    row += ( male + female ).ToString() + ",";
                    var revisionsWithDirectContacts = monthData.Revisions.Where( s => s.SnapDirect != null);

                    var ageAudienceValues = new List<SnapDirectAgesAudienceValue>();
                    foreach( var rev in revisionsWithDirectContacts){
                        ageAudienceValues.AddRange( rev.SnapDirect.SnapDirectAgesAudienceValues);
                    }


                    foreach( var audnc in snapDirectAudience){
                        row += ageAudienceValues.Where( a => a.SnapDirectAudienceId == audnc.Id).Sum( s => s.Value ).ToString() + ",";
                    }

                    foreach( var ags in snapDirectAges){
                        row += ageAudienceValues.Where( a => a.SnapDirectAgesId == ags.Id).Sum( s => s.Value ).ToString() + ",";
                    }

                    row += male.ToString() + ",";
                    row += female.ToString() + ",";


                    var RaceEthnicityValues = new List<RaceEthnicityValue>();

                    foreach( var rev in monthData.Revisions){
                        RaceEthnicityValues.AddRange(rev.RaceEthnicityValues);
                    }

                    foreach(var race in races){
                        foreach( var ethn in ethnicities){
                            row += RaceEthnicityValues.Where( v => v.EthnicityId == ethn.Id && v.RaceId == race.Id).Sum( s => s.Amount).ToString() + ",";
                        }
                    }


                    var withIndirect = monthData.Revisions.Where( r => r.SnapIndirect != null);
                    var indirects = 0;
                    foreach( var ind in withIndirect){
                        indirects += ind.SnapIndirect.SnapIndirectReachedValues.Sum( s => s.Value);
                    }
                    row += indirects.ToString();
                    result += row + "\n";
                }
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    }); 
            }
            return result;
        }
        public string TotalByCounty(FiscalYear fiscalYear, Boolean refreshCache = false){

            string result;
            var cacheKey = CacheKeys.SnapEdTotalByCounty + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                /*********************************/

                // Build result from data source 

                /*********************************/
                var keys = new List<string>();
                keys.Add("FY");
                keys.Add("PlanningUnit");
                keys.Add("HoursReported");
                keys.Add("DirectContacts");

                var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);
                
                foreach( var audnc in snapDirectAudience){
                    keys.Add(audnc.Name);
                }

                var snapDirectAges = this.context.SnapDirectAges.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);

                foreach( var ags in snapDirectAges){
                    keys.Add(ags.Name);
                }

                keys.Add("Male");
                keys.Add("Female");

                var races = this.context.Race;
                var ethnicities = this.context.Ethnicity;

                foreach(var race in races){
                    foreach( var ethn in ethnicities){
                        keys.Add( race.Name + ethn.Name);
                    }
                }

                keys.Add("IndirectContacts");
                result = string.Join(", ", keys.ToArray()) + "\n";


                var SnapData = this.SnapData( fiscalYear);

                var byUnit = SnapData.GroupBy( s => s.User.RprtngProfile.PlanningUnit.Id).Select( 
                                            d => new {
                                                Unit = d.Select( s => s.User.RprtngProfile.PlanningUnit ).First(),
                                                Revisions = d.Select( s => s.Revision )
                                            }
                                        )
                                        .OrderBy( d => d.Unit.Name);
                foreach( var unitData in byUnit ){
                    var row = fiscalYear.Name + ",";
                    row += string.Concat("\"", unitData.Unit.Name, "\"") + ",";
                    row += unitData.Revisions.Sum( s => s.Hours).ToString() + ",";

                    var male = unitData.Revisions.Sum( s => s.Male);
                    var female = unitData.Revisions.Sum( s => s.Female);

                    row += ( male + female ).ToString() + ",";
                    var revisionsWithDirectContacts = unitData.Revisions.Where( s => s.SnapDirect != null);

                    var ageAudienceValues = new List<SnapDirectAgesAudienceValue>();
                    foreach( var rev in revisionsWithDirectContacts){
                        ageAudienceValues.AddRange( rev.SnapDirect.SnapDirectAgesAudienceValues);
                    }


                    foreach( var audnc in snapDirectAudience){
                        row += ageAudienceValues.Where( a => a.SnapDirectAudienceId == audnc.Id).Sum( s => s.Value ).ToString() + ",";
                    }

                    foreach( var ags in snapDirectAges){
                        row += ageAudienceValues.Where( a => a.SnapDirectAgesId == ags.Id).Sum( s => s.Value ).ToString() + ",";
                    }

                    row += male.ToString() + ",";
                    row += female.ToString() + ",";


                    var RaceEthnicityValues = new List<RaceEthnicityValue>();

                    foreach( var rev in unitData.Revisions){
                        RaceEthnicityValues.AddRange(rev.RaceEthnicityValues);
                    }

                    foreach(var race in races){
                        foreach( var ethn in ethnicities){
                            row += RaceEthnicityValues.Where( v => v.EthnicityId == ethn.Id && v.RaceId == race.Id).Sum( s => s.Amount).ToString() + ",";
                        }
                    }


                    var withIndirect = unitData.Revisions.Where( r => r.SnapIndirect != null);
                    var indirects = 0;
                    foreach( var ind in withIndirect){
                        indirects += ind.SnapIndirect.SnapIndirectReachedValues.Sum( s => s.Value);
                    }
                    row += indirects.ToString();
                    result += row + "\n";
                }
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    }); 
            }
            return result;
        }


        public string TotalByEmployee(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapEdTotalByEmployee + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                /*********************************/

                // Build result from data source 

                /*********************************/
            
                var keys = new List<string>();
                keys.Add("FY");
                keys.Add("PlanningUnit");
                keys.Add("EmployeeName");
                keys.Add("Position");
                keys.Add("Program(s)");
                keys.Add("HoursReported");
                keys.Add("DirectContacts");

                var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);
                
                foreach( var audnc in snapDirectAudience){
                    keys.Add(audnc.Name);
                }

                var snapDirectAges = this.context.SnapDirectAges.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);

                foreach( var ags in snapDirectAges){
                    keys.Add(ags.Name);
                }

                keys.Add("Male");
                keys.Add("Female");

                var races = this.context.Race;
                var ethnicities = this.context.Ethnicity;

                foreach(var race in races){
                    foreach( var ethn in ethnicities){
                        keys.Add( race.Name + ethn.Name);
                    }
                }

                keys.Add("IndirectContacts");

                result = string.Join(", ", keys.ToArray()) + "\n";

                var SnapData = this.SnapData( fiscalYear);

                var byUser = SnapData.GroupBy( s => s.User.Id).Select( 
                                            d => new {
                                                User = d.Select( s => s.User ).First(),
                                                Revisions = d.Select( s => s.Revision )
                                            }
                                        )
                                        .OrderBy( d => d.User.RprtngProfile.PlanningUnit.Name).ThenBy( d => d.User.RprtngProfile.Name);
                foreach( var userData in byUser ){
                    var row = fiscalYear.Name + ",";
                    row += string.Concat("\"", userData.User.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                    row += string.Concat("\"", userData.User.RprtngProfile.Name, "\"") + ",";
                    row += string.Concat("\"", userData.User.ExtensionPosition.Code, "\"") + ",";
                    var spclt = "";
                    foreach( var sp in userData.User.Specialties){
                        spclt += " " + sp.Specialty.Code;
                    }
                    row += spclt + ", ";
                    row += userData.Revisions.Sum( s => s.Hours).ToString() + ",";

                    var male = userData.Revisions.Sum( s => s.Male);
                    var female = userData.Revisions.Sum( s => s.Female);

                    row += ( male + female ).ToString() + ",";
                    var revisionsWithDirectContacts = userData.Revisions.Where( s => s.SnapDirect != null);

                    var ageAudienceValues = new List<SnapDirectAgesAudienceValue>();
                    foreach( var rev in revisionsWithDirectContacts){
                        ageAudienceValues.AddRange( rev.SnapDirect.SnapDirectAgesAudienceValues);
                    }


                    foreach( var audnc in snapDirectAudience){
                        row += ageAudienceValues.Where( a => a.SnapDirectAudienceId == audnc.Id).Sum( s => s.Value ).ToString() + ",";
                    }

                    foreach( var ags in snapDirectAges){
                        row += ageAudienceValues.Where( a => a.SnapDirectAgesId == ags.Id).Sum( s => s.Value ).ToString() + ",";
                    }

                    row += male.ToString() + ",";
                    row += female.ToString() + ",";


                    var RaceEthnicityValues = new List<RaceEthnicityValue>();

                    foreach( var rev in userData.Revisions){
                        RaceEthnicityValues.AddRange(rev.RaceEthnicityValues);
                    }

                    foreach(var race in races){
                        foreach( var ethn in ethnicities){
                            row += RaceEthnicityValues.Where( v => v.EthnicityId == ethn.Id && v.RaceId == race.Id).Sum( s => s.Amount).ToString() + ",";
                        }
                    }


                    var withIndirect = userData.Revisions.Where( r => r.SnapIndirect != null);
                    var indirects = 0;
                    foreach( var ind in withIndirect){
                        indirects += ind.SnapIndirect.SnapIndirectReachedValues.Sum( s => s.Value);
                    }
                    row += indirects.ToString();
                    result += row + "\n";
                }
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    }); 
            
            
            }
            return result;
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


    }


}