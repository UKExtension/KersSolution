using System;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Kers.Tasks{
    public class TaskBase{
        public KERScoreContext context;
        public IDistributedCache cache;
        public TaskBase(KERScoreContext context, IDistributedCache cache){
            this.context = context;
            this.cache = cache;

        }





    }
}