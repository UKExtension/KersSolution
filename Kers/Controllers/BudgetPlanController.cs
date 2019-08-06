using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
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


namespace Kers.Controllers
{
    [Route("api/[controller]")]
    public class BudgetPlanController : BaseController
    {

        KERScoreContext _context;
        public BudgetPlanController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo
            ):base(mainContext, context, userRepo){
                _context = context;
        }




/*************************************************/
//    Office Operations                          //
/*************************************************/

        [HttpGet]
        [Route("officeoperations/{onlyactive?}")]
        public async Task<IActionResult> OfficeOperations(Boolean onlyactive = false)
        {
            var list = await this.context.BudgetPlanOfficeOperation.ToListAsync();
            if(onlyactive) list = list.Where( o => o.Active).ToList();
            return new OkObjectResult(list.OrderBy( o => o.Order));
        }

        [HttpPost()]
        [Authorize]
        public IActionResult AddOfficeOperation( [FromBody] BudgetPlanOfficeOperation officeOperation){
            if(officeOperation != null){
                this.context.BudgetPlanOfficeOperation.Add(officeOperation);
                this.context.SaveChanges();
                return new OkObjectResult(officeOperation);
            }else{
                this.Log( officeOperation,"BudgetPlanOfficeOperation", "Error in adding Budget Plan Office Operation attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateOfficeOperation( int id, [FromBody] BudgetPlanOfficeOperation officeOperation){
            var operation = this.context.BudgetPlanOfficeOperation.Find(id);
            if(operation != null ){
                
                operation.Active = officeOperation.Active;
                operation.Name = officeOperation.Name;
                operation.Order = officeOperation.Order;
                this.context.SaveChanges();

                this.Log(officeOperation,"ExtensionEvent", "ExtensionEvent Updated.");
                
                return new OkObjectResult(officeOperation);
            }else{
                this.Log( officeOperation ,"BudgetPlanOfficeOperation", "Not Found BudgetPlanOfficeOperation in an update attempt.", "BudgetPlanOfficeOperation", "Error");
                return new StatusCodeResult(500);
            }
        }







/* 
        [HttpGet]
        [Route("range/{skip?}/{take?}/{order?}/{type?}")]
        public virtual IActionResult GetRange(int skip = 0, int take = 10, string order = "start", string type = "Training")
        {
            IQueryable<ExtensionEvent> query = _context.ExtensionEvent.Where( t => t.End != null && t.DiscriminatorValue == type);
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             
            query = query.Skip(skip).Take(take);
            query = query
                        .Include( e => e.Organizer)
                        .ThenInclude( o => o.PersonalProfile);
            var list = query.ToList();
            return new OkObjectResult(list);
        }


        [HttpGet("perPeriod/{start}/{end}/{order?}/{type?}")]
        [Authorize]
        public virtual IActionResult PerPeriod(DateTime start, DateTime end, string order = "start", string type = "Training" ){
            IQueryable<ExtensionEvent> query = _context.ExtensionEvent.Where( t => t.Start > start && t.Start < end && t.DiscriminatorValue == type);
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             

            var list = query.ToList();
            return new OkObjectResult(list);
        }


        [HttpPost()]
        [Authorize]
        public IActionResult AddExtensionEvent( [FromBody] object ExEvent){
            if(ExEvent != null){
                
                return new OkObjectResult(ExEvent);
            }else{
                this.Log( ExEvent,"ExtensionEvent", "Error in adding extension event attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateExtensionEvent( int id, [FromBody] ExtensionEvent ExEvent){
           


            if(ExEvent != null ){
                
                this.Log(ExEvent,"ExtensionEvent", "ExtensionEvent Updated.");
                
                return new OkObjectResult(ExEvent);
            }else{
                this.Log( ExEvent ,"ExtensionEvent", "Not Found ExtensionEvent in an update attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteExtensionEvent( int id ){
            var entity = context.ExtensionEvent.Find(id);
            
            
            if(entity != null){
                
                context.ExtensionEvent.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"ExtensionEvent", "ExtensionEvent Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ExtensionEvent", "Not Found ExtensionEvent in a delete attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }








 */




        

        
        public IActionResult Error()
        {
            return View();
        }


    }
    
}
