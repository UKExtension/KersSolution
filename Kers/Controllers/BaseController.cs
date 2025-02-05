using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        public KERScoreContext context;
        public KERSmainContext mainContext;
        public IKersUserRepository userRepo;
        IMemoryCache memoryCache;
        public BaseController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IMemoryCache memoryCache
            ){
           this.context = context;
           this.mainContext = mainContext;
           this.userRepo = userRepo;
           this.memoryCache = memoryCache;
        }



        public void Log(   object obj, 
                            string objectType = "ActivityRevision",
                            string description = "Submitted Service Log Revision", 
                            string type = "ServiceLog",
                            string level = "Information"
                        ){
                             
            var log = new Log();
            log.Level = level;
            log.Time = DateTime.Now;
            log.User = this.CurrentUser();
            log.ObjectType = objectType;
            log.Object = JsonConvert.SerializeObject(obj,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
            log.Agent = Request.Headers["User-Agent"].ToString();
            log.Ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            log.Description = description;
            log.Type = type;
            this.context.Log.Add(log);
            context.SaveChanges();

        }

        private KersUser userByProfileId(string profileId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.personID == profileId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = userRepo.findByProfileID(profile.Id);
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }


        private KersUser userByLinkBlueId(string linkBlueId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                //user = userRepo.findByProfileID(profile.Id);


                user = this.context.KersUser.
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


        public KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }



        protected string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public FiscalYear GetFYByName(string fy, string type = "snapEd"){
            FiscalYear fiscalYear;
            var fiscalYearCacheKey = "fiscalYearCacheKey"+fy+type;
            if (!memoryCache.TryGetValue(fiscalYearCacheKey, out fiscalYear)){
                if(fy == "0"){
                    var current = this.context.
                            FiscalYear.
                            Where(y => y.Start < DateTime.Now && y.End > DateTime.Now && y.Type == type).
                            FirstOrDefault();
                    if(current == null){
                        current = this.context.FiscalYear.Where( y => y.Name=="2025" && y.Type== type).FirstOrDefault();
                    }
                    fiscalYear = current;
                }else{
                    fiscalYear = this.context.FiscalYear.Where( f => f.Name == fy && f.Type == type).FirstOrDefault();
                }
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetAbsoluteExpiration(TimeSpan.FromHours(5));
                // Save data in cache.
                memoryCache.Set(fiscalYearCacheKey, fiscalYear, cacheEntryOptions);
            }
            return fiscalYear;
        }



    }
}