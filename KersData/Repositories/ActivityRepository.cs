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
    public class ActivityRepository : EntityBaseRepository<Activity>, IActivityRepository
    {

        private KERScoreContext coreContext;
        public ActivityRepository(KERScoreContext context)
            : base(context)
        { 
            this.coreContext = context;
        }


        public List<int> LastActivityRevisionIds( FiscalYear fiscalYear, IDistributedCache _cache){
            var cacheKey = "ActivityLastRevisionIdsPerFiscalYear" + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            List<int> ids;
            if (!string.IsNullOrEmpty(cacheString)){
                ids = JsonConvert.DeserializeObject<List<int>>(cacheString);
            }else{
                ids = new List<int>();
                var activities = coreContext.Activity.
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

        public List<ActivityRevision> PerMonth(KersUser user, int year, int month, string order = "desc"){
            
            IOrderedQueryable<Activity> lastActivities;
            if(order == "desc"){
                lastActivities = coreContext.Activity.
                                Where(e=>e.KersUser == user && e.ActivityDate.Month == month && e.ActivityDate.Year == year).
                                Include(e=>e.Revisions).ThenInclude(r => r.MajorProgram).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionSelections).ThenInclude(s => s.ActivityOption).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionNumbers).ThenInclude(s => s.ActivityOptionNumber).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).
                                OrderByDescending(e=>e.ActivityDate);
            }else{
                lastActivities = coreContext.Activity.
                                Where(e=>e.KersUser == user && e.ActivityDate.Month == month && e.ActivityDate.Year == year).
                                Include(e=>e.Revisions).ThenInclude(r => r.MajorProgram).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionSelections).ThenInclude(s => s.ActivityOption).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionNumbers).ThenInclude(s => s.ActivityOptionNumber).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).
                                OrderBy(e=>e.ActivityDate);
            }
            
            var revs = new List<ActivityRevision>();
            if( lastActivities != null){
                foreach(var expense in lastActivities){
                    if(expense.Revisions.Count != 0){
                        revs.Add( expense.Revisions.OrderBy(r=>r.Created).Last() );
                    }
                }
            }
            return revs;
        }



        public List<PerUnitActivities> ProcessUnitActivities(List<ActivityUnitResult> activities, IDistributedCache _cache){
            var result = new List<PerUnitActivities>();
            foreach( var unt in activities){
                var unitRevisions = new List<ActivityRevision>();
                var OptionNumbers = new List<IOptionNumberValue>();
                var RaceEthnicities = new List<IRaceEthnicityValue>();
                foreach( var rev in unt.Ids){
                    var cacheKey = "ActivityLastRevision" + rev.ToString();
                    

                    var cacheString = _cache.GetString(cacheKey);

                    ActivityRevision lstrvsn;
                    if (!string.IsNullOrEmpty(cacheString)){
                        lstrvsn = JsonConvert.DeserializeObject<ActivityRevision>(cacheString);
                    }else{
                        lstrvsn = coreContext.ActivityRevision.
                            Where(r => r.ActivityId == rev).
                            Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                            Include(a => a.ActivityOptionSelections).ThenInclude( s => s.ActivityOption).
                            Include(a => a.RaceEthnicityValues).
                            OrderBy(a => a.Created).Last();
                        var serialized = JsonConvert.SerializeObject(lstrvsn);
                        _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                            });
                    }
                    unitRevisions.Add(lstrvsn);
                    OptionNumbers.AddRange(lstrvsn.ActivityOptionNumbers);
                    RaceEthnicities.AddRange(lstrvsn.RaceEthnicityValues);
                }
                var actvts = new PerUnitActivities();
                actvts.RaceEthnicityValues = RaceEthnicities;
                actvts.OptionNumberValues = OptionNumbers;
                actvts.Hours = unt.Hours;
                actvts.Audience = unt.Audience;
                actvts.PlanningUnit = unt.Unit;
                actvts.Multistate = unitRevisions.Where( r => r.ActivityOptionSelections.Where( s => s.ActivityOption.Name == "Multistate effort?").Count() > 0).Sum(s => s.Hours);
                result.Add(actvts);
            }

            return result;
        }


        public List<PerPersonActivities> ProcessPersonActivities(List<ActivityPersonResult> activities, IDistributedCache _cache){
            var result = new List<PerPersonActivities>();
            foreach( var unt in activities){
                var unitRevisions = new List<ActivityRevision>();
                var OptionNumbers = new List<IOptionNumberValue>();
                var RaceEthnicities = new List<IRaceEthnicityValue>();
                foreach( var rev in unt.Ids){
                    var cacheKey = "ActivityLastRevision" + rev.ToString();
                    

                    var cacheString = _cache.GetString(cacheKey);

                    ActivityRevision lstrvsn;
                    if (!string.IsNullOrEmpty(cacheString)){
                        lstrvsn = JsonConvert.DeserializeObject<ActivityRevision>(cacheString);
                    }else{
                        lstrvsn = coreContext.ActivityRevision.
                            Where(r => r.ActivityId == rev).
                            Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                            Include(a => a.ActivityOptionSelections).ThenInclude( s => s.ActivityOption).
                            Include(a => a.RaceEthnicityValues).
                            OrderBy(a => a.Created).Last();
                        var serialized = JsonConvert.SerializeObject(lstrvsn);
                        _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                            });
                    }
                    unitRevisions.Add(lstrvsn);
                    OptionNumbers.AddRange(lstrvsn.ActivityOptionNumbers);
                    RaceEthnicities.AddRange(lstrvsn.RaceEthnicityValues);
                }
                var user = this.coreContext.
                                    KersUser.Where( u => u == unt.KersUser)
                                    .Include( u => u.PersonalProfile)
                                    .Include(u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit)
                                    .First();
                var actvts = new PerPersonActivities();
                actvts.RaceEthnicityValues = RaceEthnicities;
                actvts.OptionNumberValues = OptionNumbers;
                actvts.Hours = unt.Hours;
                actvts.Audience = unt.Audience;
                actvts.KersUser = user;
                actvts.Multistate = unitRevisions.Where( r => r.ActivityOptionSelections.Where( s => s.ActivityOption.Name == "Multistate effort?").Count() > 0).Sum(s => s.Hours);
                result.Add(actvts);
            }

            return result;
        }



        public List<PerProgramActivities> ProcessMajorProgramActivities(List<ActivityMajorProgramResult> activities, IDistributedCache _cache){
            var result = new List<PerProgramActivities>();
            foreach( var unt in activities){
                var unitRevisions = new List<ActivityRevision>();
                var OptionNumbers = new List<IOptionNumberValue>();
                var RaceEthnicities = new List<IRaceEthnicityValue>();
                foreach( var rev in unt.Ids){
                    var cacheKey = "ActivityLastRevision" + rev.ToString();
                    

                    var cacheString = _cache.GetString(cacheKey);

                    ActivityRevision lstrvsn;
                    if (!string.IsNullOrEmpty(cacheString)){
                        lstrvsn = JsonConvert.DeserializeObject<ActivityRevision>(cacheString);
                    }else{
                        lstrvsn = coreContext.ActivityRevision.
                            Where(r => r.ActivityId == rev).
                            Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                            Include(a => a.ActivityOptionSelections).ThenInclude( s => s.ActivityOption).
                            Include(a => a.RaceEthnicityValues).
                            OrderBy(a => a.Created).Last();
                        var serialized = JsonConvert.SerializeObject(lstrvsn);
                        _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                            });
                    }
                    unitRevisions.Add(lstrvsn);
                    OptionNumbers.AddRange(lstrvsn.ActivityOptionNumbers);
                    RaceEthnicities.AddRange(lstrvsn.RaceEthnicityValues);
                }
                var actvts = new PerProgramActivities();
                actvts.RaceEthnicityValues = RaceEthnicities;
                actvts.OptionNumberValues = OptionNumbers;
                actvts.Hours = unt.Hours;
                actvts.Audience = unt.Audience;
                actvts.MajorProgram = unt.MajorProgram;
                actvts.Multistate = unitRevisions.Where( r => r.ActivityOptionSelections.Where( s => s.ActivityOption.Name == "Multistate effort?").Count() > 0).Sum(s => s.Hours);
                result.Add(actvts);
            }

            return result;
        }


    }


}