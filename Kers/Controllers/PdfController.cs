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


namespace Kers.Controllers
{
	[Route("api/[controller]")]
    public class PdfController : PdfBaseController
    {

		IExpenseRepository expenseRepo;
		KERS_SNAPED2017Context snapContext;
		IFiscalYearRepository fiscalYearRepo;

		const int width = 612;
		const int height = 792;

		string[] typefaceNames = {	"HelveticaNeue", "HelveticaNeue-Bold", 
									"HelveticaNeue-CondensedBold", "HelveticaNeue-Light"
								};



        public PdfController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
			KERSmainContext mainContext,
			IExpenseRepository expenseRepo,
			KERS_SNAPED2017Context snapContext,
			IHostingEnvironment env,
			IMemoryCache _cache,
			IFiscalYearRepository fiscalYearRepo
        ):
		base(_context, userRepo, mainContext, _cache){
            
			this.expenseRepo = expenseRepo;
			this.snapContext = snapContext;
			this.fiscalYearRepo = fiscalYearRepo;
			
			
        }


		[HttpGet("expenses/{year}/{month}/{userId?}")]
        [Authorize]
		public IActionResult Expenses(int year, int month, int userId = 0)
        {
			//seedDatabase();
			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata())) {
				KersUser user;
				if(userId == 0){
					user = this.CurrentUser();
				}else{
					user = this._context.KersUser.
									Where(u => u.Id == userId).
									Include(u => u.PersonalProfile).
									Include(u => u.RprtngProfile).ThenInclude(p => p.PlanningUnit).
									Include(u => u.ExtensionPosition).
									FirstOrDefault();
				}

				var expenses = this.expenseRepo.PerMonth(user, year, month, "asc");


				float count = (float)expenses.Count();
				float ratio = count/4 ;
				var pages = (int) Math.Ceiling(ratio);

				summary(document, year, month, user, pages + 1);
				details(document, expenses, user, year, month);
				document.Close();

            	return File(stream.DetachAsData().AsStream(), "application/pdf", "ExpenseReport.pdf");	
			}			
		}
		private void details(SKDocument document, List<ExpenseRevision> expenses, KersUser user, int year, int month){
			float count = (float)expenses.Count();
			float ratio = count/4 ;
			var pages = (int) Math.Ceiling(ratio) + 1;
			int pageIndex = 2;
			int position = 0;
			var pdfCanvas = document.BeginPage(width, height);
			detailsPageInfo(pdfCanvas, user, year, month, pageIndex, pages);
			foreach(var expense in expenses){

				position++;
				if(position > 4){
					position = 1;
					pageIndex++;
					document.EndPage();
					pdfCanvas = document.BeginPage(width, height);
					detailsPageInfo(pdfCanvas, user, year, month, pageIndex, pages);
				}
				addDetail(pdfCanvas, expense, position);

			}
			document.EndPage();
		}

		private void detailsPageInfo(SKCanvas pdfCanvas, KersUser user, int year, int month, int pageIndex, int totalPages){
			var date = new DateTime(year, month, 1);
			AddPageInfo(pdfCanvas, pageIndex, totalPages, user, date);
			var text = date.ToString("MMMM yyyy");
			pdfCanvas.DrawText(text, 109, 93, getPaint(20.0f, 1, 0xFF000000));

			var subText = "Monthly Expenses Report";
			pdfCanvas.DrawText(subText, 43, 67, getPaint(16.0f, 3, 0xFF000000));


			text = user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName;
			pdfCanvas.DrawText(text, 300, 63, getPaint(18.0f, 1));
			pdfCanvas.DrawText(user.ExtensionPosition.Title, 300, 79, getPaint(12.0f));
			pdfCanvas.DrawText(user.RprtngProfile.PlanningUnit.Name, 300, 93, getPaint(12.0f));

			pdfCanvas.DrawText("Details", 43, 93, getPaint(20.0f, 3));
		}

		private void addDetail(SKCanvas pdfCanvas, ExpenseRevision expense, int position){
			var offset = 140;
			var panelHeight = 150;

			var startingY = offset + (position - 1) * panelHeight;

			SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.DarkGray,
												StrokeWidth = 0.5f
											};
			pdfCanvas.DrawLine(43, startingY, 580, startingY, thinLinePaint);
			pdfCanvas.DrawLine(43, startingY + panelHeight - 20, 580, startingY + panelHeight - 20, thinLinePaint);

			pdfCanvas.DrawText(expense.ExpenseDate.ToString("MMMM dd, yyyy"), 43, startingY + 25, getPaint(18.0f, 2));
			pdfCanvas.DrawText("Location: ", 43, startingY + 38, getPaint(9.0f));
			pdfCanvas.DrawText(expense.ExpenseLocation, 85, startingY + 38, getPaint(9.0f, 2));
			
			
			// Left Column
			if(expense.FundingSourceMileage != null){
				pdfCanvas.DrawText("Mileage Funding: ", 43, startingY + 60, getPaint(9.0f));
				pdfCanvas.DrawText(expense.FundingSourceMileage.Name, 43, startingY + 70, getPaint(9.0f, 2));
			}
			pdfCanvas.DrawText("Miles: ", 43, startingY + 86, getPaint(9.0f));
			pdfCanvas.DrawText(expense.Mileage.ToString(), 71, startingY + 86, getPaint(9.0f, 2));
			if(expense.departTime != null){
				pdfCanvas.DrawText("Time Departed: ", 43, startingY + 98, getPaint(9.0f));
				pdfCanvas.DrawText(expense.departTime?.ToString("hh:mm tt"), 108, startingY + 98, getPaint(9.0f, 2));
			}if(expense.returnTime != null){
				pdfCanvas.DrawText("Time Returned: ", 43, startingY + 110, getPaint(9.0f));
				pdfCanvas.DrawText(expense.returnTime?.ToString("hh:mm tt"), 108, startingY + 110, getPaint(9.0f, 2));
			}
			

			// Right Column
			if(expense.FundingSourceNonMileage != null){
				pdfCanvas.DrawText("Expense Funding: ", 300, startingY + 60, getPaint(9.0f));
				pdfCanvas.DrawText(expense.FundingSourceNonMileage.Name, 300, startingY + 70, getPaint(9.0f, 2));
			}
			pdfCanvas.DrawText("Breakfast: ", 300, startingY + 86, getPaint(9.0f));
			pdfCanvas.DrawText("$"+expenseRepo.Breakfast(expense).ToString("0.00"), 344, startingY + 86, getPaint(9.0f, 2));
			pdfCanvas.DrawText("Lunch: ", 400, startingY + 86, getPaint(9.0f));
			pdfCanvas.DrawText("$"+expenseRepo.Lunch(expense).ToString("0.00"), 455, startingY + 86, getPaint(9.0f, 2));
			pdfCanvas.DrawText("Dinner: ", 510, startingY + 86, getPaint(9.0f));
			pdfCanvas.DrawText("$"+expenseRepo.Dinner(expense).ToString("0.00"), 543, startingY + 86, getPaint(9.0f, 2));


			pdfCanvas.DrawText("Lodging: ", 300, startingY + 98, getPaint(9.0f));
			pdfCanvas.DrawText("$"+expense.Lodging.ToString("0.00"), 344, startingY + 98, getPaint(9.0f, 2));
			pdfCanvas.DrawText("Registration: ", 400, startingY + 98, getPaint(9.0f));
			pdfCanvas.DrawText("$"+expense.Registration.ToString("0.00"), 455, startingY + 98, getPaint(9.0f, 2));
			pdfCanvas.DrawText("Other: ", 510, startingY + 98, getPaint(9.0f));
			pdfCanvas.DrawText("$"+expense.otherExpenseCost.ToString("0.00"), 543, startingY + 98, getPaint(9.0f, 2));

			if(expense.otherExpenseExplanation != null && expense.otherExpenseExplanation != ""){
				pdfCanvas.DrawText("Other Expense Explanation: ", 300, startingY + 110, getPaint(9.0f));
				pdfCanvas.DrawText(expense.otherExpenseExplanation, 300, startingY + 122, getPaint(9.0f, 2));
			}

		}


		private void summary(SKDocument document, int year, int month, KersUser user, int totalPages = 1){
			using (var pdfCanvas = document.BeginPage(width, height))
				{
					AddUkLogo(pdfCanvas, 16, 31);
					var date = new DateTime(year, month, 1);
					AddPageInfo(pdfCanvas, 1, totalPages, user, date);
					
					var text = date.ToString("MMMM yyyy");
					// draw contents
					pdfCanvas.DrawText(text, 257, 80, getPaint(20.0f, 1, 0xFF000000));

					var subText = "Monthly Expenses Report";
					pdfCanvas.DrawText(subText, 257, 102, getPaint(20.0f, 3, 0xFF000000));


					text = user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName;
					pdfCanvas.DrawText(text, 43, 204, getPaint(18.0f, 1));
					text = user.ExtensionPosition.Title + ", " + user.RprtngProfile.PlanningUnit.Name;
					pdfCanvas.DrawText(text, 43, 222, getPaint(12.0f));
					if(user.RprtngProfile.PlanningUnit.Address != null ){
						text = user.RprtngProfile.PlanningUnit.Address;
						pdfCanvas.DrawText(text, 43, 238, getPaint(9.0f));
						text = user.RprtngProfile.PlanningUnit.City + ", KY " + user.RprtngProfile.PlanningUnit.Zip;
						pdfCanvas.DrawText(text, 43, 251, getPaint(9.0f));
					}
					

					pdfCanvas.DrawText("Summary", 43, 300, getPaint(20.0f));

					var header = new List<string>();
					header.Add("FUNDING SOURCE");
					header.Add("MILES");
					header.Add("MILEAGE COST");
					header.Add("MEALS");
					header.Add("LODGING");
					header.Add("REGISTRATION");
					header.Add("OTHER");
					header.Add("MTD TOTALS");
					header.Add("YTD");

					SummaryTableRow(pdfCanvas, header, 360);

					var smr = expenseRepo.Summaries(user, year, month);
					var fiscalYear = fiscalYearRepo.byDate( new DateTime(year, month, 15), FiscalYearType.ServiceLog );
					DateTime endOfMonth = new DateTime(year, 
                                   month, 
                                   DateTime.DaysInMonth(year, 
                                                        month), 23, 59, 59);
					var yearTotals = expenseRepo.SummariesPerPeriod( user, fiscalYear.Start, endOfMonth );


					var blancRows = new List<ExpenseSummary>();
					foreach( var yearTotalRow in yearTotals ){
						if( !smr.Where( s => s.fundingSource.Id == yearTotalRow.fundingSource.Id).Any()){
							blancRows.Add( yearTotalRow );
						}
					}

					var lineIndex = 0;
					foreach( var exp in smr){
						var line = new List<String>();
						line.Add(exp.fundingSource.Name);
						line.Add( Math.Round(exp.miles, 2).ToString());
						line.Add("$" + exp.mileageCost.ToString("0.00"));
						line.Add("$" + exp.meals.ToString("0.00"));
						line.Add("$" + exp.lodging.ToString("0.00"));
						line.Add("$" + exp.registration.ToString("0.00"));
						line.Add("$" + exp.other.ToString("0.00"));
						line.Add("$" + exp.total.ToString("0.00"));
						float totl = 0;
						var yearTotal = yearTotals.Where( t => t.fundingSource.Id == exp.fundingSource.Id ).FirstOrDefault();
						if( yearTotal != null ){
							totl = yearTotal.total;
						}
						line.Add("$" + totl.ToString("0.00"));
						SummaryTableRow(pdfCanvas, line, 390 + (30*lineIndex));
						lineIndex++;

					}
					foreach( var blanc in blancRows){
						var line = new List<String>();
						line.Add(blanc.fundingSource.Name);
						line.Add("0");
						line.Add("$0.00");
						line.Add("$0.00");
						line.Add("$0.00");
						line.Add("$0.00");
						line.Add("$0.00");
						line.Add("$0.00");
						line.Add("$" + blanc.total.ToString("0.00"));
						SummaryTableRow(pdfCanvas, line, 390 + (30*lineIndex));
						lineIndex++;

					}

/*
					var snapActivities = this.snapContext.
                                zSnapEdActivities.Where(a => a.personID == user.RprtngProfile.PersonId && a.snapDate.Substring(4,2) == month.ToString("D2")).
                                GroupBy(a => a.personID).
                                Select( g => new
                                            {
                                                TotalHours = g.Sum(x => Convert.ToDouble( x.snapHours ))
                                            }).
                                FirstOrDefault();
					text = "Hours reported on Supplemental Nutrition Assistance Program (SNAP): ";
					if(snapActivities == null){
						text += "0";
					}else{
						text += snapActivities.TotalHours.ToString();
					}
					pdfCanvas.DrawText(text, 43, 550, getPaint(10.0f));
 */
					SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
					pdfCanvas.DrawLine(43, 680, 243, 680, thinLinePaint);
					pdfCanvas.DrawLine(320, 680, 520, 680, thinLinePaint);
					pdfCanvas.DrawText("Signature", 120, 695, getPaint(10.0f));
					pdfCanvas.DrawText("Date", 405, 695, getPaint(10.0f));
					document.EndPage();
					Log(smr);
			}

		}


		private void SummaryTableRow( SKCanvas pdfCanvas, List<string> data, int y, int x = 43){
			var paint = getPaint(7.1f, 2);
			pdfCanvas.DrawText(data[0], x, y,  paint);
			paint = getPaint(7.1f, 2 , 0xFF000000, SKTextAlign.Right);
			for(var i = 1; i<9; i++){
				pdfCanvas.DrawText(data[i], x + 156 + ( 45 * i), y,  paint);
			}
			SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
			pdfCanvas.DrawLine(x, y + 11, x + 516, y + 11, thinLinePaint);
		}
/*
		private void AddPageInfo(SKCanvas pdfCanvas, int page, int totalPages, KersUser user, DateTime date, string Ttl = "Monthly Expenses Report"){
			var text = "Page " + page.ToString() + " of " + totalPages.ToString();
			var paint = getPaint(9.0f, 3, 0xFF000000, SKTextAlign.Right);
			pdfCanvas.DrawText(text, 590, 20, paint);


			text = user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName + ", " + date.ToString("MMMM yyyy") + ", " + Ttl;
			paint = getPaint(9.0f, 1, 0xFF000000, SKTextAlign.Right);
			pdfCanvas.DrawText(text, 530, 20, paint);
			pdfCanvas.DrawText("Report Generated: ", 43, 770, getPaint(9.0f, 1));
			pdfCanvas.DrawText( DateTime.Now.ToString(), 125, 770, getPaint(9.0f));
		}
 */

    }
}
