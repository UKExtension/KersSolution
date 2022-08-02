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
            
            if( id != 0){
                var county = context.PlanningUnit.Find(id);
                if(county == null){
                    return new StatusCodeResult(500);
                }
                ViewData["county"] = county.Name;
            }else{
                ViewData["county"] = "STATE TOTALS";
            }


            var cacheKey = "ProgramIndicatorsReport" + fiscalYear.Name + "_" + id.ToString();
            var cacheKeyCounties = "ProgramIndicatorsReportCountiesPerProgram" + fiscalYear.Name + "_" + id.ToString();
            var cacheString = _cache.GetString(cacheKey);
            var cacheStringCounties = _cache.GetString(cacheKeyCounties);
            List<ReportingCountiesPerProgram> ReportingCountiesPerProgram;
            List<StrategicInitiativeIndicatorsViewModel> indicators;
            if (!string.IsNullOrEmpty(cacheString)){
                indicators = JsonConvert.DeserializeObject<List<StrategicInitiativeIndicatorsViewModel>>(cacheString);
                ReportingCountiesPerProgram = JsonConvert.DeserializeObject<List<ReportingCountiesPerProgram>>(cacheStringCounties);
            }else{
                indicators = new List<StrategicInitiativeIndicatorsViewModel>();




                var initiatives = await context.StrategicInitiative.Where(s => s.FiscalYear == fiscalYear)
                                        .Include( s => s.ProgramCategory)
                                        .Include( s => s.MajorPrograms)
                                        .OrderBy( s => s.order)
                                        .ToListAsync();
                var indicatorsThisFiscalYear = await context.ProgramIndicator.Where( i => i.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear).ToListAsync();
                
                var indicatorsPerProgram = indicatorsThisFiscalYear
                                                    .GroupBy( p => p.MajorProgramId )
                                                    .Select( g => new {
                                                        MajorProgramId = g.Key,
                                                        Indicators = g.Select( d => d )
                                                    }).ToList();

                ReportingCountiesPerProgram = new List<ReportingCountiesPerProgram>();

                foreach( var initiative in initiatives){
                    initiative.MajorPrograms = initiative.MajorPrograms.OrderBy( m => m.order).ToList();
                    var intv = new StrategicInitiativeIndicatorsViewModel();
                    intv.Title = initiative.ProgramCategory.ShortName + " " + initiative.Name;
                    intv.Code = initiative.PacCode;
                    intv.Indicators = new List<MajorProgramIndicatorsViewModel>();
                    foreach(var program in initiative.MajorPrograms){


                        if( id == 0 ){
                            var allIndicators = context.ProgramIndicatorValue
                                                    .Where( v => v.ProgramIndicator.MajorProgram == program )
                                                    .Include( v => v.PlanningUnit)
                                                    .ToList()
                                                    .GroupBy( v => v.PlanningUnit)
                                                    .Select( v => new {
                                                        Unit = v.Key,
                                                        Reported = v.Sum( l => l.Value )
                                                    });
                            var reported = allIndicators.Where( i => i.Reported != 0).Select( s => s.Unit ).ToList();
                            var reporting = new ReportingCountiesPerProgram();
                            reporting.Units = reported;
                            reporting.MajorProgram = program;
                            reporting.UnitsToString = string.Join(", ", reporting.Units.OrderBy(s => s.Name).Select(x => x.Name.Substring(0, x.Name.Length - 11)));
                            ReportingCountiesPerProgram.Add(reporting);
                        }

                        var programViewModel = new MajorProgramIndicatorsViewModel();
                        programViewModel.Title = program.Name;
                        programViewModel.Code = program.PacCode;
                        programViewModel.Indicators = new List<IndicatorViewModel>();
                        var indicatorsPerThisProgram = indicatorsPerProgram.Where( n => n.MajorProgramId == program.Id).FirstOrDefault();
                        if( indicatorsPerThisProgram != null ){
                            var i = 1;
                            foreach( var indctr in indicatorsPerThisProgram.Indicators.OrderBy( d => d.order)){
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
                                                                            v.PlanningUnitId == id
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
 
                _cache.SetString(cacheKey, JsonConvert.SerializeObject(indicators), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                    });
                _cache.SetString(cacheKeyCounties, JsonConvert.SerializeObject(ReportingCountiesPerProgram), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                    });
            }
            ViewData["fy"] = fiscalYear.Name;
            ViewData["id"] = id;
            ViewData["ReportingCountiesPerProgram"] = ReportingCountiesPerProgram;
            return View(indicators);
        }

        [HttpGet("countylist/{fy?}")]
        public async Task<IActionResult> Countylist(string fy="0"){

            List<PlanningUnit> counties;
            var cacheKey = "CountiesList";
            var cached = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cached)){
                counties = JsonConvert.DeserializeObject<List<PlanningUnit>>(cached);
            }else{
            
            
                counties = await this.context.PlanningUnit.
                                Where(c=>c.District != null).
                                OrderBy(c => c.Name).ToListAsync();
                
                counties = counties.Where( c => c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
                var serializedCounties = JsonConvert.SerializeObject(counties);
                _cache.SetString(cacheKey, serializedCounties, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                        });
            }
            ViewData["fy"] = fy;
            return View(counties);
        }



        [HttpGet("countyindicatorswithstories/{countyId}/{majorProgramId?}/{fy?}", Name="CountyIndicatorsStory")]
        public async Task<IActionResult> CountyIndicatorsStory(int countyId, int majorProgramId = 0, string fy="0"){
            FiscalYear fiscalYear;

            var model = new ProgramStoryViewModel();
            
            if(majorProgramId != 0){
                var program = await this.context.MajorProgram.Where( u => u.Id == majorProgramId)
                                        .Include( p => p.StrategicInitiative).ThenInclude( i => i .FiscalYear)
                                        .FirstOrDefaultAsync();
                if(program != null){
                    model.MajorProgram = program;
                    ViewData["MajorProgram"] = program;
                    var str = from stry in context.Story
                            let l = (from lim in stry.Revisions
                                    orderby lim.Created descending
                                    select lim).FirstOrDefault()
                            where l.MajorProgramId == majorProgramId select stry;


                    var stories = await str
                                            .Include(s => s.Revisions).ThenInclude( r => r.StoryImages).ThenInclude( i => i.UploadImage).ThenInclude( m => m.UploadFile)
                                            .Include(s => s.KersUser).ThenInclude( u => u.PersonalProfile)
                                            .Include(s => s.KersUser).ThenInclude( u => u.RprtngProfile).ThenInclude(u => u.PlanningUnit)
                                            //.Include( s => s.Revisions).ThenInclude( r => r.PlanOfWork).ThenInclude( p => p.Revisions)
                                            .Include( s => s.Revisions ).ThenInclude( r => r.StoryOutcome)
                                            .Include( s => s.Revisions).ThenInclude( r => r.MajorProgram)
                                            .ToListAsync();
                    model.Stories = this.storyViewModelList(stories);


                    var indicators = await this.context.ProgramIndicatorValue
                                                .Where( v => v.ProgramIndicator.MajorProgramId == program.Id && v.PlanningUnitId == countyId)
                                                .Include( v => v.ProgramIndicator)
                                                .ToListAsync();

                    var groupedIncicators = indicators
                                                .GroupBy( i => i.ProgramIndicator )
                                                .Select( s => new IndicatorViewModel{
                                                    Code = s.Key.order,
                                                    Description = s.Key.Question,
                                                    Amount = s.Sum(l => l.Value)
                                                }).ToList();
                    
                    MajorProgramIndicatorsViewModel indicatorsPerMajorProgram = new MajorProgramIndicatorsViewModel();


                    
                }
                fiscalYear = program.StrategicInitiative.FiscalYear;
            }else{
                fiscalYear = GetFYByName(fy);

                if(fiscalYear == null){
                    
                    return new StatusCodeResult(500);
                }
            }
            var programs = await this.context.MajorProgram.Where( p => p.StrategicInitiative.FiscalYear == fiscalYear).OrderBy(l => l.order).ToListAsync();
            model.MajorPrograms = programs;
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;

            ViewData["county"] = this.context.PlanningUnit.Find(countyId);


            ViewData["fy"] = fy;
            return View(model);
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
        protected int getCacheSpan(FiscalYear fiscalYear){
            int cacheDaysSpan = 350;
            var today = DateTime.Now;
            if(fiscalYear.Start < today && Math.Max( fiscalYear.End.Ticks, fiscalYear.ExtendedTo.Ticks) > today.Ticks){
                cacheDaysSpan = 3;
            }
            return cacheDaysSpan;
        }

        private List<StoryViewModel> storyViewModelList(List<Story> stories){
            List<StoryViewModel> modelStories = new List<StoryViewModel>();
            foreach( var story in stories ){
                var strViewModel = new StoryViewModel();
                var lastRevision = story.Revisions.OrderBy( r => r.Created).Last();
                strViewModel.Title = lastRevision.Title;
                strViewModel.Story = lastRevision.Story;
                strViewModel.KersUser = story.KersUser;
                strViewModel.MajorProgram = lastRevision.MajorProgram;
                strViewModel.PlanningUnit = story.KersUser.RprtngProfile.PlanningUnit;
                strViewModel.StoryOutcome = lastRevision.StoryOutcome;
                strViewModel.StoryId = story.Id;
                strViewModel.Updated = lastRevision.Created;
                if(lastRevision.PlanOfWork != null){
                    strViewModel.PlanOfWork = lastRevision.PlanOfWork.Revisions.OrderBy( p => p.Created ).Last();
                }
                var firstImage = lastRevision.StoryImages.OrderBy( i => i.Created).FirstOrDefault();
                if(firstImage != null){
                    strViewModel.ImageName = firstImage.UploadImage.UploadFile.Name;
                }else{
                    strViewModel.ImageName = "";
                }
                modelStories.Add(strViewModel);
            }
            return modelStories;
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

    public class ReportingCountiesPerProgram{
        public MajorProgram MajorProgram;
        public List<PlanningUnit> Units;
        public string UnitsToString;
    }



}