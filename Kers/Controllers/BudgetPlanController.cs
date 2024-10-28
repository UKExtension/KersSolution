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
using Microsoft.Extensions.Caching.Memory;


namespace Kers.Controllers
{
    [Route("api/[controller]")]
    public class BudgetPlanController : BaseController
    {

        KERScoreContext _context;
        public BudgetPlanController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IMemoryCache memoryCache
            ):base(mainContext, context, userRepo, memoryCache){
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


        [HttpPost("budget")]
        [Authorize]
        public IActionResult AddBudgetPlan( [FromBody] BudgetPlan plan){
            if(plan != null){
                this.context.BudgetPlan.Add(plan);
                //this.context.SaveChanges();
                this.Log(plan,"BudgetPlan", "BudgetPlan Added.");
                return new OkObjectResult(plan);
            }else{
                this.Log( plan,"BudgetPlanOfficeOperation", "Error in adding Budget Plan Office Operation attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("budget/{id}")]
        [Authorize]
        public IActionResult UpdateOfficeOperation( int id, [FromBody] BudgetPlanRevision plan){
            var pln = this.context.BudgetPlanRevision.Find(id);
            if(pln != null ){
                
                
                this.context.SaveChanges();

                this.Log(plan,"BudgetPlan", "BudgetPlan Updated.");
                
                return new OkObjectResult(pln);
            }else{
                this.Log( plan ,"BudgetPlan", "Not Found BudgetPlan in an update attempt.", "BudgetPlanOfficeOperation", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpPost()]
        [Authorize]
        public IActionResult AddOfficeOperation( [FromBody] BudgetPlanOfficeOperation officeOperation){
            if(officeOperation != null){
                this.context.BudgetPlanOfficeOperation.Add(officeOperation);
                this.context.SaveChanges();
                this.Log(officeOperation,"BudgetPlanOfficeOperation", "BudgetPlanOfficeOperation Added.");
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

                this.Log(officeOperation,"BudgetPlanOfficeOperation", "BudgetPlanOfficeOperation Updated.");
                
                return new OkObjectResult(officeOperation);
            }else{
                this.Log( officeOperation ,"BudgetPlanOfficeOperation", "Not Found BudgetPlanOfficeOperation in an update attempt.", "BudgetPlanOfficeOperation", "Error");
                return new StatusCodeResult(500);
            }
        }


    }
    
}
