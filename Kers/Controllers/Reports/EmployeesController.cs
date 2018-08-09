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
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class EmployeesController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepository;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        private FiscalYear currentFiscalYear;

        private IConfiguration _configuration;

        string[] types = new string[]{ "District Reports", "Planning Unit Report", "KSU" };
        public EmployeesController( 
                    KERScoreContext context,
                    IFiscalYearRepository fiscalYearRepository,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo,
                    IConfiguration Configuration
            ){
           this.context = context;
           this.fiscalYearRepository = fiscalYearRepository;
           this.currentFiscalYear = this.fiscalYearRepository.currentFiscalYear("serviceLog");
           this._cache = _cache;
           this.activityRepo = activityRepo;
           this.contactRepo = contactRepo;
           _configuration = Configuration;
        }


        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            /* 
            var optionsBuilder = new DbContextOptionsBuilder<KersReportingContext>();
            optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:connKersReporting"]);

            using (var contexReporting = new KersReportingContext(optionsBuilder.Options))
                    {
                        var trainings = contexReporting.vInServiceQualtricsSurveysToCreate.ToList();

                        var qualtricsApiHost = _configuration["QualtricsApi:sApiHost"];
                        var qualtricsUser = _configuration["QualtricsApi:sUser"];
                        var qualtricsToken = _configuration["QualtricsApi:sToken"];
                        var qualtricsFormat = _configuration["QualtricsApi:sFormat"];
                        var qualtricsVersion = _configuration["QualtricsApi:sVersion"];
                        var qualtricsImportFormat = _configuration["QualtricsApi:sImportFormat"];
                        var qualtricsActivate = _configuration["QualtricsApi:sActivate"];
                        var client = new HttpClient();
                        
                        foreach( var training in trainings ){

                            string sSurveyURL = "https://kers.ca.uky.edu/CES/rpt/zQualtricsInServiceEvaluationSurveyText.aspx?t=" + training.tID;

                            String url = qualtricsApiHost
                            + "Request=importSurvey"
                            + "&User=" + HttpUtility.UrlEncode(qualtricsUser)
                            + "&Token=" + qualtricsToken
                            + "&Format=" + qualtricsFormat
                            + "&Version=" + qualtricsVersion
                            + "&ImportFormat=" + qualtricsImportFormat
                            + "&Activate=" + qualtricsActivate
                            + "&Name=" + HttpUtility.UrlEncode(training.qualtricsTitle)
                            + "&URL=" + HttpUtility.UrlEncode(sSurveyURL);
                            
                            try
                            {
                                client.DefaultRequestHeaders.Accept.Clear();
                                var result = client.GetAsync(url).Result;
                                var data = result.Content.ReadAsStringAsync().Result;
                                XDocument xmlDoc = new XDocument();
                                    try
                                    {
                                        xmlDoc = XDocument.Parse(data);
                                        String surveyID = xmlDoc.Root.Element("Result").Value;
                                        var commandText = "UPDATE [UKCA_Reporting]..[zInServiceTrainingCatalog] SET qualtricsSurveyID = @p1 WHERE rID = @p2";;
                                        var surveyParameter = new SqlParameter("@p1", surveyID);
                                        var trainingParameter = new SqlParameter("@p2", training.tID);
                                        contexReporting.Database.ExecuteSqlCommand(commandText, parameters: new[] {
                                                                                                        surveyParameter, trainingParameter
                                                                                    });
                                    }
                                    catch (Exception e)
                                    {
                                        
                                    }
                            }
                            catch (WebException e)
                            {
                                  
                            }
                        }
                    }
            
 */
            return View();
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


    class KersReportingContext:DbContext{
        public KersReportingContext(DbContextOptions<KersReportingContext> options)
        : base(options)
        { }

        public virtual DbSet<vInServiceQualtricsSurveysToCreate> vInServiceQualtricsSurveysToCreate { get; set; }
    }

    public partial class vInServiceQualtricsSurveysToCreate
    {
        [Key]
        public int rID {get;set;}
        public string tID {get; set;}
        public string trainDateBegin {get;set;}
        public string  tTitle {get;set;}
        public string qualtricsTitle {get;set;}
    }




}