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
    public class SoilController : Controller
    {
        KERScoreContext _coreContext;
        SoilDataContext _context;
        private IDistributedCache _cache;

        public SoilController( 
                    SoilDataContext _context,
                    KERScoreContext _coreContext
            ){
                this._context = _context;
                this._coreContext = _coreContext;

        }

        [HttpGet]
        [Route("customer/{code}/")]
        public async Task<IActionResult> Index(string code)
        {
            var address = await this._context.FarmerAddress.Where(a => a.UniqueCode == code).FirstOrDefaultAsync();
            return View(address);
        }

/* 

        [HttpGet]
        [Route("{type}/{fy?}/{id?}/")]
        // type: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
        public async Task<IActionResult> Index(int type, int id = 0, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                //this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", "Reports", "Error");
                return new StatusCodeResult(500);
            }
            if( type == 0 ){
               ViewData["Title"] = this.context.District.Find( id ).Name;
            }else if( type == 1 ){
                ViewData["Title"] = this.context.PlanningUnit.Find( id ).Name;
            }else if( type == 2) {
                ViewData["Title"] = "KSU";
            }else if ( type == 3){
                ViewData["Title"] = "UK";
            }

            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            var table = await contactRepo.DataByMajorProgram(fiscalYear, type == 1 ? 0 : type, id);

    
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

 */

    }
}