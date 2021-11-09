using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.UKCAReporting;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace Kers.Controllers
{
    [Route("api/[controller]")]
    public class SignUpController : BaseController
    {

        KERScoreContext _context;
        KERSreportingContext _reportingContext;
        IWebHostEnvironment environment;
        public SignUpController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    KERSreportingContext reportingContext,
                    IWebHostEnvironment env
            ):base(mainContext, context, userRepo){
                _context = context;
                _reportingContext = reportingContext;
                this.environment = env;
        }


        [HttpPost("add")]
        [Authorize]
        public IActionResult Add( [FromBody] ActivitySignUpEntry SignUp){
            if(SignUp != null){
                SignUp.Created = DateTime.Now;
                SignUp.Updated = SignUp.Created;
                this.context.Add(SignUp);
                this.context.SaveChanges();
                this.Log(SignUp,"ActivitySignUpEntry", "Sign Up Added.", "ActivitySignUpEntry");
                return new OkObjectResult(SignUp);
            }else{
                this.Log( SignUp ,"ActivitySignUpEntry", "Error in adding Activity Sign Up attempt.", "ActivitySignUpEntry", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("update/{id}")]
        [Authorize]
        public IActionResult Update( int id, [FromBody] ActivitySignUpEntry SignUp ){
            var entry = context.ActivitySignUpEntry.Find(id);
            if(entry != null && SignUp != null){
                entry.Name = SignUp.Name;
                entry.Address = SignUp.Address;
                entry.RaceId = SignUp.RaceId;
                entry.Email = SignUp.Email;
                entry.EthnicityId = SignUp.EthnicityId;
                entry.Gender = SignUp.Gender;
                entry.Updated = DateTime.Now;
                this.Log(entry,"ActivitySignUpEntry", "Activity Sign Up Entry Updated.", "ActivitySignUpEntry");
                
                return new OkObjectResult(entry);
            }else{
                this.Log( entry ,"ActivitySignUpEntry", "Not Found ActivitySignUpEntry in an update attempt.", "ActivitySignUpEntry", "Error");
                return new StatusCodeResult(500);
            }     
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public IActionResult DeleteExtensionEvent( int id ){
            
            
            var entry = context.ActivitySignUpEntry.Find(id);
            if(entry != null){

                context.ActivitySignUpEntry.Remove(entry);
                //context.SaveChanges();
                
                this.Log(entry,"ActivitySignUpEntry", "ActivitySignUpEntry Removed.", "ActivitySignUpEntry");

                return new OkResult();
            }else{
                this.Log( id ,"ActivitySignUpEntry", "Not Found ActivitySignUpEntry in a delete attempt.", "ActivitySignUpEntry", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpGet("hasattendance/{ActivityId}")]
        public IActionResult HasAttendance(int ActivityId){
            var isAttended = this._context.ActivitySignUpEntry.Where(e => e.ActivityId == ActivityId).Any();
            return new OkObjectResult(isAttended);
        }

        [HttpGet("attendedby/{ActivityId}")]
        public async Task<IActionResult> Attendance(int ActivityId){
            var Attended = this._context.ActivitySignUpEntry.Where(e => e.ActivityId == ActivityId);
            return new OkObjectResult(await Attended.ToListAsync());
        }


        [HttpGet]
        [Route("attendiescsv/{activityId}/data.csv")]
        [Authorize]
        public IActionResult ListCSV(int activityId){
            var activity = this.context.Activity.Where( a => a.Id == activityId)
                                    .Include( a => a.LastRevision)
                                    .FirstOrDefault();
            if( activity != null ){
                string result = activity.LastRevision.Title + "\n";
                result += activity.LastRevision.ActivityDate.ToShortDateString() + "\n\n";
                var keys = new List<string>();
                keys.Add("Name");
                keys.Add("Address");
                keys.Add("Email");
                keys.Add("Race");
                keys.Add("Ethnicity");
                keys.Add("Gender");
                result += string.Join(",", keys.ToArray()) + "\n";

                var attendies = this._context.ActivitySignUpEntry
                                    .Where(s => s.ActivityId == activityId)
                                        .Include( s => s.Race)
                                        .Include( s => s.Ethnicity);
                foreach( var attendie in attendies){
                    var row = new List<string>();
                    row.Add(attendie.Name);
                    row.Add( attendie.Address);
                    row.Add( attendie.Email);
                    row.Add( attendie.Race != null ? attendie.Race.Name : "");
                    row.Add( attendie.Ethnicity != null ? attendie.Ethnicity.Name : "");
                    row.Add( (attendie.Gender == 1?"Male":attendie.Gender == 2 ? "Female" : "" ));
                    result += string.Join(",", row.ToArray()) + "\n";
                }
                this.Log(attendies,"ActivitySignUpEntry", "ActivitySignUpEntry CSV Created.", "ActivitySignUpEntry");
                return Ok(result);
            }else{
                this.Log( activityId ,"ActivitySignUpEntry", "Not Found Activity in CSV creation attempt.", "ActivitySignUpEntry", "Error");
                return new StatusCodeResult(500);
            }
        }
    }
}
