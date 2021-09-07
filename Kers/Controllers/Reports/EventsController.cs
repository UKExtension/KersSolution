using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Contexts;
using KersData.Models;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class EventsController : Controller
    {
        KERScoreContext context;
        IKersUserRepository userRepo;

        public EventsController( 
                    KERScoreContext context,
                    IKersUserRepository userRepo
            ){
           this.context = context;
           this.userRepo = userRepo;
        }


        [HttpGet]
        [Route("{fy?}")]
        public async Task<ActionResult> Index(string fy="0")
        {
            ViewData["fy"] = fy;
            var start = new DateTimeOffset(DateTime.Now);

            var events = await this.context.ExtensionEvent
                                .Where( e => e.Start > start )
                                .OrderBy(e => e.Start)
                                .ToListAsync();

            var result = new List<ExtensionEvent>();
            foreach( var e in events ){
                if( e.DiscriminatorValue == "Training"){
                    var training = context.Training.Find(e.Id);
                    if(training != null){
                        if(training.tStatus == "A"){
                            result.Add(e);
                        }
                    }

                }else{
                    result.Add(e);
                }
            }

            return View(result);
        }

        [HttpGet]
        [Route("county/info/{id}/{countyId?}", Name="CountyEventDetails")]
        public async Task<ActionResult> CountyEvent(int id, int countyId = 0)
        {
            var evnt = await this.context.CountyEvent
                                .Where( e => e.Id == id )
                                .Include( e => e.Location).ThenInclude( l => l.Address)
                                .Include( e => e.Units).ThenInclude( u => u.PlanningUnit)
                                .Include( e => e.ProgramCategories).ThenInclude( c => c.ProgramCategory)
                                .Include( e => e.ExtensionEventImages)
                                .FirstOrDefaultAsync();
            int cntId =  (countyId != 0 ? countyId : evnt.Units.First().PlanningUnitId);
            ViewData["unit"] = await this.context.PlanningUnit
                                        .Where( u => u.Id == cntId )
                                        .FirstOrDefaultAsync();
            return View(evnt);
        }


        [HttpGet]
        [Route("redirect/{code}", Name="RedirectCountyEvents")]
        public ActionResult RedirectCountyEvents(string code)
        {
            var unit = this.context.PlanningUnit.ToList();
            unit = unit.Where( u => u.Code.Substring(2) == code).FirstOrDefault();
            if( unit == null ) return RedirectToAction("CountyEvents", "Events");
            return RedirectToAction("CountyEvents", "Events", new {id = unit.Id});
        }
        [HttpGet]
        [Route("redirectevent/{code}", Name="RedirectCountyEvent")]
        public ActionResult RedirectCountyEvent(int code)
        {
            var evnt = this.context.CountyEvent.Where( u => u.classicCountyEventId == code).FirstOrDefault();
            if( evnt == null ) return RedirectToAction("CountyEvents", "Events");
            return RedirectToAction("CountyEvent", "Events", new {id = evnt.Id});
        }

        [HttpGet]
        [Route("county/{id?}/{upcomming?}", Name="CountyEvents")]
        public async Task<ActionResult> CountyEvents(int id = 0, int upcomming = 1)
        {
            ViewData["unit"] = null;
            if( id != 0){
                var unit = await this.context.PlanningUnit.Where( u => u.Id == id).FirstOrDefaultAsync();
                if(unit != null){
                    ViewData["unit"] = unit;
                }else{
                    id = 0;
                }
            }

            var filteredEvents = await this.context.CountyEvent
                                .Where( e => 
                                                (id != 0 
                                                        ?
                                                        e.Units.Select( u => u.PlanningUnitId ).Contains(id) 
                                                        :
                                                        true
                                                )
                                                &&
                                                (upcomming == 1 ? 
                                                    e.Start.Date >= DateTime.Now.Date
                                                    :
                                                    e.Start.Date < DateTime.Now.Date
                                                    )
                                                
                                                )
                                    .Include( e => e.Location ).ThenInclude( l => l.Address)
                                .OrderBy( e => e.Start).ToListAsync();
            
            var evnt = filteredEvents
                                .GroupBy( e => e.Start.Date)
                                .Select( d => new GroupedCountyEvents {
                                    events = d.ToList(),
                                    date = d.First().Start
                                })
                                .Take( 100 )
                                .ToList();
            return View(evnt);
        }

        [HttpGet]
        [Route("calendar/{fy?}")]
        public ActionResult Calendar(string fy="0")
        {
            ViewData["fy"] = fy;

/* 
            var start = new DateTimeOffset(DateTime.Now);

            var events = await this.context.ExtensionEvent
                                .Where( e => e.Start > start )
                                .OrderBy(e => e.Start)
                                .ToListAsync();

            var result = new List<ExtensionEvent>();
            foreach( var e in events ){
                if( e.DiscriminatorValue == "Training"){
                    var training = context.Training.Find(e.Id);
                    if(training != null){
                        if(training.tStatus == "A"){
                            result.Add(e);
                        }
                    }

                }else{
                    result.Add(e);
                }
            }
 */
            return View();
        }





    }

    public class GroupedCountyEvents{
        public List<CountyEvent> events;
        public DateTimeOffset date;
    }
}