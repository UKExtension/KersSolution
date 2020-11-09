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

        [HttpGet("selected/{id}")]
        [Authorize]
        public IActionResult UpdatSelectionCount( int id){
            var lctn = context.ExtensionEventLocationConnection
                        .Where( t => t.Id == id)
                        .FirstOrDefault();
            if(lctn != null ){
                lctn.SelectedCount++;
                context.SaveChanges();
                this.Log(lctn,"ExtensionEventLocation", "ExtensionEventLocation Selection Couny Updated.", "ExtensionEventLocation");
                return new OkObjectResult(lctn);
            }else{
                this.Log( id ,"ExtensionEventLocation", "Not Found ExtensionEventLocation in an update count attempt.", "ExtensionEventLocation", "Error");
                return new StatusCodeResult(500);
            }
        }
        
        [HttpPut("updatelocation/{id}")]
        [Authorize]
        public IActionResult UpdateLocaton( int id, [FromBody] ExtensionEventLocationConnection location){
            var lctn = context.ExtensionEventLocationConnection
                        .Where( t => t.Id == id)
                        .Include( t => t.ExtensionEventLocation).ThenInclude( l => l.Address)
                        .FirstOrDefault();
            if(location != null && lctn != null ){
                lctn.ExtensionEventLocation.Address.Building = location.ExtensionEventLocation.Address.Building;
                lctn.ExtensionEventLocation.Address.City = location.ExtensionEventLocation.Address.City;
                lctn.ExtensionEventLocation.Address.PostalCode = location.ExtensionEventLocation.Address.PostalCode;
                lctn.ExtensionEventLocation.Address.State = location.ExtensionEventLocation.Address.State;
                lctn.ExtensionEventLocation.Address.Street = location.ExtensionEventLocation.Address.Street;
                lctn.ExtensionEventLocation.LocationUri = location.ExtensionEventLocation.LocationUri;
                lctn.ExtensionEventLocation.DisplayName = location.ExtensionEventLocation.DisplayName;
                context.SaveChanges();
                this.Log(location,"ExtensionEventLocation", "ExtensionEventLocation Updated.");
                return new OkObjectResult(location);
            }else{
                this.Log( location ,"ExtensionEventLocation", "Not Found ExtensionEventLocation in an update attempt.", "ExtensionEventLocation", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("countylocations/{id?}/{skip?}/{take?}/{includeCountyOffice?}/{order?}/{search?}")]
        [Authorize]
        public IActionResult CountyLocations(int id = 0, int skip = 0, int take = 10, bool includeCountyOffice = false, string order = "", string search = ""){
            if( id == 0){
                var user = CurrentUser();
                id = user.RprtngProfile.PlanningUnitId;
            }
            var locations = this._context
                            .ExtensionEventLocationConnection
                            .Where( e => e.PlanningUnitId == id);
            if(search != ""){
                locations = locations.Where( l => l.ExtensionEventLocation.Address.Building.Contains(search));
            }
            if(order=="often"){
                locations = locations.OrderByDescending( l => l.SelectedCount);
            }else{
                locations = locations.OrderBy(l => l.ExtensionEventLocation.Address.Building);
            }
            var res = new ExtensionEventLocationConnectionSearchResult();
            res.Results = locations.Include( e => e.ExtensionEventLocation)
                                .ThenInclude( l => l.Address).ToList();
            
            res.Count = res.Results.Count();
            res.Results = res.Results.Skip(skip).Take(take).ToList();
            if(includeCountyOffice){
                var county = this._context.PlanningUnit.Where( u => u.Id == id)
                                .Include( u => u.Location ).ThenInclude( l => l.Address )
                                .FirstOrDefault();
                if( county != null){
                    if( county.Location != null){
                        var loc = new ExtensionEventLocationConnection();
                        loc.ExtensionEventLocation = county.Location;
                        loc.PlanningUnit = county;
                        res.Results.Insert( 0, loc);
                        res.Count++;
                    }
                }
                    
                        

            }
            return new OkObjectResult(res);
        }

        [HttpGet("userlocations/{id?}/{skip?}/{take?}/{includeCountyOffice?}/{order?}/{search?}")]
        [Authorize]
        public IActionResult UserLocations(int id = 0, int skip = 0, int take = 10, bool includeCountyOffice = false, string order = "", string search = ""){
            KersUser user = null;
            if( id == 0){
                user = CurrentUser();
                id = user.Id;
            }
            var locations = this._context
                            .ExtensionEventLocationConnection
                            .Where( e => e.KersUserId == id);
            if(search != ""){
                locations = locations.Where( l => (l.ExtensionEventLocation.Address.Building.Contains(search) || l.ExtensionEventLocation.Address.Street.Contains(search)));
            }
            if(order=="often"){
                locations = locations.OrderByDescending( l => l.SelectedCount);
            }else{
                locations = locations.OrderBy(l => l.ExtensionEventLocation.Address.Building);
            }
            var res = new ExtensionEventLocationConnectionSearchResult();
            res.Results = locations.Include( e => e.ExtensionEventLocation)
                                .ThenInclude( l => l.Address).ToList();
            
            res.Count = res.Results.Count();
            res.Results = res.Results.Skip(skip).Take(take).ToList();
            if(includeCountyOffice && search == ""){
                if( user == null){
                    user = this._context.KersUser.Where( u => u.Id == id)
                                .Include( u => u.RprtngProfile)
                                .FirstOrDefault();
                }
                var county = this._context.PlanningUnit.Where( u => u.Id == user.RprtngProfile.PlanningUnitId )
                                .Include( u => u.Location ).ThenInclude( l => l.Address )
                                .FirstOrDefault();
                if( county != null){
                    if( county.Location != null){
                        var loc = new ExtensionEventLocationConnection();
                        loc.ExtensionEventLocation = county.Location;
                        loc.PlanningUnit = county;
                        res.Results.Insert( 0, loc);
                        res.Count++;
                    }
                }
                    
                        

            }
            return new OkObjectResult(res);
        }


        [HttpDelete("deletelocationconnection/{id}")]
        [Authorize]
        public IActionResult DeleteLocationConnnection( int id ){
            var entity = context.ExtensionEventLocationConnection.Where( c => c.Id == id)
                            .Include( c => c.ExtensionEventLocation ).ThenInclude( l => l.Address)
                            .FirstOrDefault();
            if(entity != null){
                context.Remove(entity.ExtensionEventLocation.Address);
                context.Remove( entity.ExtensionEventLocation);
                context.ExtensionEventLocationConnection.Remove(entity);
                context.SaveChanges();
                this.Log(entity,"ExtensionEventLocation", "ExtensionEventLocation Deleted.");
                return new OkResult();
            }else{
                this.Log( id ,"ExtensionEventLocation", "Not Found ExtensionEventLocation in a delete attempt.", "ExtensionEventLocation", "Error");
                return new StatusCodeResult(500);
            }
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

        public class ExtensionEventLocationConnectionSearchResult{
            public List<ExtensionEventLocationConnection> Results;
            public int Count;
        }

    }
 
}