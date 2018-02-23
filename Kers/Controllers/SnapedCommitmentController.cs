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
                                );

            return new OkObjectResult(new {Commitments = committed, Items = items, Suggestion = itemSuggestion});
        }





        
        [HttpGet("activitytypes/{fiscalyearId?}")]
        public IActionResult ActivityTypes(int fiscalyearId = 0){
            FiscalYear fiscalYear;
            if(fiscalyearId == 0){
                fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.SnapEd);
            }else{
                fiscalYear = context.FiscalYear.Find(fiscalyearId);
            }
            var types = context.SnapEd_ActivityType.Where( t => t.FiscalYear == fiscalYear);
            return new OkObjectResult(types);
        }
        [HttpGet("projecttypes/{fiscalyearId?}")]
        public IActionResult ProjectTypes(int fiscalyearId = 0){
            FiscalYear fiscalYear;
            if(fiscalyearId == 0){
                fiscalYear = fiscalYearRepo.nextFiscalYear(FiscalYearType.SnapEd);
            }else{
                fiscalYear = context.FiscalYear.Find(fiscalyearId);
            }
            var types = context.SnapEd_ProjectType.Where( t => t.FiscalYear == fiscalYear);
            return new OkObjectResult(types);
        }



        

    }
}