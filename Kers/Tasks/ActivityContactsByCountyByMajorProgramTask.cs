using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kers.Tasks
{
    public class ActivityContactsByCountyByMajorProgramTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public ActivityContactsByCountyByMajorProgramTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "42 2 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var repo = new ActivityRepository(cache, context);
                    var contactRepo = new ContactRepository(cache, context);
                    
                    var counties = context.PlanningUnit.Where( c => c.Name.Substring(c.Name.Count() - 3) == "CES");
                    var currnetfiscalYear = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
                    var startTime = DateTime.Now;

                    var str = new List<PlanningUnit>();
                    //Rebuild summary cache
                    Random rnd = new Random();
                    foreach( var county in counties){
                        int RndInt = rnd.Next(1, 3);
                        if(RndInt == 1){
                            var summary = await contactRepo.GetPerPeriodSummaries(currnetfiscalYear.Start, currnetfiscalYear.End,1,county.Id,true);
                            str.Add(county);
                        }
                    }
                    int r = rnd.Next(1, 10);
                    if(r == 1){
                        var s = await contactRepo.GetPerPeriodSummaries(currnetfiscalYear.Start, currnetfiscalYear.End,4,0,true);
                    } 
                    
                    //r = rnd.Next(1, 100);
                    //var str = await repo.ContactsByCountyByMajorProgram(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog), true);
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "Planning Unit", str, 
                                    "Per Period Summaries Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "ActivityContactsByCountyByMajorProgramTask", e, 
                                    "Per Period Summaries Task failed"
                            );
                }
                
                
            }

            
        }
    }

}