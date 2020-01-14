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
											.Where( b => b.UniqueCode == uniqueId)
											.Include( b => b.Reports)
											.Include( b => b.PlanningUnit)
											.Include( b => b.TypeForm)
											.Include( b => b.FarmerForReport)
											.FirstOrDefault();
					if( sample != null){
						foreach( var report in sample.Reports){
							ReportPerCrop( document, report, sample);
						}
                    }
                    
					document.Close();

            	return new FileStreamResult(stream.DetachAsData().AsStream(), "application/pdf");	
			}			
		}

		private void ReportPerCrop(SKDocument document, SoilReport report, SoilReportBundle bundle){
			using (var pdfCanvas = document.BeginPage(width, height)){
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

				//Horizontal Lines

				 SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
			
			    pdfCanvas.DrawLine(29,80, 612 - 29, 80, thinLinePaint);

				pdfCanvas.DrawLine(320,120, 612 - 29, 120, thinLinePaint);


				// Under the header

				pdfCanvas.DrawText("REPORT TYPE: " + bundle.TypeForm.Code, 29, 95, getPaint(10.0f, 1));
				pdfCanvas.DrawText("LAB NUM: " + report.LabNum, 29, 112, getPaint(10.0f, 1));

				// Farmer Address
				if(bundle.FarmerForReport != null){
					pdfCanvas.DrawText(bundle.FarmerForReport.First + " " + bundle.FarmerForReport.Last, 205, 95, getPaint(10.0f, 1));
					pdfCanvas.DrawText(bundle.FarmerForReport.Address , 205, 112, getPaint(10.0f, 1));
					pdfCanvas.DrawText(bundle.FarmerForReport.City + ", " + bundle.FarmerForReport.St + " " + bundle.FarmerForReport.Zip , 205, 129, getPaint(10.0f, 1));
					pdfCanvas.DrawText(bundle.FarmerForReport.HomeNumber , 205, 146, getPaint(10.0f, 1));
					pdfCanvas.DrawText(bundle.FarmerForReport.EmailAddress , 205, 163, getPaint(10.0f, 1));
				}
				var signee = _soilContext.FormTypeSignees
									.Where( s => s.TypeForm == bundle.TypeForm && s.PlanningUnit == bundle.PlanningUnit )
									.FirstOrDefault();
				if( signee != null){
					if(signee.Signee != null ) pdfCanvas.DrawText(signee.Signee, 320, 146, getPaint(10.0f, 1));
					if(signee.Title != null ) pdfCanvas.DrawText(signee.Title, 320, 163, getPaint(10.0f, 1));
				}

				// Bordered Rectangle

				var rect = new SKRect(29, 180, 612 - 29, 250 );
				SKPaint thickLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 1.5f
											};
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
						pdfCanvas.DrawText(val.ToString(), coordinates[position], getPaint(10.0f, 1));
						position++;
						if(position > 8) break;
					} 
				}











				using (var paint = new SKPaint()
					{
						Color = SKColors.Red,
						Style = SKPaintStyle.Fill,
						TextSize = 10,
					})
					{
						var area = SKRect.Create(50, 400, 300, 300);

						// Background
						pdfCanvas.DrawRect(area, paint);

						paint.Color = SKColors.White;
						this.DrawText(pdfCanvas,report.Comment1, area, paint);
					}



				document.EndPage();
			}
		}








		public class Line
        {
            public string Value { get; set; }

            public float Width { get; set; }
        }

        private void DrawText(SKCanvas canvas, string text, SKRect area, SKPaint paint)
        {
            float lineHeight = paint.TextSize * 1.2f;
            var lines = SplitLines(text, paint, area.Width);
            var height = lines.Count() * lineHeight;

            var y = area.MidY - height / 2;

            foreach (var line in lines)
            {
                y += lineHeight;
                var x = area.MidX - line.Width / 2;
                canvas.DrawText(line.Value, x, y, paint);
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
