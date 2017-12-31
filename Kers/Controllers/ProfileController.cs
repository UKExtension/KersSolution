using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private KERSmainContext mContext;
        private IzEmpRptProfileRepository repo;

        public ProfileController( 
                                KERSmainContext mediaContext,
                                IzEmpRptProfileRepository profileRepository
            
                            ){
            mContext = mediaContext;
            repo = profileRepository;
        }

        [HttpGet()]
        public IActionResult Get(){
            return NotFound(new {Error = "not found"});
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id){
            var profile = repo.GetSingle( id );
            return new OkObjectResult(profile);
        }

        
        [HttpGet("GetLatest")]
        [Authorize]
        public IActionResult GetLatest(){
            return GetLatest(DefaultNumberOfItems);
        }

        [HttpGet("GetLatest/{n}")]
        [Authorize]
        public IActionResult GetLatest(int n){
            if ( n > MaxNumberOfItems ) n = MaxNumberOfItems;
            var profiles = mContext.zEmpRptProfiles.OrderByDescending( i => i.rDT ).Take(n);
            return new OkObjectResult(profiles);
        }

        [HttpGet("GetCustom")]
        public IActionResult GetCustom( [FromQuery] string search, 
                                        [FromQuery] string unit = "0", 
                                        [FromQuery] string position = "0",
                                        [FromQuery] string amount = "0"
                                        ){
            var theAmount = Convert.ToInt32(amount);
            theAmount =  theAmount <= 0 ? DefaultNumberOfItems : theAmount ;
            var profiles = from i in mContext.zEmpRptProfiles select i;
            if(search != null){
                profiles = profiles.Where( i => i.personName.Contains(search));
            }
            if( unit != "0" ){
                profiles = profiles.Where( i => i.planningUnitID == unit );
            }
            if( position != "0" ){
                profiles = profiles.Where( i => i.positionID == position );
            }
            profiles = profiles.Take(theAmount);
            return new OkObjectResult(profiles);
        }

        [HttpGet("GetCustomCount")]
        public IActionResult GetCustomCount([FromQuery] string search, [FromQuery] string unit = "0", [FromQuery] string position = "0"){
            var profiles = from i in mContext.zEmpRptProfiles select i;
            if(search != null){
                profiles = profiles.Where( i => i.personName.Contains(search));
            }
            if( unit != "0" ){
                profiles = profiles.Where( i => i.planningUnitID == unit );
            }
            if( position != "0" ){
                profiles = profiles.Where( i => i.positionID == position );
            }
            return new OkObjectResult(profiles.Count());
        }



        [HttpGet("GetRandom")]
        public IActionResult GetRandom(){
            return GetRandom(DefaultNumberOfItems);
        }

        [HttpGet("GetRandom/{n}")]
        public IActionResult GetRandom(int n){
            if ( n > MaxNumberOfItems ) n = MaxNumberOfItems;
            var profiles = repo.GetAll().OrderBy( i => Guid.NewGuid() ).Take(n);
            return new OkObjectResult(profiles);
        }

        [HttpPut("{id}/{admin?}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] zEmpRptProfile prfl, bool admin = false){
            if(prfl != null){
                var profile = mContext.zEmpRptProfiles.Where( i => i.Id == id).FirstOrDefault();
                if( profile != null ){
                    
                    if(prfl.positionID != profile.positionID){
                        var position = mContext.zzExtensionPositions.Where( i => i.posCode == prfl.positionID).FirstOrDefault();
                        profile.zzExtensionPosition = position;
                    }
                    
                    
                    if(profile.planningUnitID != prfl.planningUnitID){
                        var unit = mContext.zzPlanningUnits.Where( i => i.planningUnitID == prfl.planningUnitID).FirstOrDefault();
                        if(unit != null){
                            profile.zzPlaningUnit = unit;
                            profile.planningUnitName = unit.planningUnitName;
                        }
                    }
                    
                    profile.progANR = prfl.progANR;
                    profile.progHORT = prfl.progHORT;
                    profile.progFCS = prfl.progFCS;
                    profile.prog4HYD = prfl.prog4HYD;
                    profile.progFACLD = prfl.progFACLD;
                    profile.progNEP = prfl.progNEP;
                    profile.progOther = prfl.progOther;

                    if(profile.locationID != prfl.locationID){
                        var location = mContext.zzGeneralLocations.Where( i => i.locationID == prfl.locationID).FirstOrDefault();
                        if(location != null){
                            profile.zzGeneralLocation = location;
                        }
                    }

                    profile.emailDeliveryAddress = prfl.emailDeliveryAddress;
                    profile.emailUEA = prfl.emailUEA;
                    mContext.SaveChangesAsync();
                    return new OkObjectResult(profile);
                }
            }
        
            return NotFound(new {Error = String.Format("Profile with id {0} has not been found", id)});
        }

        [HttpGet("CurrentUser")]
        [Authorize]
        public IActionResult CurrentUser(){
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var profile = repo.GetSingle(i => i.linkBlueID == userId);
            return new OkObjectResult(profile);
        }

        [HttpGet("PlanningUnit")]
        public IActionResult PlanningUnit(){
            var units = mContext.zzPlanningUnits.Where( i => i.reportsExtension == true).OrderBy(i=>i.orderID);
            return new OkObjectResult(units);
        }

        [HttpGet("Position")]
        public IActionResult Position(){
            var units = mContext.zzExtensionPositions.OrderBy(i=>i.orderID);
            return new OkObjectResult(units);
        }

        [HttpGet("Location")]
        public IActionResult Location(){
            var units = mContext.zzGeneralLocations.OrderBy(i=>i.orderID);
            return new OkObjectResult(units);
        }


        private int DefaultNumberOfItems{
            get
            {
                return 40;
            }
        }
        private int MaxNumberOfItems{
            get
            {
                return 22000;
            }
        }
    }
}