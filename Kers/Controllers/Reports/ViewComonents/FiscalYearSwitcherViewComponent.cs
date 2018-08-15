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

        public FiscalYearSwitcherViewComponent(KERScoreContext context){
            this.context = context;
        }
        // initially: next, current, previous
        public async Task<IViewComponentResult> InvokeAsync( string type = "serviceLog", string initially = "next", bool showNext = true){
            

            var users = context.KersUser.Where( u => u.PersonalProfile.Bio != null && u.PersonalProfile.Bio.Length > 100 && u.PersonalProfile.UploadImage != null);

            if( PlanningUnitId != 0 ){
                users = users.Where( u => u.RprtngProfile.PlanningUnitId == PlanningUnitId);
            }

            int total = users.Count();
            Random rndm = new Random();
            int offset = rndm.Next(0, total);

            users = users
                        .Include( u => u.PersonalProfile ).ThenInclude( p => p.UploadImage ).ThenInclude( i => i.UploadFile )
                        .Include( u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit )
                        .Include( u => u.ExtensionPosition);

            var user = await users.Skip(offset).FirstOrDefaultAsync();

            return View(user);
        }
        
    }
}