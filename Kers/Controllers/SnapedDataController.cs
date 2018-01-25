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
using Microsoft.Extensions.Caching.Memory;
using System.Dynamic;
using Kers.Models.Data;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class SnapedDataController : BaseController
    {
        
        IFiscalYearRepository fiscalRepo;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        const string LogType = "SnapEdData";
        public SnapedDataController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IDistributedCache _cache,
                    IFiscalYearRepository fiscalRepo,
                    IActivityRepository activityRepo
            ):base(mainContext, context, userRepo){
                this.fiscalRepo = fiscalRepo;
                this._cache = _cache;
                this.activityRepo = activityRepo;
        }
    
        [HttpGet]
        [Route("totalbymonth/{fy}/data.csv")]
        //[Produces("text/csv")]
        [Authorize]
        public IActionResult TotalByMonth(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("HoursReported");
            keys.Add("DirectContacts");

            var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);
            
            foreach( var audnc in snapDirectAudience){
                keys.Add(audnc.Name);
            }

            var snapDirectAges = this.context.SnapDirectAges.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);

            foreach( var ags in snapDirectAges){
                keys.Add(ags.Name);
            }

            keys.Add("Male");
            keys.Add("Female");

            var races = this.context.Race;
            var ethnicities = this.context.Ethnicity;

            foreach(var race in races){
                foreach( var ethn in ethnicities){
                    keys.Add( race.Name + ethn.Name);
                }
            }

            keys.Add("IndirectContacts");


            var result = string.Join(", ", keys.ToArray()) + "\n";






            var SnapData = this.SnapData( fiscalYear);

            var byMonth = SnapData.GroupBy( s => new {
                                        Year = s.Revision.ActivityDate.Year,
                                        Month = s.Revision.ActivityDate.Month

                                    }
            
                                ).Select( 
                                        d => new {
                                            Year = d.Key.Year,
                                            Month = d.Key.Month,
                                            Revisions = d.Select( s => s.Revision )
                                        }
                                    )
                                    .OrderBy( d => d.Year).ThenBy( d => d.Month);
            foreach( var monthData in byMonth ){
                var dt = new DateTime( monthData.Year, monthData.Month, 15);
                var row = dt.ToString("yyyyMM") + ",";
                row += dt.ToString("yyyy-MMM") + ",";
                row += monthData.Revisions.Sum( s => s.Hours).ToString() + ",";

                var male = monthData.Revisions.Sum( s => s.Male);
                var female = monthData.Revisions.Sum( s => s.Female);

                row += ( male + female ).ToString() + ",";
                var revisionsWithDirectContacts = monthData.Revisions.Where( s => s.SnapDirect != null);

                var ageAudienceValues = new List<SnapDirectAgesAudienceValue>();
                foreach( var rev in revisionsWithDirectContacts){
                    ageAudienceValues.AddRange( rev.SnapDirect.SnapDirectAgesAudienceValues);
                }


                foreach( var audnc in snapDirectAudience){
                    row += ageAudienceValues.Where( a => a.SnapDirectAudienceId == audnc.Id).Sum( s => s.Value ).ToString() + ",";
                }

                foreach( var ags in snapDirectAges){
                     row += ageAudienceValues.Where( a => a.SnapDirectAgesId == ags.Id).Sum( s => s.Value ).ToString() + ",";
                }

                row += male.ToString() + ",";
                row += female.ToString() + ",";


                var RaceEthnicityValues = new List<RaceEthnicityValue>();

                foreach( var rev in monthData.Revisions){
                    RaceEthnicityValues.AddRange(rev.RaceEthnicityValues);
                }

                foreach(var race in races){
                    foreach( var ethn in ethnicities){
                        row += RaceEthnicityValues.Where( v => v.EthnicityId == ethn.Id && v.RaceId == race.Id).Sum( s => s.Amount).ToString() + ",";
                    }
                }


                var withIndirect = monthData.Revisions.Where( r => r.SnapIndirect != null);
                var indirects = 0;
                foreach( var ind in withIndirect){
                    indirects += ind.SnapIndirect.SnapIndirectReachedValues.Sum( s => s.Value);
                }
                row += indirects.ToString();
                result += row + "\n";
            }


            return Ok(result);
        }

        [HttpGet]
        [Route("totalbycounty/{fy}/data.csv")]
        //[Produces("text/csv")]
        [Authorize]
        public IActionResult TotalByCounty(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            keys.Add("FY");
            keys.Add("PlanningUnit");
            keys.Add("HoursReported");
            keys.Add("DirectContacts");

            var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);
            
            foreach( var audnc in snapDirectAudience){
                keys.Add(audnc.Name);
            }

            var snapDirectAges = this.context.SnapDirectAges.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);

            foreach( var ags in snapDirectAges){
                keys.Add(ags.Name);
            }

            keys.Add("Male");
            keys.Add("Female");

            var races = this.context.Race;
            var ethnicities = this.context.Ethnicity;

            foreach(var race in races){
                foreach( var ethn in ethnicities){
                    keys.Add( race.Name + ethn.Name);
                }
            }

            keys.Add("IndirectContacts");
            var result = string.Join(", ", keys.ToArray()) + "\n";


            var SnapData = this.SnapData( fiscalYear);

            var byUnit = SnapData.GroupBy( s => s.User.RprtngProfile.PlanningUnit.Id).Select( 
                                        d => new {
                                            Unit = d.Select( s => s.User.RprtngProfile.PlanningUnit ).First(),
                                            Revisions = d.Select( s => s.Revision )
                                        }
                                    )
                                    .OrderBy( d => d.Unit.Name);
            foreach( var unitData in byUnit ){
                var row = fiscalYear.Name + ",";
                row += string.Concat("\"", unitData.Unit.Name, "\"") + ",";
                row += unitData.Revisions.Sum( s => s.Hours).ToString() + ",";

                var male = unitData.Revisions.Sum( s => s.Male);
                var female = unitData.Revisions.Sum( s => s.Female);

                row += ( male + female ).ToString() + ",";
                var revisionsWithDirectContacts = unitData.Revisions.Where( s => s.SnapDirect != null);

                var ageAudienceValues = new List<SnapDirectAgesAudienceValue>();
                foreach( var rev in revisionsWithDirectContacts){
                    ageAudienceValues.AddRange( rev.SnapDirect.SnapDirectAgesAudienceValues);
                }


                foreach( var audnc in snapDirectAudience){
                    row += ageAudienceValues.Where( a => a.SnapDirectAudienceId == audnc.Id).Sum( s => s.Value ).ToString() + ",";
                }

                foreach( var ags in snapDirectAges){
                     row += ageAudienceValues.Where( a => a.SnapDirectAgesId == ags.Id).Sum( s => s.Value ).ToString() + ",";
                }

                row += male.ToString() + ",";
                row += female.ToString() + ",";


                var RaceEthnicityValues = new List<RaceEthnicityValue>();

                foreach( var rev in unitData.Revisions){
                    RaceEthnicityValues.AddRange(rev.RaceEthnicityValues);
                }

                foreach(var race in races){
                    foreach( var ethn in ethnicities){
                        row += RaceEthnicityValues.Where( v => v.EthnicityId == ethn.Id && v.RaceId == race.Id).Sum( s => s.Amount).ToString() + ",";
                    }
                }


                var withIndirect = unitData.Revisions.Where( r => r.SnapIndirect != null);
                var indirects = 0;
                foreach( var ind in withIndirect){
                    indirects += ind.SnapIndirect.SnapIndirectReachedValues.Sum( s => s.Value);
                }
                row += indirects.ToString();
                result += row + "\n";
            }

            return Ok(result);
        }


        [HttpGet]
        [Route("totalbyemployee/{fy}/data.csv")]
        //[Produces("text/csv")]
        [Authorize]
        public IActionResult TotalByEmployee(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            keys.Add("FY");
            keys.Add("PlanningUnit");
            keys.Add("EmployeeName");
            keys.Add("Position");
            keys.Add("Program(s)");
            keys.Add("HoursReported");
            keys.Add("DirectContacts");

            var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);
            
            foreach( var audnc in snapDirectAudience){
                keys.Add(audnc.Name);
            }

            var snapDirectAges = this.context.SnapDirectAges.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);

            foreach( var ags in snapDirectAges){
                keys.Add(ags.Name);
            }

            keys.Add("Male");
            keys.Add("Female");

            var races = this.context.Race;
            var ethnicities = this.context.Ethnicity;

            foreach(var race in races){
                foreach( var ethn in ethnicities){
                    keys.Add( race.Name + ethn.Name);
                }
            }

            keys.Add("IndirectContacts");

            var result = string.Join(", ", keys.ToArray()) + "\n";

            var SnapData = this.SnapData( fiscalYear);

            var byUser = SnapData.GroupBy( s => s.User.Id).Select( 
                                        d => new {
                                            User = d.Select( s => s.User ).First(),
                                            Revisions = d.Select( s => s.Revision )
                                        }
                                    )
                                    .OrderBy( d => d.User.RprtngProfile.PlanningUnit.Name).ThenBy( d => d.User.RprtngProfile.Name);
            foreach( var userData in byUser ){
                var row = fiscalYear.Name + ",";
                row += string.Concat("\"", userData.User.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                row += string.Concat("\"", userData.User.RprtngProfile.Name, "\"") + ",";
                row += string.Concat("\"", userData.User.ExtensionPosition.Code, "\"") + ",";
                var spclt = "";
                foreach( var sp in userData.User.Specialties){
                    spclt += " " + sp.Specialty.Code;
                }
                row += spclt + ", ";
                row += userData.Revisions.Sum( s => s.Hours).ToString() + ",";

                var male = userData.Revisions.Sum( s => s.Male);
                var female = userData.Revisions.Sum( s => s.Female);

                row += ( male + female ).ToString() + ",";
                var revisionsWithDirectContacts = userData.Revisions.Where( s => s.SnapDirect != null);

                var ageAudienceValues = new List<SnapDirectAgesAudienceValue>();
                foreach( var rev in revisionsWithDirectContacts){
                    ageAudienceValues.AddRange( rev.SnapDirect.SnapDirectAgesAudienceValues);
                }


                foreach( var audnc in snapDirectAudience){
                    row += ageAudienceValues.Where( a => a.SnapDirectAudienceId == audnc.Id).Sum( s => s.Value ).ToString() + ",";
                }

                foreach( var ags in snapDirectAges){
                     row += ageAudienceValues.Where( a => a.SnapDirectAgesId == ags.Id).Sum( s => s.Value ).ToString() + ",";
                }

                row += male.ToString() + ",";
                row += female.ToString() + ",";


                var RaceEthnicityValues = new List<RaceEthnicityValue>();

                foreach( var rev in userData.Revisions){
                    RaceEthnicityValues.AddRange(rev.RaceEthnicityValues);
                }

                foreach(var race in races){
                    foreach( var ethn in ethnicities){
                        row += RaceEthnicityValues.Where( v => v.EthnicityId == ethn.Id && v.RaceId == race.Id).Sum( s => s.Amount).ToString() + ",";
                    }
                }


                var withIndirect = userData.Revisions.Where( r => r.SnapIndirect != null);
                var indirects = 0;
                foreach( var ind in withIndirect){
                    indirects += ind.SnapIndirect.SnapIndirectReachedValues.Sum( s => s.Value);
                }
                row += indirects.ToString();
                result += row + "\n";
            }

            return Ok(result);
        }


        [HttpGet]
        [Route("indirectbyemployee/{fy}/data.csv")]
        //[Produces("text/csv")]
        [Authorize]
        public IActionResult IndirectByEmployee(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            keys.Add("FY");
            keys.Add("PlanningUnit");
            keys.Add("EmployeeName");
            keys.Add("Position");
            keys.Add("Program(s)");
            keys.Add("IndirectContacts");

            var reached = this.context.SnapIndirectReached.OrderBy( r => r.order);
            foreach( var r in reached){
                keys.Add(r.Name);
            }

            var result = string.Join(", ", keys.ToArray()) + "\n";

            var SnapData = this.SnapData( fiscalYear);

            var indirectSnapData = SnapData.Where( s => s.Revision.SnapIndirect != null && s.Revision.ActivityDate.Month == 11);

            var byUser = indirectSnapData.GroupBy( s => s.User.Id).Select( 
                                        d => new {
                                            User = d.Select( s => s.User ).First(),
                                            Revisions = d.Select( s => s.Revision )
                                        }
                                    )
                                    .OrderBy( d => d.User.RprtngProfile.PlanningUnit.Name).ThenBy( d => d.User.RprtngProfile.Name);
            foreach( var userData in byUser ){
                var row = fiscalYear.Name + ",";
                row += string.Concat("\"", userData.User.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                row += string.Concat("\"", userData.User.RprtngProfile.Name, "\"") + ",";
                row += string.Concat("\"", userData.User.ExtensionPosition.Code, "\"") + ",";
                var spclt = "";
                foreach( var sp in userData.User.Specialties){
                    spclt += " " + sp.Specialty.Code;
                }
                row += spclt + ", ";
                
                var optNumbrs = new List<ActivityOptionNumberValue>();
                

                var reachedData = new List<SnapIndirectReachedValue>();
                foreach( var dt in userData.Revisions){
                    optNumbrs.AddRange(dt.ActivityOptionNumbers);
                    reachedData.AddRange(dt.SnapIndirect.SnapIndirectReachedValues);
                }
                row += optNumbrs.Where( k =>k.ActivityOptionNumberId == 3).Sum( r => r.Value).ToString() + ",";
                foreach( var r in reached){
                    row += reachedData.Where( d => d.SnapIndirectReachedId == r.Id).Sum( l => l.Value).ToString() + ",";
                }
                result += row + "\n";
            }

            return Ok(result);
        }





        [HttpGet]
        [Route("directbypersonbymonth/{fy}/data.csv")]
        [Authorize]
        public IActionResult DirectSitesByPersonByMonth(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();

            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("PlanningUnit");
            keys.Add("PersonName");
            keys.Add("Position");
            keys.Add("Program");
            keys.Add("DirectDeliverySiteName");
            keys.Add("DirectSpecificSiteName");

            var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);
            
            foreach( var audnc in snapDirectAudience){
                keys.Add(audnc.Name);
            }


            var perPerson = context.Activity.
                                Where(e=>e.ActivityDate > fiscalYear.Start && e.ActivityDate < fiscalYear.End && e.Revisions.OrderBy(r => r.Created).Last().SnapDirect != null )
                                .Include( a => a.KersUser ).ThenInclude( u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit)
                                .Include( a => a.KersUser ).ThenInclude( u => u.ExtensionPosition)
                                .Include( a => a.KersUser).ThenInclude( u => u.Specialties ).ThenInclude( s => s.Specialty)
                                .OrderBy(e => e.ActivityDate.Month).ThenBy(e => e.KersUser.PersonalProfile.FirstName).ToList();
            
            
            var result = string.Join(",", keys.ToArray()) + "\n";



            foreach (var rw in perPerson){

                
                var lastRevision = this.context
                                            .ActivityRevision.Where( r => r.ActivityId == rw.Id )
                                            .Include( r => r.SnapDirect ).ThenInclude( d => d.SnapDirectAgesAudienceValues)
                                            .Include( r => r.SnapDirect ).ThenInclude( d => d.SnapDirectDeliverySite)
                                            .Include( r => r.SnapIndirect).ThenInclude( d => d.SnapIndirectReachedValues)
                                            .OrderBy( r => r.Created).LastOrDefault();

                
                var row = rw.ActivityDate.Year.ToString() + rw.ActivityDate.Month.ToString() + ",";
                row += rw.ActivityDate.ToString( "yyyy-MMM") + ",";
                row += string.Concat("\"", rw.KersUser.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                
                row +=  string.Concat("\"", rw.KersUser.RprtngProfile.Name, "\"")  + ",";
                row += rw.KersUser.ExtensionPosition.Code + ",";
                var spclt = "";
                foreach( var sp in rw.KersUser.Specialties){
                    spclt += " " + sp.Specialty.Code;
                }
                row += spclt + ", ";
                if(lastRevision.SnapDirect.SnapDirectDeliverySite != null){
                    row += string.Concat("\"", lastRevision.SnapDirect.SnapDirectDeliverySite.Name, "\"") + ",";
                }else{
                    row += ",";
                }
                row += string.Concat("\"",lastRevision.SnapDirect.SiteName, "\"") + ",";

                foreach( var audnc in snapDirectAudience){
                    var s = lastRevision.SnapDirect.SnapDirectAgesAudienceValues.Where( v => v.SnapDirectAudienceId == audnc.Id).Sum( v => v.Value);
                    row += s.ToString() + ",";
                }

                result += row + "\n";
            }
             return Ok(result);
        }


        [HttpGet]
        [Route("specificsitenamesbymonth/{fy}/data.csv")]
        [Authorize]        
        public IActionResult SpecificSiteNamesByMonth(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();

            keys.Add("YearMonth");
            keys.Add("Count");
            keys.Add("SpecificSiteName");
            var result = string.Join(",", keys.ToArray()) + "\n";

            var perPerson = context.Activity.
                                Where(e=>e.ActivityDate > fiscalYear.Start && e.ActivityDate < fiscalYear.End && e.Revisions.OrderBy(r => r.Created).Last().SnapDirect != null )
                                .Select( s => new {
                                    Last = s.Revisions.Where(r => true).OrderBy(r => r.Created).Last(),
                                    Snap = s.Revisions.Where(r => true).OrderBy(r => r.Created).Last().SnapDirect
                                })
                                .OrderBy(e => e.Last.ActivityDate.Month).ToList();
            
            
            var grouped = perPerson.Where( r => r.Snap!= null)
                            .GroupBy( p => p.Snap.SiteName)
                            .Select( s => new {
                                SiteName = s.Key,
                                Dt = s.OrderBy(l => l.Last.Id).First().Last.ActivityDate,
                                Count = s.Count()
                            });


            foreach( var k in grouped){
                var row = k.Dt.ToString("yyyyMM") + ",";
                row += k.Count.ToString() + ",";
                row += string.Concat( "\"", k.SiteName, "\"") + ",";
                result += row + "\n";
            }

             return Ok(result);
        }


        [HttpGet]
        [Route("individualcontacttotals/{fy}/data.csv")]
        [Authorize]
        public IActionResult IndividualContactTotals(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            
            keys.Add("FY");
            keys.Add("Name");
            keys.Add("PositionTitle");
            keys.Add("Program(s)");
            keys.Add("PlanningUnit");
            keys.Add("HoursReported");
            keys.Add("Indirect");
            keys.Add("Direct");
            
            var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);
            
            foreach( var audnc in snapDirectAudience){
                keys.Add(audnc.Name);
            }

            var snapDirectAges = this.context.SnapDirectAges.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order);
            
            foreach( var age in snapDirectAges){
                keys.Add(age.Name);
            }

            var result = string.Join(",", keys.ToArray()) + "\n";

            var perPerson = context.Activity.
                                Where(e=>e.ActivityDate > fiscalYear.Start && e.ActivityDate < fiscalYear.End && (e.Revisions.OrderBy(r => r.Created).Last().SnapDirect != null || e.Revisions.OrderBy(r => r.Created).Last().SnapIndirect != null) )
                                .Select( s => new {
                                    Last = s.Revisions.Where(r => true).OrderBy(r => r.Created).Last(),
                                    User = s.KersUser,
                                    Profile = s.KersUser.RprtngProfile,
                                    Unit = s.KersUser.RprtngProfile.PlanningUnit,
                                    Position = s.KersUser.ExtensionPosition
                                })
                                .OrderBy(e => e.User.RprtngProfile.Name).ToList();
            
            
            var grouped = perPerson.Where( r => true)
                            .GroupBy( p => p.User)
                            .Select( s => new {
                                User = s.Key,
                                Revs = s.Select( r => r.Last),
                                Profile = s.Select( r => r.Profile).First(),
                                Unit = s.Select( r => r.Unit).First(),
                                Position = s.Select( r => r.Position).First(),
                            });


            foreach( var k in grouped){
                var row = fiscalYear.Name + ",";
                row += string.Concat( "\"", k.Profile.Name, "\"") + ",";
                row += string.Concat( "\"", k.Position.Code, "\"") + ",";

                var spclt = "";
                var sp = this.context.KersUser.Where( r => r.Id == k.User.Id).Include( u => u.Specialties).ThenInclude( s => s.Specialty).FirstOrDefault();
                foreach( var s in sp.Specialties){
                    spclt += " " + s.Specialty.Code;
                }
                row += string.Concat( "\"", spclt, "\"") + ",";
                row += string.Concat( "\"", k.Unit.Name, "\"") + ",";
                row += k.Revs.Sum( r => r.Hours).ToString() + ",";


                var revIds = k.Revs.Select( r => r.Id);
                var indrAud = this.context.ActivityRevision.Where( r => revIds.Contains(r.Id) && r.SnapIndirect != null).Include( r => r.SnapIndirect).ThenInclude( i => i.SnapIndirectReachedValues);
                var inrVals = new List<SnapIndirectReachedValue>();
                foreach( var rvsn in indrAud){
                    inrVals.AddRange(rvsn.SnapIndirect.SnapIndirectReachedValues);
                }
                row += inrVals.Sum( s => s.Value).ToString() + ",";
                var dirAud = this.context.ActivityRevision.Where( r => revIds.Contains(r.Id) && r.SnapDirect != null).Include( r => r.SnapDirect).ThenInclude( d => d.SnapDirectAgesAudienceValues);
                var aavs = new List<SnapDirectAgesAudienceValue>();
                foreach( var rvsn in dirAud ){
                    aavs.AddRange( rvsn.SnapDirect.SnapDirectAgesAudienceValues );
                }

                row += aavs.Sum( s => s.Value).ToString() + ",";
                foreach( var audnc in snapDirectAudience){
                    row += aavs.Where( a => a.SnapDirectAudienceId == audnc.Id).Sum( s => s.Value ).ToString() + ",";
                }
                foreach( var age in snapDirectAges){
                    row += aavs.Where( a => a.SnapDirectAudienceId == age.Id).Sum( s => s.Value ).ToString() + ",";
                }
                result += row + "\n";
                 
            }

             return Ok(result);
        }


        [HttpGet]
        [Route("personnelhourdetails/{fy}/data.csv")]
        [Authorize]
        public IActionResult PersonnelHourDetails(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            		

            keys.Add("District");
            keys.Add("PlanningUnit");
            keys.Add("PersonID");
            keys.Add("Name");
            keys.Add("Title");
            keys.Add("Program(s)");
            keys.Add("StartDate");
            keys.Add("EndDate");


            var runningDate = fiscalYear.Start;
            var difference = (int)Math.Floor(fiscalYear.End.Subtract(fiscalYear.Start).Days / (365.2425 / 12)) + 1;
            var months = new DateTime[difference];
            var i = 0;
            do{
                months[i] = runningDate.AddMonths( i );
                keys.Add(months[i].ToString("MMM"));
                i++;
            }while(i < difference);

            keys.Add("ReportedHours");
            keys.Add("CommitmentHours");
            keys.Add("OverShort");


            var result = string.Join(",", keys.ToArray()) + "\n";

            var SnapData = this.SnapData( fiscalYear);


            var byUser = SnapData.GroupBy( s => s.User.Id).Select( 
                                        d => new {
                                            User = d.Select( s => s.User ).First(),
                                            Revisions = d.Select( s => s.Revision )
                                        }
                                    )
                                    .OrderBy(d => d.User.RprtngProfile.PlanningUnit.DistrictId).ThenBy( d => d.User.RprtngProfile.PlanningUnit.Name).ThenBy( d => d.User.RprtngProfile.Name);

            foreach( var userData in byUser){
                var row = userData.User.RprtngProfile.PlanningUnit.DistrictId + ",";
                row += string.Concat( "\"", userData.User.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                row += userData.User.RprtngProfile.PersonId + ",";
                row += string.Concat( "\"", userData.User.RprtngProfile.Name, "\"") + ",";
                row += string.Concat( "\"", userData.User.ExtensionPosition.Code, "\"") + ",";

                var spclt = "";
                foreach( var s in userData.User.Specialties){
                    spclt += " " + s.Specialty.Code;
                }
                row += string.Concat( "\"", spclt, "\"") + ",";
                
                var sapData = this.mainContext.SAP_HR_ACTIVE.Where( s => s.PersonID == userData.User.RprtngProfile.PersonId).FirstOrDefault();
                if(sapData != null){
                    row += sapData.BeginDate?.ToString("MM/dd/yy") + ","??",";
                    var endDate = sapData.EndDate;
                    if( endDate == null || endDate?.Year > 2300){
                        row += ",";
                    }else{
                        row += endDate?.ToString("MM/dd/yy") + ",";
                    }
                }else{
                    row += ",,";
                }
                float totalHours = 0;
                foreach( var month in months){
                    var hrs = userData.Revisions.Where( r => r.ActivityDate.Month == month.Month && r.ActivityDate.Year == month.Year).Sum( s => s.Hours);
                    totalHours += hrs;
                    row += hrs.ToString() + ",";
                }
                row += totalHours.ToString() + ",";
                var committed = this.context.SnapEd_Commitment.Where( c => c.KersUser.Id == userData.User.Id && c.FiscalYear == fiscalYear && c.SnapEd_ActivityType.Measurement == "Hour").Sum( m => m.Amount);
                row += committed.ToString() + ",";
                row += (committed - totalHours).ToString();
                result += row + "\n";
            }
            return Ok(result);
        }


        [HttpGet]
        [Route("numberofdeliverysitesbytypeofsetting/{fy}/data.csv")]
        [Authorize]
        public IActionResult NumberofDeliverySitesbyTypeofSetting(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
 
            keys.Add("FY");
            keys.Add("TypeOfSetting");



            var runningDate = fiscalYear.Start;
            var difference = (int)Math.Floor(fiscalYear.End.Subtract(fiscalYear.Start).Days / (365.2425 / 12)) + 1;
            var months = new DateTime[difference];
            var i = 0;
            do{
                months[i] = runningDate.AddMonths( i );
                keys.Add(months[i].ToString("MMM"));
                i++;
            }while(i < difference);

            var result = string.Join(",", keys.ToArray()) + "\n";
            var settings = this.context.SnapDirectDeliverySite.Where( d => d.FiscalYearId == fiscalYear.Id && d.Active).OrderBy(d => d.order).ToList();
            
            var snapPerMonth = new List<SnapDirect>[difference];
            for( i = 0; i< difference; i++){
                var activitiesPerMonth = context.Activity.Where( a => a.ActivityDate.Month == months[i].Month && a.ActivityDate.Year == months[i].Year);
                var activitiesWithSnapDirect = activitiesPerMonth
                                                .Select( v => v.Revisions.OrderBy( r => r.Created).Last())
                                                .Where( a => a.SnapDirect != null);
                snapPerMonth[i] = activitiesWithSnapDirect.Select( s => s.SnapDirect).ToList();
            }
            
            
            foreach( var setting in settings){
                var row = fiscalYear.Name + ",";
                row += setting.Name + ",";
                for( i = 0; i< difference; i++){
                    row += snapPerMonth[i].Where(s => s.SnapDirectDeliverySiteId == setting.Id).Count().ToString() + ",";
                }
                result += row + "\n";
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("methodsusedrecordcount/{fy}/data.csv")]
        [Authorize]
        public IActionResult MethodsUsedRecordCount(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            		
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            var methods = context.SnapIndirectMethod.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
            foreach( var met in methods){
                keys.Add(string.Concat( "\"", met.Name, "\""));
            }

            var result = string.Join(",", keys.ToArray()) + "\n";

            var perMonth = RevisionsWithIndirectContactsPerMonth( fiscalYear);
            foreach( var mnth in perMonth){
                var dt = mnth.Revs.Last().ActivityDate;
                var row = dt.ToString("yyyyMM") + ",";
                row += dt.ToString("yyyy-MMM") + ",";
                var ids = mnth.Revs.Select( r => r.Id);
                var indirects = context.ActivityRevision.Where( r => ids.Contains( r.Id ))
                            .Select( i => i.SnapIndirect.SnapIndirectMethodSelections  );
                var selections = new List<SnapIndirectMethodSelection>();
                foreach( var ind in indirects){
                    if(ind != null){
                        selections.AddRange( ind );
                    }
                    
                }       
                foreach( var mt in methods){
                    row += selections.Where( r => r.SnapIndirectMethodId == mt.Id).Count().ToString() + ",";
                }

                result += row + "\n";
            }

             return Ok(result);
        }


        [HttpGet]
        [Route("estimatedsizeofaudiencesreached/{fy}/data.csv")]
        [Authorize]
        public IActionResult EstimatedSizeofAudiencesReached(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            		
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            var methods = context.SnapIndirectReached.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
            foreach( var met in methods){
                keys.Add(string.Concat( "\"", met.Name, "\""));
            }

            var result = string.Join(",", keys.ToArray()) + "\n";

            var perMonth = RevisionsWithIndirectContactsPerMonth( fiscalYear);
            foreach( var mnth in perMonth){
                var dt = new DateTime(mnth.Year, mnth.Month, 15);
                var row = dt.ToString("yyyyMM") + ",";
                row += dt.ToString("yyyy-MMM") + ",";
                var ids = mnth.Revs.Select( r => r.Id);
                var indirects = context.ActivityRevision.Where( r => ids.Contains( r.Id ))
                            .Select( i => i.SnapIndirect.SnapIndirectReachedValues  );
                var selections = new List<SnapIndirectReachedValue>();
                foreach( var ind in indirects){
                    if(ind != null){
                        selections.AddRange( ind );
                    }
                    
                }       
                foreach( var mt in methods){
                    row += selections.Where( r => r.SnapIndirectReachedId == mt.Id).Sum( s => s.Value).ToString() + ",";
                }

                result += row + "\n";
            }
             return Ok(result);
        }



        [HttpGet]
        [Route("sessiontypebymonth/{fy}/data.csv")]
        [Authorize]
        public IActionResult SessionTypebyMonth(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
 
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            var types = context.SnapDirectSessionType.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
            foreach( var met in types){
                keys.Add(string.Concat( "\"", met.Name, " Number Delivered\""));
                keys.Add(string.Concat( "\"", met.Name, " Min Minutes\""));
                keys.Add(string.Concat( "\"", met.Name, " Miax Minutes\""));
            }
            keys.Add("MonthlyTotal");
            var result = string.Join(",", keys.ToArray()) + "\n";




            var perMonth = RevisionsWithDirectContactsPerMonth( fiscalYear);
            foreach( var mnth in perMonth){
                var dt = new DateTime(mnth.Year, mnth.Month, 15);
                var row = dt.ToString("yyyyMM") + ",";
                row += dt.ToString("yyyy-MMM") + ",";
                
                var ids = mnth.Revs.Select( r => r.Id);
                var MonthlyTotal = 0;
                foreach( var type in types){
                    var byType = context.ActivityRevision.Where( r => ids.Contains( r.Id) && r.SnapDirect.SnapDirectSessionTypeId == type.Id);
                    var cnt = byType.Count();
                    MonthlyTotal += cnt;
                    row += cnt.ToString() + ",";
                    if( cnt == 0){
                        row += ",,";
                    }else{
                        row += (byType.Min( t => t.Hours) * 60).ToString() + ",";
                        row += (byType.Max( t => t.Hours) * 60).ToString() + ",";
                    }
                }

                row += MonthlyTotal.ToString();
                result += row + "\n";
            }
             
             return Ok(result);
        }

        
        [HttpGet]
        [Route("agentcommunityeventdetail/{fy}/data.csv")]
        [Authorize]
        public IActionResult AgentCommunityEventDetail(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("PersonName");
            keys.Add("PlanningUnit");
            keys.Add("District");
            keys.Add("Program(s)");
            keys.Add("EventDate");
            keys.Add("Hours");


            var types = context.SnapPolicyAimed.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
            foreach( var met in types){
                keys.Add(string.Concat( "\"", met.Name, "\""));
            }
            keys.Add("PurposeGoal");
            keys.Add("ResultImpact");
            var result = string.Join(",", keys.ToArray()) + "\n";
            var activitiesThisFiscalYear = context.Activity.Where( a => a.ActivityDate > fiscalYear.Start && a.ActivityDate < fiscalYear.End);
            var activitiesWithPolicy = activitiesThisFiscalYear.Where( r => r.Revisions.Last().SnapPolicy != null).OrderBy( a => a.ActivityDate.Year).ThenBy( a => a.ActivityDate.Month).ThenBy(a => a.KersUser.PersonalProfile.FirstName);
            var policyMeetings = activitiesWithPolicy.Select(
                                    a => new {
                                        SnapPolicy = a.Revisions.OrderBy( r => r.Created).Last().SnapPolicy,
                                        ActivityDate = a.ActivityDate,
                                        PersonalProfile = a.KersUser.PersonalProfile,
                                        PlanningUnit = a.KersUser.RprtngProfile.PlanningUnit,
                                        Hours = a.Hours,
                                        Programs = a.KersUser.Specialties
                                    }
                                ).ToList();
            var specialties = context.Specialty.ToList();
            foreach( var meeting in policyMeetings){
                var row = meeting.ActivityDate.ToString("yyyyMM") + ",";
                row += meeting.ActivityDate.ToString("yyyy-MMM") + ",";
                row += meeting.PersonalProfile.FirstName + meeting.PersonalProfile.LastName + ",";
                row += meeting.PlanningUnit.Name + ",";
                row += meeting.PlanningUnit.DistrictId + ",";
                var prgrms = "";
                foreach( var program in meeting.Programs){
                    prgrms += specialties.Where( s => s.Id == program.SpecialtyId).FirstOrDefault() ?.Code + " "??"";
                }
                row += prgrms + ",";
                row += meeting.ActivityDate.ToString("mm/dd/yyy") + ",";
                row += meeting.Hours.ToString() + ",";
                var aimed = context.SnapPolicy.Where( p => p.Id == meeting.SnapPolicy.Id).Include( s => s.SnapPolicyAimedSelections).FirstOrDefault();
                foreach( var tp in types){
                    if( aimed.SnapPolicyAimedSelections == null){
                        row += ",";
                    }else{
                        var sels = aimed.SnapPolicyAimedSelections.Where( a => a.SnapPolicyAimedId == tp.Id).FirstOrDefault();
                        if( sels != null ){
                            row += "X,";
                        }else{
                            row += ",";
                        }
                    }
                }
                row += string.Concat( "\"", StripHTML(aimed.Purpose), "\"") + ",";
                row += string.Concat( "\"", StripHTML(aimed.Result), "\"") ;
                
                result += row + "\n";
            }
            return Ok(result);
        }


        [HttpGet]
        [Route("bypartnercategory/{fy}/data.csv")]
        [Authorize]
        public IActionResult ByPartnerCategory(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("PartnerName");
            keys.Add("NumberOfAgentsReporting");
            var result = string.Join(",", keys.ToArray()) + "\n";
            var lastActivityRevs = this.activityRepo.LastActivityRevisionIds(fiscalYear, _cache);
            var activitiesWithPolicy = context.ActivityRevision.Where( r => lastActivityRevs.Contains(r.Id) && r.SnapPolicy != null).OrderBy( a => a.ActivityDate.Year).ThenBy( a => a.ActivityDate.Month);
            var groupedByMonth = activitiesWithPolicy.GroupBy(
                                                        p => new {
                                                            Year = p.ActivityDate.Year,
                                                            Month = p.ActivityDate.Month
                                                        }
                                                )
                                                .Select(
                                                        k => new {
                                                            Month = k.Key.Month,
                                                            Year = k.Key.Year,
                                                            Revisions = k.Select( a => a)
                                                        }
                                                );
            var partners = this.context.SnapPolicyPartner.Where( p => p.Active && p.FiscalYear == fiscalYear).ToList();
            foreach( var byMonth in groupedByMonth){
                var revisionIds = byMonth.Revisions.Select( a => a.Id);
                var byPartner = context.ActivityRevision
                                                    .Where( r => revisionIds.Contains( r.Id ) )
                                                    .Select( r => r.SnapPolicy.SnapPolicyPartnerValue)
                                                    .ToList();
                var partnerValues = new List<SnapPolicyPartnerValue>();
                foreach( var byPartnr in byPartner){
                    partnerValues.AddRange( byPartnr);
                }
                var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                foreach( var partner in partners){
                    
                    var row = dt.ToString("yyyyMM") + ",";
                    row += dt.ToString("yyyy-MMM") + ",";
                    row += string.Concat( "\"", partner.Name, "\"") + ",";
                    row += partnerValues.Where( p => p.SnapPolicyPartnerId == partner.Id && p.Value != 0).Count().ToString();
                    result += row + "\n";
                }
            }
            return Ok(result);
        }


        [HttpGet]
        [Route("byaimedtowardsimprovement/{fy}/data.csv")]
        [Authorize]
        public IActionResult ByAimedTowardsImprovement(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("AimedTowardImprovementInName");
            keys.Add("NumberOfAgentsReporting");
            keys.Add("TotalHoursReported");


            var result = string.Join(",", keys.ToArray()) + "\n";
            var lastActivityRevs = this.activityRepo.LastActivityRevisionIds(fiscalYear, _cache);
            var activitiesWithPolicy = context.ActivityRevision.Where( r => lastActivityRevs.Contains(r.Id) && r.SnapPolicy != null).OrderBy( a => a.ActivityDate.Year).ThenBy( a => a.ActivityDate.Month);
            var groupedByMonth = activitiesWithPolicy.GroupBy(
                                                        p => new {
                                                            Year = p.ActivityDate.Year,
                                                            Month = p.ActivityDate.Month
                                                        }
                                                )
                                                .Select(
                                                        k => new {
                                                            Month = k.Key.Month,
                                                            Year = k.Key.Year,
                                                            Revisions = k.Select( a => a)
                                                        }
                                                );
            var partners = this.context.SnapPolicyAimed.Where( p => p.Active && p.FiscalYear == fiscalYear).ToList();
            foreach( var byMonth in groupedByMonth){
                var revisionIds = byMonth.Revisions.Select( a => a.Id);
                var byAimed = context.ActivityRevision
                                                    .Where( r => revisionIds.Contains( r.Id ) )
                                                    .Select( r => new {
                                                                Hours = r.Hours,
                                                                Aimed = r.SnapPolicy.SnapPolicyAimedSelections
                                                            }
                                                    ).ToList();
                
                
                
                var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                foreach( var partner in partners){
                    
                    var row = dt.ToString("yyyyMM") + ",";
                    row += dt.ToString("yyyy-MMM") + ",";

                    float totalHours = 0;
                    var totalMeetings = 0;
                    foreach( var revs in byAimed){
                        if( revs.Aimed != null){
                            var rv = revs.Aimed.Where( r => r.SnapPolicyAimedId == partner.Id).FirstOrDefault();
                            if(rv != null){
                                totalHours += revs.Hours;
                                totalMeetings++;
                            }
                        }
                    }

                    row += string.Concat( "\"", partner.Name, "\"") + ",";
                    row += totalMeetings.ToString() + ",";
                    row += totalHours.ToString();
                    result += row + "\n";
                }
            }
            return Ok(result);
        }


        [HttpGet]
        [Route("copiessummarybycountyagents/{fy}/data.csv")]
        [Authorize]
        public IActionResult CopiesSummarybyCountyAgents(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            
            return Ok( CopiesReportPerCounty( fiscalYear, 1) );
        }

        [HttpGet]
        [Route("copiessummarybycountynotagents/{fy}/data.csv")]
        [Authorize]
        public IActionResult CopiesSummarybyCountyNotAgents(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            
            return Ok( CopiesReportPerCounty( fiscalYear, 2) );
        }


        // type: 1 agents, 2 non agents
        private string CopiesReportPerCounty(FiscalYear fiscalYear, int type){

            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("PlanningUnit");
            keys.Add("NumberOfCopies");


            var result = string.Join(",", keys.ToArray()) + "\n";
            var lastActivityRevs = this.activityRepo.LastActivityRevisionIds(fiscalYear, _cache);

            var activitiesWithCopies = context.ActivityRevision
                                                .Where( r => 
                                                    
                                                        lastActivityRevs.Contains(r.Id) 
                                                            && 
                                                        r.SnapCopies != 0
                                                            &&
                                                            (
                                                                r.SnapAdmin
                                                                    ||
                                                                r.SnapDirect != null    
                                                                    ||
                                                                r.SnapIndirect != null
                                                                    ||
                                                                r.SnapPolicy != null
                                                            )
                                                                            
                                                                            
                                                                            );
            var groupedByMonth = activitiesWithCopies.GroupBy(
                                                        p => new {
                                                            Year = p.ActivityDate.Year,
                                                            Month = p.ActivityDate.Month
                                                        }
                                                )
                                                .Select(
                                                        k => new {
                                                            Month = k.Key.Month,
                                                            Year = k.Key.Year,
                                                            Revisions = k.Select( a => a)
                                                        }
                                                );
            foreach( var byMonth in groupedByMonth){
                var revisionIds = byMonth.Revisions.Select( a => a.Id);

                var activities = context.ActivityRevision.Where( r => revisionIds.Contains(r.Id))
                                    .Select(
                                        r => new UserRevisionData {
                                            Revision = r,
                                            User = context.Activity.Where( a => a.Id == r.ActivityId).FirstOrDefault().KersUser
                                        }
                                    )
                                    .ToList();
                
                var fullRevisions = new List<UserRevisionData>();
                foreach( var actvt in activities){
                    var usr = context.KersUser
                                        .Where( u => u.Id == actvt.User.Id )
                                        .Include( u => u.ExtensionPosition)
                                        .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit)
                                        .FirstOrDefault();
                    fullRevisions.Add(
                        new UserRevisionData{
                            Revision = actvt.Revision,
                            User = usr
                        }
                    );
                }

                IEnumerable<UserRevisionData> byUnit;

                if( type == 1){
                    byUnit = fullRevisions.Where( r => r.User.ExtensionPosition.Code == "AGENT");
                }else{
                    byUnit = fullRevisions.Where( r => r.User.ExtensionPosition.Code != "AGENT");
                }
                var grouppedByUnit = byUnit.GroupBy( r => r.User.RprtngProfile.PlanningUnit)
                                    .Select( g => new {
                                        Unit = g.Key,
                                        Copies = g.Select( r => r.Revision).Sum( s => s.SnapCopies)
                                    }).OrderBy( o => o.Unit.Name).ToList();

                var dt = new DateTime( byMonth.Year, byMonth.Month, 15);
                foreach( var unit in grouppedByUnit){
                    var row = dt.ToString("yyyyMM") + ",";
                    row += dt.ToString("yyyy-MMM") + ",";
                    row += string.Concat( "\"", unit.Unit.Name, "\"") + ",";
                    row += unit.Copies.ToString();
                    result += row + "\n";
                }
               
            }
            return result;
        }



        [HttpGet]
        [Route("copiesdetailagents/{fy}/data.csv")]
        [Authorize]
        public IActionResult CopiesDetailAgents(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            
            return Ok( CopiesReportDetails( fiscalYear, 1) );
        }

        [HttpGet]
        [Route("copiesdetailnotagents/{fy}/data.csv")]
        [Authorize]
        public IActionResult CopiesDetailANotAgents(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }
            
            return Ok( CopiesReportDetails( fiscalYear, 2) );
        }

        private string CopiesReportDetails(FiscalYear fiscalYear, int type){
	

            var keys = new List<string>();
            keys.Add("YearMonth");
            keys.Add("YearMonthName");
            keys.Add("PlanningUnit");
            keys.Add("PersonName");
            keys.Add("EventDate");
            keys.Add("NumberOfCopies");
            keys.Add("EntryDate");
            keys.Add("Mode");
            keys.Add("Title");
            keys.Add("Program(s)");


            var result = string.Join(",", keys.ToArray()) + "\n";
            var lastActivityRevs = this.activityRepo.LastActivityRevisionIds(fiscalYear, _cache);

            var activitiesWithCopies = context.ActivityRevision
                                            .Where( r => lastActivityRevs.Contains(r.Id) 
                                                            && 
                                                        r.SnapCopies != 0
                                                            &&
                                                            (
                                                                r.SnapAdmin
                                                                    ||
                                                                r.SnapDirect != null    
                                                                    ||
                                                                r.SnapIndirect != null
                                                                    ||
                                                                r.SnapPolicy != null
                                                            )
                                                        
                                                        );
            var revisionIds = activitiesWithCopies.Select( a => a.Id);
            var activities = context.ActivityRevision.Where( r => revisionIds.Contains(r.Id))
                                    .Select(
                                        r => new UserRevisionData {
                                            Revision = r,
                                            User = context.Activity.Where( a => a.Id == r.ActivityId).FirstOrDefault().KersUser
                                        }
                                    )
                                    .ToList();
            var fullRevisions = new List<UserRevisionData>();
            foreach( var actvt in activities){
                var usr = context.KersUser
                                    .Where( u => u.Id == actvt.User.Id )
                                    .Include( u => u.ExtensionPosition)
                                    .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit)
                                    .Include( u => u.Specialties).ThenInclude( s => s.Specialty)
                                    .FirstOrDefault();
                fullRevisions.Add(
                    new UserRevisionData{
                        Revision = actvt.Revision,
                        User = usr
                    }
                );
            }
            List<UserRevisionData> filteredRevisions;
            if( type == 1){
                filteredRevisions = fullRevisions.Where( r => r.User.ExtensionPosition.Code == "AGENT").ToList();
            }else{
                filteredRevisions = fullRevisions.Where( r => r.User.ExtensionPosition.Code != "AGENT").ToList();
            }
            var orderedRevisions = filteredRevisions.OrderBy(r => r.Revision.ActivityDate.Year).ThenBy( r => r.Revision.ActivityDate.Month ).ThenBy( r => r.User.RprtngProfile.PlanningUnit.Name ).ThenBy( r => r.User.RprtngProfile.Name);
            foreach( var rev in orderedRevisions){
                var dt = new DateTime( rev.Revision.ActivityDate.Year, rev.Revision.ActivityDate.Month, 15);
                var row = dt.ToString("yyyyMM") + ",";
                row += dt.ToString("yyyy-MMM") + ",";
                row += string.Concat( "\"", rev.User.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                row += string.Concat( "\"", rev.User.RprtngProfile.Name, "\"") + ",";
                row += rev.Revision.ActivityDate.ToString( "MM/dd/yyyy") + ",";
                row += rev.Revision.SnapCopies.ToString() + ",";
                row += string.Concat( "\"", rev.Revision.Created.ToString(), "\"" ) + ",";
                var mode = "";
                if(rev.Revision.SnapAdmin){
                    mode = "Admin";
                }else{
                    if( rev.Revision.SnapDirectId != null){
                        mode = "Direct";
                    }else if( rev.Revision.SnapPolicyId != null){
                        mode = "Commmunity";
                    }
                    if( rev.Revision.SnapIndirectId != null){
                        mode += " Indirect";
                    }
                }
                row += string.Concat( "\"", mode, "\"" ) + ",";
                row += rev.User.ExtensionPosition.Code + ",";
                var spclt = "";
                foreach( var sp in rev.User.Specialties){
                    spclt += " " + sp.Specialty.Code;
                }
                row += spclt;
                result += row + "\n";
            }

            return result;
        }


        private List<ActivityRevisionsPerMonth> RevisionsWithIndirectContactsPerMonth( FiscalYear fiscalYear){
            var perMonth = new List<ActivityRevisionsPerMonth>();
            var currentDate = DateTime.Now;
            var runningDate = fiscalYear.Start;
            var difference = (int)Math.Floor(fiscalYear.End.Subtract(fiscalYear.Start).Days / (365.2425 / 12)) + 1;
            var months = new DateTime[difference];
            var i = 0;
            do{
                months[i] = runningDate.AddMonths( i );
                if( months[i].Year < currentDate.Year || ( months[i].Year == currentDate.Year && months[i].Month <= currentDate.Month ) ){
                    var cacheKey = "IndirectActivityRevisionsPerMonth" + months[i].Month.ToString() + months[i].Year.ToString();
                    var cachedTypes = _cache.GetString(cacheKey);
                    var entity = new ActivityRevisionsPerMonth();
                    if (!string.IsNullOrEmpty(cachedTypes)){
                        entity = JsonConvert.DeserializeObject<ActivityRevisionsPerMonth>(cachedTypes);
                    }else{
                        var byMonth = context.Activity.Where( c => c.ActivityDate.Month == months[i].Month && c.ActivityDate.Year == months[i].Year);
                        var activityRevisionsPerMonty = byMonth
                                .Select( a => a.Revisions.OrderBy(r => r.Created).Last())
                                .Where( e => e.SnapIndirect != null)
                                .ToList();
                        entity.Revs = activityRevisionsPerMonty;
                        entity.Month = months[i].Month;
                        entity.Year = months[i].Year;

                        var yearDifference = currentDate.Year - months[i].Year;
                        var monthsDifference = currentDate.Month - months[i].Month;

                        var cachePeriod = Math.Floor( (float) (yearDifference * 10 + monthsDifference + 5) / 2 );
                        if(cachePeriod <= 0){
                            cachePeriod = 1;
                        }

                        var serializedActivities = JsonConvert.SerializeObject(entity);
                        _cache.SetString(cacheKey, serializedActivities, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cachePeriod)
                            });
                                    
                    }
                    perMonth.Add(entity);
                }
                i++;
            }while(i < difference);

            return perMonth;
        }

        private List<ActivityRevisionsPerMonth> RevisionsWithDirectContactsPerMonth( FiscalYear fiscalYear){
            var perMonth = new List<ActivityRevisionsPerMonth>();
            var currentDate = DateTime.Now;
            var runningDate = fiscalYear.Start;
            var difference = (int)Math.Floor(fiscalYear.End.Subtract(fiscalYear.Start).Days / (365.2425 / 12)) + 1;
            var months = new DateTime[difference];
            var i = 0;
            do{
                months[i] = runningDate.AddMonths( i );
                if( months[i].Year < currentDate.Year || ( months[i].Year == currentDate.Year && months[i].Month <= currentDate.Month ) ){
                    var cacheKey = "DirectActivityRevisionsPerMonth" + months[i].Month.ToString() + months[i].Year.ToString();
                    var cachedTypes = _cache.GetString(cacheKey);
                    var entity = new ActivityRevisionsPerMonth();
                    if (!string.IsNullOrEmpty(cachedTypes)){
                        entity = JsonConvert.DeserializeObject<ActivityRevisionsPerMonth>(cachedTypes);
                    }else{
                        var byMonth = context.Activity.Where( c => c.ActivityDate.Month == months[i].Month && c.ActivityDate.Year == months[i].Year);
                        var activityRevisionsPerMonty = byMonth
                                .Select( a => a.Revisions.OrderBy(r => r.Created).Last())
                                .Where( e => e.SnapDirect != null)
                                .ToList();
                        entity.Revs = activityRevisionsPerMonty;
                        entity.Month = months[i].Month;
                        entity.Year = months[i].Year;

                        var yearDifference = currentDate.Year - months[i].Year;
                        var monthsDifference = currentDate.Month - months[i].Month;

                        var cachePeriod = Math.Floor( (float) (yearDifference * 10 + monthsDifference + 5) / 2 );
                        if(cachePeriod <= 0){
                            cachePeriod = 1;
                        }

                        var serializedActivities = JsonConvert.SerializeObject(entity);
                        _cache.SetString(cacheKey, serializedActivities, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cachePeriod)
                            });           
                    }
                    perMonth.Add(entity);
                }
                i++;
            }while(i < difference);

            return perMonth;
        }






        [HttpGet]
        [Route("reimbursementnepassistants/{fy}/data.csv")]
        [Authorize]
        public IActionResult ReimbursementNepAssistants(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();

            keys.Add("FY");
            keys.Add("AssistantName");
            keys.Add("PlanningUnit");
            keys.Add("ReimbursementsYearToDateTotal");
            keys.Add("BudgetRemaining");


            var result = string.Join(",", keys.ToArray()) + "\n";

            //List<KersUser> assistants;
            var assistants = this.context.KersUser.
                            Where(c=> (
                                c.Specialties.Where(s => s.Specialty.Name == "Expanded Food and Nutrition Education Program").Count() != 0 
                                ||
                                c.Specialties.Where(s => s.Specialty.Name == "Supplemental Nutrition Assistance Program Education").Count() != 0
                                )
                                &&
                                c.RprtngProfile.enabled
                            ).
                            Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                            Include(u=>u.PersonalProfile).
                            OrderBy(c => c.RprtngProfile.Name);

            var allowance = context.SnapBudgetAllowance.Where( a => a.FiscalYear == fiscalYear && a.BudgetDescription == "SNAP Ed NEP Assistant Budget").First().AnnualBudget;
            foreach( var assistant in assistants){
                var row = fiscalYear.Name + ",";
                row += string.Concat( "\"", assistant.RprtngProfile.Name, "\"") + ",";
                row += string.Concat( "\"", assistant.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                var reimbursement = context.SnapBudgetReimbursementsNepAssistant.Where( r => r.FiscalYear == fiscalYear && r.To == assistant).Sum( r => r.Amount);
                row += reimbursement.ToString() + ",";
                row += (allowance - reimbursement).ToString();
                result += row + "\n";
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("reimbursementcounty/{fy}/data.csv")]
        [Authorize]
        public IActionResult ReimbursementCounty(string fy){

            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", LogType, "Error");
                return new StatusCodeResult(500);
            }

            var keys = new List<string>();
			
            keys.Add("PlanningUnit");
            keys.Add("ReimbursementsYearToDateTotal");
            keys.Add("BudgetRemaining");


            var result = string.Join(",", keys.ToArray()) + "\n";


            List<PlanningUnit> counties;

           
            
            var cacheKey = "CountiesList";
            var cached = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cached)){
                counties = JsonConvert.DeserializeObject<List<PlanningUnit>>(cached);
            }else{
            
            
                counties = this.context.PlanningUnit.
                                Where(c=>c.District != null && c.Name.Substring(c.Name.Count() - 3) == "CES").
                                OrderBy(c => c.Name).ToList();
                

                var serializedCounties = JsonConvert.SerializeObject(counties);
                _cache.SetString(cacheKey, serializedCounties, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                        });
            }
            var allowance = context.SnapBudgetAllowance.Where( a => a.FiscalYear == fiscalYear && a.BudgetDescription == "SNAP Ed County Budget (separate from NEP Assistant Budget)").First().AnnualBudget;
            foreach( var county in counties){
                
                var row = string.Concat( "\"", county.Name.Substring(0, county.Name.Count() - 11), "\"") + ",";
                var reimbursement = context.SnapBudgetReimbursementsCounty.Where( r => r.FiscalYear == fiscalYear && r.PlanningUnitId == county.Id).Sum( r => r.Amount);
                row += reimbursement.ToString() + ",";
                var countyAllowance = context.SnapCountyBudget.Where( b => b.PlanningUnitId == county.Id && b.FiscalYear == fiscalYear).FirstOrDefault();
                var thisAllowance = allowance;
                if( countyAllowance != null){
                    thisAllowance = countyAllowance.AnnualBudget;
                }
                row += (thisAllowance - reimbursement).ToString();
                result += row + "\n";
            }
            return Ok(result);
        }




        private List<UserRevisionData> SnapData( FiscalYear fiscalYear){
            var today = DateTime.Now;
            var revs = activityRepo.LastActivityRevisionIds(fiscalYear, _cache);

            var snapEligible = context.ActivityRevision.Where( r => revs.Contains( r.Id ) &&  (r.SnapPolicy != null || r.SnapDirect != null || r.SnapIndirect != null || r.SnapAdmin ));


            List<UserRevisionData> SnapData = new List<UserRevisionData>();

            foreach( var rev in snapEligible ){


                
                var cacheKey = "UserRevisionWithSnapData" + rev.Id.ToString();
                var cacheString = _cache.GetString(cacheKey);
                UserRevisionData data;
                if (!string.IsNullOrEmpty(cacheString)){
                    data = JsonConvert.DeserializeObject<UserRevisionData>(cacheString);
                }else{
                    
                    data = new UserRevisionData();
                    var activity = context.Activity.Where( a => a.Id == rev.ActivityId )
                                    .Include( a => a.KersUser ).ThenInclude( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                                    .Include( a => a.KersUser ).ThenInclude( u => u.ExtensionPosition)
                                    .Include( a => a.KersUser).ThenInclude( u => u.Specialties).ThenInclude( s => s.Specialty)
                                    .FirstOrDefault();
                    var revision = context.ActivityRevision.Where( r => r.Id == rev.Id)
                                        .Include( s => s.SnapDirect).ThenInclude( d => d.SnapDirectAgesAudienceValues )
                                        .Include( s => s.SnapIndirect).ThenInclude( i => i.SnapIndirectReachedValues)
                                        .Include( s => s.RaceEthnicityValues)
                                        .Include( s => s.ActivityOptionNumbers).FirstOrDefault();
                    
                    data.User = activity.KersUser;
                    data.Revision = revision;


                    var expiration = (today - activity.ActivityDate).Days;
                    if( expiration < 1){
                        expiration = 1;
                    }

                    var serialized = JsonConvert.SerializeObject(data);
                    _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( expiration )
                        }); 
                }

                

                
                SnapData.Add(data);
            }
            return SnapData;
        }


        private int ContactsPerRaceEthnicity( List<RaceEthnicityValue> vals, Race race, Ethnicity ethnicity){
            return vals.Where( v => v.Race == race && v.Ethnicity == ethnicity).Sum(v => v.Amount);
        }

        private int ContactsPerSnapDirectAudience(List<SnapDirectAgesAudienceValue> vals, SnapDirectAudience audience){
                
                return vals.Where(v => v.SnapDirectAudience == audience).Sum( v => v.Value );

        }

        private int ContactsPerSnapDirectAge(List<SnapDirectAgesAudienceValue> vals, SnapDirectAges ages){
                
                return vals.Where(v => v.SnapDirectAges == ages).Sum( v => v.Value );

        }
        private string StripHTML(string htmlString){

            string pattern = @"<(.|\n)*?>";

            return Regex.Replace(htmlString, pattern, string.Empty);
        }

        public FiscalYear GetFYByName(string fy, string type = "snapEd"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalRepo.currentFiscalYear(type);
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == "snapEd").FirstOrDefault();
            }
            return fiscalYear;
        }
    
    }

    class UserRevisionData{
        public KersUser User;
        public ActivityRevision Revision;
    }

}