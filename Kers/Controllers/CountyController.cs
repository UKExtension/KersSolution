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
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class CountyController : BaseController
    {


        private IDistributedCache _cache;

        public CountyController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IDistributedCache _cache,
                    IKersUserRepository userRepo
            ):base(mainContext, context, userRepo){
           
                this._cache = _cache;
        }


        [HttpGet("{id?}")]
        public IActionResult Get(int id = 0){
            if(id == 0){
                var unit = CurrentPlanningUnit();
                id = unit.Id;
            }
            var county = this.context.PlanningUnit.
                                Where(c=>c.Id == id).
                                FirstOrDefault();
            return new OkObjectResult(county);
        }

        [HttpGet("countylist")]
        public async Task<IActionResult> Countylist(){

            List<PlanningUnit> counties;

           
            
            var cacheKey = "CountiesList";
            var cached = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cached)){
                counties = JsonConvert.DeserializeObject<List<PlanningUnit>>(cached);
            }else{
            
            
                counties = await this.context.PlanningUnit.
                                Where(c=>c.District != null && c.Name.Substring(c.Name.Count() - 3) == "CES").
                                OrderBy(c => c.Name).ToListAsync();
                

                var serializedCounties = JsonConvert.SerializeObject(counties);
                _cache.SetString(cacheKey, serializedCounties, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                        });
            }
            return new OkObjectResult(counties);
        }



        private KersUser userByLinkBlueId(string linkBlueId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
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

        private PlanningUnit CurrentPlanningUnit(){
            var u = this.CurrentUserId();
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == u).
                            FirstOrDefault();
            return  this.context.PlanningUnit.
                    Where( p=>p.Code == profile.planningUnitID).
                    FirstOrDefault();
        }
    }
}