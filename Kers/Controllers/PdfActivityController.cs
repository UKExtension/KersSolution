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
    public class PdfActivityController : PdfBaseController
    {

		

		const int width = 612;
		const int height = 792;

		IActivityRepository activityRepo;


        public PdfActivityController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
			KERSmainContext mainContext,
			IActivityRepository activityRepo,
			IMemoryCache _cache
        ):
		base(_context, userRepo, mainContext, _cache){
            this.activityRepo = activityRepo;
        }


		[HttpGet("activities/{year}/{month}/{userId?}")]
        [Authorize]
		public IActionResult Activities(int year, int month, int userId = 0)
        {
			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata( "Kers, Service Log, Reporting, Activity", "Meetings/Activities Report", "Detailed Activities Report") )) {
				
				
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
				var activities = activityRepo.PerMonth(user,year, month, "asc");
				float count = (float)activities.Count();
				float ratio = count/5 ;
				var pages = (int) Math.Ceiling(ratio) + 1;
				var pdfCanvas = document.BeginPage(width, height);
				AddUkLogo(pdfCanvas, 16, 31);
				AddPageInfo(pdfCanvas, 1, pages, user, new DateTime(year, month, 1), "Monthly Activities Report");
				SummaryInfo(pdfCanvas, year, month, user, "Monthly Activities Report");
				ActivitiesSummary(pdfCanvas, activities);


				document.EndPage();

				details(document, activities, user, year, month);
				

				document.Close();
                
            	return File(stream.DetachAsData().AsStream(), "application/pdf", "ActivitiesReport.pdf");	
			}			
		}

		private void ActivitiesSummary(SKCanvas pdfCanvas, List<ActivityRevision> activities){
			float hoursTotal = 0;
			var audienceTotal = 0;
			List<ActivitySummaryPerMajorProgram> summary = new List<ActivitySummaryPerMajorProgram>();
			foreach(var activity in activities){
				hoursTotal += activity.Hours;
				audienceTotal += activity.Male + activity.Female;
				var mp = summary.Where(d => d.MajorProgram == activity.MajorProgram).FirstOrDefault();
				if(mp == null){
					var smr = new ActivitySummaryPerMajorProgram();
					smr.Audience = activity.Male + activity.Female;
					smr.Hours = activity.Hours;
					smr.MajorProgram = activity.MajorProgram;
					summary.Add(smr);
				}else{
					mp.Audience += activity.Male + activity.Female;
					mp.Hours += activity.Hours;
				}
			}
			pdfCanvas.DrawText("Hours Total: ", 43, 400, getPaint(14.0f, 1));
			pdfCanvas.DrawText(hoursTotal.ToString(), 140, 400, getPaint(24.0f, 3));
			pdfCanvas.DrawText("Audience Total: ", 333, 400, getPaint(14.0f, 1));
			pdfCanvas.DrawText(audienceTotal.ToString(), 446, 400, getPaint(24.0f, 3));

			var hdr = new List<string>();
			hdr.Add("Major Program");
			hdr.Add("Audience");
			hdr.Add("Hours");
			SummaryTableRow(pdfCanvas, hdr, 450);
			var y = 480;
			var ordrd = summary.OrderBy(s => s.MajorProgram.Name);
			foreach( var smr in ordrd){
				var line = new List<string>();
				line.Add(smr.MajorProgram.Name);
				line.Add(smr.Audience.ToString());
				line.Add(smr.Hours.ToString());
				SummaryTableRow(pdfCanvas, line, y);
				y += 25;
			}
		}



		private void SummaryTableRow( SKCanvas pdfCanvas, List<string> data, int y, int x = 43){
			var paint = getPaint(11f, 2);
			pdfCanvas.DrawText(data[0], x, y,  paint);
			paint = getPaint(12f, 0 , 0xFF000000, SKTextAlign.Right);
			for(var i = 1; i< data.Count; i++){
				pdfCanvas.DrawText(data[i], x + 330 + ( 80 * i), y,  paint);
			}
			SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
			pdfCanvas.DrawLine(x, y + 10, x + 516, y + 10, thinLinePaint);
		}

        
		private void details(SKDocument document, List<ActivityRevision> activities, KersUser user, int year, int month){
			float count = (float)activities.Count();
			float ratio = count/5 ;
			var pages = (int) Math.Ceiling(ratio) + 1;
			int pageIndex = 2;
			int position = 0;
			var pdfCanvas = document.BeginPage(width, height);

			AddPageInfo(pdfCanvas, 2, pages, user, new DateTime(year, month, 1), "Monthly Activities Report");

			foreach(var activity in activities){

				position++;
				if(position > 5){
					position = 1;
					pageIndex++;
					document.EndPage();
					pdfCanvas = document.BeginPage(width, height);
					AddPageInfo(pdfCanvas, pageIndex, pages, user, new DateTime(year, month, 1), "Monthly Activities Report");
				}
				addDetail(pdfCanvas, activity, position);

			}
			document.EndPage();
		}

		




		private void addDetail(SKCanvas pdfCanvas, ActivityRevision activity, int position){
			var offset = 40;
			var panelHeight = 140;

			var startingY = offset + (position - 1) * panelHeight;

			SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.DarkGray,
												StrokeWidth = 0.5f
											};
			pdfCanvas.DrawLine(43, startingY, 580, startingY, thinLinePaint);
			pdfCanvas.DrawLine(43, startingY + panelHeight - 10, 580, startingY + panelHeight - 10, thinLinePaint);

			pdfCanvas.DrawText(activity.ActivityDate.ToString("MMMM dd, yyyy"), 43, startingY + 25, getPaint(18.0f, 2));
			pdfCanvas.DrawText(activity.Title, 43, startingY + 45, getPaint(11.0f, 1));


			pdfCanvas.DrawText("Major Program:", 250, startingY + 18, getPaint(10.0f));
			pdfCanvas.DrawText(activity.MajorProgram.Name, 250, startingY + 30, getPaint(10.0f, 2));

			var attendance = activity.Male + activity.Female;
			pdfCanvas.DrawText("Attendance:", 43, startingY + 65, getPaint(10.0f));
			pdfCanvas.DrawText(attendance.ToString(), 103, startingY + 65, getPaint(10.0f, 2));

			pdfCanvas.DrawText("Hours:", 150, startingY + 65, getPaint(10.0f));
			pdfCanvas.DrawText(activity.Hours.ToString(), 185, startingY + 65, getPaint(10.0f, 2));
			

			var cleanned = Kers.HtmlHelpers.StripHtmlHelper.StripHtml(activity.Description).Trim();
			
			//var cleanned =Regex.Replace(activity.Description, "<.*?>", String.Empty).Trim();
			var lines = SplitLineToMultiline(cleanned, 110);

			var StartY = startingY + 85;
			var linesCount = 1;
			foreach( var ln in lines){
				pdfCanvas.DrawText(	ln, 43, StartY, getPaint(10.0f));
				StartY += 12;
				linesCount++;
				if(linesCount > 4){
					break;
				}
			}
			StartY = startingY + 45;
			foreach(var ops in activity.ActivityOptionSelections){
				pdfCanvas.DrawText(	ops.ActivityOption.Name.TrimEnd('?'), 250, StartY, getPaint(10.0f));
				StartY += 12;
			}
			
		}



    }


	class ActivitySummaryPerMajorProgram{
		public MajorProgram MajorProgram {get;set;}
		public float Hours {get;set;}
		public int Audience {get;set;}
	}
}
