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

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        public ContactController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    IFiscalYearRepository fiscalYearRepo
            ){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
        }

        [HttpGet("numb")]
        [Authorize]
        public IActionResult GetNumb(){
            
            var numContacts = context.Contact.
                                Where(e=>e.KersUser == this.CurrentUser());
            
            return new OkObjectResult(numContacts.Count());
        }

        [HttpGet("latest/{skip?}/{amount?}")]
        [Authorize]
        public IActionResult Get(int skip = 0, int amount = 10){
            
            var lastContacts = context.Contact.
                                Where(e=>e.KersUser == this.CurrentUser()).
                                OrderByDescending(e=>e.ContactDate).
                                Include(e=>e.Revisions).ThenInclude(r => r.ContactOptionNumbers).
                                Include(e=>e.Revisions).ThenInclude(r => r.ContactRaceEthnicityValues).ThenInclude(r => r.Race).
                                Include(e=>e.Revisions).ThenInclude(r => r.ContactRaceEthnicityValues).ThenInclude(r => r.Ethnicity).
                                Include(e=>e.Revisions).ThenInclude(r => r.MajorProgram).ThenInclude( m => m.StrategicInitiative).ThenInclude( i => i.FiscalYear).
                                Skip(skip).
                                Take(amount);
            
            var revs = new List<ContactRevision>();
            if(lastContacts != null){
                foreach(var contact in lastContacts){
                    revs.Add( contact.Revisions.OrderBy(r=>r.Created).Last() );
                }
                foreach( var a in revs){
                    a.ContactRaceEthnicityValues = a.ContactRaceEthnicityValues.
                                                OrderBy(r => r.Race.Order).
                                                ThenBy(e => e.Ethnicity.Order).
                                                ToList();
                }
            }
            return new OkObjectResult(revs);
        }



        [HttpGet("perPeriod/{start}/{end}/{userid?}")]
        [Authorize]
        public IActionResult PerPeriod(DateTime start, DateTime end, int userid = 0 ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            end = end.AddDays(1);
            var lastActivities = context.Contact.
                                Where(a=>a.KersUser.Id == userid & a.ContactDate > start & a.ContactDate < end).
                                Include(e=>e.Revisions).ThenInclude(r => r.MajorProgram).
                                Include(e=>e.Revisions).ThenInclude(r => r.ContactOptionNumbers).ThenInclude(n => n.ActivityOptionNumber).
                                Include(e=>e.Revisions).ThenInclude(r => r.ContactRaceEthnicityValues).
                                OrderByDescending(a => a.ContactDate);
            var revs = new List<ContactRevision>();
            if( lastActivities != null){
                foreach(var activity in lastActivities){
                    if(activity.Revisions.Count != 0){
                        revs.Add( activity.Revisions.OrderBy(r=>r.Created).Last() );
                    }
                }
            }

            return new OkObjectResult(revs);
        }



        [HttpGet("summaryPerMonth/{userid?}")]
        [Authorize]
        public IActionResult summaryPerMonth(int userid = 0 ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            var numPerMonth = context.Contact.
                                Where(a=>a.KersUser.Id == userid).
                                GroupBy(e => new {
                                    Month = e.ContactDate.Month,
                                    Year = e.ContactDate.Year
                                }).
                                Select(c => new {
                                    Revisions = c.Select(
                                                        s => new {
                                                            Revs = s.
                                                            Revisions.
                                                            OrderBy(f=>f.Created).
                                                            Last()
                                                        }
                                                ),
                                    Races = c.Select(
                                                        s => new {
                                                            Revs = s.
                                                            Revisions.
                                                            OrderBy(f=>f.Created).
                                                            Last().ContactRaceEthnicityValues
                                                        }
                                                ),
                                    OptionNumbers = c.
                                                    Select(
                                                        s => new {
                                                            Revs = s.
                                                            Revisions.
                                                            OrderBy(f=>f.Created).
                                                            Last().ContactOptionNumbers
                                                        }
                                                ),
                                    Days = c.Sum(s => s.Revisions.OrderBy(r => r.Created).Last().Days),
                                    Multistate = c.Sum(s => s.Revisions.OrderBy(r => r.Created).Last().Multistate),
                                    Males = c.Sum(s => s.Revisions.OrderBy(r => r.Created).Last().Male),
                                    Females = c.Sum(s => s.Revisions.OrderBy(r => r.Created).Last().Female),
                                    Month = c.Key.Month,
                                    Year = c.Key.Year
                                }).
                                OrderByDescending(e => e.Year).ThenByDescending(e => e.Month);
            return new OkObjectResult(numPerMonth);
        }





        [HttpGet("summaryPerProgram/{userid?}")]
        [Authorize]
        public IActionResult summaryPerProgram(int userid = 0 ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            var numPerMonth = context.Contact.
                                Where(a=>a.KersUser.Id == userid).
                                GroupBy(e => new {
                                    MajorProgram = e.Revisions.OrderBy(f=>f.Created).Last().MajorProgramId
                                }).
                                Select(c => new {
                                    Races = c.Select(
                                                        s => new {
                                                            Revs = s.
                                                            Revisions.
                                                            OrderBy(f=>f.Created).
                                                            Last().ContactRaceEthnicityValues
                                                        }
                                                ),
                                    OptionNumbers = c.
                                                    Select(
                                                        s => new {
                                                            Revs = s.
                                                            Revisions.
                                                            OrderBy(f=>f.Created).
                                                            Last().ContactOptionNumbers
                                                        }
                                                ),
                                    Days = c.Sum(s => s.Revisions.OrderBy(r => r.Created).Last().Days),
                                    Multistate = c.Sum(s => s.Revisions.OrderBy(r => r.Created).Last().Multistate),
                                    Males = c.Sum(s => s.Revisions.OrderBy(r => r.Created).Last().Male),
                                    Females = c.Sum(s => s.Revisions.OrderBy(r => r.Created).Last().Female),
                                    Program = c.
                                                    Select(
                                                        s => new {
                                                            Progr = s.
                                                            Revisions.
                                                            OrderBy(f=>f.Created).
                                                            Last().MajorProgram
                                                        }
                                                )
                                }).
                                OrderBy( s => s.Program.First().Progr.Name);
            return new OkObjectResult(numPerMonth);
        }





        [HttpPost()]
        [Authorize]
        public IActionResult AddContact( [FromBody] ContactRevision contact){
            if(contact != null){
                var user = this.CurrentUser();
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
                cnt.LastRevisionId = contact.Id;
                context.SaveChanges();
                contact.MajorProgram = this.context.MajorProgram
                                        .Where( m => m.Id == contact.MajorProgramId)
                                        .Include( m => m.StrategicInitiative).ThenInclude( i => i.FiscalYear )
                                        .FirstOrDefault();
                return new OkObjectResult(contact);
            }else{
                this.Log( contact ,"ContactRevision", "Error in adding statistical contact attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateContact( int id, [FromBody] ContactRevision contact){
           
            var entity = context.ContactRevision.Find(id);
            var acEntity = context.Contact.Find(entity.ContactId);

            if(contact != null && acEntity != null){
                contact.Created = DateTime.Now;
                acEntity.Updated = DateTime.Now;
                acEntity.MajorProgramId = contact.MajorProgramId;
                acEntity.Audience = contact.Male + contact.Female;
                acEntity.ContactDate = contact.ContactDate;
                acEntity.Days = contact.Days;
                acEntity.Revisions.Add(contact);
                context.SaveChanges();
                acEntity.LastRevisionId = contact.Id;
                this.Log(entity,"ContactRevision", "Statistical Contact Updated.");
                contact.MajorProgram = this.context.MajorProgram
                                        .Where( m => m.Id == contact.MajorProgramId)
                                        .Include( m => m.StrategicInitiative).ThenInclude( i => i.FiscalYear )
                                        .FirstOrDefault();
                return new OkObjectResult(contact);
            }else{
                this.Log( contact ,"ContactRevision", "Not Found Statistical Contact in an update attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteContact( int id ){
            var entity = context.ContactRevision.Find(id);

            /* 
            var acEntity = context.Contact.Where(a => a.Id == entity.ContactId).
                                Include(e=>e.Revisions).ThenInclude(r => r.ContactOptionNumbers).
                                Include(e=>e.Revisions).ThenInclude(r => r.ContactRaceEthnicityValues).
                                FirstOrDefault();
             */
            if(entity != null){
                var contactId = entity.ContactId;
                context.RemoveRange(
                    context.ContactRevision.Where( c => c.ContactId == contactId)
                );
                context.SaveChanges();
                context.Contact.Remove(context.Contact.Find( contactId));
                context.SaveChanges();
                
                this.Log(entity,"ContactRevision", "Contact Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ContactRevision", "Not Found Contact in a delete attempt.", "Statistical Contact", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpGet("optionnumbers")]
        public IActionResult OptionNumbers(){
            var ops = this.context.ActivityOptionNumber.OrderBy(o => o.Order);
            return new OkObjectResult(ops);
        }

        [HttpGet("races")]
        public IActionResult Races(){
            var rcs = this.context.Race.OrderBy(o => o.Order);
            return new OkObjectResult(rcs);
        }

        [HttpGet("ethnicities")]
        public IActionResult Ethnicities(){
            var rcs = this.context.Ethnicity.OrderBy(o => o.Order);
            return new OkObjectResult(rcs);
        }

        private void Log(   object obj, 
                            string objectType = "ContactRevision",
                            string description = "Submitted Statistical Contact Revision", 
                            string type = "Statistical Contact",
                            string level = "Information"
                        ){
                             
            var log = new Log();
            log.Level = level;
            log.Time = DateTime.Now;
            log.User = this.CurrentUser();
            log.ObjectType = objectType;
            log.Object = JsonConvert.SerializeObject(obj,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
            log.Agent = Request.Headers["User-Agent"].ToString();
            log.Ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            log.Description = description;
            log.Type = type;
            this.context.Log.Add(log);
            context.SaveChanges();

        }

        private KersUser userByProfileId(string profileId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.personID == profileId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = userRepo.findByProfileID(profile.Id);
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }


        private KersUser userByLinkBlueId(string linkBlueId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = this.context.KersUser.
                            Where( u => u.classicReportingProfileId == profile.Id).
                            Include(u => u.RprtngProfile).
                            Include(u => u.ExtensionPosition).
                            FirstOrDefault();
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }


        private KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }



        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }



    }
}