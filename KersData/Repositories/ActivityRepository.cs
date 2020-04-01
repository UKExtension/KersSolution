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
using Kers.Models.ViewModels;
using System.Text.RegularExpressions;

namespace Kers.Models.Repositories
{
    public class ActivityRepository : EntityBaseRepository<Activity>, IActivityRepository
    {

        private KERScoreContext coreContext;
        private IDistributedCache _cache;
        const int workDaysPerYear = 228;
        public ActivityRepository(
            IDistributedCache _cache,
            KERScoreContext context
            ) : base(context)
        { 
            this.coreContext = context;
            this._cache = _cache;
        }



        public async Task<List<ProgramDataViewModel>> TopProgramsPerMonth(int year = 0, int month = 0, int amount = 5, int PlanningUnitId = 0, bool refreshCache = false){
            
            
            
            List<ProgramDataViewModel> data;


            // If no month or year is provided, get the last month
            if( year == 0 || month == 0){
                var currentDate = DateTime.Now;
                month = currentDate.Month;
                if( month == 1 ){
                    year = currentDate.Year - 1;
                    month = 12;
                }else{
                    year = currentDate.Year;
                    month = currentDate.Month - 1;
                }
            }

            /* 
            var cacheKey = CacheKeys.StatsPerMonth + month.ToString() + year.ToString() + PlanningUnitId.ToString() + MajorProgramId.ToString();
            var cachedStats = _cache.GetString(cacheKey);
            StatsViewModel stats;
            if (!string.IsNullOrEmpty(cachedStats) && !refreshCache){
                stats = JsonConvert.DeserializeObject<StatsViewModel>(cachedStats);
            }else{
            */
            var firstDay = new DateTime( year, month, 1, 0, 0, 0);
            var lastDay = new DateTime( year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

            var activities = this.coreContext.Activity.Where( a => a.ActivityDate > firstDay && a.ActivityDate < lastDay );

            if( PlanningUnitId != 0) {
                activities = activities.Where( a => a.PlanningUnitId == PlanningUnitId );
            }

            // Exclude Administrative Functions and PSD programs

            activities = activities.Where( a => a.MajorProgram.Name != "Administrative Functions" && a.MajorProgram.Name != "Staff Development");


            data = await activities.GroupBy( a => a.MajorProgram )
                                .Select( g => new ProgramDataViewModel {
                                    Program = g.Key,
                                    DirectContacts = g.Sum( a => a.Audience ),
                                    Hours = g.Sum( a => a.Hours )
                                })
                                .OrderByDescending( g => g.DirectContacts )
                                .Take( amount )
                                .ToListAsync();
            return data;
        }




        public async Task<List<ProgramDataViewModel>> TopProgramsPerFiscalYear(FiscalYear FiscalYear, int amount = 5, int PlanningUnitId = 0, bool refreshCache = false){
            
            
            
            List<ProgramDataViewModel> data;

            var cacheKey = CacheKeys.TopProgramsPerFiscalYear + FiscalYear.Name;
            var cachedStats = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cachedStats) && !refreshCache){
                data = JsonConvert.DeserializeObject<List<ProgramDataViewModel>>(cachedStats);
            }else{

                var activities = this.coreContext.Activity.Where( a => a.ActivityDate > FiscalYear.Start && a.ActivityDate < FiscalYear.End );

                if( PlanningUnitId != 0) {
                    activities = activities.Where( a => a.PlanningUnitId == PlanningUnitId );
                }

                // Exclude Administrative Functions and PSD programs

                activities = activities.Where( a => a.MajorProgram.Name != "Administrative Functions" && a.MajorProgram.Name != "Staff Development");


                data = await activities.GroupBy( a => a.MajorProgram )
                                    .Select( g => new ProgramDataViewModel {
                                        Program = g.Key,
                                        DirectContacts = g.Sum( a => a.Audience ),
                                        Hours = g.Sum( a => a.Hours )
                                    })
                                    .OrderByDescending( g => g.DirectContacts )
                                    .Take( amount )
                                    .ToListAsync();
                var serialized = JsonConvert.SerializeObject(data);

                var keepCacheInDays = 3;
                if( FiscalYear.End < DateTime.Now ){
                        keepCacheInDays = 200;
                }
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(keepCacheInDays)
                    });
            }
            return data;
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


        public TableViewModel ReportsStateAll(FiscalYear fiscalYear, bool refreshCache = false){

            var cacheKey = CacheKeys.StateAllContactsData + fiscalYear.Name;
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes) && !refreshCache){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{
                var actvtsCacheKey = CacheKeys.AllActivitiesByPlanningUnit + fiscalYear.Name;
                var cachedActivities = _cache.GetString(actvtsCacheKey);
                List<ActivityUnitResult> activities;
                if (!string.IsNullOrEmpty(cachedActivities)){
                    activities = JsonConvert.DeserializeObject<List<ActivityUnitResult>>(cachedActivities);
                }else{
                    activities = coreContext.Activity
                                                    .Where( a => a.ActivityDate < fiscalYear.End && a.ActivityDate > fiscalYear.Start)
                                                    .GroupBy(e => new {
                                                        Unit = e.PlanningUnit
                                                    })
                                                    .Select(c => new ActivityUnitResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        Unit = c.Key.Unit
                                                    })
                                                    .ToList();


                    foreach( var activity in activities){
                        var males = 0;
                        var females = 0;
                        foreach( var perUserId in activity.Ids ){
                            var last = coreContext.ActivityRevision.Where( a => a.ActivityId == perUserId ).OrderBy( a => a.Created ).Last();
                            males += last.Male;
                            females += last.Female;
                        }
                        activity.Male = males;
                        activity.Female = females;
                    }

                    var serializedActivities = JsonConvert.SerializeObject(activities);
                    _cache.SetString(actvtsCacheKey, serializedActivities, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
                        });
                                
                }
                var result = ProcessUnitActivities( activities, _cache);


                var contactsCacheKey = CacheKeys.AllContactsByPlanningUnit + fiscalYear.Name;
                var cachedContacts = _cache.GetString(contactsCacheKey);
                List<ContactUnitResult> contacts;
                if (!string.IsNullOrEmpty(cachedContacts)){
                    contacts = JsonConvert.DeserializeObject<List<ContactUnitResult>>(cachedContacts);
                }else{
                    contacts = coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < fiscalYear.End 
                                                && 
                                                c.ContactDate > fiscalYear.Start 
                                                && 
                                                c.PlanningUnit != null
                                        )
                                        .GroupBy(e => new {
                                            Unit = e.PlanningUnit
                                        })
                                        .Select(c => new ContactUnitResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            Unit = c.Key.Unit
                                        })
                                        .ToList();
                    var serializedContacts = JsonConvert.SerializeObject(contacts);
                    _cache.SetString(contactsCacheKey, serializedContacts, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
                        });
                                
                }
                result = ProcessUnitContacts( contacts, result);
                result = result.OrderBy( r => r.PlanningUnit.order).ToList();
                table = new TableViewModel();
                table.Header = new List<string>{
                            "Planning Unit", "Days", "Multistate", "Total Contacts"
                        };
                var Races = coreContext.Race.OrderBy(r => r.Order);
                var Ethnicities = coreContext.Ethnicity.OrderBy( e => e.Order);
                var OptionNumbers = coreContext.ActivityOptionNumber.OrderBy( n => n.Order);
                foreach( var race in Races){
                    table.Header.Add(race.Name);
                }
                foreach( var ethn in Ethnicities){
                    table.Header.Add(ethn.Name);
                }
                table.Header.Add("Males");
                table.Header.Add("Females");
                foreach( var opnmb in OptionNumbers){
                    table.Header.Add(opnmb.Name);
                }
                var Rows = new List<List<string>>();
                float TotalHours = 0;
                float TotalMultistate = 0;
                int TotalAudience = 0;
                int ToatalMales = 0;
                int TotalFemales = 0;
                int[] totalPerRace = new int[Races.Count()];
                int[] totalPerEthnicity = new int[Ethnicities.Count()];
                int[] totalPerOptionNumber = new int[OptionNumbers.Count()];
                int i = 0;
                foreach(var res in result){
                    TotalHours += res.Hours;
                    TotalAudience += res.Audience;
                    ToatalMales += res.Male;
                    TotalFemales += res.Female;
                    TotalMultistate += res.Multistate;
                    var Row = new List<string>();
                    Row.Add(res.PlanningUnit.Name);
                    Row.Add((res.Hours / 8).ToString());
                    Row.Add((res.Multistate / 8).ToString());
                    Row.Add(res.Audience.ToString());
                    i = 0;
                    foreach( var race in Races){
                        var raceAmount = res.RaceEthnicityValues.Where( v => v.RaceId == race.Id).Sum( r => r.Amount);
                        Row.Add(raceAmount.ToString());
                        totalPerRace[i] += raceAmount;
                        i++;
                    }
                    i=0;
                    foreach( var et in Ethnicities){
                        var ethnAmount = res.RaceEthnicityValues.Where( v => v.EthnicityId == et.Id).Sum( r => r.Amount);
                        Row.Add(ethnAmount.ToString());
                        totalPerEthnicity[i] += ethnAmount;
                        i++;
                    }
                    Row.Add(res.Male.ToString());
                    Row.Add(res.Female.ToString());
                    i=0;
                    foreach( var opnmb in OptionNumbers){
                        var optNmbAmount = res.OptionNumberValues.Where( o => o.ActivityOptionNumberId == opnmb.Id).Sum( s => s.Value);
                        Row.Add( optNmbAmount.ToString());
                        totalPerOptionNumber[i] += optNmbAmount;
                        i++;
                    }
                    Rows.Add(Row);
                }
                table.Rows = Rows;
                table.Foother = new List<string>{
                            "Total", (TotalHours / 8).ToString(), (TotalMultistate / 8).ToString(), TotalAudience.ToString()
                        };
                i = 0;
                foreach( var race in Races){
                    table.Foother.Add(totalPerRace[i].ToString());
                    i++;
                }
                i = 0;
                foreach( var et in Ethnicities){
                    table.Foother.Add(totalPerEthnicity[i].ToString());
                    i++;
                }
                table.Foother.Add(ToatalMales.ToString());
                table.Foother.Add(TotalFemales.ToString());
                i = 0;
                    foreach( var opnmb in OptionNumbers){
                    table.Foother.Add( totalPerOptionNumber[i].ToString());
                    i++;
                }
                var serialized = JsonConvert.SerializeObject(table);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(23)
                    });
            }
            return table;
        } 


        public async Task<TableViewModel> ContactsByCountyByMajorProgram(FiscalYear fiscalYear, bool refreshCache = false)
        {

            var cacheKey = CacheKeys.ActivityContactsByCountyByMajorProgram + fiscalYear.Name;
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes) && !refreshCache){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{
                var counties = await this.coreContext.PlanningUnit
                                        .Where( u => 
                                                    u.District != null
                                                    &&
                                                    u.Name.Substring( u.Name.Length - 3) == "CES"
                                            )
                                        .OrderBy( u => u.Name)
                                        .ToListAsync();
                var majorPrograms = await this.coreContext.MajorProgram
                                        .Where( u => 
                                                    
                                                    u.StrategicInitiative.FiscalYear == fiscalYear
                                            )
                                        .OrderBy( u => u.Name)
                                        .ToListAsync();
                var header = new List<string>();
                header.Add( "Counties" );
                foreach( var program in majorPrograms){
                    header.Add(program.Name);
                }
                var rows = new List<List<string>>();
                foreach( var county in counties ){
                    var row = new List<string>();
                    row.Add( county.Name );

                    // Add county numbers
                    foreach( var prgrm in majorPrograms){
                        var sm = coreContext.Activity.Where( a => a.MajorProgramId == prgrm.Id && a.PlanningUnitId == county.Id && a.ActivityDate.Year < 2018).Sum(s => s.Audience);
                        row.Add(sm.ToString());
                    }

                    rows.Add(row);

                }
                table = new TableViewModel();
                table.Header = header;
                table.Rows = rows;
                table.Foother = new List<string>();
                var serialized = JsonConvert.SerializeObject(table);
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(13)
                    });
            }
            return table;
        }
        // type: 0 - all, 1 - UK, 2 - KSU
        public async Task<TableViewModel> StateByMajorProgram(FiscalYear fiscalYear, int type = 0, bool refreshCache = false)
        {

            var cacheKey = CacheKeys.StateByMajorProgram + type.ToString() + fiscalYear.Name;
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes) && !refreshCache){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{


                var actvtsCacheKey = "AllActivitiesByMajorProgram" + type.ToString() + fiscalYear.Name;
                var cachedActivities = _cache.GetString(actvtsCacheKey);
                List<ActivityMajorProgramResult> activities;
                if (!string.IsNullOrEmpty(cachedActivities) && !refreshCache){
                    activities = JsonConvert.DeserializeObject<List<ActivityMajorProgramResult>>(cachedActivities);
                }else{

                    // Only UK
                    if(type == 1){
                        activities = await this.coreContext.Activity
                                                    .Where( a =>    a.ActivityDate < fiscalYear.End 
                                                                    && 
                                                                    a.ActivityDate > fiscalYear.Start
                                                                    &&
                                                                    a.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                                    )
                                                    .GroupBy(e => new {
                                                        Program = e.MajorProgram
                                                    })
                                                    .Select(c => new ActivityMajorProgramResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        MajorProgram = c.Key.Program
                                                    })
                                                    .ToListAsync();
                    // Only KSU
                    }else if( type == 2){
                        activities = await this.coreContext.Activity
                                                    .Where( a =>    a.ActivityDate < fiscalYear.End 
                                                                    && 
                                                                    a.ActivityDate > fiscalYear.Start
                                                                    &&
                                                                    a.KersUser.RprtngProfile.Institution.Code == "21000-1890")
                                                    .GroupBy(e => new {
                                                        Program = e.MajorProgram
                                                    })
                                                    .Select(c => new ActivityMajorProgramResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        MajorProgram = c.Key.Program
                                                    })
                                                    .ToListAsync();
                    }else{
                        activities = await this.coreContext.Activity
                                                    .Where( a => a.ActivityDate < fiscalYear.End && a.ActivityDate > fiscalYear.Start)
                                                    .GroupBy(e => new {
                                                        Program = e.MajorProgram
                                                    })
                                                    .Select(c => new ActivityMajorProgramResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        MajorProgram = c.Key.Program
                                                    })
                                                    .ToListAsync();
                    }
                    foreach( var activity in activities){
                        var males = 0;
                        var females = 0;
                        foreach( var perUserId in activity.Ids ){
                            var last = coreContext.ActivityRevision.Where( a => a.ActivityId == perUserId ).OrderBy( a => a.Created ).Last();
                            males += last.Male;
                            females += last.Female;
                        }
                        activity.Male = males;
                        activity.Female = females;
                    }


                    
                    var serializedActivities = JsonConvert.SerializeObject(activities);
                    _cache.SetString(actvtsCacheKey, serializedActivities, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                        });
                                
                }
                var result = ProcessMajorProgramActivities( activities, _cache);


                var contactsCacheKey = "AllContactsByMajorProgram" + type.ToString() + "_" + fiscalYear.Name;
                var cachedContacts = _cache.GetString(contactsCacheKey);
                List<ContactMajorProgramResult> contacts;
                if (!string.IsNullOrEmpty(cachedContacts) && !refreshCache){
                    contacts = JsonConvert.DeserializeObject<List<ContactMajorProgramResult>>(cachedContacts);
                }else{
                    if(type == 1){
                        contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < fiscalYear.End 
                                                && 
                                                c.ContactDate > fiscalYear.Start 
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                        )
                                        .GroupBy(e => new {
                                            Program = e.MajorProgram
                                        })
                                        .Select(c => new ContactMajorProgramResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            MajorProgram = c.Key.Program
                                        })
                                        .ToListAsync();
                    }else if( type == 2){
                        contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < fiscalYear.End 
                                                && 
                                                c.ContactDate > fiscalYear.Start 
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code == "21000-1890"
                                        )
                                        .GroupBy(e => new {
                                            Program = e.MajorProgram
                                        })
                                        .Select(c => new ContactMajorProgramResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            MajorProgram = c.Key.Program
                                        })
                                        .ToListAsync();
                    }else{
                        contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < fiscalYear.End 
                                                && 
                                                c.ContactDate > fiscalYear.Start 
                                        )
                                        .GroupBy(e => new {
                                            Program = e.MajorProgram
                                        })
                                        .Select(c => new ContactMajorProgramResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            MajorProgram = c.Key.Program
                                        })
                                        .ToListAsync();
                    }

                    

                     
                    var serializedContacts = JsonConvert.SerializeObject(contacts);
                    _cache.SetString(contactsCacheKey, serializedContacts, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(21)
                        });
                                
                }
                result = ProcessMajorProgramContacts( contacts, result, _cache);
                result = result.OrderBy( r => r.MajorProgram.PacCode).ToList();
                table = new TableViewModel();
                table.Header = new List<string>{
                            "Major Program", "Days", "FTE", "Multistate", "Total Contacts"
                        };
                var Races = this.coreContext.Race.OrderBy(r => r.Order);
                var Ethnicities = this.coreContext.Ethnicity.OrderBy( e => e.Order);
                var OptionNumbers = this.coreContext.ActivityOptionNumber.OrderBy( n => n.Order);
                foreach( var race in Races){
                    table.Header.Add(race.Name);
                }
                foreach( var ethn in Ethnicities){
                    table.Header.Add(ethn.Name);
                }
                table.Header.Add("Male");
                table.Header.Add("Female");
                foreach( var opnmb in OptionNumbers){
                    table.Header.Add(opnmb.Name);
                }
                var Rows = new List<List<string>>();
                float TotalHours = 0;
                float TotalMultistate = 0;
                int TotalAudience = 0;
                int TotalMale = 0;
                int TotalFemale = 0;
                int[] totalPerRace = new int[Races.Count()];
                int[] totalPerEthnicity = new int[Ethnicities.Count()];
                int[] totalPerOptionNumber = new int[OptionNumbers.Count()];
                int i = 0;
                foreach(var res in result){
                    TotalHours += res.Hours;
                    TotalAudience += res.Audience;
                    TotalMale += res.Male;
                    TotalFemale += res.Female;
                    TotalMultistate += res.Multistate;
                    var Row = new List<string>();
                    Row.Add(res.MajorProgram.Name + " (" + res.MajorProgram.PacCode + ")");
                    Row.Add((res.Hours / 8).ToString());
                    Row.Add( (res.Hours / (8 * workDaysPerYear) ).ToString("0.000"));
                    Row.Add((res.Multistate / 8).ToString());
                    Row.Add(res.Audience.ToString());
                    i = 0;
                    foreach( var race in Races){
                        var raceAmount = res.RaceEthnicityValues.Where( v => v.RaceId == race.Id).Sum( r => r.Amount);
                        Row.Add(raceAmount.ToString());
                        totalPerRace[i] += raceAmount;
                        i++;
                    }
                    i=0;
                    foreach( var et in Ethnicities){
                        var ethnAmount = res.RaceEthnicityValues.Where( v => v.EthnicityId == et.Id).Sum( r => r.Amount);
                        Row.Add(ethnAmount.ToString());
                        totalPerEthnicity[i] += ethnAmount;
                        i++;
                    }
                    Row.Add(res.Male.ToString());
                    Row.Add(res.Female.ToString());
                    i=0;
                    foreach( var opnmb in OptionNumbers){
                        var optNmbAmount = res.OptionNumberValues.Where( o => o.ActivityOptionNumberId == opnmb.Id).Sum( s => s.Value);
                        Row.Add( optNmbAmount.ToString());
                        totalPerOptionNumber[i] += optNmbAmount;
                        i++;
                    }
                    Rows.Add(Row);
                }
                table.Rows = Rows;
                table.Foother = new List<string>{
                            "Total", (TotalHours / 8).ToString(), (TotalHours / (8 * workDaysPerYear)).ToString("0.000"), (TotalMultistate / 8).ToString(), TotalAudience.ToString()
                        };
                i = 0;
                foreach( var race in Races){
                    table.Foother.Add(totalPerRace[i].ToString());
                    i++;
                }
                i = 0;
                foreach( var et in Ethnicities){
                    table.Foother.Add(totalPerEthnicity[i].ToString());
                    i++;
                }
                table.Foother.Add(TotalMale.ToString());
                table.Foother.Add(TotalFemale.ToString());
                i = 0;
                foreach( var opnmb in OptionNumbers){
                    table.Foother.Add( totalPerOptionNumber[i].ToString());
                    i++;
                }
                var serialized = JsonConvert.SerializeObject(table);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(3)
                    });
            }


            return table;
        }

        public List<PerUnitActivities> ProcessUnitContacts(List<ContactUnitResult> contacts, List<PerUnitActivities> result){
            foreach( var contactGroup in contacts ){
                    var unitRevisions = new List<ContactRevision>();
                    var OptionNumbers = new List<IOptionNumberValue>();
                    var RaceEthnicities = new List<IRaceEthnicityValue>();
                    foreach( var rev in contactGroup.Ids){

                        var cacheKey = "ContactLastRevision" + rev.ToString();

                        var cacheString = _cache.GetString(cacheKey);
                    
                        ContactRevision lstrvsn;
                        if (!string.IsNullOrEmpty(cacheString)){
                            lstrvsn = JsonConvert.DeserializeObject<ContactRevision>(cacheString);
                        }else{
                            lstrvsn = coreContext.ContactRevision.
                                    Where(r => r.ContactId == rev).
                                    Include(a => a.ContactOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                                    Include(a => a.ContactRaceEthnicityValues).
                                    OrderBy(a => a.Created).Last();
                            
                            

                            var serialized = JsonConvert.SerializeObject(lstrvsn);
                            _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                            });         
                        }
                        unitRevisions.Add(lstrvsn);
                        OptionNumbers.AddRange(lstrvsn.ContactOptionNumbers);
                        RaceEthnicities.AddRange(lstrvsn.ContactRaceEthnicityValues);
                    }
                    var unitInResults = result.Where( r => r.PlanningUnit.Id == contactGroup.Unit.Id).FirstOrDefault();
                    if(unitInResults == null){
                        var actvts = new PerUnitActivities();
                        actvts.RaceEthnicityValues = RaceEthnicities;
                        actvts.OptionNumberValues = OptionNumbers;
                        actvts.Hours = unitRevisions.Sum( r => r.Days) * 8;
                        actvts.Male = unitRevisions.Sum( r => r.Male);
                        actvts.Female = unitRevisions.Sum( r => r.Female);
                        actvts.Audience = actvts.Male + actvts.Female;
                        actvts.PlanningUnit = contactGroup.Unit;
                        actvts.Multistate = unitRevisions.Sum(r => r.Multistate) * 8;
                        result.Add(actvts);
                    }else{
                        unitInResults.RaceEthnicityValues.AddRange(RaceEthnicities);
                        unitInResults.OptionNumberValues.AddRange(OptionNumbers);
                        unitInResults.Hours += unitRevisions.Sum( r => r.Days) * 8;
                        unitInResults.Male += unitRevisions.Sum( r => r.Male);
                        unitInResults.Female += unitRevisions.Sum( r => r.Female);
                        unitInResults.Audience += unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                        unitInResults.Multistate += unitRevisions.Sum(r => r.Multistate) * 8;
                    }
                }
            return result;
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
                actvts.Male = unt.Male;
                actvts.Female = unt.Female;
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
                actvts.Male = unt.Male;
                actvts.Female = unt.Female;
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
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
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
                actvts.Male = unt.Male;
                actvts.Female = unt.Female;
                actvts.MajorProgram = unt.MajorProgram;
                actvts.Multistate = unitRevisions.Where( r => r.ActivityOptionSelections.Where( s => s.ActivityOption.Name == "Multistate effort?").Count() > 0).Sum(s => s.Hours);
                result.Add(actvts);
            }

            return result;
        }

        public List<PerProgramActivities> ProcessMajorProgramContacts(List<ContactMajorProgramResult> contacts, List<PerProgramActivities> result, IDistributedCache _cache){
            foreach( var contactGroup in contacts ){
                    var unitRevisions = new List<ContactRevision>();
                    var OptionNumbers = new List<IOptionNumberValue>();
                    var RaceEthnicities = new List<IRaceEthnicityValue>();
                    foreach( var rev in contactGroup.Ids){

                        var cacheKey = "ContactLastRevision" + rev.ToString();

                        var cacheString = _cache.GetString(cacheKey);
                    
                        ContactRevision lstrvsn;
                        if (!string.IsNullOrEmpty(cacheString)){
                            lstrvsn = JsonConvert.DeserializeObject<ContactRevision>(cacheString);
                        }else{
                            lstrvsn = coreContext.ContactRevision.
                                    Where(r => r.ContactId == rev).
                                    Include(a => a.ContactOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                                    Include(a => a.ContactRaceEthnicityValues).
                                    OrderBy(a => a.Created).Last();
                            var serialized = JsonConvert.SerializeObject(lstrvsn);
                            _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                            });         
                        }
                        unitRevisions.Add(lstrvsn);
                        OptionNumbers.AddRange(lstrvsn.ContactOptionNumbers);
                        RaceEthnicities.AddRange(lstrvsn.ContactRaceEthnicityValues);
                    }
                    var unitInResults = result.Where( r => r.MajorProgram.Id == contactGroup.MajorProgram.Id).FirstOrDefault();
                    if(unitInResults == null){
                        var actvts = new PerProgramActivities();
                        actvts.RaceEthnicityValues = RaceEthnicities;
                        actvts.OptionNumberValues = OptionNumbers;
                        actvts.Hours = unitRevisions.Sum( r => r.Days) * 8;
                        actvts.Audience = unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                        actvts.Male = unitRevisions.Sum( r => r.Male);
                        actvts.Female = unitRevisions.Sum( r => r.Female );
                        actvts.MajorProgram = contactGroup.MajorProgram;
                        actvts.Multistate = unitRevisions.Sum(r => r.Multistate) * 8;
                        result.Add(actvts);
                    }else{
                        unitInResults.RaceEthnicityValues.AddRange(RaceEthnicities);
                        unitInResults.OptionNumberValues.AddRange(OptionNumbers);
                        unitInResults.Hours += unitRevisions.Sum( r => r.Days) * 8;
                        unitInResults.Male += unitRevisions.Sum( r => r.Male);
                        unitInResults.Female += unitRevisions.Sum( r => r.Female);
                        unitInResults.Audience += unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                        unitInResults.Multistate += unitRevisions.Sum(r => r.Multistate) * 8;
                    }
                }
            return result;
        }

        public List<string> ReportRow(  int id, 
                                        Activity activity = null, 
                                        List<Race> races = null,
                                        List<Ethnicity> ethnicities = null,
                                        List<ActivityOption> options = null, 
                                        List<ActivityOptionNumber> optionNumbers = null,
                                        List<SnapDirectAges> ages = null,
                                        List<SnapDirectAudience> audience = null,
                                        List<SnapIndirectMethod> method = null,
                                        List<SnapIndirectReached> reached = null,
                                        List<SnapPolicyAimed> aimed = null,
                                        List<SnapPolicyPartner> partners = null){
            var result = new List<string>();
            ActivityRevision lastRevision;
            if( activity == null){
                activity = this.coreContext.Activity
                            .Where( a => a.Id == id)
                            .Include( a => a.PlanningUnit).ThenInclude( u => u.ExtensionArea).ThenInclude(a => a.ExtensionRegion)
                            .Include( a => a.KersUser).ThenInclude( u => u.Specialties ).ThenInclude( s => s.Specialty)
                            .Include( a => a.KersUser).ThenInclude( u => u.RprtngProfile)
                            .Include( a => a.Revisions)
                            .FirstOrDefault();
                if( activity == null ) return null;
                lastRevision = this.coreContext.ActivityRevision
                                    .Where( r => r.Id == activity.Revisions.OrderByDescending( lr => lr.Created).First().Id)
                                    .Include( r => r.MajorProgram)
                                    .Include( r => r.RaceEthnicityValues)
                                    .Include( r => r.ActivityOptionNumbers)
                                    .Include( r => r.ActivityOptionSelections)
                                    .Include( r => r.SnapDirect).ThenInclude( d => d.SnapDirectAgesAudienceValues)
                                    .Include( r => r.SnapDirect).ThenInclude( d => d.SnapDirectDeliverySite)
                                    .Include( r => r.SnapDirect).ThenInclude( d => d.SnapDirectSessionType)
                                    .Include( r => r.SnapIndirect).ThenInclude( i => i.SnapIndirectMethodSelections)
                                    .Include( r => r.SnapIndirect).ThenInclude( i => i.SnapIndirectReachedValues)
                                    .Include( r => r.SnapPolicy).ThenInclude( p => p.SnapPolicyAimedSelections)
                                    .Include( r => r.SnapPolicy).ThenInclude( p => p.SnapPolicyPartnerValue)
                                    .FirstOrDefault();
            }else{
                lastRevision = activity.Revisions.OrderByDescending(r => r.Created).FirstOrDefault();
            }
            result.Add( lastRevision.ActivityDate.ToString("MM-dd-yy"));
            result.Add( lastRevision.Title);
            string pattern = @"<(.|\n)*?>";
            result.Add(Regex.Replace(lastRevision.Description, pattern, string.Empty));
            result.Add( activity.KersUser.RprtngProfile.Name);
            result.Add( activity.PlanningUnit.Name);
            if(activity.PlanningUnit.ExtensionArea != null){
                result.Add( activity.PlanningUnit.ExtensionArea.Name);
                result.Add( activity.PlanningUnit.ExtensionArea.ExtensionRegion.Name);
            }else{
                result.Add("");
                result.Add("");
            }
            var congrDistrict = coreContext.CongressionalDistrictUnit
                                        .Where( d => d.PlanningUnit == activity.PlanningUnit)
                                        .Include( d => d.CongressionalDistrict)
                                        .FirstOrDefault();
            if(congrDistrict != null){
                result.Add(congrDistrict.CongressionalDistrict.Name);
            }else{
                result.Add("");
            }
            result.Add( lastRevision.MajorProgram.Name);
            


            return result;
        }



    }

}