using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Contexts;
using Kers.Models.Entities.UKCAReporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class TaxExemptController : ExtensionEventController
    {
        KERScoreContext _context;
        KERSmainContext _mainContext;
        KERSreportingContext _reportingContext;
        IKersUserRepository _userRepo;
        IMessageRepository messageRepo;
        ILogRepository logRepo;
        IFiscalYearRepository fiscalYearRepo;
        IWebHostEnvironment environment;
        public TaxExemptController( 
                    KERSmainContext mainContext,
                    KERSreportingContext _reportingContext,
                    KERScoreContext context,
                    IMessageRepository messageRepo,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo,
                    ITrainingRepository trainingRepo,
                    IFiscalYearRepository fiscalYearRepo,
                    IWebHostEnvironment env
            ):base(mainContext, context, userRepo){
           this._context = context;
           this._mainContext = mainContext;
           this._reportingContext = _reportingContext;
           this.messageRepo = messageRepo;
           this._userRepo = userRepo;
           this.logRepo = logRepo;
           this.fiscalYearRepo = fiscalYearRepo;
           this.environment = env;
        }


        [HttpGet("financialyears")]
        public async Task<IActionResult> FinancialYears( ){
            var yrs = this.context.TaxExemptFinancialYear.OrderBy(r => r.Order);
            return new OkObjectResult(await yrs.ToListAsync());
        }



    

    }


}