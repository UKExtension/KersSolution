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
        public async Task<IActionResult> Index(int fy = 0)
        {
            FiscalYear fiscalYear;
            if( fy == 0 ){
                fiscalYear = currentFiscalYear;
            }else{
                fiscalYear = await context.FiscalYear.FindAsync( fy );
                if( fiscalYear == null ){
                    this.fiscalYearRepository.previoiusFiscalYear("serviceLog");
                }
            }

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
            
            var Program = context.MajorProgram.FindAsync(id);
            var lastMonth = DateTime.Now.AddMonths(-1);
            var StatsLastMonth = await contactRepo.StatsPerMonth(lastMonth.Year,lastMonth.Month,0, id);
            ViewData["StatsLastMonth"] = StatsLastMonth;
            DateTime ago = DateTime.Now.AddMonths(-2);
            var StathsTwoMonthsAgo = await contactRepo.StatsPerMonth( ago.Year, ago.Month, 0, id );
            ViewData["StathsTwoMonthsAgo"] = StathsTwoMonthsAgo;
            ViewData["Indicators"] = await initiativeRepo.IndicatorSumPerMajorProgram( id );


            return View(await Program);
        }


        


    }
}