using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Tasks.Services{
    public class TaskService{
        KERScoreContext context;
        IDistributedCache cache;
        public TaskService(KERScoreContext context, IDistributedCache cache){
            this.context = context;
            this.cache = cache;
        }

        public void run(){
            foreach( var operation in getOperations()){
                execute(operation);
            }
        }

        private List<TaskOperation> getOperations(){
            var operations = this.context.TaskOperation.ToList();
            return operations;
        }

        private void execute(TaskOperation operation){
            Type type = Assembly.GetEntryAssembly().GetType("Kers.Tasks." + operation.ClassName);
            if(type == null){
                throw new Exception("Object type is NULL.");
                //Console.WriteLine("Object type is NULL.");
            }else{
                ITask entity = (ITask) Activator.CreateInstance(type, context, cache);
                entity.run(operation.Arguments.Split(','));
            }
        }
    }
}