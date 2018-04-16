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

          /*   var keys = new List<string>();
            		
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            var methods = context.SnapIndirectReached.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
            foreach( var met in methods){
                keys.Add(string.Concat( "\"", met.Name, "\""));
            }

            var result = string.Join(",", keys.ToArray()) + "\n";

            var perMonth = RevisionsWithIndirectContactsPerMonth( fiscalYear);
            foreach( var mnth in perMonth){
                var dt = new DateTime(mnth.Year, mnth.Month, 15);
                var row = dt.ToString("yyyyMM") + ",";
                row += dt.ToString("yyyy-MMM") + ",";
                var ids = mnth.Revs.Select( r => r.Id);
                var indirects = context.ActivityRevision.Where( r => ids.Contains( r.Id ))
                            .Select( i => i.SnapIndirect.SnapIndirectReachedValues  );
                var selections = new List<SnapIndirectReachedValue>();
                foreach( var ind in indirects){
                    if(ind != null){
                        selections.AddRange( ind );
                    }
                    
                }       
                foreach( var mt in methods){
                    row += selections.Where( r => r.SnapIndirectReachedId == mt.Id).Sum( s => s.Value).ToString() + ",";
                }

                result += row + "\n";
            } */
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

            var keys = new List<string>();
 
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            var types = context.SnapDirectSessionType.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
            foreach( var met in types){
                keys.Add(string.Concat( "\"", met.Name, " Number Delivered\""));
                keys.Add(string.Concat( "\"", met.Name, " Min Minutes\""));
                keys.Add(string.Concat( "\"", met.Name, " Miax Minutes\""));
            }
            keys.Add("MonthlyTotal");
            var result = string.Join(",", keys.ToArray()) + "\n";




            var perMonth = RevisionsWithDirectContactsPerMonth( fiscalYear);
            foreach( var mnth in perMonth){
                var dt = new DateTime(mnth.Year, mnth.Month, 15);
                var row = dt.ToString("yyyyMM") + ",";
                row += dt.ToString("yyyy-MMM") + ",";
                
                var ids = mnth.Revs.Select( r => r.Id);
                var MonthlyTotal = 0;
                foreach( var type in types){
                    var byType = context.ActivityRevision.Where( r => ids.Contains( r.Id) && r.SnapDirect.SnapDirectSessionTypeId == type.Id);
                    var cnt = byType.Count();
                    MonthlyTotal += cnt;
                    row += cnt.ToString() + ",";
                    if( cnt == 0){
                        row += ",,";
                    }else{
                        row += (byType.Min( t => t.Hours) * 60).ToString() + ",";
                        row += (byType.Max( t => t.Hours) * 60).ToString() + ",";
                    }
                }

                row += MonthlyTotal.ToString();
                result += row + "\n";
            }
             
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

            FiscalYear fiscalYear = GetFYByName(fy);

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


        private List<ActivityRevisionsPerMonth> RevisionsWithIndirectContactsPerMonth( FiscalYear fiscalYear){
            var perMonth = new List<ActivityRevisionsPerMonth>();
            var currentDate = DateTime.Now;
            var runningDate = fiscalYear.Start;
            var difference = (int)Math.Floor(fiscalYear.End.Subtract(fiscalYear.Start).Days / (365.2425 / 12)) + 1;
            var months = new DateTime[difference];
            var i = 0;
            do{
                months[i] = runningDate.AddMonths( i );
                if( months[i].Year < currentDate.Year || ( months[i].Year == currentDate.Year && months[i].Month <= currentDate.Month ) ){
                    var cacheKey = "IndirectActivityRevisionsPerMonth" + months[i].Month.ToString() + months[i].Year.ToString();
                    var cachedTypes = _cache.GetString(cacheKey);
                    var entity = new ActivityRevisionsPerMonth();
                    if (!string.IsNullOrEmpty(cachedTypes)){
                        entity = JsonConvert.DeserializeObject<ActivityRevisionsPerMonth>(cachedTypes);
                    }else{
                        var byMonth = context.Activity.Where( c => c.ActivityDate.Month == months[i].Month && c.ActivityDate.Year == months[i].Year);
                        var activityRevisionsPerMonty = byMonth
                                .Select( a => a.Revisions.OrderBy(r => r.Created).Last())
                                .Where( e => e.SnapIndirect != null)
                                .ToList();
                        entity.Revs = activityRevisionsPerMonty;
                        entity.Month = months[i].Month;
                        entity.Year = months[i].Year;

                        var yearDifference = currentDate.Year - months[i].Year;
                        var monthsDifference = currentDate.Month - months[i].Month;

                        var cachePeriod = Math.Floor( (float) (yearDifference * 10 + monthsDifference + 5) / 2 );
                        if(cachePeriod <= 0){
                            cachePeriod = 1;
                        }

                        var serializedActivities = JsonConvert.SerializeObject(entity);
                        _cache.SetString(cacheKey, serializedActivities, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cachePeriod)
                            });
                                    
                    }
                    perMonth.Add(entity);
                }
                i++;
            }while(i < difference);

            return perMonth;
        }

        private List<ActivityRevisionsPerMonth> RevisionsWithDirectContactsPerMonth( FiscalYear fiscalYear){
            var perMonth = new List<ActivityRevisionsPerMonth>();
            var currentDate = DateTime.Now;
            var runningDate = fiscalYear.Start;
            var difference = (int)Math.Floor(fiscalYear.End.Subtract(fiscalYear.Start).Days / (365.2425 / 12)) + 1;
            var months = new DateTime[difference];
            var i = 0;
            do{
                months[i] = runningDate.AddMonths( i );
                if( months[i].Year < currentDate.Year || ( months[i].Year == currentDate.Year && months[i].Month <= currentDate.Month ) ){
                    var cacheKey = "DirectActivityRevisionsPerMonth" + months[i].Month.ToString() + months[i].Year.ToString();
                    var cachedTypes = _cache.GetString(cacheKey);
                    var entity = new ActivityRevisionsPerMonth();
                    if (!string.IsNullOrEmpty(cachedTypes)){
                        entity = JsonConvert.DeserializeObject<ActivityRevisionsPerMonth>(cachedTypes);
                    }else{
                        var byMonth = context.Activity.Where( c => c.ActivityDate.Month == months[i].Month && c.ActivityDate.Year == months[i].Year);
                        var activityRevisionsPerMonty = byMonth
                                .Select( a => a.Revisions.OrderBy(r => r.Created).Last())
                                .Where( e => e.SnapDirect != null)
                                .ToList();
                        entity.Revs = activityRevisionsPerMonty;
                        entity.Month = months[i].Month;
                        entity.Year = months[i].Year;

                        var yearDifference = currentDate.Year - months[i].Year;
                        var monthsDifference = currentDate.Month - months[i].Month;

                        var cachePeriod = Math.Floor( (float) (yearDifference * 10 + monthsDifference + 5) / 2 );
                        if(cachePeriod <= 0){
                            cachePeriod = 1;
                        }

                        var serializedActivities = JsonConvert.SerializeObject(entity);
                        _cache.SetString(cacheKey, serializedActivities, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cachePeriod)
                            });           
                    }
                    perMonth.Add(entity);
                }
                i++;
            }while(i < difference);

            return perMonth;
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
            var result = await snapCommitmentRepo.CommitmentHoursDetail(fiscalYear);
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
            var result = await snapCommitmentRepo.AgentsWithoutCommitment(fiscalYear);
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







        private List<UserRevisionData> SnapData( FiscalYear fiscalYear){
            var today = DateTime.Now;
            var revs = activityRepo.LastActivityRevisionIds(fiscalYear, _cache);

            var snapEligible = context.ActivityRevision.Where( r => revs.Contains( r.Id ) &&  (r.SnapPolicy != null || r.SnapDirect != null || r.SnapIndirect != null || r.SnapAdmin ));


            List<UserRevisionData> SnapData = new List<UserRevisionData>();

            foreach( var rev in snapEligible ){


                
                var cacheKey = "UserRevisionWithSnapData" + rev.Id.ToString();
                var cacheString = _cache.GetString(cacheKey);
                UserRevisionData data;
                if (!string.IsNullOrEmpty(cacheString)){
                    data = JsonConvert.DeserializeObject<UserRevisionData>(cacheString);
                }else{
                    
                    data = new UserRevisionData();
                    var activity = context.Activity.Where( a => a.Id == rev.ActivityId )
                                    .Include( a => a.KersUser ).ThenInclude( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                                    .Include( a => a.KersUser ).ThenInclude( u => u.ExtensionPosition)
                                    .Include( a => a.KersUser).ThenInclude( u => u.Specialties).ThenInclude( s => s.Specialty)
                                    .FirstOrDefault();
                    var revision = context.ActivityRevision.Where( r => r.Id == rev.Id)
                                        .Include( s => s.SnapDirect).ThenInclude( d => d.SnapDirectAgesAudienceValues )
                                        .Include( s => s.SnapIndirect).ThenInclude( i => i.SnapIndirectReachedValues)
                                        .Include( s => s.RaceEthnicityValues)
                                        .Include( s => s.ActivityOptionNumbers).FirstOrDefault();
                    
                    data.User = activity.KersUser;
                    data.Revision = revision;


                    var expiration = (today - activity.ActivityDate).Days;
                    if( expiration < 1){
                        expiration = 1;
                    }

                    var serialized = JsonConvert.SerializeObject(data);
                    _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( expiration )
                        }); 
                }

                

                
                SnapData.Add(data);
            }
            return SnapData;
        }


        private int ContactsPerRaceEthnicity( List<RaceEthnicityValue> vals, Race race, Ethnicity ethnicity){
            return vals.Where( v => v.Race == race && v.Ethnicity == ethnicity).Sum(v => v.Amount);
        }

        private int ContactsPerSnapDirectAudience(List<SnapDirectAgesAudienceValue> vals, SnapDirectAudience audience){
                
                return vals.Where(v => v.SnapDirectAudience == audience).Sum( v => v.Value );

        }

        private int ContactsPerSnapDirectAge(List<SnapDirectAgesAudienceValue> vals, SnapDirectAges ages){
                
                return vals.Where(v => v.SnapDirectAges == ages).Sum( v => v.Value );

        }
        private string StripHTML(string htmlString){

            string pattern = @"<(.|\n)*?>";

            return Regex.Replace(htmlString, pattern, string.Empty);
        }







/* 
        !!!!!!! Keep this action for now !!!!!!!

        This info was requested just once. Will see if it will be needed again
 */
        [HttpGet]
        [Route("indirectbyemployee/{fy}/data.csv")]
        [Authorize]
        public IActionResult IndirectByEmployee(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
/* 
            var keys = new List<string>();
            keys.Add("FY");
            keys.Add("PlanningUnit");
            keys.Add("EmployeeName");
            keys.Add("Position");
            keys.Add("Program(s)");
            keys.Add("IndirectContacts");

            var reached = this.context.SnapIndirectReached.OrderBy( r => r.order);
            foreach( var r in reached){
                keys.Add(r.Name);
            }

            var result = string.Join(", ", keys.ToArray()) + "\n";

            var SnapData = this.SnapData( fiscalYear);

            var indirectSnapData = SnapData.Where( s => s.Revision.SnapIndirect != null && s.Revision.ActivityDate < fiscalYear.End && s.Revision.ActivityDate > fiscalYear.Start);

            var byUser = indirectSnapData.GroupBy( s => s.User.Id).Select( 
                                        d => new {
                                            User = d.Select( s => s.User ).First(),
                                            Revisions = d.Select( s => s.Revision )
                                        }
                                    )
                                    .OrderBy( d => d.User.RprtngProfile.PlanningUnit.Name).ThenBy( d => d.User.RprtngProfile.Name);
            foreach( var userData in byUser ){
                var row = fiscalYear.Name + ",";
                row += string.Concat("\"", userData.User.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                row += string.Concat("\"", userData.User.RprtngProfile.Name, "\"") + ",";
                row += string.Concat("\"", userData.User.ExtensionPosition.Code, "\"") + ",";
                var spclt = "";
                foreach( var sp in userData.User.Specialties){
                    spclt += " " + sp.Specialty.Code;
                }
                row += spclt + ", ";
                
                var optNumbrs = new List<ActivityOptionNumberValue>();
                

                var reachedData = new List<SnapIndirectReachedValue>();
                foreach( var dt in userData.Revisions){
                    optNumbrs.AddRange(dt.ActivityOptionNumbers);
                    reachedData.AddRange(dt.SnapIndirect.SnapIndirectReachedValues);
                }
                row += optNumbrs.Where( k =>k.ActivityOptionNumberId == 3).Sum( r => r.Value).ToString() + ",";
                foreach( var r in reached){
                    row += reachedData.Where( d => d.SnapIndirectReachedId == r.Id).Sum( l => l.Value).ToString() + ",";
                }
                result += row + "\n";
            }
 */
            return Ok(this.snapDirectRepo.IndirectByEmployee(fiscalYear));
        }



    
    }


}