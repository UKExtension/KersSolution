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

        [HttpGet]
        [Route("{fy?}")]
        public async Task<IActionResult> Index(string fy="0")
        {

            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;

            var fiscalYearSummaries = await contactRepo.GetPerPeriodSummaries(fiscalYear.Start, fiscalYear.End, 4, 0, false);
            float[] SummariesArray = fiscalYearSummaries.ToArray();

            ViewData["totalHours"] = SummariesArray[0];
            ViewData["totalContacts"] = (int) SummariesArray[1];
            ViewData["totalMultistate"] = SummariesArray[2];
            ViewData["totalActivities"] = (int) SummariesArray[3];




            var StatsLastMonth = await contactRepo.StatsPerMonth();
            ViewData["StatsLastMonth"] = StatsLastMonth;
            DateTime ago = DateTime.Now.AddMonths(-2);
            var StathsTwoMonthsAgo = await contactRepo.StatsPerMonth( ago.Year, ago.Month );
            ViewData["StathsTwoMonthsAgo"] = StathsTwoMonthsAgo;
            var TopPrograms = await activityRepo.TopProgramsPerFiscalYear(fiscalYear);
            ViewData["TopPrograms"] = TopPrograms;
            var sum = TopPrograms.Sum( a => a.DirectContacts );
            ViewData["TopProgramsAudienceSum"] = sum;
            return View();
        }
        [HttpGet]
        [Route("counties/{fy?}")]
        public IActionResult Counties(string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            var filteredCounties = this.context.PlanningUnit.
                                Where(c=>c.District != null)
                                .Include( c => c.District)
                                .ToList();
            filteredCounties = filteredCounties.Where( c =>  c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
            var counties =  filteredCounties.
                                GroupBy( c => c.District )
                                .Select( g => new DistrictViewModel{
                                                District = g.Key,
                                                Counties = g.Select( a => a).OrderBy( c => c.Name).ToList()
                                            }
                                        )
                                .OrderBy(c => c.District.Name).ToList();
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
            ViewData["fy"] = fiscalYear.Name;


            var fiscalYearSummaries = await contactRepo.GetPerPeriodSummaries(fiscalYear.Start, fiscalYear.End, 1, id, false, 100);
            float[] SummariesArray = fiscalYearSummaries.ToArray();

            ViewData["totalHours"] = SummariesArray[0];
            ViewData["totalContacts"] = (int) SummariesArray[1];
            ViewData["totalMultistate"] = SummariesArray[2];
            ViewData["totalActivities"] = (int) SummariesArray[3];
            

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
            var TopPrograms = await activityRepo.TopProgramsPerFiscalYear( fiscalYear, 5, id );
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
            ViewData["Units"] = await units;
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            

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
                    ViewData["MakeupDiversityGroups"] = await MakeupDiversityGroups;
                    var AdvisoryGroups = this.context.AffirmativeAdvisoryGroupType.Where( t => true ).OrderBy( t => t.Order ).ToListAsync();
                    ViewData["AdvisoryGroups"] = await AdvisoryGroups;
                    var SummaryDiversityType = this.context.AffirmativeSummaryDiversityType.Where( t => true ).OrderBy( t => t.Order ).ToListAsync();
                    ViewData["SummaryDiversityType"] = await SummaryDiversityType;
                    model = await context.AffirmativeActionPlanRevision
                                    .Where( r => r.AffirmativeActionPlan == plan )
                                    .Include( r => r.MakeupValues)
                                    .Include( r => r.SummaryValues)
                                    .OrderBy( r => r.Created )
                                    .LastOrDefaultAsync();
                    
                    
                    
                }
            }

            
            
            return View( model);


        }
        
        private FiscalYear GetFYByName(string fy, string type = "serviceLog"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalYearRepo.previoiusFiscalYear(type);
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == type).FirstOrDefault();
                if(fiscalYear == null ){
                    fiscalYear = this.fiscalYearRepo.currentFiscalYear(type);
                }
            }
            return fiscalYear;
        }


    }

    public class DistrictViewModel{
        public District District {get;set;}
        public List<PlanningUnit> Counties {get;set;}
    }

}