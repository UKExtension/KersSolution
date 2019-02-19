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
using System.Net.Http;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class TrainingsController : BaseController
    {
        KERScoreContext _context;
        KERSmainContext _mainContext;
        IKersUserRepository _userRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        public TrainingsController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    IFiscalYearRepository fiscalYearRepo
            ):base(mainContext, context, userRepo){
           this._context = context;
           this._mainContext = mainContext;
           this._userRepo = userRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
        }



        [HttpGet]
        [Route("range/{skip?}/{take?}/{order?}")]
        public IActionResult GetRange(int skip = 0, int take = 10, string order = "start")
        {
            IQueryable<ExtensionEvent> query = _context.ExtensionEvent.Where( t => t.End != null);
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             
            query = query.Skip(skip).Take(take);

            var list = query.ToList();
            return new OkObjectResult(list);
        }


        

        [HttpPost()]
        [Authorize]
        public IActionResult AddTraining( [FromBody] Training ExEvent){
            if(ExEvent != null){
                
                return new OkObjectResult(ExEvent);
            }else{
                this.Log( ExEvent,"ExtensionEvent", "Error in adding extension event attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateTraining( int id, [FromBody] Training ExEvent){
           


            if(ExEvent != null ){
                
                this.Log(ExEvent,"ExtensionEvent", "ExtensionEvent Updated.");
                
                return new OkObjectResult(ExEvent);
            }else{
                this.Log( ExEvent ,"ExtensionEvent", "Not Found ExtensionEvent in an update attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteTraining( int id ){
            var entity = context.Training.Find(id);
            
            
            if(entity != null){
                
                context.Training.Remove(entity);
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