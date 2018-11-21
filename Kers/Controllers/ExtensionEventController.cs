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
    public class ExtensionEventController : BaseController
    {

        KERScoreContext _context;
        public ExtensionEventController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo
            ):base(mainContext, context, userRepo){
                _context = context;
        }




        [HttpPost()]
        [Authorize]
        public IActionResult AddExtensionEvent( [FromBody] ExtensionEvent ExEvent){
            if(ExEvent != null){
                
                return new OkObjectResult(ExEvent);
            }else{
                this.Log( ExEvent,"ExtensionEvent", "Error in adding extension event attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateExtensionEvent( int id, [FromBody] ExtensionEvent ExEvent){
           


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
