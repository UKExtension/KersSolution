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
using System.Dynamic;
using Kers.Models.Data;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class SnapedDataController : BaseController
    {
        
        IFiscalYearRepository fiscalRepo;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        ISnapDirectRepository snapDirectRepo;
        ISnapPolicyRepository snapPolicyRepo;
        ISnapFinancesRepository snapFinancesRepo;
        ISnapCommitmentRepository snapCommitmentRepo;
        const string LogType = "SnapEdData";
        public SnapedDataController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IDistributedCache _cache,
                    IFiscalYearRepository fiscalRepo,
                    IActivityRepository activityRepo,
                    ISnapDirectRepository snapDirectRepo,
                    ISnapPolicyRepository snapPolicyRepo,
                    ISnapFinancesRepository snapFinancesRepo,
                    ISnapCommitmentRepository snapCommitmentRepo
            ):base(mainContext, context, userRepo){
                this.fiscalRepo = fiscalRepo;
                this._cache = _cache;
                this.activityRepo = activityRepo;
                this.snapDirectRepo = snapDirectRepo;
                this.snapPolicyRepo = snapPolicyRepo;
                this.snapFinancesRepo = snapFinancesRepo;
                this.snapCommitmentRepo = snapCommitmentRepo;
        }
    
        [HttpGet]
        [Route("totalbymonth/{fy}/data.csv")]
        //[Produces("text/csv")]
        [Authorize]
        public IActionResult TotalByMonth(string fy){
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            return Ok(snapDirectRepo.TotalByMonth(fiscalYear));
        }

        [HttpGet]
        [Route("totalbycounty/{fy}/data.csv")]
        [Authorize]
        public IActionResult TotalByCounty(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            return Ok(snapDirectRepo.TotalByCounty(fiscalYear));
        }


        [HttpGet]
        [Route("totalbyemployee/{fy}/data.csv")]
        //[Produces("text/csv")]
        [Authorize]
        public IActionResult TotalByEmployee(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            return Ok(snapDirectRepo.TotalByEmployee(fiscalYear));
        }




        [HttpGet]
        [Route("directbypersonbymonth/{fy}/data.csv")]
        [Authorize]
        public IActionResult DirectSitesByPersonByMonth(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
             return Ok(snapDirectRepo.SitesPerPersonPerMonth(fiscalYear));
        }

        //audienceagecategory
        [HttpGet]
        [Route("audienceagecategory/{fy}/data.csv")]
        [Authorize]
        public IActionResult AudienceAgeCategory(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
             return Ok(snapDirectRepo.AudienceAgeCategory(fiscalYear));
        }


        [HttpGet]
        [Route("specificsitenamesbymonth/{fy}/data.csv")]
        [Authorize]        
        public IActionResult SpecificSiteNamesByMonth(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapDirectRepo.SpecificSiteNamesByMonth(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("specificsitedetails/{fy}/data.csv")]
        [Authorize]        
        public IActionResult SpecificSiteDetails(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapDirectRepo.SpecificSiteNamesDetails(fiscalYear);
            return Ok(result);
        }


        


        [HttpGet]
        [Route("individualcontacttotals/{fy}/data.csv")]
        [Authorize]
        public IActionResult IndividualContactTotals(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapDirectRepo.IndividualContactTotals(fiscalYear);
            return Ok(result);
        }


        [HttpGet]
        [Route("personnelhourdetails/{fy}/data.csv")]
        [Authorize]
        public IActionResult PersonnelHourDetails(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            
            return Ok(snapDirectRepo.PersonalHourDetails(fiscalYear));
        }


        [HttpGet]
        [Route("numberofdeliverysitesbytypeofsetting/{fy}/data.csv")]
        [Authorize]
        public IActionResult NumberofDeliverySitesbyTypeofSetting(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var result = this.snapDirectRepo.NumberofDeliverySitesbyTypeofSetting(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("methodsusedrecordcount/{fy}/data.csv")]
        [Authorize]
        public IActionResult MethodsUsedRecordCount(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapDirectRepo.MethodsUsedRecordCount(fiscalYear);
            return Ok(result);
        }


        [HttpGet]
        [Route("estimatedsizeofaudiencesreached/{fy}/data.csv")]
        [Authorize]
        public IActionResult EstimatedSizeofAudiencesReached(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapDirectRepo.EstimatedSizeofAudiencesReached( fiscalYear );
            return Ok(result);
        }



        [HttpGet]
        [Route("sessiontypebymonth/{fy}/data.csv")]
        [Authorize]
        public IActionResult SessionTypebyMonth(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = this.snapDirectRepo.SessionTypebyMonth(fiscalYear, true);
            return Ok(result);
        }

        
        [HttpGet]
        [Route("agentcommunityeventdetail/{fy}/data.csv")]
        [Authorize]
        public IActionResult AgentCommunityEventDetail(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapPolicyRepo.AgentCommunityEventDetail(fiscalYear);
            return Ok(result);
        }


        [HttpGet]
        [Route("bypartnercategory/{fy}/data.csv")]
        [Authorize]
        public IActionResult ByPartnerCategory(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapPolicyRepo.PartnerCategory(fiscalYear);
            return Ok(result);
        }


        [HttpGet]
        [Route("byaimedtowardsimprovement/{fy}/data.csv")]
        [Authorize]
        public IActionResult ByAimedTowardsImprovement(string fy){

            FiscalYear fiscalYear = GetFYByName(fy,"snapEd");

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            return Ok(snapPolicyRepo.AimedTowardsImprovement(fiscalYear));
        }


        [HttpGet]
        [Route("copiessummarybycountyagents/{fy}/data.csv")]
        [Authorize]
        public IActionResult CopiesSummarybyCountyAgents(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            //var result = CopiesReportPerCounty( fiscalYear, 1);
            var result = snapFinancesRepo.CopiesSummarybyCountyAgents(fiscalYear);
            
            return Ok( result );
        }

        [HttpGet]
        [Route("copiessummarybycountynotagents/{fy}/data.csv")]
        [Authorize]
        public IActionResult CopiesSummarybyCountyNotAgents(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapFinancesRepo.CopiesSummarybyCountyNotAgents(fiscalYear);
            
            return Ok( result );
        }


        [HttpGet]
        [Route("copiesdetailagents/{fy}/data.csv")]
        [Authorize]
        public IActionResult CopiesDetailAgents(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            //var result = CopiesReportDetails( fiscalYear, 1);
            var result = snapFinancesRepo.CopiesDetailAgents(fiscalYear);
            return Ok( result );
        }

        [HttpGet]
        [Route("copiesdetailnotagents/{fy}/data.csv")]
        [Authorize]
        public IActionResult CopiesDetailANotAgents(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            
            var result = snapFinancesRepo.CopiesDetailNotAgents(fiscalYear);
            return Ok( result );
        }


        [HttpGet]
        [Route("reimbursementnepassistants/{fy}/data.csv")]
        [Authorize]
        public IActionResult ReimbursementNepAssistants(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var result = snapFinancesRepo.ReimbursementNepAssistants(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("reimbursementcounty/{fy}/data.csv")]
        [Authorize]
        public IActionResult ReimbursementCounty(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = snapFinancesRepo.ReimbursementCounty(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("commitmentsummary/{fy}/data.csv")]
        [Authorize]
        public async Task<IActionResult> CommitmentSummary(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = await snapCommitmentRepo.CommitmentSummary(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("commitmenthoursdetail/{fy}/data.csv")]
        [Authorize]
        public async Task<IActionResult> CommitmentHoursDetail(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Commitment Hours Detail Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = await snapCommitmentRepo.CommitmentHoursDetail(fiscalYear,true);
            return Ok(result);
        }

        [HttpGet]
        [Route("agentswithoutcommitment/{fy}/data.csv")]
        [Authorize]
        public async Task<IActionResult> AgentsWithoutCommitment(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Agents Without Commitment CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = await snapCommitmentRepo.AgentsWithoutCommitment(fiscalYear, true);
            return Ok(result);
        }



        [HttpGet]
        [Route("summarybyplanningunit/{fy}/data.csv")]
        [Authorize]
        public async Task<IActionResult> SummaryByPlanningUnit(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Summary By PlanningUnit CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = await snapCommitmentRepo.SummaryByPlanningUnit(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("summarybyplanningunitnotassistants/{fy}/data.csv")]
        [Authorize]
        public async Task<IActionResult> SummaryByPlanningUnitNotNEPAssistants(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Summary By PlanningUnit CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = await snapCommitmentRepo.SummaryByPlanningUnitNotNEPAssistants(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("reinforcementitems/{fy}/data.csv")]
        [Authorize]
        public async Task<IActionResult> ReinforcementItems(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Reinforcement Items CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = await snapCommitmentRepo.ReinforcementItems(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("reinforcementitemspercounty/{fy}/data.csv")]
        [Authorize]
        public async Task<IActionResult> ReinforcementItemsPerCounty(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Reinforcement Items Per County CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = await snapCommitmentRepo.ReinforcementItemsPerCounty(fiscalYear);
            return Ok(result);
        }

        [HttpGet]
        [Route("suggestedincentiveitems/{fy}/data.csv")]
        [Authorize]
        public async Task<IActionResult> SuggestedIncentiveItems(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Suggested Incentive Items CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var result = await snapCommitmentRepo.SuggestedIncentiveItems(fiscalYear);
            return Ok(result);
        }


        [HttpGet]
        [Route("indirectbyemployee/{fy}/data.csv")]
        [Authorize]
        public IActionResult IndirectByEmployee(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            return Ok(this.snapDirectRepo.IndirectByEmployee(fiscalYear));
        }


        [HttpGet]
        [Route("partnersofacounty/{unitid}/{fy}/data.csv")]
        [Authorize]
        public IActionResult PartnesOfCounty(int unitid, string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            return Ok(this.snapPolicyRepo.PartnersOfACounty(unitid, fiscalYear));
        }



    
    }


}