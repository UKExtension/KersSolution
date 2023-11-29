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
		const int margin = 42;
		const int textLineHeight = 14;
		int[] trainingsTableLines = new int[]{ 0, 100, 143, 440, 500, 530};
		int trainingsTableLineHight = 14;
		int trainingsTableCellMargin = 2;
		string pageTitle = "Professional Career Ladder Promotion Application";
		IFiscalYearRepository _fiscalYearRepo;


        public PdfLadderController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
			KERSmainContext mainContext,
			IMemoryCache _cache,
			IFiscalYearRepository _fiscalYearRepo
        ):
			base(_context, userRepo, mainContext, _cache){
				this._fiscalYearRepo = _fiscalYearRepo;
				


			}






		[HttpGet("application/{id}")]

		public IActionResult Application(int id)
        {

			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, 
							this.metadata( "Kers, Career Ladder, Reporting", "Career Ladder Application", "Professional Career Ladder Promotion Application") )) {
				

					var application = _context.LadderApplication
											.Where(a => a.Id == id)
											.Include( a => a.KersUser).ThenInclude( u => u.RprtngProfile).ThenInclude( u => u.PlanningUnit).ThenInclude( n => n.ExtensionArea)
											.Include( a => a.KersUser).ThenInclude( u => u.PersonalProfile)
											.Include( a => a.KersUser).ThenInclude( u => u.Specialties ).ThenInclude( s => s.Specialty)
											.Include( a => a.LadderLevel)
											.Include( a => a.LadderEducationLevel)
											.Include( a => a.Images).ThenInclude( i => i.UploadImage)
											.Include( a => a.Stages).ThenInclude( s => s.LadderStage)
											.Include( a => a.Stages).ThenInclude( s => s.KersUser ).ThenInclude( u => u.RprtngProfile)
											.Include( a => a.Ratings)
											.FirstOrDefault();
					if( application != null){
						var pdfCanvas = document.BeginPage(height, width);
						var pageNum = 1;
						AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, pageTitle);
						var positionX = margin;
						var runningY = 31;
						//AddUkLogo(pdfCanvas, 16, runningY);
						AddUkCaLogo(pdfCanvas, positionX, runningY+15);
						pdfCanvas.DrawText("Professional Career Ladder", 223, 62, getPaint(20.0f, 1));
						pdfCanvas.DrawText("Promotion Application", 223, 82, getPaint(20.0f, 1));
						pdfCanvas.DrawText("For Outstanding Job Performance and Experiences Gained Through Program Development", 223, 95, getPaint(7.5f));
						

						runningY += 115;
						pdfCanvas.DrawText(application.KersUser.PersonalProfile.FirstName + " " +application.KersUser.PersonalProfile.LastName, positionX, runningY, getPaint(18f, 1));
						var textCounty = application.KersUser.RprtngProfile.PlanningUnit.Name;
						if( textCounty.Count() > 15 ){
							textCounty = textCounty.Substring(0, application.KersUser.RprtngProfile.PlanningUnit.Name.Count() - 11);
						}
						pdfCanvas.DrawText("County: " + textCounty, 350, runningY - 5, getPaint(10f));
						var textArea = "";
						if( application.KersUser.RprtngProfile.PlanningUnit.ExtensionArea != null){
							textArea = application.KersUser.RprtngProfile.PlanningUnit.ExtensionArea.Name;
						}
						pdfCanvas.DrawText("Extension Area: " + textArea, 350, runningY + 10, getPaint(10f));

						var rightColumnY = 191;

						pdfCanvas.DrawText("Performance Ratings: ", 350, rightColumnY, getPaint(10.0f, 1));
						foreach( var rtng in application.Ratings){
							rightColumnY += textLineHeight;
							pdfCanvas.DrawText(rtng.Year + ": " + rtng.Ratting, 350, rightColumnY, getPaint(10.0f));
						}


						runningY += 12;
						pdfCanvas.DrawText("UK/person ID: " + application.KersUser.RprtngProfile.PersonId, positionX, runningY, getPaint(8.5f));
						runningY += 32;
						pdfCanvas.DrawText("Start Date: " + application.StartDate.ToString("MM/dd/yyyy"), positionX, runningY, getPaint(10.0f));
						runningY += textLineHeight;
						pdfCanvas.DrawText("Number of years of Extension service: " + application.NumberOfYears, positionX, runningY, getPaint(10.0f));
						runningY += textLineHeight;
						pdfCanvas.DrawText("Program Area/Responsibility: ", positionX, runningY, getPaint(10.0f));
						foreach( var spclty in application.KersUser.Specialties){
							runningY += textLineHeight;
							pdfCanvas.DrawText(spclty.Specialty.Name, positionX, runningY, getPaint(10.0f, 3));
						}

						runningY += 28;
						pdfCanvas.DrawText("Track: " + (application.Track == 0 ? 'A' : 'B'), positionX, runningY, getPaint(10.0f));
						runningY += textLineHeight;
						pdfCanvas.DrawText("Promotion to Level: " + application.LadderLevel.Name, positionX, runningY, getPaint(10.0f));
						runningY += textLineHeight;
						pdfCanvas.DrawText("Date of last Career Ladder Promotion: " + application.LastPromotion.ToString("MM/dd/yyyy"), positionX, runningY, getPaint(10.0f));
						runningY += textLineHeight * 2;
						pdfCanvas.DrawText("Highest level of education: " + application.LadderEducationLevel.Name, positionX, runningY, getPaint(10.0f));
						runningY += textLineHeight;
						pdfCanvas.DrawText("Program of Study: " + application.ProgramOfStudy, positionX, runningY, getPaint(10.0f));
						runningY += textLineHeight;
						pdfCanvas.DrawText("Evidence of Further Professional or academic Training: " , positionX, runningY, getPaint(10.0f));



						var evedenceLines = SplitLineToMultiline( StripHTML(application.Evidence), 120);
						foreach( var evd in evedenceLines ){
							runningY += textLineHeight;
							pdfCanvas.DrawText(evd , positionX, runningY, getPaint(10.0f, 3));
						}
						document.EndPage();



						// !!!!!!!!!!!!!!!!!!!   Refine actual start of the year   !!!!!!!!!!!!!!!!!!

						var fiscalYear = this._fiscalYearRepo.byDate(application.Created, FiscalYearType.ServiceLog);

						var startOfTheYear = new DateTime( fiscalYear.End.Year, 1, 1);

						var startOfTheYearOfTheLastPromotion = new DateTime(application.LastPromotion.Year, 1, 1);


						var trainings = TrainingsByUser(application.KersUserId, startOfTheYearOfTheLastPromotion, startOfTheYear);
						var hours = HourssByUser(application.KersUserId, startOfTheYearOfTheLastPromotion, startOfTheYear);
						var coreHours = HourssByUser(application.KersUserId, startOfTheYearOfTheLastPromotion, startOfTheYear, true);
					

						pdfCanvas = document.BeginPage(height, width);
						pageNum++;

						runningY = margin + 5;
						AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, pageTitle);
						
						pdfCanvas.DrawText("InService Hours Earned from " 
													+ startOfTheYearOfTheLastPromotion.ToString("MM/dd/yyyy") 
													+ " till " + startOfTheYear.ToString("MM/dd/yyyy") 
													+ ": "  + hours.ToString() , positionX, runningY, getPaint(10.0f));

						runningY += textLineHeight;
						pdfCanvas.DrawText("Core Training Hours: " 
													+ coreHours.ToString() , positionX, runningY, getPaint(10.0f));


						runningY += 24;
						TrainingsTableHeader(pdfCanvas, runningY);
						runningY += trainingsTableLineHight;
						foreach( var trnng in trainings ){
							
							runningY += (trainingsTableLineHight * TrainingRow(trnng, pdfCanvas, runningY, application.KersUser));
							if( runningY > height ){
								runningY = margin + 5;
								document.EndPage();
								pdfCanvas = document.BeginPage(height, width);
								TrainingsTableHeader(pdfCanvas, runningY);
								runningY += trainingsTableLineHight;
								pageNum++;
								AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, pageTitle);
							}
						}

						if( application.Stages.Count() > 1 ){
							runningY = margin + 5;
							document.EndPage();
							pdfCanvas = document.BeginPage(height, width);
							pageNum++;
							AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, pageTitle);
							pdfCanvas.DrawText( "Reviews: " , margin, runningY, getPaint(12.0f, 1));
							runningY += textLineHeight * 2;
							var count = 1;
							foreach( var review in application.Stages){
								runningY += (TrainingReview( pdfCanvas, review, runningY) * textLineHeight);
								count++;
								// Skip the last review as it is by design empty
								if( count >= application.Stages.Count()) break;
								if( runningY > height - margin ){
									runningY = margin + 5;
									document.EndPage();
									pdfCanvas = document.BeginPage(height, width);
									pageNum++;
									AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, pageTitle);
								}
							}
						}

						foreach( var im in application.Images.OrderBy( i => i.UploadImageId)){
							runningY = margin + 5;
							document.EndPage();
							pdfCanvas = document.BeginPage(height, width);
							pageNum++;
							AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, pageTitle);
							runningY = margin + 5;
							var imageDescription = SplitLineToMultiline( im.Description, 120);
							foreach( var dscr in imageDescription ){
								pdfCanvas.DrawText(dscr, positionX, runningY, getPaint(10.0f));
								runningY += textLineHeight;
							}
							this.addBitmap(pdfCanvas,im.UploadImage.Name, margin, runningY, height - margin, width - margin);
						}
							
					

						

						document.EndPage();

					
					}else{
						Log(id,"LadderApplication", "Career Ladder Error", "int", "Error");
						return new StatusCodeResult(500);
					}

					document.Close();
					Log(application,"LadderApplication", "Career Ladder Pdf Created", "LadderApplication");
					return File(stream.DetachAsData().AsStream(), "application/pdf", "CareerLadderApplication.pdf");	
			}

		}

		private int TrainingReview( SKCanvas pdfCanvas, LadderApplicationStage review, int positionY ){
			var numLines = 1;
			var reviewLine = review.LadderStage.Name + " on " + review.Reviewed.ToString() + " by ";
			if( review.KersUser != null ){
				reviewLine += review.KersUser.RprtngProfile.Name;
			}
			pdfCanvas.DrawText( reviewLine , 
							margin, 
							positionY,
							getPaint(10.0f, 1));
			if( review.Note != null & review.Note != ""){
				var note = SplitLineToMultiline( review.Note, 120);
				foreach( var dscr in note ){
					pdfCanvas.DrawText(dscr, margin, positionY + numLines * textLineHeight, getPaint(10.0f));
					numLines++;
				}
			}
			return numLines;
		}


		private void TrainingsTableHeader( SKCanvas pdfCanvas, int positionY){
			DrawTrainingTableLines(pdfCanvas, positionY);
			TrainingsTableHorizontalLine( pdfCanvas, positionY);
			pdfCanvas.DrawText("Training Date(s)" , 
							margin + trainingsTableLines[0] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin,
							getPaint(10.0f));
			pdfCanvas.DrawText("Hours" ,
							margin + trainingsTableLines[1] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(10.0f));
			pdfCanvas.DrawText("Title" , 
							margin + trainingsTableLines[2] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(10.0f));
			pdfCanvas.DrawText("Attendance" , 
							margin + trainingsTableLines[3] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(10.0f));
			pdfCanvas.DrawText("Core" , 
							margin + trainingsTableLines[4] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(10.0f));
			TrainingsTableHorizontalLine( pdfCanvas, positionY + trainingsTableLineHight);
		}

		private int TrainingRow( Training Training, SKCanvas pdfCanvas, int positionY, KersUser user){
			int numRows = 0;
			var datesString = Training.Start.ToString("MM/dd/yyyy");
			if( Training.End != null){
				datesString += " - " + Training.Start.ToString("MM/dd/yyyy");
			}
			pdfCanvas.DrawText(datesString , margin + trainingsTableLines[0] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(8.5f));
			if(Training.iHour != null){
				pdfCanvas.DrawText(Training.iHour.iHoursTxt, margin + trainingsTableLines[1] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(9.0f));
			}
			var enrollment = Training.Enrollment.Where( e => e.Attendie == user).FirstOrDefault();
			var attended = "NO";
			if( enrollment != null & enrollment.attended??false) attended = "YES";
			pdfCanvas.DrawText(attended , margin + trainingsTableLines[3] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(8.5f));
			var core = "NO";
			if( Training.IsCore == true ) core = "YES";
			pdfCanvas.DrawText(core , margin + trainingsTableLines[4] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(8.5f));
			var SubjectLines = SplitLineToMultiline(Training.Subject, 57);
			foreach( var ln in SubjectLines){
				numRows++;
				DrawTrainingTableLines(pdfCanvas, positionY);
				pdfCanvas.DrawText(ln , margin + trainingsTableLines[2] + trainingsTableCellMargin, 
							positionY + trainingsTableLineHight - trainingsTableCellMargin, 
							getPaint(10.0f, 3));
				positionY += trainingsTableLineHight;
			}
			TrainingsTableHorizontalLine( pdfCanvas, positionY);
			return numRows;
		}
		private void TrainingsTableHorizontalLine( SKCanvas pdfCanvas, int positionY){
			pdfCanvas.DrawLine(margin + trainingsTableLines[0], positionY, margin + trainingsTableLines[5], positionY, thinLinePaint);
		}

		private void DrawTrainingTableLines(SKCanvas pdfCanvas, int positionY){
            foreach( var pos in this.trainingsTableLines){
                pdfCanvas.DrawLine(margin + pos, positionY, margin + pos, positionY + trainingsTableLineHight, thinLinePaint);
            }
		}


		private List<Training> TrainingsByUser( int id, DateTime Start, DateTime End){
			
			
			var trainings = from training in _context.Training
                from enfolment in training.Enrollment
                where enfolment.AttendieId == id
                select training;
            
            trainings = trainings.Where( t => t.Start > Start && t.Start < End);
            
            trainings = trainings.Include( t => t.Enrollment).Include(t => t.iHour)
            			.Include( t => t.SurveyResults);
            var tnngs = trainings.OrderBy(t => t.Start).ToList();
			return tnngs;

		}

		private int HourssByUser( int id, DateTime start, DateTime end, bool onlyCore = false){
            IQueryable<Training> trainings = from training in _context.Training
            from enfolment in training.Enrollment
            where enfolment.AttendieId == id && enfolment.attended == true
            select training;
        
            trainings = trainings.Where( t => t.Start > start && t.Start < end);

			if( onlyCore ) trainings = trainings.Where( t => t.IsCore == true );
            
            trainings = trainings.Include(t => t.iHour);
            var hours = trainings.Sum( t => t.iHour == null ? 0 : t.iHour.iHourValue );
			return hours;
		}






	}

}
