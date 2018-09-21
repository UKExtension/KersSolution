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

        const int workDaysPerYear = 228;
        public ContactRepository(
            IDistributedCache _cache,
            KERScoreContext context
            )
            : base(context)
        { 
            this.coreContext = context;
            this._cache = _cache;
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



        /***********************************************************************************************/
        // Generate Contacts Reports Groupped by Employee or Major Program
        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All, 5 Major Program, 6 Employee
        // Returns List with Indexes: 0 Total Hours, 1 Contacts, 2 Multistate Hours, 3 Number of Activities
        /***********************************************************************************************/
        public async Task<List<float>> GetPerPeriodSummaries( DateTime start, DateTime end, int filter = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 ){
            

            var cacheKey = CacheKeys.FilteredContactSummaries + filter.ToString() + id.ToString() + "_" + start.ToString("s") + end.ToString("s");
            var cachedTypes = await _cache.GetStringAsync(cacheKey);
            List<float> result;
            if (!string.IsNullOrEmpty(cachedTypes) && !refreshCache){
                result = JsonConvert.DeserializeObject<List<float>>(cachedTypes);
            }else{
                float TotalHours = 0;
                int TotalContacts = 0;
                float TotalMultistate = 0;
                int TotalNumActivities = 0;
                var revs = await this.LastActivityRevisionIds(start, end, filter, id );
                // Divide revs into batches as SQL server is having trouble to process more then several thousands at once
                var FilteredActivities = new List<ActivityRevision>();
                var batchCount = 10000;
                for(var i = 0; i <= revs.Count(); i += batchCount){
                    var currentBatch = revs.Skip(i).Take(batchCount);
                    FilteredActivities.AddRange(  await coreContext.ActivityRevision
                                            .Where( r => currentBatch.Contains( r.Id ))
                                            .Include( a => a.ActivityOptionSelections ).ThenInclude( s => s.ActivityOption)
                                            .ToListAsync()
                                        );
                }
                foreach( var activity in FilteredActivities ){
                    if( activity.ActivityOptionSelections.Where( s => s.ActivityOption.Name == "Multistate effort?").Any()){
                        TotalMultistate += activity.Hours;
                    }
                    
                    TotalHours += activity.Hours;
                    TotalContacts += activity.Male + activity.Female;
                }

                var contactRevs = await this.LastContactRevisionIds(start, end, filter, id );
                var FilteredContacts = new List<ContactRevision>();
                for(var i = 0; i <= contactRevs.Count(); i += batchCount){
                    var currentBatch = contactRevs.Skip(i).Take(batchCount);
                    FilteredContacts.AddRange(  await coreContext.ContactRevision
                                            .Where( r => currentBatch.Contains( r.Id ))
                                            .ToListAsync()
                                        );
                }

                foreach( var contact in FilteredContacts ){
                    TotalHours += contact.Days * 8;
                    TotalContacts += contact.Male + contact.Female;
                    TotalMultistate += contact.Multistate * 8;
                }
                TotalNumActivities = revs.Count;
                TotalNumActivities += contactRevs.Count();
                result = new List<float>();
                result.Add(TotalHours);
                result.Add(TotalContacts);
                result.Add(TotalMultistate);
                result.Add(TotalNumActivities);

                var serialized = JsonConvert.SerializeObject(result);

                // If keep cache is not specified, figure it out depending on the past or current period
                if( keepCacheInDays == 0 ){
                    if( end < DateTime.Now ){
                        keepCacheInDays = 200;
                    }else{
                        keepCacheInDays = 3;
                    }
                }


                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(keepCacheInDays)
                    });


            }
            return result;
        }



        /*****************************************************************/
        // Generate Contacts Reports Groupped by Employee or Major Program
        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
        // grouppedBy: 0 Employee, 1 MajorProgram
        /*******************************************************************/
        public async Task<List<PerGroupActivities>> GetActivitiesAndContactsAsync( DateTime start, DateTime end, int filter = 0, int grouppedBy = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 ){
            List<ActivityGrouppedResult> activities;

            var actvtsCacheKey = "AllActivitiesGroupped" + filter.ToString() + "_" + grouppedBy.ToString() + "_" + id.ToString() + "_" + start.ToString("s") + end.ToString("s");
            var cachedActivities = _cache.GetString(actvtsCacheKey);

            if (!string.IsNullOrEmpty(cachedActivities) && !refreshCache){
                activities = JsonConvert.DeserializeObject<List<ActivityGrouppedResult>>(cachedActivities);
            }else{
                if( grouppedBy == 0){ 
                    if(filter == 0){
                        activities = await DistrictEmployeeGroupppedActivities(id, start, end);
                    }else if(filter == 1){
                        activities = await UnitEmployeeGroupppedActivities(id, start, end);
                    }else if(filter == 2){
                        activities = await KSUEmployeeGroupppedActivities(start, end);
                    }else if(filter == 3){
                        activities = await UKEmployeeGroupppedActivities(start, end);
                    }else{
                        activities = await AllEmployeeGroupppedActivities(start, end);
                    }
                }else{
                    if(filter == 0){
                        activities = await DistrictProgramGroupppedActivities(id, start, end);
                    }else if(filter == 1){
                        activities = await UnitProgramGroupppedActivities(id, start, end);
                    }else if(filter == 2){
                        activities = await KSUProgramGroupppedActivities(start, end);
                    }else if(filter == 3){
                        activities = await UKProgramGroupppedActivities(start, end);
                    }else{
                        activities = await AllProgramGroupppedActivities(start, end);
                    }
                }
                
                var serializedActivities = JsonConvert.SerializeObject(activities);
                _cache.SetString(actvtsCacheKey, serializedActivities, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(keepCacheInDays)
                    });


            }
            var result = ProcessGrouppedActivities(activities);

            List<ContactGrouppedResult> contacts;

            var contactsCacheKey = "ContactsGroupped" + filter.ToString() + "_" + grouppedBy.ToString() + "_" + id.ToString() + "_" + start.ToString("s") + end.ToString("s");
            var cachedContacts = _cache.GetString(contactsCacheKey);

            if (!string.IsNullOrEmpty(cachedContacts) && !refreshCache){
                contacts = JsonConvert.DeserializeObject<List<ContactGrouppedResult>>(cachedContacts);
            }else{
                contacts = new List<ContactGrouppedResult>();
                if( grouppedBy == 0){
                    if(filter == 0){
                        contacts = await DistrictEmployeeGroupppedContacts(id, start, end);
                    }else if(filter == 1){
                        contacts = await UnitEmployeeGroupppedContacts(id, start, end);
                    }else if( filter == 2){
                        contacts = await KSUEmployeeGroupppedContacts(start, end);
                    }else if( filter == 3){
                        contacts = await UKEmployeeGroupppedContacts(start, end);
                    }else{
                        contacts = await AllEmployeeGroupppedContacts(start, end);
                    }
                }else{
                    if(filter == 0){
                        contacts = await DistrictProgramGroupppedContacts(id, start, end);
                    }else if(filter == 1){
                        contacts = await UnitProgramGroupppedContacts(id, start, end);
                    }else if( filter == 2){
                        contacts = await KSUProgramGroupppedContacts(start, end);
                    }else if( filter == 3){
                        contacts = await UKProgramGroupppedContacts(start, end);
                    }else{
                        contacts = await AllProgramGroupppedContacts(start, end);
                    }
                }
                result = ProcessGrouppedContacts(contacts, result);
                var serializedContacts = JsonConvert.SerializeObject(contacts);
                var cacheDaysSpan = 350;
                if(keepCacheInDays == 0){
                    var today = DateTime.Now;
                    if(start < today && end.Ticks > today.Ticks){
                        cacheDaysSpan = 7;
                    }
                }else{
                    cacheDaysSpan = keepCacheInDays;
                }
                
                _cache.SetString(contactsCacheKey, serializedContacts, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cacheDaysSpan)
                    });
            }
            return result;
        } 




        /********************************************************/
        // Generate Contacts Reports Groupped by Employee
        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
        /********************************************************/

        public async Task<TableViewModel> DataByEmployee(FiscalYear fiscalYear, int filter = 0, int id = 0, bool refreshCache = false )
        {

            var cacheKey = CacheKeys.ByEmployeeContactsData + filter.ToString() + id.ToString() + "_" + fiscalYear.Name;
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes) && !refreshCache){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{
                var result = this.GetActivitiesAndContactsAsync( fiscalYear.Start, fiscalYear.End , filter, 0, id, refreshCache);

                table = new TableViewModel();
                List<PerPersonActivities> userResult = new List<PerPersonActivities>();
                foreach( var res in await result ){
                    var personGroup = new PerPersonActivities();
                    personGroup.Audience = res.Audience;
                    personGroup.Female = res.Female;
                    personGroup.Male = res.Male;
                    personGroup.Hours = res.Hours;
                    personGroup.Multistate = res.Multistate;
                    personGroup.OptionNumberValues = res.OptionNumberValues;
                    personGroup.RaceEthnicityValues = res.RaceEthnicityValues;
                    personGroup.KersUser = await coreContext.KersUser.Where( u => u.Id == res.GroupId)
                                                    .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit )
                                                    .Include( u => u.PersonalProfile)
                                                    .FirstOrDefaultAsync();
                    userResult.Add(personGroup);
                }


                userResult = userResult.OrderBy( r => r.KersUser.RprtngProfile.PlanningUnit.order).ThenBy(r => r.KersUser.PersonalProfile.FirstName).ToList();



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
                foreach(var res in userResult){
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


                var cacheDaysSpan = 350;
                var today = DateTime.Now;
                if(fiscalYear.Start < today && Math.Max( fiscalYear.End.Ticks, fiscalYear.ExtendedTo.Ticks) > today.Ticks){
                    cacheDaysSpan = 3;
                }


                var serialized = JsonConvert.SerializeObject(table);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cacheDaysSpan)
                    });
            }

            return table;
        }


        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
        public async Task<TableViewModel> DataByMajorProgram(FiscalYear fiscalYear, int filter = 0, int id = 0, bool refreshCache = false )
        {

            var cacheKey = CacheKeys.ByMajorProgramContactData + filter.ToString() + id.ToString() + "_" + fiscalYear.Name;
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes) && !refreshCache){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{
                table = new TableViewModel();
                var result = await this.GetActivitiesAndContactsAsync( fiscalYear.Start, fiscalYear.End, filter, 1, id, refreshCache);
                var defaultMajorProgram = await coreContext.MajorProgram.Where( u => u.Name == "Administrative Functions").FirstOrDefaultAsync();
                List<PerProgramActivities> programResult = new List<PerProgramActivities>();
                foreach( var res in result ){
                    var programGroup = new PerProgramActivities();
                    programGroup.Audience = res.Audience;
                    programGroup.Female = res.Female;
                    programGroup.Male = res.Male;
                    programGroup.Hours = res.Hours;
                    programGroup.Multistate = res.Multistate;
                    programGroup.OptionNumberValues = res.OptionNumberValues;
                    programGroup.RaceEthnicityValues = res.RaceEthnicityValues;
                    programGroup.MajorProgram = await coreContext.MajorProgram.Where( u => u.Id == res.GroupId)
                                                    .FirstOrDefaultAsync();
                    if(programGroup.MajorProgram == null){
                        programGroup.MajorProgram = defaultMajorProgram;
                    }
                    programResult.Add(programGroup);
                    
                    
                }


                programResult = programResult.OrderBy( r => r.MajorProgram.order).ToList();



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
                foreach(var res in programResult){
                    TotalHours += res.Hours;
                    TotalAudience += res.Audience;
                    TotalMale += res.Male;
                    TotalFemale += res.Female;
                    TotalMultistate += res.Multistate;
                    var Row = new List<string>();
                    Row.Add(res.MajorProgram.Name);
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
                            "Total", (TotalHours / 8).ToString(),(TotalHours / (8 * workDaysPerYear) ).ToString("0.000") , (TotalMultistate / 8).ToString(), TotalAudience.ToString()
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


                // Save cache for longer period in case of not active fiscal years

                var cacheDaysSpan = 350;
                var today = DateTime.Now;
                if(fiscalYear.Start < today && Math.Max( fiscalYear.End.Ticks, fiscalYear.ExtendedTo.Ticks) > today.Ticks){
                    cacheDaysSpan = 3;
                }

                var serialized = JsonConvert.SerializeObject(table);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cacheDaysSpan)
                    });
            }

            return table;
        }



        private async Task<List<ActivityGrouppedResult>> DistrictEmployeeGroupppedActivities(int id, DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                                &&
                                                                a.KersUser.RprtngProfile.PlanningUnit.DistrictId == id
                                                            )
                                                    .GroupBy(e => new {
                                                        KersUser = e.KersUser
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.KersUser.Id
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

        private async Task<List<ContactGrouppedResult>> DistrictEmployeeGroupppedContacts(int id, DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                                && 
                                                c.KersUser.RprtngProfile.PlanningUnit.DistrictId == id
                                        )
                                        .GroupBy(e => new {
                                            User = e.KersUser
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.User.Id
                                        })
                                        .ToListAsync();
            return contacts;
        }


        private async Task<List<ActivityGrouppedResult>> UnitEmployeeGroupppedActivities(int id, DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                                &&
                                                                a.KersUser.RprtngProfile.PlanningUnitId == id
                                                            )
                                                    .GroupBy(e => new {
                                                        KersUser = e.KersUser
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.KersUser.Id
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

        private async Task<List<ContactGrouppedResult>> UnitEmployeeGroupppedContacts(int id, DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                                && 
                                                c.KersUser.RprtngProfile.PlanningUnitId == id
                                        )
                                        .GroupBy(e => new {
                                            User = e.KersUser
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.User.Id
                                        })
                                        .ToListAsync();
            return contacts;
        }


        private async Task<List<ActivityGrouppedResult>> KSUEmployeeGroupppedActivities(DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                                &&
                                                                a.KersUser.RprtngProfile.Institution.Code == "21000-1890"
                                                            )
                                                    .GroupBy(e => new {
                                                        KersUser = e.KersUser
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.KersUser.Id
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

        private async Task<List<ContactGrouppedResult>> KSUEmployeeGroupppedContacts(DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code == "21000-1890"
                                        )
                                        .GroupBy(e => new {
                                            User = e.KersUser
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.User.Id
                                        })
                                        .ToListAsync();
            return contacts;
        }

        private async Task<List<ActivityGrouppedResult>> UKEmployeeGroupppedActivities(DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                                &&
                                                                a.KersUser.RprtngProfile.Institution.Code != "21000-1890"
                                                            )
                                                    .GroupBy(e => new {
                                                        KersUser = e.KersUser
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.KersUser.Id
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

        private async Task<List<ContactGrouppedResult>> UKEmployeeGroupppedContacts(DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code != "21000-1890"
                                        )
                                        .GroupBy(e => new {
                                            User = e.KersUser
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.User.Id
                                        })
                                        .ToListAsync();
            return contacts;
        }

        private async Task<List<ActivityGrouppedResult>> AllEmployeeGroupppedActivities(DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                            )
                                                    .GroupBy(e => new {
                                                        KersUser = e.KersUser
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.KersUser.Id
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

        private async Task<List<ContactGrouppedResult>> AllEmployeeGroupppedContacts(DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                        )
                                        .GroupBy(e => new {
                                            User = e.KersUser
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.User.Id
                                        })
                                        .ToListAsync();
            return contacts;
        }



        private async Task<List<ActivityGrouppedResult>> DistrictProgramGroupppedActivities(int id, DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                                &&
                                                                a.KersUser.RprtngProfile.PlanningUnit.DistrictId == id
                                                            )
                                                    .GroupBy(e => new {
                                                        ProgramId = e.MajorProgramId
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.ProgramId
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

        private async Task<List<ContactGrouppedResult>> DistrictProgramGroupppedContacts(int id, DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                                && 
                                                c.KersUser.RprtngProfile.PlanningUnit.DistrictId == id
                                        )
                                        .GroupBy(e => new {
                                            ProgramId = e.MajorProgramId
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.ProgramId
                                        })
                                        .ToListAsync();
            return contacts;
        }


        private async Task<List<ActivityGrouppedResult>> UnitProgramGroupppedActivities(int id, DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                                &&
                                                                a.PlanningUnitId == id
                                                            )
                                                    .GroupBy(e => new {
                                                        ProgramId = e.MajorProgramId
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.ProgramId
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

        private async Task<List<ContactGrouppedResult>> UnitProgramGroupppedContacts(int id, DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                                && 
                                                c.PlanningUnitId == id
                                        )
                                        .GroupBy(e => new {
                                            ProgramId = e.MajorProgramId
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.ProgramId
                                        })
                                        .ToListAsync();
            return contacts;
        }


        private async Task<List<ActivityGrouppedResult>> KSUProgramGroupppedActivities(DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                                &&
                                                                a.KersUser.RprtngProfile.Institution.Code == "21000-1890"
                                                            )
                                                    .GroupBy(e => new {
                                                        ProgramId = e.MajorProgramId
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.ProgramId
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

        private async Task<List<ContactGrouppedResult>> KSUProgramGroupppedContacts(DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code == "21000-1890"
                                        )
                                        .GroupBy(e => new {
                                            ProgramId = e.MajorProgramId
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.ProgramId
                                        })
                                        .ToListAsync();
            return contacts;
        }

        private async Task<List<ActivityGrouppedResult>> UKProgramGroupppedActivities(DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                                &&
                                                                a.KersUser.RprtngProfile.Institution.Code != "21000-1890"
                                                            )
                                                    .GroupBy(e => new {
                                                        ProgramId = e.MajorProgramId
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.ProgramId
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

        private async Task<List<ContactGrouppedResult>> UKProgramGroupppedContacts(DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code != "21000-1890"
                                        )
                                        .GroupBy(e => new {
                                            ProgramId = e.MajorProgramId
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.ProgramId
                                        })
                                        .ToListAsync();
            return contacts;
        }

        private async Task<List<ActivityGrouppedResult>> AllProgramGroupppedActivities(DateTime start, DateTime end){
            var activities = await this.coreContext.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < end 
                                                                && 
                                                                a.ActivityDate > start
                                                            )
                                                    .GroupBy(e => new {
                                                        ProgramId = e.MajorProgramId
                                                    })
                                                    .Select(c => new ActivityGrouppedResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        GroupId = c.Key.ProgramId
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

        private async Task<List<ContactGrouppedResult>> AllProgramGroupppedContacts(DateTime start, DateTime end){
           var contacts = await this.coreContext.Contact.
                                    Where( c => 
                                                c.ContactDate < end 
                                                && 
                                                c.ContactDate > start 
                                        )
                                        .GroupBy(e => new {
                                            ProgramId = e.MajorProgramId
                                        })
                                        .Select(c => new ContactGrouppedResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            GroupId = c.Key.ProgramId
                                        })
                                        .ToListAsync();
            return contacts;
        }

        public List<PerGroupActivities> ProcessGrouppedActivities(List<ActivityGrouppedResult> activities){
            var result = new List<PerGroupActivities>();
            foreach( var group in activities){
                var GroupRevisions = new List<ActivityRevision>();
                var OptionNumbers = new List<IOptionNumberValue>();
                var RaceEthnicities = new List<IRaceEthnicityValue>();
                foreach( var rev in group.Ids){
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
                    GroupRevisions.Add(lstrvsn);
                    OptionNumbers.AddRange(lstrvsn.ActivityOptionNumbers);
                    RaceEthnicities.AddRange(lstrvsn.RaceEthnicityValues);
                }

                var actvts = new PerGroupActivities();
                actvts.RaceEthnicityValues = RaceEthnicities;
                actvts.OptionNumberValues = OptionNumbers;
                actvts.Hours = group.Hours;
                actvts.Audience = group.Audience;
                actvts.Male = group.Male;
                actvts.Female = group.Female;
                actvts.GroupId = group.GroupId;
                actvts.Multistate = GroupRevisions.Where( r => r.ActivityOptionSelections.Where( s => s.ActivityOption.Name == "Multistate effort?").Count() > 0).Sum(s => s.Hours);
                result.Add(actvts);
            }
            return result;
        }


        public List<PerGroupActivities> ProcessGrouppedContacts(List<ContactGrouppedResult> contacts, List<PerGroupActivities> result){
            foreach( var contactGroup in contacts ){
                var GroupRevisions = new List<ContactRevision>();
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
                    GroupRevisions.Add(lstrvsn);
                    OptionNumbers.AddRange(lstrvsn.ContactOptionNumbers);
                    RaceEthnicities.AddRange(lstrvsn.ContactRaceEthnicityValues);
                }
                var groupInResults = result.Where( r => r.GroupId == contactGroup.GroupId).FirstOrDefault();
                if(groupInResults == null){
                    var actvts = new PerGroupActivities();
                    actvts.RaceEthnicityValues = RaceEthnicities;
                    actvts.OptionNumberValues = OptionNumbers;
                    actvts.Hours = GroupRevisions.Sum( r => r.Days) * 8;
                    actvts.Audience = GroupRevisions.Sum( r => r.Male) + GroupRevisions.Sum( r => r.Female);
                    actvts.Male = GroupRevisions.Sum( r => r.Male);
                    actvts.Female = GroupRevisions.Sum( r => r.Female);
                    actvts.GroupId = contactGroup.GroupId;
                    actvts.Multistate = GroupRevisions.Sum(r => r.Multistate) * 8;
                    result.Add(actvts);
                }else{
                    groupInResults.RaceEthnicityValues.AddRange(RaceEthnicities);
                    groupInResults.OptionNumberValues.AddRange(OptionNumbers);
                    groupInResults.Hours += GroupRevisions.Sum( r => r.Days) * 8;
                    groupInResults.Audience += GroupRevisions.Sum( r => r.Male) + GroupRevisions.Sum( r => r.Female);
                    groupInResults.Male += GroupRevisions.Sum( r => r.Male);
                    groupInResults.Female += GroupRevisions.Sum( r => r.Female);
                    groupInResults.Multistate += GroupRevisions.Sum(r => r.Multistate) * 8;
                }
            }
            return result;
        }


        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All, 5 Major Program, 6 Employee
        public async Task<List<int>> LastActivityRevisionIds( DateTime start, DateTime end, int filter = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 ){
            var cacheKey = CacheKeys.ActivityLastRevisionIdsPerPeriod + filter.ToString() + "_" + id.ToString() + start.ToString("s") + end.ToString("s");
            var cacheString = await _cache.GetStringAsync(cacheKey);
            List<int> ids;
            if (!string.IsNullOrEmpty(cacheString)){
                ids = JsonConvert.DeserializeObject<List<int>>(cacheString);
            }else{
                ids = new List<int>();
                var activities = coreContext.Activity.
                    Where(r => r.ActivityDate > start && r.ActivityDate < end);
                if( filter ==  0 ){
                    activities = activities.Where( a => a.PlanningUnit.District != null && a.PlanningUnit.District.Id == id );
                }else if( filter == 1 ){
                    activities = activities.Where( a => a.PlanningUnitId == id );
                }else if( filter == 2 ){
                    activities = activities.Where( a => a.KersUser.RprtngProfile.Institution.Code == "21000-1890");
                }else if( filter == 3 ){
                    activities = activities.Where( a => a.KersUser.RprtngProfile.Institution.Code != "21000-1890");
                }else if( filter == 5 ){
                    activities = activities.Where( a => a.MajorProgramId == id);
                }else if( filter == 6 ){
                    activities = activities.Where( a => a.KersUserId == id);
                }
                activities = activities.Include( r => r.Revisions);
                foreach( var actvt in activities){
                    var rev = actvt.Revisions.OrderBy( r => r.Created );
                    var last = rev.Last();
                    ids.Add(last.Id);
                }
                    
                var serialized = JsonConvert.SerializeObject(ids);

                // If keep cache is not specified, figure it out depending on the past or current period
                if( keepCacheInDays == 0 ){
                    if( end < DateTime.Now ){
                        keepCacheInDays = 200;
                    }else{
                        keepCacheInDays = 3;
                    }
                }
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(keepCacheInDays)
                    });
            }
            return ids;
        }

        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All, 5 Major Program, 6 Employee
        public async Task<List<int>> LastContactRevisionIds( DateTime start, DateTime end, int filter = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 ){
            var cacheKey = CacheKeys.ContactsLastRevisionIdsPerPeriod + filter.ToString() + "_" + id.ToString() + start.ToString("s") + end.ToString("s");
            var cacheString = await _cache.GetStringAsync(cacheKey);
            List<int> ids;
            if (!string.IsNullOrEmpty(cacheString)){
                ids = JsonConvert.DeserializeObject<List<int>>(cacheString);
            }else{
                ids = new List<int>();
                var contacts = coreContext.Contact.
                    Where(r => r.ContactDate > start && r.ContactDate < end);
                if( filter ==  0 ){
                    contacts = contacts.Where( a => a.PlanningUnit.District != null && a.PlanningUnit.District.Id == id );
                }else if( filter == 1 ){
                    contacts = contacts.Where( a => a.PlanningUnitId == id );
                }else if( filter == 2 ){
                    contacts = contacts.Where( a => a.KersUser.RprtngProfile.Institution.Code == "21000-1890");
                }else if( filter == 3 ){
                    contacts = contacts.Where( a => a.KersUser.RprtngProfile.Institution.Code != "21000-1890");
                }else if( filter == 5 ){
                    contacts = contacts.Where( a => a.MajorProgramId == id);
                }else if( filter == 6 ){
                    contacts = contacts.Where( a => a.KersUserId == id);
                }
                contacts = contacts.Include( r => r.Revisions);
                foreach( var cntct in contacts){
                    var rev = cntct.Revisions.OrderBy( r => r.Created );
                    var last = rev.Last();
                    ids.Add(last.Id);
                }
                    
                var serialized = JsonConvert.SerializeObject(ids);

                // If keep cache is not specified, figure it out depending on the past or current period
                if( keepCacheInDays == 0 ){
                    if( end < DateTime.Now ){
                        keepCacheInDays = 200;
                    }else{
                        keepCacheInDays = 3;
                    }
                }
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(keepCacheInDays)
                    });
            }
            return ids;
        }





    }


}