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
        public string Schedule => "52 8 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    
                   
                    var startTime = DateTime.Now;
                    
                    var tables = new List<vInServiceQualtricsSurveysToCreate>();
                    var data = "";
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                    var optionsBuilder = new DbContextOptionsBuilder<KersReportingContext>();
                    optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:connKersReporting"]);

                    using (var contexReportingt = new KersReportingContext(optionsBuilder.Options))
                    {
                        tables = contexReportingt.vInServiceQualtricsSurveysToCreate.ToList();


                        var qualtricsApiHost = _configuration["QualtricsApi:sApiHost"];
                        var qualtricsUser = _configuration["QualtricsApi:sUser"];
                        var qualtricsToken = _configuration["QualtricsApi:sToken"];
                        var qualtricsFormat = _configuration["QualtricsApi:sFormat"];
                        var qualtricsVersion = _configuration["QualtricsApi:sVersion"];
                        var qualtricsImportFormat = _configuration["QualtricsApi:sImportFormat"];
                        var qualtricsActivate = _configuration["QualtricsApi:sActivate"];
                        
                        var uri = qualtricsApiHost + 
                        "Request=getSurveys&User=" + HttpUtility.UrlEncode(qualtricsUser) + 
                        "&Token=" + qualtricsToken + 
                        "&Format=" + qualtricsFormat + 
                        "&Version=" + qualtricsVersion;
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Accept.Clear();
                        

                        var result = client.GetAsync(uri).Result;
                        data = result.Content.ReadAsStringAsync().Result;


                        
                    }

                    
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