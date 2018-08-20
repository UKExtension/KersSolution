using Kers.Models.Abstract;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kers.Controllers.Reports.ViewComponents
{
    public class FiscalYearSwitcherViewComponent : ViewComponent
    {
        private readonly KERScoreContext context;
        private IFiscalYearRepository fiscalYearRepo;

        public FiscalYearSwitcherViewComponent(
                                    KERScoreContext context,
                                    IFiscalYearRepository fiscalYearRepo
                                    ){
            this.context = context;
            this.fiscalYearRepo = fiscalYearRepo;
        }
        // selected: next, current, previous, [year name]
        // urlString replacement patern {name} | {id} will be replaced with the fiscal year name or id
        public async Task<IViewComponentResult> InvokeAsync( string type = "serviceLog", string selected = "next", bool showNext = true, string urlString = ""){
            

            var FiscalYear = context.FiscalYear.Where( f => f.Type == type);
            if( !showNext ){
                FiscalYear = FiscalYear.Where( y => y.Start <= DateTime.Now );
            }
            ViewData["urlString"] = urlString;

            ViewData["selected"] = fiscalYearRepo.currentFiscalYear(type);
            if(selected == "next"){
                ViewData["selected"] = fiscalYearRepo.nextFiscalYear( type );
            }else if( selected == "previous"){
                ViewData["selected"] = fiscalYearRepo.previoiusFiscalYear( type );
            }else{
                var yearByName = context.FiscalYear.Where( y => y.Name == selected && y.Type == type);
                if( yearByName.Any() ){
                    ViewData["selected"] = await yearByName.FirstOrDefaultAsync();
                }
            }


            return View(await FiscalYear.ToListAsync());
        }
        
    }
}