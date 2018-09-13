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
    public class MajorProgramController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepository;
        IContactRepository contactRepo;
        IInitiativeRepository initiativeRepo;
        private FiscalYear currentFiscalYear;
        public MajorProgramController( 
                    KERScoreContext context,
                    IFiscalYearRepository fiscalYearRepository,
                    IContactRepository contactRepo,
                    IInitiativeRepository initiativeRepo
            ){
           this.context = context;
           this.fiscalYearRepository = fiscalYearRepository;
           this.currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");
           this.contactRepo = contactRepo;
           this.initiativeRepo = initiativeRepo;
        }



        [HttpGet]
        [Route("{fy?}")]
        public async Task<IActionResult> Index(string fy = "0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);
            

            ViewData["fy"] = fiscalYear.Name;
            var initiatives = await context.StrategicInitiative.Where( i => i.FiscalYear == fiscalYear)
                                .Include( i => i.MajorPrograms )
                                .ToListAsync();


            return View(initiatives);
        }
        [HttpGet]
        [Route("program/{id}")]
        public async Task<IActionResult> Program(int id)
        {
            
            var Program = await context.MajorProgram
                                .Where( m => m.Id == id)
                                .Include( m => m.StrategicInitiative).ThenInclude( i => i.FiscalYear)
                                .FirstOrDefaultAsync();
            var lastMonth = DateTime.Now.AddMonths(-1);
            var StatsLastMonth = await contactRepo.StatsPerMonth(lastMonth.Year,lastMonth.Month,0, id);
            ViewData["StatsLastMonth"] = StatsLastMonth;
            DateTime ago = DateTime.Now.AddMonths(-2);
            var StathsTwoMonthsAgo = await contactRepo.StatsPerMonth( ago.Year, ago.Month, 0, id );
            ViewData["StathsTwoMonthsAgo"] = StathsTwoMonthsAgo;
            ViewData["Indicators"] = await initiativeRepo.IndicatorSumPerMajorProgram( id );
            var fiscalYear = Program.StrategicInitiative.FiscalYear;
            ViewData["fy"] = fiscalYear.Name;

            return View( Program );
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