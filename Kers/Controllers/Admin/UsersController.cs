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
using System.Diagnostics;
using Kers.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Kers.Controllers.Admin
{

    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        
        IKersUserRepository userRepo;
        IzEmpRptProfileRepository profileRepo;
        KERScoreContext coreContext;
        public UsersController( 
              IKersUserRepository userRepo,
              IzEmpRptProfileRepository profileRepo,
              KERScoreContext coreContext
            ){
           this.userRepo = userRepo;
           this.profileRepo = profileRepo;
           this.coreContext = coreContext;
        }

        [HttpGet()]
        public IActionResult Get(){
            return NotFound(new {Error = "not found"});
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id){
            var user = this.coreContext.KersUser.Where(u => u.Id == id).
                            Include(u => u.PersonalProfile).
                            Include(u => u.RprtngProfile).
                            FirstOrDefault();
            return new OkObjectResult(user);
        }

        [HttpGet("byprofile/{id}")]
        public IActionResult ByProfile(int id){
            KersUser user = userRepo.findByProfileID(id);
            if(user == null){
                return NotFound(new {Error = "No user found."});
                /* 
                zEmpRptProfile profile = profileRepo.GetSingle(id);
                if( profile != null ){
                    user =  userRepo.createUserFromProfile(profile);
                }else{
                    return NotFound(new {Error = "not found"});
                }
                */
            }
            var to = userRepo.GetSingle( u => u.Id == user.Id, u => u.PersonalProfile);
            return new OkObjectResult(to);
        }

        [HttpGet("roles/{id}")]
        public IActionResult roles(int id){
            var roles = userRepo.roles(id);           
            return new OkObjectResult(roles);
        }
        [HttpGet("roleids/{id}")]
        [Authorize]
        public IActionResult roleIdsPerUser(int id){
            var roles = userRepo.roles(id);
            int[] ids = new int[roles.Count];
            var i = 0;
            foreach( var role in roles){
                ids[i] = role.Id;
                i++;
            }

            return new OkObjectResult(ids);
        }

        [HttpGet("positions")]
        [Authorize]
        public IActionResult ListPositions(){
            var positions = this.coreContext.ExtensionPosition;
            return new OkObjectResult(positions);
        }


        [HttpPut("updateroles/{id}")]
        public IActionResult UpdateRoles( int id, [FromBody] List<zEmpProfileRole> roles){
            KersUser entity = userRepo.GetSingle( u => u.Id == id, u => u.Roles);

            if(roles != null){

                var newRoles = new List<zEmpProfileRole>();
                foreach(var role in roles){
                    var rle = userRepo.roleForId(role.zEmpRoleType.Id);
                    var conn = new zEmpProfileRole();
                    conn.zEmpRoleType = rle;
                    conn.User = entity;
                    newRoles.Add(conn);
                }
                 
                entity.Roles = roles;
                this.userRepo.Commit();
                return new OkObjectResult(entity.Roles);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("personal/{id}")]
        public IActionResult UpdatePersonal( int id, [FromBody] PersonalProfile profile){
            KersUser entity = userRepo.GetSingle( u => u.Id == id, u => u.PersonalProfile);

            if(profile != null){
                
                var now = DateTime.Now;
                entity.PersonalProfile.FirstName = profile.FirstName;
                entity.PersonalProfile.LastName = profile.LastName;
                entity.PersonalProfile.OfficePhone = profile.OfficePhone;
                entity.PersonalProfile.MobilePhone = profile.MobilePhone;
                entity.PersonalProfile.Bio = profile.Bio;
                this.userRepo.Commit();
                
                return new OkObjectResult(entity.PersonalProfile);
            }else{
                return new StatusCodeResult(500);
            }
        }


/*
        [HttpGet("All")]
        public IActionResult All(){


            
            var all = this.repo.GetAll();
            
            return new OkObjectResult(all);
        }

        [HttpPost]
        public IActionResult Add( [FromBody] zEmpRoleType role){

            if(role != null){
                
                role.created = DateTime.Now;
                role.updated = DateTime.Now;
                role.enabled = true;
                this.repo.Add(role);
                this.repo.Commit();
                
                return new OkObjectResult(role);
            }else{
                return new StatusCodeResult(500);
            }
        }

        

        [HttpDelete("{id}")]
        public IActionResult Delete( int id){
            var entity = repo.GetSingle(id);

            if(entity != null){
                
                this.repo.Delete(entity);
                this.repo.Commit();
                
                return new OkResult();
            }else{
                return new StatusCodeResult(500);
            }
        }


        



        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
   
        }
 */
    }
}