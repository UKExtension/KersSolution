using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Kers.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace Kers.Tasks
{
    public class InServiceQualtricsTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public InServiceQualtricsTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "37 17 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var startTime = DateTime.Now;
                    
                    var trainings = new List<vInServiceQualtricsSurveysToCreate>();
                    var data = "";
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();



                    var trnngs = context.Training
                                .Where( t => t.qualtricsSurveyID == null
                                                &&
                                                t.Start > new DateTime(2017,01,01,0, 0, 0, 0)
                                                &&
                                                t.Start < DateTimeOffset.Now
                                                &&
                                                t.tStatus == "A"
                                                )
                                .Include( t => t.submittedBy).ThenInclude( s => s.RprtngProfile);


/* 

                    var optionsBuilder = new DbContextOptionsBuilder<KersReportingContext>();
                    optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:connKersReporting"]);

                    using (var contexReporting = new KersReportingContext(optionsBuilder.Options))
                    {
                        trainings = contexReporting.vInServiceQualtricsSurveysToCreate.ToList();
 */
                        var qualtricsApiHost = _configuration["QualtricsApi:sApiHost"];
                        var qualtricsUser = _configuration["QualtricsApi:sUser"];
                        var qualtricsToken = _configuration["QualtricsApi:sToken"];
                        var qualtricsFormat = _configuration["QualtricsApi:sFormat"];
                        var qualtricsVersion = _configuration["QualtricsApi:sVersion"];
                        var qualtricsImportFormat = _configuration["QualtricsApi:sImportFormat"];
                        var qualtricsActivate = _configuration["QualtricsApi:sActivate"];
                        var client = new HttpClient();
                        
                        foreach( var training in trnngs ){

                            string sSurveyURL = "https://kers.ca.uky.edu/core/reports/Data/qltrx?title=" +
                                                    HttpUtility.UrlEncode(training.Subject) +
                                                    "&id=" + training.Id +
                                                    "&dates=" + HttpUtility.UrlEncode(training.Start.ToString("MM/dd/yyyy") +
                                                    (training.End != null ? " - " +training.End?.ToString("MM/dd/yyyy") : "")); //title/id/dates
                            //"https://kers.ca.uky.edu/CES/rpt/zQualtricsInServiceEvaluationSurveyText.aspx?t=" + training.Id;
 
                            String url = qualtricsApiHost
                            + "Request=importSurvey"
                            + "&User=" + HttpUtility.UrlEncode(qualtricsUser)
                            + "&Token=" + qualtricsToken
                            + "&Format=" + qualtricsFormat
                            + "&Version=" + qualtricsVersion
                            + "&ImportFormat=" + qualtricsImportFormat
                            + "&Activate=" + qualtricsActivate
                            + "&Name=" + HttpUtility.UrlEncode(training.Start.ToString("yyyyMMdd")+" ["+training.submittedBy.RprtngProfile.Name+"] "+training.Subject)
                            + "&URL=" + HttpUtility.UrlEncode(sSurveyURL);
                            
                            try
                            {
                                client.DefaultRequestHeaders.Accept.Clear();
                                var result = client.GetAsync(url).Result;
                                data = result.Content.ReadAsStringAsync().Result;
                                XDocument xmlDoc = new XDocument();
                                    try
                                    {
                                        xmlDoc = XDocument.Parse(data);
                                        String surveyID = xmlDoc.Root.Element("Result").Value;
                                        training.qualtricsSurveyID = surveyID;

                                    }
                                    catch (Exception e)
                                    {
                                        await LogError(context, 
                                                "InServiceQualtricsTask", e, 
                                                "InService Qualtrics Task failed"
                                            ); 
                                    }
                            }
                            catch (WebException e)
                            {
                                await LogError(context, 
                                    "InServiceQualtricsTask", e, 
                                    "InService Qualtrics Task failed"
                                );    
                            } 
                        }
                   // }
                    context.SaveChanges();
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "InServiceQualtricsTask", data, 
                                    "InService Qualtrics Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "InServiceQualtricsTask", e, 
                                    "InService Qualtrics Task failed"
                            );
                }   
            }
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

