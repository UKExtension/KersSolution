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

namespace Kers.Controllers
{

    

    [Route("api/[controller]")]
    public class SnapedCommitmentController : BaseController
    {
        private IFiscalYearRepository fiscalYearRepo;

        public SnapedCommitmentController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IFiscalYearRepository fiscalYearRepo
            ):base(mainContext, context, userRepo){
                this.fiscalYearRepo = fiscalYearRepo;
        }

        [HttpGet]
        [Route("[action]/{FyName}")]
        public async Task<IActionResult> FiscalYearByName( string FyName)
        {
            var fy = await this.context.FiscalYear
                                    .Where(y => y.Name == FyName && y.Type == FiscalYearType.SnapEd)
                                    .FirstOrDefaultAsync();
            return new OkObjectResult(fy);
        }

        

        [HttpGet("commitments/{fiscalyearId?}/{userId?}")]
        [Authorize]
        public IActionResult Commitments(int fiscalyearId = 0, int userId = 0){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = context.KersUser.Find(userId);
            }
            FiscalYear fiscalYear;
            if(fiscalyearId == 0){
                fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.SnapEd);
            }else{
                fiscalYear = context.FiscalYear.Find(fiscalyearId);
            }
            var committed = context.SnapEd_Commitment.
                                Where(e=>   e.KersUser == user
                                            &&
                                            e.FiscalYear == fiscalYear);
            
            var items = context.SnapEd_ReinforcementItemChoice
                                .Where( i =>
                                    i.KersUser == user
                                    &&
                                    i.FiscalYear == fiscalYear
                                );
            var itemSuggestion = context.SnapEd_ReinforcementItemSuggestion
                                .Where( i =>
                                    i.KersUser == user
                                    &&
                                    i.FiscalYear == fiscalYear
                                ).FirstOrDefault();

            return new OkObjectResult(new {Commitments = committed, Items = items, Suggestion = itemSuggestion, Fiscalyearid = fiscalYear.Id});
        }





        
        [HttpPost()]
        [Authorize]
        public IActionResult AddOrEditCommitment( [FromBody] CommitmentBundle commitment){
            
            if(commitment != null){
                KersUser user;
                if(commitment.Userid == 0){
                    user = this.CurrentUser();
                }else{
                    user = context.KersUser.Find(commitment.Userid);
                }
                FiscalYear fiscalYear;
                if(commitment.Fiscalyearid == 0){
                    fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.SnapEd);
                }else{
                    fiscalYear = context.FiscalYear.Find(commitment.Fiscalyearid);
                }

                foreach( var commtmnt in commitment.Commitments){
                    commtmnt.KersUser = user;
                    commtmnt.FiscalYear = fiscalYear;
                    commtmnt.KersUserId = user.classicReportingProfileId;
                }
                foreach( var item in commitment.Items){
                    item.KersUser = user;
                    item.FiscalYear = fiscalYear;
                    item.zEmpProfileId = user.classicReportingProfileId;
                }


                var currentCommitment = context.SnapEd_Commitment.Where( c => c.KersUser == user && c.FiscalYear == fiscalYear);
                context.SnapEd_Commitment.RemoveRange(currentCommitment);
                var currnetItemsChoice = context.SnapEd_ReinforcementItemChoice.Where( c => c.KersUser == user && c.FiscalYear == fiscalYear);
                context.SnapEd_ReinforcementItemChoice.RemoveRange(currnetItemsChoice);
                var currentSuggestion = context.SnapEd_ReinforcementItemSuggestion.Where( c => c.KersUser == user && c.FiscalYear == fiscalYear);
                context.SnapEd_ReinforcementItemSuggestion.RemoveRange(currentSuggestion);

                context.SnapEd_Commitment.AddRange(commitment.Commitments);
                context.SnapEd_ReinforcementItemChoice.AddRange(commitment.Items);
                if(commitment.Suggestion != ""){
                    var sug = new SnapEd_ReinforcementItemSuggestion();
                    sug.FiscalYear = fiscalYear;
                    sug.KersUser = user;
                    sug.zEmpProfileId = user.classicReportingProfileId;
                    sug.Suggestion = commitment.Suggestion;
                    context.SnapEd_ReinforcementItemSuggestion.Add(sug);
                }
                
                this.Log(commitment, "CommitmentBundle", "Snap-Ed commitment added or updated.", "Commitment");
                context.SaveChanges();
                return new OkObjectResult(commitment);
            }else{
                this.Log( commitment ,"CommitmentBundle", "Not Found Commitment Bundle in an adding/editent attempt.", "Commitment", "Error");
                return new StatusCodeResult(500);
            }
        }
    }

    public class CommitmentBundle{
        public List<SnapEd_Commitment> Commitments;
        public List<SnapEd_ReinforcementItemChoice> Items;
        public string Suggestion;
        public int Userid;
        public int Fiscalyearid;

    }
}