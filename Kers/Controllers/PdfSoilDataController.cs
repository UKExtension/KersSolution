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
using Kers.Models.Entities.SoilData;

namespace Kers.Controllers
{

    
	[Route("api/[controller]")]
    public class PdfSoilDataController : PdfBaseController
    {
        private SoilDataContext _soilContext;

		const int width = 612;
		const int height = 792;

		string[] typefaceNames = {	"HelveticaNeue", "HelveticaNeue-Bold", 
									"HelveticaNeue-CondensedBold", "HelveticaNeue-Light"
								};


		int currentYPosition = 0;
        public PdfSoilDataController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
            SoilDataContext _soilContext,
			KERSmainContext mainContext,
			IMemoryCache _cache
        ):
		base(_context, userRepo, mainContext, _cache){
            this._soilContext = _soilContext;
        }

        [HttpGet("report/{uniqueId}")]
		public IActionResult ReportsPdf(string uniqueId)
        {
			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata())) {
					
					var sample = this._soilContext.SoilReportBundle
											.Where( b => b.UniqueCode == uniqueId && b.Reports.Count() > 0)
											.Include( b => b.Reports)
											.Include( b => b.PlanningUnit)
											.Include( b => b.TypeForm)
											.Include( b => b.FarmerForReport)
											.FirstOrDefault();
					if( sample != null){
						foreach( var report in sample.Reports){
							ReportPerCrop( document, report, sample);
						}
                    }else{
						this.Log( uniqueId,"SoilReportBundle", "Error in finding SoilReportBundle attempt.", "SoilReportBundle", "Error");
                		return new StatusCodeResult(500);
					}
                    
					document.Close();

            	//return new FileStreamResult(stream.DetachAsData().AsStream(), "application/pdf");	
				return File(stream.DetachAsData().AsStream(), "application/pdf", "SoilTestResult_"+sample.SampleLabelCreated.ToString("MM_dd_yyyy ")+".pdf");	
			}			
		}

		private void ReportPerCrop(SKDocument document, SoilReport report, SoilReportBundle bundle){
				var pdfCanvas = document.BeginPage(width, height);
				ReportHeader(pdfCanvas, report, bundle);
				UnderTheHeader(pdfCanvas, report, bundle);
				CropInfo(pdfCanvas, report, bundle);
				currentYPosition = 265;
				ExtraInfo(pdfCanvas, report);
				TestResults(pdfCanvas, report);
				pdfCanvas = LimeComment(pdfCanvas, report, document);
				pdfCanvas = AgentComment(pdfCanvas, report, document);
				pdfCanvas = Comments(pdfCanvas, report, document);
				BottomNote(pdfCanvas);
				document.EndPage();
			
		}

		private void ReportHeader(SKCanvas pdfCanvas, SoilReport report, SoilReportBundle bundle){
			//Logo
			addBitmap(pdfCanvas);
			//Header Right
			pdfCanvas.DrawText("Soil Test Report", 452, 42, getPaint(17.0f, 1));
			if(bundle.LabTestsReady != null){
				pdfCanvas.DrawText(bundle.LabTestsReady.ToString("d"), 490, 60, getPaint(10.0f, 1));
			}
			//County Office Address
			var unit =  _context.PlanningUnit.Where( u => u.Id == bundle.PlanningUnit.PlanningUnitId).FirstOrDefault();   
			pdfCanvas.DrawText(unit.FullName, 29, 29, getPaint(12.0f, 1));
			pdfCanvas.DrawText(unit.Address, 29, 42, getPaint(10.0f));
			pdfCanvas.DrawText(unit.City + ", KY " + unit.Zip, 29, 54, getPaint(10.0f));
			pdfCanvas.DrawText(unit.Phone, 29, 66, getPaint(10.0f));
			//Horizontal Line
			pdfCanvas.DrawLine(29, 80, width - 29, 80, thinLinePaint);
		}

		private void UnderTheHeader(SKCanvas pdfCanvas, SoilReport report, SoilReportBundle bundle){
			pdfCanvas.DrawText("REPORT TYPE: " + bundle.TypeForm.Code, 29, 95, getPaint(10.0f, 1));
			pdfCanvas.DrawText("LAB NUM: " + report.LabNum, 29, 112, getPaint(10.0f, 1));

			// Farmer Address
			if(bundle.FarmerForReport != null){
				pdfCanvas.DrawText(bundle.FarmerForReport.First??"" + " " + bundle.FarmerForReport.Last??"", 205, 95, getPaint(10.0f, 1));
				pdfCanvas.DrawText(bundle.FarmerForReport.Address??"" , 205, 112, getPaint(10.0f, 1));
				pdfCanvas.DrawText(bundle.FarmerForReport.City??"" + ", " + bundle.FarmerForReport.St + " " + bundle.FarmerForReport.Zip , 205, 129, getPaint(10.0f, 1));
				pdfCanvas.DrawText(bundle.FarmerForReport.HomeNumber??"" , 205, 146, getPaint(10.0f, 1));
				pdfCanvas.DrawText(bundle.FarmerForReport.EmailAddress??"" , 205, 163, getPaint(10.0f, 1));
			}

			pdfCanvas.DrawLine(330, 120, width - 29, 120, thinLinePaint);
			var signee = _soilContext.FormTypeSignees
								.Where( s => s.TypeForm == bundle.TypeForm && s.PlanningUnit == bundle.PlanningUnit )
								.FirstOrDefault();
			if( signee != null){
				if(signee.Signee != null ) pdfCanvas.DrawText(signee.Signee, 330, 146, getPaint(10.0f, 1));
				if(signee.Title != null ) pdfCanvas.DrawText(signee.Title, 330, 163, getPaint(10.0f, 1));
			}
		}

		private void CropInfo(SKCanvas pdfCanvas, SoilReport report, SoilReportBundle bundle){
			// Bordered Rectangle

			var rect = new SKRect(29, 180, 612 - 29, 250 );
			
			pdfCanvas.DrawRect( rect, thickLinePaint);

			// Inside the rectangle
			var startingX = 29;
			var startingY = 190;
			var rectHeight = 72;
			SKPoint[] coordinates  = new SKPoint[] {
				new SKPoint( startingX + 5, startingY + 5),
				new SKPoint( startingX + (612 - 58)/3, startingY + 5),
				new SKPoint( startingX + (612 - 58)/3*2, startingY + 5),
				new SKPoint( startingX + 5, startingY + rectHeight/3),
				new SKPoint( startingX + (612 - 58)/3, startingY + rectHeight/3),
				new SKPoint( startingX + (612 - 58)/3*2, startingY + rectHeight/3),
				new SKPoint( startingX + 5, startingY + rectHeight/3*2),
				new SKPoint( startingX + (612 - 58)/3, startingY + rectHeight/3*2),
				new SKPoint( startingX + (612 - 58)/3*2, startingY + rectHeight/3*2)
			};
			var position = 0;
			for( var i = 1; i < 12; i++){
				var val = report.GetType().GetProperty("CropInfo" + i.ToString())?.GetValue(report, null);
				if(val != null){
					pdfCanvas.DrawText(val.ToString(), coordinates[position], getPaint(9.5f, 1));
					position++;
					if(position > 8) break;
				} 
			}
		}
		private void ExtraInfo(SKCanvas pdfCanvas, SoilReport report){
			if(report.ExtInfo1 != null){
				pdfCanvas.DrawText(report.ExtInfo1, 29, currentYPosition, getPaint(10.0f, 1));
				currentYPosition+=12;
			}
			if(report.ExtInfo2 != null){
				pdfCanvas.DrawText(report.ExtInfo2, 29, currentYPosition, getPaint(10.0f, 1));
				currentYPosition+=12;
			}
			if(report.ExtInfo3 != null){
				pdfCanvas.DrawText(report.ExtInfo3, 29, currentYPosition, getPaint(10.0f, 1));
				currentYPosition+=12;
			}
		}
		private void TestResults(SKCanvas pdfCanvas, SoilReport report){
			var results = _soilContext.TestResults
								.Where( r => r.PrimeIndex == report.Prime_Index)
								.OrderBy( r => r.Order);
			int[] tablePositionsX = {36, 126, 216, 316, 416, 516};
			int[] verticalLinesX = {29, 121, 211, 311, 411, 511, width - 29};
			using (var paint = new SKPaint()
					{
						Color = SKColors.LightGray,
						Style = SKPaintStyle.Fill,
						TextSize = 10,
					}){
				var area = SKRect.Create(29, currentYPosition, width - 58, 22);
				pdfCanvas.DrawRect(area, paint);
			}
			pdfCanvas.DrawLine(29,currentYPosition, width - 29, currentYPosition, thinLinePaint);
			foreach( var position in verticalLinesX){
				pdfCanvas.DrawLine(position,currentYPosition,position, currentYPosition + 22, thinLinePaint);
			}
			currentYPosition += 15;
			// Background
			
			pdfCanvas.DrawText("Determination", tablePositionsX[0], currentYPosition, getPaint(9.0f, 1));
			pdfCanvas.DrawText("Result", tablePositionsX[1], currentYPosition, getPaint(9.0f, 1));
			pdfCanvas.DrawText("Unit", tablePositionsX[2], currentYPosition, getPaint(9.0f, 1));
			pdfCanvas.DrawText("Level", tablePositionsX[3], currentYPosition, getPaint(9.0f, 1));
			pdfCanvas.DrawText("Recomendation", tablePositionsX[4], currentYPosition, getPaint(9.0f, 1));
			pdfCanvas.DrawText("Test Method", tablePositionsX[5], currentYPosition, getPaint(9.0f, 1));
			currentYPosition += 7;
			pdfCanvas.DrawLine(29,currentYPosition, width - 29, currentYPosition, thinLinePaint);
			foreach( var result in results){
				TestRow(pdfCanvas,result,tablePositionsX, verticalLinesX);
			}
					
		}
		private void TestRow(SKCanvas pdfCanvas, TestResults result, int[] tablePositionsX, int[] verticalLinesX){
			var rowHeight = 18;
			foreach( var position in verticalLinesX){
				pdfCanvas.DrawLine(position,currentYPosition,position, currentYPosition + rowHeight, thinLinePaint);
			}
			currentYPosition += 12;	
			pdfCanvas.DrawText(result.TestName??"", tablePositionsX[0], currentYPosition, getPaint(8.0f, 2));
			pdfCanvas.DrawText(result.Result??"", tablePositionsX[1], currentYPosition, getPaint(8.0f, 2));
			pdfCanvas.DrawText(result.Unit??"", tablePositionsX[2], currentYPosition, getPaint(8.0f, 2));
			pdfCanvas.DrawText(result.Level??"", tablePositionsX[3], currentYPosition, getPaint(8.0f, 2));
			pdfCanvas.DrawText(result.Recommmendation??"", tablePositionsX[4], currentYPosition, getPaint(8.0f, 2));
			pdfCanvas.DrawText(result.SuppInfo1??"", tablePositionsX[5], currentYPosition, getPaint(8.0f, 2));
			currentYPosition += (rowHeight - 12);
			pdfCanvas.DrawLine(29,currentYPosition, width - 29, currentYPosition, thinLinePaint);
		}
		private SKCanvas LimeComment(SKCanvas pdfCanvas, SoilReport report, SKDocument document){
			var paint = getPaint(9.0f);
			var lineHeight = 12;
			if(report.LimeComment != null){
				
				var lines = this.SplitLines(report.LimeComment, paint, width - 2 * 29);
				if( currentYPosition + lines.Count() * lineHeight + 20 > height - 2*29){
					BottomNote(pdfCanvas);
					document.EndPage();
					pdfCanvas = document.BeginPage(width, height);
					currentYPosition = 29;
				}
				pdfCanvas.DrawText("Lime Recommendation:", 29, currentYPosition + 19, getPaint(9.0f, 1));
				currentYPosition += 20;
				SKRect area = new SKRect(29,currentYPosition, width - 29, currentYPosition + lines.Count() * lineHeight);
				
				this.DrawText(pdfCanvas, report.LimeComment, area, paint);
				currentYPosition += (lines.Count() * lineHeight);
			}
			return pdfCanvas;
		}

		private SKCanvas AgentComment(SKCanvas pdfCanvas, SoilReport report, SKDocument document){
			var paint = getPaint(9.0f);
			var lineHeight = 12;
			if(report.AgentNote != null){
				
				var lines = this.SplitLines(report.AgentNote, paint, width - 2 * 29);

				if( currentYPosition + lines.Count() * lineHeight + 20 > height - 2*29){
					BottomNote(pdfCanvas);
					document.EndPage();
					pdfCanvas = document.BeginPage(width, height);
					currentYPosition = 29;
				}
				pdfCanvas.DrawText("Extension Agent Note:", 29, currentYPosition + 19, getPaint(9.0f, 1));
				currentYPosition += 20;
				SKRect area = new SKRect(29,currentYPosition, width - 29, currentYPosition + lines.Count() * lineHeight);
				
				this.DrawText(pdfCanvas, report.AgentNote, area, paint);
				currentYPosition += (lines.Count() * lineHeight);
			}
			return pdfCanvas;
		}

		private SKCanvas Comments(SKCanvas pdfCanvas, SoilReport report, SKDocument document){
			pdfCanvas.DrawText("Comments:", 29, currentYPosition + 19, getPaint(9.0f, 1));
			currentYPosition += 20;
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment1, document);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment2, document);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment3, document);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment4, document);
			return pdfCanvas;
		}
		private SKCanvas DisplayComment(SKCanvas pdfCanvas, string comment, SKDocument document){
			var paint = getPaint(9.0f);
			var lineHeight = 12;
			if(comment != null){
				
				var lines = this.SplitLines(comment, paint, width - 2 * 29);
				if( currentYPosition + lines.Count() * lineHeight + 10 > height - 2*29){
					BottomNote(pdfCanvas);
					document.EndPage();
					pdfCanvas = document.BeginPage(width, height);
					currentYPosition = 29;
				}
				currentYPosition += 10;
				SKRect area = new SKRect(29,currentYPosition, width - 29, currentYPosition + lines.Count() * lineHeight);
				this.DrawText(pdfCanvas, comment, area, paint);
				currentYPosition += (lines.Count() * lineHeight);
			}
			return pdfCanvas;
		}

		private void BottomNote(SKCanvas pdfCanvas){

			SKRect bottomArrea = new SKRect(29,height - 45 , width - 29, height - 15);
			var text = "Education programs of the Kentucky Cooperative Extension Service serve all people regardless of race, color, age, sex,\n religion, disability, or national origin. UNIVERSITY OF KENTUCKY, KENTUCKY STAE UNIVERSITY, U.S. DEPARTMENT OF AGRICULTURE\n AND KENTUCKY COUNTIES COOPERATING";
			this.DrawText(pdfCanvas, text, bottomArrea, getPaint(8.0f), "center");
			
		}



		public class Line
        {
            public string Value { get; set; }

            public float Width { get; set; }
        }

        private void DrawText(SKCanvas canvas, string text, SKRect area, SKPaint paint, string align = "left")
        {
            float lineHeight = paint.TextSize * 1.2f;
            var lines = SplitLines(text, paint, area.Width);
            var height = lines.Count() * lineHeight;

            var y = area.MidY - height / 2;

            foreach (var line in lines)
            {
                y += lineHeight;
				if(align == "center"){
					var x = area.MidX - line.Width / 2;
					canvas.DrawText(line.Value, x, y, paint);
				}else{
					canvas.DrawText(line.Value, area.Left, y, paint);
				}
                
                
            }
        }

        private Line[] SplitLines(string text, SKPaint paint, float maxWidth)
        {
            var spaceWidth = paint.MeasureText(" ");
            var lines = text.Split('\n');

            return lines.SelectMany((line) =>
            {
                var result = new List<Line>();

                var words = line.Split(new[] { " " }, StringSplitOptions.None);

                var lineResult = new StringBuilder();
                float width = 0;
                foreach (var word in words)
                {
					if(word != ""){
						var wordWidth = paint.MeasureText(word);
						var wordWithSpaceWidth = wordWidth + spaceWidth;
						var wordWithSpace = word + " ";

						if (width + wordWidth > maxWidth)
						{
							result.Add(new Line() { Value = lineResult.ToString(), Width = width });
							lineResult = new StringBuilder(wordWithSpace);
							width = wordWithSpaceWidth;
						}
						else
						{
							lineResult.Append(wordWithSpace);
							width += wordWithSpaceWidth;
						}
					}
                }


                result.Add(new Line() { Value = lineResult.ToString(), Width = width });

                return result.ToArray();
            }).ToArray();
        }


    }
}
