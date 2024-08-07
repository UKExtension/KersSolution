﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Kers.Models.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kers.Tasks
{
    public class SnapSpecificSiteNameDetailsTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public SnapSpecificSiteNameDetailsTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "22 23 * * *";
        
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
                    var repo = new SnapDirectRepository(context, cache, mainContext, memoryCache);
                    var startTime = DateTime.Now;
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    var tables = new List<string>();
                    var tbl = repo.SpecificSiteNamesDetails(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
                    if(RndInt == 2){
                        tbl = repo.SpecificSiteNamesDetails(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.SnapEd), true);
                    }
                    tables.Add(tbl);
                    
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "SnapSpecificSiteNamesDetailsTask", tables, 
                                    "Snap Specific Site Name Details Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "SnapSpecificSiteNamesDetailsTask", e, 
                                    "Snap Specific Site Name Details Task failed"
                            );
                }
                
                
            }

            
        }
    }

}