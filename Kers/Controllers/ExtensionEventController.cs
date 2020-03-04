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


        [HttpPost()]
        [Authorize]
        public IActionResult AddExtensionEvent( [FromBody] object ExEvent){
            if(ExEvent != null){
                
                return new OkObjectResult(ExEvent);
            }else{
                this.Log( ExEvent,"ExtensionEvent", "Error in adding extension event attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpPost("getevents")]
        public IActionResult GetEvents( string start, string end){
            var list = new List<object>();

            var StartDate = DateTimeOffset.Parse( start);
            var EndtDate = DateTimeOffset.Parse( end);

            var events = this.context.ExtensionEvent.Where( e => e.Start < EndtDate && e.Start > StartDate);
            foreach( var e in events ){
                if(e.DiscriminatorValue == "Training"){
                    var trnng = context.Training.Find(e.Id);
                    if(trnng.tStatus == "A"){
                        if(trnng != null ){
                            if( trnng.End == null || trnng.Start.ToString("ddMMyyyy") == trnng.End?.ToString("ddMMyyyy") ){
                                var times = ProcessTime(trnng.day1);
                                if( times != null ){
                                    var startDate = new DateTimeOffset(new DateTime(trnng.Start.Year, trnng.Start.Month, trnng.Start.Day, times.start.hour, times.start.minute, 0), new TimeSpan(times.timeshift, 0, 0));
                                    var endDate = new DateTimeOffset(new DateTime(trnng.Start.Year, trnng.Start.Month, trnng.Start.Day, times.end.hour, times.end.minute, 0), new TimeSpan(times.timeshift, 0, 0));
                                    list.Add(new {
                                        title = "In-Service Training: " + e.Subject,
                                        start = startDate,
                                        end = endDate,
                                        description = e.Body,
                                        tContact = e.tContact,
                                        tLocation = e.tLocation,
                                        day1=trnng.day1,
                                        day2=trnng.day2,
                                        day3=trnng.day3,
                                        day4=trnng.day4,
                                        allDay = false,
                                        id = e.Id,
                                        type = e.DiscriminatorValue,
                                        backgroundColor = "#3a87ad"
                                    });

                                }else{
                                    list.Add(new {
                                        title = "In-Service Training: " + e.Subject,
                                        start = e.Start,
                                        end = e.End,
                                        description = e.Body,
                                        tContact = e.tContact,
                                        tLocation = e.tLocation,
                                        day1=trnng.day1,
                                        day2=trnng.day2,
                                        day3=trnng.day3,
                                        day4=trnng.day4,
                                        allDay = true,
                                        id = e.Id,
                                        type = e.DiscriminatorValue,
                                        backgroundColor = "#3a87ad"
                                    });
                                }

                            }else{
                                list.Add(new {
                                    title = "In-Service Training: " + e.Subject,
                                    start = e.Start,
                                    end = e.End,
                                    description = e.Body,
                                    tContact = e.tContact,
                                    tLocation = e.tLocation,
                                    day1=trnng.day1,
                                    day2=trnng.day2,
                                    day3=trnng.day3,
                                    day4=trnng.day4,
                                    allDay = true,
                                    id = e.Id,
                                    type = e.DiscriminatorValue,
                                    backgroundColor = "#3a87ad"
                                });

                            }
                        }
                    }
                }else{
                    list.Add(new {
                        title = (e.IsCancelled == true ? "(Canceled) " : "" ) + e.Subject,
                        start = e.Start,
                        end = e.End,
                        description = e.Body,
                        tContact = e.tContact,
                        tLocation = e.tLocation,
                        allDay = e.IsAllDay,
                        id = e.Id,
                        type = e.DiscriminatorValue,
                        backgroundColor = (e.IsCancelled == true ? "#E74C3C" : "#73879C")
                    });
                }
                
            }

            
            return new OkObjectResult(list);
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



        private StartAndEndTimes ProcessTime( string time){
            if(time != null){
                var parts = time.Split('-');
                if(parts.Count() > 1){
                    var start = TryToParseTime( parts[0] );
                    var end = TryToParseTime( parts[1] );
                    if( start != null && end != null){
                        return new StartAndEndTimes(){
                            start = start,
                            end = end,
                            timeshift = this.Timeshift( time )
                        };
                    }
                }
            }
            
            return null;
        }

        private EventTime TryToParseTime(string timestring){
            timestring = timestring.Trim();
            var tmArr = timestring.Split(':');
            if(tmArr.Count() > 1){
                if (Int32.TryParse(tmArr[0], out int hourPart)){
                    var minuteArr = tmArr[1].Split(' ');
                    if( minuteArr.Count() > 1){
                        if( minuteArr[1].ToLower() == "pm" || minuteArr[1].ToLower()=="p.m."){
                            hourPart = hourPart < 12 ? (hourPart + 12) : 12;
                        }else if(minuteArr[0].Length > 1){
                            var lastTwoChars = minuteArr[0].Substring( minuteArr[0].Length - 2);
                            var lastFourChars = "";
                            if(minuteArr[0].Length > 4){
                                lastFourChars = minuteArr[0].Substring( minuteArr[0].Length - 4);
                            }
                            if( lastTwoChars.ToLower() == "pm" || lastFourChars.ToLower() == "p.m."){
                                hourPart = hourPart < 12 ? (hourPart + 12) : 12;
                            }

                        }
                    }
                    if (minuteArr[0].Length > 1 && Int32.TryParse(minuteArr[0].Substring( 0, 2 ), out int minutePart)){
                        return new EventTime(){ hour = hourPart, minute = minutePart};
                    }
                }
            }
            return null;
        }

        private int Timeshift(string timestring){
            int shift = -4;
            var tmArr = timestring.Split(' ');
            var lastPart = tmArr.Count() - 1;
            if(tmArr[lastPart] == "CT" || tmArr[lastPart] == "CST" || tmArr[lastPart] == "CDT" || tmArr[lastPart] == "Central"){
                shift = -5;
            }
            return shift;
        }













        

        
        public IActionResult Error()
        {
            return View();
        }


    }
    
}

public class StartAndEndTimes{
    public EventTime start;
    public EventTime end;
    public int timeshift;
}
public class EventTime{
    public int hour;
    public int minute;
}
