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
    public class SnapIndividualContactTotalsTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapIndividualContactTotalsTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "11 1 * * *";
        
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
                    var mainContext = scope.ServiceProvider.GetService<KERSmainContext>();
                    var repo = new SnapDirectRepository(context, cache, mainContext, memoryCache);
                    var startTime = DateTime.Now;
                    var str = repo.IndividualContactTotals(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    if( RndInt == 2 ){
                        str = repo.IndividualContactTotals(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.SnapEd), true);
                    }
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapIndividualContactTotalsTask", str, 
                                    "Snap Individual Contact Totals Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapIndividualContactTotalsTask", e, 
                                    "Snap Individual Contact Totals Task failed"
                            );
                }
                
                
            }

            
        }
    }

}