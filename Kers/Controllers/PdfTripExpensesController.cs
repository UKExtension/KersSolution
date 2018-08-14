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
    public class PdfTripExpensesController : PdfBaseController
    {

		

		const int width = 792;
		const int height = 612;

		IExpenseRepository expenseRepo;


        public PdfTripExpensesController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
            IExpenseRepository expenseRepo,
			KERSmainContext mainContext,
			IMemoryCache _cache
        ):
		base(_context, userRepo, mainContext, _cache){
            this.expenseRepo = expenseRepo;
        }


		[HttpGet("tripexpenses/{year}/{month}/{userId?}/{overnight?}")]
        [Authorize]
		public IActionResult TripExpenses(int year, int month, int userId = 0, Boolean overnight = false)
        {
			

			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata( "Kers, Expenses, Reporting, Activity", "Expenses Report", "Mileage Report") )) {
				
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

                var dayExpenses = expenses.Where( e => e.isOvernight == overnight && e.Mileage != 0);

				var dataObjext = new TripExpenses(dayExpenses.ToList());
				var pagesData = dataObjext.getData();
				var currentPageNumber = 1;
				foreach( var pg in pagesData){
					var pdfCanvas = document.BeginPage(width, height);
					var runningY = dataObjext.GetPageMargins()/2;
					AddPageInfo(pdfCanvas, currentPageNumber, pagesData.Count(), user, new DateTime(year, month, 1), "Mileage Log", "landscape");
					if(pg.header){
						AddUkLogo(pdfCanvas, 16, 31);
                		SummaryLandscapeInfo(pdfCanvas, year, month, user, "Mileage Log", overnight);
						runningY += dataObjext.GetHeaderHeight();
					}
					foreach( var exp in pg.data){
						table(pdfCanvas, exp.expenses, 25, runningY, true, exp.title, exp.total);
						runningY += exp.expenses.Sum( e => e.lines) * dataObjext.GetLineHeight() + dataObjext.GetSpaceBetweenTables();
					}


					if(pg.signatures){
						Signatures(pdfCanvas, 30, 490);
					}
					document.EndPage();
					currentPageNumber++;
				}

				document.Close();
                
            	return File(stream.DetachAsData().AsStream(), "application/pdf", "TripExpensesReport.pdf");	
			}			
		}

		private int table(SKCanvas pdfCanvas, List<ExpenseNumLines> expenses, int x, int y, Boolean header = true, string title = "", float total = 0){
            var rowHeight = 15;
            var beginningY = y;
            int[] verticalLinesX = { 0, 74, 154, 415, 650, 700, 746 };
            SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
			if(title != ""){
				pdfCanvas.DrawText( title, x, y, getPaint(10.0f, 1));
				y += 5;
			}
            if(header){
                pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
			    pdfCanvas.DrawLine(x, y + rowHeight, x + 746, y + rowHeight, thinLinePaint);
                pdfCanvas.DrawLine(x, y + rowHeight - 0.5f, x + 746, y + rowHeight - 0.5f, thinLinePaint);
                pdfCanvas.DrawText("Date", x + 4, y + 11, getPaint(9.5f, 1));
				pdfCanvas.DrawText("Starting Location", x + 76, y + 11, getPaint(9.35f, 1));
                pdfCanvas.DrawText("Destination(s)", x + 158, y + 11, getPaint(9.5f, 1));
                pdfCanvas.DrawText("Business Purpose", x + 419, y + 11, getPaint(9.5f, 1));
                pdfCanvas.DrawText("Program", x + 652, y + 11, getPaint(9.5f, 1));
                pdfCanvas.DrawText("Mileage", x + 704, y + 11, getPaint(9.5f, 1));
                DrawTableVerticalLines(pdfCanvas, verticalLinesX, x, y, 15);
                y += rowHeight;
            }else{
                pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
            }
            var i = 0;
            foreach( var expense in expenses){
                var thisRowHeight = 0;
                var initialY = y;
                pdfCanvas.DrawText(expense.expense.ExpenseDate.ToString("MM/dd/yyyy") + "(" + expense.expense.ExpenseDate.ToString("ddd").Substring(0,2) + ")", x + 2, y + 11, getPaint(10.0f));
				if(expense.expense.ProgramCategory != null){
					pdfCanvas.DrawText(expense.expense.ProgramCategory.ShortName, x + 655, y + 11, getPaint(10.0f));
				}
				pdfCanvas.DrawText(Math.Round(expense.expense.Mileage, 2).ToString(), x + 737, y + 11, getPaint(10.0f, 0, 0xFF000000, SKTextAlign.Right));
				var startingLocation = "Workplace";
				if( expense.expense.StartingLocationType == 2 ){
					startingLocation = "Home";
				}
				pdfCanvas.DrawText(startingLocation, x + 77, y + 11, getPaint(10.0f));
                var locationLines = SplitLineToMultiline(expense.expense.ExpenseLocation, 52);
                var locationLinesY = y;
                var locationLinesHight = 0;
                foreach( var line in locationLines){
                    pdfCanvas.DrawText(line, x + 158, locationLinesY + 11, getPaint(10.0f));
                    locationLinesY += rowHeight;
                    locationLinesHight += rowHeight;
                }

                var businessPurposeLines = SplitLineToMultiline(expense.expense.BusinessPurpose, 52);
                var purposeLinesY = y;
                var purposeLineHight = 0;
                foreach( var line in businessPurposeLines){
                    pdfCanvas.DrawText(line, x + 420, purposeLinesY + 11, getPaint(10.0f));
                    purposeLinesY += rowHeight;
                    purposeLineHight += rowHeight;
                }
                y = Math.Max(locationLinesY, purposeLinesY);
                thisRowHeight = Math.Max( locationLinesHight, purposeLineHight);

                pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
                DrawTableVerticalLines(pdfCanvas, verticalLinesX, x, initialY, thisRowHeight);
                i++;
                
            }
			if(total != 0){
				pdfCanvas.DrawText("Total: ", x + 4, y + 13, getPaint(10.0f, 1));
				pdfCanvas.DrawText(Math.Round(total, 2).ToString(), x + 737, y + 13, getPaint(10.0f, 1, 0xFF000000, SKTextAlign.Right));
			}
            return i;
        }

        private int paintTable(SKCanvas pdfCanvas, IEnumerable<ExpenseRevision> expenses, int x, int y, int space, Boolean header = true, float total = 0){
            var rowHeight = 15;
            var beginningY = y;
            int[] verticalLinesX = { 0, 74, 360, 650, 700, 746 };
            SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
            if(header){
                pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
			    pdfCanvas.DrawLine(x, y + rowHeight, x + 746, y + rowHeight, thinLinePaint);
                pdfCanvas.DrawLine(x, y + rowHeight - 0.5f, x + 746, y + rowHeight - 0.5f, thinLinePaint);
                pdfCanvas.DrawText("Date", x + 10, y + 11, getPaint(10.0f, 1));
                pdfCanvas.DrawText("Destination(s)", x + 78, y + 11, getPaint(10.0f, 1));
                pdfCanvas.DrawText("Business Purpose", x + 364, y + 11, getPaint(10.0f, 1));
                pdfCanvas.DrawText("Program", x + 652, y + 11, getPaint(10.0f, 1));
                pdfCanvas.DrawText("Mileage", x + 704, y + 11, getPaint(10.0f, 1));
                DrawTableVerticalLines(pdfCanvas, verticalLinesX, x, y, 15);
                y += rowHeight;
            }else{
                pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
            }
            var i = 0;
            foreach( var expense in expenses){
                var thisRowHeight = 0;
                var initialY = y;
                pdfCanvas.DrawText(expense.ExpenseDate.ToString("MM/dd/yyyy") + "(" + expense.ExpenseDate.ToString("ddd").Substring(0,2) + ")", x + 2, y + 11, getPaint(10.0f));
				if(expense.ProgramCategory != null){
					pdfCanvas.DrawText(expense.ProgramCategory.ShortName, x + 655, y + 11, getPaint(10.0f));
				}
				pdfCanvas.DrawText(expense.Mileage.ToString(), x + 737, y + 11, getPaint(10.0f, 0, 0xFF000000, SKTextAlign.Right));
                var locationLines = SplitLineToMultiline(expense.ExpenseLocation, 58);
                var locationLinesY = y;
                var locationLinesHight = 0;
                foreach( var line in locationLines){
                    pdfCanvas.DrawText(line, x + 78, locationLinesY + 11, getPaint(10.0f));
                    locationLinesY += rowHeight;
                    locationLinesHight += rowHeight;
                }

                var businessPurposeLines = SplitLineToMultiline(expense.BusinessPurpose, 58);
                var purposeLinesY = y;
                var purposeLineHight = 0;
                foreach( var line in businessPurposeLines){
                    pdfCanvas.DrawText(line, x + 364, purposeLinesY + 11, getPaint(10.0f));
                    purposeLinesY += rowHeight;
                    purposeLineHight += rowHeight;
                }
                y = Math.Max(locationLinesY, purposeLinesY);
                thisRowHeight = Math.Max( locationLinesHight, purposeLineHight);

                pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
                DrawTableVerticalLines(pdfCanvas, verticalLinesX, x, initialY, thisRowHeight);
                i++;
                if( (y - beginningY) > space ){
                    break;
                }
            }
			if(total != 0){
				pdfCanvas.DrawText("Total: ", x + 4, y + 13, getPaint(10.0f, 1));
				pdfCanvas.DrawText(total.ToString(), x + 737, y + 13, getPaint(10.0f, 1, 0xFF000000, SKTextAlign.Right));
			}
            return i;
        }
        private void DrawTableVerticalLines(SKCanvas pdfCanvas, int[] PosiitionsX, int x, int y, int length){
            SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
            foreach( var pos in PosiitionsX){
                pdfCanvas.DrawLine(x + pos, y, x + pos, y + length, thinLinePaint);
            }
            
        }

		private void Signatures( SKCanvas pdfCanvas, int x, int y){
			     SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
                pdfCanvas.DrawLine(x, y + 60, x + 300, y + 60, thinLinePaint);
                pdfCanvas.DrawText("Employee Signature & Date",  x + 85, y + 72, getPaint(10.0f));


                pdfCanvas.DrawLine(x + 400, y, x + 700, y, thinLinePaint);
                pdfCanvas.DrawText("Authorized Reviewer Printed Name", x + 475, y + 12, getPaint(10.0f));

				pdfCanvas.DrawLine(x + 400, y + 60, x + 700, y + 60, thinLinePaint);
                pdfCanvas.DrawText("Authorized Reviewer Signature & Date",  x + 470, y + 72, getPaint(10.0f));
		}



    }



	public class TripExpenses{

		List<ExpenseRevision> _expenses;
		List<ExpenseNumLines> _expenseLines;
		List<ExpenseNumLines> _countyExpenses;
		List<ExpenseNumLines> _nonCountyExpenses;
		List<ExpenseMileageLogPageData> pages = new List<ExpenseMileageLogPageData>();
		string[] nonCountySourceNames = new string[]{"State", "Federal"};
		int locationLinesCharacterLength = 58;
		int businessPurposeLinesCharacterLength = 58;

		//********************************/
		// Dimensions definition         //
		//********************************/

		int headerHeight = 130;
		int lineHeight = 15;
		int signaturesHeight = 160;
		int pageHeight = 612;
		int pageMargins = 92;
		int spaceBetweenTables = 80;
		
		public TripExpenses(
			List<ExpenseRevision> _expenses
		){
			this._expenses = _expenses;
			getLines();
			DivideExpenses();
			
		}


		/************************/
		// Getters
		/************************/
		public int GetHeaderHeight(){
			return headerHeight;
		}
		public int GetLineHeight(){
			return lineHeight;
		}
		public int GetSignaturesHeight(){
			return signaturesHeight;
		}
		public int GetSpaceBetweenTables(){
			return spaceBetweenTables;
		}
		public int getLinesCounty(){
			return getLinesCount(_countyExpenses);
		}
		public int getLinesNotCounty(){
			return getLinesCount(_nonCountyExpenses);
		}
		public int GetPageMargins(){
			return pageMargins;
		}
		public List<ExpenseNumLines> getCountyExpenses(){
			return _countyExpenses;
		}
		public List<ExpenseNumLines> getNonCountyExpenses(){
			return _nonCountyExpenses;
		}
		public List<ExpenseMileageLogPageData> getData(){
			CalculatePageData();
			return pages;
		}
		private int getLinesCount(List<ExpenseNumLines> exp){
			return exp.Sum( e => e.lines);
		}

		private void getLines(){
			_expenseLines = new List<ExpenseNumLines>();
			foreach( var expense in _expenses){
				var ln = new ExpenseNumLines();
				ln.expense = expense;
				var locLines = PdfBaseController.SplitLineToMultiline(expense.ExpenseLocation, locationLinesCharacterLength);
				ln.locationLines = locLines;
				var busnLines = PdfBaseController.SplitLineToMultiline(expense.BusinessPurpose, businessPurposeLinesCharacterLength);
				ln.purposeLines = busnLines;
				ln.lines = Math.Max(locLines.Count(), busnLines.Count());
				_expenseLines.Add(ln);
			}
		}

		public void CalculatePageData(){
			var pageSpace = pageHeight - pageMargins;
			
			var countyRemaining = _countyExpenses.Count();
			var nonCountyRemaining = _nonCountyExpenses.Count();
			var signaturesAdded = false;
			var currentPage = 1;
			do{
				var spaceRemaining = pageSpace;
				var pg = new ExpenseMileageLogPageData();
				if(currentPage == 1){
					pg.header = true;
					spaceRemaining -= headerHeight;
				}
				
				if(countyRemaining > 0){
					var tbl = new ExpenseMileageLogTableData();
					if(countyRemaining < _countyExpenses.Count()){
						tbl.title = "";
					}
					var rmnng = _countyExpenses.Skip(_countyExpenses.Count() - countyRemaining);
					foreach( var exp in rmnng){
						if(exp.lines * lineHeight < spaceRemaining){
							tbl.expenses.Add(exp);
							countyRemaining--;
							spaceRemaining -= exp.lines * lineHeight;
						}else{
							break;
						}
					}
					if(tbl.expenses.Count() > 0){
						if(countyRemaining <= 0){
							tbl.total = _countyExpenses.Sum( e => e.expense.Mileage);
							spaceRemaining -= lineHeight;
						}
						pg.data.Add(tbl);
					}

				}
				if( countyRemaining <= 0 && nonCountyRemaining > 0){
					var tbl = new ExpenseMileageLogTableData();
					var rmnng = _nonCountyExpenses.Skip(_nonCountyExpenses.Count() - nonCountyRemaining);
					if( countyRemaining == 0 && nonCountyRemaining == _nonCountyExpenses.Count()){
						spaceRemaining -= spaceBetweenTables;
						spaceRemaining -= lineHeight;
						tbl.title = "UK Funded Travel:";
					}else{
						tbl.title = "";
					}
					foreach( var exp in rmnng){
						if(exp.lines * lineHeight < spaceRemaining){
							tbl.expenses.Add(exp);
							nonCountyRemaining--;
							spaceRemaining -= exp.lines * lineHeight;
						}else{
							break;
						}
					}
					if(tbl.expenses.Count() > 0){
						if(nonCountyRemaining <= 0){
							tbl.total = _nonCountyExpenses.Sum( e => e.expense.Mileage);
							spaceRemaining -= lineHeight;
						}
						pg.data.Add(tbl);
					}
				}
				if(countyRemaining <= 0 && nonCountyRemaining <= 0 && spaceRemaining >= signaturesHeight){
					signaturesAdded = true;
					pg.signatures = true;
				}
				
				currentPage++;
				pages.Add(pg);
			}while( !signaturesAdded);

		}



		/************************/
		// Setters
		/************************/
		public void SetLocationLinesCharacterLength(int length){
			locationLinesCharacterLength = length;
		}
		public void SetBusinessPurposeLinesCharacterLength(int length){
			businessPurposeLinesCharacterLength = length;
		}
		public void setPageHeight(int height){
			this.pageHeight = height;
		}

		private void DivideExpenses(){
			this._nonCountyExpenses = _expenseLines.Where( e => nonCountySourceNames.Contains( e.expense.FundingSourceMileage.Name ) ).ToList();
			this._countyExpenses = _expenseLines.Except( this._nonCountyExpenses ).ToList();

		}
	}

	public class ExpenseNumLines{
		public int lines;
		public ExpenseRevision expense;
		public List<string> locationLines;
		public List<string> purposeLines;
	}

	public class ExpenseMileageLogPageData{
		public bool header = false;
		public bool signatures = false;

		public List<ExpenseMileageLogTableData> data = new List<ExpenseMileageLogTableData>();

	}

	public class ExpenseMileageLogTableData{
		public bool header = true;
		public string title = "County Funded Travel:";
		public float total = 0;
		public List<ExpenseNumLines> expenses = new List<ExpenseNumLines>();
	}




}
