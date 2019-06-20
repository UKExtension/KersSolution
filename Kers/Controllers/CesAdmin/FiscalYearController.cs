using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;
using Kers.Models.Contexts;

namespace Kers.Controllers.Admin
{

    [Route("api/[controller]")]
    public class FiscalYearController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepository;
        public FiscalYearController( 
              KERScoreContext context ,
              IFiscalYearRepository fiscalYearRepository   
            ){
           this.context = context;
           this.fiscalYearRepository = fiscalYearRepository;
        }

        [HttpGet()]
        public IActionResult Get(){
            return NotFound(new {Error = "not found"});
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id){
            var year = context.FiscalYear.Find(id);
            return new OkObjectResult(year);
        }

        [HttpGet("current/{type?}/{extendedto?}/{availableat?}")]
        public IActionResult Get(string type = "serviceLog", Boolean extendedto = false, Boolean availableat = false){
            var year = fiscalYearRepository.currentFiscalYear(type, extendedto, availableat);
            return new OkObjectResult(year);
        }
        [HttpGet("forDate/{dt}/{type?}/{extendedto?}/{availableat?}")]
        public IActionResult ForDate(DateTime dt, string type = "serviceLog", Boolean extendedto = false, Boolean availableat = false){
            var year = fiscalYearRepository.byDate(dt, type,extendedto, availableat);
            /* 
            dt = new DateTime(dt.Year, dt.Month, dt.Day, 12, 0, 0);
            var year = await context.FiscalYear
                        .Where( f => 
                                f.Type == type
                                &&
                                f.Start <= dt
                                &&  
                                f.End >= dt    
                            )
                        .FirstOrDefaultAsync();
            if( year == null ){
                year = fiscalYearRepository.currentFiscalYear(type);
            }
 */
            if(year == null){
                return new StatusCodeResult(500);
            }           
            return new OkObjectResult(year);
        }

        [HttpGet("next/{type?}/{extendedto?}/{availableat?}")]
        public IActionResult Next(string type = "serviceLog", Boolean extendedto = false, Boolean availableat = false){
            var year = fiscalYearRepository.nextFiscalYear(type, extendedto, availableat);
            return new OkObjectResult(year);
        }

        [HttpGet("previous/{type?}/{extendedto?}/{availableat?}")]
        public IActionResult Previous(string type = "serviceLog", Boolean extendedto = false, Boolean availableat = false){
            var year = fiscalYearRepository.previoiusFiscalYear(type);
            return new OkObjectResult(year);
        }

        [HttpGet("bytype/{type?}")]
        public async Task<IActionResult> ByType(string type = "serviceLog"){
            var year = await context.FiscalYear.Where( f => f.Type == type).OrderBy(f => f.Start).ToListAsync();
            return new OkObjectResult(year);
        }

        [HttpGet("All")]
        public IActionResult All(){
            var all = context.FiscalYear.OrderBy(f => f.Start);
            return new OkObjectResult(all);
        }

        [HttpPost("")]
        [Authorize]
        public IActionResult AddFiscalYear( [FromBody] FiscalYear year){
            if(year != null){
                this.context.FiscalYear.Add(year);
                this.context.SaveChanges();
                return new OkObjectResult(year);
            }else{
                return new StatusCodeResult(500);
            }
        }


        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateSection( int id, [FromBody] FiscalYear year){
            var entity = context.FiscalYear.Find(id);
            if(entity != null && year != null){
                entity.Name = year.Name;
                entity.Start = year.Start;
                entity.End = year.End;
                entity.AvailableAt = year.AvailableAt;
                entity.ExtendedTo = year.ExtendedTo;
                entity.Type = year.Type;
                this.context.SaveChanges();
                return new OkObjectResult(entity);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete( int id){
            var entity = context.FiscalYear.Find(id);
            if(entity != null){             
                this.context.Remove(entity);
                this.context.SaveChanges();
                return new OkResult();
            }else{
                return new StatusCodeResult(500);
            }
        }

    }
}