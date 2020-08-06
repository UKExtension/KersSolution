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
        [Route("county/info/{id}/{conuntyId?}", Name="CountyEventDetails")]
        public async Task<ActionResult> CountyEvent(int id, int countyId = 0)
        {
            var evnt = await this.context.CountyEvent
                                .Where( e => e.Id == id )
                                .Include( e => e.Units).ThenInclude( u => u.PlanningUnit)
                                .Include( e => e.ProgramCategories).ThenInclude( c => c.ProgramCategory)
                                .FirstOrDefaultAsync();
            ViewData["unit"] = await this.context.PlanningUnit
                                        .Where( u => u.Id ==  (countyId != 0 ? countyId : evnt.Units.First().PlanningUnitId))
                                        .FirstOrDefaultAsync();
            return View(evnt);
        }

        [HttpGet]
        [Route("county/{id}")]
        public async Task<ActionResult> CountyEvents(int id)
        {
            ViewData["unit"] = await this.context.PlanningUnit.Where( u => u.Id == id).FirstOrDefaultAsync();
            var evnt = await this.context.CountyEvent
                                .Where( e => e.Units.Select( u => u.PlanningUnitId ).Contains(id) )
                                    .Include( e => e.Location ).ThenInclude( l => l.Address)
                                .OrderBy( e => e.Start)
                                .GroupBy( e => e.Start.Date)
                                .Select( d => new GroupedCountyEvents {
                                    events = d.ToList(),
                                    date = d.First().Start
                                })
                                
                                .ToListAsync();
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