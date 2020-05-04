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
    public class LadderController : BaseController
    {

        KERScoreContext _context;
        public LadderController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo
            ):base(mainContext, context, userRepo){
                _context = context;
        }


        [HttpGet("levels")]
        [Authorize]
        public async Task<IActionResult> Levels(){
            var levels = context.LadderLevel.OrderBy( o => o.Order);
            return new OkObjectResult(await levels.ToListAsync());
        }
        [HttpGet("educationlevels")]
        [Authorize]
        public async Task<IActionResult> EducationLevels(){
            var levels = context.LadderEducationLevel.OrderBy( o => o.Order);
            return new OkObjectResult(await levels.ToListAsync());
        }


        [HttpPost("addladder")]
        [Authorize]
        public IActionResult AddLadderApplication( [FromBody] LadderApplication LadderApplication){
            if(LadderApplication != null){
                
                this.context.Add(LadderApplication);
                this.context.SaveChanges();
                return new OkObjectResult(LadderApplication);
            }else{
                this.Log( LadderApplication,"LadderApplication", "Error in adding LadderApplication attempt.", "LadderApplication", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("updateladder/{id}")]
        [Authorize]
        public IActionResult UpdateLadderApplication( int id, [FromBody] LadderApplication LadderApplication){
           


            if(LadderApplication != null ){
                if(LadderApplication.KersUserId == 0){
                    var user = this.CurrentUser();
                    LadderApplication.KersUserId = user.Id;
                }
                
                this.Log(LadderApplication,"LadderApplication", "LadderApplication Updated.");
                
                return new OkObjectResult(LadderApplication);
            }else{
                this.Log( LadderApplication ,"LadderApplication", "Not Found LadderApplication in an update attempt.", "LadderApplication", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("deleteladder/{id}")]
        [Authorize]
        public IActionResult DeleteLadderApplication( int id ){
            //var entity = context.LadderApplication.Find(id);
            
            /* 
            if(entity != null){
                
                context.ExtensionEvent.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"ExtensionEvent", "ExtensionEvent Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ExtensionEvent", "Not Found ExtensionEvent in a delete attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            } */
            return new OkResult();
        }

       
        public IActionResult Error()
        {
            return View();
        }


    }
    
}
