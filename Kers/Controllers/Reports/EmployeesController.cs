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

        private IConfiguration _configuration;

        string[] types = new string[]{ "District Reports", "Planning Unit Report", "KSU" };
        public EmployeesController( 
                    KERScoreContext context,
                    IFiscalYearRepository fiscalYearRepository,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo,
                    IConfiguration Configuration
            ){
           this.context = context;
           this.fiscalYearRepository = fiscalYearRepository;
           this.currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");
           this._cache = _cache;
           this.activityRepo = activityRepo;
           this.contactRepo = contactRepo;
           _configuration = Configuration;
        }


        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {

            // Include County into program indicator values

            var pind = context.ProgramIndicatorValue.Where( a => true );
            foreach( var ind in pind ){
                if(ind.PlanningUnitId == 0){
                    var user = context.KersUser.Where( u => u.Id == ind.KersUserId).Include( u => u.RprtngProfile ).FirstOrDefault();
                    ind.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                    ind.CreatedDateTime = DateTimeOffset.Now;
                    ind.LastModifiedDateTime = DateTimeOffset.Now;
                }
            }



            /* 
            var revsWith2019MP = context.ActivityRevision.Where( a => a.ActivityDate < new DateTime( 2018, 7, 1) );
            revsWith2019MP = revsWith2019MP.Where( r => r.MajorProgram.StrategicInitiative.FiscalYear.Name == "2019");
            revsWith2019MP = revsWith2019MP
                                    .Include( r => r.MajorProgram);
            
            foreach( var rev in revsWith2019MP ){
                var mp2018 = context.MajorProgram.Where( m => m.StrategicInitiative.FiscalYear.Name == "2018" && m.Name == rev.MajorProgram.Name).FirstOrDefault();
                if( mp2018 != null){
                    rev.MajorProgram = mp2018;
                    var actvt = context.Activity.Where( a => a.Id == rev.ActivityId ).FirstOrDefault();
                    actvt.MajorProgram = mp2018;
                }
                

            } */
            context.SaveChanges();
            return View();
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


    class KersReportingContext:DbContext{
        public KersReportingContext(DbContextOptions<KersReportingContext> options)
        : base(options)
        { }

        public virtual DbSet<vInServiceQualtricsSurveysToCreate> vInServiceQualtricsSurveysToCreate { get; set; }
    }

    public partial class vInServiceQualtricsSurveysToCreate
    {
        [Key]
        public int rID {get;set;}
        public string tID {get; set;}
        public string trainDateBegin {get;set;}
        public string  tTitle {get;set;}
        public string qualtricsTitle {get;set;}
    }




}