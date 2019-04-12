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


        [HttpPost("addtraining")]
        [Authorize]
        public IActionResult AddTraining( [FromBody] Training training){
            if(training != null){



                var user = this.CurrentUser();

/* 

                var cnt = new Contact();
                cnt.KersUser = user;
                cnt.Created = DateTime.Now;
                cnt.Updated = DateTime.Now;
                cnt.MajorProgramId = contact.MajorProgramId;
                cnt.Days = contact.Days;
                cnt.Audience = contact.Male + contact.Female;
                cnt.ContactDate = contact.ContactDate;
                cnt.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                contact.Created = DateTime.Now;
                cnt.Revisions = new List<ContactRevision>();
                cnt.Revisions.Add(contact);
                context.Add(cnt); 
                this.Log(contact,"ContactRevision", "Statistical Contact Added.");
                context.SaveChanges();
                contact.MajorProgram = this.context.MajorProgram
                                        .Where( m => m.Id == contact.MajorProgramId)
                                        .Include( m => m.StrategicInitiative).ThenInclude( i => i.FiscalYear )
                                        .FirstOrDefault();


                 */
                
                return new OkObjectResult(training);
            }else{
                this.Log( training ,"ContactRevision", "Error in adding statistical contact attempt.", "Activity", "Error");
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