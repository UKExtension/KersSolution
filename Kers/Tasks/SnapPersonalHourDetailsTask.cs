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
    public class SnapPersonalHourDetailsTask : TaskBase, IScheduledTask
    {
        //KERScoreContext context;
        IServiceProvider serviceProvider;
        public SnapPersonalHourDetailsTask(
            //KERScoreContext context
            IServiceProvider serviceProvider
        ){
            //this.context = context;
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "58 0 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                var fiscalYearRepo = new FiscalYearRepository( context );
                var fiscalYear = fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd, true);
                try{
                    var mainContext = scope.ServiceProvider.GetService<KERSmainContext>();
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var memoryCache = scope.ServiceProvider.GetService<IMemoryCache>();
                    var repo = new SnapDirectRepository(context, cache, mainContext, memoryCache);
                    var startTime = DateTime.Now;
                    var str = repo.PersonalHourDetails(fiscalYear, true);
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    if( RndInt == 2 ){
                        str = repo.PersonalHourDetails(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.SnapEd, true), true);
                    }
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapPersonalHourDetailsTask", str, 
                                    "Snap Personal Hour Details (FY"+fiscalYear.Name+") Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapPersonalHourDetailsTask", e, 
                                    "Snap Personal Hour Details (FY"+fiscalYear.Name+") Task failed"
                            );
                }
                
            }

            
        }
    }

}