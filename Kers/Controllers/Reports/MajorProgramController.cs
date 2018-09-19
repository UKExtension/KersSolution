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
        [Route("program/{id}/{fy?}")]
        public async Task<IActionResult> Program(int id, string fy = "0")
        {
            var Program = await context.MajorProgram
                                .Where( m => m.Id == id)
                                .Include( m => m.StrategicInitiative).ThenInclude( i => i.FiscalYear)
                                .FirstOrDefaultAsync();
            ViewData["Indicators"] = await initiativeRepo.IndicatorSumPerMajorProgram( id );
            FiscalYear fiscalYear = Program.StrategicInitiative.FiscalYear;

            if( fy == "0"){
                ViewData["fy"] = fiscalYear.Name;
            }else{ 

                if( fy != fiscalYear.Name ){
                    var toFY = GetFYByName(fy);
                    if( toFY != null ){
                        var programWithTheSameName = await context.MajorProgram.Where( p => p.Name == Program.Name && p.StrategicInitiative.FiscalYear == toFY ).FirstOrDefaultAsync();
                        if(programWithTheSameName != null ){
                            return RedirectToAction("Program", new {id=programWithTheSameName.Id});
                        }
                        return RedirectToAction("Index", "Reports", new {fy=toFY.Name});
                    }else{
                        return RedirectToAction("Index", "Reports", new {fy=fy});
                    }
                }
            }

            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            var fiscalYearSummaries = await contactRepo.GetPerPeriodSummaries(fiscalYear.Start, fiscalYear.End, 5, id);
            float[] SummariesArray = fiscalYearSummaries.ToArray();

            ViewData["totalHours"] = SummariesArray[0];
            ViewData["totalContacts"] = (int) SummariesArray[1];
            ViewData["totalMultistate"] = SummariesArray[2];
            ViewData["totalActivities"] = (int) SummariesArray[3];



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