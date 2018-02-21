using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Kers.Tasks.Services{
    public class TaskService{
        KERScoreContext context;
        KERSmainContext mainContext;
        IDistributedCache cache;
        TaskScheduleService scheduleService;
        public TaskService(KERScoreContext context, KERSmainContext mainContext, IDistributedCache cache){
            this.context = context;
            this.mainContext = mainContext;
            this.cache = cache;
            scheduleService = new TaskScheduleService(context);
        }

        public void run(){
            foreach( var schedule in getOperations()){
                var performed = new TaskPerformed();
                performed.TaskSchedule = schedule;
                performed.PerformedAt = DateTime.Now;
                context.Add(performed);
                context.SaveChanges();
                execute(schedule.TaskOperation);
            }
        }

        private List<TaskSchedule> getOperations(){

            var schedules = this.context.TaskSchedule.Where( a => true)
                                .Include( a => a.TaskRecurringSchedule)
                                .Include( a => a.TaskOperation)
                                .ToList();


            var operations = new List<TaskSchedule>();

            foreach( var schedule in schedules){
                if(scheduleService.ShouldItBeExecuted(schedule)){
                    operations.Add(schedule);
                }
            }

            return operations;
        }

        private void execute(TaskOperation operation){
            Type type = Assembly.GetEntryAssembly().GetType("Kers.Tasks." + operation.ClassName);
            if(type == null){
                LogError(operation.ClassName, "Object type is NULL.");
            }else{
                try{
                    ITask entity = (ITask) Activator.CreateInstance(type, context, mainContext, cache);
                    var result = entity.run(operation.Arguments.Split(','));
                    LogComplete(operation.ClassName,  result );
                }catch(Exception e){
                    LogError(operation.ClassName, e );
                }
                
            }
        }


        
        
        
        
        
        
        
        
        public void LogError(string className = null, object obj = null){
            this.Log(className, obj, "Error");
        }
        public void LogComplete(string className = null, object obj = null){
            this.Log(className, obj);
        }
        public void Log(    string className,
                            object obj,
                            string level = "Information", 
                            string description = "Ececuted Scheduled Task"
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
            this.context.Log.Add(log);
            context.SaveChangesAsync();

        }



    }
}