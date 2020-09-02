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
    public class SnapReimbursementCountyTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapReimbursementCountyTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "45 1 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var memoryCache = scope.ServiceProvider.GetService<IMemoryCache>();
                    var mainContext = scope.ServiceProvider.GetService<KERSmainContext>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var repo = new SnapFinancesRepository(context, cache, memoryCache);
                    var startTime = DateTime.Now;
                    var str = repo.ReimbursementCounty(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    if( RndInt == 2 ){
                        str = repo.ReimbursementCounty(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.SnapEd), true);
                    }
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "ReimbursementCountyTask", str, 
                                    "Reimbursement County Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "ReimbursementCountyTask", e, 
                                    "Reimbursement County Task failed"
                            );
                }
                
                
            }

            
        }
    }

}