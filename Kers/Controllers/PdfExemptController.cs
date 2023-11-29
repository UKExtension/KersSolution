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
    public class PdfExemptController : PdfBaseController
    {
		const int width = 792;
		const int height = 612;
		const int margin = 42;
		const int textLineHeight = 14;
		int[] trainingsTableLines = new int[]{ 0, 100, 143, 440, 500, 530};
		string pageTitle = "Professional Career Ladder Promotion Application";
		IFiscalYearRepository _fiscalYearRepo;


        public PdfExemptController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
			KERSmainContext mainContext,
			IMemoryCache _cache,
			IFiscalYearRepository _fiscalYearRepo
        ):
			base(_context, userRepo, mainContext, _cache){
				this._fiscalYearRepo = _fiscalYearRepo;
				


			}






		[HttpGet("exempt/{id}")]

		public IActionResult Application(int id)
        {

			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, 
							this.metadata( "Kers, Tax Exempt, Reporting", "Tax Exempt Entity", "University of Kentucky Extension Tax Exempt Entity") )) {
					var exempt = this._context.TaxExempt.Where( e => e.Id == id)
										.Include(e => e.By).ThenInclude( u => u.PersonalProfile)
										.FirstOrDefault();
					if(exempt != null){

						var pdfCanvas = document.BeginPage(height, width);
						var pageNum = 1;
						AddPageInfo(pdfCanvas, pageNum, 0, exempt.By, DateTime.Now, pageTitle);
						var positionX = margin;
						var runningY = 31;
						AddUkCaLogo(pdfCanvas, positionX, runningY+15);
						pdfCanvas.DrawText("Professional Career Ladder", 223, 62, getPaint(20.0f, 1));
						pdfCanvas.DrawText("Promotion Application", 223, 82, getPaint(20.0f, 1));
						pdfCanvas.DrawText("For Outstanding Job Performance and Experiences Gained Through Program Development", 223, 95, getPaint(7.5f));
						

						runningY += 115;
						
					

						

						document.EndPage();

					
					}else{
						Log(id,"Tax Exempt", "Tax Exempt Error", "int", "Error");
						return new StatusCodeResult(500);
					}

					document.Close();
					Log(exempt,"TaxExempt", "Tax Exempt Pdf Created", "LadderApplication");
					return File(stream.DetachAsData().AsStream(), "application/pdf", "TaxExemptEntry.pdf");	
			}

		}






	}

}
