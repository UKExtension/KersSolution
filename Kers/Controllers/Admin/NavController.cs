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
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Extensions;

namespace Kers.Controllers.Admin
{

    [Route("api/[controller]")]
    public class NavController : Controller
    {
        KERSmainContext mainContext;
        KERScoreContext context;
        INavSectionRepository repo;
        IKersUserRepository userRepo;
        KersUser user;

        IHttpContextAccessor _httpContextAccessor;
        public NavController(
              INavSectionRepository repo,
              IKersUserRepository userRepo,
              KERSmainContext mainContext,
              KERScoreContext context,
              IHttpContextAccessor _httpContextAccessor
            )
        {
            this.repo = repo;
            this.userRepo = userRepo;
            this.mainContext = mainContext;
            this.context = context;
            this._httpContextAccessor = _httpContextAccessor;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return NotFound(new { Error = "not found" });
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            var section = repo.GetSingle(id);
            return new OkObjectResult(section);
        }

        [HttpGet("All")]
        public IActionResult All()
        {

            var all = this.repo
                .AllIncludingQuery(s => s.groups)
                .Include(g => g.groups).ThenInclude(s => s.items)
                .OrderBy(s => s.order);
            return new OkObjectResult(all);
        }
        [HttpGet("user")]
        [Authorize]
        public IActionResult NavForCurrentUser()
        {

            List<NavSection> result = new List<NavSection>();


            List<NavSection> all = this.repo
                .AllIncludingQuery(s => s.groups)
                .Include(g => g.groups).ThenInclude(s => s.items)
                .OrderBy(s => s.order).ToList();
            foreach (NavSection section in all)
            {
                if (this.isCurrentUserAuthorized(section))
                {
                    List<NavGroup> resultGroups = new List<NavGroup>();
                    var g = section.groups.OrderBy(gr => gr.order).ToList();
                    foreach (NavGroup group in g)
                    {
                        if (this.isCurrentUserAuthorized(group))
                        {
                            List<NavItem> resultItems = new List<NavItem>();
                            var i = group.items.OrderBy(it => it.order).ToList();
                            foreach (NavItem item in i)
                            {
                                if (this.isCurrentUserAuthorized(item))
                                {
                                    resultItems.Add(item);
                                }
                            }
                            group.items = resultItems;
                            resultGroups.Add(group);
                        }
                    }
                    section.groups = resultGroups;
                    result.Add(section);
                }
            }
            return new OkObjectResult(result);
        }

        private bool isCurrentUserAuthorized(IEntityCredentials entity)
        {
            var result = true;
            KersUser usr = this.CurrentUser();
            if (!(entity.EmployeePositionId == null || entity.EmployeePositionId == 0))
            {
                if (!(usr.ExtensionPositionId == entity.EmployeePositionId))
                {
                    result = false;
                }

            }
            if (!(entity.zEmpRoleTypeId == null || entity.zEmpRoleTypeId == 0))
            {
                if (usr.Roles.Where(r => r.zEmpRoleTypeId == entity.zEmpRoleTypeId).FirstOrDefault() == null)
                {
                    result = false;
                }

            }
            if (!(entity.isContyStaff == null || entity.isContyStaff == 0))
            {
                if (entity.isContyStaff == 1)
                {
                    if (usr.RprtngProfile.PlanningUnit.District == null)
                    {
                        return false;
                    }
                }
                else if (entity.isContyStaff == 2)
                {
                    if (usr.RprtngProfile.PlanningUnit.District != null)
                    {
                        return false;
                    }
                }
            }

            return result;
        }

        [HttpPost("checktoken")]
        public IActionResult CheckToken([FromBody] CheckToken tok)
        {
            bool result = false;
            var usr = User;
            if (usr.IsAuthenticated()) result = true;
            return new OkObjectResult(result);
        }

        /***************************************************/
        /*   Navigation Section CRUD operations            */
        /***************************************************/

        [HttpPost("section")]
        public IActionResult AddSection([FromBody] NavSection section)
        {

            if (section != null)
            {
                var last = this.repo.GetAll().OrderByDescending(s => s.order).FirstOrDefault();
                if (section != null)
                {
                    section.order = last.order + 1;
                }
                else
                {
                    section.order = 1;
                }

                this.repo.Add(section);
                this.repo.Commit();

                return new OkObjectResult(section);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }


        [HttpPut("section/{id}")]
        public IActionResult UpdateSection(int id, [FromBody] NavSection section)
        {
            var entity = repo.GetSingle(id);

            if (section != null)
            {
                entity.name = section.name;
                entity.EmployeePositionId = section.EmployeePositionId;
                entity.zEmpRoleTypeId = section.zEmpRoleTypeId;
                entity.isContyStaff = section.isContyStaff;
                this.repo.Commit();
                return new OkObjectResult(entity);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("section/{id}")]
        public IActionResult Delete(int id)
        {
            var entity = repo.GetSingle(s => s.Id == id, s => s.groups);

            if (entity != null)
            {

                this.repo.Delete(entity);
                this.repo.Commit();

                return new OkResult();
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        /***************************************************/
        /*   Navigation Group CRUD operations              */
        /***************************************************/

        [HttpPost("group/{sectionId}")]
        public IActionResult AddGroup(int sectionId, [FromBody] NavGroup group)
        {

            if (group != null)
            {
                if (group.order == 0)
                {
                    var last = this.repo.GetSingle(s => s.Id == sectionId, s => s.groups).groups.OrderByDescending(s => s.order).FirstOrDefault();
                    if (last != null)
                    {
                        group.order = last.order + 1;
                    }
                    else
                    {
                        group.order = 1;
                    }
                }
                this.repo.GetSingle(s => s.Id == sectionId, s => s.groups).groups.Add(group);
                this.repo.Commit();

                return new OkObjectResult(group);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }


        [HttpPut("group/{id}")]
        public IActionResult UpdateGroup(int id, [FromBody] NavGroup group)
        {
            var entity = repo.groupWithId(id);

            if (group != null)
            {

                entity.name = group.name;
                entity.EmployeePositionId = group.EmployeePositionId;
                entity.zEmpRoleTypeId = group.zEmpRoleTypeId;
                entity.icon = group.icon;
                entity.isContyStaff = group.isContyStaff;
                entity.order = group.order;
                this.repo.Commit();
                return new OkObjectResult(group);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("group/{id}")]
        public IActionResult DeleteGroup(int id)
        {
            var entity = repo.groupWithIdWithItems(id);

            if (entity != null)
            {

                this.repo.deleteEntity(entity);
                this.repo.Commit();

                return new OkResult();
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        /***************************************************/
        /*   Navigation Item CRUD operations              */
        /***************************************************/

        [HttpPost("item/{groupId}")]
        public IActionResult AddItem(int groupId, [FromBody] NavItem item)
        {

            if (item != null)
            {
                if (item.order == 0)
                {
                    var last = repo.groupWithIdWithItems(groupId).items.OrderByDescending(s => s.order).FirstOrDefault();
                    if (last != null)
                    {
                        item.order = last.order + 1;
                    }
                    else
                    {
                        item.order = 1;
                    }
                }
                this.repo.groupWithIdWithItems(groupId).items.Add(item);
                this.repo.Commit();

                return new OkObjectResult(item);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }


        [HttpPut("item/{id}")]
        public IActionResult UpdateItem(int id, [FromBody] NavItem item)
        {
            var entity = repo.itemWithId(id);

            if (item != null)
            {

                entity.name = item.name;
                entity.EmployeePositionId = item.EmployeePositionId;
                entity.zEmpRoleTypeId = item.zEmpRoleTypeId;
                entity.isContyStaff = item.isContyStaff;
                entity.isRelative = item.isRelative;
                entity.route = item.route;
                entity.order = item.order;
                this.repo.Commit();
                return new OkObjectResult(entity);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("item/{id}")]
        public IActionResult DeleteItem(int id)
        {
            var entity = repo.itemWithId(id);

            if (entity != null)
            {

                this.repo.deleteEntity(entity);
                this.repo.Commit();

                return new OkResult();
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }




        private string CurrentUserId()
        {
            //var usr = _httpContextAccessor.HttpContext.User;
            //var nm = usr.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var nm = User;
            var val = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            return val;
        }

        private KersUser CurrentUser()
        {
            if (this.user == null)
            {
                var linkBlueId = this.CurrentUserId();
                this.user = this.context.KersUser.
                            Where(u => u.RprtngProfile.LinkBlueId == linkBlueId).
                            Include(u => u.RprtngProfile).ThenInclude(p => p.PlanningUnit).ThenInclude(u => u.District).
                            Include(u => u.ExtensionPosition).
                            Include(u => u.Roles).
                            FirstOrDefault();

            }
            return this.user;

        }

    }
    public class CheckToken
    {
    }
}