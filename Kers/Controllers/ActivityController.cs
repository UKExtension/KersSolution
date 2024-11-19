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
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Kers.Models.Data;
using CsvHelper.Configuration;
using System.IO;
using System.Text;
using CsvHelper;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class ActivityController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        IActivityRepository activityRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        private IMemoryCache _cache;
        private IDistributedCache _distributedCache;
        public ActivityController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IActivityRepository activityRepo,
                    ILogRepository logRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IMemoryCache memoryCache,
                    IDistributedCache _distributedCache
            ){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.activityRepo = activityRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           _cache = memoryCache;
           this._distributedCache = _distributedCache;
        }

/* 
        [HttpGet]
        [Route("ms4/data.csv")]
        [Produces("text/csv")]
        public async Task<IActionResult> Ms4Data(){

            string[] agents = new string[]
            {
                "tyankey",
                "alst259",
                "casc243",
                "sjwhite",
                "rmcbr2",
                "dkoester",
                "lbbowlin",
                "lharned",
                "jlittle",
                "aeam222",
                "lrgeor2",
                "mtch226",
                "djscully",
                "smstol2",
                "krjack4",
                "mlfu225",
                "dhda222",
                "cast247",
                "chardy",
                "ameyer",
                "jadock3",
                "ncarter",
                "ajle227",
                "sran223",
                "lsexton",
                "aaldende",
                "dshepher",
                "mdad223",
                "caha245",
                "pri223",
                "eply223",
                "clda235",
                "pwlong",
                "bppr223",
                "slmu223",
                "mast263",
                "tmmiss2",
                "kawi223",
                "arferg0",
                "bgsear2",
                "amills",
                "robsmith",
                "bgwilson",
                "sflynt",
                "clferr4",
                "kjba237",
                "phardest",
                "jcoles",
                "kcgo222",
                //"idene3"
            };

            List<Object> Output = new List<Object>();
            foreach( var agntId in agents){
                var usr = await context.KersUser.Where( u => u.RprtngProfile.LinkBlueId == agntId)
                                .Include( u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit )
                                .FirstOrDefaultAsync();
                if( usr != null){

                    var activities = context.Activity.Where( a => a.KersUser == usr && a.ActivityDate.Year == 2018);
                    foreach( var act in activities){
                        var last = await context.ActivityRevision
                                        .Where(a => a.ActivityId == act.Id)
                                        .Include( a => a.ActivityOptionNumbers).ThenInclude( n => n.ActivityOptionNumber)
                                        .Include( a => a.MajorProgram)
                                        .OrderBy(a => a.Created).LastAsync();
                        var indirect = last.ActivityOptionNumbers.Where(n => n.ActivityOptionNumber.Name == "Number of Indirect Contacts").FirstOrDefault();
                        var ind = 0;
                        if(indirect != null){
                            ind = indirect.Value;
                        }
                        var lst = new {
                            userid = usr.Id,
                            name = usr.RprtngProfile.Name,
                            countyId = usr.RprtngProfile.PlanningUnitId,
                            county = usr.RprtngProfile.PlanningUnit.Name,
                            date = act.ActivityDate.ToShortDateString(),
                            title = act.Title,
                            description = Kers.HtmlHelpers.StripHtmlHelper.StripHtml(last.Description),
                            MajorProgramId = act.MajorProgramId,
                            MajorProgram = act.MajorProgram.Name,
                            direct = act.Audience,
                            indirect = ind
                        };
                        Output.Add( lst );
                    }
                }
            }

            return Ok(Output);
        }
 */


        [HttpPost("GetCustomData/{userID?}")]
        [Authorize]
        public async Task<IActionResult> GetCustomData([FromBody] ActivitySearchCriteria criteria, int? userID = null ){
            var ret = new List<List<string>>();
            var result = await SeearchResults(criteria, userID);
            var races = context.Race.OrderBy( r => r.Order).ToList();
            var ethnicities = context.Ethnicity.OrderBy( r => r.Order).ToList();
            var options = context.ActivityOption.OrderBy( o => o.Order).ToList();
            var optionNumbers = context.ActivityOptionNumber.OrderBy( v => v.Order).ToList();
            var ages = context.SnapDirectAges.Where( a => a.Active).OrderBy( a => a.order).ToList();
            var audience = context.SnapDirectAudience.Where( a => a.Active).OrderBy( a => a.order).ToList();
            foreach( var res in result.Results){
                ret.Add( activityRepo.ReportRow(
                                            res.Revision.ActivityId,
                                            null,
                                            races,
                                            ethnicities,
                                            options,
                                            optionNumbers,
                                            ages,
                                            audience
                                        )      
                        );
            }

            return new OkObjectResult(ret);
        }
        [HttpGet("GetCustomDataHeader")]
        [Authorize]
        public IActionResult GetCustomDataHeader(  ){
            return new OkObjectResult( activityRepo.ReportHeaderRow() );
        }

        [HttpPost("GetCustom/{userID?}")]
        [Authorize]
        public async Task<IActionResult> GetCustom( [FromBody] ActivitySearchCriteria criteria, int? userID = null
                                        ){
            var ret = await SeearchResults(criteria, userID);
            if( criteria.Skip == 0 ){
                this.Log( criteria ,"ActivitySearchCriteria", "Custom Activities Report Initiated" + (userID == null ? " by an Admin" : " by User"), "ActivitySearchCriteria", "Info");
                this.context.SaveChanges();
            }
            
            return new OkObjectResult(ret);
        }
        private async Task<ActivitySeearchResultsWithCount> SeearchResults(ActivitySearchCriteria criteria, int? userID = null){
            var result = this.context.Activity.AsNoTracking()
                                .Where( a => a.ActivityDate >= criteria.Start && a.ActivityDate <= criteria.End);
            if( userID != null ){
                if(userID == 0 ){
                    KersUser user = this.CurrentUser();
                    userID = user.Id;
                }
                result = result.Where( a => a.KersUserId == userID );
            }
            if( criteria.Search != ""){
                result = result.Where( a => a.KersUser.RprtngProfile.Name.Contains(criteria.Search));
            }
            if( criteria.CongressionalDistrictId != null && criteria.CongressionalDistrictId != 0){
                result = result.Where( a => a.PlanningUnit.CongressionalDistrictUnit.CongressionalDistrictId == criteria.CongressionalDistrictId);
            }
            if(criteria.RegionId != null && criteria.RegionId != 0){
                result = result.Where( a => a.PlanningUnit.ExtensionArea.ExtensionRegionId == criteria.RegionId);
            }
            if(criteria.AreaId != null && criteria.AreaId != 0){
                result = result.Where( a => a.PlanningUnit.ExtensionAreaId == criteria.AreaId);
            }
            if( criteria.UnitId != null && criteria.UnitId != 0){
                result = result.Where( a => a.PlanningUnitId == criteria.UnitId);
            }
            if( criteria.PositionId != null && criteria.PositionId != 0){
                result = result.Where( a => a.KersUser.ExtensionPositionId == criteria.PositionId);
            }
         /*    if( criteria.SpecialtyId != null && criteria.SpecialtyId != 0){
                var spec = new Specialty();
                spec.Id = criteria.SpecialtyId??0;
                result = result.Where( a => a.KersUser.Specialties.Contains( spec  ));
            } */
            result = result
                        .Include( a=>a.LastRevision).ThenInclude(r => r.ActivityOptionSelections)
                        .Include( a => a.KersUser).ThenInclude( u => u.Specialties);
            var LastRevs = new List<ActivityRevision>();
            if( criteria.SpecialtyId != null && criteria.SpecialtyId != 0){
                foreach( var res in result){
                    if(res.KersUser.Specialties.Select( s => s.SpecialtyId).ToList().Contains(criteria.SpecialtyId??0)){
                        LastRevs.Add(res.LastRevision);
                    }
                }
            }else{
                foreach( var res in result){
                    LastRevs.Add(res.LastRevision);
                } 
            }
            
            var searchResult = new List<ActivitySearchResult>();
            var ret = new ActivitySeearchResultsWithCount();
            var skipped = 0;
            var taken = 0;
            IEnumerable<ActivityRevision> filtered = LastRevs;
            if(criteria.Options != null && criteria.Options.Count() > 0){
                filtered = filtered.Where( r => r.ActivityOptionSelections.Any( a => criteria.Options.Contains( a.ActivityOptionId) ) );
            }
            ret.ResultsCount =  filtered == null ? 0 : filtered.Count() ;
            if(criteria.Order == "asc"){
                filtered = filtered.OrderBy(r => r.ActivityDate);
            }else if(criteria.Order == "dsc" ){
                filtered = filtered.OrderByDescending( r => r.ActivityDate);
            }else{
                filtered = filtered.OrderBy( r => r.Title);
            }
            foreach( var rev in filtered){
                skipped++;
                if( criteria.Skip < skipped){
                    if( taken >= criteria.Take) break;
                    var res = new ActivitySearchResult();
                    var activity = await this.context.Activity.AsNoTracking().Where( a => a.Id == rev.ActivityId)
                                                .Include( a => a.KersUser).ThenInclude( u => u.RprtngProfile)
                                                .Include( a => a.PlanningUnit)
                                                .FirstOrDefaultAsync();
                    res.User = activity.KersUser;
                    res.Unit = activity.PlanningUnit;
                    res.Unit.GeoFeature = null;
                    res.Revision = rev;
                    searchResult.Add(res);
                    taken++;
                }      
            }
            ret.Results = searchResult;
            return ret;
        }

        
        public class ActivitySearchCriteria{
            public DateTime Start;
            public DateTime End;
            public string Search;
            public int[] Options;
            public string Order;
            public int? CongressionalDistrictId;
            public int? RegionId;
            public int? AreaId;
            public int? UnitId;
            public int? PositionId;
            public int? SpecialtyId;
            public int? Skip;
            public int? Take;

        }


        class ActivitySearchResult{
            public KersUser User;
            public ActivityRevision Revision;
            public PlanningUnit Unit;
        }
        class ActivitySeearchResultsWithCount{
            public List<ActivitySearchResult> Results;
            public int ResultsCount;
        }




        [HttpGet("numb")]
        [Authorize]
        public IActionResult GetNumb(){
            
            var numActivities = context.Activity.
                                Where(e=>e.KersUser == this.CurrentUser());
            
            return new OkObjectResult(numActivities.Count());
        }

        [HttpGet("latest/{skip?}/{amount?}/{userId?}")]
        [Authorize]
        public IActionResult Get(int skip = 0, int amount = 10, int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var lastActivities = context.Activity.
                                Where(e=>e.KersUser.Id == userId).
                                OrderByDescending(e=>e.ActivityDate).
                                Include(e=>e.LastRevision).ThenInclude(r => r.ActivityOptionSelections).
                                Include(e=>e.LastRevision).ThenInclude(r => r.ActivityOptionNumbers).
                                Include(e=>e.LastRevision).ThenInclude(r => r.RaceEthnicityValues).ThenInclude(r => r.Race).
                                Include(e=>e.LastRevision).ThenInclude(r => r.RaceEthnicityValues).ThenInclude(r => r.Ethnicity).
                                AsSplitQuery().
                                Skip(skip).
                                Take(amount);
            
            var revs = lastActivities.Select( a => a.LastRevision );
            foreach( var a in revs){
                a.RaceEthnicityValues = a.RaceEthnicityValues.
                                            OrderBy(r => r.Race.Order).
                                            ThenBy(e => e.Ethnicity.Order).
                                            ToList();
            }
            return new OkObjectResult(revs);
        }

        [HttpGet("years/{userId?}")]
        [Authorize]
        public IActionResult GetYears(int userId = 0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var years = context.Activity.
                                Where(e=>e.KersUser.Id == userId).
                                GroupBy(e => new {
                                    Year = e.ActivityDate.Year
                                }).
                                Select(c => new {
                                    Year = c.Key.Year
                                }).
                                OrderByDescending(e => e.Year);
            return new OkObjectResult(years);
        }

        [HttpGet("months/{year}/{userId?}")]
        [Authorize]
        public IActionResult GetMonths(int year, int userId=0){
            if(userId == 0){
                var user = this.CurrentUser();
                userId = user.Id;
            }
            var lastExpenses = context.Activity.
                                Where(e=>e.KersUser.Id == userId && e.ActivityDate.Year == year).
                                GroupBy(e => new {
                                    Month = e.ActivityDate.Month
                                }).
                                Select(c => new {
                                    Month = c.Key.Month
                                }).
                                OrderByDescending(e => e.Month);
            return new OkObjectResult(lastExpenses);
        }



        [HttpGet("perDay/{userid}/{start}/{end}")]
        [Authorize]
        public IActionResult PerDay(int userid, DateTime start, DateTime end ){
            
            end = end.AddDays(1);
            var filtered = context.Activity.
                                Where(a=>a.KersUser.Id == userid & a.ActivityDate > start & a.ActivityDate < end).ToList();
            var numPerDay = filtered.
                                GroupBy(e => new {
                                    Date = e.ActivityDate.DayOfYear
                                }).
                                Select(c => new {
                                    Day = c.FirstOrDefault().ActivityDate.ToString("yyyy-MM-dd"),
                                    Count = c.Count()
                                }).
                                OrderByDescending(e => e.Day);
            return new OkObjectResult(numPerDay);
        }



        [HttpGet("summaryPerMonth/{userid?}")]
        [Authorize]
        public IActionResult summaryPerMonth(int userid = 0 ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            var numPerMonth = context.Activity.
                                Where(a=>a.KersUser.Id == userid).
                                GroupBy(e => new {
                                    Month = e.ActivityDate.Month,
                                    Year = e.ActivityDate.Year
                                }).
                                Select(c => new {
                                    Ids = c.Select(
                                        s => s.Id
                                    ),
                                    Hours = c.Sum(s => s.Hours),
                                    Audience = c.Sum(s => s.Audience),
                                    Month = c.Key.Month,
                                    Year = c.Key.Year
                                }).
                                OrderByDescending(e => e.Year).ThenByDescending(e => e.Month).ToList();

            var result = new List<PerMonthActivities>();

            foreach(var mnth in numPerMonth){
                var MntRevs = new List<ActivityRevision>();
                foreach(var rev in mnth.Ids){
                    
                    /* 
                    var lstrvsn = context.ActivityRevision.
                            Where(r => r.ActivityId == rev).
                            Include(a => a.ActivityOptionNumbers).
                            Include(a => a.ActivityOptionSelections).
                            Include(a => a.RaceEthnicityValues).
                            OrderBy(a => a.Created).Last();
 */

                    var revCacheKey = CacheKeys.ActivityLastRevision + rev.ToString();
                    

                    var revCacheString = _distributedCache.GetString(revCacheKey);

                    ActivityRevision lstrvsn;
                    if (!string.IsNullOrEmpty(revCacheString)){
                        lstrvsn = JsonConvert.DeserializeObject<ActivityRevision>(revCacheString);
                    }else{
                        lstrvsn = context.ActivityRevision.
                            AsNoTracking().
                            Where(r => r.ActivityId == rev).
                            Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                            Include(a => a.ActivityOptionSelections).ThenInclude( s => s.ActivityOption).
                            Include(a => a.RaceEthnicityValues).
                            Include( a => a.ActivityImages).
                            OrderBy(a => a.Created).Last();
                            lstrvsn.ActivityImages = context.Activity.Where( a => a.Id == rev).Include( a => a.ActivityImages).First().ActivityImages;
                            //Store revision only if it is recent
                            if( (DateTime.Now - lstrvsn.ActivityDate).Days < 60 ){
                                var revSerialized = JsonConvert.SerializeObject(lstrvsn);
                                _distributedCache.SetString(revCacheKey, revSerialized, new DistributedCacheEntryOptions
                                    {
                                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                                    });
                            }
                    }




                    MntRevs.Add(lstrvsn);
                }


                var actvts = new PerMonthActivities();
                actvts.Revisions = MntRevs;
                actvts.Hours = mnth.Hours;
                actvts.Audience = mnth.Audience;
                actvts.Month = mnth.Month;
                actvts.Year = mnth.Year;
                result.Add(actvts);
                
            }


            return new OkObjectResult(result);
        }



        [HttpGet("allContactsSummaryPerMonth/{userid?}/{fy?}")]
        [Authorize]
         public IActionResult allContactsSummaryPerMonth(int userid = 0, string fy = "0" ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            /****************************************/
            //    ACTIVITIES
            /****************************************/
            var numPerMonth = context.Activity.
                                AsNoTracking().
                                Where(a=> a.KersUser.Id == userid
                                            &&
                                            a.ActivityDate > fiscalYear.Start
                                            &&
                                            a.ActivityDate < fiscalYear.End
                                ).ToList();
            var groupedPetMonth = numPerMonth.GroupBy(e => new {
                                    Month = e.ActivityDate.Month,
                                    Year = e.ActivityDate.Year
                                }).
                                Select(c => new {
                                    Ids = c.Select(
                                        s => s.Id
                                    ),
                                    Month = c.Key.Month,
                                    Year = c.Key.Year
                                }).
                                OrderByDescending(e => e.Year).ThenByDescending(e => e.Month).ToList();
            var result = new List<PerMonthContacts>();
            foreach(var mnth in groupedPetMonth){
                var contactsThisMonth = new PerMonthContacts();
                contactsThisMonth.Month = mnth.Month;
                contactsThisMonth.Year = mnth.Year;
                contactsThisMonth.RaceEthnicityValues = new List<IRaceEthnicityValue>();
                contactsThisMonth.OptionNumberValues = new List<IOptionNumberValue>();
                contactsThisMonth.Hours = 0;
                contactsThisMonth.Multistate = 0;
                contactsThisMonth.Males = 0;
                contactsThisMonth.Females = 0;
                foreach(var rev in mnth.Ids){
                    var revCacheKey = CacheKeys.ActivityLastRevision + rev.ToString();
                    var revCacheString = _distributedCache.GetString(revCacheKey);
                    ActivityRevision lstrvsn;
                    if (!string.IsNullOrEmpty(revCacheString)){
                        lstrvsn = JsonConvert.DeserializeObject<ActivityRevision>(revCacheString);
                    }else{
                        lstrvsn = context.ActivityRevision.
                            AsNoTracking().
                            Where(r => r.ActivityId == rev).
                            Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                            Include(a => a.ActivityOptionSelections).ThenInclude( s => s.ActivityOption).
                            Include(a => a.RaceEthnicityValues).
                            AsSplitQuery().
                            OrderBy(a => a.Created).Last();
                            //Store revision only if it is recent
                            if( (DateTime.Now - lstrvsn.ActivityDate).Days < 60 ){
                                var revSerialized = JsonConvert.SerializeObject(lstrvsn);
                                _distributedCache.SetString(revCacheKey, revSerialized, new DistributedCacheEntryOptions
                                    {
                                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                                    });
                            }
                    }
                    contactsThisMonth.Males += lstrvsn.Male;
                    contactsThisMonth.Females += lstrvsn.Female;
                    contactsThisMonth.Hours += lstrvsn.Hours;
                    contactsThisMonth.OptionNumberValues.AddRange(lstrvsn.ActivityOptionNumbers);
                    contactsThisMonth.RaceEthnicityValues.AddRange(lstrvsn.RaceEthnicityValues);
                    if( lstrvsn.ActivityOptionSelections.Where( n => n.ActivityOption.Name == "Multistate effort?").Any()){
                        contactsThisMonth.Multistate += lstrvsn.Hours;
                    }
                } 
                result.Add(contactsThisMonth); 
            }
            /****************************************/
            //    CONTACTS
            /****************************************/
            var contactsPerMonth = context.Contact.
                                AsNoTracking().
                                Where(a=> a.KersUser.Id == userid
                                            &&
                                            a.ContactDate > fiscalYear.Start
                                            &&
                                            a.ContactDate < fiscalYear.End
                                ).ToList();
            var groupedContactsPerMonth =  contactsPerMonth.GroupBy(e => new {
                                    Month = e.ContactDate.Month,
                                    Year = e.ContactDate.Year
                                }).
                                Select(c => new {
                                    Ids = c.Select(
                                        s => s.Id
                                    ),
                                    Month = c.Key.Month,
                                    Year = c.Key.Year
                                }).
                                OrderByDescending(e => e.Year).ThenByDescending(e => e.Month).ToList();
            foreach(var mnth in groupedContactsPerMonth){
                var contactsThisMonth = new PerMonthContacts();
                contactsThisMonth.Month = mnth.Month;
                contactsThisMonth.Year = mnth.Year;
                contactsThisMonth.RaceEthnicityValues = new List<IRaceEthnicityValue>();
                contactsThisMonth.OptionNumberValues = new List<IOptionNumberValue>();
                contactsThisMonth.Hours = 0;
                contactsThisMonth.Multistate = 0;
                contactsThisMonth.Males = 0;
                contactsThisMonth.Females = 0;
                foreach(var rev in mnth.Ids){
                    var revCacheKey = CacheKeys.ContactLastRevision + rev.ToString();
                    var revCacheString = _distributedCache.GetString(revCacheKey);
                    ContactRevision lstrvsn;
                    if (!string.IsNullOrEmpty(revCacheString)){
                        lstrvsn = JsonConvert.DeserializeObject<ContactRevision>(revCacheString);
                    }else{
                        lstrvsn = context.ContactRevision.
                            AsNoTracking().
                            Where(r => r.ContactId == rev).
                            Include(a => a.ContactOptionNumbers).
                            Include(a => a.ContactRaceEthnicityValues).
                            AsSplitQuery().
                            OrderBy(a => a.Created).Last();;
                            var revSerialized = JsonConvert.SerializeObject(lstrvsn);
                            _distributedCache.SetString(revCacheKey, revSerialized, new DistributedCacheEntryOptions
                                {
                                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                                });
                    }
                    contactsThisMonth.Males += lstrvsn.Male;
                    contactsThisMonth.Females += lstrvsn.Female;
                    contactsThisMonth.Hours += lstrvsn.Days * 8;
                    contactsThisMonth.OptionNumberValues.AddRange(lstrvsn.ContactOptionNumbers);
                    contactsThisMonth.RaceEthnicityValues.AddRange(lstrvsn.ContactRaceEthnicityValues);
                    contactsThisMonth.Multistate += lstrvsn.Multistate * 8;
                }
                var thisMonth = result.Where( r => r.Month == mnth.Month && r.Year == mnth.Year ).FirstOrDefault();
                if( thisMonth == null){
                    result.Add(contactsThisMonth);
                }else{
                    thisMonth.Females += contactsThisMonth.Females;
                    thisMonth.Males += contactsThisMonth.Males;
                    thisMonth.Hours += contactsThisMonth.Hours;
                    thisMonth.Multistate += contactsThisMonth.Multistate;
                    thisMonth.RaceEthnicityValues.AddRange(contactsThisMonth.RaceEthnicityValues);
                    thisMonth.OptionNumberValues.AddRange(contactsThisMonth.OptionNumberValues);
                }
            }

            result = result.OrderByDescending( r => r.Year).ThenByDescending( r => r.Month).ToList();
            return new OkObjectResult(result);
        }



        [HttpGet("summaryPerProgram/{userid?}/{fy?}")]
        [Authorize]
        public IActionResult summaryPerProgram(int userid = 0, string fy = "0", bool refreshCache = false ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            FiscalYear fiscalYear;
            if(fy != "0"){
                fiscalYear = fiscalYearRepo.byName(fy, FiscalYearType.ServiceLog);
            }else{
                fiscalYear = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
            }
            
            List<PerProgramActivities> result;



            var cacheKey = CacheKeys.ActivitiesPerFyPerUserPerMajorProgram + fiscalYear.Name + userid.ToString();
            var cacheString = _distributedCache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = JsonConvert.DeserializeObject<List<PerProgramActivities>>(cacheString);
            }else{
                var filteredActivities = context.Activity.
                                    AsNoTracking().
                                    Where( a => a.KersUser.Id == userid 
                                                &&
                                                a.ActivityDate >= fiscalYear.Start
                                                &&
                                                a.ActivityDate <= fiscalYear.End
                                            ).ToList();
                var numPerMonth = filteredActivities.
                                    GroupBy(e => new {
                                        MajorProgram = e.MajorProgram
                                    }).
                                    Select(c => new {
                                        Ids = c.Select(
                                            s => s.Id
                                        ),
                                        Hours = c.Sum(s => s.Hours),
                                        Audience = c.Sum(s => s.Audience),
                                        MajorProgram = c.Key.MajorProgram
                                    })
                                    .OrderBy( s => s.MajorProgram.Name);
                result = new List<PerProgramActivities>();

                foreach(var mnth in numPerMonth){
                    var ids = new List<int>();
                    var MntRevs = new List<ActivityRevision>();
                    foreach(var rev in mnth.Ids){
                        /* var lstrvsn = context.ActivityRevision.
                                AsNoTracking().
                                Where(r => r.ActivityId == rev).
                                Include(a => a.ActivityOptionNumbers).
                                Include(a => a.ActivityOptionSelections).
                                Include(a => a.RaceEthnicityValues).
                                OrderBy(a => a.Created).Last(); */

                        var revCacheKey = CacheKeys.ActivityLastRevision + rev.ToString();
                    

                        var revCacheString = _distributedCache.GetString(revCacheKey);

                        ActivityRevision lstrvsn;
                        if (!string.IsNullOrEmpty(revCacheString)){
                            lstrvsn = JsonConvert.DeserializeObject<ActivityRevision>(revCacheString);
                        }else{
                            lstrvsn = context.ActivityRevision.
                                AsNoTracking().
                                Where(r => r.ActivityId == rev).
                                Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                                Include(a => a.ActivityOptionSelections).ThenInclude( s => s.ActivityOption).
                                Include(a => a.RaceEthnicityValues).
                                OrderBy(a => a.Created).Last();
                                //Store revision only if it is recent
                            if( (DateTime.Now - lstrvsn.ActivityDate).Days < 60 ){
                                var revSerialized = JsonConvert.SerializeObject(lstrvsn);
                                _distributedCache.SetString(revCacheKey, revSerialized, new DistributedCacheEntryOptions
                                    {
                                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                                    });
                            }
                        }



                        MntRevs.Add(lstrvsn);
                    }


                    var actvts = new PerProgramActivities();
                    actvts.Revisions = MntRevs;
                    actvts.Hours = mnth.Hours;
                    actvts.Audience = mnth.Audience;
                    actvts.MajorProgram = mnth.MajorProgram;
                    result.Add(actvts);
                    
                }

                var serialized = JsonConvert.SerializeObject(result);

                var expireIn = 3;

                if( fiscalYear.ExtendedTo < DateTime.Now ){
                    expireIn = 365;
                }


                _distributedCache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(expireIn)
                    });
            
            
            
            }

            return new OkObjectResult(result);
        }



        [HttpGet("allContactsSummaryPerProgram/{userid?}/{fy?}")]
        [Authorize]
        public IActionResult allContactsSummaryPerProgram(int userid = 0, string fy = "0", bool refreshCache = false ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            List<PerProgramContacts> result = new List<PerProgramContacts>();
            
            /* var cacheKey = CacheKeys.ActivitiesPerFyPerUserPerMajorProgram + fiscalYear.Name + userid.ToString();
            var cacheString = _distributedCache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = JsonConvert.DeserializeObject<List<PerProgramContacts>>(cacheString);
            }else{ */
                /****************************************/
                //    ACTIVITIES
                /****************************************/
                var numPerMonth = context.Activity.
                                    AsNoTracking().
                                    Where( a => a.KersUser.Id == userid 
                                                &&
                                                a.ActivityDate >= fiscalYear.Start
                                                &&
                                                a.ActivityDate <= fiscalYear.End
                                            )
                                            .ToList();
                var groupedPerMonth = numPerMonth.GroupBy(e => new {
                                        MajorProgram = e.MajorProgramId
                                    }).
                                    Select(c => new {
                                        Ids = c.Select(
                                            s => s.Id
                                        ),
                                        MajorProgram = c.Key
                                    });
                result = new List<PerProgramContacts>();

                foreach(var mnth in groupedPerMonth){
                    var perProgramContacts = new PerProgramContacts();
                    perProgramContacts.MajorProgram = context.MajorProgram.Find(mnth.MajorProgram.MajorProgram);
                    perProgramContacts.Males = 0;
                    perProgramContacts.Females = 0;
                    perProgramContacts.Multistate = 0;
                    perProgramContacts.Hours = 0;
                    perProgramContacts.RaceEthnicityValues = new List<IRaceEthnicityValue>();
                    perProgramContacts.OptionNumberValues = new List<IOptionNumberValue>();
                    foreach(var rev in mnth.Ids){
                        var revCacheKey = CacheKeys.ActivityLastRevision + rev.ToString();
                    
                        var revCacheString = _distributedCache.GetString(revCacheKey);

                        ActivityRevision lstrvsn;
                        if (!string.IsNullOrEmpty(revCacheString)){
                            lstrvsn = JsonConvert.DeserializeObject<ActivityRevision>(revCacheString);
                        }else{
                            lstrvsn = context.ActivityRevision.
                                AsNoTracking().
                                Where(r => r.ActivityId == rev).
                                Include(a => a.ActivityOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                                Include(a => a.ActivityOptionSelections).ThenInclude( s => s.ActivityOption).
                                Include(a => a.RaceEthnicityValues).
                                OrderBy(a => a.Created).Last();
                                //Store revision only if it is recent
                            if( (DateTime.Now - lstrvsn.ActivityDate).Days < 60 ){
                                var revSerialized = JsonConvert.SerializeObject(lstrvsn);
                                _distributedCache.SetString(revCacheKey, revSerialized, new DistributedCacheEntryOptions
                                    {
                                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                                    });
                            }
                        }
                        perProgramContacts.Males += lstrvsn.Male;
                        perProgramContacts.Females += lstrvsn.Female;
                        if( lstrvsn.ActivityOptionSelections.Where( n => n.ActivityOption.Name == "Multistate effort?").Any()){
                            perProgramContacts.Multistate += lstrvsn.Hours;
                        }
                        perProgramContacts.Hours += lstrvsn.Hours;
                        perProgramContacts.RaceEthnicityValues.AddRange(lstrvsn.RaceEthnicityValues);
                        perProgramContacts.OptionNumberValues.AddRange(lstrvsn.ActivityOptionNumbers);
                    }
                    result.Add(perProgramContacts);
                }


                /****************************************/
                //    CONTACTS
                /****************************************/

                var contactsNumPerMonth = context.Contact.
                                    AsNoTracking().
                                    Where( a => a.KersUser.Id == userid 
                                                &&
                                                a.ContactDate >= fiscalYear.Start
                                                &&
                                                a.ContactDate <= fiscalYear.End
                                            )
                                    .ToList();
                groupedPerMonth = contactsNumPerMonth.GroupBy(e => new {
                                        MajorProgram = e.MajorProgramId
                                    }).
                                    Select(c => new {
                                        Ids = c.Select(
                                            s => s.Id
                                        ),
                                        MajorProgram = c.Key
                                    });
                //result = new List<PerProgramContacts>();

                foreach(var mnth in groupedPerMonth){
                    var perProgramContacts = new PerProgramContacts();
                    perProgramContacts.MajorProgram = context.MajorProgram.Find(mnth.MajorProgram.MajorProgram);
                    perProgramContacts.Males = 0;
                    perProgramContacts.Females = 0;
                    perProgramContacts.Multistate = 0;
                    perProgramContacts.Hours = 0;
                    perProgramContacts.RaceEthnicityValues = new List<IRaceEthnicityValue>();
                    perProgramContacts.OptionNumberValues = new List<IOptionNumberValue>();
                    foreach(var rev in mnth.Ids){
                        var revCacheKey = CacheKeys.ContactLastRevision + rev.ToString();
                    
                        var revCacheString = _distributedCache.GetString(revCacheKey);

                        ContactRevision lstrvsn;
                        if (!string.IsNullOrEmpty(revCacheString)){
                            lstrvsn = JsonConvert.DeserializeObject<ContactRevision>(revCacheString);
                        }else{
                            lstrvsn = context.ContactRevision.
                                AsNoTracking().
                                Where(r => r.ContactId == rev).
                                Include(a => a.ContactOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                                Include(a => a.ContactRaceEthnicityValues).
                                OrderBy(a => a.Created).Last();
                                var revSerialized = JsonConvert.SerializeObject(lstrvsn);
                                _distributedCache.SetString(revCacheKey, revSerialized, new DistributedCacheEntryOptions
                                    {
                                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                                    });
                        }
                        perProgramContacts.Males += lstrvsn.Male;
                        perProgramContacts.Females += lstrvsn.Female;
                        perProgramContacts.Multistate += lstrvsn.Multistate * 8;
                        perProgramContacts.Hours += lstrvsn.Days * 8;
                        perProgramContacts.RaceEthnicityValues.AddRange(lstrvsn.ContactRaceEthnicityValues);
                        perProgramContacts.OptionNumberValues.AddRange(lstrvsn.ContactOptionNumbers);
                    }


                    var thisProgram = result.Where( r => r.MajorProgram.Id == mnth.MajorProgram.MajorProgram ).FirstOrDefault();
                    if( thisProgram == null){
                        result.Add(perProgramContacts);
                    }else{
                        thisProgram.Females += perProgramContacts.Females;
                        thisProgram.Males += perProgramContacts.Males;
                        thisProgram.Hours += perProgramContacts.Hours;
                        thisProgram.Multistate += perProgramContacts.Multistate;
                        thisProgram.RaceEthnicityValues.AddRange(perProgramContacts.RaceEthnicityValues);
                        thisProgram.OptionNumberValues.AddRange(perProgramContacts.OptionNumberValues);
                    }
                }
       /*          var serialized = JsonConvert.SerializeObject(result);

                var expireIn = 3;

                if( fiscalYear.ExtendedTo < DateTime.Now ){
                    expireIn = 365;
                }


                _distributedCache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(expireIn)
                    });
            
            
            
            }
 */
            if(result.Count() > 0) result = result.OrderBy( r => r.MajorProgram.Name).ToList();
            return new OkObjectResult(result);
        }




        [HttpGet("perPeriod/{start}/{end}/{userid?}")]
        [Authorize]
        public IActionResult PerPeriod(DateTime start, DateTime end, int userid = 0 ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            end = end.AddDays(1);
            var lastActivities = context.Activity
                                .Where(a=>a.KersUser.Id == userid & a.ActivityDate > start & a.ActivityDate < end)
                                .Select( a => a.LastRevisionId).ToList();
            var revs = new List<ActivityRevision>();
            foreach( var RevId in lastActivities){
                var rev = context.ActivityRevision.Where( r => r.Id == RevId).
                                Include(r => r.MajorProgram).
                                Include(r => r.ActivityOptionSelections).ThenInclude(s => s.ActivityOption).
                                Include(r => r.ActivityOptionNumbers).ThenInclude(s => s.ActivityOptionNumber).
                                Include(r => r.RaceEthnicityValues).
                                AsSplitQuery()
                                .FirstOrDefault();
                if( rev != null ) revs.Add(rev);

            }
            
                                
                                
                                
             /*                   
                                .
                                OrderByDescending(a => a.ActivityDate);
            var revs = lastActivities.Select(a => a.LastRevision);
             if( lastActivities != null){
                foreach(var activity in lastActivities){
                    if(activity.Revisions.Count != 0){
                        revs.Add( activity.Revisions.OrderBy(r=>r.Created).Last() );
                    }
                }
            } */

            return new OkObjectResult(revs);
        }

        [HttpGet("perPeriodLite/{start}/{end}/{userid?}")]
        [Authorize]
        public IActionResult PerPeriodLight(DateTime start, DateTime end, int userid = 0 ){
            if(userid == 0){
                userid = this.CurrentUser().Id;
            }
            end = end.AddDays(1);
            var lastActivities = context.Activity.
                                Where(a=>a.KersUser.Id == userid & a.ActivityDate > start & a.ActivityDate < end);
            
            return new OkObjectResult(lastActivities);
        }



        [HttpGet("latestbyuser/{userid}/{amount?}")]
        [Authorize]
        public IActionResult LatestByUser(int userid, int amount = 10){
            
            var lastActivities = context.Activity.
                                Where(e=>e.KersUser.Id == userid).
                                Include(e=>e.LastRevision).ThenInclude(r => r.MajorProgram).
                                Include(e=>e.LastRevision).ThenInclude(r => r.ActivityOptionSelections).ThenInclude(s => s.ActivityOption).
                                Include(e=>e.LastRevision).ThenInclude(r => r.ActivityOptionNumbers).ThenInclude(s => s.ActivityOptionNumber).
                                Include(e=>e.LastRevision).ThenInclude(r => r.RaceEthnicityValues).
                                AsSplitQuery().
                                OrderByDescending(e=>e.ActivityDate).
                                Take(amount);
            var revs = lastActivities.Select( a => a.LastRevision);
           /*  if( lastActivities != null){
                foreach(var activity in lastActivities){
                    if(activity.Revisions.Count != 0){
                        revs.Add( activity.Revisions.OrderBy(r=>r.Created).Last() );
                    }
                }
            } */

            return new OkObjectResult(revs);
        }


        [HttpGet("permonth/{year}/{month}/{userId?}/{orderBy?}/{isSnap?}")]
        [Authorize]
        public IActionResult PerMonth(int year, int month, int userId = 0, string orderBy = "desc", bool isSnap = false){
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
                userId = user.Id;
            }else{
                user = this.context.KersUser.Find(userId);
            }
            var activities = activityRepo.PerMonth(user, year, month, orderBy);
            if(isSnap){
                activities = activities.Where(a => a.isSnap).ToList();
            }
            return new OkObjectResult(activities);
        }


        [HttpGet]
        [Route("{year}/{month}/{userId}/data.csv")]
        //[Produces("text/csv")]
        [Authorize]
        public async Task<IActionResult> GetDataAsCsv(int year, int month, int userId){
/* 
            KersUser user;
            if(userId == 0){
                user = this.CurrentUser();
                userId = user.Id;
            }else{
                user = this.context.KersUser.Find(userId);
            } */


            var criteria = new ActivitySearchCriteria();
            criteria.Start = new DateTime(year,month, 1, 0,0,0);
            criteria.End = new DateTime( year, month, DateTime.DaysInMonth(year, month),23,59,59);
            criteria.Search = "";
            criteria.Skip = 0;
            criteria.Take = 10000;

            var ret = new List<List<string>>();
            ret.Add(activityRepo.ReportHeaderRow());
            var result = await SeearchResults(criteria, userId);
            var races = context.Race.OrderBy( r => r.Order).ToList();
            var ethnicities = context.Ethnicity.OrderBy( r => r.Order).ToList();
            var options = context.ActivityOption.OrderBy( o => o.Order).ToList();
            var optionNumbers = context.ActivityOptionNumber.OrderBy( v => v.Order).ToList();
            var ages = context.SnapDirectAges.Where( a => a.Active).OrderBy( a => a.order).ToList();
            var audience = context.SnapDirectAudience.Where( a => a.Active).OrderBy( a => a.order).ToList();
            foreach( var res in result.Results){
                ret.Add( activityRepo.ReportRow(
                                            res.Revision.ActivityId,
                                            null,
                                            races,
                                            ethnicities,
                                            options,
                                            optionNumbers,
                                            ages,
                                            audience
                                        )      
                        );
            }

            return new OkObjectResult(ret);


          /*   var lastActivities = context.Activity.
                                Where(e=>e.KersUser == user && e.ActivityDate.Month == month && e.ActivityDate.Year == year).
                                Select(a =>
                                    new {
                                        Date = a.ActivityDate.ToString("MM/dd/yyyy"),
                                        Title = a.Title,
                                        Hours = a.Hours,
                                        Audience = a.Audience,
                                        MajorProgram = a.MajorProgram.Name
                                    }
                                
                                ).ToList();
                             
            var cc = new CsvConfiguration(new System.Globalization.CultureInfo("en-US"));
            using (var ms = new MemoryStream()) {
                using (var sw = new StreamWriter(stream: ms, encoding: new UTF8Encoding(true))) {
                    using (var cw = new CsvWriter(sw, cc)) {
                        cw.WriteRecords(ret);
                    }// The stream gets flushed here.
                    return File(ms.ToArray(), "text/csv", $"export_{DateTime.UtcNow.Ticks}.csv");
                }
            } */
            //return File(lastActivities, "text/csv", $"export_{DateTime.UtcNow.Ticks}.csv");
            //return Ok( lastActivities );
        }


        [HttpPost()]
        [Authorize]
        public IActionResult AddActivity( [FromBody] ActivityRevision activity){
            if(activity != null){

                
                var user = this.CurrentUser();
                var act = new Activity();
                act.KersUser = user;
                act.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                act.Created = DateTime.Now;
                act.Updated = DateTime.Now;
                act.ActivityDate = activity.ActivityDate;
                activity.Created = DateTime.Now;
                act.Title = activity.Title;
                act.Hours = activity.Hours;
                act.Audience = activity.Male + activity.Female;
                act.MajorProgramId = activity.MajorProgramId;
                act.Revisions = new List<ActivityRevision>();
                act.Revisions.Add(activity);
                context.Add(act); 
                this.Log(activity,"ActivityRevision", "Activity Added.");
                context.SaveChanges();
                return new OkObjectResult(activity);
            }else{
                this.Log( activity ,"ActivityRevision", "Error in adding Activity attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateActivity( int id, [FromBody] ActivityRevision activity){
           
            
            var entity = context.ActivityRevision.Find(id);
            var acEntity = context.Activity.Find(entity.ActivityId);

            if(activity != null && acEntity != null){
                activity.Created = DateTime.Now;
                acEntity.ActivityDate = activity.ActivityDate;

                acEntity.Title = activity.Title;
                acEntity.Hours = activity.Hours;
                acEntity.Audience = activity.Male + activity.Female;
                acEntity.MajorProgramId = activity.MajorProgramId;

                acEntity.Revisions.Add(activity);
                context.SaveChanges();
                this.Log(entity,"ActivityRevision", "Activity Updated.");
                return new OkObjectResult(activity);
            }else{
                this.Log( activity ,"ActivityRevision", "Not Found Activity in an update attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteActivity( int id ){
            var entity = context.ActivityRevision.Find(id);
            var acEntity = context.Activity.Where(a => a.Id == entity.ActivityId).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionSelections).
                                Include(e=>e.Revisions).ThenInclude(r => r.ActivityOptionNumbers).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).
                                Include(e=>e.Revisions).ThenInclude(r => r.RaceEthnicityValues).
                                FirstOrDefault();
            
            if(acEntity != null){
                
                context.Activity.Remove(acEntity);
                context.SaveChanges();
                
                this.Log(entity,"ActivityRevision", "Activity Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ActivityRevision", "Not Found Activity in a delete attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpGet("options")]
        public IActionResult Options(){
            var cacheKey = "ServiceLogOptions";
            List<ActivityOption> ops;

            if (!_cache.TryGetValue(cacheKey, out ops))
            {
                // Key not in cache, so get data.
                ops = this.context.ActivityOption.Where(o => o.Active ).OrderBy(o => o.Order).ToList();
            
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(cacheKey, ops, cacheEntryOptions);
            }

            
            return new OkObjectResult(ops);
        }

        [HttpGet("optionnumbers")]
        public IActionResult OptionNumbers(){

            var cacheKey = "ServiceLogOptionNumbers";
            List<ActivityOptionNumber> ops;
            
            if (!_cache.TryGetValue(cacheKey, out ops))
            {
                // Key not in cache, so get data.
                ops = this.context.ActivityOptionNumber.OrderBy(o => o.Order).ToList();
            
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(cacheKey, ops, cacheEntryOptions);
            }
            
            return new OkObjectResult(ops);
        }

        [HttpGet("races")]
        public IActionResult Races(){

            var cacheKey = "races";
            List<Race> rcs;
            


            if (!_cache.TryGetValue(cacheKey, out rcs))
            {
                // Key not in cache, so get data.
                rcs = this.context.Race.OrderBy(o => o.Order).ToList();
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(cacheKey, rcs, cacheEntryOptions);
            }
            

            return new OkObjectResult(rcs);
        }

        [HttpGet("ethnicities")]
        public IActionResult Ethnicities(){

            var cacheKey = "ethnicities";
            List<Ethnicity> rcs;
            
            if (!_cache.TryGetValue(cacheKey, out rcs))
            {
                // Key not in cache, so get data.
                rcs = this.context.Ethnicity.OrderBy(o => o.Order).ToList();
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(cacheKey, rcs, cacheEntryOptions);
            }

            return new OkObjectResult(rcs);
        }



        /*******************************************/
        // Stats Per Month
        /*******************************************/



        [HttpGet("numberActivitiesPerYear/{FiscalYearId}/{UserId}")]
        public IActionResult NumberActivitiesPerYear(int FiscalYearId, int UserId){
            if( UserId == 0 ) UserId = this.CurrentUser().Id;

            var NumActivities = context.Activity.Where( a => a.KersUserId == UserId && a.MajorProgram.StrategicInitiative.FiscalYear.Id == FiscalYearId).Count();

            return new OkObjectResult(NumActivities);
        }


        [HttpGet("GetActivitiesBatch/{start}/{amount}/{FiscalYearId}/{UserId}")]
        public IActionResult GetActivitiesBatch(int start, int amount, int FiscalYearId, int UserId){
            if( UserId == 0 ) UserId = this.CurrentUser().Id;
            var Activities = context.Activity.Where( a => a.KersUserId == UserId && a.MajorProgram.StrategicInitiative.FiscalYear.Id == FiscalYearId).OrderBy( a => a.Id).Skip(start).Take(amount);
            List<ActivityRevision> revs = new List<ActivityRevision>();
            foreach( var act in Activities){
                var rev = context.ActivityRevision.Where( a => a.Id == act.LastRevisionId)
                            .Include( a => a.RaceEthnicityValues)
                            .Include( a => a.ActivityOptionNumbers)
                            .Include( a => a.ActivityOptionSelections).ThenInclude( s => s.ActivityOption)
                            .FirstOrDefault();
                revs.Add(rev);
            }
            return new OkObjectResult(revs);
        }
















        private void Log(   object obj, 
                            string objectType = "ActivityRevision",
                            string description = "Submitted Activity Revision", 
                            string type = "Activity",
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
                //user = userRepo.findByProfileID(profile.Id);


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

        public FiscalYear GetFYByName(string fy, string type = "serviceLog"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalYearRepo.currentFiscalYear(type);
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == type).FirstOrDefault();
            }
            return fiscalYear;
        }



    }



    class PerMonthActivities{
        public List<ActivityRevision> Revisions;
        public float Hours;
        public int Audience;
        public int Month;
        public int Year; 
    }


    class PerMonthContacts{
        public List<IRaceEthnicityValue> RaceEthnicityValues {get;set;}
        public List<IOptionNumberValue> OptionNumberValues {get; set;}
        public float Hours;
        public float Multistate;
        public int Males;
        public int Females;
        public int Month;
        public int Year; 
    }


    class PerProgramActivities{
        public List<ActivityRevision> Revisions;
        public float Hours;
        public int Audience;
        public MajorProgram MajorProgram;
    }

    class PerProgramContacts{
        public List<IRaceEthnicityValue> RaceEthnicityValues {get;set;}
        public List<IOptionNumberValue> OptionNumberValues {get; set;}
        public float Hours;
        public float Multistate;
        public int Males;
        public int Females;
        public MajorProgram MajorProgram; 
    }


}