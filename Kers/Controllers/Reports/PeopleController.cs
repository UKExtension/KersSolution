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
using Kers.Models.Data;
using Kers.Models.ViewModels;
using KersData.Models;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class PeopleController : Controller
    {
        KERScoreContext context;
        IKersUserRepository userRepo;
        IStoryRepository storyRepo;

        public PeopleController( 
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IStoryRepository storyRepo
            ){
           this.context = context;
           this.userRepo = userRepo;
           this.storyRepo = storyRepo;
        }



        [HttpGet]
        [Route("", Name = "PeopleSearch")]
        public async Task<ActionResult> Index(
            string currentFilter,
            string searchString,
            int? page,
            string sortOrder = "alphabetically",
            int planningUnitId = 0,
            int extensionPositionId = 0,
            int length = 18
        )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentLength"] = length;
            ViewData["Units"] = this.context.PlanningUnit.OrderBy( u => u.order).ToList();
            ViewData["Position"] = this.context.ExtensionPosition.OrderBy( u => u.order).ToList();

            ViewBag.CurrentUnit = planningUnitId;
            ViewBag.CurrentPosition = extensionPositionId;

            if (searchString != null)
            {
                page = 1;
            }else{
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var users = from s in context.KersUser
                        select s;
            users = users.Where( u =>u.RprtngProfile.enabled);
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.PersonalProfile.LastName.Contains(searchString)
                                    || s.PersonalProfile.FirstName.Contains(searchString));
            }
            if(planningUnitId != 0 ){
                users = users.Where( u => u.RprtngProfile.PlanningUnitId == planningUnitId );
            }
            if(extensionPositionId != 0 ){
                users = users.Where( u => u.ExtensionPositionId == extensionPositionId );
            }
            
            switch (sortOrder)
            {

                case "position":
                    users = users.OrderBy(s => s.ExtensionPosition.Title);
                    break;
                case "unit":
                    users = users.OrderBy(s => s.RprtngProfile.PlanningUnit.Name);
                    break;
                default:
                    users = users.OrderBy(s => s.PersonalProfile.FirstName).ThenBy( s => s.PersonalProfile.LastName);
                    break;
            }

            int pageSize = length;
            users = users.Include( u => u.PersonalProfile ).ThenInclude( p => p.UploadImage).ThenInclude( i => i.UploadFile )
                        .Include(u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit)
                        .Include( u => u.ExtensionPosition);

            var list = await PaginatedList<KersUser>.CreateAsync(users.AsNoTracking(), page ?? 1, pageSize);


            return View(list);

            /* 
            var searchCrigeria = new SearchCriteriaViewModel();
            searchCrigeria.Skip = 0;
            searchCrigeria.Take = length;
            searchCrigeria.OrderBy = "name";
            searchCrigeria.SearchString = SearchString;

            var users = await userRepo.Search(searchCrigeria);
 
            return View(users);*/
        }

        [HttpGet]
        [Route("person/{id}")]
        public async Task<ActionResult> Person(int id)
        {
            var person = await context.KersUser.Where( u => u.Id == id)
                        .Include( u => u.ExtensionPosition)
                        .Include( u => u.Specialties).ThenInclude( s => s.Specialty)
                        .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit )
                        .Include( u => u.PersonalProfile).ThenInclude( p => p.UploadImage ).ThenInclude( i => i.UploadFile )
                        .Include( u => u.PersonalProfile).ThenInclude( p => p.Interests ).ThenInclude( i => i.Interest)
                        .Include( u => u.PersonalProfile).ThenInclude( p => p.SocialConnections ).ThenInclude( i => i.SocialConnectionType )
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

            var stories = await this.storyRepo.LastStoriesByUser( person.Id );

            ViewData["stories"] = stories;

            return View(person);
        }
        



    }
}