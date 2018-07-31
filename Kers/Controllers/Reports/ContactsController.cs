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
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult State()
        {
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
            var table = await activityRepo.ContactsByCountyByMajorProgram(fiscalYear);
            return View(table);
        }




        


        [HttpGet]
        [Route("[action]")]
        public IActionResult  Districts()
        {

            var districts =  this.context.District.ToList();

            return View(districts);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> County()
        {
            var counties = await this.context.PlanningUnit
                                    .Where( u => 
                                                u.District != null
                                                &&
                                                u.Name.Substring( u.Name.Length - 3) == "CES"
                                           )
                                    .OrderBy( u => u.Name)
                                    .ToListAsync();
            return View(counties);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Units()
        {
            var units = await this.context.PlanningUnit
                                    .Where( u => 
                                                u.District == null
                                                &&
                                                u.Name.Substring( u.Name.Length - 3) != "CES"
                                           )
                                    .OrderBy( u => u.Name)
                                    .ToListAsync();
            return View(units);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Ksu()
        {
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
            //Get All Data for the fy by employee
            var result = contactRepo.GetActivitiesAndContactsAsync(fiscalYear,4);

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


    public class PersonIndicator{
        public KersUser KersUser;
        public List<ProgramIndicatorValue> Indicators;
    }


}