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
        public CountyEventController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo
            ):base(mainContext, context, userRepo){
                _context = context;
        }

        [HttpGet]
        [Route("range/{skip?}/{take?}/{order?}/{type?}")]
        public virtual IActionResult GetRange(int skip = 0, int take = 10, string order = "start", string type = "Training")
        {
            IQueryable<ExtensionEvent> query = _context.ExtensionEvent.Where( t => t.End != null && t.DiscriminatorValue == type);
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             
            query = query.Skip(skip).Take(take);
            query = query
                        .Include( e => e.Organizer)
                        .ThenInclude( o => o.PersonalProfile);
            var list = query.ToList();
            return new OkObjectResult(list);
        }


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


        [HttpPost("addcountyevent")]
        [Authorize]
        public IActionResult AddCountyEvent( [FromBody] CountyEvent CntEvent){
            if(CntEvent != null){
                this.context.Add(CntEvent);
                this.context.SaveChanges();
                return new OkObjectResult(CntEvent);
            }else{
                this.Log( CntEvent,"ExtensionEvent", "Error in adding extension event attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("updatecountyevent/{id}")]
        [Authorize]
        public IActionResult UpdateCountyEvent( int id, [FromBody] CountyEvent CntEvent){
           


            if(CntEvent != null ){
                
                
                this.Log(CntEvent,"ExtensionEvent", "ExtensionEvent Updated.");
                
                return new OkObjectResult(CntEvent);
            }else{
                this.Log( CntEvent ,"ExtensionEvent", "Not Found ExtensionEvent in an update attempt.", "ExtensionEvent", "Error");
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
    
}
