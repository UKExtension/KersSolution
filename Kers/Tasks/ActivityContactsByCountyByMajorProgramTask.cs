﻿using System;
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
                    var startTime = DateTime.Now;
                    var str = await repo.ContactsByCountyByMajorProgram(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog), true);
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "TableViewModel", str, 
                                    "Activity Contacts By County By Major Program Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "ActivityContactsByCountyByMajorProgramTask", e, 
                                    "Activity Contacts By County By Major Program Task failed"
                            );
                }
                
                
            }

            
        }
    }

}