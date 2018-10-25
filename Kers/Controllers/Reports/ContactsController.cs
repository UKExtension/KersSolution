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
using Microsoft.Extensions.Caching.Distributed;
using Kers.Models.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Kers.Models.Data;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class ContactsController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepository;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        const int workDaysPerYear = 228;
        public ContactsController( 
                    KERScoreContext context,
                    IFiscalYearRepository fiscalYearRepository,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo
            ){
           this.context = context;
           this.fiscalYearRepository = fiscalYearRepository;
           this._cache = _cache;
           this.activityRepo = activityRepo;
           this.contactRepo = contactRepo;
        }
        [HttpGet]
        [Route("{fy?}")]
        public IActionResult Index(string fy="0")
        {
            ViewData["fy"] = fy;
            return View();
        }
        [HttpGet]
        [Route("[action]/{fy?}")]
        public IActionResult State(string fy="0")
        {
            ViewData["fy"] = fy;
            return View();
        }

        [HttpGet]
        [Route("[action]/{fy?}")]
        public async Task<IActionResult> StateAll(string fy = "0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);

            if(fiscalYear == null){
                //this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", "Reports", "Error");
                return new StatusCodeResult(500);
            }

            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            var table = await contactRepo.DataByEmployee(fiscalYear, 4);
            return View(table);
        }


        [HttpGet]
        [Route("[action]/{type?}/{fy?}")]
        // type: 0 - all, 1 - UK, 2 - KSU
        public async Task<IActionResult> StateByMajorProgram(int type = 0, string fy = "0")
        {
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            var repoType = 4;


            

            if(type == 1){
                repoType = 3;
                ViewData["Title"] = "UK ";
            }else if( type == 2 ){
                repoType = 2;
                ViewData["Title"] = "KSU ";
            }
            var table = await contactRepo.DataByMajorProgram(fiscalYear, repoType);
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            return View(table);
        }


        [HttpGet]
        [Route("[action]/{fy?}")]
        public async Task<IActionResult> ContactsByCountyByMajorProgram(string fy = "0")
        {
            
            FiscalYear fiscalYear = GetFYByName(fy);
            

            if(fiscalYear == null){
                //this.Log( fy ,"string", "Invalid Fiscal Year Idetifyer in Total By Month Snap Ed CSV Data Request.", "Reports", "Error");
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;
            var table = await activityRepo.ContactsByCountyByMajorProgram(fiscalYear);
            return View(table);
        }


        



        [HttpGet]
        [Route("[action]/{filter?}/{id?}/{fy?}", Name = "ContactsDataByMonthByMajorProgram")]
        public async Task<IActionResult> DataByMonthByMajorProgram(int filter = 1, int id = 0, string fy = "0")
        {
            // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;

            var fiscalYearSummaries = await contactRepo.GetPerPeriodSummaries(fiscalYear.Start, fiscalYear.End, filter, id);
            float[] SummariesArray = fiscalYearSummaries.ToArray();

            ViewData["totalHours"] = SummariesArray[0];
            ViewData["totalContacts"] = (int) SummariesArray[1];
            ViewData["totalMultistate"] = SummariesArray[2];
            ViewData["totalActivities"] = (int) SummariesArray[3];

            if( filter == 1 ){
                ViewData["unit"] = context.PlanningUnit.Where( u => u.Id == id ).FirstOrDefault();
                if(ViewData["unit"] == null){
                    return new StatusCodeResult(500);
                }
            }else if( filter == 0 ){
                ViewData["district"] = context.District.Where( u => u.Id == id ).FirstOrDefault();
                if(ViewData["district"] == null){
                    return new StatusCodeResult(500);
                }
            }else if( filter > 4 ){
                return new StatusCodeResult(500);
            }


            List< List< PerGroupSummary >> result = await this.DataByMonth(fiscalYear, filter, 1, id);


            var output = new List< List< Kers.Models.Data.PerProgramActivities >>();

            var months = new List<string>();
            var hours = new List<string>();
            var runningDate = new DateTime( fiscalYear.Start.Year, fiscalYear.Start.Month, 28);


            foreach( var MonthResult in result){
                var MonthData = new List<Kers.Models.Data.PerProgramActivities>();
                months.Add( "'" + runningDate.ToString("MM/yyyy") + "'");
                runningDate = runningDate.AddMonths( 1 );
                hours.Add( MonthResult.Sum( s => s.Hours ).ToString());
                foreach( var res in MonthResult ){
                    var ProgramActivities = new Kers.Models.Data.PerProgramActivities();
                    ProgramActivities.Audience = res.Audience;
                    ProgramActivities.Female = res.Female;
                    ProgramActivities.Hours = res.Hours;
                    ProgramActivities.MajorProgram = this.context.MajorProgram.Find( res.GroupId );
                    ProgramActivities.Male = res.Male;
                    ProgramActivities.Multistate = res.Multistate;
                    MonthData.Add( ProgramActivities );
                }
                output.Add(MonthData);
            }



            var monthsString = "[" + months.Aggregate((i, j) => i  + "," +  j) + "]";

            ViewData["months"] = monthsString;
            ViewData["hours"] = "[" + hours.Aggregate((i, j) => i + "," + j) + "]";



            

            var returnList = new List<Kers.Models.Data.PerProgramActivities>();

            var currentMonthNum = 0;

            var ProgramDataPerMonth = new List<ProgramDataPerMonth>();

            foreach( var month in output ){
                foreach( var program in month ){
                    var existingProgram = returnList.Where( r => r.MajorProgram == program.MajorProgram).FirstOrDefault();
                    if( existingProgram == null ){

                        var programEntry = new ProgramDataPerMonth();
                        programEntry.MajorProgram = program.MajorProgram;
                        programEntry.Audience = new List<int>();
                        programEntry.Hours = new List<float>();
                        programEntry.Male = new List<int>();
                        programEntry.Female = new List<int>();
                        programEntry.Multistate = new List<float>();
                        for( var i = 0; i < currentMonthNum; i++ ){
                            programEntry.Audience.Add(0);
                            programEntry.Hours.Add(0);
                            programEntry.Male.Add(0);
                            programEntry.Female.Add( 0 );
                            programEntry.Multistate.Add( 0 );
                        }
                        programEntry.Audience.Add( program.Audience );
                        programEntry.Hours.Add( program.Hours );
                        programEntry.Male.Add( program.Male );
                        programEntry.Female.Add( program.Female );
                        programEntry.Multistate.Add( program.Multistate );

                        ProgramDataPerMonth.Add(programEntry);

                        existingProgram = new Kers.Models.Data.PerProgramActivities();
                        existingProgram.Audience = program.Audience;
                        existingProgram.Female = program.Female;
                        existingProgram.Hours = program.Hours;
                        existingProgram.MajorProgram = program.MajorProgram;
                        existingProgram.Male = program.Male;
                        existingProgram.Multistate = program.Multistate;
                        existingProgram.OptionNumberValues = program.OptionNumberValues;
                        existingProgram.RaceEthnicityValues = program.RaceEthnicityValues;
                        returnList.Add(existingProgram);

                    }else{
                        existingProgram.Audience += program.Audience;
                        existingProgram.Female += program.Female;
                        existingProgram.Hours += program.Hours;
                        existingProgram.Male += program.Male;
                        existingProgram.Multistate += program.Multistate;

                        var programEntry = ProgramDataPerMonth.Where( p => p.MajorProgram == program.MajorProgram ).First();
                        programEntry.Audience.Add(program.Audience);
                        programEntry.Female.Add(program.Female);
                        programEntry.Hours.Add(program.Hours);
                        programEntry.Male.Add(program.Male);
                        programEntry.Multistate.Add(program.Multistate);
                    }
                }
                foreach( var inPrograms in ProgramDataPerMonth ){
                    if(  !month.Where( m => m.MajorProgram == inPrograms.MajorProgram ).Any()){
                        inPrograms.Audience.Add( 0 );
                        inPrograms.Male.Add( 0 );
                        inPrograms.Female.Add( 0 );
                        inPrograms.Hours.Add(0);
                        inPrograms.Multistate.Add(0);
                    }
                }
                currentMonthNum++;
            }

            ProgramDataPerMonth = ProgramDataPerMonth
                                    .Where( p => p.MajorProgram != null ).ToList();

            ViewData["AllProgramsData"] = ProgramDataPerMonth.OrderByDescending( p => p.Audience.Sum(s => s)).ToList();
           
            var ProgramsGendersGraphDataList = new List<string>();
            foreach( var theProgram in ProgramDataPerMonth ){
                ProgramsGendersGraphDataList.Add(" ["+theProgram.Female.Sum(s => s)+", "+theProgram.Male.Sum(s => s)+", \""+theProgram.MajorProgram.Name+"\"]");
            }
            ViewData["ProgramsGendersGraphDataList"] = "[" + string.Join(",", ProgramsGendersGraphDataList.ToArray() ) + "]";

            ProgramDataPerMonth = ProgramDataPerMonth
                                    .OrderByDescending( p => p.Audience.Sum(s => s))
                                    .Take(5)
                                    .OrderBy(p => p.MajorProgram.Name )
                                    .ToList();

            
            var ProgramsListOfStrings = new List<string>();
            var ProgramsHoursGraphDataList = new List<string>();
            var ProgramsContactsByProgramSeries = new List<string>();
            var ProgramLength = 15;
            foreach(var theProgram in ProgramDataPerMonth ){
                var shortenedProgramName = (theProgram.MajorProgram.Name.Length > ProgramLength ? theProgram.MajorProgram.Name.Substring(0, ProgramLength) + "..." : theProgram.MajorProgram.Name);
                ProgramsListOfStrings.Add( "\"" + shortenedProgramName + "\"" );
                ProgramsContactsByProgramSeries.Add("{ name: \""+shortenedProgramName+"\", type: \"bar\", data: [" + string.Join(",", theProgram.Audience.Select(n => n.ToString()).ToArray())+"],}");
                ProgramsHoursGraphDataList.Add("{ name: \""+shortenedProgramName+"\", type: \"line\", smooth: !0, itemStyle: { normal: { areaStyle: { type: \"default\" } } }, data: [" + string.Join(",", theProgram.Hours.Select(n => n.ToString()).ToArray())+"]  }");
            }

            ViewData["programsForTheLegend"] = "[" + string.Join(",", ProgramsListOfStrings.ToArray() ) + "]";
            ViewData["ProgramsHoursGraphDataList"] = "[" + string.Join(",", ProgramsHoursGraphDataList.ToArray() ) + "]";

            ViewData["ProgramsContactsByProgramSeries"] = "[" + string.Join(",", ProgramsContactsByProgramSeries.ToArray() ) + "]";

            ViewData["ProgramDataPerMonth"] = ProgramDataPerMonth;



            var FilteredActivities = context.Activity.Where( a => 
                                                                    a.ActivityDate > fiscalYear.Start
                                                                    &&
                                                                    a.ActivityDate < fiscalYear.End
                                                                );

            var FilteredStories = context.Story.Where( s =>
                                                                    s.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear
                                                                );

            if(filter == 1 ){
                //Planning Unit
                FilteredActivities = FilteredActivities.Where( a => a.PlanningUnitId == id);
                FilteredStories = FilteredStories.Where( s => s.PlanningUnitId == id );
            }else if(filter == 0){
                //District
                FilteredActivities = FilteredActivities.Where( a => a.PlanningUnit.DistrictId == id);   
                FilteredStories = FilteredStories.Where( s => s.PlanningUnit.DistrictId == id );
            }else if( filter == 2 ){
                //KSU
                FilteredActivities = FilteredActivities.Where( a => a.KersUser.RprtngProfile.Institution.Code == "21000-1890"); 
                FilteredStories = FilteredStories.Where( s => s.KersUser.RprtngProfile.Institution.Code == "21000-1890" );
            }else if ( filter == 3){
                //UK
                FilteredActivities = FilteredActivities.Where( a => a.KersUser.RprtngProfile.Institution.Code != "21000-1890"); 
                FilteredStories = FilteredStories.Where( s => s.KersUser.RprtngProfile.Institution.Code != "21000-1890" );
            }

            var EmployeeActivities = FilteredActivities.GroupBy( a => 
                                                    new {
                                                        User = a.KersUser
                                                    }
                                                )
                                                .Select( s => new {
                                                        User = s.Key,
                                                        Activities = s.Select(a => a)
                                                    }   
                                                ).OrderByDescending( s => s.Activities.Count())
                                                .Take(10);

            var EmployeeDataForTheGraph = new List<string>();

            foreach( var EmployeeData in EmployeeActivities ){
                var name = context.KersUser.Where( u => u.Id == EmployeeData.User.User.Id).Include( u => u.RprtngProfile).First();
                EmployeeDataForTheGraph.Add(
                                "\n{ \"name\": \"" + System.Net.WebUtility.HtmlEncode(name.RprtngProfile.Name) + "\","
                                + " \"category\": \"Employees\","
                                + "\"label\":{\"normal\":{"
                                + "\"show\":"+(EmployeeData.Activities.Count() > 3 ? "true" : "false" ) + ","
                                + "\"textStyle\":{\"color\":\"#72c380\"}}},"
                                + "\"symbolSize\":"+Math.Min(EmployeeData.Activities.Count(), 15) + ","
                                + "\"value\": 1}"
                );
            }

            var MajorProgramActivities = FilteredActivities.GroupBy( a => new {
                                                        MajorProgram = a.MajorProgram
                                                    }
                                                )
                                                .Select( s => new {
                                                        MajorProgram = s.Key,
                                                        Activities = s.Select(a => a)
                                                    }   
                                                ).OrderByDescending( s => s.Activities.Count())
                                                .Take(14);
            var MajorProgramStories = FilteredStories.GroupBy( a => new {
                                                        MajorProgram = a.MajorProgram
                                                    }
                                                )
                                                .Select( s => new {
                                                        MajorProgram = s.Key,
                                                        Stories = s.Select(a => a)
                                                    }   
                                                ).Take(10);

            var ProgramDataForTheGraph = new List<string>();

            var LinksDataForTheGraph = new List<string>();

            int LabelLength = 25;

            foreach( var ProgramData in MajorProgramActivities ){
                var shortenedProgramName = System.Net.WebUtility.HtmlEncode(ProgramData.MajorProgram.MajorProgram.Name.Count() > LabelLength ? ProgramData.MajorProgram.MajorProgram.Name.Replace("'", "").Replace("&", "").Substring( 0, LabelLength ) + "..." : ProgramData.MajorProgram.MajorProgram.Name.Replace("'", "").Replace("&", ""));
                ProgramDataForTheGraph.Add(
                                            "\n{ \"name\": \"" + shortenedProgramName + "\", "
                                            + "\"label\":{\"normal\":{\"show\":"
                                            + (ProgramData.Activities.Count() > 20 ? "true" : "false" )
                                            +",\"textStyle\":{\"color\":\"#6f7a8a\"}}}, "
                                            + "\"category\": \"Major Programs\",\"symbolSize\":" + Math.Min(ProgramData.Activities.Count(), 15) + ", "
                                            + "\"value\": 1}");
                
                var ProgramDataGrouppedByEmployee = ProgramData.Activities.GroupBy( a => a.KersUserId ).Select( s => s );
                foreach( var GrouppedProgramData in ProgramDataGrouppedByEmployee ){
                    var TargetName = context.KersUser.Where( u => u.Id == GrouppedProgramData.Key).Include( u => u.RprtngProfile).First();
                    LinksDataForTheGraph.Add("\n{ \"source\": \"" + shortenedProgramName + "\",\"target\": \"" + System.Net.WebUtility.HtmlEncode(TargetName.RprtngProfile.Name)+"\"}");
                }
                
            }


            var StoryDataForTheGraph = new List<string>();


            foreach( var StoryData in MajorProgramStories ){
                var shortenedProgramName = System.Net.WebUtility.HtmlEncode(StoryData.MajorProgram.MajorProgram.Name.Count() > LabelLength ? StoryData.MajorProgram.MajorProgram.Name.Replace("'", "").Replace("&", "").Substring( 0, LabelLength ) + "..." : StoryData.MajorProgram.MajorProgram.Name.Replace("'", "").Replace("&", ""));
                /* if( !MajorProgramActivities.Where( a => a.MajorProgram == StoryData.MajorProgram).Any()){
                    ProgramDataForTheGraph.Add(
                                            "\n{ \"name\": \"" + shortenedProgramName + "\", "
                                            + "\"label\":{\"normal\":{\"show\":false"
                                            +",\"textStyle\":{\"color\":\"#6f7a8a\"}}}, "
                                            + "\"category\": \"Major Programs\",\"symbolSize\":6, "
                                            + "\"value\": 1}");   
                } */
                foreach( var story in StoryData.Stories){
                    var lastRev = context.StoryRevision.Where( s => s.StoryId == story.Id ).OrderBy( s => s.Created ).Last();
                    var shortenedStoryTitle = lastRev.Title.Count() > LabelLength ? lastRev.Title.Substring( 0, LabelLength ) + "..." : lastRev.Title;
                    StoryDataForTheGraph.Add(
                                            "\n{ \"name\": \"" + System.Net.WebUtility.HtmlEncode(shortenedStoryTitle) + "\", "
                                            + "\"label\":{\"normal\":{\"show\":true"
                                            +",\"textStyle\":{\"color\":\"#f7cb38\"}}}, "
                                            + "\"category\": \"Success Stories\",\"symbolSize\":15, "
                                            + "\"value\":1}");
                    LinksDataForTheGraph.Add("\n{ \"source\": \"" + System.Net.WebUtility.HtmlEncode(shortenedStoryTitle) + "\",\"target\": \"" + shortenedProgramName +"\"}");
                    var author = context.KersUser.Where( u => u.Id == story.KersUserId).Include( u => u.RprtngProfile).First();
                    /* if( !EmployeeActivities.Where( e => e.User.User == author).Any()){
                        EmployeeDataForTheGraph.Add(
                                "\n{ \"name\": \"" + author.RprtngProfile.Name + "\","
                                + " \"category\": \"Employees\","
                                + "\"label\":{\"normal\":{"
                                + "\"show\":false ,"
                                + "\"textStyle\":{\"color\":\"#72c380\"}}},"
                                + "\"symbolSize\":5,"
                                + "\"value\": 1}");
                    } */
                    LinksDataForTheGraph.Add("\n{ \"source\": \"" + System.Net.WebUtility.HtmlEncode(author.RprtngProfile.Name) + "\",\"target\": \"" + System.Net.WebUtility.HtmlEncode(shortenedStoryTitle) +"\"}");
                }                
            }



            ViewData["GraphCategories"] = "[\"Employees\", \"Major Programs\", \"Success Stories\"]";
            ViewData["GraphData"] = "[" 
                                        + string.Join(",", EmployeeDataForTheGraph.ToArray() ) 
                                        + ","
                                        + string.Join(",", ProgramDataForTheGraph.ToArray() )
                                        + ","
                                        + string.Join(",", StoryDataForTheGraph.ToArray() )
                                        + "]";
            ViewData["GraphLinks"] = "[" + string.Join(",", LinksDataForTheGraph.ToArray() ) + "]";

            
            return View();
        }



        [HttpGet]
        [Route("[action]/{filter?}/{id?}/{fy?}")]
        public async Task<IActionResult> DataByMonthByEmployee(int filter = 1, int id = 0, string fy = "0")
        {
            var MaxEmployeeNum = 15;
            
            // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
            FiscalYear fiscalYear = GetFYByName(fy);
            if(fiscalYear == null){
                return new StatusCodeResult(500);
            }
            ViewData["FiscalYear"] = fiscalYear;
            ViewData["fy"] = fiscalYear.Name;

            var fiscalYearSummaries = await contactRepo.GetPerPeriodSummaries(fiscalYear.Start, fiscalYear.End, filter, id);
            float[] SummariesArray = fiscalYearSummaries.ToArray();

            ViewData["totalHours"] = SummariesArray[0];
            ViewData["totalContacts"] = (int) SummariesArray[1];
            ViewData["totalMultistate"] = SummariesArray[2];
            ViewData["totalActivities"] = (int) SummariesArray[3];


            if( filter == 1 ){
                ViewData["unit"] = context.PlanningUnit.Where( u => u.Id == id ).FirstOrDefault();
                if(ViewData["unit"] == null){
                    return new StatusCodeResult(500);
                }
            }else if( filter == 0 ){
                ViewData["district"] = context.District.Where( u => u.Id == id ).FirstOrDefault();
                if(ViewData["district"] == null){
                    return new StatusCodeResult(500);
                }
            }else if( filter > 4 ){
                return new StatusCodeResult(500);
            }

            var output = new List< List< Kers.Models.Data.PerPersonActivities >>();

            var months = new List<string>();
            var hours = new List<string>();
            var runningDate = new DateTime( fiscalYear.Start.Year, fiscalYear.Start.Month, 28);

            var result = await this.DataByMonth(fiscalYear, filter, 0, id);

            foreach( var MonthResult in result){
                var MonthData = new List<Kers.Models.Data.PerPersonActivities>();
                months.Add( "'" + runningDate.ToString("MM/yyyy") + "'");
                runningDate = runningDate.AddMonths( 1 );
                hours.Add( MonthResult.Sum( s => s.Hours ).ToString());
                foreach( var res in MonthResult ){
                    var ProgramActivities = new Kers.Models.Data.PerPersonActivities();
                    ProgramActivities.Audience = res.Audience;
                    ProgramActivities.Female = res.Female;
                    ProgramActivities.Hours = res.Hours;
                    ProgramActivities.KersUser = await this.context.KersUser
                                                        .Where( u => u.Id == res.GroupId )
                                                        .Include( u => u.PersonalProfile )
                                                        .FirstOrDefaultAsync();
                    ProgramActivities.Male = res.Male;
                    ProgramActivities.Multistate = res.Multistate;
                    MonthData.Add( ProgramActivities );
                }
                output.Add(MonthData);
            }
            var monthsString = "[" + months.Aggregate((i, j) => i  + "," +  j) + "]";

            ViewData["months"] = monthsString;
            ViewData["hours"] = "[" + hours.Aggregate((i, j) => i + "," + j) + "]";

            var returnList = new List<Kers.Models.Data.PerPersonActivities>();

            var currentMonthNum = 0;

            var PersonDataPerMonth = new List<PersonDataPerMonth>();

            foreach( var month in output ){
                foreach( var person in month ){
                    var existingPerson = returnList.Where( r => r.KersUser == person.KersUser).FirstOrDefault();
                    if( existingPerson == null ){

                        var personEntry = new PersonDataPerMonth();
                        personEntry.KersUser = person.KersUser;
                        personEntry.Audience = new List<int>();
                        personEntry.Hours = new List<float>();
                        personEntry.Male = new List<int>();
                        personEntry.Female = new List<int>();
                        personEntry.Multistate = new List<float>();
                        for( var i = 0; i < currentMonthNum; i++ ){
                            personEntry.Audience.Add(0);
                            personEntry.Hours.Add(0);
                            personEntry.Male.Add(0);
                            personEntry.Female.Add( 0 );
                            personEntry.Multistate.Add( 0 );
                        }
                        personEntry.Audience.Add( person.Audience );
                        personEntry.Hours.Add( person.Hours );
                        personEntry.Male.Add( person.Male );
                        personEntry.Female.Add( person.Female );
                        personEntry.Multistate.Add( person.Multistate );

                        PersonDataPerMonth.Add(personEntry);

                        existingPerson = new Kers.Models.Data.PerPersonActivities();
                        existingPerson.Audience = person.Audience;
                        existingPerson.Female = person.Female;
                        existingPerson.Hours = person.Hours;
                        existingPerson.KersUser = person.KersUser;
                        existingPerson.Male = person.Male;
                        existingPerson.Multistate = person.Multistate;
                        existingPerson.OptionNumberValues = person.OptionNumberValues;
                        existingPerson.RaceEthnicityValues = person.RaceEthnicityValues;
                        returnList.Add(existingPerson);

                    }else{
                        existingPerson.Audience += person.Audience;
                        existingPerson.Female += person.Female;
                        existingPerson.Hours += person.Hours;
                        existingPerson.Male += person.Male;
                        existingPerson.Multistate += person.Multistate;
                        var personEntry = PersonDataPerMonth.Where( p => p.KersUser == person.KersUser ).First();
                        personEntry.Audience.Add(person.Audience);
                        personEntry.Female.Add(person.Female);
                        personEntry.Hours.Add(person.Hours);
                        personEntry.Male.Add(person.Male);
                        personEntry.Multistate.Add(person.Multistate);

                    }
                }
                foreach( var inPerson in PersonDataPerMonth ){
                    if(  !month.Where( m => m.KersUser == inPerson.KersUser ).Any()){
                        inPerson.Audience.Add( 0 );
                        inPerson.Male.Add( 0 );
                        inPerson.Female.Add( 0 );
                        inPerson.Hours.Add(0);
                        inPerson.Multistate.Add(0);
                    }
                }
                currentMonthNum++;
            }

            PersonDataPerMonth = PersonDataPerMonth
                                    .Where( p => p.KersUser != null ).ToList();

            ViewData["AllPersonssData"] = PersonDataPerMonth.OrderByDescending( p => p.Audience.Sum(s => s)).ToList();
           
            var PersonsGendersGraphDataList = new List<string>();
            foreach( var thePerson in PersonDataPerMonth ){
                PersonsGendersGraphDataList.Add(" ["+thePerson.Female.Sum(s => s)+", "+thePerson.Male.Sum(s => s)+", \""+thePerson.KersUser.PersonalProfile.FirstName.Replace("\"", "") + " " + thePerson.KersUser.PersonalProfile.LastName.Replace("\"", "") +"\"]");
            }
            ViewData["ProgramsGendersGraphDataList"] = "[" + string.Join(",", PersonsGendersGraphDataList.ToArray() ) + "]";

            PersonDataPerMonth = PersonDataPerMonth
                                    .OrderByDescending( p => p.Audience.Sum(s => s))
                                    .Take(MaxEmployeeNum)
                                    .OrderBy(p => p.KersUser.PersonalProfile.FirstName )
                                    .ToList();

            
            var PersonsListOfStrings = new List<string>();
            var PrersonsHoursGraphDataList = new List<string>();
            var PersonsContactsByProgramSeries = new List<string>();
            var PersonLength = 15;

            var PeopleHours = new List<string>();
            var PeopleContacts = new List<string>();

            foreach(var thePerson in PersonDataPerMonth ){
                var name = thePerson.KersUser.PersonalProfile.FirstName.Replace("\"", "") + " " + thePerson.KersUser.PersonalProfile.LastName.Replace("\"", "");
                var shortenedPersonName = (name.Length > PersonLength ? name.Substring(0, PersonLength) + "..." : name);
                PersonsListOfStrings.Add( "\"" + shortenedPersonName + "\"" );
                
                PersonsContactsByProgramSeries.Add("{ name: \""+shortenedPersonName+"\", type: \"bar\", data: [" + string.Join(",", thePerson.Audience.Select(n => n.ToString()).ToArray())+"],}");
                PrersonsHoursGraphDataList.Add("{ name: \""+shortenedPersonName+"\", type: \"line\", smooth: !0, itemStyle: { normal: { areaStyle: { type: \"default\" } } }, data: [" + string.Join(",", thePerson.Hours.Select(n => n.ToString()).ToArray())+"]  }");
                


                PeopleHours.Add("{value:" + thePerson.Hours.Sum( p => p ).ToString() + ", name:\"" + shortenedPersonName + "\"}");
                PeopleContacts.Add( "{value:" + thePerson.Audience.Sum( p => p).ToString()  + ", name:\"" + shortenedPersonName + "\"}");
            
            }




            ViewData["hours"] = string.Join(",", PeopleHours.ToArray());
            ViewData["contacts"] = string.Join(",", PeopleContacts.ToArray());
            ViewData["people"] = "[" + string.Join(",", PersonsListOfStrings.ToArray() ) + "]";
            ViewData["PrersonsHoursGraphDataList"] = "[" + string.Join(",", PrersonsHoursGraphDataList.ToArray() ) + "]";
            ViewData["PersonsContactsByProgramSeries"] = "[" + string.Join(",", PersonsContactsByProgramSeries.ToArray() ) + "]";
            ViewData["PersonDataPerMonth"] = PersonDataPerMonth;

            var table = await contactRepo.DataByEmployee(fiscalYear, 1, id);

            return View(table);
        }




        


        [HttpGet]
        [Route("[action]/{fy?}")]
        public IActionResult  Districts(string fy = "0")
        {

            var districts =  this.context.District.ToList();
            ViewData["fy"] = fy;
            return View(districts);
        }
        [HttpGet]
        [Route("[action]/{fy?}")]
        public async Task<IActionResult> County(string fy = "0")
        {
            var counties = await this.context.PlanningUnit
                                    .Where( u => 
                                                u.District != null
                                                &&
                                                u.Name.Substring( u.Name.Length - 3) == "CES"
                                           )
                                    .OrderBy( u => u.Name)
                                    .ToListAsync();
            ViewData["fy"] = fy;
            return View(counties);
        }
        [HttpGet]
        [Route("[action]/{fy?}")]
        public async Task<IActionResult> Units(string fy = "0")
        {
            var units = await this.context.PlanningUnit
                                    .Where( u => 
                                                u.District == null
                                                &&
                                                u.Name.Substring( u.Name.Length - 3) != "CES"
                                           )
                                    .OrderBy( u => u.Name)
                                    .ToListAsync();
            ViewData["fy"] = fy;
            return View(units);
        }
        [HttpGet]
        [Route("[action]/{fy?}")]
        public IActionResult Ksu(string fy = "0")
        {
            ViewData["fy"] = fy;
            return View();
        }


        /***********************************************************
        
        
        Following actions are for generating reports for this request





        Hi Ken, 
 
        I am working on the 2019 BSF Profiles and in the past you have provided Cherry K with a data file which included Program Indicators as well 
        as Statistical Contacts for FCS agents and paraprofessionals. 
        Would you be able to provide me with that same information. I included what was provided to me last year for your review. 
        
        Thank you!!!
        Mrs. Maria Harris, M.Ed
        Extension Associate for Building Strong Families
        Family and Consumer Sciences (FCS)
        University of Kentucky College of Agriculture, Food and Environment
        Scovell Hall RM 243
        Lexington, KY 40546-0064
        Phone: (859) 218-1547
        Fax: (859) 257-3095
        
         */





        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Person()
        {
            FiscalYear fiscalYear = GetFYByName("2018");
            ViewData["fy"] = fiscalYear.Name;
            //Get All Data for the fy by employee
            var result = contactRepo.GetActivitiesAndContactsAsync(fiscalYear.Start, fiscalYear.End,4);

            List<PerPersonActivities> userResult = new List<PerPersonActivities>();
            foreach( var res in await result ){

                var user = await context.KersUser.Where( u => u.Id == res.GroupId)
                                    .Include( u => u.Specialties ).ThenInclude( s => s.Specialty)
                                    .FirstOrDefaultAsync();
                if( user != null && user.Specialties.Where( s => s.Specialty.Code == "progFCS").FirstOrDefault() != null){
                    var personGroup = new PerPersonActivities();
                    personGroup.Audience = res.Audience;
                    personGroup.Female = res.Female;
                    personGroup.Male = res.Male;
                    personGroup.Hours = res.Hours;
                    personGroup.Multistate = res.Multistate;
                    personGroup.OptionNumberValues = res.OptionNumberValues;
                    personGroup.RaceEthnicityValues = res.RaceEthnicityValues;
                    personGroup.KersUser = await context.KersUser.Where( u => u.Id == res.GroupId)
                                                    .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit )
                                                    .Include( u => u.RprtngProfile).ThenInclude( r => r.Institution)
                                                    .Include( u => u.Specialties )
                                                    .Include( u => u.PersonalProfile)
                                                    .Include( u => u.ExtensionPosition)
                                                    .FirstOrDefaultAsync();
                    userResult.Add(personGroup);
                }

                
            }
            userResult = userResult.OrderBy( r => r.KersUser.RprtngProfile.PlanningUnit.order).ThenBy(r => r.KersUser.PersonalProfile.FirstName).ToList();
            var table = new TableViewModel();


            table.Header = new List<string>{
                            "FY", "Institution", "Planning Unit", "PersonID", "Employee", "PositionID"
                        };
            var Specialties = this.context.Specialty.OrderBy( s => s.Code );

            var Races = this.context.Race.OrderBy(r => r.Order);
            var Ethnicities = this.context.Ethnicity.OrderBy( e => e.Order);
            var OptionNumbers = this.context.ActivityOptionNumber.OrderBy( n => n.Order);


            foreach( var Specialty in Specialties ){
                table.Header.Add(Specialty.Code);
            }

            table.Header.Add("Days");
            table.Header.Add("Multistate");
            table.Header.Add("Total Contacts");

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

            var Rows = new List<List<string>>();

            foreach(var res in userResult){
                
                var Row = new List<string>();
                Row.Add(fiscalYear.Name);
                Row.Add( res.KersUser.RprtngProfile.Institution.Code);
                Row.Add(res.KersUser.RprtngProfile.PlanningUnit.Name);
                Row.Add( res.KersUser.RprtngProfile.PersonId);
                Row.Add( res.KersUser.PersonalProfile.FirstName + " " + res.KersUser.PersonalProfile.LastName);
                Row.Add( res.KersUser.ExtensionPosition.Code);
                foreach( var Specialty in Specialties ){
                    if( res.KersUser.Specialties.Where( s => s.SpecialtyId == Specialty.Id).Any()){
                        Row.Add("1");
                    }else{
                        Row.Add("0");
                    }
                }
                Row.Add((res.Hours / 8).ToString());
                Row.Add((res.Multistate / 8).ToString());
                Row.Add(res.Audience.ToString());
                foreach( var race in Races){
                    var raceAmount = res.RaceEthnicityValues.Where( v => v.RaceId == race.Id).Sum( r => r.Amount);
                    Row.Add(raceAmount.ToString());
                    
                }

                foreach( var et in Ethnicities){
                    var ethnAmount = res.RaceEthnicityValues.Where( v => v.EthnicityId == et.Id).Sum( r => r.Amount);
                    Row.Add(ethnAmount.ToString());
                    
                }
                Row.Add(res.Male.ToString());
                Row.Add(res.Female.ToString());

                foreach( var opnmb in OptionNumbers){
                    var optNmbAmount = res.OptionNumberValues.Where( o => o.ActivityOptionNumberId == opnmb.Id).Sum( s => s.Value);
                    Row.Add( optNmbAmount.ToString());
                }
                Rows.Add(Row);
            }
            table.Rows = Rows;
            table.Foother = new List<string>();

            return View(table);
        }



        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Indicators()
        {
            FiscalYear fiscalYear = GetFYByName("2018");
            //Get All Data for the fy by employee
            


            var IndicatorValuesPerPerson = context.ProgramIndicatorValue
                                        .Where( i => 
                                                    i.ProgramIndicator.MajorProgram.StrategicInitiative.FiscalYear.Id == fiscalYear.Id
                                                    &&
                                                    i.KersUser.Specialties.Where( s => s.Specialty.Code == "progFCS").Any() 
                                                    
                                                    )
                                        .GroupBy(
                                            u => u.KersUser
                                        )
                                        .Select( i => new PersonIndicator{
                                                KersUser = i.Key,
                                                Indicators = i.Select(s => s).ToList()
                                            }
                                        )
                                        .ToListAsync();



            var Indicators = await context.StrategicInitiative
                                .Where( i => i.FiscalYear == fiscalYear)
                                .Include( i => i.ProgramCategory)
                                .Include( i => i.MajorPrograms ).ThenInclude( m => m.ProgramIndicators )
                                .OrderBy( i => i.order)
                                .ToListAsync();




            var table = new TableViewModel();


            table.Header = new List<string>{
                            "FY", "Institution", "Planning Unit", "PersonID", "Employee", "PositionID"
                        };
            var Specialties = this.context.Specialty.OrderBy( s => s.Code );

            
            foreach( var Specialty in Specialties ){
                table.Header.Add(Specialty.Code);
            }


            foreach( var Initiative in Indicators ){
                foreach( var Program in Initiative.MajorPrograms ){
                    var i = 1;
                    foreach( var Indicator in Program.ProgramIndicators ){
                        table.Header.Add( Initiative.ProgramCategory.ShortName + "_" + Program.PacCode + "_" + i.ToString() );
                        i++;
                    }
                }
            }

            

            var Rows = new List<List<string>>();

            foreach(var res in await IndicatorValuesPerPerson){

                var user = await context.KersUser.Where( i => i.Id == res.KersUser.Id)
                            .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit )
                            .Include( u => u.RprtngProfile).ThenInclude( r => r.Institution)
                            .Include( u => u.Specialties )
                            .Include( u => u.PersonalProfile)
                            .Include( u => u.ExtensionPosition)
                            .FirstOrDefaultAsync();
                
                var Row = new List<string>();
                Row.Add(fiscalYear.Name);
                Row.Add( user.RprtngProfile.Institution.Code);
                Row.Add(user.RprtngProfile.PlanningUnit.Name);
                Row.Add( user.RprtngProfile.PersonId);
                Row.Add( user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName);
                Row.Add( user.ExtensionPosition.Code);
                foreach( var Specialty in Specialties ){
                    if( user.Specialties.Where( s => s.SpecialtyId == Specialty.Id).Any()){
                        Row.Add("1");
                    }else{
                        Row.Add("0");
                    }
                }


                foreach( var Initiative in Indicators ){
                    foreach( var Program in Initiative.MajorPrograms ){
                        foreach( var Indicator in Program.ProgramIndicators ){
                            Row.Add( res.Indicators.Where( i => i.ProgramIndicatorId == Indicator.Id).Sum( s => s.Value).ToString());
                        }
                    }
                }


                
                Rows.Add(Row);
            }
            table.Rows = Rows;
            table.Foother = new List<string>();
            ViewData["fy"] = fiscalYear.Name;
            return View(table);
        }


        private async Task<List<List<PerGroupSummary>>> DataByMonth(FiscalYear fiscalYear, int filter = 1, int grouppedBy = 1, int id = 0)
        {

            var result = new List<List<PerGroupSummary>>();

 
            var cacheKey = CacheKeys.ReportsDataByMonth + filter.ToString() + id.ToString() + "_" + grouppedBy.ToString() + "_" + fiscalYear.Name;
            var cachedTypes = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedTypes)){
                result = JsonConvert.DeserializeObject<List<List<PerGroupSummary>>>(cachedTypes);
            }else{

                for (DateTime dt = fiscalYear.Start; dt <= fiscalYear.End; dt = dt.AddMonths(1))
                {
                    /*****************************************************************/
                    // Generate Contacts Reports Groupped by Employee or Major Program
                    // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
                    // grouppedBy: 0 Employee, 1 MajorProgram
                    /*******************************************************************/
                    var first = new DateTime( dt.Year, dt.Month, 1, 0, 0, 0);
                    var last = new DateTime( dt.Year, dt.Month , DateTime.DaysInMonth(dt.Year, dt.Month), 23, 59, 59 );

                    List<PerGroupSummary> activities = await contactRepo.GetActivitiesAndContactsSummaryAsync( first, last, filter, grouppedBy, id );
                    result.Add( activities );
                }    
                _cache.SetString(cacheKey, JsonConvert.SerializeObject(result), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( this.getCacheSpan(fiscalYear) )
                    });      
            } 
            return result;
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

        protected int getCacheSpan(FiscalYear fiscalYear){
            int cacheDaysSpan = 350;
            var today = DateTime.Now;
            if(fiscalYear.Start < today && Math.Max( fiscalYear.End.Ticks, fiscalYear.ExtendedTo.Ticks) > today.Ticks){
                cacheDaysSpan = 3;
            }
            return cacheDaysSpan;
        }



    }

    public class PersonIndicator{
        public KersUser KersUser;
        public List<ProgramIndicatorValue> Indicators;
    }

    public class ProgramDataPerMonth{
        public MajorProgram MajorProgram;
        public List<float> Hours;
        public List<int> Audience;
        public List<int> Male;
        public List<int> Female;
        public List<float> Multistate;

    }

    public class PersonDataPerMonth{
        public KersUser KersUser;
        public List<float> Hours;
        public List<int> Audience;
        public List<int> Male;
        public List<int> Female;
        public List<float> Multistate;

    }


}