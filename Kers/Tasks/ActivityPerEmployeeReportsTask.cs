using System;
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
    public class ActivityPerEmployeeReportsTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public ActivityPerEmployeeReportsTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "20 21 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                var progressLog = "";
                try{
                    var cache = scope.ServiceProvider.GetService<IDistributedCache>();
                    var memoryCache = scope.ServiceProvider.GetService<IMemoryCache>();
                    var fiscalYearRepo = new FiscalYearRepository( context );
                    var repo = new ContactRepository(cache, context, memoryCache);
                    var startTime = DateTime.Now;
                    progressLog += startTime.ToString() + ": ActivityPerEmployeeReportsTask Started\n";
                    Random rnd = new Random();
                    int RndInt = rnd.Next(1, 53);
                    var tables = new List<TableViewModel>();
                    progressLog += DateTime.Now.ToString() + ": Random Number = "+ RndInt.ToString() + "\n";
                    // Districts
                    var districts =  context.District;
                    foreach( var district in districts){
                       progressLog += DateTime.Now.ToString() + ": " + district.Name + " Data for current fiscal year started.\n";
                       var tbl = await repo.DataByEmployee(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),0, district.Id, true);
                       progressLog += DateTime.Now.ToString() + ": " + district.Name + " Data for current fiscal year finished.\n";
                       if(RndInt == 1){
                           progressLog += DateTime.Now.ToString() + ": " + district.Name + " Data for previous fiscal year started.\n";
                           tbl = await repo.DataByEmployee(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),0, district.Id, true);
                           progressLog += DateTime.Now.ToString() + ": " + district.Name + " Data for previous fiscal year finished.\n";
                       }
                       tables.Add(tbl);
                    }
                    // Planning Units
                    var units = context.PlanningUnit;
                    foreach( var unit in units){
                        progressLog += DateTime.Now.ToString() + ": " + unit.Name + " Data for current fiscal year started.\n";
                        var tbl = await repo.DataByEmployee(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),1, unit.Id, true);
                        progressLog += DateTime.Now.ToString() + ": " + unit.Name + " Data for current fiscal year finished.\n";
                        if(RndInt == 2){
                            progressLog += DateTime.Now.ToString() + ": " + unit.Name + " Data for previous fiscal year started.\n";
                            tbl = await repo.DataByEmployee(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),1, unit.Id, true);
                            progressLog += DateTime.Now.ToString() + ": " + unit.Name + " Data for previous fiscal year finished.\n";
                        }
                        tables.Add(tbl);
                    }
                    // KSU
                    if( startTime.DayOfWeek == DayOfWeek.Friday){
                        progressLog += DateTime.Now.ToString() + ": KSU Data for current fiscal year started.\n";
                        var tblKSU = await repo.DataByEmployee(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),2, 0, true, 20);
                        progressLog += DateTime.Now.ToString() + ": KSU Data for current fiscal year finished.\n";
                        if( RndInt == 3 ){
                            progressLog += DateTime.Now.ToString() + ": KSU Data for previous fiscal year started.\n";
                            tblKSU = await repo.DataByEmployee(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),2, 0, true);
                            progressLog += DateTime.Now.ToString() + ": KSU Data for previous fiscal year finished.\n";
                        }
                        tables.Add(tblKSU);
                    }

                    // UK
                    if( startTime.DayOfWeek == DayOfWeek.Saturday){
                        progressLog += DateTime.Now.ToString() + ": UK Data for current fiscal year started.\n";
                        var tblUK = await repo.DataByEmployee(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),3, 0, true, 20);
                        progressLog += DateTime.Now.ToString() + ": UK Data for current fiscal year finished.\n";
                        if( RndInt == 4 ){
                            progressLog += DateTime.Now.ToString() + ": UK Data for previous fiscal year started.\n";
                            tblUK = await repo.DataByEmployee(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),3, 0, true);
                            progressLog += DateTime.Now.ToString() + ": UK Data for previous fiscal year finished.\n";
                        }
                        tables.Add(tblUK);
                    }

                    // ALL
                    if( startTime.DayOfWeek == DayOfWeek.Sunday){
                        progressLog += DateTime.Now.ToString() + ": ALL Data for current fiscal year started.\n";
                        var tblAll = await repo.DataByEmployee(fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog),4, 0, true, 20);
                        progressLog += DateTime.Now.ToString() + ": ALL Data for current fiscal year finished.\n";
                        if( RndInt == 5 ){
                            progressLog += DateTime.Now.ToString() + ": All Data for previous fiscal year started.\n";
                            tblAll = await repo.DataByEmployee(fiscalYearRepo.previoiusFiscalYear(FiscalYearType.ServiceLog),4, 0, true);
                            progressLog += DateTime.Now.ToString() + ": All Data for previous fiscal year finished.\n";
                        }
                        tables.Add(tblAll);
                    }

                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "ActivityPerEmployeeReportsTask", tables, 
                                    "Activity Per Employee Reports Task executed for " + (endTime - startTime).TotalSeconds + " seconds", progressLog
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "ActivityPerEmployeeReportsTask", e, 
                                    "Activity Per Employee Reports Task failed", progressLog
                            );
                }
                
                
            }

            
        }
    }

}