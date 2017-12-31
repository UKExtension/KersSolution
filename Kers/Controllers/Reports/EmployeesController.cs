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
    public class EmployeesController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepository;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        private FiscalYear currentFiscalYear;

        string[] types = new string[]{ "District Reports", "Planning Unit Report", "KSU" };
        public EmployeesController( 
                    KERScoreContext context,
                    IFiscalYearRepository fiscalYearRepository,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo
            ){
           this.context = context;
           this.fiscalYearRepository = fiscalYearRepository;
           this.currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");
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
        [Route("[action]/{type}/{id?}")]
        public async Task<IActionResult> Data(int type, int id = 0)
        {

            var cacheKey = "ByEmployeeContactsData" + type.ToString() + id.ToString();
            var cachedTypes = _cache.GetString(cacheKey);
            TableViewModel table;
            if (!string.IsNullOrEmpty(cachedTypes)){
                table = JsonConvert.DeserializeObject<TableViewModel>(cachedTypes);
            }else{


                table = new TableViewModel();

                List<ActivityPersonResult> activities;




                var actvtsCacheKey = "AllActivitiesByEmployee" + type.ToString() + id.ToString();
                var cachedActivities = _cache.GetString(actvtsCacheKey);

                if (!string.IsNullOrEmpty(cachedActivities)){
                    activities = JsonConvert.DeserializeObject<List<ActivityPersonResult>>(cachedActivities);
                }else{

                    if(type == 0){
                        activities = await DistrictActivities(id);
                    }else if(type == 1){
                        activities = await UnitActivities(id);
                    }else if( type == 2){
                        activities = await KSUActivities();
                    }else{
                        return new StatusCodeResult(404);
                    }
                    
                    var serializedActivities = JsonConvert.SerializeObject(activities);
                    _cache.SetString(actvtsCacheKey, serializedActivities, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
                        });


                }
                var result = activityRepo.ProcessPersonActivities(activities,  _cache);

                List<ContactPersonResult> contacts;

                var contactsCacheKey = "ContactsByEmployee" + type.ToString() + id.ToString();
                var cachedContacts = _cache.GetString(contactsCacheKey);

                if (!string.IsNullOrEmpty(cachedContacts)){
                    contacts = JsonConvert.DeserializeObject<List<ContactPersonResult>>(cachedContacts);
                }else{
                    contacts = new List<ContactPersonResult>();
                    if(type == 0){
                        contacts = await DistrictContacts(id);
                    }else if(type == 1){
                        contacts = await UnitContacts(id);
                    }else if( type == 2){
                        contacts = await KSUContacts();
                    }
                    var serializedContacts = JsonConvert.SerializeObject(contacts);
                    _cache.SetString(contactsCacheKey, serializedContacts, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
                        });
                }

                result = contactRepo.ProcessPersonContacts(contacts, result, _cache);
                result = result.OrderBy( r => r.KersUser.RprtngProfile.PlanningUnit.order).ThenBy(r => r.KersUser.PersonalProfile.FirstName).ToList();



                table.Header = new List<string>{
                                "Planning Unit", "Employee", "Days", "Multistate", "Total Contacts"
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
                i = 0;
                foreach( var opnmb in OptionNumbers){
                    table.Foother.Add( totalPerOptionNumber[i].ToString());
                    i++;
                }

                var serialized = JsonConvert.SerializeObject(table);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });
            }
            ViewData["Type"] = type;
            ViewData["Subtitle"] = types[type];
            if(type == 0){
                ViewData["Title"] = this.context.District.Find(id).Name;
            }else if(type == 1){
                ViewData["Title"] = this.context.PlanningUnit.Find(id).Name;
            }
            

            return View(table);
        }

        private async Task<List<ActivityPersonResult>> DistrictActivities(int id){
            var activities = await this.context.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < currentFiscalYear.End 
                                                                && 
                                                                a.ActivityDate > currentFiscalYear.Start
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

            return activities;

        }

        private async Task<List<ContactPersonResult>> DistrictContacts(int id){
           var contacts = await this.context.Contact.
                                    Where( c => 
                                                c.ContactDate < currentFiscalYear.End 
                                                && 
                                                c.ContactDate > currentFiscalYear.Start 
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


        private async Task<List<ActivityPersonResult>> UnitActivities(int id){
            var activities = await this.context.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < currentFiscalYear.End 
                                                                && 
                                                                a.ActivityDate > currentFiscalYear.Start
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

            return activities;

        }

        private async Task<List<ContactPersonResult>> UnitContacts(int id){
           var contacts = await this.context.Contact.
                                    Where( c => 
                                                c.ContactDate < currentFiscalYear.End 
                                                && 
                                                c.ContactDate > currentFiscalYear.Start 
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


        private async Task<List<ActivityPersonResult>> KSUActivities(){
            var activities = await this.context.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < currentFiscalYear.End 
                                                                && 
                                                                a.ActivityDate > currentFiscalYear.Start
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

            return activities;

        }

        private async Task<List<ContactPersonResult>> KSUContacts(){
           var contacts = await this.context.Contact.
                                    Where( c => 
                                                c.ContactDate < currentFiscalYear.End 
                                                && 
                                                c.ContactDate > currentFiscalYear.Start 
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
        

        
       
    }

}