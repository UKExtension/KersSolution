using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Abstract;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;

namespace Kers.Controllers
{
    [Route("api/[controller]")]
    public class LogController : Controller
    {

        private IDistributedCache _cache;
        KERScoreContext _context;
        KERSmainContext mainContext;
        IKersUserRepository userRepo;
        public LogController(
            KERScoreContext _context,
            KERSmainContext mainContext,
            IKersUserRepository userRepo,
            //IMemoryCache memoryCache,
            IDistributedCache _cache
        ){
            this._context = _context;
            this.mainContext = mainContext;
            this.userRepo = userRepo;
            this._cache = _cache;
        }
        
        [HttpGet("{skip?}/{amount?}")]
        [Authorize] 
        public IActionResult Logs(int skip = 0, int amount = 10)
        {
            var logs = this._context.Log.Where(l => l.Id > 0).
                                Include(l=>l.User).ThenInclude(u=>u.PersonalProfile).ThenInclude(pr => pr.UploadImage).ThenInclude(pr => pr.UploadFile).
                                OrderByDescending(l=>l.Time).Skip(skip).Take(amount);
            foreach(var log in logs){
                if(log.User.PersonalProfile.UploadImage != null){
                    log.User.PersonalProfile.UploadImage.UploadFile.Content = null;
                }
            }
            return new OkObjectResult(logs);
        }

        [HttpGet("types")]
        [Authorize]
        public IActionResult ListTypes(){
            var cacheKey = "LogTypes";
            List<string> LogTypes;

            var cachedTypes = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cachedTypes)){
                LogTypes = JsonConvert.DeserializeObject<List<string>>(cachedTypes);
            }else{
                LogTypes = this._context.Log.GroupBy(l => l.Type).Select(a => a.Key).ToList();
                var serialized = JsonConvert.SerializeObject(LogTypes);
                _cache.SetString(cacheKey, serialized, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(20)
                    });
            }


            /*

            if (!_cache.TryGetValue<List<string>>(cacheKey, out LogTypes))
            {
                // Key not in cache, so get data.
                LogTypes = this._context.Log.GroupBy(l => l.Type).Select(a => a.Key).ToList();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromDays(10));

                // Save data in cache.
                _cache.Set<List<string>>(cacheKey, LogTypes, cacheEntryOptions);
            }
             */
            return new OkObjectResult(LogTypes);
        }



        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> AddLog( [FromBody] Log log){
            if(log != null){
                var user = this.CurrentUser();
                log.User = user;
                log.Time = DateTime.Now;
                log.Agent = Request.Headers["User-Agent"].ToString();
                log.Ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
                log.ObjectType = "HttpErrorMessage";
                log.Type = "Frontend Error";
                log.Level = "Error";
                this._context.Log.Add(log);
                await _context.SaveChangesAsync();;  
            }
            return new OkObjectResult(log);
        }


        [HttpGet("GetCustom")]
            public IActionResult GetCustom( [FromQuery] string search, 
                                        [FromQuery] DateTime rangeStart, 
                                        [FromQuery] DateTime rangeEnd,
                                        [FromQuery] string type = "",
                                        [FromQuery] string amount = "0"
                                        ){
            var theAmount = Convert.ToInt32(amount);
            var logs = from i in _context.Log select i;
            if(search != null){
                logs = logs.Where( i => i.User.PersonalProfile.FirstName.Contains(search) || i.User.PersonalProfile.LastName.Contains(search));
            }
            if( rangeStart.Year != 1 && rangeEnd.Year != 1){
                logs = logs.Where( i => i.Time > rangeStart && i.Time < rangeEnd );
            }
            if( type != null ){
                logs = logs.Where( i => i.Type == type );
            }
            logs = logs.
                        OrderByDescending( l => l.Time).
                        Take(theAmount);
            foreach(var log in logs){

                if(log.UserId != null ){
                    log.User = _context.KersUser
                                    .Where( u => u.Id == log.UserId)
                                    .Include( u => u.PersonalProfile)
                                        .ThenInclude( p => p.UploadImage)
                                        .ThenInclude( i => i.UploadFile )
                                    .FirstOrDefault();
                    if(log.User.PersonalProfile.UploadImage != null){
                        log.User.PersonalProfile.UploadImage.UploadFile.Content = null;
                    }
                }
            }
            return new OkObjectResult(logs);
        }

        [HttpGet("GetCustomCount")]
        public IActionResult GetCustomCount([FromQuery] string search, [FromQuery] DateTime rangeStart, 
                                        [FromQuery] DateTime rangeEnd,
                                        [FromQuery] string type = ""){
            var logs = from i in _context.Log select i;
            if(search != null){
                logs = logs.Where( i => i.User.PersonalProfile.FirstName.Contains(search) || i.User.PersonalProfile.LastName.Contains(search));
            }
            if( rangeStart.Year != 1 && rangeEnd.Year != 1){
                logs = logs.Where( i => i.Time > rangeStart && i.Time < rangeEnd );
            }
            if( type != null ){
                logs = logs.Where( i => i.Type == type );
            }
            return new OkObjectResult(logs.Count());
        }

        
        [HttpGet("numb")]
        [Authorize]
        public IActionResult GetNumb(){
            
            var numLogs = _context.Log;
            
            return new OkObjectResult(numLogs.Count());
        }


        private KersUser userByLinkBlueId(string linkBlueId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = this._context.KersUser.
                            Where( u => u.classicReportingProfileId == profile.Id).
                            Include(u => u.RprtngProfile).
                            Include(u => u.ExtensionPosition).
                            FirstOrDefault();
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }



        private KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }

        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }



    }
}
