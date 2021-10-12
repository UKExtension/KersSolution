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
        public IActionResult UpdateCountyEvent( int id, [FromBody] ActivitySignUpEntry SignUp ){
           /* 
            var evnt = context.CountyEvent.Where(a => a.Id == id)
            
                        .Include(a => a.Location)
                        .Include( a => a.ProgramCategories)
                        .Include( a => a.Units)
                        .Include( a => a.ExtensionEventImages)
                        .FirstOrDefault();

            if(CntEvent != null && evnt != null ){


                evnt.LastModifiedDateTime = DateTimeOffset.Now;
                var starttime = this.DefaultTime;
                evnt.IsAllDay = true;
                var timezone = CntEvent.Etimezone ? " -04:00":" -05:00";
                if(CntEvent.Starttime != null && CntEvent.Starttime != ""){
                    starttime = CntEvent.Starttime+":00.1000000";
                    evnt.IsAllDay = false;
                    evnt.HasStartTime = true;
                }else{
                    evnt.HasEndTime = false;
                }
                evnt.Start = DateTimeOffset.Parse(CntEvent.Start.ToString("yyyy-MM-dd ") + starttime + timezone);
                
                if(CntEvent.End != null ){
                    var endtime = this.DefaultTime;
                    if(CntEvent.Endtime != ""){
                        endtime = CntEvent.Endtime+":00.1000000";
                        evnt.HasEndTime = true;
                    }else{
                        evnt.HasEndTime = false;
                    }
                    evnt.End = DateTimeOffset.Parse(CntEvent.End?.ToString("yyyy-MM-dd ") + endtime + timezone);
                }else{
                    evnt.End = null;
                }
                evnt.BodyPreview = evnt.Body = CntEvent.Body;
                evnt.WebLink = CntEvent.WebLink;
                evnt.Subject = CntEvent.Subject;
                context.RemoveRange( evnt.ProgramCategories );
                if(evnt.Location != null) context.Remove( evnt.Location );
                evnt.Location = CntEvent.Location;
                CntEvent.ExtensionEventImages.ForEach(s => s.Created = DateTime.Now);
                if( evnt.ExtensionEventImages == null){
                    evnt.ExtensionEventImages = CntEvent.ExtensionEventImages;
                }else{
                    evnt.ExtensionEventImages.AddRange(CntEvent.ExtensionEventImages);
                }
                evnt.Units = CntEvent.Units;
                evnt.ProgramCategories = CntEvent.ProgramCategories;
                this.Log(CntEvent,"CountyEvent", "County Event Updated.", "CountyEvent");
                
                return new OkObjectResult(new CountyEventWithTime(evnt));
            }else{
                this.Log( CntEvent ,"CountyEvent", "Not Found CountyEvent in an update attempt.", "CountyEvent", "Error");
                return new StatusCodeResult(500);
            }


             */
             return new OkObjectResult(null);
        }

        [HttpDelete("deletecountyevent/{id}")]
        [Authorize]
        public IActionResult DeleteExtensionEvent( int id ){
            
            
            var entity = context.CountyEvent.Where(c => c.Id == id)
                            .Include( c => c.Units )
                            .Include( c => c.Location)
                            .Include( c => c.ExtensionEventImages)
                            .FirstOrDefault();
            if(entity != null){
                this.context.RemoveRange( this.context.CountyEventProgramCategory.Where(c => c.CountyEventId == id));
                foreach( var im in entity.ExtensionEventImages ){
                    var imgs = this.context.UploadImage.Where( i => i.Id == im.UploadImageId).First();
                    this.context.Remove( this.context.UploadFile.Where( f => f.Id == imgs.UploadFileId).First());
                    this.context.Remove( imgs );
                } 
                this.context.RemoveRange( entity.ExtensionEventImages);
                if( entity.Location != null ) context.Remove(entity.Location);
                context.CountyEvent.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"CountyEvent", "CountyEvent Removed.", "CountyEvent");

                return new OkResult();
            }else{
                this.Log( id ,"CountyEvent", "Not Found CountyEvent in a delete attempt.", "CountyEvent", "Error");
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
            
            string result;
            var keys = new List<string>();
            keys.Add("Name");
            keys.Add("Address");
            keys.Add("Email");
            keys.Add("Race");
            keys.Add("Ethnicity");
            keys.Add("Gender");
            result = string.Join(",", keys.ToArray()) + "\n";

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


            return Ok(result);
        }



    }
}
