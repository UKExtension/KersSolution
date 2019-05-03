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
using Microsoft.AspNetCore.Hosting;

namespace Kers.Tasks
{
    public class MessageProcessingTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        IHostingEnvironment environment;
        public MessageProcessingTask(
            IServiceProvider serviceProvider,
            IHostingEnvironment environment
        ){
            this.serviceProvider = serviceProvider;
            this.environment = environment;
        }
        public string Schedule => "* * * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                var repo = new MessageRepository(context);
                try{
                    var startTime = DateTime.Now;
                    
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                    repo.ProcessMessageQueue( _configuration, environment );







/* 
                    using (var contexReporting = new KersReportingContext(optionsBuilder.Options))
                    {
                        trainings = contexReporting.vInServiceQualtricsSurveysToCreate.ToList();

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
                                data = result.Content.ReadAsStringAsync().Result;
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
                    }
 */
                    var endTime = DateTime.Now;
                    
                    
                    
/*                     
                    await LogComplete(context, 
                                    "InServiceQualtricsTask", data, 
                                    "InService Qualtrics Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );

 */



                }catch( Exception e){
                    await LogError(context, 
                                    "InServiceQualtricsTask", e, 
                                    "InService Qualtrics Task failed"
                            );
                }   
            }
        }
    }


}