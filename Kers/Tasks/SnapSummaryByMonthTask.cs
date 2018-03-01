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
    public class SnapSummaryByMonthTask : TaskBase, IScheduledTask
    {
        //KERScoreContext context;
        IServiceProvider serviceProvider;
        public SnapSummaryByMonthTask(
            //KERScoreContext context
            IServiceProvider serviceProvider
        ){
            //this.context = context;
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "18 0 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                var mainContext = scope.ServiceProvider.GetService<KERSmainContext>();
                var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                var fiscalYearRepo = new FiscalYearRepository( context );
                var repo = new SnapDirectRepository(context, cache, mainContext);
                var startTime = DateTime.Now;
                var str = repo.TotalByMonth(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                var endTime = DateTime.Now;
                await LogComplete(context, 
                                    "SnapSummaryByMonthTask", str, 
                                    "Snap Summary By Month Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
            }

            
        }
    }

}