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
using Kers.Models.ViewModels;

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
                                .Include( r => r.Revisions);
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
                    revs.AddRange(
                                    context.StoryRevision.Where( r => currentBatch.Contains( r.Id ))
                                        .Include(s => s.StoryImages)
                                        .Include( s => s.MajorProgram)
                                        .ToList()
                                );
                }
                revs = revs.OrderByDescending( r => r.Created ).ToList();
                var serialized = JsonConvert.SerializeObject(revs);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });
            }
            return revs;
        }

        public async Task<StoryViewModel> LastStoryWithImages(){
            var LastRevWithImages = await this.context.StoryRevision
                                    .Where( s => s.StoryImages.Count() > 0 )
                                    .Include( s => s.StoryImages).ThenInclude( i => i.UploadImage ).ThenInclude( u => u.UploadFile )
                                    .Include( s => s.MajorProgram )
                                    .OrderBy( s => s.Created ).LastAsync();
            
            

            var story = new StoryViewModel();
            story.StoryId = LastRevWithImages.StoryId;
            story.Title = LastRevWithImages.Title;
            story.Story = LastRevWithImages.Story;
            story.ImageName = LastRevWithImages.StoryImages.Last().UploadImage.UploadFile.Name;

            //Find Landscape Image if available.
            foreach( var img in LastRevWithImages.StoryImages){
                if( img.UploadImage.Width > img.UploadImage.Height){
                    story.ImageName = img.UploadImage.UploadFile.Name;
                    break;
                }
            }

            story.KersUser = context.Story.Where( s => s.Id == LastRevWithImages.StoryId)
                                .Include( s => s.KersUser ).ThenInclude( u => u.RprtngProfile ).ThenInclude( p => p.PlanningUnit )
                                .Include( s => s.KersUser ).ThenInclude( u => u.PersonalProfile )
                                .FirstOrDefault().KersUser;
            story.PlanningUnit = story.KersUser.RprtngProfile.PlanningUnit;
            story.MajorProgram = LastRevWithImages.MajorProgram;
            story.Updated = LastRevWithImages.Created;
            return story;

            
        }

        public async Task<List<StoryViewModel>> LastStories( int amount = 4 ){
            var Last = await this.context.Story.OrderByDescending( s => s.Updated ).Take( amount ).ToListAsync();
            var lastStories = new List<StoryViewModel>();

            foreach( var str in Last ){
                var lastRev = await context.StoryRevision
                                        .Where(s => s.StoryId == str.Id)
                                        .Include( s => s.StoryImages).ThenInclude( i => i.UploadImage ).ThenInclude( u => u.UploadFile )
                                        .Include( s => s.MajorProgram )
                                        .OrderBy( s => s.Created)
                                        .LastAsync();
                var story = new StoryViewModel();
                story.StoryId = lastRev.StoryId;
                story.Title = lastRev.Title;
                story.Story = lastRev.Story;


                if(lastRev.StoryImages.Count > 0){
                    story.ImageName = lastRev.StoryImages.Last().UploadImage.UploadFile.Name;

                    //Find Landscape Image if available.
                    foreach( var img in lastRev.StoryImages){
                        if( img.UploadImage.Width > img.UploadImage.Height){
                            story.ImageName = img.UploadImage.UploadFile.Name;
                            break;
                        }
                    }
                }
                

                story.KersUser = context.KersUser.Where( s => s.Id == str.KersUserId)
                                    .Include( u => u.RprtngProfile ).ThenInclude( p => p.PlanningUnit )
                                    .Include( u => u.PersonalProfile )
                                    .FirstOrDefault();
                story.PlanningUnit = story.KersUser.RprtngProfile.PlanningUnit;
                story.MajorProgram = lastRev.MajorProgram;
                story.Updated = lastRev.Created;
                lastStories.Add(story);
            }
            return lastStories;
        }




    }


    
}