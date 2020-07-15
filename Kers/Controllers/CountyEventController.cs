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
    public class CountyEventController : BaseController
    {

        KERScoreContext _context;
        private string DefaultTime = "12:34:56.1000000";
        public CountyEventController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo
            ):base(mainContext, context, userRepo){
                _context = context;
        }

        [HttpGet]
        [Route("range/{countyId?}/{skip?}/{take?}/{order?}")]
        public virtual IActionResult GetRange(int skip = 0, int take = 10, string order = "start", int countyId = 0)
        {
            if(countyId == 0 ){
                var user = CurrentUser();
                countyId = user.RprtngProfile.PlanningUnitId;
            }
            IQueryable<CountyEvent> query = _context.CountyEvent
                        .Where( e => e.Units.Select( u => u.PlanningUnitId).Contains(countyId))
                        .Include( e => e.Location).ThenInclude( l => l.Address)
                        .Include( e => e.Units)
                        .Include( e => e.ProgramCategories);
             
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             
            query = query.Skip(skip).Take(take);
            var reslt = new List<CountyEventWithTime>();
            foreach( var ce in query){
                var e = new CountyEventWithTime(ce);
                reslt.Add( e );
            }

            return new OkObjectResult(reslt);
        }
/* 

        [HttpGet("perPeriod/{start}/{end}/{order?}/{type?}")]
        [Authorize]
        public virtual IActionResult PerPeriod(DateTime start, DateTime end, string order = "start", string type = "Training" ){
            IQueryable<ExtensionEvent> query = _context.ExtensionEvent.Where( t => t.Start > start && t.Start < end && t.DiscriminatorValue == type);
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             

            var list = query.ToList();
            return new OkObjectResult(list);
        }
 */

        [HttpPost("addcountyevent")]
        [Authorize]
        public IActionResult AddCountyEvent( [FromBody] CountyEventWithTime CntEvent){
            if(CntEvent != null){
                CountyEvent evnt = new CountyEvent();
                evnt.Organizer = this.CurrentUser();
                evnt.CreatedDateTime = DateTimeOffset.Now;
                evnt.LastModifiedDateTime = DateTimeOffset.Now;
                var starttime = this.DefaultTime;
                evnt.IsAllDay = true;
                var timezone = CntEvent.Etimezone ? " -04:00":" -05:00";
                if(CntEvent.Starttime != ""){
                    starttime = CntEvent.Starttime+":00.1000000";
                    evnt.IsAllDay = false;
                    evnt.HasStartTime = true;
                } 
                evnt.Start = DateTimeOffset.Parse(CntEvent.Start.ToString("yyyy-MM-dd ") + starttime + timezone);
                
                if(CntEvent.End != null ){
                    var endtime = this.DefaultTime;
                    if(CntEvent.Endtime != ""){
                        endtime = CntEvent.Endtime+":00.1000000";
                        evnt.HasEndTime = true;
                    }
                    evnt.End = DateTimeOffset.Parse(CntEvent.End?.ToString("yyyy-MM-dd ") + endtime + timezone);
                }
                evnt.BodyPreview = evnt.Body = CntEvent.Body;
                evnt.WebLink = CntEvent.WebLink;
                evnt.Subject = CntEvent.Subject;
                evnt.Location = CntEvent.Location;
                evnt.Units = CntEvent.Units;
                evnt.ProgramCategories = CntEvent.ProgramCategories;
                this.context.Add(evnt);
                this.context.SaveChanges();
                return new OkObjectResult(evnt);
            }else{
                this.Log( CntEvent,"ExtensionEvent", "Error in adding extension event attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("updatecountyevent/{id}")]
        [Authorize]
        public IActionResult UpdateCountyEvent( int id, [FromBody] CountyEventWithTime CntEvent){
           
            var evnt = context.CountyEvent.Where(a => a.Id == id)
            
                        .Include(a => a.Location)
                        .Include( a => a.ProgramCategories)
                        .Include( a => a.Units)
                        .FirstOrDefault();

            if(CntEvent != null && evnt != null ){


                evnt.LastModifiedDateTime = DateTimeOffset.Now;
                var starttime = this.DefaultTime;
                evnt.IsAllDay = true;
                var timezone = CntEvent.Etimezone ? " -04:00":" -05:00";
                if(CntEvent.Starttime != ""){
                    starttime = CntEvent.Starttime+":00.1000000";
                    evnt.IsAllDay = false;
                    evnt.HasStartTime = true;
                }else{
                    evnt.HasEndTime = false;
                }
                evnt.Start = DateTimeOffset.Parse(CntEvent.Start.ToString("yyyy-MM-dd ") + starttime + timezone);
                
                if(CntEvent.End != null ){
                    var endtime = this.DefaultTime;
                    if(CntEvent.Endtime != ""){
                        endtime = CntEvent.Endtime+":00.1000000";
                        evnt.HasEndTime = true;
                    }else{
                        evnt.HasEndTime = false;
                    }
                    evnt.End = DateTimeOffset.Parse(CntEvent.End?.ToString("yyyy-MM-dd ") + endtime + timezone);
                }else{
                    evnt.End = null;
                }
                evnt.BodyPreview = evnt.Body = CntEvent.Body;
                evnt.WebLink = CntEvent.WebLink;
                evnt.Subject = CntEvent.Subject;

                context.Remove( evnt.Location );

                //context.Remove( evnt.Units );
                //context.Remove( evnt.ProgramCategories );


                evnt.Location = CntEvent.Location;
                evnt.Units = CntEvent.Units;
                evnt.ProgramCategories = CntEvent.ProgramCategories;


                
                
                this.Log(CntEvent,"CountyEvent", "County Event Updated.");
                
                return new OkObjectResult(CntEvent);
            }else{
                this.Log( CntEvent ,"CountyEvent", "Not Found CountyEvent in an update attempt.", "CountyEvent", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("deletecountyevent/{id}")]
        [Authorize]
        public IActionResult DeleteExtensionEvent( int id ){
            var entity = context.ExtensionEvent.Find(id);
            
            
            if(entity != null){
                
                context.ExtensionEvent.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"ExtensionEvent", "ExtensionEvent Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ExtensionEvent", "Not Found ExtensionEvent in a delete attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }

        
        public IActionResult Error()
        {
            return View();
        }


    }

    public class CountyEventWithTime:CountyEvent{
        public string Starttime;
        public string Endtime;
        public bool Etimezone;

        public CountyEventWithTime(){}

        public CountyEventWithTime(CountyEvent m){
            this.Id = m.Id;
            this.Body = m.Body;
            this.BodyPreview = m.BodyPreview;
            this.Start = m.Start;
            this.End = m.End;
            this.IsAllDay = m.IsAllDay;
            this.CreatedDateTime = m.CreatedDateTime;
            this.LastModifiedDateTime = m.LastModifiedDateTime;
            this.Subject = m.Subject;
            this.IsCancelled = m.IsCancelled;
            this.HasEndTime = m.HasEndTime;
            this.HasStartTime = m.HasStartTime;
            this.Units = m.Units;
            this.ProgramCategories = m.ProgramCategories;
            this.WebLink = m.WebLink;
            this.Location = m.Location;

            if(m.End != null){
                TimeSpan tmzn = m.Start.Offset;
                var hrs = tmzn.TotalHours;
                this.Etimezone = hrs == -4;
                if(this.End.HasValue ){
                    this.Endtime = this.End.Value.Hour.ToString("D2")+ ":" + this.End.Value.Minute.ToString("D2");
                }
                
            }
            this.Starttime = this.Start.Hour.ToString("D2") + ":" + this.Start.Minute.ToString("D2");

        }


    }
    
}
