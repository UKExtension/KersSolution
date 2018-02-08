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


        private List<UserRevisionData> SnapData( FiscalYear fiscalYear){
            var today = DateTime.Now;
            var revs = LastActivityRevisionIds(fiscalYear);

            var snapEligible = context.ActivityRevision.Where( r => revs.Contains( r.Id ) &&  (r.SnapPolicy != null || r.SnapDirect != null || r.SnapIndirect != null || r.SnapAdmin ));


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


    }


}