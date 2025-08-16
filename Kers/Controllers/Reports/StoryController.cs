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
using Kers.Models.ViewModels;
using KersData.Models;

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class StoryController : Controller
    {
        KERScoreContext context;
        IFiscalYearRepository fiscalYearRepo;
        public StoryController(
                    KERScoreContext context,
                    IFiscalYearRepository fiscalYearRepo
            )
        {
            this.context = context;
            this.fiscalYearRepo = fiscalYearRepo;
        }

        [HttpGet]
        [Route("{fy?}")]
        public async Task<IActionResult> Index(
            string currentFilter,
            string searchString,
            int? page,
            string sortOrder = "newest",
            int planningUnitId = 0,
            int programId = 0,
            int length = 18,
            string fy = "0")
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentLength"] = length;
            ViewData["Units"] = this.context.PlanningUnit.OrderBy(u => u.order).ToList();
            ViewData["Program"] = this.context.MajorProgram.Where(p => p.StrategicInitiative.FiscalYear.Name == fy).OrderBy(u => u.order).ToList();

            ViewBag.CurrentUnit = planningUnitId;
            ViewBag.CurrentProgram = programId;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (page == null) page = 1;
            if (fy == "0")
            {
                var fiscalYear = this.GetFYByName(fy);
                fy = fiscalYear.Name;
            }
            ViewData["CurrentFilter"] = searchString;

            var stories = from s in context.Story
                          select s;


            stories = stories.Where(s => s.MajorProgram.StrategicInitiative.FiscalYear.Name == fy);
            if (planningUnitId != 0)
            {
                stories = stories.Where(u => u.PlanningUnitId == planningUnitId);
            }
            if (programId != 0)
            {
                stories = stories.Where(u => u.MajorProgramId == programId);
            }


            switch (sortOrder)
            {

                case "oldest":
                    stories = stories.OrderBy(s => s.Updated);
                    break;
                case "author":
                    stories = stories
                            .OrderBy(s => s.KersUser.PersonalProfile.FirstName)
                            .ThenBy(s => s.KersUser.PersonalProfile.LastName);
                    break;
                default:
                    stories = stories.OrderByDescending(s => s.Updated);
                    break;
            }
            int pageSize = length;
            stories = stories.Include(u => u.Revisions).ThenInclude(p => p.StoryImages).ThenInclude(p => p.UploadImage).ThenInclude(i => i.UploadFile)
                        .Include(u => u.KersUser.PersonalProfile)
                        .Include(u => u.MajorProgram);

            var list = await PaginatedList<Story>.CreateAsync(stories.AsNoTracking(), page ?? 1, pageSize);
            ViewData["fy"] = fy;
            ViewData["FiscalYear"] = GetFYByName(fy);

            return View(list);
        }








        [HttpGet]
        [Route("ksu/{fy?}")]
        public async Task<IActionResult> Ksu(
            string currentFilter,
            string searchString,
            int? page,
            string sortOrder = "newest",
            int planningUnitId = 0,
            int programId = 0,
            int length = 18,
            string fy = "0")
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentLength"] = length;
            ViewData["Units"] = this.context.PlanningUnit.OrderBy(u => u.order).ToList();
            ViewData["Program"] = this.context.MajorProgram.Where(p => p.StrategicInitiative.FiscalYear.Name == fy).OrderBy(u => u.order).ToList();

            ViewBag.CurrentUnit = planningUnitId;
            ViewBag.CurrentProgram = programId;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (fy == "0")
            {
                var fiscalYear = this.GetFYByName(fy);
                fy = fiscalYear.Name;
            }
            ViewData["CurrentFilter"] = searchString;

            var stories = from s in context.Story
                          select s;


            stories = stories.Where(s => s.MajorProgram.StrategicInitiative.FiscalYear.Name == fy
                                            && s.KersUser.RprtngProfile.Institution.Name == "Kentucky State University"
                );

            if (programId != 0)
            {
                stories = stories.Where(u => u.MajorProgramId == programId);
            }


            switch (sortOrder)
            {

                case "oldest":
                    stories = stories.OrderBy(s => s.Updated);
                    break;
                case "author":
                    stories = stories
                            .OrderBy(s => s.KersUser.PersonalProfile.FirstName)
                            .ThenBy(s => s.KersUser.PersonalProfile.LastName);
                    break;
                default:
                    stories = stories.OrderByDescending(s => s.Updated);
                    break;
            }
            int pageSize = length;
            stories = stories.Include(u => u.Revisions).ThenInclude(p => p.StoryImages).ThenInclude(p => p.UploadImage).ThenInclude(i => i.UploadFile)
                        .Include(u => u.KersUser.PersonalProfile)
                        .Include(u => u.MajorProgram);

            var list = await PaginatedList<Story>.CreateAsync(stories.AsNoTracking(), page ?? 1, pageSize);
            ViewData["fy"] = fy;
            ViewData["FiscalYear"] = GetFYByName(fy);

            return View(list);
        }
















        [HttpGet]
        [Route("astable/{fy?}")]
        public async Task<IActionResult> AsTable(
            string currentFilter,
            string searchString,
            int? page,
            string sortOrder = "newest",
            int planningUnitId = 0,
            int programId = 0,
            int length = 18,
            string fy = "0")
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentLength"] = length;
            ViewData["Units"] = this.context.PlanningUnit.OrderBy(u => u.order).ToList();
            ViewData["Program"] = this.context.MajorProgram.Where(p => p.StrategicInitiative.FiscalYear.Name == fy).OrderBy(u => u.order).ToList();

            ViewBag.CurrentUnit = planningUnitId;
            ViewBag.CurrentProgram = programId;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var stories = from s in context.Story
                          select s;


            stories = stories.Where(s => s.MajorProgram.StrategicInitiative.FiscalYear.Name == fy);
            if (planningUnitId != 0)
            {
                stories = stories.Where(u => u.PlanningUnitId == planningUnitId);
            }
            if (programId != 0)
            {
                stories = stories.Where(u => u.MajorProgramId == programId);
            }


            switch (sortOrder)
            {

                case "oldest":
                    stories = stories.OrderBy(s => s.Updated);
                    break;
                case "author":
                    stories = stories
                            .OrderBy(s => s.KersUser.PersonalProfile.FirstName)
                            .ThenBy(s => s.KersUser.PersonalProfile.LastName);
                    break;
                default:
                    stories = stories.OrderByDescending(s => s.Updated);
                    break;
            }
            /* 
                        if (!String.IsNullOrEmpty(searchString))
                        {
                            stories = stories.Where(s => s.PersonalProfile.LastName.Contains(searchString)
                                                || s.PersonalProfile.FirstName.Contains(searchString));
                        }
             */
            int pageSize = length;
            stories = stories.Include(u => u.Revisions).ThenInclude(p => p.StoryImages).ThenInclude(p => p.UploadImage).ThenInclude(i => i.UploadFile)
                        .Include(u => u.KersUser.PersonalProfile)
                        .Include(u => u.MajorProgram)
                        .Include(u => u.PlanningUnit);

            var list = await PaginatedList<Story>.CreateAsync(stories.AsNoTracking(), page ?? 1, pageSize);
            ViewData["fy"] = fy;
            ViewData["FiscalYear"] = GetFYByName(fy);

            return View(list);
        }




        [HttpGet]
        [Route("s/{id}/{fy?}", Name = "ReportsFullStory")]
        public async Task<IActionResult> GetAction(int id, string fy = "0")
        {
            var story = await this.context.Story.Where(s => s.Id == id)
                                            .Include(s => s.Revisions).ThenInclude(r => r.StoryImages).ThenInclude(i => i.UploadImage).ThenInclude(m => m.UploadFile)
                                            .Include(s => s.KersUser).ThenInclude(u => u.PersonalProfile)
                                            .Include(s => s.KersUser).ThenInclude(u => u.RprtngProfile).ThenInclude(u => u.PlanningUnit)
                                            //.Include( s => s.Revisions).ThenInclude( r => r.PlanOfWork).ThenInclude( p => p.Revisions)
                                            .Include(s => s.Revisions).ThenInclude(r => r.StoryOutcome)
                                            .Include(s => s.Revisions).ThenInclude(r => r.MajorProgram)
                                                .ThenInclude(m => m.StrategicInitiative)
                                                .ThenInclude(i => i.FiscalYear)
                                            .AsSplitQuery()
                                            .FirstOrDefaultAsync();

            if (story == null)
            {
                return StatusCode(500);
            }
            var strViewModel = new StoryViewModel();
            var lastRevision = story.Revisions.OrderBy(r => r.Created).Last();
            strViewModel.Title = lastRevision.Title;
            strViewModel.Story = lastRevision.Story;
            strViewModel.KersUser = story.KersUser;
            strViewModel.MajorProgram = lastRevision.MajorProgram;
            strViewModel.PlanningUnit = story.KersUser.RprtngProfile.PlanningUnit;
            strViewModel.StoryOutcome = lastRevision.StoryOutcome;
            strViewModel.StoryId = story.Id;
            strViewModel.Updated = lastRevision.Created;
            if (lastRevision.PlanOfWorkId != 0)
            {
                strViewModel.PlanOfWork = this.context.PlanOfWorkRevision.Find(lastRevision.PlanOfWorkId);
            }
            var firstImage = lastRevision.StoryImages.OrderBy(i => i.Created).FirstOrDefault();
            if (firstImage != null)
            {
                strViewModel.ImageName = firstImage.UploadImage.UploadFile.Name;
            }
            else
            {
                strViewModel.ImageName = "";
            }
            if (fy == "0")
            {
                fy = lastRevision.MajorProgram.StrategicInitiative.FiscalYear.Name;
            }
            ViewData["fy"] = fy;
            ViewData["FiscalYear"] = GetFYByName(fy);
            return View(strViewModel);
        }

        private FiscalYear GetFYByName(string fy, string type = "serviceLog")
        {
            FiscalYear fiscalYear;
            if (fy == "0")
            {
                fiscalYear = this.fiscalYearRepo.previoiusFiscalYear(type);
            }
            else
            {
                fiscalYear = this.context.FiscalYear.Where(f => f.Name == fy && f.Type == type).FirstOrDefault();
                if (fiscalYear == null)
                {
                    fiscalYear = this.fiscalYearRepo.currentFiscalYear(type);
                }
            }
            return fiscalYear;
        }


    }
}