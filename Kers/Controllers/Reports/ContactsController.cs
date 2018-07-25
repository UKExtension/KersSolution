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
        public async Task<IActionResult> StateAll(string fy = "0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                //this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", "Reports", "Error");
                return new StatusCodeResult(500);
            }

            ViewData["FiscalYear"] = fiscalYear;
            var table = await contactRepo.DataByEmployee(fiscalYear, 4);
            return View(table);
        }


        [HttpGet]
        [Route("[action]/{type?}/{fy?}")]
        // type: 0 - all, 1 - UK, 2 - KSU
        public async Task<IActionResult> StateByMajorProgram(int type = 0, string fy = "0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            var repoType = 4;


            

            if(type == 1){
                repoType = 3;
                ViewData["Title"] = "UK ";
            }else if( type == 2 ){
                repoType = 2;
                ViewData["Title"] = "KSU ";
            }
            var table = await contactRepo.DataByMajorProgram(fiscalYear, repoType);
            ViewData["FiscalYear"] = fiscalYear;
            return View(table);
        }


        [HttpGet]
        [Route("[action]/{fy?}")]
        public async Task<IActionResult> ContactsByCountyByMajorProgram(string fy = "0")
        {
            
            FiscalYear fiscalYear = GetFYByName(fy);
            

            if(fiscalYear == null){
                //this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", "Reports", "Error");
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;
            var table = await activityRepo.ContactsByCountyByMajorProgram(fiscalYear);
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