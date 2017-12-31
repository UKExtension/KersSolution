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

namespace Kers.Controllers.Reports
{

    [Route("reports/[controller]")]
    public class PeopleController : Controller
    {
        KERScoreContext context;
        private IDistributedCache _cache;
        IActivityRepository activityRepo;
        IContactRepository contactRepo;
        public PeopleController( 
                    KERScoreContext context,
                    IDistributedCache _cache,
                    IActivityRepository activityRepo,
                    IContactRepository contactRepo
            ){
           this.context = context;
           this._cache = _cache;
           this.activityRepo = activityRepo;
           this.contactRepo = contactRepo;
        }



        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        



    }
}