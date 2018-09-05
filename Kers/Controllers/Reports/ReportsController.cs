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

namespace Kers.Controllers.Reports
{

    [Route("[controller]")]
    public class ReportsController : Controller
    {
        KERScoreContext context;
        IStoryRepository storyRepo;
        IActivityRepository activityRepo;
        IFiscalYearRepository fiscalYearRepo;
        IContactRepository contactRepo;

        public ReportsController( 
                    KERScoreContext context,
                    IStoryRepository storyRepo,
                    IContactRepository contactRepo,
                    IActivityRepository activityRepo,
                    IFiscalYearRepository fiscalYearRepo
            ){
            this.context = context;
            this.storyRepo = storyRepo;
            this.contactRepo = contactRepo;
            this.activityRepo = activityRepo;
            this.fiscalYearRepo = fiscalYearRepo;
        }

        public async Task<IActionResult> Index()
        {
            var StatsLastMonth = await contactRepo.StatsPerMonth();
            ViewData["StatsLastMonth"] = StatsLastMonth;
            DateTime ago = DateTime.Now.AddMonths(-2);
            var StathsTwoMonthsAgo = await contactRepo.StatsPerMonth( ago.Year, ago.Month );
            ViewData["StathsTwoMonthsAgo"] = StathsTwoMonthsAgo;
            var TopPrograms = await activityRepo.TopProgramsPerMonth();
            ViewData["TopPrograms"] = TopPrograms;
            var sum = TopPrograms.Sum( a => a.DirectContacts );
            ViewData["TopProgramsAudienceSum"] = sum;
            return View();
        }
        [HttpGet]
        [Route("counties")]
        public async Task<IActionResult> Counties()
        {
            var counties = await this.context.PlanningUnit.
                                Where(c=>c.District != null && c.Name.Substring(c.Name.Count() - 3) == "CES").
                                Include( c => c.District).
                                GroupBy( c => c.District )
                                .Select( g => new DistrictViewModel{
                                                District = g.Key,
                                                Counties = g.Select( a => a).OrderBy( c => c.Name).ToList()
                                            }
                                        )
                                .OrderBy(c => c.District.Name).ToListAsync();
            return View(counties);
        }

        [HttpGet]
        [Route("county/{id}/{fy?}")]
        public async Task<IActionResult> County(int id, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;

            ViewData["County"] = await this.context
                                .PlanningUnit.Where( p => p.Id == id )
                                .Include( p => p.District)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
            var lastMonth = DateTime.Now.AddMonths(-1);
            var StatsLastMonth = await contactRepo.StatsPerMonth(lastMonth.Year,lastMonth.Month,id);
            ViewData["StatsLastMonth"] = StatsLastMonth;
            DateTime ago = DateTime.Now.AddMonths(-2);
            var StathsTwoMonthsAgo = await contactRepo.StatsPerMonth( ago.Year, ago.Month, id );
            ViewData["StathsTwoMonthsAgo"] = StathsTwoMonthsAgo;
            var TopPrograms = await activityRepo.TopProgramsPerMonth( 0, 0, 5, id );
            ViewData["TopPrograms"] = TopPrograms;
            var sum = TopPrograms.Sum( a => a.DirectContacts );
            ViewData["TopProgramsAudienceSum"] = sum;
            return View();
        }

        [HttpGet]
        [Route("[action]/{fy?}/{countyId?}", Name = "AffirmativeAction")]
        public async Task<IActionResult> AffirmativeAction(string fy = "0", int countyId = 0)
        {
            
            FiscalYear fiscalYear = GetFYByName(fy);
            var units = this.context.PlanningUnit.OrderBy(l => l.order).ToListAsync();
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;
            
            

            AffirmativeActionPlanRevision model = null;
            if(countyId != 0){
                var unit = await this.context.PlanningUnit.Where( u => u.Id == countyId).FirstOrDefaultAsync();
                if(unit == null ){
                    return new StatusCodeResult(500);
                }
                ViewData["Unit"] = unit;
                var plan = await context.AffirmativeActionPlan.Where( p => p.FiscalYear == fiscalYear && p.PlanningUnit == unit ).FirstOrDefaultAsync();
                if( plan != null){
                    var MakeupDiversityGroups = this.context.AffirmativeMakeupDiversityTypeGroup
                                                        .Where( g => true )
                                                        .Include( g => g.Types)
                                                        .OrderBy( g => g.Order ).ToListAsync();
                    var AdvisoryGroups = this.context.AffirmativeAdvisoryGroupType.Where( t => true ).OrderBy( t => t.Order ).ToListAsync();
                    var SummaryDiversityType = this.context.AffirmativeSummaryDiversityType.Where( t => true ).OrderBy( t => t.Order ).ToListAsync();
                    model = await context.AffirmativeActionPlanRevision
                                    .Where( r => r.AffirmativeActionPlan == plan )
                                    .Include( r => r.MakeupValues)
                                    .Include( r => r.SummaryValues)
                                    .OrderBy( r => r.Created )
                                    .LastOrDefaultAsync();
                    ViewData["MakeupDiversityGroups"] = await MakeupDiversityGroups;
                    ViewData["AdvisoryGroups"] = await AdvisoryGroups;
                    ViewData["SummaryDiversityType"] = await SummaryDiversityType;
                }
            }

            ViewData["Units"] = await units;
            
            return View( model);


        }
        
        private FiscalYear GetFYByName(string fy, string type = "serviceLog"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalYearRepo.currentFiscalYear(type);
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == type).FirstOrDefault();
            }
            return fiscalYear;
        }


    }

    public class DistrictViewModel{
        public District District {get;set;}
        public List<PlanningUnit> Counties {get;set;}
    }

}