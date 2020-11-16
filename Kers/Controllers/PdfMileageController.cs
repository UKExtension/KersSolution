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
    public class PdfMileageController : PdfBaseController
    {
		const int width = 792;
		const int height = 612;

		IExpenseRepository expenseRepo;


        public PdfMileageController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
            IExpenseRepository expenseRepo,
			KERSmainContext mainContext,
			IMemoryCache _cache
        ):
			base(_context, userRepo, mainContext, _cache){
				this.expenseRepo = expenseRepo;
			}


		[HttpGet("mileagelog/{year}/{month}/{userId?}/{overnight?}/{personal?}")]
        [Authorize]
		public IActionResult MiliageLog(int year, int month, int userId = 0, Boolean overnight = false, Boolean personal = true)
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
                

					var expenses = this.expenseRepo.MileagePerMonth(user, year, month);
	
					if(personal){
						expenses = expenses.Where( e => e.LastRevision.VehicleType != 2 && e.LastRevision.isOvernight == overnight);
					}else{
						expenses = expenses.Where( e => e.LastRevision.VehicleType == 2);
					}

					var dataObjext = new MileageData(expenses.OrderBy(o => o.LastRevision.ExpenseDate));
					dataObjext.SetIsItPersonalVehicle(personal);
					var dt = dataObjext.getData();
					var currentPageNumber = 1;
					foreach( var pg in dt){
						var pdfCanvas = document.BeginPage(width, height);

						var runningY = dataObjext.GetPageMargins()/2;
						AddPageInfo(pdfCanvas, currentPageNumber, dt.Count(), user, new DateTime(year, month, 1), "Mileage Log", "landscape");
 

						if(pg.header){
							AddUkLogo(pdfCanvas, 16, 31);
							SummaryLandscapeInfo(pdfCanvas, year, month, user, "Mileage Log", overnight, personal);
							runningY += dataObjext.GetHeaderHeight();
						}
						
						if( pg.segments.Count() > 0 ){
							runningY = table(pdfCanvas, pg.segments, 25, runningY, dataObjext);
						}
						

						if(pg.signatures){
							Signatures(pdfCanvas, 30, 490);
						}

						// Check to see if totals should be added on the bottom of the table
						if(
							// this is the last page and there is table here
							(pg.signatures && pg.segments.Count() > 0)
							||
							// there is no table on the next page and this is the last page with a table
							(currentPageNumber < dt.Count() &&   dt.ElementAt( currentPageNumber ).segments.Count() == 0)
						){

							pdfCanvas.DrawText("Totals: ", 28, runningY + 15, getPaint(10.5f, 1));
							var verticalLines = dataObjext.getVerticalLinesX();
							var numMileages = dataObjext.mileageColumnsCount;
							var runningX = verticalLines[ verticalLines.Length - numMileages];
							if(dataObjext.CountyColumnPresent){
								pdfCanvas.DrawText(dataObjext.countyMileage.ToString(), runningX + dataObjext.mileageColumnPixelLength - 8, runningY + 15, getPaint(10.0f, 1, 0xFF000000, SKTextAlign.Right));
								runningX += dataObjext.mileageColumnPixelLength;
							}
							if(dataObjext.ProfImprvmntColumnPresent){
								pdfCanvas.DrawText(dataObjext.profImprvMileage.ToString(), runningX + dataObjext.mileageColumnPixelLength - 8, runningY + 15, getPaint(10.0f, 1, 0xFF000000, SKTextAlign.Right));
								runningX += dataObjext.mileageColumnPixelLength;
							}
							if(dataObjext.UKColumnPresent){
								pdfCanvas.DrawText(dataObjext.UKMileage.ToString(), runningX + dataObjext.mileageColumnPixelLength - 8, runningY + 15, getPaint(10.0f, 1, 0xFF000000, SKTextAlign.Right));
							}


						}


						 
						document.EndPage();
						currentPageNumber++;
					}

					
					
					document.Close();
					
					return File(stream.DetachAsData().AsStream(), "application/pdf", "TripExpensesReport.pdf");	
			}			
		}

		private int table(SKCanvas pdfCanvas, List<MileageNumLines> segments, int x, int y, MileageData dataObject){
            var rowHeight = 15;
            var beginningY = y;
			var runningXIndex = 0;
			var padding = 2;

			int[] verticalLinesX = dataObject.getVerticalLinesX();
            SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
			SKPaint mediumLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 1.0f
											};

			pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
			pdfCanvas.DrawLine(x, y + rowHeight, x + 746, y + rowHeight, thinLinePaint);
			pdfCanvas.DrawLine(x, y + rowHeight - 0.5f, x + 746, y + rowHeight - 0.5f, thinLinePaint);



			pdfCanvas.DrawText("Date", x + verticalLinesX[runningXIndex] + padding, y + 11, getPaint(9.5f, 1));

			//pdfCanvas.DrawText("Vehicle Name", x + 76, y + 11, getPaint(9.35f, 1));


			runningXIndex++;

			pdfCanvas.DrawText("Start", x + verticalLinesX[runningXIndex] + padding, y + 11, getPaint(9.35f, 1));

			runningXIndex++;

			pdfCanvas.DrawText("Location", x + verticalLinesX[runningXIndex] + padding, y + 11, getPaint(9.35f, 1));

			runningXIndex++;

			pdfCanvas.DrawText("Business Purpose", x + verticalLinesX[runningXIndex] + padding, y + 11, getPaint(9.5f, 1));


			runningXIndex++;

			pdfCanvas.DrawText("Program", x + verticalLinesX[runningXIndex] + padding - 2, y + 11, getPaint(9.5f, 1));

			runningXIndex++;

			
			pdfCanvas.DrawText("Mileage", x + verticalLinesX[runningXIndex] + (verticalLinesX[verticalLinesX.Count() - 1] - verticalLinesX[runningXIndex])/2, y + 11 - rowHeight, getPaint(8.0f, 1, 0xFF000000, SKTextAlign.Center));
			pdfCanvas.DrawLine(x + verticalLinesX[runningXIndex], y - rowHeight, x + verticalLinesX[runningXIndex], y, thinLinePaint);
			pdfCanvas.DrawLine(x + verticalLinesX[verticalLinesX.Count() - 1], y - rowHeight, x + verticalLinesX[verticalLinesX.Count() - 1], y, thinLinePaint);
			pdfCanvas.DrawLine(x + verticalLinesX[runningXIndex], y - rowHeight, x + verticalLinesX[verticalLinesX.Count() - 1], y - rowHeight, mediumLinePaint);


			if( dataObject.CountyColumnPresent){
				pdfCanvas.DrawText("County", x + verticalLinesX[runningXIndex] + padding - 1, y + 11, getPaint(6.5f, 1));
				runningXIndex++;
			}

			if( dataObject.ProfImprvmntColumnPresent){
				pdfCanvas.DrawText("Prf Imprv", x + verticalLinesX[runningXIndex] + padding - 1, y + 11, getPaint(6.5f, 1));
				runningXIndex++;
			}

			if( dataObject.UKColumnPresent){
				pdfCanvas.DrawText("UK Fnded", x + verticalLinesX[runningXIndex] + padding - 2, y + 11, getPaint(6.5f, 1));
				runningXIndex++;
			}

			DrawTableVerticalLines(pdfCanvas, verticalLinesX, x, y, 15);
			y += rowHeight;
			int LastExpenseRevisionId = 0;

			var comments = dataObject.GetComments();

            foreach( var segment in segments){
				
				
				if(LastExpenseRevisionId != segment.segment.segment.ExpenseRevisionId){
					pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
					var comment = comments.Where( c => c.ExpenseRevisionId == LastExpenseRevisionId).FirstOrDefault();
					if(comment != null){
						pdfCanvas.DrawLine(x + verticalLinesX[0], y, x + verticalLinesX[0], y + rowHeight * comment.commentLines.Count(), thinLinePaint);
						pdfCanvas.DrawLine(x + verticalLinesX[verticalLinesX.Length - 1], y, x + verticalLinesX[verticalLinesX.Length - 1], y + rowHeight * comment.commentLines.Count(), thinLinePaint);
						foreach( var line in comment.commentLines){
							pdfCanvas.DrawText(line, x + verticalLinesX[0] + padding, y + 11, getPaint(10.0f));
							y += rowHeight;
						}
					}

					pdfCanvas.DrawLine(x, y, x + 746, y, mediumLinePaint);


				}else{
					pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
				}
				var initialY = y;
				var runningY = initialY;
				runningXIndex = 0;
                var thisRowHeight = 0;
                
                pdfCanvas.DrawText(segment.segment.MileageDate.ToString("MM/dd/yyyy") + "(" + segment.segment.MileageDate.ToString("ddd").Substring(0,2) + ")", x + verticalLinesX[runningXIndex] + padding, y + 11, getPaint(10.0f));
				runningXIndex++;
                
				
 
                foreach( var line in segment.startLocationLines){
                    pdfCanvas.DrawText(line, x + verticalLinesX[runningXIndex] + padding, runningY + 11, getPaint(10.0f));
                    runningY += rowHeight;
                }
				runningXIndex++;
				runningY = initialY;
				foreach( var line in segment.endLocationLines){
                    pdfCanvas.DrawText(line, x + verticalLinesX[runningXIndex] + padding, runningY + 11, getPaint(10.0f));
                    runningY += rowHeight;
                }
				runningXIndex++;
				runningY = initialY;
				foreach( var line in segment.purposeLines){
                    pdfCanvas.DrawText(line, x + verticalLinesX[runningXIndex] + padding, runningY + 11, getPaint(10.0f));
                    runningY += rowHeight;
                }
				runningXIndex++;
				if( segment.segment.segment.ProgramCategory != null ){
					pdfCanvas.DrawText(segment.segment.segment.ProgramCategory.ShortName, x + verticalLinesX[runningXIndex] + padding, initialY + 11, getPaint(10.0f));
				}
				runningXIndex++;
				if( dataObject.CountyColumnPresent){
					if( dataObject.countySourceNames.Contains(  segment.segment.segment.FundingSource.Name ) ){
						pdfCanvas.DrawText(segment.segment.segment.Mileage.ToString(), x + verticalLinesX[runningXIndex] - padding + dataObject.mileageColumnPixelLength, y + 11, getPaint(10.0f, 0, 0xFF000000, SKTextAlign.Right));
					}
					runningXIndex++;
				}
				if( dataObject.ProfImprvmntColumnPresent){
					if( dataObject.professionalDevelopmentNames.Contains(  segment.segment.segment.FundingSource.Name ) ){
						pdfCanvas.DrawText(segment.segment.segment.Mileage.ToString(), x + verticalLinesX[runningXIndex] - padding + dataObject.mileageColumnPixelLength, y + 11, getPaint(10.0f, 0, 0xFF000000, SKTextAlign.Right));
					}
					runningXIndex++;
				}
				if( dataObject.UKColumnPresent){
					if( dataObject.UKSourceNames.Contains(  segment.segment.segment.FundingSource.Name ) ){
						pdfCanvas.DrawText(segment.segment.segment.Mileage.ToString(), x + verticalLinesX[runningXIndex] - padding + dataObject.mileageColumnPixelLength, y + 11, getPaint(10.0f, 0, 0xFF000000, SKTextAlign.Right));
					}
					runningXIndex++;
				}
				

                
				


                thisRowHeight =  segment.lines * rowHeight;
				y += thisRowHeight;
                
				


                DrawTableVerticalLines(pdfCanvas, verticalLinesX, x, initialY, thisRowHeight);
				LastExpenseRevisionId = segment.segment.segment.ExpenseRevisionId;
                
            }
			var cmnt = comments.Where( c => c.ExpenseRevisionId == LastExpenseRevisionId).FirstOrDefault();
			if(cmnt != null){
				pdfCanvas.DrawLine(x, y, x + 746, y, thinLinePaint);
				pdfCanvas.DrawLine(x + verticalLinesX[0], y, x + verticalLinesX[0], y + rowHeight * cmnt.commentLines.Count(), thinLinePaint);
				pdfCanvas.DrawLine(x + verticalLinesX[verticalLinesX.Length - 1], y, x + verticalLinesX[verticalLinesX.Length - 1], y + rowHeight * cmnt.commentLines.Count(), thinLinePaint);
				foreach( var line in cmnt.commentLines){
					pdfCanvas.DrawText(line, x + verticalLinesX[0] + padding, y + 11, getPaint(10.0f));
					y += rowHeight;
				}
			}
			pdfCanvas.DrawLine(x, y, x + 746, y, mediumLinePaint);
            return y;
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


	class MileageData{



		IQueryable<Expense> _expenses;
		List<MileageSegmentWithDate> _segments = new List<MileageSegmentWithDate>();
		List<MileageComment> _comments = new List<MileageComment>();
		public int mileageColumnsCount = 3;



		List<MileageNumLines> _sectionLines;
		bool isItPersonalVehicle = true;
		List<MileageLogTableData> pages = new List<MileageLogTableData>();
	
		public string[] UKSourceNames = new string[]{"State", "Federal"};
		public string[] countySourceNames = new string[]{"County Travel (Reimbursed to Employee)"};
		public string[] professionalDevelopmentNames = new string[]{"Professional Improvement (Reimbursed to Employee)"};



		public int dateCharacterLength = 24;
		public int startLocationCharacterLength = 32;
		public int endLocationCharacterLength = 32;
		public int businessPurposeCharacterLength = 30;
		public int mileageColumnCharacterLength = 15;
		public int totalCharacterLength = 160;


		int datePixelLength = 74;
		int startingLocationPixelLength = 184;
		int endLocationPixelLength = 184;
		int purposePixelLength = 170;
		int programsPixelLength = 41;
		public int mileageColumnPixelLength = 31;

		
		public bool CountyColumnPresent = true;
		public bool ProfImprvmntColumnPresent = true;
		public bool UKColumnPresent = true;

		public float countyMileage = 0;
		public float profImprvMileage = 0;
		public float UKMileage = 0;

		
		//********************************/
		// Dimensions definition         //
		//********************************/

		int headerHeight = 130;
		int lineHeight = 15;
		int signaturesHeight = 160;
		int pageHeight = 612;
		int pageMargins = 92;


		public MileageData( IQueryable<Expense> data){
			this._expenses = data;
		}
		public void SetIsItPersonalVehicle( bool personal ){
			this.isItPersonalVehicle = personal;
		}

		private void ExtractSegments(){
			foreach( var expns in this._expenses){
				var StartingLocation = expns.LastRevision.StartingLocation;
				var Segments = expns.LastRevision.Segments.OrderBy( s => s.order);
				foreach( var segment in Segments){
					var SegmentWithDate = new MileageSegmentWithDate(){
						MileageDate = expns.ExpenseDate,
						segment = segment,
						StartigngLocation = StartingLocation,
						EndingLocation = segment.Location
					};
					StartingLocation = segment.Location;
					_segments.Add(SegmentWithDate);
				}
			}
		}
		private void ExtractComments(){
			foreach( var expns in this._expenses){
				if( expns.LastRevision.Comment != null && expns.LastRevision.Comment != ""){

					var lines = PdfBaseController.SplitLineToMultiline(expns.LastRevision.Comment, this.totalCharacterLength);


					var cmnt = new MileageComment(){
						ExpenseRevisionId = expns.LastRevisionId,
						comment = expns.LastRevision.Comment,
						commentLines = lines,
						lines = lines.Count()
					};
					this._comments.Add(cmnt);
				}
			}
		}
		private void AdjustCharacterLengths(){
			countyMileage = _segments.Where( e => countySourceNames.Contains( e.segment.FundingSource.Name) ).Sum( e => e.segment.Mileage );
			profImprvMileage = _segments.Where( e => this.professionalDevelopmentNames.Contains( e.segment.FundingSource.Name) ).Sum( e => e.segment.Mileage );
			UKMileage = _segments.Where( e => this.UKSourceNames.Contains( e.segment.FundingSource.Name) ).Sum( e => e.segment.Mileage );
			if( countyMileage == 0 ){
				this.CountyColumnPresent = false;
				this.startLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.endLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.businessPurposeCharacterLength += (mileageColumnCharacterLength/3);

				this.startingLocationPixelLength += (mileageColumnPixelLength/3);
				this.endLocationPixelLength += (mileageColumnPixelLength/3);
				this.programsPixelLength += (mileageColumnPixelLength/3);

				this.mileageColumnsCount--;
			}
			if( profImprvMileage == 0 ){
				this.ProfImprvmntColumnPresent = false;
				this.startLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.endLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.businessPurposeCharacterLength += (mileageColumnCharacterLength/3);

				this.startingLocationPixelLength += (mileageColumnPixelLength/3);
				this.endLocationPixelLength += (mileageColumnPixelLength/3);
				this.programsPixelLength += (mileageColumnPixelLength/3);


				this.mileageColumnsCount--;
			}
			if( UKMileage == 0 ){
				this.UKColumnPresent = false;

				this.startLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.endLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.businessPurposeCharacterLength += (mileageColumnCharacterLength/3);


				this.startingLocationPixelLength += (mileageColumnPixelLength/3);
				this.endLocationPixelLength += (mileageColumnPixelLength/3);
				this.programsPixelLength += (mileageColumnPixelLength/3);

				this.mileageColumnsCount--;
			}
		}

		private void getLines(){
			_sectionLines = new List<MileageNumLines>();
			foreach( var segment in _segments){
				var ln = new MileageNumLines();
				ln.segment = segment;
				var StartingLocation = this.formatLocation(segment.StartigngLocation.Address);
				ln.startLocationLines = PdfBaseController.SplitLineToMultiline(StartingLocation, this.startLocationCharacterLength);
				var EndingLocation = this.formatLocation(segment.EndingLocation.Address);
				ln.endLocationLines = PdfBaseController.SplitLineToMultiline(EndingLocation, this.endLocationCharacterLength);
				ln.purposeLines = PdfBaseController.SplitLineToMultiline(segment.segment.BusinessPurpose, this.businessPurposeCharacterLength);
				ln.lines = Math.Max(ln.startLocationLines.Count(), ln.endLocationLines.Count());
				ln.lines = Math.Max(ln.lines, ln.purposeLines.Count());
				_sectionLines.Add(ln);
			}
		}

		private string formatLocation( PhysicalAddress loc ){
			List<string> startingLocationParts = new List<string>();
			if( loc.Building != null && loc.Building != ""){
				startingLocationParts.Add( loc.Building );
			}
			if( loc.Street != null && loc.Street != ""){
				startingLocationParts.Add( loc.Street );
			}
			startingLocationParts.Add( loc.City );
			startingLocationParts.Add( loc.State );
			return startingLocationParts.Aggregate((i, j) => i + ", " + j);
		}



		public void CalculatePageData(){
			int pageSpace = pageHeight - pageMargins;
			int processedSegmentsCount = 0;
			bool signaturesAdded = false;
			do{
				int LastExpenseRevisionId = 0;
				var spaceRemaining = pageSpace;
				var pg = new MileageLogTableData();
				if(processedSegmentsCount == 0){
					pg.header = true;
					spaceRemaining -= headerHeight;
				}
				var remaining = _sectionLines.Skip(processedSegmentsCount);
				foreach( var sgmnt in remaining ){
					if( LastExpenseRevisionId != sgmnt.segment.segment.ExpenseRevisionId ){
						var cmnt = this._comments.Where( c => c.ExpenseRevisionId == sgmnt.segment.segment.ExpenseRevisionId).FirstOrDefault();
						if( cmnt != null ){
							spaceRemaining -= cmnt.lines * lineHeight;
						}
					}
					if( sgmnt.lines * lineHeight < spaceRemaining ){
						processedSegmentsCount++;
						spaceRemaining -= sgmnt.lines * lineHeight;
						pg.segments.Add(sgmnt);
					}else{
						break;
					}
					LastExpenseRevisionId = sgmnt.segment.segment.ExpenseRevisionId;
				}
				if(processedSegmentsCount == _sectionLines.Count()){
					if(spaceRemaining > signaturesHeight){
						pg.signatures = true;
						signaturesAdded = true;
					}
				}
				pages.Add(pg);
			}while( !signaturesAdded);
		}

		public int GetPageMargins(){
			return pageMargins;
		}

		public int GetHeaderHeight(){
			return headerHeight;
		}
		public int GetLineHeight(){
			return lineHeight;
		}
		public int GetSignaturesHeight(){
			return signaturesHeight;
		}
		public List<MileageComment> GetComments(){
			return _comments;
		}

		public int[] getVerticalLinesX(){
			List<int> lines = new List<int>();
			lines.Add(0);
			lines.Add(datePixelLength);
			lines.Add(datePixelLength+startingLocationPixelLength);
			lines.Add(datePixelLength+startingLocationPixelLength + this.endLocationPixelLength);
			lines.Add(datePixelLength+startingLocationPixelLength + this.endLocationPixelLength + this.purposePixelLength);
			lines.Add(datePixelLength+startingLocationPixelLength + this.endLocationPixelLength + this.purposePixelLength + this.programsPixelLength);
			for( var i = 1; i<= this.mileageColumnsCount; i++){
				lines.Add(datePixelLength+startingLocationPixelLength + this.endLocationPixelLength + this.purposePixelLength + this.programsPixelLength + i * this.mileageColumnPixelLength);
			}
			return lines.ToArray();
		}




		public List<MileageLogTableData> getData(){
			ExtractComments();
			ExtractSegments();
			AdjustCharacterLengths();
			getLines();
			CalculatePageData();
			return pages;
		}



	}

	public class MileageSegmentWithDate{
		public DateTime MileageDate;
		public MileageSegment segment;
		public ExtensionEventLocation StartigngLocation;
		public ExtensionEventLocation EndingLocation;
	}

	public class MileageComment{
		public int ExpenseRevisionId;
		public string comment;
		public List<string> commentLines;
		public int lines;
	}

	public class MileageLogPageData{
		public bool header = false;
		public bool signatures = false;
		public List<ExpenseMileageLogTableData> data = new List<ExpenseMileageLogTableData>();

	}

	public class MileageLogTableData{
		public bool header = false;
		public bool signatures = false;
		public List<MileageNumLines> segments = new List<MileageNumLines>();
	}

	public class MileageNumLines{
		public int lines;
		public MileageSegmentWithDate segment;
		public List<string> startLocationLines;
		public List<string> endLocationLines;
		public List<string> purposeLines;
	}


}