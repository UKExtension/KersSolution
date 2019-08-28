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
    public class MessageProcessingTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        IHostingEnvironment environment;
        public MessageProcessingTask(
            IServiceProvider serviceProvider,
            IHostingEnvironment environment
        ){
            this.serviceProvider = serviceProvider;
            this.environment = environment;
        }
        public string Schedule => "* * * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                var repo = new MessageRepository(context);
                try{
                    var startTime = DateTime.Now;
                    
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                    var messages = repo.ProcessMessageQueue( _configuration, environment );




                    if( messages.Count() != 0 ){
                        var endTime = DateTime.Now;
                        await LogComplete(context, 
                                        "ProcessMessageQueue", messages, 
                                        messages.Count().ToString() + " Messages send for " + (endTime - startTime).TotalSeconds + " seconds"
                                    );
                    }

                    





                }catch( Exception e){
                    await LogError(context, 
                                    "ProcessMessageQueue", e, 
                                    "Process Message Queue Task failed"
                            );
                }   
            }
        }
    }


}