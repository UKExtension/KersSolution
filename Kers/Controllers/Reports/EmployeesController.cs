using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Contexts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Kers.Models.ViewModels;
using Kers.Models.Data;
using System;
using System.IO;
using CsvHelper;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class EmployeesController : Controller
    {
        KERScoreContext context;
        KERSreportingContext reportingContext;
        ITrainingRepository trainingRepo;
        IFiscalYearRepository fiscalYearRepository;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        IExpenseRepository expensesRepo;
        private FiscalYear currentFiscalYear;

        private IConfiguration _configuration;
        // Faculty FY2020
        string[] faculty = new string[] {
            "enad222",
            "lmbi222",
            "lmca222",
            "nrdo232",
            "ktgraves",
            "lakenn4",
            "bsp245",
            "oaad223",
            "landerso",
            "bar244",
            "abailey",
            "rbessin",
            "cbr269",
            "ndbrea0",
            "dbullock",
            "kburdine",
            "fccama2",
            "rcoleman",
            "evcr224",
            "adreum2",
            "rdurham",
            "acel229",
            "chfinn0",
            "gega226",
            "nwa232",
            "gshali2",
            "nlhu222",
            "jhunter",
            "sisaacs",
            "krjone3",
            "dwka224",
            "camurp2",
            "jko234",
            "afhosi2",
            "jlla226",
            "bdle222",
            "cdlee2",
            "trle233",
            "jwle222",
            "clu247",
            "jtmcma2",
            "jjmu233",
            "mewarr2",
            "hlno222",
            "rpearce",
            "ppvi223",
            "gkrent2",
            "elritc2",
            "reru226",
            "jmshoc2",
            "srsmitd",
            "wsnell",
            "msp238",
            "tss",
            "cdteut0",
            "hssi228",
            "krva228",
            "rtvi223",
            "kwi283",
            "woodch",
            "tawoods",
            "qyi226",
            "jzimm",
            "hzan222",
            "hbe257",
            "emde224",
            "tmwilk2",
            "crhe249",
            "jaberr0",
            "kkr234",
            "sly240",
            "rme247",
            "arabad2",
            "jph235",
            "mlturn0",
            "sfcrad2",
            "almeye2",
            "jksc222",
            "acse223",
            "jlwahr2",
            "daaron",
            "aaadam3",
            "aad244",
            "saad232",
            "mba444",
            "barton",
            "tjba240",
            "arbe243",
            "dbr239",
            "pbrid2",
            "ljbr240",
            "scbu225",
            "rca253",
            "elmars2",
            "krcorb3",
            "jjcox2",
            "crofche",
            "sdebo2",
            "zde234",
            "sdobson",
            "adownie",
            "jdu282",
            "jdv223",
            "pdyk",
            "hetliz",
            "hel226",
            "dely",
            "rep222",
            "hfa243",
            "farman",
            "wiford2",
            "cfox",
            "lsgo226",
            "djgo227",
            "sggo228",
            "jgrove",
            "agu227",
            "nlha239",
            "bhain2",
            "kmha388",
            "jhans2",
            "erha235",
            "raharg0",
            "dharmon",
            "rwharr00",
            "dhild",
            "dkhowe2",
            "aghunt00",
            "klja223",
            "wdjo227",
            "bmjo263",
            "apkach2",
            "pk62",
            "tska223",
            "yka251",
            "tka234",
            "tcke230",
            "ceke263",
            "hkim3",
            "knyo224",
            "llawrenc",
            "mlee6",
            "jmlhot2",
            "fli230",
            "lmu246",
            "mdlind1",
            "ylu232",
            "jnmacl2",
            "sjma278",
            "tbmark0",
            "cjmato2",
            "rlmccu2",
            "kmcleod",
            "dhmcne2",
            "djmc261",
            "tlgrah2",
            "tlmi250",
            "kmi320",
            "lmo225",
            "montross",
            "mnewman",
            "pdnagy2",
            "tooc222",
            "bol232",
            "edos225",
            "abpa227",
            "aepa225",
            "rpalli",
            "japa291",
            "mpe277",
            "tphillip",
            "jpl225",
            "hjpo223",
            "sjpr223",
            "jsra234",
            "sre290",
            "lrieske",
            "krign2",
            "ccri226",
            "cmro267",
            "dbro223",
            "mgross2",
            "shsagh2",
            "msa293",
            "mpsama2",
            "wsa223",
            "msa520",
            "csa233",
            "schardl",
            "rrsc223",
            "clshaf2",
            "cbsh232",
            "jsh278",
            "jasmal3",
            "ademi0",
            "basp238",
            "srsp224",
            "sdst245",
            "tjhann00",
            "tast268",
            "cjst223",
            "spsuma2",
            "jrsw222",
            "zsy224",
            "ktanaka",
            "nmte222",
            "ttobin",
            "mhtr222",
            "rjtr224",
            "otsyu2",
            "junri2",
            "klur222",
            "vaillan",
            "dvs",
            "evanzant",
            "ava233",
            "lavi228",
            "skvinc2",
            "zwa301",
            "oowend2",
            "scwesl2",
            "jawh222",
            "rpwi236",
            "mawillia",
            "cjwi284",
            "nwo222",
            "ylxiong",
            "jya246",
            "lyuan3",
            "szh295",
            "zyu232",
            "hzhu4",
            "bcme228",
            "yahe223",
            "jro225",
            "dwa240",
            "rmbrow00",
            "ubryant",
            "rlhi227",
            "jgjane2",
            "atloyn2",
            "aamc264",
            "mcro237",
            "reru228",
            "mpsw224",                       
        };

        string[] types = new string[]{ "District Reports", "Planning Unit Report", "KSU", "UK", "All","","","Extension Area Reports", "Extension Region Report" };
        public EmployeesController( 
                    KERScoreContext context,
                    KERSreportingContext reportingContext ,
                    ITrainingRepository trainingRepo,
                    IFiscalYearRepository fiscalYearRepository,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo,
                    IExpenseRepository expenseRepo,
                    IConfiguration Configuration
            ){
           this.context = context;
           this.reportingContext = reportingContext;
           this.trainingRepo = trainingRepo;
           this.fiscalYearRepository = fiscalYearRepository;
           this.currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");
           this._cache = _cache;
           this.activityRepo = activityRepo;
           this.contactRepo = contactRepo;
           this.expensesRepo = expenseRepo;
           _configuration = Configuration;
        }


        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            

            // Import Areas
            //ImportAreas();
            

            /* 
            var fiscalYear = this.fiscalYearRepository.byName("2019", FiscalYearType.ServiceLog);




            var districts =  context.District;
            foreach( var district in districts){
                var tbl = await contactRepo.DataByEmployee(fiscalYear,0, district.Id, true);  
                tbl = await contactRepo.DataByMajorProgram(fiscalYear, 0,district.Id,true);
            }
            // Planning Units
            var units = context.PlanningUnit;
            foreach( var unit in units){
                var tbl = await contactRepo.DataByEmployee(fiscalYear,1, unit.Id, true); 
                tbl = await contactRepo.DataByMajorProgram(fiscalYear, 1,unit.Id,true);   
            }



 */

            //UnitAddress();

            //var table = await contactRepo.DataByMajorProgram(fiscalYear, 2,0,true);
            //table = await contactRepo.DataByMajorProgram(fiscalYear, 3,0,true);
            //var table = await contactRepo.DataByEmployee(fiscalYear, 2, 0, true);
            //table = await contactRepo.DataByEmployee(fiscalYear, 3, 0, true);
            return View();
        }

        private void UnitAddress(){
            var cnts = context.PlanningUnit.Where( u => u.GeoFeature != null);
            foreach( var cnt in cnts){
               /*  var location = new ExtensionEventLocation();
                location.DisplayName = "County Extension Office";
                location.Address = new PhysicalAddress();
                location.Address.Building = cnt.FullName;
                location.Address.Street = cnt.Address;
                location.Address.City = cnt.City;
                location.Address.PostalCode = cnt.Zip;
                location.Address.State = "Kentucky";
                cnt.Location = location; */
                var Geography = new PlanningUnitGeography();
                Geography.GeoFeature = cnt.GeoFeature;
                Geography.FIPSCode = cnt.FIPSCode;
                cnt.Geography = Geography;
            }
            this.context.SaveChanges();
        }

        

        private List<ProgramIndicator> moveIndicators( string fromFYName, string toFYName){
            var indicators = new List<ProgramIndicator>();

            
            var vals = this.context.ProgramIndicatorValue
                        .Where( v => v.ProgramIndicator.MajorProgram.StrategicInitiative.FiscalYear.Name == fromFYName)
                        .Include( v => v.ProgramIndicator);

            foreach( var val in vals){

                var replacement = this.context.ProgramIndicator
                                    .Where( i => i.MajorProgram.StrategicInitiative.FiscalYear.Name == toFYName
                                                &&
                                                i.Question == val.ProgramIndicator.Question)
                                    .FirstOrDefault();
                if( replacement != null ) val.ProgramIndicator = replacement;
                indicators.Add( val.ProgramIndicator);
            }

            this.context.SaveChanges();

            return indicators;
        }

        private List<Story> moveStories( string fromFYName, string toFYName){
            var strs = new List<Story>();

            var stories = this.context.Story
                            .Where( s => 
                                s.MajorProgram.StrategicInitiative.FiscalYear.Name == fromFYName 
                                && 
                                s.MajorProgram.StrategicInitiative.FiscalYear.Type == FiscalYearType.ServiceLog)
                            .Include( r => r.Revisions)
                            .Include( r => r.MajorProgram);

            foreach( var str in stories){
                var replacement = this.context.MajorProgram.Where(
                    m => 
                        m.StrategicInitiative.FiscalYear.Name == toFYName
                        &&
                        m.StrategicInitiative.FiscalYear.Type == FiscalYearType.ServiceLog
                        &&
                        m.PacCode == str.MajorProgram.PacCode
                ).FirstOrDefault();
                if(replacement != null){
                    foreach( var rev in str.Revisions){
                        rev.MajorProgram = replacement;
                        rev.PlanOfWorkId = 0;
                    }
                    str.MajorProgram = replacement;
                }
                strs.Add( str );
            }

            this.context.SaveChanges();
            return strs;
        }


        [HttpGet]
        [Route("facultydata/{year?}")]
        public async Task<IActionResult> FacultyData(string year = "2024")
        {
            


            var fiscalYear = this.GetFYByName(year);
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;





            //var data = contactRepo.GetActivitiesAndContactsAsync(fiscalYear.Start, fiscalYear.End, 4, 0);

            var data = new List<PerGroupActivities>();


            var filteredContacts = context.Contact
                                .Where( a =>    a.ContactDate < fiscalYear.End && a.ContactDate > fiscalYear.Start 
                                                &&
                                                faculty.Contains(a.KersUser.RprtngProfile.LinkBlueId)).ToList();

            var Contacts = filteredContacts
                                .GroupBy( a => a.KersUserId)
                                .Select( c => new {
                                                Ids = c.Select(s => s.Id).ToList(),
                                                UserID = c.Key
                                            }
                                );


            foreach( var contactGroup in Contacts ){
                var GroupRevisions = new List<ContactRevision>();
                var OptionNumbersValues = new List<IOptionNumberValue>();
                var RaceEthnicities = new List<IRaceEthnicityValue>();
                foreach( var rev in contactGroup.Ids){
                
                    var lstrvsn = context.ContactRevision.
                                Where(r => r.ContactId == rev).
                                Include(a => a.ContactOptionNumbers).ThenInclude(o => o.ActivityOptionNumber).
                                Include(a => a.ContactRaceEthnicityValues).
                                OrderBy(a => a.Created).LastOrDefault();
                    if(lstrvsn != null ){
                        GroupRevisions.Add(lstrvsn);
                    }
                    OptionNumbersValues.AddRange(lstrvsn.ContactOptionNumbers);
                    RaceEthnicities.AddRange(lstrvsn.ContactRaceEthnicityValues);
                }
                var actvts = new PerGroupActivities();
                actvts.RaceEthnicityValues = RaceEthnicities;
                actvts.OptionNumberValues = OptionNumbersValues;
                actvts.Hours = GroupRevisions.Sum( r => r.Days) * 8;
                actvts.Audience = GroupRevisions.Sum( r => r.Male) + GroupRevisions.Sum( r => r.Female);
                actvts.Male = GroupRevisions.Sum( r => r.Male);
                actvts.Female = GroupRevisions.Sum( r => r.Female);
                actvts.GroupId = contactGroup.UserID;
                actvts.Multistate = GroupRevisions.Sum(r => r.Multistate) * 8;
                data.Add(actvts);
            }




            var table = new TableViewModel();
            table.Header = new List<string>{
                                "Planning Unit", "LinkBlueId", "Employee", "Days", "Multistate", "Total Contacts"
                            };

            var Races = this.context.Race.OrderBy(r => r.Order);
            var Ethnicities = this.context.Ethnicity.OrderBy( e => e.Order);
            var OptionNumbers = this.context.ActivityOptionNumber.OrderBy( n => n.Order);
            foreach( var race in Races){
                table.Header.Add(race.Name);
            }
            foreach( var ethn in Ethnicities){
                table.Header.Add(ethn.Name);
            }
            table.Header.Add("Male");
            table.Header.Add("Female");
            foreach( var opnmb in OptionNumbers){
                table.Header.Add(opnmb.Name);
            }
            table.Header.Add("Success Stories Count");
            var Rows = new List<List<string>>();


            foreach( var d in data){
                var user = await this.context.KersUser.Where(u => u.Id == d.GroupId)
                                .Include( u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit)
                                .FirstOrDefaultAsync();
                if( user != null && faculty.Contains(user.RprtngProfile.LinkBlueId) ){
                    var row = new List<string>();
                    row.Add(user.RprtngProfile.PlanningUnit.Name);
                    row.Add( user.RprtngProfile.LinkBlueId);
                    row.Add( user.RprtngProfile.Name);
                    row.Add( (d.Hours / 8 ).ToString());
                    row.Add( (d.Multistate / 8 ).ToString());
                    row.Add( d.Audience.ToString());
                    foreach( var race in Races){
                        var raceAmount = d.RaceEthnicityValues.Where( v => v.RaceId == race.Id).Sum( r => r.Amount);
                        row.Add(raceAmount.ToString());
                    }
                    foreach( var et in Ethnicities){
                        var ethnAmount = d.RaceEthnicityValues.Where( v => v.EthnicityId == et.Id).Sum( r => r.Amount);
                        row.Add(ethnAmount.ToString());
                    }
                    row.Add(d.Male.ToString());
                    row.Add(d.Female.ToString());
                    foreach( var opnmb in OptionNumbers){
                        var optNmbAmount = d.OptionNumberValues.Where( o => o.ActivityOptionNumberId == opnmb.Id).Sum( s => s.Value);
                        row.Add( optNmbAmount.ToString());
                    }
                    row.Add( context.Story.Where(s => s.KersUser == user && s.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear).Count().ToString() );
                    Rows.Add(row);
                }
            }
            table.Rows = Rows;
            table.Foother = new List<string>();
            return View(table);
        }

        [HttpGet]
        [Route("icsfile")]
        public FileResult DownloadFile(){
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter objstreamwriter = new StreamWriter(stream);


                var text = "BEGIN:VCALENDAR\n"+
"VERSION:2.0\n"+
"PRODID:-//ical.marudot.com//iCal Event Maker\n"+
"CALSCALE:GREGORIAN\n"+
"BEGIN:VTIMEZONE\n"+
"TZID:America/Chicago\n"+
"TZURL:http://tzurl.org/zoneinfo-outlook/America/Chicago\n"+
"X-LIC-LOCATION:America/Chicago\n"+
"BEGIN:DAYLIGHT\n"+
"TZOFFSETFROM:-0600\n"+
"TZOFFSETTO:-0500\n"+
"TZNAME:CDT\n"+
"DTSTART:19700308T020000\n"+
"RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU\n"+
"END:DAYLIGHT\n"+
"BEGIN:STANDARD\n"+
"TZOFFSETFROM:-0500\n"+
"TZOFFSETTO:-0600\n"+
"TZNAME:CST\n"+
"DTSTART:19701101T020000\n"+
"RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU\n"+
"END:STANDARD\n"+
"END:VTIMEZONE\n"+
"BEGIN:VEVENT\n"+
"DTSTAMP:20210114T163918Z\n"+
"UID:20210114T163918Z-1385374703@marudot.com\n"+
"DTSTART;TZID=America/Chicago:20220112T120000\n"+
"DTEND;TZID=America/Chicago:\n"+
"SUMMARY:Welcome to the Motorway\n"+
"URL:https://en.wikipedia.org/wiki/Gridlock_(Doctor_Who)\n"+
"DESCRIPTION:Learn all about the rules of the Motorway and how to access the fast lane. \n"+
"LOCATION:New Earth\n"+
"TRANSP:OPAQUE\n"+
"X-MICROSOFT-CDO-BUSYSTATUS:BUSY\n"+
"BEGIN:VALARM\n"+
"ACTION:DISPLAY\n"+
"DESCRIPTION:Welcome to the Motorway\n"+
"TRIGGER:-PT15M\n"+
"END:VALARM\n"+
"END:VEVENT\n"+
"END:VCALENDAR";

                objstreamwriter.Write(text);
                objstreamwriter.Flush();
                objstreamwriter.Close(); 
                return File(stream.ToArray(), "text/calendar", "ExtensionTraining.ics");
            }
            
        }

        [HttpGet]
        [Route("facultystories/{year?}")]
        public async Task<IActionResult> FacultyStories(string year = "2023")
        {
            var fiscalYear = this.GetFYByName(year);
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            var stories = new List<StoryViewModel>();
            foreach( var LinkBlueId in faculty){
                var user = await this.context.KersUser.Where( r => r.RprtngProfile.LinkBlueId == LinkBlueId).FirstOrDefaultAsync();
                if( user != null){
                    var userStories =  await this.context.Story
                                        .Where( s => s.KersUser == user && s.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear)
                                        .Include( s => s.Revisions)
                                        .Include( s => s.MajorProgram)
                                        .Include( s => s.PlanningUnit)
                                        .Include( s => s.KersUser).ThenInclude( u => u.PersonalProfile)
                                        .OrderBy( s => s.KersUserId)
                                        .ToListAsync();
                    foreach( var story in userStories ){
                        var storyModel = new StoryViewModel();
                        storyModel.KersUser = user;
                        storyModel.MajorProgram = story.MajorProgram;
                        storyModel.PlanningUnit = story.PlanningUnit;
                        storyModel.StoryId = story.Id;
                        var lastRevision = story.Revisions.OrderBy( s => s.Created).Last();
                        storyModel.Story = lastRevision.Story;
                        storyModel.Title = lastRevision.Title;
                        storyModel.Updated = lastRevision.Created;
                        stories.Add( storyModel);
                    }
                }
            }
            return View(stories);
        }

        [HttpGet]
        [Route("expenses/{districtid?}/{month?}/{year?}")]
        public IActionResult Expenses(int districtid=4, int month = 8, int year = 2018)
        {

            var ret = new List<List<ExpenseSummary>>();   

            /* 
            var allSummaries = new List<ExpenseSummary>();

            var counties = context.PlanningUnit
                                        .Where( u => u.DistrictId == districtid && u.Name.Substring( u.Name.Count() - 3, 3) == "CES")
                                        .OrderBy( u => u.Name )
                                        .ToArray();
            ViewData["counties"] = counties;
            var allEmployees = new List<KersUser>();
            foreach( var county in counties){
                var ByCounty = new List<ExpenseSummary>();
                var emp = this.context.KersUser.Where( u => u.RprtngProfile.PlanningUnit == county).Include( u => u.RprtngProfile);
                allEmployees.AddRange( emp );
                foreach( var e in emp ){
                    var sumr = this.expensesRepo.Summaries(e, year, month);
                    ByCounty.AddRange( sumr );
                }
                allSummaries.AddRange( ByCounty );
                ret.Add( BySource( ByCounty ) );
            }

            ViewData["total"] = BySource(allSummaries);
            
 */
            

            return View(ret);
        }

        private List<ExpenseSummary> BySource( List<ExpenseSummary> mixed ){
            var sources = this.context.ExpenseFundingSource;
            var bySource = new List<ExpenseSummary>();
            foreach( var src in sources ){
                var sumrs = mixed.Where( s => s.fundingSource == src);
                var smr = new ExpenseSummary{
                    fundingSource = src,
                    miles = sumrs.Sum( s => s.miles),
                    mileageCost = sumrs.Sum( s => s.mileageCost),
                    meals = sumrs.Sum( s => s.meals),
                    lodging = sumrs.Sum( s => s.lodging),
                    registration = sumrs.Sum( s => s.registration),
                    other = sumrs.Sum( s => s.other),
                    total = sumrs.Sum( s => s.total)
                };
                bySource.Add( smr );

            }

            return bySource;
        }


        [HttpGet]
        [Route("[action]/{type}/{id?}/{fy?}")]
        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All, 7 Area, 8 Region
        public async Task<IActionResult> Data(int type, int id = 0, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                
                return new StatusCodeResult(500);
            }
            
            // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All, 7 Area, 8 Region
            var table = await contactRepo.DataByEmployee(fiscalYear, type, id);

            ViewData["Type"] = type;
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            ViewData["Subtitle"] = types[type];
            if(type == 0){
                ViewData["Title"] = this.context.District.Find(id).Name;
            }else if(type == 1){
                ViewData["Title"] = this.context.PlanningUnit.Find(id).Name;
            }else if(type == 7){
                ViewData["Title"] = this.context.ExtensionArea.Find(id).Name;
            }else if(type == 8){
                ViewData["Title"] = this.context.ExtensionRegion.Find(id).Name;
            }
            

            return View(table);
        }


        public FiscalYear GetFYByName(string fy, string type = "serviceLog"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalYearRepository.currentFiscalYear(type);
            }else{
                fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == type).FirstOrDefault();
            }
            return fiscalYear;
        }
        

        
       
    }

}