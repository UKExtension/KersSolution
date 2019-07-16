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
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.IO;
using CsvHelper;
using Kers.Models.Entities.UKCAReporting;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class EmployeesController : Controller
    {
        KERScoreContext context;
        KERSreportingContext reportingContext;
        ITrainingRepository trainingRepo;
        IFiscalYearRepository fiscalYearRepository;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        IExpenseRepository expensesRepo;
        private FiscalYear currentFiscalYear;

        private IConfiguration _configuration;

        string[] types = new string[]{ "District Reports", "Planning Unit Report", "KSU" };
        public EmployeesController( 
                    KERScoreContext context,
                    KERSreportingContext reportingContext ,
                    ITrainingRepository trainingRepo,
                    IFiscalYearRepository fiscalYearRepository,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo,
                    IExpenseRepository expenseRepo,
                    IConfiguration Configuration
            ){
           this.context = context;
           this.reportingContext = reportingContext;
           this.trainingRepo = trainingRepo;
           this.fiscalYearRepository = fiscalYearRepository;
           this.currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");
           this._cache = _cache;
           this.activityRepo = activityRepo;
           this.contactRepo = contactRepo;
           this.expensesRepo = expenseRepo;
           _configuration = Configuration;
        }


        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            //List<zInServiceTrainingCatalog> inServices = this.trainingRepo.csv2list().Take(10).ToList();

            //var inServices = reportingContext.zInServiceTrainingCatalog.Where(s => true).OrderByDescending(r => r.rID);
            //ViewData["trainings"] = inServices;
            //var trainings = this.trainingRepo.InServicesToTrainings(inServices.Skip(300).Take(10).ToList());
            //this.context.Training.AddRange(trainings);
            //this.context.SaveChanges();


/* 
            //Update Users timezones based on the timezone of their county


            var cnts = context.PlanningUnit.Where( u => u.TimeZoneId != null);
            foreach( var cnt in cnts){
                var usrs = context.KersUser
                    .Where( u => u.RprtngProfile.enabled == true && u.RprtngProfile.PlanningUnit == cnt)
                    .Include( u => u.PersonalProfile);
                foreach( var usr in usrs){
                    usr.PersonalProfile.TimeZoneId = cnt.TimeZoneId;
                }
            }

            context.SaveChanges();


 */

            //var indctrs = this.moveIndicators("2020", "2019");
            //var strs = this.moveStories("2020", "2019");
            return View();
        }

        private List<ProgramIndicator> moveIndicators( string fromFYName, string toFYName){
            var indicators = new List<ProgramIndicator>();

            
            var vals = this.context.ProgramIndicatorValue
                        .Where( v => v.ProgramIndicator.MajorProgram.StrategicInitiative.FiscalYear.Name == fromFYName)
                        .Include( v => v.ProgramIndicator);

            foreach( var val in vals){

                var replacement = this.context.ProgramIndicator
                                    .Where( i => i.MajorProgram.StrategicInitiative.FiscalYear.Name == toFYName
                                                &&
                                                i.Question == val.ProgramIndicator.Question)
                                    .FirstOrDefault();
                if( replacement != null ) val.ProgramIndicator = replacement;
                indicators.Add( val.ProgramIndicator);
            }

            this.context.SaveChanges();

            return indicators;
        }

        private List<Story> moveStories( string fromFYName, string toFYName){
            var strs = new List<Story>();

            var stories = this.context.Story
                            .Where( s => 
                                s.MajorProgram.StrategicInitiative.FiscalYear.Name == fromFYName 
                                && 
                                s.MajorProgram.StrategicInitiative.FiscalYear.Type == FiscalYearType.ServiceLog)
                            .Include( r => r.Revisions)
                            .Include( r => r.MajorProgram);

            foreach( var str in stories){
                var replacement = this.context.MajorProgram.Where(
                    m => 
                        m.StrategicInitiative.FiscalYear.Name == toFYName
                        &&
                        m.StrategicInitiative.FiscalYear.Type == FiscalYearType.ServiceLog
                        &&
                        m.PacCode == str.MajorProgram.PacCode
                ).FirstOrDefault();
                if(replacement != null){
                    foreach( var rev in str.Revisions){
                        rev.MajorProgram = replacement;
                        rev.PlanOfWorkId = 0;
                    }
                    str.MajorProgram = replacement;
                }
                strs.Add( str );
            }

            this.context.SaveChanges();
            return strs;
        }


        [HttpGet]
        [Route("expenses/{districtid?}/{month?}/{year?}")]
        public IActionResult Expenses(int districtid=4, int month = 8, int year = 2018)
        {

            var ret = new List<List<ExpenseSummary>>();   

            /* 
            var allSummaries = new List<ExpenseSummary>();

            var counties = context.PlanningUnit
                                        .Where( u => u.DistrictId == districtid && u.Name.Substring( u.Name.Count() - 3, 3) == "CES")
                                        .OrderBy( u => u.Name )
                                        .ToArray();
            ViewData["counties"] = counties;
            var allEmployees = new List<KersUser>();
            foreach( var county in counties){
                var ByCounty = new List<ExpenseSummary>();
                var emp = this.context.KersUser.Where( u => u.RprtngProfile.PlanningUnit == county).Include( u => u.RprtngProfile);
                allEmployees.AddRange( emp );
                foreach( var e in emp ){
                    var sumr = this.expensesRepo.Summaries(e, year, month);
                    ByCounty.AddRange( sumr );
                }
                allSummaries.AddRange( ByCounty );
                ret.Add( BySource( ByCounty ) );
            }

            ViewData["total"] = BySource(allSummaries);
            
 */
            

            return View(ret);
        }

        private List<ExpenseSummary> BySource( List<ExpenseSummary> mixed ){
            var sources = this.context.ExpenseFundingSource;
            var bySource = new List<ExpenseSummary>();
            foreach( var src in sources ){
                var sumrs = mixed.Where( s => s.fundingSource == src);
                var smr = new ExpenseSummary{
                    fundingSource = src,
                    miles = sumrs.Sum( s => s.miles),
                    mileageCost = sumrs.Sum( s => s.mileageCost),
                    meals = sumrs.Sum( s => s.meals),
                    lodging = sumrs.Sum( s => s.lodging),
                    registration = sumrs.Sum( s => s.registration),
                    other = sumrs.Sum( s => s.other),
                    total = sumrs.Sum( s => s.total)
                };
                bySource.Add( smr );

            }

            return bySource;
        }


        [HttpGet]
        [Route("[action]/{type}/{id?}/{fy?}")]
        // type: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
        public async Task<IActionResult> Data(int type, int id = 0, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                
                return new StatusCodeResult(500);
            }
            

            var table = await contactRepo.DataByEmployee(fiscalYear, type, id);

            ViewData["Type"] = type;
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            ViewData["Subtitle"] = types[type];
            if(type == 0){
                ViewData["Title"] = this.context.District.Find(id).Name;
            }else if(type == 1){
                ViewData["Title"] = this.context.PlanningUnit.Find(id).Name;
            }
            

            return View(table);
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