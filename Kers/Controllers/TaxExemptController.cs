using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Contexts;
using Kers.Models.Entities.UKCAReporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class TaxExemptController:BaseController
    {
        KERScoreContext _context;
        KERSmainContext _mainContext;
        KERSreportingContext _reportingContext;
        IKersUserRepository _userRepo;
        IMessageRepository messageRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IWebHostEnvironment environment;
        public TaxExemptController( 
                    KERSmainContext mainContext,
                    KERSreportingContext _reportingContext,
                    KERScoreContext context,
                    IMessageRepository messageRepo,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    ITrainingRepository trainingRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IWebHostEnvironment env
            ):base(mainContext, context, userRepo){

           this._context = context;
           this._mainContext = mainContext;
           this._reportingContext = _reportingContext;
           this.messageRepo = messageRepo;
           this._userRepo = userRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           this.environment = env;
        }


        [HttpGet("financialyears")]
        public async Task<IActionResult> FinancialYears( ){
            var yrs = this._context.TaxExemptFinancialYear.OrderBy(r => r.Order);
            return new OkObjectResult(await yrs.ToListAsync());
        }
        [HttpGet("programcategories")]
        public async Task<IActionResult> ProgramCategores( ){
            var yrs = this._context.TaxExemptProgramCategory.OrderBy(r => r.Order);
            return new OkObjectResult(await yrs.ToListAsync());
        }
        [HttpGet("fundshandled")]
        public async Task<IActionResult> FundsHandled( ){
            var yrs = this._context.TaxExemptFundsHandled.Where(a => a.Active).OrderBy(r => r.Order);
            return new OkObjectResult(await yrs.ToListAsync());
        }
        [HttpGet("exemptslist")]
        public async Task<IActionResult> ExemptsList( ){
            var yrs = this._context.TaxExempt.Where(a => true)
                                .Include( a => a.Handled)
                                .Include( a => a.TaxExemptProgramCategories).ThenInclude( c => c.TaxExemptProgramCategory)
                                .Include( a => a.TaxExemptFinancialYear)
                                .Include( a => a.Areas).ThenInclude( r => r.Unit)
                                .OrderByDescending(r => r.Created);
            return new OkObjectResult(await yrs.ToListAsync());
        }

        [HttpPost()]
        [Authorize]
        public IActionResult AddExempt( [FromBody] TaxExempt exempt){
            
            if(exempt != null){

                
                var user = this.CurrentUser();


                exempt.Created = DateTime.Now;
                exempt.Updated = DateTime.Now;
                exempt.By = user;
                exempt.UnitId = user.RprtngProfile.PlanningUnitId;

                this._context.Add(exempt);

                this.Log(exempt,"TaxExempt", "TaxExempt record Added.");
                context.SaveChanges();
                return new OkObjectResult(exempt);
            }else{
                this.Log( exempt ,"TaxExempt", "Not Found TaxExempt in an adding attempt.", "Success Story", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateExempt( int id, [FromBody] TaxExempt exempt){
            var entity = context.TaxExempt.Where( e => e.Id == id)
                            .Include( a => a.TaxExemptProgramCategories)
                            .Include( a => a.Areas)
                            .FirstOrDefault();
            
            if(exempt != null && entity != null){
                entity.Updated = DateTime.Now;
                entity.Name = exempt.Name;
                entity.Ein = exempt.Ein;
                entity.BankName = exempt.BankName;
                entity.BankAccountName = exempt.BankAccountName;
                entity.DonorsReceivedAck = exempt.DonorsReceivedAck;
                entity.AnnBudget = exempt.AnnBudget;
                entity.AnnFinancialRpt = exempt.AnnFinancialRpt;
                entity.AnnAuditRpt = exempt.AnnAuditRpt;
                entity.AnnInvRpt = exempt.AnnInvRpt;
                entity.TaxExemptFinancialYearId = exempt.TaxExemptFinancialYearId;
                entity.HandledId = exempt.HandledId;
                entity.DistrictName = exempt.DistrictName;
                entity.DistrictEin = exempt.DistrictEin;
                entity.OrganizationName = exempt.OrganizationName;
                entity.OrganizationResidesId = exempt.OrganizationResidesId;
                entity.OrganizationLetterDate = exempt.OrganizationLetterDate;
                entity.OrganizationSignedDate = exempt.OrganizationSignedDate;
                entity.OrganizationAppropriate = exempt.OrganizationAppropriate;
                entity.Areas = exempt.Areas;
                entity.TaxExemptProgramCategories = exempt.TaxExemptProgramCategories;
                context.SaveChanges();
                var newEntity = context.TaxExempt.Where( e => e.Id == id)
                            .Include( a => a.TaxExemptProgramCategories).ThenInclude(c => c.TaxExemptProgramCategory)
                            .Include( a => a.Areas).ThenInclude( r => r.Unit)
                            .Include( e => e.TaxExemptFinancialYear)
                            .FirstOrDefault();
                this.Log( newEntity ,"TaxExempt", "Tax Exempt Updated.");
                return new OkObjectResult(newEntity);
            }else{
                this.Log( exempt ,"TaxExempt", "Not Found Tax Exempt in an update attempt.", "Tax Exempt", "Error");
                return new StatusCodeResult(500);
            }
        }


/* 
[HttpPost()]
        [Authorize]
        public IActionResult AddStory( [FromBody] StoryRevision story){
            
            if(story != null){

                
                var user = this.CurrentUser();
                var str = new Story();
                str.KersUser = user;
                str.Created = DateTime.Now;
                str.Updated = DateTime.Now;
                str.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                story.Created = DateTime.Now;
                story.MajorProgram = this.context.MajorProgram.Where( m => m.Id == story.MajorProgramId)
                                            .Include(m => m.StrategicInitiative ).ThenInclude( i => i.FiscalYear)
                                            .FirstOrDefault();
                str.MajorProgramId = story.MajorProgramId;
                str.Revisions = new List<StoryRevision>();
                str.Revisions.Add(story);
                str.HasImages = story.StoryImages.Count > 0;
                context.Add(str); 
                this.Log(str,"Story", "Success Story Added.");
                context.SaveChanges();
                return new OkObjectResult(story);
            }else{
                this.Log( story ,"StoryRevision", "Not Found Success Story in an adding attempt.", "Success Story", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateStory( int id, [FromBody] StoryRevision story){
            var entity = context.StoryRevision.Find(id);
            var stEntity = context.Story.Find(entity.StoryId);

            if(story != null && stEntity != null){
                story.Created = DateTime.Now;
                story.MajorProgram = this.context.MajorProgram.Where( m => m.Id == story.MajorProgramId)
                                            .Include(m => m.StrategicInitiative ).ThenInclude( i => i.FiscalYear)
                                            .FirstOrDefault();
                stEntity.MajorProgramId = story.MajorProgramId;
                stEntity.HasImages = story.StoryImages.Count > 0;
                stEntity.Revisions.Add(story);
                stEntity.Updated = DateTime.Now;
                context.SaveChanges();
                this.Log( story ,"StoryRevision", "Success Story Updated.");
                return new OkObjectResult(story);
            }else{
                this.Log( story ,"StoryRevision", "Not Found Success Story in an update attempt.", "Success Story", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStory( int id ){
            var entity = context.StoryRevision.Find(id);
            var acEntity = context.Story.Where(a => a.Id == entity.StoryId).
                                Include(e=>e.Revisions).
                                FirstOrDefault();
            
            if(acEntity != null){
                
                context.Story.Remove(acEntity);
                context.SaveChanges();
                
                this.Log(acEntity,"SuccessStory", "Success Story Deleted.");

                return new OkResult();
            }else{
                this.Log( id ,"StoryRevision", "Not Found Success Story in a delete attempt.", "Success Story", "Error");
                return new StatusCodeResult(500);
            }
        }

 */

    

    }


}