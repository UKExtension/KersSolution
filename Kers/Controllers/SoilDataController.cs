using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Contexts;
using Kers.Models.Entities.UKCAReporting;
using Microsoft.AspNetCore.Hosting;
using Kers.Models.Entities.SoilData;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class SoilDataController : ExtensionEventController
    {
        KERScoreContext _context;
        SoilDataContext _soilDataContext;
        IKersUserRepository _userRepo;
        ILogRepository logRepo;
        public SoilDataController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    SoilDataContext soilDataContext,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo
            ):base(mainContext, context, userRepo){
           this._context = context;
           this._userRepo = userRepo;
           this.logRepo = logRepo;
           this._soilDataContext = soilDataContext;
            
            //Associate County Codes with Planning Units if not done yet
            if(!soilDataContext.CountyCodes.Where( c => c.PlanningUnitId != 0).Any()){
                foreach( var countyCode in _soilDataContext.CountyCodes){
                    var unit = this.context.PlanningUnit.
                                    Where( u => 
                                                u.Name.Count() > 11
                                                &&
                                                u.Name.Substring(0,u.Name.Count() - 11).ToUpper() == countyCode.Name 
                                        ).
                                    FirstOrDefault();
                    if( unit != null) countyCode.PlanningUnitId = unit.Id;
                }
                soilDataContext.SaveChanges();
            }



        }

        [HttpPost("addaddress")]
        [Authorize]
        public IActionResult AddAddress( [FromBody] FarmerAddress address){
            if(address != null){
                var user = this.CurrentUser();
                var countyCode = _soilDataContext.CountyCodes.FirstOrDefault( c => c.PlanningUnitId == user.RprtngProfile.PlanningUnitId);
                address.CountyCode = countyCode;
                _soilDataContext.Add(address); 
                _soilDataContext.SaveChanges();
                this.Log(address,"FarmerAddress", "Farmer Address added.");

                return new OkObjectResult(address);
            }else{
                this.Log( address ,"FarmerAddress", "Error in adding FarmerAddress attempt.", "FarmerAddress", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("updateaddress/{id}")]
        [Authorize]
        public IActionResult UpdateAddress( int id, [FromBody] FarmerAddress address){
            var adr = _soilDataContext.FarmerAddress.Find(id);
            if(address != null && adr != null ){
                adr.First = address.First;
                adr.Last = address.Last;
                adr.Address = address.Address;
                adr.City = address.City;
                adr.St = address.St;
                adr.Zip = address.Zip;
                adr.HomeNumber = address.HomeNumber;
                adr.EmailAddress = address.EmailAddress;
                _soilDataContext.SaveChanges();
                this.Log(address,"FarmerAddress", "FarmerAddress Updated.");
                return new OkObjectResult(address);
            }else{
                this.Log( address ,"FarmerAddress", "Not Found Farmer Address in an update attempt.", "FarmerAddress", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("addresses/{countyid?}")]
        public async Task<IActionResult> RegisterWindows(int countyid = 0){
            if( countyid == 0 ){
                countyid = this.CurrentUser().RprtngProfile.PlanningUnitId;
            }
            var addresses = await _soilDataContext.FarmerAddress.
                                    Where(a => a.CountyCode.PlanningUnitId == countyid).
                                    ToListAsync();
            return new OkObjectResult(addresses);
        }

    }
}