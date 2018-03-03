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
    public class SnapBaseRepository
    {

        private KERScoreContext context;
        private IDistributedCache _cache;
        public SnapBaseRepository(KERScoreContext context, IDistributedCache _cache)
        { 
            this.context = context;
            this._cache = _cache;
        }

        protected List<UserRevisionData> SnapData( FiscalYear fiscalYear){
            List<UserRevisionData> SnapData;
            var cacheKeyData = CacheKeys.SnapData + fiscalYear.Name;
            var cacheStringData = _cache.GetString(cacheKeyData);
            if (!string.IsNullOrEmpty(cacheStringData)){
                SnapData = JsonConvert.DeserializeObject<List<UserRevisionData>>(cacheStringData);
            }else{
                var today = DateTime.Now;            
                var snapEligible = RevisionsWithSnapData(fiscalYear);
                SnapData = new List<UserRevisionData>();
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
                                        .Include( a => a.KersUser ).ThenInclude( u => u.PersonalProfile)
                                        .Include( a => a.KersUser).ThenInclude( u => u.Specialties).ThenInclude( s => s.Specialty)
                                        .FirstOrDefault();
                        var revision = context.ActivityRevision.Where( r => r.Id == rev.Id)
                                            .Include( s => s.SnapDirect).ThenInclude( d => d.SnapDirectAgesAudienceValues )
                                            .Include( s => s.SnapIndirect).ThenInclude( i => i.SnapIndirectReachedValues)
                                            .Include( s => s.SnapPolicy)
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

                var serializedData = JsonConvert.SerializeObject(SnapData);
                _cache.SetString(cacheKeyData, serializedData, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3)
                    });
            }
            return SnapData;
        }

        protected List<int> LastActivityRevisionIds( FiscalYear fiscalYear ){
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

        protected IEnumerable<ActivityRevision> RevisionsWithSnapData(FiscalYear fiscalYear){
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