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
    public class ExpenseController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        IExpenseRepository expenseRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IAffirmativeActionPlanRevisionRepository repo;
        public ExpenseController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IExpenseRepository expenseRepo,
                    ILogRepository logRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IAffirmativeActionPlanRevisionRepository repo
            ){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.expenseRepo = expenseRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           this.repo = repo;
        }


        [HttpGet("numb")]
        [Authorize]
        public IActionResult GetNumb(){
            
            var lastExpenses = context.Expense.
                                Where(e=>e.KersUser == this.CurrentUser());
            
            return new OkObjectResult(lastExpenses.Count());
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
                        revs.Add( expense.Revisions.OrderBy(r=>r.Created).Last() );
                    }
                }
            }
            return new OkObjectResult(revs);
        }



        [HttpGet("perPeriod/{start}/{end}/{userid?}")]
        [Authorize]
        public IActionResult PerPeriod(DateTime start, DateTime end, int userid = 0 ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            end = end.AddDays(1);
            var lastExpenses = context.Expense.
                                Where(e=>e.KersUser.Id == userid && e.ExpenseDate > start && e.ExpenseDate < end).
                                Include(e=>e.Revisions).ThenInclude( r => r.FundingSourceNonMileage).
                                Include(e=>e.Revisions).ThenInclude( r => r.FundingSourceMileage).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateBreakfast).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateLunch).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateDinner).
                                OrderBy(e=>e.ExpenseDate);
                              
            var revs = new List<ExpenseRevision>();
            if( lastExpenses != null){
                foreach(var expense in lastExpenses){
                    if(expense.Revisions.Count != 0){
                        var lastRevision = expense.Revisions.OrderBy(r=>r.Created).Last();
                        if( lastRevision.ProgramCategoryId != 0){
                            var category = context.ProgramCategory.Find(lastRevision.ProgramCategoryId);
                            if(category != null){
                                lastRevision.ProgramCategory = category;
                            }
                        }
                        revs.Add( lastRevision );
                    }
                }
            }

            return new OkObjectResult(revs);
        }

        [HttpGet("perPeriodLite/{start}/{end}/{userid?}")]
        [Authorize]
        public IActionResult PerPeriodLite(DateTime start, DateTime end, int userid = 0 ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            end = end.AddDays(1);
            var lastExpenses = context.Expense.
                                Where(e=>e.KersUser.Id == userid && e.ExpenseDate > start && e.ExpenseDate < end);
                              
            var revs = new List<ExpenseRevision>();
            foreach( var last in lastExpenses){
                revs.Add(context.ExpenseRevision.Where(e => e.ExpenseId == last.Id).OrderBy(r => r.Created).Last());
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
            return new OkObjectResult(expenseRepo.PerMonth(user, year, month, orderBy));
        }


        [HttpGet("fysummaries/{userId?}/{fiscalYear?}")]
        [Authorize]
        public IActionResult FySummaries(int userId=0, string fiscalYear = ""){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = this.context.KersUser.Where(u => u.Id == userId).Include(u => u.RprtngProfile).FirstOrDefault();
            }
            FiscalYear fYear;
            if(fiscalYear == ""){
                fYear = fiscalYearRepo.currentFiscalYear("serviceLog");
            }else{
                fYear = fiscalYearRepo.GetSingle( y => y.Name == fiscalYear);
                if(fYear == null){
                    this.Log( user ,"ExpenseRevision", "Fiscal Year not found in generating summaries per year.", "Expense", "Error");
                    return new StatusCodeResult(500);
                }
            }
            return new OkObjectResult(expenseRepo.SummariesPerFiscalYear( user, fYear));
        }


        //public List<ExpenseSummary> SummariesPerFiscalYear(KersUser user, FiscalYear fiscalYear)


        [HttpGet("months/{year}/{userId?}")]
        [Authorize]
        public IActionResult GetMonths(int year, int userId=0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var lastExpenses = context.Expense.
                                Where(e=>e.KersUser.Id == userId && e.ExpenseDate.Year == year).
                                GroupBy(e => new {
                                    Month = e.ExpenseDate.Month
                                }).
                                Select(c => new {
                                    Month = c.Key.Month
                                }).
                                OrderByDescending(e => e.Month);
            return new OkObjectResult(lastExpenses);
        }



        [HttpGet("years/{userId?}")]
        [Authorize]
        public IActionResult GetYears(int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var years = context.Expense.
                                Where(e=>e.KersUser.Id == userId).
                                GroupBy(e => new {
                                    Year = e.ExpenseDate.Year
                                }).
                                Select(c => new {
                                    Year = c.Key.Year
                                }).
                                OrderByDescending(e => e.Year);
            return new OkObjectResult(years);
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
                this.Log(expense,"ExpenseRevision", "Expense Added.");
                context.SaveChanges();
                return new OkObjectResult(expense);
            }else{
                this.Log( expense ,"ExpenseRevision", "Error in adding expense attempt.", "Expense", "Error");
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
                
                context.Expense.Remove(exEntity);
                context.SaveChanges();
                
                this.Log(entity,"ExpenseRevision", "Expense Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ExpenseRevision", "Not Found Expense in delete attempt.", "Expense", "Error");
                return new StatusCodeResult(500);
            }
        }




        [HttpGet("FundingSource")]
        public IActionResult FundingSource(){
            var fs = this.context.ExpenseFundingSource.OrderBy(f => f.Order).ToList();
            return new OkObjectResult(fs);
        }

        [HttpGet("MealRate/{userId?}")]
        [Authorize]
        public IActionResult MealRate(int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            KersUser fullUser = this.context.KersUser.Where(r=>r.Id == userId).Include(u=>u.RprtngProfile).FirstOrDefault();
            var rates = this.context.ExpenseMealRate.Where(e => e.InstitutionId == fullUser.RprtngProfile.InstitutionId);
            return new OkObjectResult(rates);
        }
        [HttpGet("mileagerate/{month?}/{year?}/{userId?}")]
        [Authorize]
        public IActionResult MileageRate(int month = 0, int year = 0, int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            KersUser fullUser = this.context.KersUser.Where(r=>r.Id == userId).Include(u=>u.RprtngProfile).FirstOrDefault();
            return new OkObjectResult(expenseRepo.MileageRate(fullUser, year, month));
        }


        private void Log(   object obj, 
                            string objectType = "ExpenseRevision",
                            string description = "Submitted Expense Revision", 
                            string type = "Expense",
                            string level = "Information"
                        ){
                             
            var log = new Log();
            log.Level = level;
            log.Time = DateTime.Now;
            log.User = this.CurrentUser();
            log.ObjectType = objectType;
            log.Object = JsonConvert.SerializeObject(obj,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
            log.Agent = Request.Headers["User-Agent"].ToString();
            log.Ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            log.Description = description;
            log.Type = type;
            this.context.Log.Add(log);
            context.SaveChanges();

        }

        private KersUser userByProfileId(string profileId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.personID == profileId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = userRepo.findByProfileID(profile.Id);
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }


        private KersUser userByLinkBlueId(string linkBlueId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = this.context.KersUser.
                            Where( u => u.classicReportingProfileId == profile.Id).
                            Include(u => u.RprtngProfile).
                            Include(u => u.ExtensionPosition).
                            FirstOrDefault();
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }

        private PlanningUnit CurrentPlanningUnit(){
            var u = this.CurrentUserId();
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == u).
                            FirstOrDefault();
            return  this.context.PlanningUnit.
                    Where( p=>p.Code == profile.planningUnitID).
                    FirstOrDefault();
        }
        private zEmpRptProfile profileByUser(KersUser user){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.Id == user.classicReportingProfileId).
                            FirstOrDefault();
            
            return profile;
        }
        private KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }
        private zEmpRptProfile CurrentProfile(){
            var u = this.CurrentUserId();
            return mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == u).
                            FirstOrDefault();
        }
        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }



    }
}