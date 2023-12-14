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
										.Include( e => e.Unit)
										.Include( e => e.TaxExemptFinancialYear)
										.Include( e => e.TaxExemptProgramCategories).ThenInclude( p => p.TaxExemptProgramCategory)
										.Include( e => e.Areas).ThenInclude( a => a.Unit)
										.Include( e => e.Handled)
										.Include( e => e.OrganizationResides)
										.FirstOrDefault();
					if(exempt != null){
						var pdfCanvas = document.BeginPage(height, width);
						pdfCanvas.DrawText("Report Generated: ", 43, 770, getPaint(9.0f, 1));
						pdfCanvas.DrawText( DateTime.Now.ToString(), 125, 770, getPaint(9.0f));
						var positionX = margin;
						var runningY = 31;
						AddUkCaLogo(pdfCanvas, positionX, runningY+15);
						pdfCanvas.DrawText("Tax Exempt/Volunteer Entity", 223, 79, getPaint(22.0f, 1));
						pdfCanvas.DrawText("For Kentucky Cooperative Extension volunteer-led entity that handles money (have income and expenses).", 223, 95, getPaint(7.4f));
						

						runningY += 135;
						var secondRowX = positionX + 170;
						var lineHight = 18;
						var lineOffset = 6;


						pdfCanvas.DrawText("Entity record database ID:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.Id.ToString(), secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Entity record last updated:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.Updated.ToString(), secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("County/Unit:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.Unit.Name, secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Official Name of Entity:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.Name??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Entity's EIN or FIN number:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.Ein??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Name of bank where account is located:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.BankName??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Name of the bank account:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.BankAccountName??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Entity's financial year:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.TaxExemptFinancialYear.Name, secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Program area(s) represented by this entity:", positionX, runningY, getPaint(7.5f));
						var categories = exempt.TaxExemptProgramCategories.Select( c => c.TaxExemptProgramCategory.Name);
						pdfCanvas.DrawText(string.Join( ", ", categories), secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Counties represented by this entity:", positionX, runningY, getPaint(7.5f));
						var counties = exempt.Areas.OrderBy( c => c.Unit.order).Select( c => c.Unit.Name.Substring(0, c.Unit.Name.Length - 11)).ToList();
						counties.Insert(0, exempt.Unit.Name.Substring( 0, exempt.Unit.Name.Length - 11) );
						pdfCanvas.DrawText(string.Join(", ", counties), secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Written acknowledgement to donors by Jan 31:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.DonorsReceivedAck??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						runningY += lineHight;
						runningY += lineHight;
						pdfCanvas.DrawText("Dates on which the specific information was last submitted.", positionX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thickLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Annual Budget:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.AnnBudget??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Annual Financial Report:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.AnnFinancialRpt??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Annual Audit Report:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.AnnAuditRpt??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Annual Inventory Report:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(exempt.AnnInvRpt??"", secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("Tax exempt status derived from:", positionX, runningY, getPaint(7.5f));
						pdfCanvas.DrawText(( exempt.Handled.Is501 ? "501(c) Organization" : "County Extension District"), secondRowX, runningY, getPaint(7.5f, 1));
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						pdfCanvas.DrawText("How funds are handled:", positionX, runningY, getPaint(7.5f));
						string fundsHandled;
						if(exempt.Handled.Is501){
							fundsHandled = exempt.Handled.Name.Substring(52);
						}else{
							fundsHandled = exempt.Handled.Name.Substring(64);
						}
						if( fundsHandled.Length > 100){
							pdfCanvas.DrawText(fundsHandled, secondRowX, runningY, getPaint(6.7f, 2));
						}else{
							pdfCanvas.DrawText(fundsHandled, secondRowX, runningY, getPaint(7.5f, 1));
						}
						pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
						runningY += lineHight;
						runningY += lineHight;
						runningY += lineHight;

						if(exempt.Handled.Is501){
							pdfCanvas.DrawText("INFORMATION SPECIFIC TO TAX EXEMPT STATUS DERIVED FROM 501(c) ORGANIZATION", positionX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thickLinePaint);
							runningY += lineHight;
							pdfCanvas.DrawText("Name of 501(c) organization:", positionX, runningY, getPaint(7.5f));
							pdfCanvas.DrawText(exempt.OrganizationName, secondRowX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
							runningY += lineHight;
							pdfCanvas.DrawText("EIN of the 501(c):", positionX, runningY, getPaint(7.5f));
							pdfCanvas.DrawText(exempt.OrganizationEin, secondRowX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
							runningY += lineHight;
							pdfCanvas.DrawText("County where the 501(c) resides:", positionX, runningY, getPaint(7.5f));
							pdfCanvas.DrawText(exempt.OrganizationResides == null ? "" : exempt.OrganizationResides.Name??"", secondRowX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
							runningY += lineHight;
							pdfCanvas.DrawText("IRS letter of determination date:", positionX, runningY, getPaint(7.5f));
							pdfCanvas.DrawText(exempt.OrganizationLetterDate??"", secondRowX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
							runningY += lineHight;
							pdfCanvas.DrawText("Signed MOU date:", positionX, runningY, getPaint(7.5f));
							pdfCanvas.DrawText(exempt.OrganizationSignedDate??"", secondRowX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
							runningY += lineHight;
							pdfCanvas.DrawText("Appropriate IRS 990 series form filed date:", positionX, runningY, getPaint(7.5f));
							pdfCanvas.DrawText(exempt.OrganizationAppropriate??"", secondRowX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
							runningY += lineHight;
						}else{
							pdfCanvas.DrawText("INFORMATION SPECIFIC TO TAX EXEMPT STATUS DERIVED FROM COUNTY EXTENSION DISTRICT", positionX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thickLinePaint);
							runningY += lineHight;
							pdfCanvas.DrawText("Name of the Extension District:", positionX, runningY, getPaint(7.5f));
							pdfCanvas.DrawText(exempt.DistrictName??"", secondRowX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
							runningY += lineHight;
							pdfCanvas.DrawText("The District's EIN:", positionX, runningY, getPaint(7.5f));
							pdfCanvas.DrawText(exempt.DistrictEin??"", secondRowX, runningY, getPaint(7.5f, 1));
							pdfCanvas.DrawLine(margin, runningY + lineOffset, margin + trainingsTableLines[5], runningY + lineOffset, thinLinePaint);
							runningY += lineHight;
						}
						
					

						

						document.EndPage();

					
					}else{
						Log(id,"Tax Exempt", "Tax Exempt Error", "int", "Error");
						return new StatusCodeResult(500);
					}

					document.Close();
					Log(exempt,"TaxExempt", "Tax Exempt Pdf Created", "Tax Exempt Entity");
					return File(stream.DetachAsData().AsStream(), "application/pdf", "TaxExemptEntry.pdf");	
			}

		}






	}

}
