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
    public class SnapAgentCommunityEventDetailTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapAgentCommunityEventDetailTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "12 1 * * *";
        
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
                    var str = repo.AgentCommunityEventDetail(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapAgentCommunityEventDetailTask", str, 
                                    "SnapAgent Community Event Detail Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapAgentCommunityEventDetailTask", e, 
                                    "SnapAgent Community Event Detail Task failed"
                            );
                }
                
                
            }

            
        }
    }

}