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
    public class SnapDirectRepository : EntityBaseRepository<SnapDirect>, ISnapDirectRepository
    {

        private KERScoreContext context;
        private IDistributedCache _cache;
        public SnapDirectRepository(KERScoreContext context, IDistributedCache _cache)
            : base(context)
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


        private List<UserRevisionData> SnapData( FiscalYear fiscalYear){
            var today = DateTime.Now;
           
            var snapEligible = RevisionsWithSnapData(fiscalYear);


            List<UserRevisionData> SnapData = new List<UserRevisionData>();

            foreach( var rev in snapEligible ){


                
                var cacheKey = CacheKeys.UserRevisionWithSnapData + rev.Id.ToString();
                var cacheString = _cache.GetString(cacheKey);
                UserRevisionData data;
                if (!string.IsNullOrEmpty(cacheString)){
                    data = JsonConvert.DeserializeObject<UserRevisionData>(cacheString);
                }else{
                    
                    data = new UserRevisionData();
                    var activity = context.Activity.Where( a => a.Id == rev.ActivityId )
                                    .Include( a => a.KersUser ).ThenInclude( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                                    .Include( a => a.KersUser ).ThenInclude( u => u.ExtensionPosition)
                                    .Include( a => a.KersUser).ThenInclude( u => u.Specialties).ThenInclude( s => s.Specialty)
                                    .FirstOrDefault();
                    var revision = context.ActivityRevision.Where( r => r.Id == rev.Id)
                                        .Include( s => s.SnapDirect).ThenInclude( d => d.SnapDirectAgesAudienceValues )
                                        .Include( s => s.SnapIndirect).ThenInclude( i => i.SnapIndirectReachedValues)
                                        .Include( s => s.RaceEthnicityValues)
                                        .Include( s => s.ActivityOptionNumbers).FirstOrDefault();
                    
                    data.User = activity.KersUser;
                    data.Revision = revision;


                    var expiration = (today - activity.ActivityDate).Days;
                    if( expiration < 1){
                        expiration = 1;
                    }

                    var serialized = JsonConvert.SerializeObject(data);
                    _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( expiration )
                        }); 
                }

                
                SnapData.Add(data);
            }
            return SnapData;
        }

        private List<int> LastActivityRevisionIds( FiscalYear fiscalYear ){
            var cacheKey = CacheKeys.ActivityLastRevisionIdsPerFiscalYear + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            List<int> ids;
            if (!string.IsNullOrEmpty(cacheString)){
                ids = JsonConvert.DeserializeObject<List<int>>(cacheString);
            }else{
                ids = new List<int>();
                var activities = context.Activity.
                    Where(r => r.ActivityDate > fiscalYear.Start && r.ActivityDate < fiscalYear.End)
                    .Include( r => r.Revisions);
                foreach( var actvt in activities){
                    var rev = actvt.Revisions.OrderBy( r => r.Created );
                    var last = rev.Last();
                    ids.Add(last.Id);
                }
                    
                var serialized = JsonConvert.SerializeObject(ids);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });
            }
            return ids;
        }

        private IEnumerable<ActivityRevision> RevisionsWithSnapData(FiscalYear fiscalYear){
            var revs = LastActivityRevisionIds(fiscalYear);
            // Divide revs into batches as SQL server is having trouble to process more then several thousands at once
            var fyactivities = new List<ActivityRevision>();
            var batchCount = 10000;
            for(var i = 0; i <= revs.Count(); i += batchCount){
                var currentBatch = revs.Skip(i).Take(batchCount);
                fyactivities.AddRange(context.ActivityRevision.Where( r => currentBatch.Contains( r.Id )).ToList());
            }
            
            var snapEligible = fyactivities.Where( r => (r.SnapPolicyId != null || r.SnapDirectId != null || r.SnapIndirectId != null || (r.SnapAdmin && r.isSnap) ));
            return snapEligible;
        }


    }


}