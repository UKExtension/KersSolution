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
    public class SnapEstimatedSizeofAudiencesReachedTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapEstimatedSizeofAudiencesReachedTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "14 1 * * *";
        
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
                    var str = repo.EstimatedSizeofAudiencesReached(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapEstimatedSizeofAudiencesReachedTask", str, 
                                    "Snap Estimated Size of Audiences Reached Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapEstimatedSizeofAudiencesReachedTask", e, 
                                    "Snap Estimated Size of Audiences Reached Task failed"
                            );
                }
                
                
            }

            
        }
    }

}