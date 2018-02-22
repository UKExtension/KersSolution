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


        [HttpGet("committed/{userId?}")]
        [Authorize]
        public IActionResult Committed(int userId = 0){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = context.KersUser.Find(userId);
            }
            var committed = context.SnapEd_Commitment.
                                Where(e =>  e.KersUserId == user.classicReportingProfileId 
                                            &&
                                            e.SnapEd_ActivityType.Measurement == "Hour"
                                        ).
                                Sum( h => h.Amount);
            if(committed == null) committed = 0;
            return new OkObjectResult(committed);
        }

        [HttpGet("commitments/{userId?}")]
        [Authorize]
        public IActionResult Commitments(int userId = 0){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
            }else{
                user = context.KersUser.Find(userId);
            }
            var committed = context.SnapEd_Commitment.
                                Where(e=>   e.KersUserId == user.classicReportingProfileId 
                                            &&
                                            e.SnapEd_ActivityType.Measurement == "Hour");
            return new OkObjectResult(committed);
        }





        
        [HttpGet("activitytypes")]
        public IActionResult ActivityTypes(){

            var committed = context.SnapEd_ActivityType;
            return new OkObjectResult(committed);
        }
        [HttpGet("projecttypes")]
        public IActionResult ProjectTypes(){

            var committed = context.SnapEd_ProjectType;
            return new OkObjectResult(committed);
        }



        

    }
}