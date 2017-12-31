using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Kers.Services.Abstract;
using Kers.Models.Abstract;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Newtonsoft.Json;

namespace Kers.Services
{

    public class MembershipService : IMembershipService
    {
        #region Variables
        private readonly KERScoreContext _coreContext;
        private readonly KERSmainContext _mainContext;
        private readonly IKersUserRepository _userRepository;

        private readonly KersUserService _kersUserService;
        private readonly ClassicUserService _classicUserService;
        
        #endregion
        public MembershipService(
            IKersUserRepository userRepository,
            KERScoreContext _coreContext, 
            KERSmainContext _mainContext
            )
        {
            _userRepository = userRepository;
            this._coreContext = _coreContext;
            this._mainContext = _mainContext;
            _kersUserService = new KersUserService(this._coreContext);
            _classicUserService = new ClassicUserService(this._mainContext, this._coreContext);
        }
 
        #region IMembershipService Implementation
        /******************************* */
        // Update KersCoreContext Kers User 
        // While Reporting Profile is still 
        // managed in the legacy system
        /******************************* */
        public KersUser RefreshKersUser(zEmpRptProfile rptProfile){
            return _kersUserService.syncUserFromProfile(rptProfile);
        }   
        /******************************* */
        // Synchronize Legacy Reporting Profile 
        // When User Management is switched to 
        // KersCoreContext
        /******************************* */
        public zEmpRptProfile RefreshRptProfile(KersUser user){
            return this._classicUserService.syncProfileFromUser(user);
        }
        public KersUser ValidateUser(string username, string password)
        {
            /*
            var membershipCtx = new MembershipContext();
 
            var user = _userRepository.GetSingleByUsername(username);
            if (user != null && isUserValid(user, password))
            {
                var userRoles = GetUserRoles(user.Username);
                membershipCtx.User = user;
 
                var identity = new GenericIdentity(user.Username);
                membershipCtx.Principal = new GenericPrincipal(
                    identity,
                    userRoles.Select(x => x.Name).ToArray());
            }
             */
            return new KersUser();
        }
        public KersUser CreateUser(string username, string email, string password, int[] roles)
        {
            /*
            var existingUser = _userRepository.GetSingleByUsername(username);
 
            if (existingUser != null)
            {
                throw new Exception("Username is already in use");
            }
 
            var passwordSalt = _encryptionService.CreateSalt();
 
            var user = new User()
            {
                Username = username,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = _encryptionService.EncryptPassword(password, passwordSalt),
                DateCreated = DateTime.Now
            };
 
            _userRepository.Add(user);
 
            _userRepository.Commit();
 
            if (roles != null || roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    addUserToRole(user, role);
                }
            }
 
            _userRepository.Commit();
             */
            return new KersUser();
        }
 
        public KersUser GetUser(int userId)
        {
            return _userRepository.GetSingle(userId);
        }
 
        public List<zEmpRoleType> GetUserRoles(KersUser user)
        {
            List<zEmpRoleType> _result = new List<zEmpRoleType>();
            /*
            var existingUser = _userRepository.GetSingleByUsername(username);
 
            if (existingUser != null)
            {
                foreach (var userRole in existingUser.UserRoles)
                {
                    _result.Add(userRole.Role);
                }
            }
  */
            return _result.Distinct().ToList();
        }
        #endregion
 
        #region Helper methods




        private void Log(   object obj, 
                            string objectType = "KersUser",
                            string description = "Membership Service", 
                            string type = "Membership",
                            string level = "Information"
                        ){
                             
            var log = new Log();
            log.Level = level;
            log.Time = DateTime.Now;
            //log.User = this.CurrentUser();
            log.ObjectType = objectType;
            log.Object = JsonConvert.SerializeObject(obj,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
            //log.Agent = Request.Headers["User-Agent"].ToString();
            //log.Ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            log.Description = description;
            log.Type = type;
            this._coreContext.Log.Add(log);
            _coreContext.SaveChanges();

        }



        /* 
        private void addUserToRole(User user, int roleId)
        {
            var role = _roleRepository.GetSingle(roleId);
            if (role == null)
                throw new Exception("Role doesn't exist.");
 
            var userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };
            _userRoleRepository.Add(userRole);
 
            _userRepository.Commit();
        }
 
        private bool isPasswordValid(User user, string password)
        {
            return string.Equals(_encryptionService.EncryptPassword(password, user.Salt), user.HashedPassword);
        }
 
        private bool isUserValid(User user, string password)
        {
            if (isPasswordValid(user, password))
            {
                return !user.IsLocked;
            }
 
            return false;
        }
        */
        #endregion
    }
}