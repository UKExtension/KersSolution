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
    public class FeaturedPersonViewComponent : ViewComponent
    {
        private readonly KERScoreContext context;

        public FeaturedPersonViewComponent(KERScoreContext context){
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync( int PlanningUnitId = 0 ){
            

            var users = context.KersUser.Where( u => u.PersonalProfile.Bio.Length > 100 && u.PersonalProfile.Bio != null && u.PersonalProfile.UploadImage != null);

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