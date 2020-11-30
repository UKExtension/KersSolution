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

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class MileageController : BaseController
    {


        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IExpenseRepository expenseRepo;
        public MileageController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    IExpenseRepository expenseRepo,
                    IFiscalYearRepository fiscalYearRepo
            ):base(mainContext, context, userRepo){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           this.expenseRepo = expenseRepo;
        }

        [HttpGet("numb")]
        [Authorize]
        public IActionResult GetNumb(){
            
            var lastExpenses = context.Expense.
                                Where(e=>e.KersUser == this.CurrentUser());
            
            return new OkObjectResult(lastExpenses.Count());
        }




        

        [HttpGet("byrevid/{Id}")]
        [Authorize]
        public IActionResult ByRevId(int Id){
            var revFull = this.context.ExpenseRevision
                                .Where( r => r.Id == Id)
                                .Include( r => r.Segments ).ThenInclude( s => s.Location).ThenInclude( l => l.Address)
                                .Include( r => r.StartingLocation).ThenInclude( s => s.Address)
                                .FirstOrDefault();
            return new OkObjectResult(revFull);
        }



        [HttpGet("latest/{skip?}/{amount?}/{userId?}")]
        [Authorize]
        public IActionResult Get(int skip = 0, int amount = 10, int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var lastExpenses = context.Expense.
                                Where(e=>e.KersUser.Id == userId).
                                Include(e=>e.Revisions).
                                OrderByDescending(e=>e.ExpenseDate).
                                Skip(skip).
                                Take(amount);
            var revs = new List<ExpenseRevision>();
            if( lastExpenses != null){
                foreach(var expense in lastExpenses){
                    if(expense.Revisions.Count != 0){
                        var lastRev = expense.Revisions.OrderBy(r=>r.Created).Last();
                        var revFull = this.context.ExpenseRevision
                                            .Where( r => r.Id == lastRev.Id)
                                            .Include( r => r.Segments ).ThenInclude( s => s.Location).ThenInclude( l => l.Address)
                                            .Include( r => r.StartingLocation).ThenInclude( s => s.Address)
                                            .FirstOrDefault();
                        revs.Add( revFull );
                    }
                }
            }
            return new OkObjectResult(revs);
        }

	
        [HttpGet("permonth/{year}/{month}/{userId?}/{orderBy?}")]
        [Authorize]
        public IActionResult PerMonth(int year, int month, int userId=0, string orderBy = "desc"){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = this.context.KersUser.Where(u => u.Id == userId).FirstOrDefault();
            }
            var mileages = expenseRepo.MileagePerMonth(user, year, month).OrderBy( r => r.ExpenseDate);
            var revs = mileages.Select( m => m.LastRevision);
            return new OkObjectResult(revs);
        }

        [HttpGet("source/{id}")]
        [Authorize]
        public IActionResult SourceById(int id){
            var srce = this.context.ExpenseFundingSource.Find(id);
            return new OkObjectResult(srce);
        }
        [HttpGet("sources")]
        [Authorize]
        public IActionResult Sources(){
            var srce = this.context.ExpenseFundingSource.Where( s => s.MileageAvailable);
            return new OkObjectResult(srce);
        }
        
        [HttpGet("category/{id}")]
        [Authorize]
        public IActionResult CategoryById(int id){
            var srce = this.context.ProgramCategory.Find(id);
            return new OkObjectResult(srce);
        }
        [HttpGet("categories")]
        [Authorize]
        public IActionResult Categories(){
            var srce = this.context.ProgramCategory;
            return new OkObjectResult(srce);
        }



        [HttpPost()]
        [Authorize]
        public IActionResult AddExpense( [FromBody] ExpenseRevision expense){
            if(expense != null){
                var user = this.CurrentUser();
                var exp = new Expense();
                exp.KersUser = user;
                exp.Created = DateTime.Now;
                exp.Updated = DateTime.Now;
                exp.ExpenseDate = expense.ExpenseDate;
                exp.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                expense.Created = DateTime.Now;
                exp.Revisions = new List<ExpenseRevision>();
                exp.Revisions.Add(expense);
                context.Add(exp); 
                context.SaveChanges(); 
                this.Log(expense,"MileageReveision", "Mileage Added.", "MileageReveision", "Created Mileage Record");
                exp.LastRevisionId = expense.Id;
                context.SaveChanges();
                return new OkObjectResult(expense);
            }else{
                this.Log( expense ,"MileageReveision", "Error in adding expense attempt.", "Expense", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateExpense( int id, [FromBody] ExpenseRevision expense){
            var entity = context.ExpenseRevision.Find(id);
            var exEntity = context.Expense.Find(entity.ExpenseId);
            if(expense != null && exEntity != null){
                expense.Created = DateTime.Now;
                exEntity.Revisions.Add(expense);
                exEntity.ExpenseDate = expense.ExpenseDate;
                context.SaveChanges();
                exEntity.LastRevisionId = expense.Id;
                this.Log(expense,"ExpenseRevision", "Expense Updated.");
                return new OkObjectResult(expense);
            }else{
                this.Log( expense ,"ExpenseRevision", "Not Found Expense in update attempt.", "Expense", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteExpense( int id ){
            var entity = context.ExpenseRevision.Find(id);
            var exEntity = context.Expense.Find(entity.ExpenseId);
            
            if(exEntity != null){
                exEntity.LastRevisionId = 0;
                context.SaveChanges();
                context.Expense.Remove(exEntity);
                context.SaveChanges();
                
                this.Log(entity,"ExpenseRevision", "Expense Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ExpenseRevision", "Not Found Expense in delete attempt.", "Expense", "Error");
                return new StatusCodeResult(500);
            }
        }




    }
}