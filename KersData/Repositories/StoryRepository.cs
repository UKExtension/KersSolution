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

        public async Task<StoryViewModel> LastStoryWithImages(int PlanningUnitId = 0, int MajorProgramId = 0, bool refreshCache = false){

            StoryViewModel story;
            


            var cacheKey = CacheKeys.LastStoryWithImages + PlanningUnitId.ToString() + MajorProgramId.ToString();
            var cacheString = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                story = JsonConvert.DeserializeObject<StoryViewModel>(cacheString);
            }else{

                story = new StoryViewModel();

                if( PlanningUnitId == 0 && MajorProgramId == 0){

                    var Last = this.context.StoryRevision
                                        .Where( s => s.StoryImages.Count() > 0 )
                                        .Include( s => s.StoryImages).ThenInclude( i => i.UploadImage ).ThenInclude( u => u.UploadFile )
                                        .Include( s => s.MajorProgram )
                                        .OrderBy( s => s.Created );
                
                    var LastRevWithImages = await Last.LastAsync();
                    
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

                }else{

                    var stories = context.Story.Where(s => true);
                    
                    if( PlanningUnitId != 0 ){
                        stories = stories.Where( s => s.PlanningUnitId == PlanningUnitId);
                    }
                    Story theStory = null;
                    StoryRevision theStoryRevision = null;

                    foreach( var stry in stories){
                        var storyRevision = context.StoryRevision.Where( s => s.StoryId == stry.Id );
                        if(MajorProgramId != 0 ){
                            storyRevision = storyRevision.Where( r => r.MajorProgramId == MajorProgramId);
                        }
                        storyRevision = storyRevision
                                            .Include( s => s.StoryImages)
                                            .OrderBy(r => r.Created);
                        var last = await storyRevision.LastAsync();
                        if( last.StoryImages != null && last.StoryImages.Count() > 0 ){
                            theStory = stry;
                            theStoryRevision = last;
                            break;
                        }
                    }
                    if(theStory == null || theStoryRevision == null){
                        if( stories != null && stories.Count() > 0){
                            theStory = stories.Last();
                            story.StoryId = theStory.Id;

                            var fullRevision = await context.StoryRevision.Where( r => r.StoryId == theStory.Id)
                                            .Include( s => s.StoryImages).ThenInclude( i => i.UploadImage ).ThenInclude( u => u.UploadFile )
                                            .Include( s => s.MajorProgram )
                                            .OrderBy( s => s.Created ).LastAsync();

                            story.Title = fullRevision.Title;
                            story.Story = fullRevision.Story;
                            story.ImageName = "32c5386951efbb7aa9e4c8b1e13dc4f3dcedbe5f.jpg";
                            story.KersUser = context.Story.Where( s => s.Id == theStory.Id)
                                            .Include( s => s.KersUser ).ThenInclude( u => u.RprtngProfile ).ThenInclude( p => p.PlanningUnit )
                                            .Include( s => s.KersUser ).ThenInclude( u => u.PersonalProfile )
                                            .FirstOrDefault().KersUser;
                            story.PlanningUnit = story.KersUser.RprtngProfile.PlanningUnit;
                            story.MajorProgram = fullRevision.MajorProgram;
                            story.Updated = fullRevision.Created;
                        }
                        
                        //
                    }else{


                        story.StoryId = theStory.Id;
                        story.Title = theStoryRevision.Title;
                        story.Story = theStoryRevision.Story;

                        var fullRevision = await context.StoryRevision.Where( r => r.Id == theStoryRevision.Id)
                                        .Include( s => s.StoryImages).ThenInclude( i => i.UploadImage ).ThenInclude( u => u.UploadFile )
                                        .Include( s => s.MajorProgram )
                                        .OrderBy( s => s.Created ).LastAsync();




                        story.ImageName = fullRevision.StoryImages.Last().UploadImage.UploadFile.Name;
                        

                        //Find Landscape Image if available.
                        foreach( var img in fullRevision.StoryImages){
                            if( img.UploadImage.Width > img.UploadImage.Height){
                                story.ImageName = img.UploadImage.UploadFile.Name;
                                break;
                            }
                        }

                        story.KersUser = context.Story.Where( s => s.Id == theStory.Id)
                                        .Include( s => s.KersUser ).ThenInclude( u => u.RprtngProfile ).ThenInclude( p => p.PlanningUnit )
                                        .Include( s => s.KersUser ).ThenInclude( u => u.PersonalProfile )
                                        .FirstOrDefault().KersUser;
                        story.PlanningUnit = story.KersUser.RprtngProfile.PlanningUnit;
                        story.MajorProgram = fullRevision.MajorProgram;
                        story.Updated = fullRevision.Created;

                    }

                }

                var serialized = JsonConvert.SerializeObject(story);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(5)
                    });


            }

            
            
            
            
            
            
            
            
            return story;

            
        }

        public async Task<List<StoryViewModel>> LastStories( int amount = 4, int PlanningUnitId = 0, int MajorProgramId = 0, bool refreshCache = false ){
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