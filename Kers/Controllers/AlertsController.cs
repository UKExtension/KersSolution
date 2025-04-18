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
using Kers.Models.Data;
using Kers.Models.ViewModels;
using System.Web;
using Microsoft.Extensions.Caching.Memory;

namespace Kers.Controllers.Reports
{

    [Route("api/[controller]")]
    public class AlertsController : BaseController
    {
        ILogRepository logRepo;
        public AlertsController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    IMemoryCache memoryCache
                    
            ):base(mainContext, context, userRepo, memoryCache){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.logRepo = logRepo;
        }

        

        [HttpGet]
        [Route("routes")]
        public async Task<IActionResult> Routes()
        {
            var routes = await this.context.AlertRoute.OrderBy( rt => rt.Id).ToListAsync();
           
            return new OkObjectResult(routes);
        }

        [HttpGet]
        [Route("getAlerts/{filter}")]
        // filter: 0 all, 1 active, 2 past
        public async Task<IActionResult> GetAlerts(int filter)
        {
            IQueryable<Alert> alerts = this.context.Alert;
            var now = DateTime.Now;
            if(filter == 1){
                alerts = alerts.Where( a => a.Start < now && a.End > now );
            }else if(filter == 2){
                alerts = alerts.Where( a => a.Start > now || a.End < now );
            }
           
            return new OkObjectResult(await alerts.OrderBy( rt => rt.Start).ToListAsync());
        }
        [HttpPost]
        [Route("getPageAlerts")]
        public async Task<IActionResult> GetPage([FromBody] RouteObject rt)
        {
            IQueryable<Alert> alerts = this.context.Alert.Where( a => a.UrlRoute == rt.route && a.Active == true);
            var now = DateTime.Now;
            alerts = alerts.Where( a => a.Start < now && a.End > now && a.Active );
            List<Alert> alertsByDateRange = await alerts.OrderBy( rt => rt.Start).ToListAsync();
            List<Alert> filteredAlerts = new List<Alert>();
            if( alertsByDateRange.Count() > 0 ){
                KersUser user = CurrentUser();
                var PlanningUnit = context.PlanningUnit.Find(user.RprtngProfile.PlanningUnitId);
                foreach( var alert in alertsByDateRange){
                    if( alert.EmployeePositionId != null && user.ExtensionPositionId != alert.EmployeePositionId ) break;
                    if( alert.isContyStaff != null){
                        if(PlanningUnit.ExtensionAreaId == null && alert.isContyStaff == 1) break;
                        if(PlanningUnit.ExtensionAreaId != null && alert.isContyStaff == 2) break;
                    }
                    if( alert.zEmpRoleTypeId != null ){
                        if( !context.zEmpProfileRole.Where( r => r.Id == alert.zEmpRoleTypeId && r.UserId == user.Id).Any()) break;
                    }
                    if(alert.PlanningUnitId != null && alert.PlanningUnitId != PlanningUnit.Id ) break;
                    if( alert.ExtensionAreaId != null && alert.ExtensionAreaId != PlanningUnit.ExtensionAreaId ) break;
                    if( alert.ExtensionRegionId != null && PlanningUnit.ExtensionAreaId != null){
                        var Area = this.context.ExtensionArea.Find( PlanningUnit.ExtensionAreaId );
                        if( Area.ExtensionRegionId != alert.ExtensionRegionId ) break;
                    }
                    filteredAlerts.Add(alert);
                }
            }
            

            return new OkObjectResult(filteredAlerts);
        }





        [HttpPost()]
        [Authorize]
        public IActionResult AddAlert( [FromBody] Alert alert){
            if(alert != null){
                alert.Start = new DateTime( alert.Start.Year, alert.Start.Month, alert.Start.Day, 7, 0, 0 );
                alert.End = new DateTime( alert.End.Year, alert.End.Month, alert.End.Day, 18, 0, 0 );
                alert.Created = DateTime.Now;
                alert.LastUpdated = alert.Created;
                alert.CreatedBy = CurrentUser();
                context.Add(alert); 
                context.SaveChanges(); 
                this.Log(alert,"Alert", "Alert Added.", "Alert", "Created Alert Record");
                return new OkObjectResult(alert);
            }else{
                this.Log( alert ,"Alert", "Error in adding alert attempt.", "Alert", "Error");
                return new StatusCodeResult(500);
            }
        }




        [HttpPut("{id}")]
        public IActionResult UpdateAlert( int id, [FromBody] Alert alert){
            var entity = context.Alert.Find(id);
            if(entity != null ){
                entity.Start = new DateTime( alert.Start.Year, alert.Start.Month, alert.Start.Day, 7, 0, 0 );
                entity.End = new DateTime( alert.End.Year, alert.End.Month, alert.End.Day, 18, 0, 0 );
                entity.Message = alert.Message;
                entity.MoreInfoUrl = alert.MoreInfoUrl;
                entity.EmployeePositionId = alert.EmployeePositionId;
                entity.AlertType = alert.AlertType;
                entity.UrlRoute = alert.UrlRoute;
                entity.zEmpRoleTypeId = alert.zEmpRoleTypeId;
                entity.isContyStaff = alert.isContyStaff;
                entity.Active = alert.Active;
                entity.ExtensionAreaId = alert.ExtensionAreaId;
                entity.ExtensionRegionId = alert.ExtensionRegionId;
                entity.PlanningUnitId = alert.PlanningUnitId;
                entity.CreatedBy = CurrentUser();
                entity.LastUpdated = DateTime.Now;
                context.SaveChanges();
                this.Log(entity,"Alert", "Alert Updated.");
                return new OkObjectResult(entity);
            }else{
                this.Log( alert ,"Alert", "Not Found Alert in update attempt.", "Alert", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAlert( int id ){
            var entity = context.Alert.Find(id);
            
            if(entity != null){

                context.Alert.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"Alert", "Alert Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"Alert", "Not Found Alert in delete attempt.", "Alert", "Error");
                return new StatusCodeResult(500);
            }
        }





    }

    public class RouteObject{
        public string route;
    }
}