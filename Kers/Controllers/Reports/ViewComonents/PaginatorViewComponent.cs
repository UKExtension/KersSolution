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
    public class PaginatorViewComponent : ViewComponent
    {


        public PaginatorViewComponent(){

        }


        public IViewComponentResult Invoke(
            Dictionary<string, string> GetParameters, string UrlString, int TotalItems, int ItemsCount, int PageIndex = 1, int PageSize = 30
        )
        {
 
             ViewData["composedString"] = UrlString + "?";
             foreach(KeyValuePair<string, string> ele in GetParameters)
                {
                    var combo = "&" + ele.Key + "=" + ele.Value; 
                    ViewData["composedString"] += combo;
                }
            ViewData["HasPreviousPage"] = PageIndex == 1 ? false : true;
            var ItemsSoFar = PageIndex * (PageSize - 1) + ItemsCount;
            ViewData["HasNextPage"] = TotalItems > ItemsSoFar;


            ViewData["TotalItems"] = TotalItems;
            ViewData["PageIndex"] = PageIndex;
            ViewData["PageSize"] = PageSize;
    


            return View();
        }


    }
}