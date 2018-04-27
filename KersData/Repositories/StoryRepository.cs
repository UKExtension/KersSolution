using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Kers.Models.Repositories
{
    public class StoryRepository : EntityBaseRepository<Story>, IStoryRepository
    {


        KERScoreContext context;
        IDistributedCache _cache;
        public StoryRepository(KERScoreContext context, IDistributedCache _cache)
            : base(context)
        { 
            this.context = context;
            this._cache = _cache;
        }


        public List<int> LastStoryRevisionIds( FiscalYear fiscalYear){
            var cacheKey = CacheKeys.LastStoryRevisionIds + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            List<int> ids;
            if (!string.IsNullOrEmpty(cacheString)){
                ids = JsonConvert.DeserializeObject<List<int>>(cacheString);
            }else{
                ids = new List<int>();
                var stories = context.Story
                                .Where(r => r.Created > fiscalYear.Start && r.Created < fiscalYear.End)
                                .Include( r => r.Revisions).OrderByDescending(s => s.Created);
                foreach( var story in stories){
                    var rev = story.Revisions.OrderBy( r => r.Created );
                    var last = rev.Last();
                    ids.Add(last.Id);
                }
                var serialized = JsonConvert.SerializeObject(ids);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });
            }
            return ids;
        }
        public List<StoryRevision> LastStoryRevisions( FiscalYear fiscalYear){
            var cacheKey = CacheKeys.LastStoryRevisions + fiscalYear.Name;
            var cacheString = _cache.GetString(cacheKey);
            List<StoryRevision> revs;
            if (!string.IsNullOrEmpty(cacheString)){
                revs = JsonConvert.DeserializeObject<List<StoryRevision>>(cacheString);
            }else{

                var revids = LastStoryRevisionIds(fiscalYear);
                revs= new List<StoryRevision>();
                var batchCount = 10000;
                for(var i = 0; i <= revids.Count(); i += batchCount){
                    var currentBatch = revids.Skip(i).Take(batchCount);
                    revs.AddRange(context.StoryRevision.Where( r => currentBatch.Contains( r.Id )).Include(s => s.StoryImages).ToList());
                }

                var serialized = JsonConvert.SerializeObject(revs);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });
            }
            return revs;
        }




    }


    
}