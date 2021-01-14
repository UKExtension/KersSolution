using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kers.Models.Abstract;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SkiaSharp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.RegularExpressions;

namespace Kers.Controllers
{
	[Route("api/[controller]")]
    public class PdfLadderController : PdfBaseController
    {
		const int width = 792;
		const int height = 612;


        public PdfLadderController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
			KERSmainContext mainContext,
			IMemoryCache _cache
        ):
			base(_context, userRepo, mainContext, _cache){
				


			}






		[HttpGet("application/{id}")]

		public IActionResult Application(int id)
        {

			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata( "Kers, Career Ladder, Reporting", "Career Ladder Application", "Professional Career Ladder Promotion Application") )) {
				
					


					


					var application = _context.LadderApplication
											.Where(a => a.Id == id)
											.Include( a => a.KersUser).ThenInclude( u => u.RprtngProfile).ThenInclude( u => u.PlanningUnit).ThenInclude( n => n.ExtensionArea)
											.Include( a => a.KersUser).ThenInclude( u => u.PersonalProfile)
											.FirstOrDefault();
					if( application != null){
						var pdfCanvas = document.BeginPage(height, width);
						var pageNum = 1;
						AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, "Professional Career Ladder Promotion Application");
						var positionX = 42;
						var runningY = 31;
						AddUkLogo(pdfCanvas, 16, runningY);
						pdfCanvas.DrawText("Professional Career Ladder", 223, 62, getPaint(20.0f, 1));
						pdfCanvas.DrawText("Promotion Application", 223, 82, getPaint(20.0f, 1));
						pdfCanvas.DrawText("For Outstanding Job Performance and Experiences Gained Through Program Development", 223, 95, getPaint(7.5f));
						

						runningY += 115;
						pdfCanvas.DrawText(application.KersUser.PersonalProfile.FirstName + " " +application.KersUser.PersonalProfile.LastName, positionX, runningY, getPaint(18f, 1));
						var textCounty = application.KersUser.RprtngProfile.PlanningUnit.Name;
						if( textCounty.Count() > 15 ){
							textCounty = textCounty.Substring(0, application.KersUser.RprtngProfile.PlanningUnit.Name.Count() - 11);
						}
						pdfCanvas.DrawText("County: " + textCounty, 300, runningY - 5, getPaint(10f));
						var textArea = "";
						if( application.KersUser.RprtngProfile.PlanningUnit.ExtensionArea != null){
							textArea = application.KersUser.RprtngProfile.PlanningUnit.ExtensionArea.Name;
						}
						pdfCanvas.DrawText("Extension Area: " + textArea, 300, runningY + 10, getPaint(10f));
						runningY += 12;
						pdfCanvas.DrawText("UK/person ID: " + application.KersUser.RprtngProfile.PersonId, positionX, runningY, getPaint(8.5f));



						
						document.EndPage();
					
					}else{
						//Log(id,"LadderApplication", "Career Ladder Error", "int", "Error");
						return new StatusCodeResult(500);
					}







					
					

					document.Close();
					
					return File(stream.DetachAsData().AsStream(), "application/pdf", "CareerLadderApplication.pdf");	
			}

		}






	}

}
