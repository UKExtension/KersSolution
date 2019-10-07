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
}