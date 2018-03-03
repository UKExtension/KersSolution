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
    public class SnapCopiesSummarybyCountyNotAgentsTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapCopiesSummarybyCountyNotAgentsTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "12 0 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var repo = new SnapFinancesRepository(context, cache);
                    var startTime = DateTime.Now;
                    var str = repo.CopiesSummarybyCountyNotAgents(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapCopiesSummarybyCountyNotAgentsTask", str, 
                                    "Snap Copies Summary by County Not Agents Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapCopiesSummarybyCountyAgentsTask", e, 
                                    "Snap Copies Summary by County Not Agents Task failed"
                            );
                }
                
                
            }

            
        }
    }

}