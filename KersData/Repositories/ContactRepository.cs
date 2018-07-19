using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;
using Kers.Models.Data;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Kers.Models.ViewModels;

namespace Kers.Models.Repositories
{
    public class ContactRepository : EntityBaseRepository<Contact>, IContactRepository
    {

        private KERScoreContext coreContext;
        private IDistributedCache _cache;
        public ContactRepository(
            IDistributedCache _cache,
            KERScoreContext context
            )
            : base(context)
        { 
            this.coreContext = context;
            this._cache = _cache;
        }


        public List<PerUnitActivities> ProcessUnitContacts(List<ContactUnitResult> contacts, List<PerUnitActivities> result, IDistributedCache _cache){
            foreach( var contactGroup in contacts ){
                    var unitRevisions = new List<ContactRevision>();
                    var OptionNumbers = new List<IOptionNumberValue>();
                    var RaceEthnicities = new List<IRaceEthnicityValue>();
                    foreach( var rev in contactGroup.Ids){

                        var cacheKey = CacheKeys.ContactLastRevision + rev.ToString();

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
                            unitRevisions.Add(lstrvsn);
                            OptionNumbers.AddRange(lstrvsn.ContactOptionNumbers);
                            RaceEthnicities.AddRange(lstrvsn.ContactRaceEthnicityValues);

                            var serialized = JsonConvert.SerializeObject(lstrvsn);
                            _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                            });         
                        }
                    }
                    var unitInResults = result.Where( r => r.PlanningUnit.Id == contactGroup.Unit.Id).FirstOrDefault();
                    if(unitInResults == null){
                        var actvts = new PerUnitActivities();
                        actvts.RaceEthnicityValues = RaceEthnicities;
                        actvts.OptionNumberValues = OptionNumbers;
                        actvts.Hours = unitRevisions.Sum( r => r.Days) * 8;
                        actvts.Audience = unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                        actvts.Male = unitRevisions.Sum( r => r.Male);
                        actvts.Female = unitRevisions.Sum( r => r.Female);
                        actvts.PlanningUnit = contactGroup.Unit;
                        actvts.Multistate = unitRevisions.Sum(r => r.Multistate) * 8;
                        result.Add(actvts);
                    }else{
                        unitInResults.RaceEthnicityValues.AddRange(RaceEthnicities);
                        unitInResults.OptionNumberValues.AddRange(OptionNumbers);
                        unitInResults.Hours += unitRevisions.Sum( r => r.Days) * 8;
                        unitInResults.Audience += unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                        unitInResults.Male += unitRevisions.Sum( r => r.Male);
                        unitInResults.Female += unitRevisions.Sum( r => r.Female);
                        unitInResults.Multistate += unitRevisions.Sum(r => r.Multistate) * 8;
                    }
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
                            unitRevisions.Add(lstrvsn);
                            OptionNumbers.AddRange(lstrvsn.ContactOptionNumbers);
                            RaceEthnicities.AddRange(lstrvsn.ContactRaceEthnicityValues);

                            var serialized = JsonConvert.SerializeObject(lstrvsn);
                            _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                            });         
                        }
                    }
                    var unitInResults = result.Where( r => r.MajorProgram.Id == contactGroup.MajorProgram.Id).FirstOrDefault();
                    if(unitInResults == null){
                        var actvts = new PerProgramActivities();
                        actvts.RaceEthnicityValues = RaceEthnicities;
                        actvts.OptionNumberValues = OptionNumbers;
                        actvts.Hours = unitRevisions.Sum( r => r.Days) * 8;
                        actvts.Audience = unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                        actvts.Male = unitRevisions.Sum( r => r.Male);
                        actvts.Female = unitRevisions.Sum( r => r.Female);
                        actvts.MajorProgram = contactGroup.MajorProgram;
                        actvts.Multistate = unitRevisions.Sum(r => r.Multistate) * 8;
                        result.Add(actvts);
                    }else{
                        unitInResults.RaceEthnicityValues.AddRange(RaceEthnicities);
                        unitInResults.OptionNumberValues.AddRange(OptionNumbers);
                        unitInResults.Hours += unitRevisions.Sum( r => r.Days) * 8;
                        unitInResults.Audience += unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                        unitInResults.Male = unitRevisions.Sum( r => r.Male);
                        unitInResults.Female = unitRevisions.Sum( r => r.Female);
                        unitInResults.Multistate += unitRevisions.Sum(r => r.Multistate) * 8;
                    }
                }
            return result;
        }

        public async Task<TableViewModel> Data(FiscalYear fiscalYear, int type = 0, int id = 0, bool refreshCache = false )
        {

            var cacheKey = CacheKeys.ByEmployeeContactsData + type.ToString() + id.ToString() + fiscalYear.Name;
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes) && !refreshCache){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{


                table = new TableViewModel();

                List<ActivityPersonResult> activities;




                var actvtsCacheKey = "AllActivitiesByEmployee" + type.ToString() + id.ToString() + "+" + fiscalYear.Name;
                var cachedActivities = _cache.GetString(actvtsCacheKey);

                if (!string.IsNullOrEmpty(cachedActivities) && !refreshCache){
                    activities = JsonConvert.DeserializeObject<List<ActivityPersonResult>>(cachedActivities);
                }else{

                    if(type == 0){
                        activities = await DistrictActivities(id, fiscalYear);
                    }else if(type == 1){
                        activities = await UnitActivities(id, fiscalYear);
                    }else{
                        activities = await KSUActivities(fiscalYear);
                    }
                    
                    
                    var serializedActivities = JsonConvert.SerializeObject(activities);
                    _cache.SetString(actvtsCacheKey, serializedActivities, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
                        });


                }
                var result = ProcessPersonActivities(activities);

                List<ContactPersonResult> contacts;

                var contactsCacheKey = "ContactsByEmployee" + type.ToString() + id.ToString() + "_" + fiscalYear.Name;
                var cachedContacts = _cache.GetString(contactsCacheKey);

                if (!string.IsNullOrEmpty(cachedContacts) && !refreshCache){
                    contacts = JsonConvert.DeserializeObject<List<ContactPersonResult>>(cachedContacts);
                }else{
                    contacts = new List<ContactPersonResult>();
                    if(type == 0){
                        contacts = await DistrictContacts(id, fiscalYear);
                    }else if(type == 1){
                        contacts = await UnitContacts(id, fiscalYear);
                    }else if( type == 2){
                        contacts = await KSUContacts(fiscalYear);
                    }
                    var serializedContacts = JsonConvert.SerializeObject(contacts);
                    _cache.SetString(contactsCacheKey, serializedContacts, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
                        });
                }

                result = ProcessPersonContacts(contacts, result, _cache);
                result = result.OrderBy( r => r.KersUser.RprtngProfile.PlanningUnit.order).ThenBy(r => r.KersUser.PersonalProfile.FirstName).ToList();



                table.Header = new List<string>{
                                "Planning Unit", "Employee", "Days", "Multistate", "Total Contacts"
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
                    Row.Add(res.KersUser.RprtngProfile.PlanningUnit.Name);
                    Row.Add( res.KersUser.PersonalProfile.FirstName + " " + res.KersUser.PersonalProfile.LastName);
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
                            "Total", "", (TotalHours / 8).ToString(), (TotalMultistate / 8).ToString(), TotalAudience.ToString()
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
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(21)
                    });
            }

            return table;
        }

        private async Task<List<ActivityPersonResult>> DistrictActivities(int id, FiscalYear fiscalYear){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < fiscalYear.End 
                                                                && 
                                                                a.ActivityDate > fiscalYear.Start
                                                                &&
                                                                a.KersUser.RprtngProfile.PlanningUnit.DistrictId == id
                                                            )
                                                    .GroupBy(e => new {
                                                        KersUser = e.KersUser
                                                    })
                                                    .Select(c => new ActivityPersonResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        KersUser = c.Key.KersUser
                                                    })
                                                    .ToListAsync();

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
            
            
            
            return activities;

        }

        private async Task<List<ContactPersonResult>> DistrictContacts(int id, FiscalYear fiscalYear){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < fiscalYear.End 
                                                && 
                                                c.ContactDate > fiscalYear.Start 
                                                && 
                                                c.KersUser.RprtngProfile.PlanningUnit.DistrictId == id
                                        )
                                        .GroupBy(e => new {
                                            User = e.KersUser
                                        })
                                        .Select(c => new ContactPersonResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            KersUser = c.Key.User
                                        })
                                        .ToListAsync();
            return contacts;
        }


        private async Task<List<ActivityPersonResult>> UnitActivities(int id, FiscalYear fiscalYear){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < fiscalYear.End 
                                                                && 
                                                                a.ActivityDate > fiscalYear.Start
                                                                &&
                                                                a.KersUser.RprtngProfile.PlanningUnitId == id
                                                            )
                                                    .GroupBy(e => new {
                                                        KersUser = e.KersUser
                                                    })
                                                    .Select(c => new ActivityPersonResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        KersUser = c.Key.KersUser
                                                    })
                                                    .ToListAsync();
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

            return activities;

        }

        private async Task<List<ContactPersonResult>> UnitContacts(int id, FiscalYear fiscalYear){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < fiscalYear.End 
                                                && 
                                                c.ContactDate > fiscalYear.Start 
                                                && 
                                                c.KersUser.RprtngProfile.PlanningUnitId == id
                                        )
                                        .GroupBy(e => new {
                                            User = e.KersUser
                                        })
                                        .Select(c => new ContactPersonResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            KersUser = c.Key.User
                                        })
                                        .ToListAsync();
            return contacts;
        }


        private async Task<List<ActivityPersonResult>> KSUActivities(FiscalYear fiscalYear){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < fiscalYear.End 
                                                                && 
                                                                a.ActivityDate > fiscalYear.Start
                                                                &&
                                                                a.KersUser.RprtngProfile.Institution.Code == "21000-1890"
                                                            )
                                                    .GroupBy(e => new {
                                                        KersUser = e.KersUser
                                                    })
                                                    .Select(c => new ActivityPersonResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        KersUser = c.Key.KersUser
                                                    })
                                                    .ToListAsync();
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

            return activities;

        }

        private async Task<List<ContactPersonResult>> KSUContacts(FiscalYear fiscalYear){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < fiscalYear.End 
                                                && 
                                                c.ContactDate > fiscalYear.Start 
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code == "21000-1890"
                                        )
                                        .GroupBy(e => new {
                                            User = e.KersUser
                                        })
                                        .Select(c => new ContactPersonResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            KersUser = c.Key.User
                                        })
                                        .ToListAsync();
            return contacts;
        }

        public List<PerPersonActivities> ProcessPersonActivities(List<ActivityPersonResult> activities){
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
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
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

        public List<PerPersonActivities> ProcessPersonContacts(List<ContactPersonResult> contacts, List<PerPersonActivities> result, IDistributedCache _cache){
            foreach( var contactGroup in contacts ){
                var unitRevisions = new List<ContactRevision>();
                var OptionNumbers = new List<IOptionNumberValue>();
                var RaceEthnicities = new List<IRaceEthnicityValue>();
                foreach( var rev in contactGroup.Ids){

                    var cacheKey = "ContactLastRevision1" + rev.ToString();

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
                var unitInResults = result.Where( r => r.KersUser.Id == contactGroup.KersUser.Id).FirstOrDefault();
                if(unitInResults == null){
                    var user = this.coreContext.
                                KersUser.Where( u => u.Id == contactGroup.KersUser.Id)
                                .Include( u => u.PersonalProfile)
                                .Include(u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit)
                                .First();
                    var actvts = new PerPersonActivities();
                    actvts.RaceEthnicityValues = RaceEthnicities;
                    actvts.OptionNumberValues = OptionNumbers;
                    actvts.Hours = unitRevisions.Sum( r => r.Days) * 8;
                    actvts.Audience = unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                    actvts.Male = unitRevisions.Sum( r => r.Male);
                    actvts.Female = unitRevisions.Sum( r => r.Female);
                    actvts.KersUser = user;
                    actvts.Multistate = unitRevisions.Sum(r => r.Multistate) * 8;
                    result.Add(actvts);
                }else{
                    unitInResults.RaceEthnicityValues.AddRange(RaceEthnicities);
                    unitInResults.OptionNumberValues.AddRange(OptionNumbers);
                    unitInResults.Hours += unitRevisions.Sum( r => r.Days) * 8;
                    unitInResults.Audience += unitRevisions.Sum( r => r.Male) + unitRevisions.Sum( r => r.Female);
                    unitInResults.Male += unitRevisions.Sum( r => r.Male);
                    unitInResults.Female += unitRevisions.Sum( r => r.Female);
                    unitInResults.Multistate += unitRevisions.Sum(r => r.Multistate) * 8;
                }
            }
            return result;
        }


        public async Task<StatsViewModel> StatsPerMonth( int year = 0, int month = 0, int PlanningUnitId = 0, int MajorProgramId = 0, bool refreshCache = false )
        {
            // If not month or year is provided, get the last month
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


            var cacheKey = CacheKeys.StatsPerMonth + month.ToString() + year.ToString() + PlanningUnitId.ToString() + MajorProgramId.ToString();
            var cachedStats = _cache.GetString(cacheKey);
            StatsViewModel stats;
            if (!string.IsNullOrEmpty(cachedStats) && !refreshCache){
                stats = JsonConvert.DeserializeObject<StatsViewModel>(cachedStats);
            }else{

                var firstDay = new DateTime( year, month, 1, 0, 0, 0);
                var lastDay = new DateTime( year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);


                var activities = this.coreContext.Activity.Where( a => a.ActivityDate > firstDay && a.ActivityDate < lastDay);

                if( PlanningUnitId != 0){
                    activities = activities.Where( a => a.PlanningUnitId == PlanningUnitId );
                }

                if( MajorProgramId != 0 ){
                    activities = activities.Where( a => a.MajorProgramId == MajorProgramId );
                }

                var activitiesList = await activities.ToListAsync();
        
                var OptionNumbers = new List<IOptionNumberValue>();

                var lastActivityRevs = new List<ActivityRevision>();

                foreach( var activity in activitiesList ){
                    var rev = await coreContext.ActivityRevision
                                .Where( a => a.ActivityId == activity.Id)
                                .Include( r => r.ActivityOptionNumbers ).ThenInclude( o => o.ActivityOptionNumber )
                                .OrderBy( r => r.Created)
                                .LastAsync();
                    lastActivityRevs.Add( rev );
                    OptionNumbers.AddRange( rev.ActivityOptionNumbers );
                }

                
                var contacts = this.coreContext.Contact.Where( c => c.ContactDate > firstDay && c.ContactDate < lastDay );

                if(PlanningUnitId != 0 ){
                    contacts = contacts.Where( c => c.PlanningUnitId == PlanningUnitId );
                }

                if( MajorProgramId != 0 ){
                    contacts = contacts.Where( c => c.MajorProgramId == MajorProgramId );
                }

                var contactsList = await contacts.ToListAsync();



                var lastContactRevs = new List<ContactRevision>();
                foreach( var contact in contactsList ){
                    var rev = await coreContext.ContactRevision
                                    .Where( c => c.ContactId == contact.Id )
                                    .Include( r => r.ContactOptionNumbers ).ThenInclude( n => n.ActivityOptionNumber)
                                    .LastAsync();
                    lastContactRevs.Add( rev );
                    OptionNumbers.AddRange( rev.ContactOptionNumbers );
                }

                stats = new StatsViewModel();

                stats.DirectContacts = activitiesList.Sum( a => a.Audience ) + contacts.Sum( c => c.Audience );
                stats.Hours = activitiesList.Sum( a => a.Hours ) + contacts.Sum( c => c.Days) * 8;
                stats.Male = lastActivityRevs.Sum( r => r.Male ) + lastContactRevs.Sum( r => r.Male );
                stats.Female = lastActivityRevs.Sum( r => r.Female ) + lastContactRevs.Sum( r =>r.Female );
                stats.IndirectContacts = OptionNumbers.Where( a => a.ActivityOptionNumber.Name == "Number of Indirect Contacts").Sum( a => a.Value );
                stats.Voluntiers = OptionNumbers.Where( a => a.ActivityOptionNumber.Name == "Adult Volunteers").Sum( a => a.Value );

                var serialized = JsonConvert.SerializeObject(stats);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(21)
                    });

            }
            return stats;
        }







    }


}