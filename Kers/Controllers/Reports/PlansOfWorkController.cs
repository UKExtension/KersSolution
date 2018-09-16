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
    public class PlansOfWorkController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepository;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        private FiscalYear currentFiscalYear;
        public PlansOfWorkController( 
                    KERScoreContext context,
                    IDistributedCache _cache,
                    IFiscalYearRepository fiscalYearRepository,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo
            ){
           this.context = context;
           this._cache = _cache;
           this.fiscalYearRepository = fiscalYearRepository;
           this.currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");
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
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            var planofwork = await this.context.PlanOfWork
                                        .Where( p =>    p.Id == id)
                                        .Include( p => p.PlanningUnit)
                                        .Include( p => p.Revisions ).ThenInclude( r => r.Map)
                                        .FirstOrDefaultAsync();
            if( planofwork == null){
                new Exception("No Plan of Work with Provided Identifier.");
            }
            PlanOfWorkViewModel plan;
            plan = new PlanOfWorkViewModel();
            plan.Id = id;
            plan.PlanningUnit = planofwork.PlanningUnit;
            plan.LastRevision = planofwork.Revisions.OrderBy( r => r.Created).Last();
                
            return View(plan);
        }

        [HttpGet("countylist/{fy?}")]
        public async Task<IActionResult> Countylist(string fy = "0"){

            List<PlanningUnit> counties;
            var cacheKey = "CountiesList";
            var cached = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cached)){
                counties = JsonConvert.DeserializeObject<List<PlanningUnit>>(cached);
            }else{
            
            
                counties = await this.context.PlanningUnit.
                                Where(c=>c.District != null && c.Name.Substring(c.Name.Count() - 3) == "CES").
                                OrderBy(c => c.Name).ToListAsync();
                

                var serializedCounties = JsonConvert.SerializeObject(counties);
                _cache.SetString(cacheKey, serializedCounties, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                        });
            }
            ViewData["fy"] = fy;
            return View(counties);
        }

        [HttpGet("programlist/{fy?}")]
        public async Task<IActionResult> Programlist(string fy = "0"){

            List<MajorProgram> programs;
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = currentFiscalYear;
            }else{
                fiscalYear = await this.context.FiscalYear.Where( f => f.Name == fy && f.Type == "serviceLog").FirstOrDefaultAsync();
                if( fiscalYear == null){
                    new Exception("No Fiscal Year with Provided Identifier.");
                }
            }
            ViewData["fy"] = fiscalYear.Name;
            programs = await this.context.MajorProgram.
                                Where(m => m.StrategicInitiative.FiscalYear == fiscalYear).
                                OrderBy(c => c.PacCode).ToListAsync();

            return View(programs);
        }



        
        [HttpGet("plansbycounty/{id}/{fy?}")]
        public async Task<IActionResult> PlansByCounty(int id, string fy = "0"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = currentFiscalYear;
            }else{
                fiscalYear = await this.context.FiscalYear.Where( f => f.Name == fy && f.Type == "serviceLog").FirstOrDefaultAsync();
                if( fiscalYear == null){
                    new Exception("No Fiscal Year with Provided Identifier.");
                }
            }
            ViewData["fy"] = fiscalYear.Name;
            var plansofwork = await this.context.PlanOfWork
                                        .Where( p =>    p.PlanningUnit.Id == id
                                                        && 
                                                        p.FiscalYear == fiscalYear)
                                        .Include( p => p.PlanningUnit)
                                        .Include( p => p.Revisions ).ThenInclude( r => r.Map)
                                        .ToListAsync();

            List<PlanOfWorkViewModel> plans = new List<PlanOfWorkViewModel>();
            foreach( var plan in plansofwork){
                var pow = new PlanOfWorkViewModel();
                pow.Id = plan.Id;
                pow.FiscalYear = fiscalYear;
                pow.PlanningUnit = plan.PlanningUnit;
                pow.LastRevision = plan.Revisions.OrderBy( r => r.Created).Last();
                plans.Add(pow);
            }
            return View(plans);
        }



        [HttpGet("plansbyprogram/{id}/{fy?}")]
        public async Task<IActionResult> PlansByProgram(int id, string fy = "0"){
                
            var program = await context.MajorProgram.Where( p => p.Id == id )
                                .Include( p => p.StrategicInitiative ).ThenInclude( i => i.FiscalYear)
                                .FirstOrDefaultAsync();

            FiscalYear fiscalYear = program.StrategicInitiative.FiscalYear;

            var FyPlansOfWork = this.context.PlanOfWork
                                        .Where( p => p.FiscalYear == fiscalYear);
            List<PlanOfWorkRevision> LastRevisions = new List<PlanOfWorkRevision>();
            foreach( var plan in FyPlansOfWork){
                var r = this.context.PlanOfWorkRevision.Where( v => v.PlanOfWorkId == plan.Id).Include( v => v.Map).OrderBy( v => v.Created ).Last();
                if( 
                    r.Mp1Id == id 
                    ||
                    r.Mp2Id == id
                    ||
                    r.Mp3Id == id
                    ||
                    r.Mp4Id == id
                ){
                    LastRevisions.Add( r );
                }
            }

            List<PlanOfWorkViewModel> plans = new List<PlanOfWorkViewModel>();
            foreach( var plan in LastRevisions){
                var pow = new PlanOfWorkViewModel();
                pow.Id = plan.PlanOfWorkId;
                pow.FiscalYear = fiscalYear;

                var planOfWork = await this.context.PlanOfWork.Where( p => p.Id == plan.PlanOfWorkId).Include( r => r.PlanningUnit).FirstOrDefaultAsync();
                if(planOfWork != null && planOfWork.PlanningUnit != null){
                    pow.PlanningUnit = planOfWork.PlanningUnit;
                }
                
                pow.LastRevision = plan;
                plans.Add(pow);
            }
            ViewData["ProgramId"] = id;
            ViewData["fy"] = fiscalYear.Name;
            return View(plans);
        }




        [HttpGet("planfullcounty/{id}")]
        public async Task<IActionResult> PlanFullCounty(int id){
            var plan = await this.PlanFull(id);
            return View(plan);
        }





        [HttpGet("planfullprogram/{id}/{programid}")]
        public async Task<IActionResult> PlanFullProgram(int id, int programid){
            var plan = await this.PlanFull(id);
            ViewData["ProgramId"] = programid;
            return View(plan);
        }

        private  async Task<PlanOfWorkViewModel> PlanFull(int id){
            var plan = await this.context.PlanOfWork.Where( p => p.Id == id)
                                        .Include( p => p.PlanningUnit)
                                        .Include( p => p.Revisions ).ThenInclude( r => r.Map)
                                        .Include( p => p.Revisions ).ThenInclude( r => r.Mp1)
                                        .Include( p => p.Revisions ).ThenInclude( r => r.Mp2)
                                        .Include( p => p.Revisions ).ThenInclude( r => r.Mp3)
                                        .Include( p => p.Revisions ).ThenInclude( r => r.Mp4)
                                        .FirstOrDefaultAsync();
            var pow = new PlanOfWorkViewModel();
            pow.Id = plan.Id;
            pow.PlanningUnit = plan.PlanningUnit;
            pow.LastRevision = plan.Revisions.OrderBy( r => r.Created).Last();
            return pow;
        }



    }
}