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

        public PeopleController( 
                    KERScoreContext context,
                    IKersUserRepository userRepo
            ){
           this.context = context;
           this.userRepo = userRepo;
        }



        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page
        )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
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
            
            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(s => s.PersonalProfile.LastName);
                    break;
                case "Date":
                    users = users.OrderBy(s => s.LastLogin);
                    break;
                case "date_desc":
                    users = users.OrderByDescending(s => s.LastLogin);
                    break;
                default:
                    users = users.OrderBy(s => s.PersonalProfile.LastName);
                    break;
            }

            int pageSize = 21;
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
                        .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit )
                        .Include( u => u.PersonalProfile).ThenInclude( p => p.UploadImage ).ThenInclude( i => i.UploadFile )
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

            return View(person);
        }
        



    }
}