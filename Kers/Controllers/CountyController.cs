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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class CountyController : BaseController
    {


        private IDistributedCache _cache;
        private IMemoryCache _memoryCache;

        public CountyController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IDistributedCache _cache,
                    IMemoryCache _memoryCache,
                    IKersUserRepository userRepo
            ):base(mainContext, context, userRepo, _memoryCache){
           
                this._cache = _cache;
                this._memoryCache = _memoryCache;
        }


        [HttpGet("{id?}")]
        public IActionResult Get(int id = 0){
            if(id == 0){
                var unit = CurrentPlanningUnit();
                id = unit.Id;
            }
            var county = this.context.PlanningUnit
                                .Where(c=>c.Id == id)
                                .Include( c => c.Vehicles)
                                .FirstOrDefault();
            return new OkObjectResult(county);
        }
        [HttpGet("location/{id?}")]
        public IActionResult Location(int id = 0){
            if(id == 0){
                var unit = CurrentPlanningUnit();
                id = unit.Id;
            }
            var county = this.context.PlanningUnit
                                .Where(c=>c.Id == id)
                                .Include( c => c.Location).ThenInclude( l => l.Address)
                                .FirstOrDefault();
            return new OkObjectResult(county.Location);
        }

        [HttpGet("countylist/{DistrictId?}")]
        public async Task<IActionResult> Countylist(int? DistrictId = null){

            List<PlanningUnit> counties;


            if(DistrictId == null ){
                var simpleListCacheKey = CacheKeys.CountiesList;
                if (!_memoryCache.TryGetValue(simpleListCacheKey, out counties)){
                    counties = this.context.PlanningUnit.
                                Where(c=>c.District != null)
                                .OrderBy( c => c.order)
                                .ToList();
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(5));
                    // Save data in cache.
                    _memoryCache.Set(simpleListCacheKey, counties, cacheEntryOptions);
                }
            
                return new OkObjectResult(counties);
            }

           var CurrentPlanningUnit = this.CurrentPlanningUnit();
            
            var cacheKey = CacheKeys.CountiesList + DistrictId.ToString() + CurrentPlanningUnit.Name;
            var cached = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cached)){
                counties = JsonConvert.DeserializeObject<List<PlanningUnit>>(cached);
            }else{
            
                var countiesQuery = this.context.PlanningUnit.
                                Where(c=>c.District != null 
                                //&& c.Name.Substring(c.Name.Count() - 3) == "CES"
                                );
                
                if(DistrictId != null){
                    if(DistrictId == 0){
                        
                        if(CurrentPlanningUnit.DistrictId != null){
                            countiesQuery = countiesQuery.Where( c => c.DistrictId == CurrentPlanningUnit.DistrictId);
                        }else{
                            countiesQuery = countiesQuery.Where( c => false);
                        }
                    }else{
                        countiesQuery = countiesQuery.Where( c => c.DistrictId == DistrictId);
                    }
                } 
                counties = await countiesQuery.OrderBy(c => c.Name).ToListAsync();
                
                counties = counties.Where( c => c.Name.Substring(c.Name.Count() - 3) == "CES").ToList();
                var serializedCounties = JsonConvert.SerializeObject(counties);
                _cache.SetString(cacheKey, serializedCounties, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
                        });
            }
            return new OkObjectResult(counties);
        }


        [HttpGet("timezones")]
        public IActionResult Timezones(){
            var zones = TimeZoneInfo.GetSystemTimeZones();
            return new OkObjectResult(zones);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCounty( int id, [FromBody] PlanningUnit unit){
           
            
            var entity = context.PlanningUnit.Find(id);
            
            if(entity != null && unit != null){
                

                entity.Name = unit.Name;
                entity.Code = unit.Code;
                entity.Description = unit.Description;
                entity.FullName = unit.FullName;
                entity.Address = unit.Address;
                entity.Zip = unit.Zip;
                entity.City = unit.City;
                entity.Phone = unit.Phone;
                entity.WebSite = unit.WebSite;
                entity.Email = unit.Email;
                entity.TimeZoneId = unit.TimeZoneId;
                entity.Population = unit.Population;
                entity.FIPSCode = unit.FIPSCode;

                var location = context.ExtensionEventLocation.Where( l => l.Id == entity.LocationId ).Include( l => l.Address).FirstOrDefault();
                if(location == null){
                    location = new ExtensionEventLocation();
                    location.Address = new PhysicalAddress();
                    location.Address.State = "Kentucky";
                    location.Address.CountryOrRegion = "USA";
                    location.DisplayName = "County Extension Office";
                    entity.Location = location;
                }
                location.Address.City = unit.City;
                location.Address.Street = unit.Address;
                location.Address.PostalCode = unit.Zip;
                location.Address.Building = unit.Name;

                context.SaveChanges();
                _cache.Remove(CacheKeys.CountiesList);
                this.Log( unit ,"PlanningUnit", "Planning Unit Updated."); 
                return new OkObjectResult(entity);
            }else{
                this.Log( unit ,"PlanningUnit", "Not Found PlanningUnit in an update attempt.", "Success Story", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPost("vehicle")]
        [Authorize]
        public IActionResult AddVehicle( [FromBody] CountyVehicle vehicle){
            if(vehicle != null){
                var user = this.CurrentUser();
                vehicle.AddedBy = user;
                vehicle.CreatedDateTime = DateTimeOffset.Now;
                vehicle.LastModifiedDateTime = DateTimeOffset.Now;
                context.Add(vehicle);  
                this.Log(vehicle,"CountyVehicle", "County Vehicle Added.");
                context.SaveChanges();
                return new OkObjectResult(vehicle);
            }else{
                this.Log( vehicle ,"CountyVehicle", "Error in adding County Vehicle attempt.", "Expense", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("vehicle/{id}")]
        public IActionResult UpdateVehicle( int id, [FromBody] CountyVehicle vehicle){
           
            
            var entity = context.CountyVehicle.Find(id);

            if(entity != null){
                entity.Make = vehicle.Make;
                entity.Name = vehicle.Name;
                entity.Model = vehicle.Model;
                entity.LicenseTag = vehicle.LicenseTag;
                entity.Color = vehicle.Color;
                entity.Odometer = vehicle.Odometer;
                entity.EndingOdometer = vehicle.EndingOdometer;
                entity.PurchasePrice = vehicle.PurchasePrice;
                entity.Year = vehicle.Year;
                entity.Enabled = vehicle.Enabled;
                entity.DatePurchased = vehicle.DatePurchased;
                entity.DateDispossed = vehicle.DateDispossed;
                entity.LastModifiedDateTime = DateTimeOffset.Now;
                entity.UploadImageId = vehicle.UploadImageId == 0 ? null : vehicle.UploadImageId;
                entity.Comments = vehicle.Comments;
                context.SaveChanges();
                this.Log(entity,"CountyVehicle", "CountyVehicle Updated.");
                return new OkObjectResult(entity);
            }else{
                this.Log( vehicle ,"CountyVehicle", "Not Found CountyVehicle in update attempt.", "CountyVehicle", "Error");
                return new StatusCodeResult(500);
            }
        }


        private KersUser userByLinkBlueId(string linkBlueId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
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

        private PlanningUnit CurrentPlanningUnit(){
            var u = this.CurrentUserId();
            var unitId = context.ReportingProfile.Where( p => p.LinkBlueId == u).Select( p => p.PlanningUnitId).FirstOrDefault();
            return  this.context.PlanningUnit.
                    Where( p=>p.Id == unitId).
                    Include( p => p.Vehicles).
                    FirstOrDefault();
        }
    }
}