using Kers.Models.Contexts;
using Microsoft.Extensions.Caching.Distributed;

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