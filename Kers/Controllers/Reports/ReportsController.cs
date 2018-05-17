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

namespace Kers.Controllers.Reports
{

    [Route("[controller]")]
    public class ReportsController : Controller
    {
        KERScoreContext context;
        IStoryRepository storyRepo;

        IContactRepository contactRepo;

        public ReportsController( 
                    KERScoreContext context,
                    IStoryRepository storyRepo,
                    IContactRepository contactRepo
            ){
           this.context = context;
           this.storyRepo = storyRepo;
           this.contactRepo = contactRepo;
        }

        public async Task<IActionResult> Index()
        {
            var StatsLastMonth = await contactRepo.StatsPerMonth();
            ViewData["StatsLastMonth"] = StatsLastMonth;
            DateTime ago = DateTime.Now.AddMonths(-2);
            var StathsTwoMonthsAgo = await contactRepo.StatsPerMonth( ago.Year, ago.Month );
            ViewData["StathsTwoMonthsAgo"] = StathsTwoMonthsAgo;
            return View();
        }
        [HttpGet]
        [Route("counties")]
        public IActionResult Counties()
        {
            var counties = this.context.PlanningUnit.
                                Where(c=>c.District != null && c.Name.Substring(c.Name.Count() - 3) == "CES").
                                OrderBy(c => c.Name).ToList();
            ViewData["Counties"] = counties;
            return View();
        }

        [HttpGet]
        [Route("county/{id}")]
        public async Task<IActionResult> County(int id)
        {

            ViewData["County"] = await this.context.PlanningUnit.Where( p => p.Id == id ).FirstOrDefaultAsync();
            var lastMonth = DateTime.Now.AddMonths(-1);
            var StatsLastMonth = await contactRepo.StatsPerMonth(lastMonth.Year,lastMonth.Month,id);
            ViewData["StatsLastMonth"] = StatsLastMonth;
            DateTime ago = DateTime.Now.AddMonths(-2);
            var StathsTwoMonthsAgo = await contactRepo.StatsPerMonth( ago.Year, ago.Month, id );
            ViewData["StathsTwoMonthsAgo"] = StathsTwoMonthsAgo;
            return View();
        }

        


    }
}