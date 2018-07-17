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
using Kers.Models.Data;
using Kers.Models.ViewModels;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class ByProgramController : Controller
    {
        KERScoreContext context;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        IFiscalYearRepository fiscalYearRepository;
        private FiscalYear currentFiscalYear;
        const int workDaysPerYear = 228;
        public ByProgramController( 
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
        [Route("{type}/{id?}/{fy?}")]
        public async Task<IActionResult> Index(int type, int id = 0, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                //this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", "Reports", "Error");
                return new StatusCodeResult(500);
            }
            
            var cacheKey = "ByMajorProgramData" + type.ToString() + id.ToString() + "_" + fy;
            var cached = _cache.GetString(cacheKey);
            
            TableViewModel table;


            if (!string.IsNullOrEmpty(cached)){
                table = JsonConvert.DeserializeObject<TableViewModel>(cached);
            }else{



                List<ActivityMajorProgramResult> activities;

                var actvtsCacheKey = "AllActivitiesByMajorProgram" + type.ToString() + id.ToString() + "_" + fy;
                var cachedActivities = _cache.GetString(actvtsCacheKey);

                if (!string.IsNullOrEmpty(cachedActivities)){
                    activities = JsonConvert.DeserializeObject<List<ActivityMajorProgramResult>>(cachedActivities);
                }else{


                    if(type == 0){
                        activities = await DistrictActivities(id, fiscalYear);
                    }else{
                        return new StatusCodeResult(404);
                    }

                    var serializedActivities = JsonConvert.SerializeObject(activities);
                    _cache.SetString(actvtsCacheKey, serializedActivities, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
                        });

                }
                var result = activityRepo.ProcessMajorProgramActivities(activities,  _cache);


                List<ContactMajorProgramResult> contacts;
                var contactsCacheKey = "ContactsByEmployee" + type.ToString() + id.ToString() + "_" + fy;
                var cachedContacts = _cache.GetString(contactsCacheKey);

                if (!string.IsNullOrEmpty(cachedContacts)){
                    contacts = JsonConvert.DeserializeObject<List<ContactMajorProgramResult>>(cachedContacts);
                }else{
                    if(type == 0){
                        contacts = await DistrictContacts(id, fiscalYear);
                    }else{
                        contacts = new List<ContactMajorProgramResult>();
                    }
                    var serializedContacts = JsonConvert.SerializeObject(contacts);
                    _cache.SetString(contactsCacheKey, serializedContacts, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
                        });
                }
                result = contactRepo.ProcessMajorProgramContacts(contacts, result, _cache);

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
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });

            }

            return View(table);
        }
        


        private async Task<List<ActivityMajorProgramResult>> DistrictActivities(int id, FiscalYear fiscalYear){
            var activities = await this.context.Activity
                                                    .Where( a => 
                                                                a.ActivityDate < fiscalYear.End 
                                                                && 
                                                                a.ActivityDate > fiscalYear.Start
                                                                &&
                                                                a.PlanningUnit.DistrictId == id
                                                            )
                                                    .GroupBy(e => new {
                                                        MajorProgram = e.MajorProgram
                                                    })
                                                    .Select(c => new ActivityMajorProgramResult{
                                                        Ids = c.Select(
                                                            s => s.Id
                                                        ).ToList(),
                                                        Hours = c.Sum(s => s.Hours),
                                                        Audience = c.Sum(s => s.Audience),
                                                        MajorProgram = c.Key.MajorProgram
                                                    })
                                                    .ToListAsync();

            return activities;

        }

        private async Task<List<ContactMajorProgramResult>> DistrictContacts(int id, FiscalYear fiscalYear){
           var contacts = await this.context.Contact.
                                    Where( c => 
                                                c.ContactDate < fiscalYear.End 
                                                && 
                                                c.ContactDate > fiscalYear.Start 
                                                && 
                                                c.PlanningUnit.DistrictId == id
                                        )
                                        .GroupBy(e => new {
                                            MajorProgram = e.MajorProgram
                                        })
                                        .Select(c => new ContactMajorProgramResult{
                                            Ids = c.Select(
                                                s => s.Id
                                            ).ToList(),
                                            MajorProgram = c.Key.MajorProgram
                                        })
                                        .ToListAsync();
            return contacts;
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