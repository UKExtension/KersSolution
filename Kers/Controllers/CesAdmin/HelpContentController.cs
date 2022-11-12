using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;
using Kers.Models.Contexts;

namespace Kers.Controllers.Admin
{

    [Route("api/[controller]")]
    public class HelpContentController : Controller
    {
        KERScoreContext context;
        KERSmainContext mainContext;
        IHelpContentRepository repo;

        IKersUserRepository userRepo;
        public HelpContentController( 
              KERScoreContext context,
              KERSmainContext mainContext,
              IHelpContentRepository repo,
              IKersUserRepository userRepo
            ){
           this.context = context;
           this.repo = repo;
           this.userRepo = userRepo;
           this.mainContext = mainContext;
        }

        [HttpGet()]
        public IActionResult Get(){
            return NotFound(new {Error = "not found"});
        }

        [HttpGet("{id}")]

        public IActionResult Get(int id){
            var help = repo.GetSingle(id);
            return new OkObjectResult(help);
        }

        [HttpGet("All")]
        public IActionResult All(){
            var all = repo.GetAll();
            return new OkObjectResult(all);
        }

        [HttpGet("bycategory/{categoryId}")]
        public IActionResult ByCategory(int categoryId){
            var articles = this.context.HelpContent.Where( g => g.CategoryId == categoryId );
            return new OkObjectResult(articles);
        }



        [HttpPost()]
        [Authorize]
        public IActionResult AddHelp([FromBody] HelpContent help){
            if(help != null){
                help.Created = DateTime.Now;
                help.Updated = DateTime.Now;
                var user = this.CurrentUser();
                help.CreatedBy = user;
                help.LastUpdatedBy = user;
                this.context.HelpContent.Add(help);
                this.context.SaveChanges();
                
                return new OkObjectResult(help);
            }else{
                return new StatusCodeResult(500);
            }
        }


        [HttpPut("{Id}")]
        [Authorize]
        public IActionResult UpdateHelp(int Id, [FromBody] HelpContent help){
            var entity = this.context.HelpContent.Find(Id);
            if(help != null && entity != null){
     
                entity.Title = help.Title;
                entity.Body = help.Body;
                entity.CategoryId = help.CategoryId;
                entity.Updated = DateTime.Now;
                entity.LastUpdatedBy = this.CurrentUser();
                entity.EmployeePositionId = help.EmployeePositionId;
                entity.zEmpRoleTypeId = help.zEmpRoleTypeId;
                entity.isContyStaff = help.isContyStaff;
                this.context.SaveChanges();
                 
                return new OkObjectResult(entity);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteHelp( int id){
            var entity = context.HelpContent.Find(id);
            
            if(entity != null){       
                this.context.Remove(entity);
                this.context.SaveChanges();
                return new OkResult();
            }else{
                return new StatusCodeResult(500);
            }
        }


        [HttpGet("allCategories")]
        public IActionResult allCategories(int parentId){
            var categories = context.HelpCategory.Include( h => h.HelpContents).ToList();
            return new OkObjectResult(categories);
        }

        

        [HttpGet("childrenCategories/{parentId}")]
        public IActionResult childrenCategories(int parentId){
            var children = context.HelpCategory.Where(c=>c.ParentId == parentId );
            return new OkObjectResult(children);
        }

        [HttpPost("newcategory/{parentId}")]
        [Authorize]
        public IActionResult AddHelpCategory(int parentId, [FromBody] HelpCategory category){
            if(category != null){
                category.ParentId = parentId;
                category.Created = DateTime.Now;
                category.Updated = DateTime.Now;
                category.CreatedBy = this.CurrentUser();
                category.LastUpdatedBy = this.CurrentUser();
                this.context.HelpCategory.Add(category);
                this.context.SaveChanges();
                return new OkObjectResult(category);
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("updatecategory/{Id}")]
        [Authorize]
        public IActionResult UpdateHelpCategory(int Id, [FromBody] HelpCategory category){
            var entity = this.context.HelpCategory.Find(Id);
            if(category != null && entity != null){
    
                entity.Title = category.Title;
                entity.Description = category.Description;
                entity.Updated = DateTime.Now;
                entity.LastUpdatedBy = this.CurrentUser();
                entity.EmployeePositionId = category.EmployeePositionId;
                entity.zEmpRoleTypeId = category.zEmpRoleTypeId;
                entity.isContyStaff = category.isContyStaff;
                this.context.SaveChanges();
                return new OkObjectResult(entity);
            }else{
                return new StatusCodeResult(500);
            }
        }


        [HttpDelete("deletecategory/{id}")]
        [Authorize]
        public IActionResult DeleteCategory( int id){
            var entity = context.HelpCategory.Find(id);
            
            if(entity != null){   
                var children = context.HelpCategory.Where(c=>c.Parent == entity);
                foreach(var child in children){
                    this.context.Remove(child);
                }          
                this.context.Remove(entity);
                this.context.SaveChanges();
                return new OkResult();
            }else{
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



        private KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }

        

        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }


    }
}