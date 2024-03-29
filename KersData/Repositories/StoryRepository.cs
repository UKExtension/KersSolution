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


        public List<int> LastStoryRevisionIds( FiscalYear fiscalYear, int filter = 4, int id = 0){
            var cacheKey = CacheKeys.LastStoryRevisionIds + fiscalYear.Name + "_" + filter.ToString() + "_" + id.ToString();
            var cacheString = _cache.GetString(cacheKey);
            List<int> ids;
            if (!string.IsNullOrEmpty(cacheString)){
                ids = JsonConvert.DeserializeObject<List<int>>(cacheString);
            }else{
                ids = new List<int>();
                var stories = context.Story
                                .Where(r => r.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear);
                if( filter == FilterKeys.PlanningUnit){
                    stories = stories.Where( s => s.PlanningUnitId == id );
                }else if( filter == FilterKeys.MajorProgram ){
                    stories = stories.Where( s => s.MajorProgramId == id);
                }else if( filter == FilterKeys.Employee ){
                    stories = stories.Where( s => s.KersUserId == id );
                }
                stories = stories.Include( r => r.Revisions);
                stories = stories.OrderByDescending( s => s.Updated );
                foreach( var story in stories){
                    var rev = story.Revisions.OrderBy( r => r.Created );
                    var last = rev.Last();
                    ids.Add(last.Id);
                }
                var serialized = JsonConvert.SerializeObject(ids);
                
                var cacheDaysSpan = 30;

                var today = DateTime.Now;
                if(fiscalYear.Start < today && Math.Max( fiscalYear.End.Ticks, fiscalYear.ExtendedTo.Ticks) > today.Ticks){
                    cacheDaysSpan = 2;
                }

                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cacheDaysSpan)
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

        

        public async Task<List<StoryViewModel>> LastStoriesWithImages(FiscalYear fiscalYear = null, int filter = 4, int id = 0, int amount = 6, bool refreshCache = false, int keepCacheInDays = 0){
            
            List<StoryViewModel> stories;
            
            var cacheKey = CacheKeys.LastStoriesWithImages + fiscalYear.Name + filter.ToString() + id.ToString() + amount.ToString();
            var cacheString = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                stories = JsonConvert.DeserializeObject<List<StoryViewModel>>(cacheString);
            }else{
                stories = new List<StoryViewModel>();
                var ids = LastStoryRevisionIds( fiscalYear, filter, id);

                foreach( var revId in ids ){
                    var rev = context.StoryRevision.Where( s => s.Id == revId).Include( r => r.StoryImages );
                    if( rev.Where(s => s.StoryImages.Count > 0).Any() ){
                        var theStoryRev = context.StoryRevision.Where( s => s.Id == revId)
                                    .Include(r => r.StoryImages).ThenInclude( v => v.UploadImage ).ThenInclude( f => f.UploadFile)
                                    .Include( r => r.MajorProgram);
                        var theStory = await theStoryRev.FirstOrDefaultAsync();
                        var uploadFile = theStory.StoryImages.OrderBy( s => s.Created).Last().UploadImage;
                        if( theStory != null && uploadFile != null ){
                            var model = new StoryViewModel();
                            model.Updated = theStory.Created;
                            model.Title = theStory.Title;
                            model.StoryOutcome = theStory.StoryOutcome;
                            model.StoryId = theStory.StoryId;
                            model.Story = theStory.Story;
                            model.MajorProgram = theStory.MajorProgram;
                            var parentStrory = await context.Story.Where( s => s.Id == theStory.StoryId)
                                                        .Include( r => r.PlanningUnit)
                                                        .Include( r => r.KersUser ).ThenInclude( u => u.PersonalProfile )
                                                        .FirstAsync();
                            model.PlanningUnit = parentStrory.PlanningUnit;
                            model.KersUser = parentStrory.KersUser;
                            model.ImageName = uploadFile.UploadFile.Name;
                            stories.Add( model );
                        }
                        if( stories.Count >= amount ) break;
                    }
                }
                var cacheDaysSpan = keepCacheInDays;
                if( cacheDaysSpan == 0 ){
                    var today = DateTime.Now;
                    if(fiscalYear.Start < today && Math.Max( fiscalYear.End.Ticks, fiscalYear.ExtendedTo.Ticks) > today.Ticks){
                        cacheDaysSpan = 2;
                    }else{
                        cacheDaysSpan = 110;
                    }
                }
                var serialized = JsonConvert.SerializeObject(stories);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cacheDaysSpan)
                    });
            }
            return stories;
        
        }


        public async Task<List<StoryViewModel>> LastStoriesWithoutImages(FiscalYear fiscalYear = null, int filter = 4, int id = 0, int amount = 6, bool refreshCache = false, int keepCacheInDays = 0){
            
            List<StoryViewModel> stories;
            
            var cacheKey = CacheKeys.LastStoriesWithoutImages + fiscalYear.Name + filter.ToString() + id.ToString() + amount.ToString();
            var cacheString = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                stories = JsonConvert.DeserializeObject<List<StoryViewModel>>(cacheString);
            }else{
                stories = new List<StoryViewModel>();
                var ids = LastStoryRevisionIds( fiscalYear, filter, id);

                foreach( var revId in ids ){
                    var rev = context.StoryRevision.Where( s => s.Id == revId).Include( r => r.StoryImages );
                    if( rev.Where(s => s.StoryImages == null || s.StoryImages.Count == 0).Any() ){
                        var theStoryRev = context.StoryRevision.Where( s => s.Id == revId)
                                    .Include(r => r.StoryImages).ThenInclude( v => v.UploadImage ).ThenInclude( f => f.UploadFile)
                                    .Include( r => r.MajorProgram);
                        var theStory = await theStoryRev.FirstOrDefaultAsync();
                        
                        var model = new StoryViewModel();
                        model.Updated = theStory.Created;
                        model.Title = theStory.Title;
                        model.StoryOutcome = theStory.StoryOutcome;
                        model.StoryId = theStory.StoryId;
                        model.Story = theStory.Story;
                        model.MajorProgram = theStory.MajorProgram;
                        var parentStrory = await context.Story.Where( s => s.Id == theStory.StoryId)
                                                    .Include( r => r.PlanningUnit)
                                                    .Include( r => r.KersUser ).ThenInclude( u => u.PersonalProfile )
                                                    .FirstAsync();
                        model.PlanningUnit = parentStrory.PlanningUnit;
                        model.KersUser = parentStrory.KersUser;
                        stories.Add( model );
                        
                        if( stories.Count >= amount ) break;
                    }
                }
                var cacheDaysSpan = keepCacheInDays;
                if( cacheDaysSpan == 0 ){
                    var today = DateTime.Now;
                    if(fiscalYear.Start < today && Math.Max( fiscalYear.End.Ticks, fiscalYear.ExtendedTo.Ticks) > today.Ticks){
                        cacheDaysSpan = 2;
                    }else{
                        cacheDaysSpan = 110;
                    }
                }
                var serialized = JsonConvert.SerializeObject(stories);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cacheDaysSpan)
                    });
            }
            return stories;
        
        }







        public async Task<List<StoryViewModel>> LastStories( FiscalYear fiscalYear = null, int amount = 4, int PlanningUnitId = 0, int MajorProgramId = 0, bool refreshCache = false ){
            
            List<StoryViewModel> lastStories;
            
            var cacheKey = CacheKeys.LastStories + PlanningUnitId.ToString() + MajorProgramId.ToString() + amount.ToString() + (fiscalYear == null ? "null" : fiscalYear.Name);
            var cacheString = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                lastStories = JsonConvert.DeserializeObject<List<StoryViewModel>>(cacheString);
            }else{
                List<Story> Last;
                if(MajorProgramId != 0 ){
                    Last = new List<Story>();


                    var RevsPerProgram = context.StoryRevision.Where( r => r.MajorProgramId == MajorProgramId );

                    if( fiscalYear != null ){
                        RevsPerProgram = RevsPerProgram.Where( s => s.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear );
                    }
                    RevsPerProgram = RevsPerProgram.Take( amount * 5 );

                    var GrouppedByStory = RevsPerProgram.ToList()
                                                .GroupBy( r => r.StoryId )
                                                .Select( r => r );

                    foreach( var stry in GrouppedByStory ){
                        var lst = stry.OrderBy( r => r.Created ).Last();
                        if( lst.MajorProgramId == MajorProgramId ){
                            Last.Add( this.context.Story.Find(lst.StoryId) );
                            if(Last.Count() >= amount ){
                                break;
                            }
                        }
                    }


                }else{
                    var LastQuery = this.context.Story.Where( s => true );

                    if( fiscalYear != null ){
                        LastQuery = LastQuery.Where( s => s.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear );
                    }

                    if( PlanningUnitId != 0 ){
                        LastQuery = LastQuery.Where( s => s.PlanningUnitId == PlanningUnitId );
                    }
                    Last = await LastQuery.OrderByDescending( s => s.Updated ).Take( amount ).ToListAsync();
                }


                lastStories = new List<StoryViewModel>();

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
                var serialized = JsonConvert.SerializeObject(lastStories);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(5)
                    });
            }
            return lastStories;
        }

        public async Task<List<StoryViewModel>> LastStoriesByUser( int userId, FiscalYear fiscalYear = null, int amount = 4, bool refreshCache = false ){
            
            List<StoryViewModel> lastStories;
            
            var cacheKey = CacheKeys.LastStoriesByUser + userId.ToString()+ "_" + amount.ToString() + (fiscalYear == null ? "null" : fiscalYear.Name);
            var cacheString = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                lastStories = JsonConvert.DeserializeObject<List<StoryViewModel>>(cacheString);
            }else{

                List<Story> Last;
                
                var LastQuery = this.context.Story.Where( s => s.KersUserId == userId );
                if( fiscalYear != null ){
                    LastQuery = LastQuery.Where( s => s.MajorProgram.StrategicInitiative.FiscalYear == fiscalYear );
                }
                Last = await LastQuery.OrderByDescending( s => s.Updated ).Take( amount ).ToListAsync();
                


                lastStories = new List<StoryViewModel>();

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
                    story.KersUser = context.KersUser.Where( s => s.Id == userId)
                                        .Include( u => u.RprtngProfile ).ThenInclude( p => p.PlanningUnit )
                                        .Include( u => u.PersonalProfile )
                                        .FirstOrDefault();
                    story.PlanningUnit = story.KersUser.RprtngProfile.PlanningUnit;
                    story.MajorProgram = lastRev.MajorProgram;
                    story.Updated = lastRev.Created;
                    lastStories.Add(story);
                }
                var serialized = JsonConvert.SerializeObject(lastStories);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(5)
                    });
            }
            return lastStories;
        }




    }


    
}