using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.UKCAReporting;
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
        KERSreportingContext _reportingContext;
        private string DefaultTime = "12:34:56.1000000";
        public CountyEventController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    KERSreportingContext reportingContext
            ):base(mainContext, context, userRepo){
                _context = context;
                _reportingContext = reportingContext;
        }

        [HttpGet]
        [Route("range/{countyId?}/{skip?}/{take?}/{order?}")]
        public virtual IActionResult GetRange(int skip = 0, int take = 10, string order = "start", int countyId = 0)
        {
            if(countyId == 0 ){
                var user = CurrentUser();
                countyId = user.RprtngProfile.PlanningUnitId;
            }
            IQueryable<CountyEvent> query = _context.CountyEvent
                        .Where( e => e.Units.Select( u => u.PlanningUnitId).Contains(countyId))
                        .Include( e => e.Location).ThenInclude( l => l.Address)
                        .Include( e => e.Units)
                        .Include( e => e.ProgramCategories);
             
            if(order == "end"){
                query = query.OrderByDescending(t => t.End);
            }else if( order == "created"){
                query = query.OrderByDescending(t => t.CreatedDateTime);
            }else{
                query = query.OrderByDescending(t => t.Start);
            }
             
            query = query.Skip(skip).Take(take);
            var reslt = new List<CountyEventWithTime>();
            foreach( var ce in query){
                var e = new CountyEventWithTime(ce);
                reslt.Add( e );
            }

            return new OkObjectResult(reslt);
        }

        [HttpGet]
        [Route("GetCustom")]
        public virtual IActionResult GetCustom(
                                        [FromQuery] string search, 
                                        [FromQuery] DateTime start,
                                        [FromQuery] DateTime end,
                                        [FromQuery] int? day,
                                        [FromQuery] string order,
                                        [FromQuery] int? countyId

        )
        {
            
            IQueryable<CountyEvent> query = _context.CountyEvent.Where( e => e.Start.Date >= start.Date && e.Start.Date <= end.Date);
            if( countyId != null){
                if(countyId == 0 ){
                    var user = CurrentUser();
                    countyId = user.RprtngProfile.PlanningUnitId;
                }
                query = query.Where( e => e.Units.Select( u => u.PlanningUnitId ).Contains(countyId??0));
            }
            if( day != null){
                query = query.Where( e => (int) e.Start.DayOfWeek == (day??0) );
            }
            if( search != null && search != ""){
                query = query.Where( e => e.Subject.Contains( search ));
            }
                        
            query = query .Include( e => e.Location).ThenInclude( l => l.Address)
                        .Include( e => e.Units)
                        .Include( e => e.ProgramCategories);
             
            if(order == "dsc"){
                query = query.OrderByDescending(t => t.Start);
            }else if( order == "asc"){
                query = query.OrderBy(t => t.CreatedDateTime);
            }else{
                query = query.OrderBy(t => t.Subject);
            }
            var reslt = new List<CountyEventWithTime>();
            foreach( var ce in query){
                var e = new CountyEventWithTime(ce);
                reslt.Add( e );
            }

            return new OkObjectResult(reslt);
        }


        [HttpPost("addcountyevent")]
        [Authorize]
        public IActionResult AddCountyEvent( [FromBody] CountyEventWithTime CntEvent){
            if(CntEvent != null){
                CountyEvent evnt = new CountyEvent();
                evnt.Organizer = this.CurrentUser();
                evnt.CreatedDateTime = DateTimeOffset.Now;
                evnt.LastModifiedDateTime = DateTimeOffset.Now;
                var starttime = this.DefaultTime;
                evnt.IsAllDay = true;
                var timezone = CntEvent.Etimezone ? " -04:00":" -05:00";
                if(CntEvent.Starttime != ""){
                    starttime = CntEvent.Starttime+":00.1000000";
                    evnt.IsAllDay = false;
                    evnt.HasStartTime = true;
                } 
                evnt.Start = DateTimeOffset.Parse(CntEvent.Start.ToString("yyyy-MM-dd ") + starttime + timezone);
                
                if(CntEvent.End != null ){
                    var endtime = this.DefaultTime;
                    if(CntEvent.Endtime != ""){
                        endtime = CntEvent.Endtime+":00.1000000";
                        evnt.HasEndTime = true;
                    }
                    evnt.End = DateTimeOffset.Parse(CntEvent.End?.ToString("yyyy-MM-dd ") + endtime + timezone);
                }
                evnt.BodyPreview = evnt.Body = CntEvent.Body;
                evnt.WebLink = CntEvent.WebLink;
                evnt.Subject = CntEvent.Subject;
                evnt.Location = CntEvent.Location;
                evnt.Units = CntEvent.Units;
                evnt.ProgramCategories = CntEvent.ProgramCategories;
                CntEvent.ExtensionEventImages.ForEach(s => s.Created = DateTime.Now);
                evnt.ExtensionEventImages = CntEvent.ExtensionEventImages;
                this.context.Add(evnt);
                this.context.SaveChanges();
                this.Log(evnt,"CountyEvent", "County Event Added.");
                return new OkObjectResult(evnt);
            }else{
                this.Log( CntEvent,"CountyEvent", "Error in adding county event attempt.", "CountyEvent", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("updatecountyevent/{id}")]
        [Authorize]
        public IActionResult UpdateCountyEvent( int id, [FromBody] CountyEventWithTime CntEvent){
           
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
                this.Log(CntEvent,"CountyEvent", "County Event Updated.");
                
                return new OkObjectResult(evnt);
            }else{
                this.Log( CntEvent ,"CountyEvent", "Not Found CountyEvent in an update attempt.", "CountyEvent", "Error");
                return new StatusCodeResult(500);
            }
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
                
                this.Log(entity,"CountyEvent", "CountyEvent Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"CountyEvent", "Not Found CountyEvent in a delete attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("migrate/{id}")]
        public async Task<IActionResult> MigrateCountyEvent(int id){
            try{
                if( !(await context.CountyEvent.Where( t => t.classicCountyEventId == id).AnyAsync()) ){
                    var service = await this._reportingContext.zCesCountyEvent.Where( s => s.rID == id).FirstOrDefaultAsync();
                    if( service != null){
                        if( !this.context.CountyEvent.Where( t => t.classicCountyEventId == service.rID).Any() ){
                                var CountyEvent = ConvertCountyEvent(service);
                                this.context.Add(CountyEvent);
                                await this.context.SaveChangesAsync();
                                return new OkObjectResult(CountyEvent);   
                        }
                    } 
                }
                return new OkObjectResult(null);
            }catch( Exception e ){
                this.Log( e.Message ,"CountyEvent", "Migration Error.", "CountyEvent", "Error");
                return new StatusCodeResult(500);
            }
        }



/* 
        [HttpGet("migrate/{id}")]
        public async Task<IActionResult> MigrateCountyEvent(int id){
            try{
                if( !(await context.CountyEvent.Where( t => t.classicCountyEventId == id).AnyAsync()) ){
                    var service = await this.context.LegacyCountyEvents.Where( s => s.rID == id).FirstOrDefaultAsync();
                    if( service != null){
                        if( !this.context.CountyEvent.Where( t => t.classicCountyEventId == service.rID).Any() ){
                                var CountyEvent = ConvertCountyEvent(service);
                                this.context.Add(CountyEvent);
                                await this.context.SaveChangesAsync();
                                return new OkObjectResult(CountyEvent);   
                        }
                    } 
                }
                return new OkObjectResult(null);
            }catch( Exception e ){
                this.Log( e.Message ,"CountyEvent", "Migration Error.", "CountyEvent", "Error");
                return new StatusCodeResult(500);
            }
        }

 */
        
        [HttpGet("getlegacy/{limit}")]
        public async Task<IActionResult> GetLegacyCountyEvents(int limit){
            var leg = new List<zCesCountyEvent>();

            var serv = _reportingContext.zCesCountyEvent
                                    .Where( a => a.rDt != null 
                                                &&
                                                     a.rDt.Value.Year > 2019
                                                &&
                                                     a.rDt.Value.Year < 2022
                                                 )
                                    .OrderByDescending(r => r.rDt).ToListAsync();
            foreach( var srv in  await serv){
                 if( !context.CountyEvent.Where( t => t.classicCountyEventId == srv.rID).Any()){
                    leg.Add(srv);
                }
            }
/* 

            do{
                var serv = await _reportingContext.zCesCountyEvent
                                    .Where( a => a.rDt != null && a.rDt.Value.Year > 2018 )
                                    .OrderBy(r => Guid.NewGuid()).LastAsync();
                if( serv == null ) break;
                if( !context.CountyEvent.Where( t => t.classicCountyEventId == serv.rID).Any()){
                    leg.Add(serv);
                }
                

            }while(leg.Count() < limit);
 */
/* 

            IQueryable<zCesCountyEvent> services;
            List<int> converted = await context.CountyEvent.Where( t => t.classicCountyEventId != null && t.classicCountyEventId != 0).Select( t => t.classicCountyEventId??0).ToListAsync();
            services = _reportingContext.zCesCountyEvent.Where( s => !converted.Contains( s.rID ));
            var sc = await services.Take(limit).ToListAsync();

             */
            return new OkObjectResult(leg);
        }




/* 
        [HttpGet("getlegacy/{limit}/{notConverted?}/{order?}")]
        public async Task<IActionResult> GetLegacyCountyEvents(int limit, Boolean notConverted = true, string order = "ASC"){
            IOrderedQueryable<LegacyCountyEvents> services;
            if( notConverted ){
                List<int> converted = await context.CountyEvent.Where( t => t.classicCountyEventId != null && t.classicCountyEventId != 0).Select( t => t.classicCountyEventId??0).ToListAsync();
                if( order == "ASC"){
                    services = context.LegacyCountyEvents.Where( s => !converted.Contains( s.rID )).OrderBy(a => a.rDt);
                }else{
                    services = context.LegacyCountyEvents.Where( s => !converted.Contains( s.rID )).OrderByDescending(a => a.rDt);
                }
            }else{
                if( order == "ASC"){
                    services = context.LegacyCountyEvents.OrderBy(a => a.rDt);
                }else{
                    services = context.LegacyCountyEvents.OrderByDescending(a => a.rDt);
                }
            }
            var sc = await services.Take(limit).ToListAsync();
            return new OkObjectResult(sc);
        } */

        
        private CountyEvent ConvertCountyEvent( zCesCountyEvent legacy){
            CountyEvent evnt = new CountyEvent();
            evnt.Subject = legacy.eventTitle;
            evnt.Body = evnt.BodyPreview = legacy.eventDescription;
            evnt.classicCountyEventId = legacy.rID;
            evnt.CreatedDateTime = Convert.ToDateTime(legacy.rDt);
            evnt.LastModifiedDateTime = DateTimeOffset.Now;
            var user = context.KersUser.Where( u => u.RprtngProfile.PersonId == legacy.rBy).FirstOrDefault();
            if( user != null ){
                evnt.Organizer = user;
            }else{
                evnt.OrganizerId = 2;
            }
            var unit = context.PlanningUnit.Where( u => u.Code == legacy.planningUnitID.ToString()).FirstOrDefault();
            evnt.Units = new List<CountyEventPlanningUnit>();
            bool isEastern = true;
            if( unit != null){
                var host = new CountyEventPlanningUnit();     
                host.CountyEvent = evnt;
                host.PlanningUnitId = unit.Id;
                host.IsHost = true;
                evnt.Units.Add(host);
                if( unit.TimeZoneId == "Central Standard Time" || unit.TimeZoneId == "America/Chicago"){
                    isEastern = false;
                }
            }
            if(legacy.eventCounties != null && legacy.eventCounties != "NULL" && legacy.eventCounties != ""){
                string[] cnts = legacy.eventCounties.Split(',');
                foreach( var cnt in cnts){
                    if( cnt != ""){
                        var otherCounty = context.PlanningUnit.Where( u => u.Code == cnt ).FirstOrDefault();
                        if( otherCounty != null ){
                            var cntConnection = new CountyEventPlanningUnit();
                            cntConnection.PlanningUnitId = otherCounty.Id;
                            cntConnection.CountyEvent = evnt;
                            cntConnection.IsHost = false;
                            evnt.Units.Add(cntConnection);
                        }
                    }
                }
            }
            var timezone = isEastern ? " -04:00":" -05:00";
            var starttime = this.DefaultTime;
            if(legacy.eventTimeBegin != null && legacy.eventTimeBegin != "" && legacy.eventTimeBegin != "NULL"){
                starttime = legacy.eventTimeBegin.Insert(2, ":")+":00.1000000";
                evnt.IsAllDay = false;
                evnt.HasStartTime = true;
            }else{
                evnt.HasEndTime = false;
            }
            string[] beginDate = legacy.eventDateBegin.Split("/");
            if(beginDate.Count() < 3 ) beginDate = legacy.eventDateBegin.Split("-");
            if(beginDate[2].Count() < 3) beginDate[2] = "20"+beginDate[1];
            evnt.Start = DateTimeOffset.Parse(beginDate[2] + "-" + beginDate[0] + "-" + beginDate[1] + " " + starttime + timezone);
            if(
                legacy.eventDateEnd != null 
                &&
                legacy.eventDateEnd != "NULL"
                && 
                legacy.eventDateEnd != ""
                &&
                 !(
                     legacy.eventDateEnd == legacy.eventDateBegin 
                     && 
                     (legacy.eventTimeEnd == "NULL" || legacy.eventTimeEnd == null)
                 )
                
                ){
                var endtime = this.DefaultTime;
                if(legacy.eventTimeEnd != null && legacy.eventTimeEnd != "" && legacy.eventTimeEnd != "NULL"){
                    endtime = legacy.eventTimeEnd.Insert(2, ":")+":00.1000000";
                    evnt.HasEndTime = true;
                }else{
                    evnt.HasEndTime = false;
                }
                string[] endDate = legacy.eventDateEnd.Split("/");
                if(endDate.Count() < 3 ) endDate = legacy.eventDateEnd.Split("-");
                if(endDate[2].Count() < 3) endDate[2] = "20"+endDate[1];
                evnt.End = DateTimeOffset.Parse( endDate[2] + "-" + endDate[0] + "-" + endDate[1] + " " + endtime + timezone);
            }else{
                evnt.End = null;
            }
            if( legacy.eventUrl != "NULL" && legacy.eventUrl != "" && legacy.eventUrl != null){
                evnt.WebLink = legacy.eventUrl;
            }
            evnt.ProgramCategories = new List<CountyEventProgramCategory>();
            if(legacy.progANR == true || legacy.progHORT == true){
                int? ANRcategoryId = context.ProgramCategory.Where( a => a.ShortName == "ANR").Select( c => c.Id ).FirstOrDefault();
                if( ANRcategoryId != null ){
                    var AnrCat = new CountyEventProgramCategory();
                    AnrCat.ProgramCategoryId = ANRcategoryId??0;
                    AnrCat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(AnrCat);
                }
            }
            if(legacy.prog4HYD == true){
                int? HcategoryId = context.ProgramCategory.Where( a => a.ShortName == "4-H").Select( c => c.Id ).FirstOrDefault();
                if( HcategoryId != null ){
                    var HCat = new CountyEventProgramCategory();
                    HCat.ProgramCategoryId = HcategoryId??0;
                    HCat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(HCat);
                }
            }
            if(legacy.progFA == true){
                int? FAcategoryId = context.ProgramCategory.Where( a => a.ShortName == "CED").Select( c => c.Id ).FirstOrDefault();
                if( FAcategoryId != null ){
                    var FACat = new CountyEventProgramCategory();
                    FACat.ProgramCategoryId = FAcategoryId??0;
                    FACat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(FACat);
                }
            }
            if(legacy.progFCS == true){
                int? FCScategoryId = context.ProgramCategory.Where( a => a.ShortName == "FCS").Select( c => c.Id ).FirstOrDefault();
                if( FCScategoryId != null ){
                    var FCSCat = new CountyEventProgramCategory();
                    FCSCat.ProgramCategoryId = FCScategoryId??0;
                    FCSCat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(FCSCat);
                }
            }
            if(legacy.progOther == true){
                int? OcategoryId = context.ProgramCategory.Where( a => a.ShortName == "OTHR").Select( c => c.Id ).FirstOrDefault();
                if( OcategoryId != null ){
                    var OCat = new CountyEventProgramCategory();
                    OCat.ProgramCategoryId = OcategoryId??0;
                    OCat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(OCat);
                }
            }
            evnt.Location = ProcessAddress(legacy, unit);
            return evnt;
        }




/* 
        private CountyEvent ConvertCountyEvent( LegacyCountyEvents legacy){
            CountyEvent evnt = new CountyEvent();
            evnt.Subject = legacy.eventTitle;
            evnt.Body = evnt.BodyPreview = legacy.eventDescription;
            evnt.classicCountyEventId = legacy.rID;
            evnt.CreatedDateTime = Convert.ToDateTime(legacy.rDt);
            evnt.LastModifiedDateTime = DateTimeOffset.Now;
            var unit = context.PlanningUnit.Where( u => u.Code == legacy.planningUnitID.ToString()).FirstOrDefault();
            evnt.Units = new List<CountyEventPlanningUnit>();
            bool isEastern = true;
            if( unit != null){
                var host = new CountyEventPlanningUnit();     
                host.CountyEvent = evnt;
                host.PlanningUnitId = unit.Id;
                host.IsHost = true;
                evnt.Units.Add(host);
                if( unit.TimeZoneId == "Central Standard Time" || unit.TimeZoneId == "America/Chicago"){
                    isEastern = false;
                }
            }
            if(legacy.eventCounties != null && legacy.eventCounties != "NULL" && legacy.eventCounties != ""){
                string[] cnts = legacy.eventCounties.Split(',');
                foreach( var cnt in cnts){
                    if( cnt != ""){
                        var otherCounty = context.PlanningUnit.Where( u => u.Code == cnt ).FirstOrDefault();
                        if( otherCounty != null ){
                            var cntConnection = new CountyEventPlanningUnit();
                            cntConnection.PlanningUnitId = otherCounty.Id;
                            cntConnection.CountyEvent = evnt;
                            cntConnection.IsHost = false;
                            evnt.Units.Add(cntConnection);
                        }
                    }
                }
            }
            var timezone = isEastern ? " -04:00":" -05:00";
            var starttime = this.DefaultTime;
            if(legacy.eventTimeBegin != null && legacy.eventTimeBegin != "" && legacy.eventTimeBegin != "NULL"){
                starttime = legacy.eventTimeBegin.Insert(2, ":")+":00.1000000";
                evnt.IsAllDay = false;
                evnt.HasStartTime = true;
            }else{
                evnt.HasEndTime = false;
            }
            string[] beginDate = legacy.eventDateBegin.Split("/");
            evnt.Start = DateTimeOffset.Parse(beginDate[2] + "-" + beginDate[0] + "-" + beginDate[1] + " " + starttime + timezone);
            if(
                legacy.eventDateEnd != null 
                &&
                legacy.eventDateEnd != "NULL"
                && 
                legacy.eventDateEnd != ""
                &&
                 !(
                     legacy.eventDateEnd == legacy.eventDateBegin 
                     && 
                     (legacy.eventTimeEnd == "NULL" || legacy.eventTimeEnd == null)
                 )
                
                ){
                var endtime = this.DefaultTime;
                if(legacy.eventTimeEnd != null && legacy.eventTimeEnd != "" && legacy.eventTimeEnd != "NULL"){
                    endtime = legacy.eventTimeEnd.Insert(2, ":")+":00.1000000";
                    evnt.HasEndTime = true;
                }else{
                    evnt.HasEndTime = false;
                }
                string[] endDate = legacy.eventDateEnd.Split("/");
                evnt.End = DateTimeOffset.Parse( endDate[2] + "-" + endDate[0] + "-" + endDate[1] + " " + endtime + timezone);
            }else{
                evnt.End = null;
            }
            if( legacy.eventUrl != "NULL" && legacy.eventUrl != "" && legacy.eventUrl != null){
                evnt.WebLink = legacy.eventUrl;
            }
            evnt.ProgramCategories = new List<CountyEventProgramCategory>();
            if(legacy.progANR == 1 || legacy.progHORT == 1){
                int? ANRcategoryId = context.ProgramCategory.Where( a => a.ShortName == "ANR").Select( c => c.Id ).FirstOrDefault();
                if( ANRcategoryId != null ){
                    var AnrCat = new CountyEventProgramCategory();
                    AnrCat.ProgramCategoryId = ANRcategoryId??0;
                    AnrCat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(AnrCat);
                }
            }
            if(legacy.prog4HYD == 1){
                int? HcategoryId = context.ProgramCategory.Where( a => a.ShortName == "4-H").Select( c => c.Id ).FirstOrDefault();
                if( HcategoryId != null ){
                    var HCat = new CountyEventProgramCategory();
                    HCat.ProgramCategoryId = HcategoryId??0;
                    HCat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(HCat);
                }
            }
            if(legacy.progFA == 1){
                int? FAcategoryId = context.ProgramCategory.Where( a => a.ShortName == "CED").Select( c => c.Id ).FirstOrDefault();
                if( FAcategoryId != null ){
                    var FACat = new CountyEventProgramCategory();
                    FACat.ProgramCategoryId = FAcategoryId??0;
                    FACat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(FACat);
                }
            }
            if(legacy.progFCS == 1){
                int? FCScategoryId = context.ProgramCategory.Where( a => a.ShortName == "FCS").Select( c => c.Id ).FirstOrDefault();
                if( FCScategoryId != null ){
                    var FCSCat = new CountyEventProgramCategory();
                    FCSCat.ProgramCategoryId = FCScategoryId??0;
                    FCSCat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(FCSCat);
                }
            }
            if(legacy.progOther == 1){
                int? OcategoryId = context.ProgramCategory.Where( a => a.ShortName == "OTHR").Select( c => c.Id ).FirstOrDefault();
                if( OcategoryId != null ){
                    var OCat = new CountyEventProgramCategory();
                    OCat.ProgramCategoryId = OcategoryId??0;
                    OCat.CountyEvent = evnt;
                    evnt.ProgramCategories.Add(OCat);
                }
            }
            evnt.Location = ProcessAddress(legacy, unit);
            return evnt;
        }

 */
        private ExtensionEventLocation ProcessAddress( zCesCountyEvent legacy, PlanningUnit unit ){
            
            var Location = new ExtensionEventLocation();
            Location.Address = new PhysicalAddress();
            Location.Address.Building = legacy.eventBldgName;
            Location.Address.City = legacy.eventCity;
            Location.Address.PostalCode = legacy.eventZip;
            Location.Address.State = legacy.eventState;
            Location.Address.Street = legacy.eventAddress;

            if( unit != null ){
                if( 
                        !context.ExtensionEventLocationConnection
                            .Where( u => 
                                            u.PlanningUnitId == unit.Id 
                                                && 
                                            legacy.eventBldgName == "" ?
                                                u.ExtensionEventLocation.Address.Street == legacy.eventAddress 
                                                :
                                                u.ExtensionEventLocation.Address.Building == legacy.eventBldgName
                                             )
                            .Any() 
                    ){

                    var loc = new ExtensionEventLocationConnection();
                    loc.ExtensionEventLocation = new ExtensionEventLocation();
                    loc.PlanningUnitId = unit.Id;
                    loc.ExtensionEventLocation.Address = new PhysicalAddress();
                    loc.ExtensionEventLocation.Address.Building = legacy.eventBldgName;
                    loc.ExtensionEventLocation.Address.City = legacy.eventCity;
                    loc.ExtensionEventLocation.Address.PostalCode = legacy.eventZip;
                    loc.ExtensionEventLocation.Address.State = legacy.eventState;
                    loc.ExtensionEventLocation.Address.Street = legacy.eventAddress == "NULL" ? null : legacy.eventAddress;
                    context.ExtensionEventLocationConnection.Add(loc);
                    context.SaveChanges();
                }

                return Location;


            }

            return null;
        }


        
        public IActionResult Error()
        {
            return View();
        }


    }

    public class CountyEventWithTime:CountyEvent{
        public string Starttime;
        public string Endtime;
        public bool Etimezone;

        public CountyEventWithTime(){}

        public CountyEventWithTime(CountyEvent m){
            this.Id = m.Id;
            this.Body = m.Body;
            this.BodyPreview = m.BodyPreview;
            this.Start = m.Start;
            this.End = m.End;
            this.IsAllDay = m.IsAllDay;
            this.CreatedDateTime = m.CreatedDateTime;
            this.LastModifiedDateTime = m.LastModifiedDateTime;
            this.Subject = m.Subject;
            this.IsCancelled = m.IsCancelled;
            this.HasEndTime = m.HasEndTime;
            this.HasStartTime = m.HasStartTime;
            this.Units = m.Units;
            this.ProgramCategories = m.ProgramCategories;
            this.WebLink = m.WebLink;
            this.Location = m.Location;

            if(m.End != null){
                TimeSpan tmzn = m.Start.Offset;
                var hrs = tmzn.TotalHours;
                this.Etimezone = hrs == -4;
                if(this.End.HasValue ){
                    this.Endtime = this.End.Value.Hour.ToString("D2")+ ":" + this.End.Value.Minute.ToString("D2");
                }
                
            }
            this.Starttime = this.Start.Hour.ToString("D2") + ":" + this.Start.Minute.ToString("D2");

        }


    }
    
}
