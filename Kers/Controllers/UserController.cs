using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using Kers.Services;
using Kers.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features;
using System.Net.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private KERSmainContext _mContext;
        private KERScoreContext _context;
        private IMembershipService _service;
        private IDistributedCache _cache;
        IFiscalYearRepository fiscalYearRepo;
        ITrainingRepository trainingRepository;
        private int DefaultNumberOfItems{
            get
            {
                return 18;
            }
        }

        public UserController( 
                                KERSmainContext _mContext,
                                KERScoreContext _context,
                                IDistributedCache _cache,
                                IMembershipService _service,
                                IFiscalYearRepository fiscalYearRepo,
                                ITrainingRepository trainingRepository
                            ){
            this._mContext = _mContext;
            this._context = _context;
            this._service = _service;
            this._cache = _cache;
            this.fiscalYearRepo = fiscalYearRepo;
            this.trainingRepository = trainingRepository;
        }

        [HttpGet()]
        public IActionResult Get(){
            return NotFound(new {Error = "not found"});
        }






        [HttpGet("current")]
        [Authorize]
        public IActionResult Current(){
            var curentUserId = CurrentUserId();
            var user = _context.KersUser.
                            Where(u => u.RprtngProfile.LinkBlueId == curentUserId).
                            Include(u => u.PersonalProfile).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.GeneralLocation).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.Institution).
                            Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                            Include(u => u.ExtensionPosition).
                            Include(u => u.Specialties).ThenInclude(s=>s.Specialty).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.Interests).ThenInclude(i=>i.Interest).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.SocialConnections).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.UploadImage).ThenInclude(i=>i.UploadFile).
                            Include(u=>u.Roles).ThenInclude( l => l.zEmpRoleType).
                            AsSplitQuery().
                            FirstOrDefault();
            if(user == null){
                var rprtProfile = _mContext.zEmpRptProfiles.Where(p=>p.linkBlueID == curentUserId).FirstOrDefault();
                user = _service.RefreshKersUser(rprtProfile);
            }
            if(user.PersonalProfile != null && user.PersonalProfile.UploadImage != null){
                user.PersonalProfile.UploadImage.UploadFile.Content = null;
            }
            return new OkObjectResult(user);
        }

        [HttpGet("{id}")]
        public IActionResult UserById(int id){

            var user = _context.KersUser.
                            Where(u => u.classicReportingProfileId == id).
                            Include(u => u.PersonalProfile).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.Interests).ThenInclude(i=>i.Interest).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.SocialConnections).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.GeneralLocation).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.Institution).
                            Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                            Include(u => u.ExtensionPosition).
                            Include(u => u.Specialties).ThenInclude(s=>s.Specialty).
                            Include(u=>u.Roles).
                            FirstOrDefault();
            if(user==null || user.RprtngProfile==null){
                user = _service.RefreshKersUser(_mContext.zEmpRptProfiles.Find(id));
            }
            return new OkObjectResult(user);
            
        }

        [HttpGet("id/{id}")]
        public IActionResult UserByKersUserId(int id){

            var user = _context.KersUser.
                            Where(u => u.Id == id).
                            Include(u => u.PersonalProfile).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.Interests).ThenInclude(i=>i.Interest).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.SocialConnections).ThenInclude(c => c.SocialConnectionType).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.UploadImage).ThenInclude(i => i.UploadFile).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.GeneralLocation).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.Institution).
                            Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                            Include(u => u.ExtensionPosition).
                            Include(u => u.Specialties).ThenInclude(s=>s.Specialty).
                            Include(u=>u.Roles).
                            FirstOrDefault();
            if(user==null || user.RprtngProfile==null){
                user = _service.RefreshKersUser(_mContext.zEmpRptProfiles.Find(id));
            }else{
                if(user.PersonalProfile.UploadImage != null){
                    user.PersonalProfile.UploadImage.UploadFile.Content = null;
                }
            }
            return new OkObjectResult(user);
            
        }



        [HttpGet("GetCustom")]
        public IActionResult GetCustom( [FromQuery] string search, 
                                        [FromQuery] string unit = "0", 
                                        [FromQuery] string position = "0",
                                        [FromQuery] string amount = "0",
                                        [FromQuery] string snapAssistants = "0",
                                        [FromQuery] string withSnapCommitment = "0",
                                        [FromQuery] string onlyKSU = "0",
                                        [FromQuery] string enabled = "0"
                                        ){
            var theAmount = Convert.ToInt32(amount);
            theAmount =  theAmount <= 0 ? DefaultNumberOfItems : theAmount ;

            var users = from i in _context.KersUser select i;



            if(search != null){
                users = users.Where( i => i.RprtngProfile.Name.Contains(search));
            }
            if( unit != "0" ){
                users = users.Where( i => i.RprtngProfile.PlanningUnitId == Convert.ToInt32(unit) );
            }
            if( position != "0" ){
                users = users.Where( i => i.ExtensionPositionId == Convert.ToInt32(position) );
            }
            if(withSnapCommitment != "0"){
                var commitmentIds = this._context.SnapEd_Commitment.Select( r => r.KersUserId1).ToList();
                users = users.Where( r => commitmentIds.Contains( r.Id ));
            }
            if(snapAssistants != "0"){
                users = users.Where(c=> c.Specialties.Where(s => s.Specialty.Name == "Expanded Food and Nutrition Education Program").Count() != 0 
                                    ||
                                    c.Specialties.Where(s => s.Specialty.Name == "Supplemental Nutrition Assistance Program Education").Count() != 0
                                 );
            }
            if( onlyKSU != "0" ){
                users = users.Where( i => i.RprtngProfile.Institution.Name == "Kentucky State University" );
            }
            if( enabled != "0" ){
                users = users.Where( i => i.RprtngProfile.enabled == true );
            }
            
            users = users.Include(i => i.PersonalProfile).ThenInclude(i=>i.UploadImage).ThenInclude( i => i.UploadFile).
                            Include( i => i.RprtngProfile).ThenInclude( r => r.PlanningUnit).
                            Include( i=> i.ExtensionPosition);
            users = users.OrderByDescending(i => i.LastLogin);
            users = users.Take(theAmount);
            foreach(var user in users){
                if(user.PersonalProfile != null){
                    if(user.PersonalProfile.UploadImage != null){
                        user.PersonalProfile.UploadImage.UploadFile.Content = null;
                    }
                }
            }
            return new OkObjectResult(users);
        }

        [HttpGet("GetCustomCount")]
        public IActionResult GetCustomCount([FromQuery] string search, [FromQuery] string unit = "0", [FromQuery] string position = "0"){
            var users = from i in _context.KersUser select i;
            if(search != null){
                users = users.Where( i => i.RprtngProfile.Name.Contains(search));
            }
            if( unit != "0" ){
                users = users.Where( i => i.RprtngProfile.PlanningUnitId == Convert.ToInt32(unit) );
            }
            if( position != "0" ){
                users = users.Where( i => i.ExtensionPositionId == Convert.ToInt32(position) );
            }
            return new OkObjectResult(users.Count());
        }


        [HttpGet("startdate/{linkBlueId}")]
        public IActionResult StartDate(string linkBlueId){
            var user = _mContext.SAP_HR_ACTIVE.Where(u => u.Userid == linkBlueId).FirstOrDefault();
            if(user != null){
                return new OkObjectResult(user.BeginDate);
            }
            return new OkObjectResult(null);
        }

        [HttpGet("isItExists/{linkBlueId}")]
        public IActionResult isItExists(string linkBlueId){
            object returnValue = null;
            if(_mContext.zEmpRptProfiles.Where( z => z.linkBlueID == linkBlueId).Any()){
                returnValue = new { linkBlueEsists = true };
            }
            return new OkObjectResult( returnValue );
        }

        [HttpGet("isPersonIdExists/{personId}")]
        public IActionResult isPersonIdExists(string personId){
            object returnValue = null;
            if(_mContext.zEmpRptProfiles.Where( z => z.personID == personId).Any()){
                returnValue = new { personId = true };
            }
            return new OkObjectResult( returnValue );
        }
        

        [HttpGet("PlanningUnit")]
        public IActionResult PlanningUnit(){
            var units = _context.PlanningUnit.OrderBy(i=>i.Name);
            return new OkObjectResult(units);
        }

        [HttpGet("Position")]
        public IActionResult Position(){
            var units = _context.ExtensionPosition.OrderBy(i=>i.Title);
            return new OkObjectResult(units);
        }


        [HttpPost()]
        public IActionResult AddUser([FromBody] KersUser user){
            if(user != null){

                user.Created = DateTime.Now;
                user.Updated = DateTime.Now;
                user.LastLogin = DateTime.Now;
                user.RprtngProfile.enabled = true;
                if(user.RprtngProfile.InstitutionId == 0 ){
                    user.RprtngProfile.InstitutionId = 1;
                }
                this._context.KersUser.Add(user);
                this._context.SaveChanges();
                var rprtProfile = _service.RefreshRptProfile(user);
                user.classicReportingProfileId = rprtProfile.Id;
                user.PersonalProfile = new PersonalProfile();
                this.populatePersonalProfileName(user.PersonalProfile, user.RprtngProfile.Name);
                if( user.RprtngProfile.PlanningUnit != null) user.PersonalProfile.TimeZoneId = user.RprtngProfile.PlanningUnit.TimeZoneId;
                this._context.SaveChanges();
                /*
                var currentUserId = CurrentUserId();
                
                if( user.RprtngProfile.LinkBlueId != currentUserId ){
                    var currentUser = this._context.KersUser.Where( u => u.RprtngProfile.LinkBlueId == currentUserId).FirstOrDefault();
                    this.Log( user, currentUser, "KersUser", "Admin New User Created", "User Profile Created");
                }else{
                    */
                    this.Log(user, user, "KersUser", "New User Created", "User Profile Created");
                //}
                return new OkObjectResult(user);
            }else{
                return new StatusCodeResult(500);
            }
        }

        private void populatePersonalProfileName(PersonalProfile personal, string name){
            char[] delimiterChars = { ',', ' ' };
            var splitName = name.Split(delimiterChars);
            personal.FirstName = splitName[2];
            personal.LastName = splitName[0];
        }


        [HttpPut("{Id}")]
        [Authorize]
        public IActionResult UpdateUser(int Id, [FromBody] KersUser user){
            var entity = _context.KersUser.
                            Where(u => u.Id == Id).
                            Include(u => u.PersonalProfile).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.GeneralLocation).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.Institution).
                            Include(u => u.RprtngProfile).ThenInclude(r=>r.PlanningUnit).
                            Include(u => u.ExtensionPosition).
                            Include(u => u.Specialties).ThenInclude(s=>s.Specialty).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.Interests).ThenInclude(i=>i.Interest).
                            Include(u=>u.PersonalProfile).ThenInclude(u=>u.SocialConnections).
                            FirstOrDefault();
            if(user != null && entity != null){
                if(user.RprtngProfile != null){
                    if(user.RprtngProfile.Name != null){
                        entity.RprtngProfile.Name = user.RprtngProfile.Name;
                        entity.RprtngProfile.InstitutionId = user.RprtngProfile.InstitutionId;
                        entity.RprtngProfile.enabled = user.RprtngProfile.enabled;
                    }
                    entity.ExtensionPositionId = user.ExtensionPositionId;
                    entity.Specialties = user.Specialties;
                    entity.RprtngProfile.Email = user.RprtngProfile.Email;
                    entity.RprtngProfile.EmailAlias = user.RprtngProfile.EmailAlias;
                    entity.RprtngProfile.PlanningUnitId = user.RprtngProfile.PlanningUnitId;
                    entity.RprtngProfile.GeneralLocationId = user.RprtngProfile.GeneralLocationId;
                }
                if(user.PersonalProfile != null){
                    //if(user.PersonalProfile.UploadImageId != null && user.PersonalProfile.UploadImageId != 0){
                        entity.PersonalProfile.UploadImageId = user.PersonalProfile.UploadImageId;
                    //}
                    entity.PersonalProfile.FirstName = user.PersonalProfile.FirstName;
                    entity.PersonalProfile.LastName = user.PersonalProfile.LastName;
                    entity.PersonalProfile.ProfessionalTitle = user.PersonalProfile.ProfessionalTitle;
                    entity.PersonalProfile.OfficePhone = user.PersonalProfile.OfficePhone;
                    entity.PersonalProfile.MobilePhone = user.PersonalProfile.MobilePhone;
                    entity.PersonalProfile.OfficeAddress = user.PersonalProfile.OfficeAddress;
                    entity.PersonalProfile.TimeZoneId = user.PersonalProfile.TimeZoneId;
                    entity.PersonalProfile.Bio = user.PersonalProfile.Bio;
                    var intersts = new List<InterestProfile>();
                    foreach(var intrst in user.PersonalProfile.Interests){
                        var intr = _context.Interest.
                                        Where(i=>i.Name == intrst.Interest.Name).
                                        FirstOrDefault();
                        if(intr != null){
                            var ipr = new InterestProfile();
                            ipr .PersonalProfile = entity.PersonalProfile;
                            ipr.Interest = intr;
                            intersts.Add(ipr);
                        }else{
                            var i = new Interest();
                            i.Name = intrst.Interest.Name;
                            i.Created = DateTime.Now;
                            var ipr = new InterestProfile();
                            ipr.Interest = i;
                            ipr.PersonalProfile = entity.PersonalProfile;

                            intersts.Add(ipr);
                        }
                    }
                    foreach(var print in entity.PersonalProfile.Interests){
                        _context.Remove(print);
                    }
                    entity.PersonalProfile.Interests = intersts;
                    foreach(var sccn in entity.PersonalProfile.SocialConnections){
                        _context.Remove(sccn);
                    }
                    var conns = new List<SocialConnection>();
                    foreach(var sc in user.PersonalProfile.SocialConnections){
                        if(sc.Identifier != "" || sc.SocialConnectionTypeId != 0){
                            conns.Add(sc);
                        }
                    }
                    entity.PersonalProfile.SocialConnections = conns;
                }
                entity.Updated = DateTime.Now;
                this._context.SaveChanges();
                if(user.RprtngProfile != null){
                    _service.RefreshRptProfile(entity);
                }
                var currentUserId = CurrentUserId();
                if( entity.RprtngProfile.LinkBlueId != currentUserId ){
                    var currentUser = this._context.KersUser.Where( u => u.RprtngProfile.LinkBlueId == currentUserId).FirstOrDefault();
                    this.Log( entity, currentUser, "KersUser", "Admin Updated User Information");
                }else{
                    this.Log(entity, entity);
                }
                
                return new OkObjectResult(entity);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("connections")]
        public IActionResult Connections(){
            var conn = _context.SocialConnectionType.ToList();
            return new OkObjectResult(conn);
        }


        [HttpGet("positions")]
        public IActionResult Positions(){
            var pos = _context.ExtensionPosition.ToList();
            return new OkObjectResult(pos);
        }

        [HttpGet("specialties")]
        public IActionResult Specialties(){
            var spclt = _context.Specialty.ToList();
            return new OkObjectResult(spclt);
        }

        [HttpGet("locations")]
        public IActionResult Locations(){
            var loctns = _context.GeneralLocation.ToList();
            return new OkObjectResult(loctns);
        }

        [HttpGet("units")]
        public async Task<IActionResult> Units(){

            List<PlanningUnit> units;
            var cacheKey = "AllPlanningUnits";
            var cached = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cached)){
                units = JsonConvert.DeserializeObject<List<PlanningUnit>>(cached);
            }else{

                units = await _context.PlanningUnit.OrderBy(u => u.order).ToListAsync();
                var serializedUnits = JsonConvert.SerializeObject(units);
                _cache.SetString(cacheKey, serializedUnits, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(20)
                    });
            }
            return new OkObjectResult(units);
        }

        [HttpGet("institutions")]
        public IActionResult Institutions(){
            var insts = _context.Institution.ToList();
            return new OkObjectResult(insts);
        }


        [HttpGet("tags")]
        public IActionResult Tags([FromQuery]string q){
            var ints = _context.Interest.Where(i =>i.Name.Contains(q)).Take(15) ;
            var res = new List<string>();
            foreach( var i in ints){
                res.Add(i.Name);
            }
            return new OkObjectResult(res);
        }

        [HttpGet("userswithrole/{role}")]
        public async Task<IActionResult> UsersWithRole(string role){

            var users = from user in _context.KersUser
                from roles in user.Roles
                where roles.zEmpRoleType.shortTitle == role && user.RprtngProfile.enabled
                select user;

            users = users.Include( u => u.PersonalProfile)
                        .Include( u => u.RprtngProfile)
                            .ThenInclude( r => r.PlanningUnit);
            return new OkObjectResult(await users.ToListAsync());
        }

        [HttpGet("unitemployees/{unitId}")]
        [Authorize]
        public async Task<IActionResult> UnitEmployees(int unitId = 0){

            if( unitId == 0){
                var user = await this._context.KersUser.Where( u => u.RprtngProfile.LinkBlueId == this.CurrentUserId())
                                    .Include( u => u.RprtngProfile).FirstOrDefaultAsync();
                if( user == null) return new StatusCodeResult(500);
                unitId = user.RprtngProfile.PlanningUnitId;
            }

            var users = this._context.KersUser.Where( u => u.RprtngProfile.PlanningUnitId == unitId)
                                .Include(u => u.RprtngProfile)
                                .Include( u => u.PersonalProfile)
                                .Include( u => u.ExtensionPosition)
                                .Include( u => u.Specialties).ThenInclude( s => s.Specialty);
            return new OkObjectResult(await users.ToListAsync());
        }



        [HttpGet("InServiceEnrolment/{userId?}/{fy?}")]
        public IActionResult InServiceEnrolment(int userId=0, string fy = "0"){
            KersUser user;
            if(userId == 0){
                var curentUserId = CurrentUserId();
                user = this._context.KersUser.Where(u => u.RprtngProfile.LinkBlueId == curentUserId).Include(u => u.RprtngProfile).FirstOrDefault();
            }else{
                user = this._context.KersUser.Where(u => u.Id == userId).Include(u => u.RprtngProfile).FirstOrDefault();
            }

            var fiscalYear = GetFYByName(fy);
            var start = fiscalYear.Start.ToString("yyyyMMdd");
            var end = fiscalYear.End.ToString("yyyyMMdd");
            

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            String uri = "https://kers.ca.uky.edu/kers_mobile/HandlerInService.ashx?Id=" + user.RprtngProfile.PersonId + "&Start=" + start + "&End=" + end;



            var result = client.GetAsync(uri).Result;
            var data = result.Content.ReadAsStringAsync().Result;

            return new OkObjectResult(data);
        }

        [HttpGet("TrainingsEnrolment/{userId}/{year}")]
        public IActionResult TrainingsEnrolment(int userId, int year = 2019){
            return new OkObjectResult(trainingRepository.trainingsPerPersonPerYear(userId, year));
        }



        private void Log(   object obj, 
                            KersUser user,
                            string objectType = "KersUser",
                            string description = "Updated User Information", 
                            string type = "User Profile Update",
                            string level = "Information"
                        ){
                             
            var log = new Log();
            log.Level = level;
            log.Time = DateTime.Now;
            log.User = user;
            log.ObjectType = objectType;
            log.Object = JsonConvert.SerializeObject(obj,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
            log.Agent = Request.Headers["User-Agent"].ToString();
            log.Ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            log.Description = description;
            log.Type = type;
            this._context.Log.Add(log);
            this._context.SaveChanges();

        }



        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        private FiscalYear GetFYByName(string fy, string type = "serviceLog"){
            FiscalYear fiscalYear;
            if(fy == "0"){
                fiscalYear = this.fiscalYearRepo.previoiusFiscalYear(type);
            }else{
                fiscalYear = this._context.FiscalYear.Where( f => f.Name == fy && f.Type == type).FirstOrDefault();
                if(fiscalYear == null ){
                    fiscalYear = this.fiscalYearRepo.currentFiscalYear(type);
                }
            }
            return fiscalYear;
        }
        
    }
}