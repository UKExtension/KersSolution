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
                                        .Include( a => a.KersUser ).ThenInclude( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit).ThenInclude( u => u.District)
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
            var snapEligible = fyactivities.Where( r => r.isSnap );
            // Check if activity is by UK employee
            var UKRevisions = new List<ActivityRevision>();
            foreach( var rev in snapEligible ){
                if(context.Activity.Where( a => a.Id == rev.ActivityId && a.KersUser.RprtngProfile.Institution.Code == "21000-1862").Any()){
                    UKRevisions.Add( rev );
                }
            }


            return UKRevisions;
        }

        protected List<ActivityRevisionsPerMonth> RevisionsWithDirectContactsPerMonth( FiscalYear fiscalYear){
            var perMonth = new List<ActivityRevisionsPerMonth>();
            var currentDate = DateTime.Now;
            var runningDate = fiscalYear.Start;
            var difference = (int)Math.Floor(fiscalYear.End.Subtract(fiscalYear.Start).Days / (365.2425 / 12)) + 1;
            var months = new DateTime[difference];
            var i = 0;
            do{
                months[i] = runningDate.AddMonths( i );
                if( months[i].Year < currentDate.Year || ( months[i].Year == currentDate.Year && months[i].Month <= currentDate.Month ) ){
                    var cacheKey = "DirectActivityRevisionsPerMonth" + months[i].Month.ToString() + months[i].Year.ToString();
                    var cachedTypes = _cache.GetString(cacheKey);
                    var entity = new ActivityRevisionsPerMonth();
                    if (!string.IsNullOrEmpty(cachedTypes)){
                        entity = JsonConvert.DeserializeObject<ActivityRevisionsPerMonth>(cachedTypes);
                    }else{
                        var byMonth = context.Activity.Where( c => 
                                                                    c.ActivityDate.Month == months[i].Month 
                                                                    &&
                                                                    c.ActivityDate.Year == months[i].Year
                                                                    &&
                                                                    c.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                                )
                                                                .Include( a => a.Revisions);
                        entity.Revs = new List<ActivityRevision>();
                        foreach( var act in byMonth){
                            entity.Revs.Add(act.Revisions.OrderBy( a => a.Created.ToString("s")).Last());
                        }
                        /* 
                        var activityRevisionsPerMonty = byMonth
                                .Select( a => a.Revisions.OrderBy(r => r.Created).Last())
                                .Where( e => e.SnapDirect != null)
                                .ToList();
                        entity.Revs = activityRevisionsPerMonty;
 */
                        entity.Revs = entity.Revs.Where(r => r.SnapDirectId != null ).ToList();
                        entity.Month = months[i].Month;
                        entity.Year = months[i].Year;

                        var yearDifference = currentDate.Year - months[i].Year;
                        var monthsDifference = currentDate.Month - months[i].Month;

                        var cachePeriod = Math.Floor( (float) (yearDifference * 10 + monthsDifference + 5) / 2 );
                        if(cachePeriod <= 0){
                            cachePeriod = 1;
                        }

                        var serializedActivities = JsonConvert.SerializeObject(entity);
                        _cache.SetString(cacheKey, serializedActivities, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cachePeriod)
                            });           
                    }
                    perMonth.Add(entity);
                }
                i++;
            }while(i < difference);

            return perMonth;
        }

        protected List<ActivityRevisionsPerMonth> RevisionsWithIndirectContactsPerMonth( FiscalYear fiscalYear){
            var perMonth = new List<ActivityRevisionsPerMonth>();
            var currentDate = DateTime.Now;
            var runningDate = fiscalYear.Start;
            var difference = (int)Math.Floor(fiscalYear.End.Subtract(fiscalYear.Start).Days / (365.2425 / 12)) + 1;
            var months = new DateTime[difference];
            var i = 0;
            do{
                months[i] = runningDate.AddMonths( i );
                if( months[i].Year < currentDate.Year || ( months[i].Year == currentDate.Year && months[i].Month <= currentDate.Month ) ){
                    var cacheKey = "IndirectActivityRevisionsPerMonth" + months[i].Month.ToString() + months[i].Year.ToString();
                    var cachedTypes = _cache.GetString(cacheKey);
                    var entity = new ActivityRevisionsPerMonth();
                    if (!string.IsNullOrEmpty(cachedTypes)){
                        entity = JsonConvert.DeserializeObject<ActivityRevisionsPerMonth>(cachedTypes);
                    }else{
                        var byMonth = context.Activity.Where( c => 
                                                                    c.ActivityDate.Month == months[i].Month 
                                                                    &&
                                                                    c.ActivityDate.Year == months[i].Year
                                                                    &&
                                                                    c.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                            )
                                                                .Include( a => a.Revisions);
                        entity.Revs = new List<ActivityRevision>();
                        foreach( var act in byMonth){
                            entity.Revs.Add(act.Revisions.OrderBy( a => a.Created.ToString("s")).Last());
                        }
                        /* 
                        var activityRevisionsPerMonty = byMonth
                                .Select( a => a.Revisions.OrderBy(r => r.Created).Last())
                                .Where( e => e.SnapDirect != null)
                                .ToList();
                        entity.Revs = activityRevisionsPerMonty;
 */
                        entity.Revs = entity.Revs.Where(r => r.SnapIndirectId != null ).ToList();
                        entity.Month = months[i].Month;
                        entity.Year = months[i].Year;

                        var yearDifference = currentDate.Year - months[i].Year;
                        var monthsDifference = currentDate.Month - months[i].Month;

                        var cachePeriod = Math.Floor( (float) (yearDifference * 10 + monthsDifference + 5) / 2 );
                        if(cachePeriod <= 0){
                            cachePeriod = 1;
                        }

                        var serializedActivities = JsonConvert.SerializeObject(entity);
                        _cache.SetString(cacheKey, serializedActivities, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cachePeriod)
                            });
                                    
                    }
                    perMonth.Add(entity);
                }
                i++;
            }while(i < difference);

            return perMonth;
        }

        protected int getCacheSpan(FiscalYear fiscalYear){
            int cacheDaysSpan = 350;
            var today = DateTime.Now;
            if(fiscalYear.Start < today && Math.Max( fiscalYear.End.Ticks, fiscalYear.ExtendedTo.Ticks) > today.Ticks){
                cacheDaysSpan = 3;
            }else if(fiscalYear.Start > today){
                cacheDaysSpan = 1;
            }
            return cacheDaysSpan;
        }

        protected string StripHTML(string htmlString){

            string pattern = @"<(.|\n)*?>";

            return Regex.Replace(htmlString, pattern, string.Empty);
        }

        


    }


}