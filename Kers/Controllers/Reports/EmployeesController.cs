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
            "aec137", 
            "rmbrow00", 
            "scbu225", 
            "kburdine", 
            "adreum2", 
            "cdillon", 
            "gshali2", 
            "sisaacs", 
            "knyo224", 
            "tbmark0", 
            "npa286", 
            "jro225", 
            "shsagh2", 
            "jksc222", 
            "jmshoc2", 
            "wsnell", 
            "cjst223", 
            "tawoods", 
            "szh295", 
            "zyu232", 
            "tdda229", 
            "zer223", 
            "woodch", 
            "daaron", 
            "saad232", 
            "damaral", 
            "landerso", 
            "wlboat1", 
            "pbrid2", 
            "dbullock", 
            "fccama2", 
            "jhca235", 
            "rcoffey", 
            "rcoleman", 
            "rmdwye2", 
            "dely", 
            "dharmon", 
            "bhennig", 
            "llawrenc", 
            "jwle222", 
            "mdlind1", 
            "kmcleod", 
            "mnewman", 
            "apescato", 
            "gkrent2", 
            "mgross2", 
            "rrsc223", 
            "spsuma2", 
            "klur222", 
            "krva228", 
            "evanzant", 
            "ppvi223", 
            "ylxiong", 
            "aad244", 
            "tjba240", 
            "collivr", 
            "crofche", 
            "jdv223", 
            "wiford2", 
            "mdhaye3", 
            "jjjack3", 
            "smcneill", 
            "tlgrah2", 
            "arabad2", 
            "montross", 
            "snokes", 
            "mpe277", 
            "mpsama2", 
            "wsa223", 
            "jsh278", 
            "tss", 
            "ctande2", 
            "ncox", 
            "bdlee2", 
            "baba225", 
            "ndbrea0", 
            "pdyk", 
            "rep222", 
            "bhain2", 
            "kgrick2", 
            "rharris", 
            "rwharr00", 
            "rhusted", 
            "dwka224", 
            "almeye2", 
            "bol232", 
            "krign2", 
            "lsh237", 
            "ktanaka", 
            "skvinc2", 
            "jzimm", 
            "mba444", 
            "sbastin", 
            "dbr239", 
            "elmars2", 
            "kdfl224", 
            "agu227", 
            "jaberr0", 
            "yljack2", 
            "clu247", 
            "jtietyen", 
            "hlno222", 
            "jpl225", 
            "akschw2", 
            "rcfi223", 
            "tjhann00", 
            "rbessin", 
            "zde234", 
            "sdobson", 
            "jdu282", 
            "tmwilk2", 
            "cfox", 
            "djgo227", 
            "khaynes", 
            "jlla226", 
            "rpalli", 
            "dapotter", 
            "lrieske", 
            "ccri226", 
            "zsy224", 
            "nmte222", 
            "rtvi223", 
            "jawh222", 
            "xnzh222", 
            "jhans2", 
            "yho236", 
            "hkim3", 
            "dbro223", 
            "avail2", 
            "ava233", 
            "rjwern2", 
            "tawe223", 
            "nwo222", 
            "marthur", 
            "barton", 
            "jjcox2", 
            "evcr224", 
            "jmlhot2", 
            "jjmu233", 
            "tooc222", 
            "sjpr223", 
            "jringe", 
            "msp238", 
            "stringer", 
            "jya246", 
            "krcorb3", 
            "sdebo2", 
            "adownie", 
            "rdurham", 
            "rgeneve", 
            "rhoutz", 
            "klja223", 
            "wgow223", 
            "cmro267", 
            "reru226", 
            "rascot0", 
            "snyder", 
            "mawillia", 
            "lyuan3", 
            "ncranksh", 
            "raharg0", 
            "jko234", 
            "jph235", 
            "csa233", 
            "acse223", 
            "mscoyn00", 
            "jmmc259", 
            "wre232", 
            "abailey", 
            "mbarrett", 
            "lgrabau", 
            "jdgreen", 
            "jgrove", 
            "erha235", 
            "jhenning", 
            "dhild", 
            "aghunt00", 
            "tka234", 
            "camurp2", 
            "bdle222", 
            "cdlee2", 
            "trle233", 
            "cjmato2", 
            "rlmccu2", 
            "dhmcne2", 
            "lmo225", 
            "rpearce", 
            "tphillip", 
            "hjpo223", 
            "elritc2", 
            "msa293", 
            "cbsh232", 
            "jasmal3", 
            "srsmitd", 
            "cdteut0", 
            "otsyu2", 
            "junri2", 
            "dvs", 
            "oowend2", 
            "hzhu4", 
            "edangelo", 
            "sperr2", 
            "arbe243", 
            "cbr269", 
            "farman", 
            "nwa232", 
            "rlhi227", 
            "apkach2", 
            "pk62", 
            "schardl", 
            "vaillan", 
            "pvincell", 
            "kwi283", 
            "pdnagy2", 
            "krjone3", 
            "jmatthew", 
            "rca253", 
            "hetliz", 
            "vpwick0", 
            "mlee6", 
            "ylu232", 
            "rme247", 
            "jrsw222", 
            "mlturn0", 
            "scwesl2", 
            "pzh227", 
            "acel229", 
            "nlhu222", 
            "jhunter", 
            "afhosi2", 
            "crhe249", 
            "jlwahr2", 
            "lmbi222", 
            "ubryant", 
            "cncart4", 
            "lmca222", 
            "eer222", 
            "jgjane2", 
            "lakenn4", 
            "atloyn2", 
            "mcro237", 
            "reru228", 
            "mpsw224", 
            "enad222", 
            "aaadam3", 
            "ebailey", 
            "tmcham1", 
            "lsgo226", 
            "ktgraves", 
            "yahe223", 
            "dwhoro2", 
            "dkhowe2", 
            "tska223", 
            "fli230", 
            "jnmacl2", 
            "mkni223", 
            "clshaf2", 
            "sdst245", 
            "ptimoney", 
            "ttobin", 
            "mhtr222", 
            "dwa240"                        
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
        public async Task<IActionResult> FacultyData(string year = "2022")
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
        [Route("facultystories/{year?}")]
        public async Task<IActionResult> FacultyStories(string year = "2021")
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