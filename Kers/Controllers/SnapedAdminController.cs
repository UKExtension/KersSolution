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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class SnapedAdminController : BaseController
    {
        
        IFiscalYearRepository fiscalRepo;
        const string LogType = "SnapEdAdmin";
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        public SnapedAdminController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IFiscalYearRepository fiscalRepo,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo
            ):base(mainContext, context, userRepo){
                this.fiscalRepo = fiscalRepo;
                this._cache = _cache;
                this.activityRepo = activityRepo;
        }

        [HttpPut("countybudget/{countyId}")]
        public IActionResult UpdateCountyBudget( int countyId, [FromBody] SnapCountyBudget budget){
           
           var currentFiscalYear = this.fiscalRepo.currentFiscalYear("snapEd");
           
            var budgetEntity = this.context.SnapCountyBudget.Where( b => b.PlanningUnitId == countyId && b.FiscalYear == currentFiscalYear).FirstOrDefault();
            if(budgetEntity == null){
                var newEntity = new SnapCountyBudget();
                newEntity.AnnualBudget = budget.AnnualBudget;
                newEntity.FiscalYear = budget.FiscalYear;
                newEntity.PlanningUnitId  = countyId;
                newEntity.Updated = DateTime.Now;
                newEntity.By = this.CurrentUser();
                this.context.Add(newEntity);
                this.Log( newEntity ,"SnapCountyBudget", "Created New Snap County Budget.", LogType, "Info");
                this.context.SaveChanges();
                return new OkObjectResult(newEntity);
            }else{
                budgetEntity.AnnualBudget = budget.AnnualBudget;
                budgetEntity.Updated = DateTime.Now;
                budgetEntity.By = this.CurrentUser();
                this.Log( budgetEntity ,"SnapCountyBudget", "Updated Snap County Budget.", LogType, "Info");
                this.context.SaveChanges();
                return new OkObjectResult(budgetEntity);
            }
        }


        [HttpPost("countyreimbursment/{countyId}")]
        [Authorize]
        public IActionResult AddACountyReimbursment(int countyId, [FromBody] SnapBudgetReimbursementsCounty budget){
            if(budget != null){
                var fiscalYear = this.fiscalRepo.currentFiscalYear("snapEd");
                budget.By = budget.UpdatedBy = this.CurrentUser();
                budget.ReimbursmentTime = DateTime.Now;
                budget.Updated = DateTime.Now;
                budget.PlanningUnitId = countyId;
                budget.FiscalYear = fiscalYear;
                this.context.Add(budget);
                this.Log( budget ,"SnapBudgetReimbursementsCounty", "Added Snap Budget Reimbursements County.", LogType, "Info");
                this.context.SaveChanges();
                return new OkObjectResult(budget);
            }else{
                this.Log( countyId ,"SnapBudgetReimbursementsCounty", "Error in adding SnapBudgetReimbursementsCounty attempt.", LogType, "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("countyreimbursment/{id}")]
        [Authorize]
        public IActionResult EditCountyReimbursment(int id, [FromBody] SnapBudgetReimbursementsCounty budget){

            var entity = this.context.SnapBudgetReimbursementsCounty.Find(id);

            if(entity != null && budget != null){  
                entity.UpdatedBy = this.CurrentUser();
                entity.Updated = DateTime.Now;
                entity.Notes = budget.Notes;
                entity.Amount = budget.Amount;
                this.Log( budget ,"SnapBudgetReimbursementsCounty", "Edited Snap Budget Reimbursements County.", LogType, "Info");
                this.context.SaveChanges();
                return new OkObjectResult(budget);
            }else{
                this.Log( id ,"SnapBudgetReimbursementsCounty", "Error in editing SnapBudgetReimbursementsCounty attempt.", LogType, "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpDelete("countyreimbursment/{id}")]
        public IActionResult DeleteCountyReimbursement( int id ){
            var entity = context.SnapBudgetReimbursementsCounty.Find(id);
            
            if(entity != null){
                
                context.SnapBudgetReimbursementsCounty.Remove(entity);
                context.SaveChanges();
                entity.UpdatedBy = this.CurrentUser();
                this.Log(entity, "SnapBudgetReimbursementsCounty", "SnapBudgetReimbursementsCounty Removed.", LogType, "Info");

                return new OkResult();
            }else{
                this.Log( id ,"SnapBudgetReimbursementsCounty", "Not Found SnapBudgetReimbursementsCounty in a delete attempt.", LogType, "Error");
                return new StatusCodeResult(500);
            }
        }




        [HttpGet("countyreimbursments/{countyId}/{fy?}")]
        [Authorize]
        public IActionResult CountyReimbursments(int countyId, string fy = "0"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalRepo.currentFiscalYear("snapEd");
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == "snapEd").FirstOrDefault();
            }
            var records = this.context.SnapBudgetReimbursementsCounty.Where( b => b.PlanningUnitId == countyId && b.FiscalYear == fiscalYear);

            return new OkObjectResult(records);
            
        }





        [HttpPost("assistantreimbursements/{userId}")]
        [Authorize]
        public IActionResult AddAssistantReimbursment(int userId, [FromBody] SnapBudgetReimbursementsNepAssistant budget){
            if(budget != null){
                var fiscalYear = this.fiscalRepo.currentFiscalYear("snapEd");
                budget.By = budget.UpdatedBy = this.CurrentUser();
                budget.ReimbursmentTime = DateTime.Now;
                budget.Updated = DateTime.Now;
                budget.ToId = userId;
                budget.FiscalYear = fiscalYear;
                this.context.Add(budget);
                this.Log( budget ,"SnapBudgetReimbursementsCounty", "Added Snap Budget Reimbursements County.", LogType, "Info");
                this.context.SaveChanges();
                return new OkObjectResult(budget);
            }else{
                this.Log( userId ,"SnapBudgetReimbursementsNepAssistant", "Error in adding SnapBudgetReimbursementsNepAssistant attempt.", LogType, "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("assistantreimbursements/{id}")]
        [Authorize]
        public IActionResult EditAssistantReimbursment(int id, [FromBody] SnapBudgetReimbursementsNepAssistant budget){

            var entity = this.context.SnapBudgetReimbursementsNepAssistant.Find(id);

            if(entity != null && budget != null){  
                entity.UpdatedBy = this.CurrentUser();
                entity.Updated = DateTime.Now;
                entity.Notes = budget.Notes;
                entity.Amount = budget.Amount;
                this.Log( budget ,"SnapBudgetReimbursementsNepAssistant", "Edited SnapBudgetReimbursementsNepAssistant.", LogType, "Info");
                this.context.SaveChanges();
                return new OkObjectResult(entity);
            }else{
                this.Log( id ,"SnapBudgetReimbursementsNepAssistant", "Error in editing SnapBudgetReimbursementsNepAssistant attempt.", LogType, "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpDelete("assistantreimbursements/{id}")]
        public IActionResult DeleteAssistantReimbursement( int id ){
            var entity = context.SnapBudgetReimbursementsNepAssistant.Find(id);
            
            if(entity != null){
                
                context.SnapBudgetReimbursementsNepAssistant.Remove(entity);
                context.SaveChanges();
                entity.UpdatedBy = this.CurrentUser();
                this.Log(entity, "SnapBudgetReimbursementsNepAssistant", "SnapBudgetReimbursementsNepAssistant Removed.", LogType, "Info");

                return new OkResult();
            }else{
                this.Log( id ,"SnapBudgetReimbursementsNepAssistant", "Not Found SnapBudgetReimbursementsNepAssistant in a delete attempt.", LogType, "Error");
                return new StatusCodeResult(500);
            }
        }




        [HttpGet("assistantreimbursements/{userId}/{fy?}")]
        [Authorize]
        public IActionResult AssistantReimbursments(int userId, string fy = "0"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalRepo.currentFiscalYear("snapEd");
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == "snapEd").FirstOrDefault();
            }
            var records = this.context.SnapBudgetReimbursementsNepAssistant.Where( b => b.ToId == userId && b.FiscalYear == fiscalYear);

            return new OkObjectResult(records);
            
        }















        [HttpGet("committed/{fy?}")]
        [Authorize]
        public IActionResult Committed(string fy = "0"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalRepo.currentFiscalYear("snapEd");
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == "snapEd").FirstOrDefault();
            }
            var hrs = 0;
            var committed = context.SnapEd_Commitment.
                                Where(
                                    e=>e.FiscalYear == fiscalYear
                                    &&
                                    e.SnapEd_ActivityType.Measurement == "Hour" );
            if(committed.Any()){
                hrs = committed.Sum( h => h.Amount) ?? 0;
            }
            return new OkObjectResult(hrs);
        }

        [HttpGet("reported/{fy?}")]
        [Authorize]
        public IActionResult Reported(string fy = "0"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalRepo.currentFiscalYear("snapEd");
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == "snapEd").FirstOrDefault();
            }

            var revs = activityRepo.LastActivityRevisionIds(fiscalYear, _cache);
            // Divide revs into batches as SQL server is having trouble to process more then several thousands at once
            var fyactivities = new List<ActivityRevision>();
            var batchCount = 10000;
            for(var i = 0; i <= revs.Count(); i += batchCount){
                var currentBatch = revs.Skip(i).Take(batchCount);
                fyactivities.AddRange(context.ActivityRevision.Where( r => currentBatch.Contains( r.Id )).ToList());
            }
            
            var snapEligible = fyactivities.Where( r => (r.SnapPolicy != null || r.SnapDirect != null || r.SnapIndirect != null || r.SnapAdmin ));
            var reported = snapEligible.Sum( h => Math.Floor(h.Hours));
            return new OkObjectResult(reported);
        }

        [HttpGet("assistants/{countyId?}")]
        public IActionResult Assistants(int countyId = 0){

            List<KersUser> assistants;
            if(countyId == 0){
                assistants = this.context.KersUser.
                                Where(c=> (
                                    c.Specialties.Where(s => s.Specialty.Name == "Expanded Food and Nutrition Education Program").Count() != 0 
                                    ||
                                    c.Specialties.Where(s => s.Specialty.Name == "Supplemental Nutrition Assistance Program Education").Count() != 0
                                    )
                                    &&
                                    c.ExtensionPosition.Code == "EXTPROGASSIST"
                                    &&
                                    c.RprtngProfile.enabled
                                ).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                                Include(u=>u.PersonalProfile).
                                OrderBy(c => c.PersonalProfile.FirstName).ToList();
            }else{
                 assistants = this.context.KersUser.
                                Where(c=> (c.Specialties.Where(s => s.Specialty.Name == "Expanded Food and Nutrition Education Program") != null 
                                    ||
                                    c.Specialties.Where(s => s.Specialty.Name == "Supplemental Nutrition Assistance Program Education") != null)
                                    &&
                                    c.RprtngProfile.PlanningUnitId == countyId
                                    &&
                                    c.RprtngProfile.enabled
                                ).
                                Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                                Include(u=>u.PersonalProfile).
                                OrderBy(c => c.PersonalProfile.FirstName).ToList();
            }
            
            return new OkObjectResult(assistants);
        }

        [HttpGet("countybudget/{countyId?}")]
        public IActionResult CountyBudget(int countyId = 0){
            float bg = 0;
            var budget = this.context.SnapCountyBudget.Where( b => b.PlanningUnitId == countyId).FirstOrDefault();
            if(budget == null){
                var defaultBudget = this.context.SnapBudgetAllowance.Find(2);
                bg = defaultBudget.AnnualBudget;
            }else{
                bg = budget.AnnualBudget;
            }
            return new OkObjectResult(bg);
        }
        [HttpGet("assistantbudget")]
        public IActionResult AssistantBudget(){
            var defaultBudget = this.context.SnapBudgetAllowance.Find(1).AnnualBudget;
            return new OkObjectResult(defaultBudget);
        }

    }
}