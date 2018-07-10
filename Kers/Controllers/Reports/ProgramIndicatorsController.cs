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
    public class ProgramIndicatorsController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepository;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        private FiscalYear currentFiscalYear;
        public ProgramIndicatorsController( 
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
        [Route("{id}/{fy?}")]
        public async Task<IActionResult> Get(int id, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                
                return new StatusCodeResult(500);
            }
            var indicators = new List<StrategicInitiativeIndicatorsViewModel>();
            if( id != 0){
                var county = context.PlanningUnit.Find(id);
                if(county == null){
                    return new StatusCodeResult(500);
                }
                ViewData["county"] = county.Name;
            }else{
                ViewData["county"] = "STATE TOTALS";
            }
            var initiatives = await context.StrategicInitiative.Where(s => s.FiscalYear == fiscalYear)
                                    .Include( s => s.ProgramCategory)
                                    .Include( s => s.MajorPrograms)
                                    .OrderBy( s => s.order)
                                    .ToListAsync();
            var indicatorsPerProgram = await context.ProgramIndicator
                                                .GroupBy( p => p.MajorProgramId )
                                                .Select( g => new {
                                                    MajorProgramId = g.Key,
                                                    Indicators = g.Select( d => d ).OrderBy( d => d.order)
                                                }).ToListAsync();



            foreach( var initiative in initiatives){
                var intv = new StrategicInitiativeIndicatorsViewModel();
                intv.Title = initiative.ProgramCategory.ShortName + " " + initiative.Name;
                intv.Code = initiative.PacCode;
                intv.Indicators = new List<MajorProgramIndicatorsViewModel>();
                foreach(var program in initiative.MajorPrograms){
                    var programViewModel = new MajorProgramIndicatorsViewModel();
                    programViewModel.Title = program.Name;
                    programViewModel.Code = program.PacCode;
                    programViewModel.Indicators = new List<IndicatorViewModel>();
                    var indicatorsPerThisProgram = indicatorsPerProgram.Where( n => n.MajorProgramId == program.Id).FirstOrDefault();
                    if( indicatorsPerThisProgram != null ){
                        var i = 1;
                        foreach( var indctr in indicatorsPerThisProgram.Indicators){
                            var ind = new IndicatorViewModel();
                            ind.Description = indctr.Question;
                            ind.Code = i;
                            if( id == 0){
                                ind.Amount = context.ProgramIndicatorValue.Where( v => v.ProgramIndicatorId == indctr.Id ).Sum( d => d.Value);
                            }else{
                                ind.Amount = context.ProgramIndicatorValue
                                                            .Where( v => 
                                                                        v.ProgramIndicatorId == indctr.Id 
                                                                            &&
                                                                        v.KersUser.RprtngProfile.PlanningUnitId == id
                                                                        )
                                                            .Sum( d => d.Value);
                            }
                            i++;
                            programViewModel.Indicators.Add( ind );
                        }
                    }
                    intv.Indicators.Add(programViewModel);
                }

                indicators.Add( intv );
            }
            
                
            return View(indicators);
        }

        [HttpGet("countylist")]
        public async Task<IActionResult> Countylist(){

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
            return View(counties);
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

    public class StrategicInitiativeIndicatorsViewModel{
        public int Code;
        public string Title;
        public List<MajorProgramIndicatorsViewModel> Indicators;
    }

    public class MajorProgramIndicatorsViewModel{
        public int Code;
        public string Title;
        public List<IndicatorViewModel> Indicators;

    }
    public class IndicatorViewModel{
        public int Code;
        public int Amount;
        public string Description;

    }



}