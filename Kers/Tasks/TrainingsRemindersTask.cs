using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Kers.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;

namespace Kers.Tasks
{
    public class TrainingsRemindersTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        IHostingEnvironment environment;
        public TrainingsRemindersTask(
            IServiceProvider serviceProvider,
            IHostingEnvironment environment
        ){
            this.serviceProvider = serviceProvider;
            this.environment = environment;
        }
        public string Schedule => "29 7 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                var reportingContext = scope.ServiceProvider.GetService<KERSreportingContext>();
                var mainContext = scope.ServiceProvider.GetService<KERSmainContext>();
                var messagingRepo = new MessageRepository(context);
                var repo = new TrainingRepository(context,reportingContext,mainContext, messagingRepo);
                try{
                    var startTime = DateTime.Now;
                    
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                    var messages = repo.Set3DaysReminders();
                    


                    if( messages.Count() != 0 ){
                        var endTime = DateTime.Now;
                        await LogComplete(context, 
                                        "3DaysReminders", messages, 
                                        messages.Count().ToString() + " Messages send for " + (endTime - startTime).TotalSeconds + " seconds"
                                    );
                    }

                    messages = repo.Set7DaysReminders();

                    if( messages.Count() != 0 ){
                        var endTime = DateTime.Now;
                        await LogComplete(context, 
                                        "7DaysReminders", messages, 
                                        messages.Count().ToString() + " Messages send for " + (endTime - startTime).TotalSeconds + " seconds"
                                    );
                    }

                     var EvaluationMeessages = repo.SetEvaluationReminders();

                    if( EvaluationMeessages.Count() != 0 ){
                        var endTime = DateTime.Now;
                        await LogComplete(context, 
                                        "EvaluationReminders", EvaluationMeessages, 
                                        messages.Count().ToString() + " Messages send for " + (endTime - startTime).TotalSeconds + " seconds"
                                    );
                    }




                }catch( Exception e){
                    await LogError(context, 
                                    "TrainingRemindersError", e, 
                                    "Process Training Reminders Task failed"
                            );
                }   
            }
        }
    }


}