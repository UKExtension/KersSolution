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
		int[] trainingsTableLines = new int[]{ 0, 100, 143, 454, 517};
		int trainingsTableLineHight = 14;
		int trainingsTableCellMargin = 2;


        public PdfLadderController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
			KERSmainContext mainContext,
			IMemoryCache _cache
        ):
			base(_context, userRepo, mainContext, _cache){
				


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
											.FirstOrDefault();
					if( application != null){
						var pdfCanvas = document.BeginPage(height, width);
						var pageNum = 1;
						AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, "Professional Career Ladder Promotion Application");
						var positionX = margin;
						var runningY = 31;
						AddUkLogo(pdfCanvas, 16, runningY);
						pdfCanvas.DrawText("Professional Career Ladder", 223, 62, getPaint(20.0f, 1));
						pdfCanvas.DrawText("Promotion Application", 223, 82, getPaint(20.0f, 1));
						pdfCanvas.DrawText("For Outstanding Job Performance and Experiences Gained Through Program Development", 223, 95, getPaint(7.5f));
						

						runningY += 115;
						pdfCanvas.DrawText(application.KersUser.PersonalProfile.FirstName + " " +application.KersUser.PersonalProfile.LastName, positionX, runningY, getPaint(18f, 1));
						var textCounty = application.KersUser.RprtngProfile.PlanningUnit.Name;
						if( textCounty.Count() > 15 ){
							textCounty = textCounty.Substring(0, application.KersUser.RprtngProfile.PlanningUnit.Name.Count() - 11);
						}
						pdfCanvas.DrawText("County: " + textCounty, 300, runningY - 5, getPaint(10f));
						var textArea = "";
						if( application.KersUser.RprtngProfile.PlanningUnit.ExtensionArea != null){
							textArea = application.KersUser.RprtngProfile.PlanningUnit.ExtensionArea.Name;
						}
						pdfCanvas.DrawText("Extension Area: " + textArea, 300, runningY + 10, getPaint(10f));
						runningY += 12;
						pdfCanvas.DrawText("UK/person ID: " + application.KersUser.RprtngProfile.PersonId, positionX, runningY, getPaint(8.5f));
						runningY += 32;
						pdfCanvas.DrawText("Start Date: " + application.StartDate.ToString("MM/dd/yyyy"), positionX, runningY, getPaint(10.0f));
						runningY += 14;
						pdfCanvas.DrawText("Number of years of Extension service: " + application.NumberOfYears, positionX, runningY, getPaint(10.0f));
						runningY += 14;
						pdfCanvas.DrawText("Program Area/Responsibility: ", positionX, runningY, getPaint(10.0f));
						foreach( var spclty in application.KersUser.Specialties){
							runningY += 14;
							pdfCanvas.DrawText(spclty.Specialty.Name, positionX, runningY, getPaint(10.0f, 3));
						}

						runningY += 28;
						pdfCanvas.DrawText("Promotion to Level: " + application.LadderLevel.Name, positionX, runningY, getPaint(10.0f));
						runningY += 14;
						pdfCanvas.DrawText("Date of last Career Ladder Promotion: " + application.LastPromotion.ToString("MM/dd/yyyy"), positionX, runningY, getPaint(10.0f));
						runningY += 28;
						pdfCanvas.DrawText("Highest level of education: " + application.LadderEducationLevel.Name, positionX, runningY, getPaint(10.0f));
						runningY += 14;
						pdfCanvas.DrawText("Program of Study: " + application.ProgramOfStudy, positionX, runningY, getPaint(10.0f));
						runningY += 14;
						pdfCanvas.DrawText("Evidence of Further Professioal or academic Training: " , positionX, runningY, getPaint(10.0f));

						var evedenceLines = SplitLineToMultiline( StripHTML(application.Evidence), 120);
						foreach( var evd in evedenceLines ){
							runningY += 14;
							pdfCanvas.DrawText(evd , positionX, runningY, getPaint(10.0f, 3));
						}
						document.EndPage();



						// !!!!!!!!!!!!!!!!!!!   Refine actual start of the year   !!!!!!!!!!!!!!!!!!
						var startOfTheYear = new DateTime( application.Created.Year, 1, 1);

						var trainings = TrainingsByUser(application.KersUserId, application.LastPromotion, startOfTheYear);
						var hours = HourssByUser(application.KersUserId, application.LastPromotion, startOfTheYear);

					

						pdfCanvas = document.BeginPage(height, width);
						pageNum++;

						runningY = margin + 5;
						AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, "Professional Career Ladder Promotion Application");
						
						pdfCanvas.DrawText("InService Hours Earned from " 
													+ application.LastPromotion.ToString("MM/dd/yyyy") 
													+ " till " + startOfTheYear.ToString("MM/dd/yyyy") 
													+ ": "  + hours.ToString() , positionX, runningY, getPaint(10.0f));

						runningY += 24;
						TrainingsTableHeader(pdfCanvas, runningY);
						runningY += trainingsTableLineHight;
						foreach( var trnng in trainings ){
							
							runningY += trainingsTableLineHight * TrainingRow(trnng, pdfCanvas, runningY, application.KersUser);
							if( runningY > height - 2 * margin ){
								runningY = margin + 5;
								document.EndPage();
								pdfCanvas = document.BeginPage(height, width);
								TrainingsTableHeader(pdfCanvas, runningY);
								runningY += trainingsTableLineHight;
								pageNum++;
								AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, "Professional Career Ladder Promotion Application");
							}
						}

						foreach( var im in application.Images){
							runningY = margin + 5;
							document.EndPage();
							pdfCanvas = document.BeginPage(height, width);
							pageNum++;
							AddPageInfo(pdfCanvas, pageNum, 0, application.KersUser, DateTime.Now, "Professional Career Ladder Promotion Application");
							runningY = margin + 5;
							var imageDescription = SplitLineToMultiline( im.Description, 120);
							foreach( var dscr in imageDescription ){
								pdfCanvas.DrawText(dscr, positionX, runningY, getPaint(10.0f));
								runningY += 14;
							}
							

							this.addBitmap(pdfCanvas,im.UploadImage.Name, margin, runningY, height - margin, width - margin);
							/* 
							Stream input = new MemoryStream(im.UploadImage.UploadFile.Content);

            				//const int quality = 75;
							
							using (var inputStream = new SKManagedStream(input))
								{
								using (var original = SKBitmap.Decode(inputStream))
									{

							
									int rectHeight = height;
									int rectWidth = width; 
									//calculate aspect ratio
									float aspect = original.Width / (float)original.Height;
									int newWidth, newHeight;

									//calculate new dimensions based on aspect ratio
									newWidth = (int)(rectWidth * aspect);
									newHeight = (int)(newWidth / aspect);

									//if one of the two dimensions exceed the box dimensions
									if (newWidth > rectWidth || newHeight > rectHeight)
									{
										//depending on which of the two exceeds the box dimensions set it as the box dimension and calculate the other one based on the aspect ratio
										if (newWidth > newHeight)
										{
											newWidth = rectWidth;
											newHeight = (int)(newWidth / aspect);
										}
										else
										{
											newHeight = rectHeight;
											newWidth = (int)(newHeight * aspect);
										}
									}


									using (var resized = original
									.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium))
										{
											using (var image = SKImage.FromBitmap(resized))
											{
												var paint = new SKPaint();
												pdfCanvas.DrawImage(image, new SKPoint( margin, margin ), paint);
											}
										}
								}
							}
 */

						}
							
					

						

						document.EndPage();

					
					}else{
						//Log(id,"LadderApplication", "Career Ladder Error", "int", "Error");
						return new StatusCodeResult(500);
					}







					
					

					document.Close();
					//Log(application,"LadderApplication", "Career Ladder Pdf Created", "LadderApplication");
					return File(stream.DetachAsData().AsStream(), "application/pdf", "CareerLadderApplication.pdf");	
			}

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

			var SubjectLines = SplitLineToMultiline(Training.Subject, 60);
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
			pdfCanvas.DrawLine(margin + trainingsTableLines[0], positionY, margin + trainingsTableLines[4], positionY, thinLinePaint);
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

		private int HourssByUser( int id, DateTime start, DateTime end){
            
            var trainings = from training in _context.Training
            from enfolment in training.Enrollment
            where enfolment.AttendieId == id && enfolment.attended == true
            select training;
        

            trainings = trainings.Where( t => t.Start > start && t.Start < end);
            

            trainings = trainings.Include(t => t.iHour);
            var hours = trainings.Sum( t => t.iHour == null ? 0 : t.iHour.iHourValue );
			return hours;
		}






	}

}
