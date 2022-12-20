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

		private int numPages;
		private int currentPage;

		private int[] packSlipTableVerticalLines = new int[]{ 29, 50, 180, 240, 280, 360, 480, 586 };


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



		[HttpPost("reports")]
		public IActionResult ConsolidatedReportsPdf([FromBody] UniqueIds unigueIds)
        {
			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata("Kers, Soil Testing, Consolidated Report", "Soil Test Reports", "Summary Soil Test Report"))) {
					var samples = this._soilContext.SoilReportBundle
											.Where( b => unigueIds.ids.Contains(b.UniqueCode) && b.Reports.Count() > 0)
											.Include( b => b.Reports)
											.Include( b => b.PlanningUnit)
											.Include( b => b.TypeForm)
											.Include( b => b.FarmerForReport)
											.Include( b => b.LastStatus).ThenInclude( s => s.SoilReportStatus)
											.OrderBy( b => b.CoSamnum)
											.ToList();
					foreach( var sample in samples){
						if( sample != null){
							foreach( var report in sample.Reports){
								ReportPerCrop( document, report, sample);
							}
							if( sample.LastStatus == null || sample.LastStatus.SoilReportStatus.Name != "Archived"){
								sample.LastStatus = new SoilReportStatusChange();
								sample.LastStatus.Created = DateTime.Now;
								sample.LastStatus.SoilReportStatus = _soilContext.SoilReportStatus.Where( s => s.Name == "Archived").FirstOrDefault();
								_soilContext.SaveChanges();
							}
						}else{
							this.Log( unigueIds,"SoilReportBundle", "Error in finding SoilReportBundle attempt.", "SoilReportBundle", "Error");
							return new StatusCodeResult(500);
						}
					}
					this.AddressesPage(samples, document);
				document.Close();
				return File(stream.DetachAsData().AsStream(), "application/pdf", "SoilTestResults.pdf");
			}
		}


		[HttpPost("packing")]
		public IActionResult PackingSlipPdf([FromBody] UniqueIds unigueIds)
        {
			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata("Kers, Soil Testing, Consolidated Report", "Soil Test Reports", "Summary Soil Test Report"))) {
					PackingSlipData data = new PackingSlipData(this._soilContext, unigueIds);
					var numPackSlipPages = data.pages.Count();
					var currentPackSlipPage = 1;
					foreach( var pageData in data.pages ){
						PackSlipPage(document, pageData, currentPackSlipPage++, numPackSlipPages);
					}
					var newStatus = this._soilContext.SoilReportStatus.Where( s => s.Name == "Sent").FirstOrDefault();
					foreach( var sample in data.samples){
						sample.LastStatus.SoilReportStatus = newStatus;
					}
					this._soilContext.SaveChanges();
					document.Close();
					return File(stream.DetachAsData().AsStream(), "application/pdf", "SoilTestResults.pdf");
			}
		}

		

		private void AddressesPage(List<SoilReportBundle> samples, SKDocument document){
			var usableWidth = width - 30;
			var usableHeight = height - 30;
			
			var index = 0;

			var grouppedBundles = samples.Where( s => s.FarmerAddressId != null)
											.GroupBy( s => s.FarmerAddressId )
											.Select( s => s.Key);

			var addresses = new List<FarmerAddress>();
			foreach( var FarmerId in grouppedBundles ) addresses.Add( this._soilContext.FarmerAddress.Find(FarmerId));
			var numPages = Math.Ceiling( (decimal) (addresses.Count() / 30));

			for( var i = 0; i <= numPages; i++ ){
				var pdfCanvas = document.BeginPage(width, height);
				for( int Xpos = 35; Xpos < usableWidth ; Xpos = Xpos + (usableWidth / 3) ){
					for( int Ypos = 35; Ypos < usableHeight ; Ypos = Ypos + usableHeight /10 ){
						if( index < addresses.Count()){
							var sample = addresses.ElementAt(index);
							if( sample != null ){
								var farmerName = (sample.Title != null && sample.Title != "" && sample.Title != " " ? sample.Title + " ": "")
														+
												(sample.First != null && sample.First != "" && sample.First != " " ? sample.First + " ": "")
														+
												(sample.Mi != null && sample.Mi != "" && sample.Mi != " " ? sample.Mi + " ": "")
														+
												(sample.Last != null ? sample.Last: "");
								pdfCanvas.DrawText(farmerName , Xpos , Ypos  , getPaint(11.0f, 1));
								pdfCanvas.DrawText(sample.Address , Xpos , Ypos + 15 , getPaint(11.0f));
								var farmerCity = (sample.City != null ? sample.City + ", ": "")
													+
												(sample.St != null ? sample.St + " ": "")
													+
												(sample.Zip != null ? sample.Zip + " ": "");
								pdfCanvas.DrawText(farmerCity , Xpos , Ypos + 30 , getPaint(11.0f));
							}
							
						}
						
						index++;
					}
				}
				document.EndPage();
			}

		}


        [HttpGet("report/{uniqueId}")]
		public IActionResult ReportsPdf(string uniqueId)
        {
			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata("Kers, Soil Testing, Soil Test Report", "Soil Test Reports", "Detailed Soil Test Report"))) {
					
					var sample = this._soilContext.SoilReportBundle
											.Where( b => b.UniqueCode == uniqueId && b.Reports.Count() > 0)
											.Include( b => b.Reports)
											.Include( b => b.PlanningUnit)
											.Include( b => b.TypeForm)
											.Include( b => b.FarmerForReport)
											.Include( b => b.LastStatus).ThenInclude( s => s.SoilReportStatus)
											.FirstOrDefault();
					if( sample != null){
						foreach( var report in sample.Reports){
							ReportPerCrop( document, report, sample);
						}
						if( sample.LastStatus == null || sample.LastStatus.SoilReportStatus.Name != "Archived"){
							sample.LastStatus = new SoilReportStatusChange();
							sample.LastStatus.Created = DateTime.Now;
							sample.LastStatus.SoilReportStatus = _soilContext.SoilReportStatus.Where( s => s.Name == "Archived").FirstOrDefault();
							_soilContext.SaveChanges();
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
				numPages = NumPages(report);
				currentPage = 1;
				ReportHeader(pdfCanvas, report, bundle);
				currentYPosition = 177;
				UnderTheHeader(pdfCanvas, report, bundle);
				CropInfo(pdfCanvas, report, bundle);
				
				ExtraInfo(pdfCanvas, report);
				TestResults(pdfCanvas, report);
				pdfCanvas = LimeComment(pdfCanvas, report, document);
				pdfCanvas = AgentComment(pdfCanvas, report, document);
				pdfCanvas = Comments(pdfCanvas, report, document);
				BottomNote(pdfCanvas);
				document.EndPage();
			
		}

		private void ReportHeader(SKCanvas pdfCanvas, SoilReport report, SoilReportBundle bundle){
			//Page Info
			PrintPageInfo(pdfCanvas, report);
			//Logo
			addBitmap(pdfCanvas);
			//Header Right
			pdfCanvas.DrawText("Soil Test Report", 452, 42, getPaint(17.0f, 1));
			//if(bundle.LabTestsReady != null){
				pdfCanvas.DrawText(bundle.DataProcessed.ToString("d"), 490, 60, getPaint(10.0f, 1));
			//}
			//County Office Address
			var unit =  _context.PlanningUnit.Where( u => u.Id == bundle.PlanningUnit.PlanningUnitId).FirstOrDefault();  
			if( unit != null){
				pdfCanvas.DrawText(unit.FullName??"", 29, 31, getPaint(12.0f, 1));
				pdfCanvas.DrawText(unit.Address??"", 29, 44, getPaint(10.0f));
				pdfCanvas.DrawText(unit.City??"" + ", KY " + unit.Zip, 29, 56, getPaint(10.0f));
				pdfCanvas.DrawText(unit.Phone??"", 29, 68, getPaint(10.0f));
				//Horizontal Line
				pdfCanvas.DrawLine(29, 80, width - 29, 80, thinLinePaint);
			}
			
		}

		private void PrintPageInfo( SKCanvas pdfCanvas, SoilReport report ){
			pdfCanvas.DrawText("CO NUM: "+report.CoSamnum+", "+report.CropInfo1??"", 29, 16, getPaint(7.0f));
			pdfCanvas.DrawText("Page "+currentPage+" of "+numPages.ToString(), width - 65, 16, getPaint(7.0f));
			pdfCanvas.DrawText( "Report Generated: " + DateTime.Now.ToString(), 29, height - 16, getPaint(7.0f) );
		}

		


		private void UnderTheHeader(SKCanvas pdfCanvas, SoilReport report, SoilReportBundle bundle){
			pdfCanvas.DrawText("REPORT TYPE: " + report.TypeForm, 29, 95, getPaint(10.0f, 1));
			pdfCanvas.DrawText("LAB NUM: " + report.LabNum, 29, 112, getPaint(10.0f, 1));
			pdfCanvas.DrawText("CO NUM: " + bundle.CoSamnum, 29, 129, getPaint(10.0f, 1));
			if(bundle.Acres != null && bundle.Acres != "0"){
				pdfCanvas.DrawText("ACRES: " + bundle.Acres, 29, 146, getPaint(10.0f, 1));
			}

			// Farmer Address
			if(bundle.FarmerForReport != null){
				pdfCanvas.DrawText(bundle.FarmerForReport.First + " " + bundle.FarmerForReport.Last, 205, 95, getPaint(10.0f, 1));
				pdfCanvas.DrawText(bundle.FarmerForReport.Address??"" , 205, 112, getPaint(10.0f, 1));
				pdfCanvas.DrawText(bundle.FarmerForReport.City + ", " + bundle.FarmerForReport.St + " " + bundle.FarmerForReport.Zip , 205, 129, getPaint(10.0f, 1));
				pdfCanvas.DrawText(bundle.FarmerForReport.HomeNumber??"" , 205, 146, getPaint(10.0f, 1));
				pdfCanvas.DrawText(bundle.FarmerForReport.EmailAddress??"" , 205, 163, getPaint(10.0f, 1));
			}
			if(report.NoteByKersUserId != null){
				
				var user = _context.KersUser.Where( u => u.Id == report.NoteByKersUserId)
					.Include( u => u.PersonalProfile )
					.FirstOrDefault();
				if(user != null){
					pdfCanvas.DrawText(user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName, 370, 125, getPaint(18.0f, 4));
					pdfCanvas.DrawText(user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName, 370, 146, getPaint(10.0f, 1));
					if(user.PersonalProfile.ProfessionalTitle != null ) pdfCanvas.DrawText(user.PersonalProfile.ProfessionalTitle, 370, 163, getPaint(10.0f, 1));
				}
			}else{
				var signee = _soilContext.FormTypeSignees
								.Where( s => s.TypeForm == bundle.TypeForm && s.PlanningUnit == bundle.PlanningUnit )
								.FirstOrDefault();
				if( signee != null){
					if(signee.Signee != null ) pdfCanvas.DrawText(signee.Signee, 370, 146, getPaint(10.0f, 1));
					if(signee.Title != null ) pdfCanvas.DrawText(signee.Title, 370, 163, getPaint(10.0f, 1));
				}
			}
			pdfCanvas.DrawLine(370, 130, width - 29, 130, thinLinePaint);
			

			if( bundle.OwnerID != null && bundle.OwnerID != ""){
				pdfCanvas.DrawText("OWNER SAMPLE ID: "+bundle.OwnerID, 29, currentYPosition, getPaint(10.0f, 1));
				currentYPosition += 2;
			}
		}

		private void CropInfo(SKCanvas pdfCanvas, SoilReport report, SoilReportBundle bundle){
			// Bordered Rectangle
			
			//var rect = new SKRect(29, 180, 612 - 29, 250 );
			
			//pdfCanvas.DrawRect( rect, thickLinePaint);

			// Inside the rectangle
			
			
			
			var cropInfo = "";
			for( var i = 1; i < 12; i++){
				var val = report.GetType().GetProperty("CropInfo" + i.ToString())?.GetValue(report, null);
				if(val != null){
					cropInfo +=  " . . . . " + val;
				} 
			}

			var paint = getPaint(9.0f);
			var lineHeight = 12;
			if(cropInfo != ""){
				currentYPosition += 3;
				pdfCanvas.DrawLine(29, currentYPosition, width - 29, currentYPosition, thinLinePaint);
				
				
				
				
				currentYPosition += 12;
				var header = "AGRICULTURE CROP INFORMATION:";
				if(report.TypeForm == "H") header = "HOME LAWN AND GARDEN CROP INFORMATION:";
				if(report.TypeForm == "C") header = "COMMERCIAL HORTICULTURE CROP INFORMATION:";
				pdfCanvas.DrawText(header, 29, currentYPosition, getPaint(9.0f, 1));
				

				var lines = this.SplitLines(cropInfo, paint, width - 2 * 29);
				SKRect area = new SKRect(29,currentYPosition, width - 29, currentYPosition + lines.Count() * lineHeight);
				
				this.DrawText(pdfCanvas, cropInfo, area, paint);
				currentYPosition += (lines.Count() * lineHeight + 6);
			}



			
			
			


			/* 

			var startingY = currentYPosition;
			var rectHeight = 72;
			var startingX = 29;
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
			currentYPosition += rectHeight;
			
			 */
			
		}
		private void ExtraInfo(SKCanvas pdfCanvas, SoilReport report){
			var extraPresent = false;
			if(report.Extra1 != null){
				extraPresent = true;
				pdfCanvas.DrawLine(29, currentYPosition, width - 29, currentYPosition, thinLinePaint);
				currentYPosition += 10;
				pdfCanvas.DrawText(report.Extra1, 29, currentYPosition, getPaint(10.0f, 1));
				currentYPosition+=12;
			}
			if(report.Extra2 != null){
				if( !extraPresent ){
					pdfCanvas.DrawLine(29, currentYPosition, width - 29, currentYPosition, thinLinePaint);
					currentYPosition += 10;
					extraPresent = true;
				}
				pdfCanvas.DrawText(report.Extra2, 29, currentYPosition, getPaint(10.0f, 1));
				currentYPosition+=12;
			}
			if(report.Extra3 != null){
				if( !extraPresent ){
					pdfCanvas.DrawLine(29, currentYPosition, width - 29, currentYPosition, thinLinePaint);
					currentYPosition += 10;
					extraPresent = true;
				}
				pdfCanvas.DrawText(report.Extra3, 29, currentYPosition, getPaint(10.0f, 1));
				currentYPosition+=12;
			}
			if(extraPresent) currentYPosition -= 3;
		}
		private void TestResults(SKCanvas pdfCanvas, SoilReport report){
			var results = _soilContext.TestResults
								.Where( r => r.PrimeIndex == report.Prime_Index)
								.OrderBy( r => r.Order);
			int[] tablePositionsX = {36, 126, 186, 316, 416, 489};
			int[] verticalLinesX = {29, 121, 181, 486, width - 29};
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
			//pdfCanvas.DrawText("Unit", tablePositionsX[2], currentYPosition, getPaint(9.0f, 1));
			pdfCanvas.DrawText(" V Low                 Low                  Med                  High                V High", 
									tablePositionsX[2], currentYPosition, getPaint(9.0f, 1));
			pdfCanvas.DrawText("Recommendation", tablePositionsX[5], currentYPosition, getPaint(9.0f, 1));
			//pdfCanvas.DrawText("Test Method", tablePositionsX[5], currentYPosition, getPaint(9.0f, 1));
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
			pdfCanvas.DrawText((result.Result??"") + " " + (result.Unit??""), tablePositionsX[1], currentYPosition, getPaint(8.0f, 2));
			//pdfCanvas.DrawText(result.Unit??"", tablePositionsX[2], currentYPosition, getPaint(8.0f, 2));
			var lvl = (result.Level??"").ToLower();


			if(lvl == "medium"){
				SKPaint skPaint = new SKPaint()
				{
					Style = SKPaintStyle.Fill,
					Color = SKColors.Orange,
					StrokeWidth = 10,
					IsAntialias = true,
				};

				SKRect skRectangle = new SKRect();
				skRectangle.Size = new SKSize(148, rowHeight - 6);
				skRectangle.Location = new SKPoint(tablePositionsX[2], currentYPosition - rowHeight/2);

				pdfCanvas.DrawRect(skRectangle, skPaint);

			}else if(lvl == "low"){
				SKPaint skPaint = new SKPaint()
				{
					Style = SKPaintStyle.Fill,
					Color = SKColors.Red,
					StrokeWidth = 10,
					IsAntialias = true,
				};

				SKRect skRectangle = new SKRect();
				skRectangle.Size = new SKSize(79, rowHeight - 6);
				skRectangle.Location = new SKPoint(tablePositionsX[2], currentYPosition - rowHeight/2);

				pdfCanvas.DrawRect(skRectangle, skPaint);

			}else if(lvl == "very low"){
				SKPaint skPaint = new SKPaint()
				{
					Style = SKPaintStyle.Fill,
					Color = SKColors.Brown,
					StrokeWidth = 10,
					IsAntialias = true,
				};

				SKRect skRectangle = new SKRect();
				skRectangle.Size = new SKSize(23, rowHeight - 6);
				skRectangle.Location = new SKPoint(tablePositionsX[2], currentYPosition - rowHeight/2);

				pdfCanvas.DrawRect(skRectangle, skPaint);

			}else if(lvl == "very high"){
				SKPaint skPaint = new SKPaint()
				{
					Style = SKPaintStyle.Fill,
					Color = SKColors.Green,
					StrokeWidth = 10,
					IsAntialias = true,
				};

				SKRect skRectangle = new SKRect();
				skRectangle.Size = new SKSize(292, rowHeight - 6);
				skRectangle.Location = new SKPoint(tablePositionsX[2], currentYPosition - rowHeight/2);

				pdfCanvas.DrawRect(skRectangle, skPaint);

			}else if(lvl == "high"){
				SKPaint skPaint = new SKPaint()
				{
					Style = SKPaintStyle.Fill,
					Color = SKColors.LightGreen,
					StrokeWidth = 10,
					IsAntialias = true,
				};

				SKRect skRectangle = new SKRect();
				skRectangle.Size = new SKSize(212, rowHeight - 6);
				skRectangle.Location = new SKPoint(tablePositionsX[2], currentYPosition - rowHeight/2);

				pdfCanvas.DrawRect(skRectangle, skPaint);

			}else{
				pdfCanvas.DrawText(result.Level??"", tablePositionsX[2], currentYPosition, getPaint(8.0f, 2));
			}
			pdfCanvas.DrawText(result.Recommmendation??"", tablePositionsX[5], currentYPosition, getPaint(8.0f, 2));
			//pdfCanvas.DrawText(result.SuppInfo1??"", tablePositionsX[5], currentYPosition, getPaint(8.0f, 2));
			currentYPosition += (rowHeight - 12);
			pdfCanvas.DrawLine(29,currentYPosition, width - 29, currentYPosition, thinLinePaint);
		}
		private SKCanvas LimeComment(SKCanvas pdfCanvas, SoilReport report, SKDocument document){
			var paint = getPaint(9.0f);
			var lineHeight = 12;
			if(report.LimeComment != null & report.LimeComment != ""){
				
				var lines = this.SplitLines(report.LimeComment, paint, width - 2 * 29);
				if( currentYPosition + lines.Count() * lineHeight + 20 > height - 2*29){
					BottomNote(pdfCanvas);
					document.EndPage();
					pdfCanvas = document.BeginPage(width, height);
					currentPage++;
					PrintPageInfo(pdfCanvas, report);
					currentYPosition = 29;
				}
				pdfCanvas.DrawText("Soil pH Recommendation:", 29, currentYPosition + 19, getPaint(9.0f, 1));
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
			if(report.AgentNote != null & report.AgentNote != ""){
				
				var lines = this.SplitLines(report.AgentNote, paint, width - 2 * 29);

				if( currentYPosition + lines.Count() * lineHeight + 20 > height - 2*29){
					BottomNote(pdfCanvas);
					document.EndPage();
					pdfCanvas = document.BeginPage(width, height);
					currentPage++;
					PrintPageInfo(pdfCanvas, report);
					currentYPosition = 29;
				}
				SKPaint agentLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.CadetBlue,
												StrokeWidth = 3.5f
											};
				pdfCanvas.DrawLine(29,currentYPosition + 8, width - 29, currentYPosition + 8 , agentLinePaint);
				currentYPosition += 5;
				pdfCanvas.DrawText("Extension Agent Recommendation:", 190, currentYPosition + 19, getPaint(11.0f, 1));
				currentYPosition += 18;
				pdfCanvas.DrawLine(29,currentYPosition + 11, width - 29, currentYPosition + 8 , agentLinePaint);
				currentYPosition += 20;
				SKRect area = new SKRect(29,currentYPosition, width - 29, currentYPosition + lines.Count() * lineHeight);
				
				this.DrawText(pdfCanvas, report.AgentNote, area, paint);
				currentYPosition += (lines.Count() * lineHeight);
				pdfCanvas.DrawLine(29,currentYPosition + 8, width - 29, currentYPosition + 8 , agentLinePaint);
				currentYPosition += 5;
			}
			return pdfCanvas;
		}

		private SKCanvas Comments(SKCanvas pdfCanvas, SoilReport report, SKDocument document){
			pdfCanvas.DrawText("Comments:", 29, currentYPosition + 19, getPaint(9.0f, 1));
			currentYPosition += 10;
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment1, document, report);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment2, document, report);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment3, document, report);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment4, document, report);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment5, document, report);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment6, document, report);
			pdfCanvas = DisplayComment(pdfCanvas, report.Comment7, document, report);
			return pdfCanvas;
		}
		private SKCanvas DisplayComment(SKCanvas pdfCanvas, string comment, SKDocument document, SoilReport report){
			var paint = getPaint(9.0f);
			var lineHeight = 12;
			if(comment != null){
				
				var lines = this.SplitLines(comment, paint, width - 2 * 29);
				if( currentYPosition + lines.Count() * lineHeight + 10 > height - 2*29){
					BottomNote(pdfCanvas);
					document.EndPage();
					pdfCanvas = document.BeginPage(width, height);
					currentPage++;
					PrintPageInfo(pdfCanvas, report);
					currentYPosition = 29;
				}
				currentYPosition += 10;
				SKRect area = new SKRect(29,currentYPosition, width - 29, currentYPosition + lines.Count() * lineHeight);
				this.DrawText(pdfCanvas, comment, area, paint);
				currentYPosition += (lines.Count() * lineHeight);
			}
			return pdfCanvas;
		}

		private int NumPages(SoilReport report){
			var numPages = 1;
			var pageHeight = height - 2 * 30;
			var pageWidth = width - 2 * 29;
			//Header
			var runningHeight = 255;
			if(report.OsId != null && report.OsId != "") runningHeight += 12;
			if(report.ExtInfo1 != null ) runningHeight += 12;
			if(report.ExtInfo2 != null ) runningHeight += 12;
			if(report.ExtInfo3 != null ) runningHeight += 12;
			//Test Results Header
			runningHeight+=22;
			//Test Results
			var TestResultsCount = _soilContext.TestResults
								.Where( r => r.PrimeIndex == report.Prime_Index)
								.Count();
			runningHeight += TestResultsCount * 18;
			
			
			//Lime Comment
			var paint = getPaint(9.0f);
			var lineHeight = 12;
			if(report.LimeComment != null){
				var lines = this.SplitLines(report.LimeComment, paint, pageWidth);
				if( runningHeight + lines.Count() * lineHeight + 20 > pageHeight){
					numPages++;
					runningHeight = 29;
				}
				runningHeight += (lines.Count() * lineHeight);
			}

			// Agent Notes
			if(report.AgentNote != null){
				var lines = this.SplitLines(report.AgentNote, paint, pageWidth);
				if( runningHeight + lines.Count() * lineHeight + 30 > pageHeight){
					numPages++;
					runningHeight = 29;
				}
				runningHeight += ((lines.Count() * lineHeight) + 29);
			}


			//Comments
			if(report.Comment1 != null){
				var lines = this.SplitLines(report.Comment1, paint, pageWidth);
				if( runningHeight + lines.Count() * lineHeight + 10 > pageHeight){
					numPages++;
					runningHeight = 29;
				}
				runningHeight += 10;
				runningHeight += (lines.Count() * lineHeight);
			}
			if(report.Comment2 != null){
				var lines = this.SplitLines(report.Comment2, paint, pageWidth);
				if( runningHeight + lines.Count() * lineHeight + 10 > pageHeight){
					numPages++;
					runningHeight = 29;
				}
				runningHeight += 10;
				runningHeight += (lines.Count() * lineHeight);
			}
			if(report.Comment3 != null){
				var lines = this.SplitLines(report.Comment3, paint, pageWidth);
				if( runningHeight + lines.Count() * lineHeight + 10 > pageHeight){
					numPages++;
					runningHeight = 29;
				}
				runningHeight += 10;
				runningHeight += (lines.Count() * lineHeight);
			}
			if(report.Comment4 != null){
				var lines = this.SplitLines(report.Comment4, paint, pageWidth);
				if( runningHeight + lines.Count() * lineHeight + 10 > pageHeight){
					numPages++;
				}
			}
			return numPages;
		}

		private void BottomNote(SKCanvas pdfCanvas){

			SKRect bottomArrea = new SKRect(29,height - 45 , width - 29, height - 18);
			var text = "Education programs of the Kentucky Cooperative Extension Service serve all people regardless of race, color, age, sex, religion, disability,\n or national origin. UNIVERSITY OF KENTUCKY, KENTUCKY STATE UNIVERSITY, U.S. DEPARTMENT\n OF AGRICULTURE AND KENTUCKY COUNTIES COOPERATING";
			this.DrawText(pdfCanvas, text, bottomArrea, getPaint(7.0f), "center");
			
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


		/***********************************/
		// Drawing Packing Slip Pages
		/***********************************/


		private void PackSlipPage(SKDocument document, List<List<string>> pageData, int currentPage, int numPages){
			var pdfCanvas = document.BeginPage(width, height);
			PackSlipPageHeader(pdfCanvas);
			PackSlipPagePageInfo( pdfCanvas, currentPage, numPages );
			PackSlipTableHeader( pdfCanvas );
			PackSlipSampleRows( pdfCanvas, pageData );
			document.EndPage();
		}

		private void PackSlipPageHeader(SKCanvas pdfCanvas){
			pdfCanvas.DrawText( "Soil Samples Packing Slip", 52, 72, getPaint(17.0f, 1));
		}

		private void PackSlipPagePageInfo( SKCanvas pdfCanvas, int currentPage, int numPages ){
			pdfCanvas.DrawText("Page "+currentPage.ToString()+" of "+numPages.ToString(), width - 65, 16, getPaint(7.0f));
			pdfCanvas.DrawText( "Packing Slip Generated: " + DateTime.Now.ToString(), 29, height - 16, getPaint(7.0f) );
		}

		private void PackSlipTableHeader( SKCanvas pdfCanvas){
			
			var padding = 4;
			var yPos = 140;
			var yPadding = 15;
			pdfCanvas.DrawLine(29,yPos, width - 26, yPos, thinLinePaint);
			foreach( var pos in packSlipTableVerticalLines){
				pdfCanvas.DrawLine(pos,yPos, pos, yPos + 26, thinLinePaint);
			}
			pdfCanvas.DrawText("UK LAB #", packSlipTableVerticalLines[1] + padding , yPos + yPadding, getPaint(10.0f, 1));
			pdfCanvas.DrawText("Type Test", packSlipTableVerticalLines[2] + padding , yPos + yPadding, getPaint(10.0f, 1));
			pdfCanvas.DrawText("Cnt ID", packSlipTableVerticalLines[3] + padding , yPos + yPadding, getPaint(10.0f, 1));
			pdfCanvas.DrawText("Sample #", packSlipTableVerticalLines[4] + padding , yPos + yPadding, getPaint(10.0f, 1));
			pdfCanvas.DrawText("Client Name", packSlipTableVerticalLines[5] + padding , yPos + yPadding, getPaint(10.0f, 1));
			pdfCanvas.DrawText("Owner ID", packSlipTableVerticalLines[6] + padding , yPos + yPadding, getPaint(10.0f, 1));
			pdfCanvas.DrawLine(29,yPos + 20, width - 26,  yPos + 20, thinLinePaint);
		}

		private void PackSlipSampleRows( SKCanvas pdfCanvas, List<List<string>> pageData  ){

			var padding = 4;
			var yPos = 160;
			var yPadding = 18;
			var yLineHight = 30;
			var rowNum = 1;
			foreach( var row in pageData ){
				foreach( var pos in packSlipTableVerticalLines){
					pdfCanvas.DrawLine(pos,yPos, pos, yPos + yLineHight, thinLinePaint);
				}
				pdfCanvas.DrawText(rowNum.ToString(), packSlipTableVerticalLines[0] + padding , yPos + yPadding, getPaint(10.0f, 1));
				pdfCanvas.DrawText(row[0], packSlipTableVerticalLines[2] + padding , yPos + yPadding, getPaint(10.0f, 1));
				pdfCanvas.DrawText(row[1] , packSlipTableVerticalLines[3] + (packSlipTableVerticalLines[4] - packSlipTableVerticalLines[3])/2, yPos + yPadding, getPaint(10.0f, 1, 0xFF000000, SKTextAlign.Center));
				pdfCanvas.DrawText(row[2].TrimStart(new Char[] { '0' } ), packSlipTableVerticalLines[4] + (packSlipTableVerticalLines[5] - packSlipTableVerticalLines[4])/2 , yPos + yPadding, getPaint(10.0f, 1, 0xFF000000, SKTextAlign.Center));
				pdfCanvas.DrawText(row[3], packSlipTableVerticalLines[5] + padding , yPos + yPadding, getPaint(10.0f, 1));
				pdfCanvas.DrawText(row[4], packSlipTableVerticalLines[6] + padding , yPos + yPadding, getPaint(10.0f, 1));
				rowNum++;
				yPos+= yLineHight;
				pdfCanvas.DrawLine(29,yPos, width - 26, yPos, thinLinePaint);
			}
		}

		



    }

	public class PackingSlipData{

		public List<SoilReportBundle> samples;
		public List<SoilReportBundle> withOptionalTests;
		public List<SoilReportBundle> samplesByTests;
		public List<SoilReportBundle> withoutOptionalTests;
		public List<List<List<string>>>pages;




		private int LinesPerPage = 18;
		private UniqueIds ids;
		private SoilDataContext _soilContext;

		public PackingSlipData(SoilDataContext soilContext, UniqueIds unigueIds){
			this._soilContext = soilContext;
			this.ids = unigueIds;
			this.ProcessData();
		}

		public PackingSlipData(SoilDataContext soilContext, UniqueIds unigueIds, int LinesPerPage){
			this._soilContext = soilContext;
			this.LinesPerPage = LinesPerPage;
			this.ids = unigueIds;
			this.ProcessData();
		}

		private void ProcessData(){
			this.samples = this._soilContext.SoilReportBundle
											.Where( b => this.ids.ids.Contains(b.UniqueCode) 
															&& b.Reports.Count() == 0 
															&& b.LastStatus.SoilReportStatus.Name == "Entered"
													)
											.Include( b => b.PlanningUnit)
											.Include( b => b.FarmerForReport)
											.Include( b => b.OptionalTestSoilReportBundles).ThenInclude( o => o.OptionalTest)
											.Include( b => b.LastStatus)
											.OrderBy( b => b.CoSamnum)
											.ToList();


			var grouppeByTests = this.samples
										.GroupBy( s => string.Join( ',', s.OptionalTestSoilReportBundles.OrderBy( v => v.OptionalTestId).Select( v => v.OptionalTest.Code ).ToArray() ))
										.Select( g => new {
											Key = g.Key,
											Samples = g.Select( s => s)
										}).ToList();

			//this.withOptionalTests = this.samples.Where( s => s.OptionalTestSoilReportBundles.Count() > 0 ).ToList();
			//this.withoutOptionalTests = this.samples.Where( s => s.OptionalTestSoilReportBundles.Count() == 0 ).ToList();
			this.pages = new List<List<List<string>>>();
			foreach( var grp in grouppeByTests ){
				for( var i = 0; i < grp.Samples.Count(); i+=this.LinesPerPage){
					this.pages.Add(this.Page(grp.Samples.Skip(i).Take(this.LinesPerPage).ToList()));
				}
			}
/* 

			if( this.withoutOptionalTests.Count()>0){
				for( var i = 0; i < this.withoutOptionalTests.Count(); i+=this.LinesPerPage){
					this.pages.Add(this.Page(this.withoutOptionalTests.Skip(i).Take(this.LinesPerPage).ToList()));
				}
			}
			if( this.withOptionalTests.Count()>0){
				for( var i = 0; i < this.withOptionalTests.Count(); i+=this.LinesPerPage){
					this.pages.Add(this.Page(this.withOptionalTests.Skip(i).Take(this.LinesPerPage).ToList()));
				}
			}
			 */
		}


		private List<string> Row( SoilReportBundle sample ){
			var row = new List<string>();
			if( sample.OptionalTestSoilReportBundles.Count() > 0 ){
				row.Add("01," + string.Join( ',', sample.OptionalTestSoilReportBundles.Select( s => s.OptionalTest.Code).ToArray() ));
			}else{
				row.Add("01");
			}
			
			row.Add( sample.PlanningUnit.CountyID.ToString());
			row.Add( sample.CoSamnum);
			row.Add( sample.FarmerForReport.First + " " + sample.FarmerForReport.Last);
			row.Add( sample.OwnerID);
			return row;
		}

		private List<List<string>> Page( List<SoilReportBundle> samples){
			List<List<string>> page = new List<List<string>>();
			foreach( var smpl in samples ){
				page.Add(Row(smpl));
			}
			return page;
		}

	}



	public class UniqueIds{
		public List<string> ids;
	}



}
