using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace Kers.Tasks
{
    public class UpdateUsersTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        IWebHostEnvironment environment;
        public UpdateUsersTask(
            IServiceProvider serviceProvider,
            IWebHostEnvironment environment
        ){
            this.serviceProvider = serviceProvider;
            this.environment = environment;
        }
        public string Schedule => "11 1 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                var repo = new KersUserRepository(context);
                try{
                    var startTime = DateTime.Now;
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                    
                    var userList = context.ReportingProfile.Where( r => r.enabled ).OrderBy(x => Guid.NewGuid()).Take(200);
                    var disabledUsers = new List<ReportingProfile>();
                    
                    foreach( var user in userList)
                    {
                        
                        if(  !repo.IsInAD(user.LinkBlueId) )
                        {
                            user.enabled = false;
                            disabledUsers.Add(user);
                        }
                    }
                    
                    
                    context.SaveChanges(); 
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                        "List<KersUser>", disabledUsers, 
                                        disabledUsers.Count().ToString() + " users disabled for " + (endTime - startTime).TotalSeconds + " seconds"
                                    );
                    
                }catch( Exception e){
                    await LogError(context, 
                                    "Update Users", e, 
                                    "Update Users Task failed"
                            );
                }   
            }
        }
    }


}