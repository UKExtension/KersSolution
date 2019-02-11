using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Kers.Models.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kers.Tasks
{
    public class SnapPartnersOfACountyTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapPartnersOfACountyTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "52 23 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var repo = new SnapPolicyRepository(context, cache);
                    var startTime = DateTime.Now;
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    var tables = new List<string>();

                    // Planning Units
                    var units = context.PlanningUnit;
                    foreach( var unit in units){
                        var tbl = repo.PartnersOfACounty(unit.Id, fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog), true);
                        if(RndInt == 2){
                            tbl = repo.PartnersOfACounty(unit.Id, fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog), true);
                        }
                        tables.Add(tbl);
                    }
                    

                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapPartnersOfACountyTask", tables, 
                                    "Snap Partners Of a County Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapPartnersOfACountyTask", e, 
                                    "Snap Partners Of a County Task failed"
                            );
                }
                
                
            }

            
        }
    }

}