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
    // uses https://theysaidso.com/api/
    public class SnapSitesPerPersonPerMonthTask : TaskBase, IScheduledTask
    {
        //KERScoreContext context;
        IServiceProvider serviceProvider;
        public SnapSitesPerPersonPerMonthTask(
            //KERScoreContext context
            IServiceProvider serviceProvider
        ){
            //this.context = context;
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "58 1 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var mainContext = scope.ServiceProvider.GetService<KERSmainContext>();
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var memoryCache = scope.ServiceProvider.GetService<IMemoryCache>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var repo = new SnapDirectRepository(context, cache, mainContext, memoryCache);
                    var startTime = DateTime.Now;
                    var str = repo.SitesPerPersonPerMonth(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd, true), true);
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    if( RndInt == 2 ){
                        str = repo.SitesPerPersonPerMonth(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.SnapEd), true);
                    }
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapSitesPerPersonPerMonthTask", str, 
                                    "Snap Sites Per Person Per Month Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapSitesPerPersonPerMonthTask", e, 
                                    "Snap Sites Per Person Per Month Task failed"
                            );
                }
                
            }

            
        }
    }

}