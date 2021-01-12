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
    public class ExtensionAreaController : BaseController
    {


        string[][] pairings = {
            new string[] {"C1", "C2"},
            new string[] {"C3", "C4"},
            new string[] {"C5", "C6"},
            new string[] {"C7", "C8"},
            new string[] {"E1", "E2"},
            new string[] {"E3", "E4"},
            new string[] {"E5", "E6"},
            new string[] {"E7", "E8"},
            new string[] {"W1", "W2"},
            new string[] {"W3", "W4"},
            new string[] {"W5", "W6"},
            new string[] {"W7", "W8"}
        };

        KERScoreContext _context;
        public ExtensionAreaController( 
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

        [HttpGet("pairing/{id?}")]
        [Authorize]
        public IActionResult GetPairing(int id = 0){
            if(id == 0){
                var unit = CurrentPlanningUnit();
                id = unit.ExtensionAreaId ?? 0;
            }
            var area = this.context.ExtensionArea.
                                Where(c=>c.Id == id).
                                FirstOrDefault();
            string[] pairing = FindContainingPair(area.Name);
            return new OkObjectResult(pairing);
        }


        [HttpGet("countiesbyareaid/{id}/{includePairings}")]
        [Authorize]
        public async Task<IActionResult> CountiesByAreaId(int id, bool includePairings = false){
            if( id == 0 ){
                var user = this.CurrentUser();
                id = (await this.context.PlanningUnit
                            .Where( u => u.Id == user.RprtngProfile.PlanningUnitId)
                            .Select( u => u.ExtensionAreaId)
                            .FirstOrDefaultAsync()) ?? 0;
            }
            var cnts = await this.counties(id, includePairings);
            return new OkObjectResult( cnts.OrderBy( u => u.Name ) );
        }

        private async Task<List<PlanningUnit>> counties( int ArreaId, bool includePairings){
            IQueryable<PlanningUnit> counties = null;
            if( includePairings ){
                var area = await this.context.ExtensionArea.FindAsync( ArreaId );
                string[] pairing = FindContainingPair(area.Name);
                counties = this.context.PlanningUnit.Where( u => pairing.Contains( u.ExtensionArea.Name ));
            }else{
                counties = this.context.PlanningUnit.Where( u => u.ExtensionAreaId == ArreaId );
            }
            return await counties.ToListAsync();
        }

        [HttpGet("countiesbycountyid/{Id?}/{includePairings?}/{userId?}")]
        [Authorize]
        public async Task<IActionResult> CountiesByCountyId(int Id=0, bool includePairings = false){
            if(Id == 0){
                var user = this.CurrentUser();
                Id = user.RprtngProfile.PlanningUnitId;
            }
            var unit = await context.PlanningUnit.Where( u => u.Id == Id).FirstOrDefaultAsync();
            return new OkObjectResult(await this.counties(unit.ExtensionAreaId??0, includePairings));
        }

        public string[] FindContainingPair( string Area ){
            string[] pairing = this.pairings.Where( r => r.Contains(Area)).FirstOrDefault();
            if( pairing == null) pairing =  new string[] {Area}; 
            return pairing;
        }


        private PlanningUnit CurrentPlanningUnit(){
            var u = this.CurrentUser();
            return  this.context.PlanningUnit.
                    Where( p=>p.Id == u.RprtngProfile.PlanningUnitId).
                    FirstOrDefault();
        }





    }
    
}
