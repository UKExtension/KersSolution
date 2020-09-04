using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Data;
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
    public class GetActivitiesAndContactsTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public GetActivitiesAndContactsTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "52 22 * * 6";
        
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
                    var repo = new ContactRepository(cache, context, memoryCache);


                    Random rnd = new Random();
                    int RndInt;

                    var startTime = DateTime.Now;

                    var tbl = new List<PerGroupActivities>();


                    var years = fiscalYearRepo.FindBy( y => y.Type == FiscalYearType.ServiceLog && y.Start < DateTime.Now);
                    
                    foreach( var year in years){
                        // Planning Units
                        var units = context.PlanningUnit;
                        foreach( var unit in units){
                            var today = DateTime.Now;
                            // if this is current fiscal year
                            if( Math.Max( year.End.Ticks, year.ExtendedTo.Ticks) > today.Ticks){
                                tbl = await repo.GetActivitiesAndContactsAsync(year.Start, year.End,1, 0, unit.Id, true);
                                tbl = await repo.GetActivitiesAndContactsAsync(year.Start, year.End,1, 1, unit.Id, true);
                            }else{
                                RndInt = rnd.Next(1, 33);
                                if( RndInt == 3 ){
                                    tbl = await repo.GetActivitiesAndContactsAsync(year.Start, year.End,1, 0, unit.Id, true);
                                    tbl = await repo.GetActivitiesAndContactsAsync(year.Start, year.End,1, 1, unit.Id, true);
                                }

                            }
                            
                            
                        }
                    }
                        



                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "GetActivitiesAndContactsTask", tbl, 
                                    "GetActivitiesAndContactsTask executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "GetActivitiesAndContactsTask", e, 
                                    "AGetActivitiesAndContactsTask failed"
                            );
                }
                
                
            }

            
        }
    }

}