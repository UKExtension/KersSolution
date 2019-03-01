using System;
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
    public class TaskBase
    {
        //KERScoreContext context;
        
        public async Task  LogError(KERScoreContext context, string className = null, object obj = null,
                            string description = "Scheduled Task Error"){
            await this.Log(context, className, obj, "Error", description);
        }
        public async Task LogComplete(KERScoreContext context, string className = null, object obj = null,
                            string description = "Ececuted Scheduled Task"){
            await this.Log(context, className, obj, "Information", description);
        }
        public async Task Log(    KERScoreContext context,
                            string className,
                            object obj,
                            string level = "Information", 
                            string description = "Excuted Scheduled Task"
                        ){
                             
            var log = new Log();
            log.Level = level;
            log.Time = DateTime.Now;
            log.ObjectType = obj.GetType().ToString();
            log.Object = JsonConvert.SerializeObject(obj,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
            log.Description = description;
            log.Type = className;
            context.Log.Add(log);
            await context.SaveChangesAsync();

        }

    }

}