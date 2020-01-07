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

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class LocationController : BaseController
    {
        KERScoreContext _context;
        KERSmainContext _mainContext;
        KERSreportingContext _reportingContext;
        IKersUserRepository _userRepo;
        IMessageRepository messageRepo;
        ITrainingRepository trainingRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IHostingEnvironment environment;
        public LocationController( 
                    KERSmainContext mainContext,
                    KERSreportingContext _reportingContext,
                    KERScoreContext context,
                    IMessageRepository messageRepo,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    ITrainingRepository trainingRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IHostingEnvironment env
            ):base(mainContext, context, userRepo){
           this._context = context;
           this._mainContext = mainContext;
           this._reportingContext = _reportingContext;
           this.messageRepo = messageRepo;
           this._userRepo = userRepo;
           this.logRepo = logRepo;
           this.trainingRepo = trainingRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           this.environment = env;
        }

 
        [HttpPost("addlocation")]
        [Authorize]
        public IActionResult AddLocation( [FromBody] ExtensionEventLocation location){
            if(location != null){
                context.Add(location); 
                context.SaveChanges();
                this.Log(location,"ExtensionEventLocation", "ExtensionEventLocation Added.");
                return new OkObjectResult(location);
            }else{
                this.Log( location ,"ExtensionEventLocation", "Error in adding ExtensionEventLocation attempt.", "ExtensionEventLocation", "Error");
                return new StatusCodeResult(500);
            }
        }
        [HttpPost("addlocationconnection")]
        [Authorize]
        public IActionResult addlocationconnection( [FromBody] ExtensionEventLocationConnection location){
            if(location != null){
                context.Add(location);  
                context.SaveChanges();
                this.Log(location,"ExtensionEventLocation", "ExtensionEventLocation Added.");
                return new OkObjectResult(location);
            }else{
                this.Log( location ,"ExtensionEventLocation", "Error in adding ExtensionEventLocation attempt.", "ExtensionEventLocation", "Error");
                return new StatusCodeResult(500);
            }
        }


        
        [HttpPut("updatelocation/{id}")]
        [Authorize]
        public IActionResult UpdateLocaton( int id, [FromBody] ExtensionEventLocation location){
            var lctn = context.ExtensionEventLocation
                        .Where( t => t.Id == id)
                        .Include( t => t.Address)
                        .FirstOrDefault();
            if(location != null && lctn != null ){
                lctn.Address.Building = location.Address.Building;
                lctn.Address.City = location.Address.City;
                lctn.Address.PostalCode = location.Address.PostalCode;
                lctn.Address.State = location.Address.State;
                lctn.Address.Street = location.Address.Street;
                context.SaveChanges();
                this.Log(location,"ExtensionEventLocation", "ExtensionEventLocation Updated.");
                return new OkObjectResult(location);
            }else{
                this.Log( location ,"ExtensionEventLocation", "Not Found ExtensionEventLocation in an update attempt.", "ExtensionEventLocation", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("countylocations/{id?}")]
        [Authorize]
        public IActionResult CountyLocations(int id = 0){
            if( id == 0){
                var user = CurrentUser();
                id = user.RprtngProfile.PlanningUnitId;
            }
            var locations = this._context
                            .ExtensionEventLocationConnection
                            .Where( e => e.PlanningUnitId == id)
                            .Include( e => e.ExtensionEventLocation)
                                .ThenInclude( l => l.Address);
            return new OkObjectResult(locations);
        }

        [HttpDelete("deletelocation/{id}")]
        [Authorize]
        public IActionResult DeleteLocation( int id ){
            var entity = context.ExtensionEventLocation.Find(id);
            if(entity != null){
                context.ExtensionEventLocation.Remove(entity);
                context.SaveChanges();
                this.Log(entity,"ExtensionEventLocation", "ExtensionEventLocation Deleted.");
                return new OkResult();
            }else{
                this.Log( id ,"ExtensionEventLocation", "Not Found ExtensionEventLocation in a delete attempt.", "ExtensionEventLocation", "Error");
                return new StatusCodeResult(500);
            }
        }

    }
 
}