﻿using System;
using System.Collections.Generic;
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
    public class ActivityPerMajorProgramTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public ActivityPerMajorProgramTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "18 0 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var repo = new ContactRepository(cache, context);
                    var startTime = DateTime.Now;
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    var tables = new List<TableViewModel>();
                    // Districts
                    var districts =  context.District;
                    foreach( var district in districts){
                       var tbl = await repo.DataByMajorProgram(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),0, district.Id, true);
                       if(RndInt == 1){
                           tbl = await repo.DataByMajorProgram(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),0, district.Id, true);
                       }
                       tables.Add(tbl);
                    }
                    // Planning Units
                    var units = context.PlanningUnit;
                    foreach( var unit in units){
                        var tbl = await repo.DataByMajorProgram(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),1, unit.Id, true);
                        if(RndInt == 2){
                            tbl = await repo.DataByMajorProgram(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),1, unit.Id, true);
                        }
                        tables.Add(tbl);
                    }
                    // KSU
                    var tblKSU = await repo.DataByMajorProgram(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),2, 0, true);
                    if( RndInt == 3 ){
                        tblKSU = await repo.DataByMajorProgram(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),2, 0, true);
                    }
                    tables.Add(tblKSU);

                    // UK
                    var tblUK = await repo.DataByMajorProgram(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),3, 0, true);
                    if( RndInt == 4 ){
                        tblUK = await repo.DataByMajorProgram(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),3, 0, true);
                    }
                    tables.Add(tblUK);

                    // ALL
                    var tblAll = await repo.DataByMajorProgram(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),4, 0, true);
                    if( RndInt == 5 ){
                        tblAll = await repo.DataByMajorProgram(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),4, 0, true);
                    }
                    tables.Add(tblAll);
                    
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "ActivityPerMajorProgramTask", tables, 
                                    "Activity Per Major Program Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "ActivityPerMajorProgramTask", e, 
                                    "Activity Per Major Program Task failed"
                            );
                }
                
                
            }

            
        }
    }

}