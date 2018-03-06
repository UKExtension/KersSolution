using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Kers.Models.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Kers.Models.Data;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class ContactsController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepository;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        const int workDaysPerYear = 228;
        public ContactsController( 
                    KERScoreContext context,
                    IFiscalYearRepository fiscalYearRepository,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo
            ){
           this.context = context;
           this.fiscalYearRepository = fiscalYearRepository;
           this._cache = _cache;
           this.activityRepo = activityRepo;
           this.contactRepo = contactRepo;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult State()
        {
            return View();
        }

        [HttpGet]
        [Route("[action]/{fy?}")]
        public IActionResult StateAll(string fy = "0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                //this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            /* var cacheKey = "StateAllContactsData";
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes)){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{

                var currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");


                var actvtsCacheKey = "AllActivitiesByPlanningUnit";
                var cachedActivities = _cache.GetString(actvtsCacheKey);
                List<ActivityUnitResult> activities;
                if (!string.IsNullOrEmpty(cachedActivities)){
                    activities = JsonConvert.DeserializeObject<List<ActivityUnitResult>>(cachedActivities);
                }else{
                    activities = await this.context.Activity
                                                    .Where( a => a.ActivityDate < currentFiscalYear.End && a.ActivityDate > currentFiscalYear.Start)
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
                                                    .ToListAsync();
                    var serializedActivities = JsonConvert.SerializeObject(activities);
                    _cache.SetString(actvtsCacheKey, serializedActivities, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                        });
                                
                }
                var result = activityRepo.ProcessUnitActivities( activities, _cache);


                var contactsCacheKey = "AllContactsByPlanningUnit";
                var cachedContacts = _cache.GetString(contactsCacheKey);
                List<ContactUnitResult> contacts;
                if (!string.IsNullOrEmpty(cachedContacts)){
                    contacts = JsonConvert.DeserializeObject<List<ContactUnitResult>>(cachedContacts);
                }else{
                    contacts = await this.context.Contact.
                                    Where( c => 
                                                c.ContactDate < currentFiscalYear.End 
                                                && 
                                                c.ContactDate > currentFiscalYear.Start 
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
                                        .ToListAsync();
                    var serializedContacts = JsonConvert.SerializeObject(contacts);
                    _cache.SetString(contactsCacheKey, serializedContacts, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                        });
                                
                }
                result = this.contactRepo.ProcessUnitContacts( contacts, result, _cache);
                result = result.OrderBy( r => r.PlanningUnit.order).ToList();
                table = new TableViewModel();
                table.Header = new List<string>{
                            "Planning Unit", "Days", "Multistate", "Total Contacts"
                        };
                var Races = this.context.Race.OrderBy(r => r.Order);
                var Ethnicities = this.context.Ethnicity.OrderBy( e => e.Order);
                var OptionNumbers = this.context.ActivityOptionNumber.OrderBy( n => n.Order);
                foreach( var race in Races){
                    table.Header.Add(race.Name);
                }
                foreach( var ethn in Ethnicities){
                    table.Header.Add(ethn.Name);
                }
                foreach( var opnmb in OptionNumbers){
                    table.Header.Add(opnmb.Name);
                }
                var Rows = new List<List<string>>();
                float TotalHours = 0;
                float TotalMultistate = 0;
                int TotalAudience = 0;
                int[] totalPerRace = new int[Races.Count()];
                int[] totalPerEthnicity = new int[Ethnicities.Count()];
                int[] totalPerOptionNumber = new int[OptionNumbers.Count()];
                int i = 0;
                foreach(var res in result){
                    TotalHours += res.Hours;
                    TotalAudience += res.Audience;
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
            } */

            var table = activityRepo.ReportsStateAll(fiscalYear);
            return View(table);
        }


        [HttpGet]
        [Route("[action]/{type?}")]
        // type: 0 - all, 1 - UK, 2 - KSU
        public async Task<IActionResult> StateByMajorProgram(int type = 0)
        {

            var cacheKey = "StateByMajorProgram" + type.ToString();
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes)){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{

                var currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");


                var actvtsCacheKey = "AllActivitiesByMajorProgram" + type.ToString();
                var cachedActivities = _cache.GetString(actvtsCacheKey);
                List<ActivityMajorProgramResult> activities;
                if (!string.IsNullOrEmpty(cachedActivities)){
                    activities = JsonConvert.DeserializeObject<List<ActivityMajorProgramResult>>(cachedActivities);
                }else{

                    // Only UK
                    if(type == 1){
                        activities = await this.context.Activity
                                                    .Where( a =>    a.ActivityDate < currentFiscalYear.End 
                                                                    && 
                                                                    a.ActivityDate > currentFiscalYear.Start
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
                        activities = await this.context.Activity
                                                    .Where( a =>    a.ActivityDate < currentFiscalYear.End 
                                                                    && 
                                                                    a.ActivityDate > currentFiscalYear.Start
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
                        activities = await this.context.Activity
                                                    .Where( a => a.ActivityDate < currentFiscalYear.End && a.ActivityDate > currentFiscalYear.Start)
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

                    
                    var serializedActivities = JsonConvert.SerializeObject(activities);
                    _cache.SetString(actvtsCacheKey, serializedActivities, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                        });
                                
                }
                var result = activityRepo.ProcessMajorProgramActivities( activities, _cache);


                var contactsCacheKey = "AllContactsByMajorProgram" + type.ToString();
                var cachedContacts = _cache.GetString(contactsCacheKey);
                List<ContactMajorProgramResult> contacts;
                if (!string.IsNullOrEmpty(cachedContacts)){
                    contacts = JsonConvert.DeserializeObject<List<ContactMajorProgramResult>>(cachedContacts);
                }else{
                    if(type == 1){
                        contacts = await this.context.Contact.
                                    Where( c => 
                                                c.ContactDate < currentFiscalYear.End 
                                                && 
                                                c.ContactDate > currentFiscalYear.Start 
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
                        contacts = await this.context.Contact.
                                    Where( c => 
                                                c.ContactDate < currentFiscalYear.End 
                                                && 
                                                c.ContactDate > currentFiscalYear.Start 
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
                        contacts = await this.context.Contact.
                                    Where( c => 
                                                c.ContactDate < currentFiscalYear.End 
                                                && 
                                                c.ContactDate > currentFiscalYear.Start 
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
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                        });
                                
                }
                result = this.contactRepo.ProcessMajorProgramContacts( contacts, result, _cache);
                result = result.OrderBy( r => r.MajorProgram.PacCode).ToList();
                table = new TableViewModel();
                table.Header = new List<string>{
                            "Major Program", "Days", "FTE", "Multistate", "Total Contacts"
                        };
                var Races = this.context.Race.OrderBy(r => r.Order);
                var Ethnicities = this.context.Ethnicity.OrderBy( e => e.Order);
                var OptionNumbers = this.context.ActivityOptionNumber.OrderBy( n => n.Order);
                foreach( var race in Races){
                    table.Header.Add(race.Name);
                }
                foreach( var ethn in Ethnicities){
                    table.Header.Add(ethn.Name);
                }
                foreach( var opnmb in OptionNumbers){
                    table.Header.Add(opnmb.Name);
                }
                var Rows = new List<List<string>>();
                float TotalHours = 0;
                float TotalMultistate = 0;
                int TotalAudience = 0;
                int[] totalPerRace = new int[Races.Count()];
                int[] totalPerEthnicity = new int[Ethnicities.Count()];
                int[] totalPerOptionNumber = new int[OptionNumbers.Count()];
                int i = 0;
                foreach(var res in result){
                    TotalHours += res.Hours;
                    TotalAudience += res.Audience;
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


            return View(table);
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ContactsByCountyByMajorProgram()
        {
            var counties = await this.context.PlanningUnit
                                    .Where( u => 
                                                u.District != null
                                                &&
                                                u.Name.Substring( u.Name.Length - 3) == "CES"
                                           )
                                    .OrderBy( u => u.Name)
                                    .ToListAsync();
            var majorPrograms = await this.context.MajorProgram
                                    .Where( u => 
                                                
                                                u.StrategicInitiative.FiscalYear == fiscalYearRepository.currentFiscalYear(FiscalYearType.ServiceLog)
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
                    var sm = context.Activity.Where( a => a.MajorProgramId == prgrm.Id && a.PlanningUnitId == county.Id && a.ActivityDate.Year < 2018).Sum(s => s.Audience);
                    row.Add(sm.ToString());
                }

                rows.Add(row);

            }



            var table = new TableViewModel();
            table.Header = header;
            table.Rows = rows;
            table.Foother = new List<string>();
            return View(table);
        }




        


        [HttpGet]
        [Route("[action]")]
        public IActionResult  Districts()
        {

            var districts =  this.context.District.ToList();

            return View(districts);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> County()
        {
            var counties = await this.context.PlanningUnit
                                    .Where( u => 
                                                u.District != null
                                                &&
                                                u.Name.Substring( u.Name.Length - 3) == "CES"
                                           )
                                    .OrderBy( u => u.Name)
                                    .ToListAsync();
            return View(counties);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Units()
        {
            var units = await this.context.PlanningUnit
                                    .Where( u => 
                                                u.District == null
                                                &&
                                                u.Name.Substring( u.Name.Length - 3) != "CES"
                                           )
                                    .OrderBy( u => u.Name)
                                    .ToListAsync();
            return View(units);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Ksu()
        {
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Person()
        {
            return View();
        }

        public FiscalYear GetFYByName(string fy, string type = "serviceLog"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalYearRepository.currentFiscalYear(type);
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == type).FirstOrDefault();
            }
            return fiscalYear;
        }



    }

}