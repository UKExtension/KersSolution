using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Data;
using Kers.Models.Contexts;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Kers.Models.Repositories
{
    public class SnapDirectRepository : SnapBaseRepository, ISnapDirectRepository
    {

        private KERScoreContext context;
        private KERSmainContext mainContext;
        private IDistributedCache _cache;
        public SnapDirectRepository(
            KERScoreContext context, 
            IDistributedCache _cache,
            KERSmainContext mainContext
            )
            : base(context, _cache)
        { 
            this.context = context;
            this._cache = _cache;
            this.mainContext = mainContext;
        }

        public string IndividualContactTotals(FiscalYear fiscalYear, Boolean refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapIndividualContactTotals + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{


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

                result = string.Join(",", keys.ToArray()) + "\n";

                var perPerson = context.Activity.
                                    Where(e=>e.ActivityDate > fiscalYear.Start && e.ActivityDate < fiscalYear.End && (e.Revisions.OrderBy(r => r.Created.ToString("s")).Last().SnapDirect != null || e.Revisions.OrderBy(r => r.Created.ToString("s")).Last().SnapIndirect != null) )
                                    .Select( s => new {
                                        Last = s.Revisions.Where(r => true).OrderBy(r => r.Created.ToString("s")).Last(),
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

                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    });

            }

            return result;
        }

        public string EstimatedSizeofAudiencesReached(FiscalYear fiscalYear, Boolean refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapEstimatedSizeofAudiencesReached + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{

                var keys = new List<string>();
                        
                keys.Add("YearMonth");
                keys.Add("YearMonthName");
                var methods = context.SnapIndirectReached.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
                foreach( var met in methods){
                    keys.Add(string.Concat( "\"", met.Name, "\""));
                }

                result = string.Join(",", keys.ToArray()) + "\n";

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
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    }); 
            }
            return result;
        }

        public string SessionTypebyMonth(FiscalYear fiscalYear, Boolean refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapSessionTypebyMonth + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{

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
                result = string.Join(",", keys.ToArray()) + "\n";

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
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    }); 
            }
            return result;
        }
        public string MethodsUsedRecordCount(FiscalYear fiscalYear, Boolean refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapMethodsUsedRecordCount + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{

                var keys = new List<string>();
                        
                keys.Add("YearMonth");
                keys.Add("YearMonthName");
                var methods = context.SnapIndirectMethod.Where(m => m.Active && m.FiscalYear == fiscalYear).OrderBy( m => m.order);
                foreach( var met in methods){
                    keys.Add(string.Concat( "\"", met.Name, "\""));
                }

                result = string.Join(",", keys.ToArray()) + "\n";

                var perMonth = RevisionsWithIndirectContactsPerMonth( fiscalYear);
                foreach( var mnth in perMonth){
                    if(mnth.Revs.Count > 0){
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
                }
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    }); 
            }
            return result;
        }

        public string SpecificSiteNamesByMonth(FiscalYear fiscalYear, Boolean refreshCache = false){

            string result;
            var cacheKey = CacheKeys.SnapSpecificSiteNamesByMonth + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();

                keys.Add("YearMonth");
                keys.Add("Count");
                keys.Add("SpecificSiteName");
                result = string.Join(",", keys.ToArray()) + "\n";
                var perPerson = this.SnapData(fiscalYear).Where( d => d.Revision.SnapDirect != null).Select( s => new {
                                        Last = s.Revision,
                                        Snap = s.Revision.SnapDirect
                                    }).ToList();
                
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


                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    }); 
            }
            return result;
        }

        public string NumberofDeliverySitesbyTypeofSetting(FiscalYear fiscalYear, Boolean refreshCache = false){
            string result;
            var cacheKey = CacheKeys.NumberofDeliverySitesbyTypeofSetting + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{


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

                    result = string.Join(",", keys.ToArray()) + "\n";
                    var settings = this.context.SnapDirectDeliverySite.Where( d => d.FiscalYearId == fiscalYear.Id && d.Active).OrderBy(d => d.order).ToList();
                    
                    var snapPerMonth = new List<int>[difference];
                    for( i = 0; i< difference; i++){
                        var activitiesPerMonth = context.Activity.Where( a => a.ActivityDate.Month == months[i].Month && a.ActivityDate.Year == months[i].Year);
                        var activitiesWithSnapDirect = activitiesPerMonth
                                                        .Select( v => v.Revisions.OrderBy( r => r.Created.ToString("s")).Last())
                                                        .Where( a => a.SnapDirect != null)
                                                        .ToList();
                        snapPerMonth[i] = activitiesWithSnapDirect.Select( s => s.SnapDirectId??0 ).Where( a => a != 0).ToList();
                    }
                    
                    
                    foreach( var setting in settings){
                        var row = fiscalYear.Name + ",";
                        row += setting.Name + ",";
                        for( i = 0; i< difference; i++){
                                var directs = context.SnapDirect.Where(s => snapPerMonth[i].Contains(s.Id) );
                                row +=  directs.Where( s => s.SnapDirectDeliverySiteId == setting.Id).Count().ToString() + ",";
                        }
                        result += row + "\n";
                }
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    }); 

            }
            return result;
        }

        public string TotalByMonth(FiscalYear fiscalYear, Boolean refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapEdTotalByMonth + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                
                /*********************************/

                // Build result from data source 

                /*********************************/
                var keys = new List<string>();
                keys.Add("YearMonth");
                keys.Add("YearMonthName");
                keys.Add("HoursReported");
                keys.Add("DirectContacts");

                var snapDirectAudience = this.context.SnapDirectAudience.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order).ToList();
                
                foreach( var audnc in snapDirectAudience){
                    keys.Add(audnc.Name);
                }
                var snapDirectAges = this.context.SnapDirectAges.Where(a => a.FiscalYear == fiscalYear && a.Active).OrderBy(a => a.order).ToList();
                foreach( var ags in snapDirectAges){
                    keys.Add(ags.Name);
                }
                keys.Add("Male");
                keys.Add("Female");
                var races = this.context.Race.ToList();
                var ethnicities = this.context.Ethnicity;

                foreach(var race in races){
                    foreach( var ethn in ethnicities){
                        keys.Add( race.Name + ethn.Name);
                    }
                }
                keys.Add("IndirectContacts");
                result = string.Join(", ", keys.ToArray()) + "\n";
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
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    }); 
            }
            return result;
        }

        public string TotalByCounty(FiscalYear fiscalYear, Boolean refreshCache = false){

            string result;
            var cacheKey = CacheKeys.SnapEdTotalByCounty + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                /*********************************/

                // Build result from data source 

                /*********************************/
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
                result = string.Join(", ", keys.ToArray()) + "\n";


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
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    }); 
            }
            return result;
        }

        public string TotalByEmployee(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapEdTotalByEmployee + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                /*********************************/

                // Build result from data source 

                /*********************************/
            
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

                result = string.Join(", ", keys.ToArray()) + "\n";

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
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    }); 
            
            
            }
            return result;
        }

        public string AimedTowardsImprovement(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.AimedTowardsImprovement + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();
                keys.Add("YearMonth");
                keys.Add("YearMonthName");
                keys.Add("AimedTowardImprovementInName");
                keys.Add("NumberOfAgentsReporting");
                keys.Add("TotalHoursReported");
                result = string.Join(",", keys.ToArray()) + "\n";
                var revis = SnapData(fiscalYear);
                var activitiesWithPolicy = revis.Where( r => r.Revision.SnapPolicy != null).OrderBy( a => a.Revision.ActivityDate.Year).ThenBy( a => a.Revision.ActivityDate.Month);
                var groupedByMonth = activitiesWithPolicy.GroupBy(
                                                            p => new {
                                                                Year = p.Revision.ActivityDate.Year,
                                                                Month = p.Revision.ActivityDate.Month
                                                            }
                                                    )
                                                    .Select(
                                                            k => new {
                                                                Month = k.Key.Month,
                                                                Year = k.Key.Year,
                                                                Revisions = k.Select( a => a.Revision)
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
                _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                    });

            }
            return result;
        }

        public string PersonalHourDetails(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.PersonalHourDetails + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
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


                result = string.Join(",", keys.ToArray()) + "\n";

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
                    row += (totalHours - committed).ToString();
                    result += row + "\n";
                    _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                        });
                }
            }
            return result;
        }

        public string SitesPerPersonPerMonth(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SitesPerPersonPerMonth + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
            
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
                result = string.Join(",", keys.ToArray()) + "\n";
                var SnapData = this.SnapData( fiscalYear);
                var perPerson = SnapData.Where( a => a.Revision.SnapDirect != null)
                                .OrderBy(e => e.Revision.ActivityDate.Month).ThenBy(e => e.User.PersonalProfile.FirstName);
                foreach (var rw in perPerson){    
                    var lastRevision = this.context
                                                .ActivityRevision.Where( r => r.Id == rw.Revision.Id )
                                                .Include( r => r.SnapDirect ).ThenInclude( d => d.SnapDirectAgesAudienceValues)
                                                .Include( r => r.SnapDirect ).ThenInclude( d => d.SnapDirectDeliverySite)
                                                .OrderBy( r => r.Created).LastOrDefault();
                    var row = rw.Revision.ActivityDate.Year.ToString() + rw.Revision.ActivityDate.Month.ToString() + ",";
                    row += rw.Revision.ActivityDate.ToString( "yyyy-MMM") + ",";
                    row += string.Concat("\"", rw.User.RprtngProfile.PlanningUnit.Name, "\"") + ",";
                    
                    row +=  string.Concat("\"", rw.User.RprtngProfile.Name, "\"")  + ",";
                    row += rw.User.ExtensionPosition.Code + ",";
                    var spclt = "";
                    foreach( var sp in rw.User.Specialties){
                        spclt += " " + (sp.Specialty.Code.Substring(0, 4) == "prog"?sp.Specialty.Code.Substring(4):sp.Specialty.Code);
                    }
                    row += spclt + ", ";
                    if( lastRevision.SnapDirect.SnapDirectDeliverySite != null){
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
                    _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 12 )
                        }); 
                }
            }
            return result;
        }

    }


}