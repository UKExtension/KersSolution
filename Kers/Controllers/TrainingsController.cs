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
    public class TrainingsController : ExtensionEventController
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



        [HttpGet]
        [Route("rangetrainings/{skip?}/{take?}/{order?}/{type?}")]
        public override IActionResult GetRange(int skip = 0, int take = 10, string order = "start", string type = "Training")
        {
            IQueryable<Training> query = _context.Training.Where( t => t.End != null );
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             
            query = query.Skip(skip).Take(take);
            query = query
                        .Include( e => e.submittedBy)
                        .ThenInclude( o => o.PersonalProfile);
            var list = query.ToList();
            return new OkObjectResult(list);
        }

        [HttpGet("perPeriod/{start}/{end}/{order?}/{type?}")]
        [Authorize]
        public override IActionResult PerPeriod(DateTime start, DateTime end, string order = "start", string type = "Training" ){
            IQueryable<Training> query = _context.Training.Where( t => t.Start > start && t.Start < end);
            
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
            query = query
                        .Include( e => e.submittedBy)
                        .ThenInclude( o => o.PersonalProfile);

            List<Training> list = query.ToList();

            var selected = list.Select( a => new {
                          Id =  a.Id,
                          submittedBy = a.submittedBy,
                          Subject = a.Subject,
                          Start = a.Start,
                          End = a.End,
                          tLocation = a.tLocation,
                          tTime = a.tTime         
            });
            return new OkObjectResult(selected);
        }



        [HttpPost("addtraining")]
        [Authorize]
        public IActionResult AddTraining( [FromBody] Training training){
            if(training != null){
                var user = this.CurrentUser();
                training.submittedBy = user;
                training.CreatedDateTime = DateTime.Now;
                training.LastModifiedDateTime = training.CreatedDateTime;
                training.Organizer = training.submittedBy;
                training.tStatus = "P";
                training.TrainDateBegin = training.Start.ToString("yyyyMMdd");
                training.TrainDateEnd = training.End?.ToString("yyyyMMdd");
                context.Add(training); 
                context.SaveChanges();
                this.Log(training,"Training", "Training Proposed.");
                return new OkObjectResult(training);
            }else{
                this.Log( training ,"Training", "Error in adding training attempt.", "Training", "Error");
                return new StatusCodeResult(500);
            }
        }





        [HttpGet("RegisterWindows")]
        public async Task<IActionResult> RegisterWindows(){
            var winds = await context.TainingRegisterWindow.ToListAsync();
            return new OkObjectResult(winds);
        }
        [HttpGet("InstructionalHours")]
        public async Task<IActionResult> InstructionalHours(){
            var hours = await context.TainingInstructionalHour.ToListAsync();
            return new OkObjectResult(hours);
        }
        [HttpGet("CancelEnrollmentWindows")]
        public async Task<IActionResult> CancelEnrollmentWindows(){
            var winds = await context.TrainingCancelEnrollmentWindow.ToListAsync();
            return new OkObjectResult(winds);
        }


        



    }
}