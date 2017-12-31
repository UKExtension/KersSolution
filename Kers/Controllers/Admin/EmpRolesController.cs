using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Kers.Controllers.Admin
{

    [Route("api/[controller]")]
    public class EmpRolesController : Controller
    {
        
        IzEmpRoleTypeRepository repo;
        public EmpRolesController( 
              IzEmpRoleTypeRepository repo 
            ){
           this.repo = repo;
        }

        [HttpGet()]
        public IActionResult Get(){
            return NotFound(new {Error = "not found"});
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id){
            var role = repo.GetSingle(id);
            return new OkObjectResult(role);
        }

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

        [HttpPut("{id}")]
        public IActionResult Update( int id, [FromBody] zEmpRoleType role){
            var entity = repo.GetSingle(id);

            if(role != null){
                
                var now = DateTime.Now;
                entity.updated = DateTime.Now;
                entity.enabled = role.enabled;
                entity.description = role.description;
                entity.title = role.title;
                entity.shortTitle = role.shortTitle;
                entity.selfEnrolling = role.selfEnrolling;
                this.repo.Commit();
                
                return new OkObjectResult(entity);
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

    }
}