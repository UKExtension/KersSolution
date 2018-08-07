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
        public string Schedule => "52 5 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    
                   
                    var startTime = DateTime.Now;
                    
                    var tables = new List<vInServiceQualtricsSurveysToCreate>();
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                    var optionsBuilder = new DbContextOptionsBuilder<KersReportingContext>();
                    optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:connKersReporting"]);

                    using (var contexReportingt = new KersReportingContext(optionsBuilder.Options))
                    {
                        tables = contexReportingt.vInServiceQualtricsSurveysToCreate.ToList();
                        
                    }

                    
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "InServiceQualtricsTask", tables, 
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