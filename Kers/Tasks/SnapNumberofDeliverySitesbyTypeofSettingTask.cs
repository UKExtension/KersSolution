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
    public class SnapNumberofDeliverySitesbyTypeofSettingTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapNumberofDeliverySitesbyTypeofSettingTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "2 1 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var memoryCache = scope.ServiceProvider.GetService<IMemoryCache>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var repo = new SnapPolicyRepository(context, cache, memoryCache);
                    var startTime = DateTime.Now;
                    var str = repo.AgentCommunityEventDetail(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    if( RndInt == 2 ){
                        str = repo.AgentCommunityEventDetail(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.SnapEd), true);
                    }
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapNumberofDeliverySitesbyTypeofSettingTask", str, 
                                    "Snap Numberof Delivery Sites by Type of Setting Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapNumberofDeliverySitesbyTypeofSettingTask", e, 
                                    "Snap Numberof Delivery Sites by Type of Setting Task failed"
                            );
                }
                
                
            }

            
        }
    }

}