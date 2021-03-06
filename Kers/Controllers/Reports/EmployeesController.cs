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
            "daaron", "enad222", "aaadam3", "aad244", "saad232", "ctande2", "damaral", "landerso", "darchbol", "lmbi222", "marthur", "ebailey", "abailey", "baba225", "aec137", 
"mba444", "mbarrett", "barton", "sbastin", "arbe243", "rbessin", "wlboat1", "dbolin", "cbr269", "ndbrea0", "dbr239", "pbrid2", "rmbrow00", "ubryant", "scbu225", "dbullock", 
"kburdine", "fccama2", "jhca235", "cncart4", "lmca222", "rca253", "tmcham1", "ckchow", "rcoffey", "rcoleman", "collivr", "elmars2", "tconners", "mco248", "jjcox2", "ncox", "mscoyn00", 
"ncranksh", "evcr224", "crofche", "mjcr236", "edangelo", "adreum2", "tdda229", "sdebo2", "zde234", "cdillon", "sdobson", "adownie", "wdunwell", "jdu282", "rdurham", "jdv223", "rmdwye2", 
"pdyk", "hetliz", "acel229", "dely", "rep222", "zer223", "eer222", "aes225", "farman", "tmwilk2", "kdfl224", "wiford2", "wfountai", "cfox", "dfresh", "clgask2", 
"nwa232", "rgeneve", "bmgo223", "djgo227", "mgoodin", "lgrabau", "ktgraves", "jdgreen", "jgrove", "agu227", "bhain2", "kgrick2", "dlhale00", "gshali2", "jhans2", "erha235", "raharg0", "dharmon", "rharris", "rwharr00", "mdhaye3", 
"khaynes", "cjheath", "gheersch", "crhe249", "bhennig", "jhenning", "dhild", "rlhi227", "mbrum2", "dwhoro2", "yho236", "jaberr0", "rhoutz", "dkhowe2", "nlhu222", "aghunt00", "jhunter", "rhusted", "dingram", "sisaacs", "jjjack3", "vpwick0", "yljack2", "klja223", "jgjane2", "krjone3", "apkach2", 
"pk62", "dwka224", "tska223", "tka234", "lakenn4", "hkim3", "camurp2", "jko234", "afhosi2", "jkurzyns", "knyo224", "mlacki", "jlla226", "llawrenc", "bdle222", "bdlee2", "cdlee2", "mlee6", "trle233", "jwle222", "jmlhot2", "fli230", "mdlind1", "atloyn2", "ylu232", "clu247", "jnmacl2", "tbmark0", "cjmato2", "jmatthew", "rlmccu2", 
"kmcd", "jmmc259", "kmcleod", "dhmcne2", "smcneill", "rme247", "almeye2", "rdmiller", "kspill", "arabad2", "lmo225", "montross", "mnewman", "jjmu233", "jtietyen", "gcmuns0", 
"pdnagy2", "mkni223", "snokes", "hlno222", "jobry2", "tooc222", "bol232", "wgow223", "rpalli", "gpalmer", "npa286", "rpearce", "sperr2", "apescato", "mpe277", "tpfeiffe", 
"epf222", "jph235", "tphillip", "jpl225", "hjpo223", "dapotter", "mpotter", "sjpr223", "mapurs2", "mrreed", "wre232", "gkrent2", "lri244", "lrieske", "krign2", "jringe", 
"elritc2", "ccri226", "cmro267", "mcro237", "dbro223", "mgross2", "jro225", "reru228", "reru226", "shsagh2", "aasa238", "msa293", "mpsama2", "wsa223", "csa233", "schardl", 
"rrsc223", "jksc222", "akschw2", "rascot0", "acse223", "twsh226", "lsh237", "clshaf2", "cbsh232", "jsh278", "jmshoc2", "rcfi223", "jasmal3", "srsmitd", 
"hdf002", "wsnell", "snyder", "msp238", "sdst245", "tjhann00", "tss", "cjst223", "jstrang", "stringer", "spsuma2", "jrsw222", "zsy224", "ktanaka", "nmte222", "cdteut0", "ptimoney", 
"ttobin", "mhtr222", "otsyu2", "mlturn0", "junri2", "klur222", "avail2", "vaillan", "dvs", "krva228", "evanzant", "ava233", "ppvi223", "rtvi223", "pvincell", 
"skvinc2", "jlwahr2", "bawebb", "oowend2", "rjwern2", "tawe223", "scwesl2", "jawh222", "dwilliam", "mawillia", "kwi283", "woodch", "nwo222", "tawoods", "ylxiong", "jya246", 
"lyuan3", "pzh227", "szh295", "zyu232", "xnzh222", "hzhu4", "jzimm"
                        };

        string[] types = new string[]{ "District Reports", "Planning Unit Report", "KSU" };
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
        public async Task<IActionResult> FacultyData(string year = "2020")
        {
            


            var fiscalYear = this.GetFYByName(year);
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;





            //var data = contactRepo.GetActivitiesAndContactsAsync(fiscalYear.Start, fiscalYear.End, 4, 0);

            var data = new List<PerGroupActivities>();


            var Contacts = context.Contact
                                .Where( a =>    a.ContactDate < fiscalYear.End && a.ContactDate > fiscalYear.Start 
                                                &&
                                                faculty.Contains(a.KersUser.RprtngProfile.LinkBlueId))
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
        public async Task<IActionResult> FacultyStories(string year = "2020")
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
        // type: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
        public async Task<IActionResult> Data(int type, int id = 0, string fy="0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                
                return new StatusCodeResult(500);
            }
            

            var table = await contactRepo.DataByEmployee(fiscalYear, type, id);

            ViewData["Type"] = type;
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            ViewData["Subtitle"] = types[type];
            if(type == 0){
                ViewData["Title"] = this.context.District.Find(id).Name;
            }else if(type == 1){
                ViewData["Title"] = this.context.PlanningUnit.Find(id).Name;
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