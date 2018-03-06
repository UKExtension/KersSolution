using System;
using System.Collections.Generic;
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
    public class SnapSessionTypebyMonthTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapSessionTypebyMonthTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "24 1 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var mainContext = scope.ServiceProvider.GetService<KERSmainContext>();
                    var repo = new SnapDirectRepository(context, cache, mainContext);
                    var startTime = DateTime.Now;
                    var str = repo.SessionTypebyMonth(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapSessionTypebyMonthTask", str, 
                                    "Snap Session Type by Month Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapSessionTypebyMonthTask", e, 
                                    "Snap Session Type by Month Task failed"
                            );
                }
                
                
            }

            
        }
    }

}