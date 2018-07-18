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
            var activitiesWith2019MajPr = this.context.Activity.Where( a => a.MajorProgram.StrategicInitiative.FiscalYear.Name == "2019");
            activitiesWith2019MajPr = activitiesWith2019MajPr.Where( a => a.ActivityDate.Month < 7 && a.ActivityDate.Year < 2019 );



            return View(activitiesWith2019MajPr.ToList());
        }


        [HttpGet]
        [Route("[action]/{type}/{id?}/{fy?}")]
        public async Task<IActionResult> Data(int type, int id = 0, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                
                return new StatusCodeResult(500);
            }

            var table = await contactRepo.Data(fiscalYear, type, id);

            ViewData["Type"] = type;
            ViewData["Subtitle"] = types[type];
            if(type == 0){
                ViewData["Title"] = this.context.District.Find(id).Name;
            }else if(type == 1){
                ViewData["Title"] = this.context.PlanningUnit.Find(id).Name;
            }
            

            return View(table);
        }
/* 
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
        } */

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