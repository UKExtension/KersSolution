using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using Kers.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kers.Controllers.Reports.ViewComponents
{
    public class eChartsViewComponent : ViewComponent
    {


        public eChartsViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync( ChartDataViewModel model = null)
        {
            
            return View(model);
        }
        
    }
}