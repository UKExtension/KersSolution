using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
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
    public class ExtensionRegionController : BaseController
    {


        KERScoreContext _context;
        public ExtensionRegionController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo
            ):base(mainContext, context, userRepo){
                _context = context;
        }

        [HttpGet("{id?}")]
        [Authorize]
        public IActionResult Get(int id = 0){
            if(id == 0){
                var unit = CurrentPlanningUnit();
                id = unit.ExtensionAreaId ?? 0;
            }
            var area = this.context.ExtensionArea.
                                Where(c=>c.Id == id).
                                FirstOrDefault();
            return new OkObjectResult(area);
        }


        [HttpGet("countiesbyregionid/{id}")]
        [Authorize]
        public async Task<IActionResult> CountiesByRegionId(int id){
            if( id == 0 ){
                var user = this.CurrentUser();
                id = (await this.context.PlanningUnit
                            .Where( u => u.Id == user.RprtngProfile.PlanningUnitId)
                            .Select( u => u.ExtensionAreaId)
                            .FirstOrDefaultAsync()) ?? 0;
            }
            var cnts = await this.counties(id);
            return new OkObjectResult( cnts );
        }

        private async Task<List<PlanningUnit>> counties( int RegionId){
            IQueryable<PlanningUnit> counties = null;

                counties = this.context.PlanningUnit.Where( u => u.ExtensionArea.ExtensionRegionId == RegionId );
            
            return await counties.ToListAsync();
        }

        [HttpGet("countiesbycountyid/{Id?}/{includePairings?}/{userId?}")]
        [Authorize]
        public async Task<IActionResult> CountiesByCountyId(int Id=0, bool includePairings = false){
            if(Id == 0){
                var user = this.CurrentUser();
                Id = user.RprtngProfile.PlanningUnitId;
            }
            var unit = await context.PlanningUnit.Where( u => u.Id == Id).Include( p => p.ExtensionArea).FirstOrDefaultAsync();
            return new OkObjectResult(await this.counties(unit.ExtensionArea.ExtensionRegionId));
        }


        private PlanningUnit CurrentPlanningUnit(){
            var u = this.CurrentUser();
            return  this.context.PlanningUnit.
                    Where( p=>p.Id == u.RprtngProfile.PlanningUnitId).
                    Include( p => p.ExtensionArea).
                    FirstOrDefault();
        }





    }
    
}
