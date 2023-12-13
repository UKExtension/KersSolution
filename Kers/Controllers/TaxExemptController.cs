using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
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
        [HttpGet("exemptslist/{countyId?}")]
        public async Task<IActionResult> ExemptsList(int countyId = 0 ){
            if(countyId == 0){
                var user = this.CurrentUser();
                countyId = user.RprtngProfile.PlanningUnitId;
            }
            var yrs = this._context.TaxExempt.Where(a => a.UnitId == countyId)
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
        [Authorize]
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
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteExempt( int id ){
            var entity = context.TaxExempt.Find(id);
            if(entity != null){
                context.TaxExempt.Remove(entity);
                context.SaveChanges();
                this.Log(entity,"TaxExempt", "Success, Tax Exempt Deleted.");
                return new OkResult();
            }else{
                this.Log( id ,"TaxExempt", "Not Found TaxExempt in a delete attempt.", "Tax Exempt", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("migrate")]
        public async Task<IActionResult> Migrate( ){
            var entities = this._context.TaxExempt.ToList();
            var oldEntries = this._reportingContext.zCesTaxExemptEntity;
            var oEnt = await oldEntries.Where( e => !entities.Select( n => n.LegacyId).Contains(e.rID) ).Take(20).ToListAsync();
            var PlanningUnits = await this._context.PlanningUnit.ToListAsync();
            var FinancialYears = await this._context.TaxExemptFinancialYear.ToListAsync();
            var oPlanningUnits = await this._mainContext.zzPlanningUnits.ToListAsync();
            var ProgramCategores = await this._context.TaxExemptProgramCategory.ToListAsync();
            var oFinYear = await this._reportingContext.zCesTaxExemptFinancialYearLookup.ToListAsync();
            var oFundsHndl = await this._reportingContext.zCesTaxExemptHowFundsHandledLookup.ToListAsync();
            foreach( var oE in oEnt){
                TaxExempt nE = new TaxExempt();
                nE.Name = oE.eName;
                var unit = this.FindUnit(oE, PlanningUnits, oPlanningUnits);
                nE.UnitId = unit != null ? unit.Id : 0;
                nE.LegacyId = oE.rID;
                nE.ById = FindUserId(oE);
                nE.Ein = oE.eID == "NULL" ? "" : oE.eID;
                nE.BankName = oE.eBankName == "NULL" ? "" : oE.eBankName;
                nE.BankAccountName = oE.eBankAcct == "NULL" ? "" : oE.eBankAcct;
                nE.TaxExemptProgramCategories = this.FindProgramCategories( oE, ProgramCategores);
                nE.DonorsReceivedAck = oE.DonorsReceivedAck == "NULL" ? "" : oE.DonorsReceivedAck;
                nE.HandledId = oE.eTaxExemptSrcFundsHandledID??1;
                nE.DistrictName = oE.eTaxExemptSrcExtDist_DistName == "NULL" ? "" : oE.eTaxExemptSrcExtDist_DistName;
                nE.DistrictEin = oE.eTaxExemptSrcExtDist_EIN == "NULL" ? "" : oE.eTaxExemptSrcExtDist_EIN;
                nE.OrganizationName = oE.eTaxExemptSrc501c_orgName == "NULL" ? "" : oE.eTaxExemptSrc501c_orgName;
                nE.OrganizationEin = oE.ein501c == "NULL" ? "" : oE.ein501c;
                nE.OrganizationResidesId = this.FindResides(oE, PlanningUnits, oPlanningUnits);
                nE.AnnBudget = oE.dtDocAnnBudget == "NULL" ? "" : oE.dtDocAnnBudget;
                nE.AnnFinancialRpt = oE.dtDocAnnFinancialRpt == "NULL" ? "" : oE.dtDocAnnFinancialRpt;
                nE.AnnAuditRpt = oE.dtDocAnnAuditRpt == "NULL" ? "" : oE.dtDocAnnAuditRpt;
                nE.AnnInvRpt = oE.dtDocAnnInvRpt == "NULL" ? "" : oE.dtDocAnnInvRpt;
                nE.OrganizationLetterDate = oE.dtDocIRSLOD == "NULL" ? "" : oE.dtDocIRSLOD;
                nE.OrganizationSignedDate = oE.dtDocMOU == "NULL" ? "" : oE.dtDocMOU;
                nE.OrganizationAppropriate = oE.dtDocIRS990 == "NULL" ? "" : oE.dtDocIRS990;
                var year = FinancialYears.Where( y => y.Name == oE.eFinancialYear ).FirstOrDefault();
                nE.TaxExemptFinancialYearId = year == null ? 1 : year.Id;
                nE.Created = nE.Updated = oE.rDT;
                nE.Areas = GeographicAreas(oE, PlanningUnits, oPlanningUnits);
                this._context.Add(nE);
            }
            this._context.SaveChanges();
            return new OkObjectResult(oEnt);
        }


        private List<TaxExemptArea> GeographicAreas(zCesTaxExemptEntity OldEntity, List<PlanningUnit> Units, List<zzPlanningUnit> oUnits){
            var areas = new List<TaxExemptArea>();
            var representations = OldEntity.eEntityGeoRepresentFIPs;
            var countyIds = representations.Split(", ");
            foreach( var countyId in countyIds){
                var oUnit = oUnits.Where( u => u.planningUnitID == countyId).FirstOrDefault();
                if(oUnit != null){
                    var unit = Units.Where( n => n.Name == oUnit.planningUnitName).FirstOrDefault();
                    if(unit != null ){
                        var area = new TaxExemptArea();
                        area.UnitId = unit.Id;
                        areas.Add(area);
                    }
                }
            }
            return areas;
        }
        private PlanningUnit FindUnit(zCesTaxExemptEntity OldEntity, List<PlanningUnit> Units, List<zzPlanningUnit> oUnits){
            var oUnit = oUnits.Where( u => u.planningUnitID == OldEntity.planningUnitID).FirstOrDefault();
            return Units.Where( n => n.Name == oUnit.planningUnitName).FirstOrDefault();
        }

        private int? FindResides(zCesTaxExemptEntity OldEntity, List<PlanningUnit> Units, List<zzPlanningUnit> oUnits){
            if(OldEntity.eTaxExemptSrc501cResidesFIPs == null || OldEntity.eTaxExemptSrc501cResidesFIPs == 0 ) return null;
            var oUnit = oUnits.Where( u => u.planningUnitID == (OldEntity.eTaxExemptSrc501cResidesFIPs??0).ToString()).FirstOrDefault();
            var unit = Units.Where( n => n.Name == oUnit.planningUnitName).FirstOrDefault();
            return unit == null ? null : unit.Id;
        }

        private int FindUserId(zCesTaxExemptEntity OldEntity){
            var user = this._context.KersUser.Where( u => u.RprtngProfile.PersonId == OldEntity.rBY).FirstOrDefault();
            return user == null ? 0 : user.Id;
        }
        
        private List<TaxExemptProgramCategoryConnection> FindProgramCategories( zCesTaxExemptEntity OldEntity, List<TaxExemptProgramCategory> programCategories){
            var Cats = new List<TaxExemptProgramCategoryConnection>();
            if( OldEntity.eProgANR == true ){
                var AnrCat = programCategories.Where( c => c.Name == "ANR").FirstOrDefault();
                var AnrConnection = new TaxExemptProgramCategoryConnection();
                AnrConnection.TaxExemptProgramCategory = AnrCat;
                Cats.Add(AnrConnection);
            }
            if( OldEntity.eProgHORT == true ){
                var HortCat = programCategories.Where( c => c.Name == "HORT").FirstOrDefault();
                var HortConnection = new TaxExemptProgramCategoryConnection();
                HortConnection.TaxExemptProgramCategory = HortCat;
                Cats.Add(HortConnection);
            }
            if( OldEntity.eProgFCS == true ){
                var FcsCat = programCategories.Where( c => c.Name == "FCS").FirstOrDefault();
                var FcsConnection = new TaxExemptProgramCategoryConnection();
                FcsConnection.TaxExemptProgramCategory = FcsCat;
                Cats.Add(FcsConnection);
            }
            if( OldEntity.eProg4HYD == true ){
                var HydCat = programCategories.Where( c => c.Name == "4-HYD").FirstOrDefault();
                var HydConnection = new TaxExemptProgramCategoryConnection();
                HydConnection.TaxExemptProgramCategory = HydCat;
                Cats.Add(HydConnection);
            }
            if( OldEntity.eProgCED == true ){
                var CedCat = programCategories.Where( c => c.Name == "CED").FirstOrDefault();
                var CedConnection = new TaxExemptProgramCategoryConnection();
                CedConnection.TaxExemptProgramCategory = CedCat;
                Cats.Add(CedConnection);
            }
            if( OldEntity.eProgFA == true ){
                var FaCat = programCategories.Where( c => c.Name == "FA").FirstOrDefault();
                var FaConnection = new TaxExemptProgramCategoryConnection();
                FaConnection.TaxExemptProgramCategory = FaCat;
                Cats.Add(FaConnection);
            }
            
            return Cats;
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