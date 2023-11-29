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
using Microsoft.VisualBasic;

namespace Kers.Controllers
{
	[Route("api/[controller]")]
    public class PdfBaseController : Controller
    {

        public KERScoreContext _context;
        public KERSmainContext mainContext;
        public IKersUserRepository userRepo;
        public IMemoryCache _cache;

		const int width = 612;
		const int height = 792;

		

		string[] typefaceNames = {	"HelveticaNeue", "HelveticaNeue-Bold", 
									"HelveticaNeue-CondensedBold", "HelveticaNeue-Light",
									"BRUSHSCI"
								};

		SKTypeface[] typefaces;

		protected SKPaint thinLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 0.5f
											};
		protected SKPaint thickLinePaint = new SKPaint
											{
												Style = SKPaintStyle.Stroke,
												Color = SKColors.Black,
												StrokeWidth = 1.5f
											};



        public PdfBaseController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
			KERSmainContext mainContext,
			IMemoryCache _cache
        ){
            this._context = _context;
			this.userRepo = userRepo;
			this.mainContext = mainContext;
			this._cache = _cache;
			
			var typefacesCacheKey = "HelveticaNeue typefaces";


			if (!_cache.TryGetValue(typefacesCacheKey, out typefaces))
            {
				var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromDays(190));
                
				typefaces = new SKTypeface[typefaceNames.Length];

				for(var i = 0; i < typefaceNames.Length; i++){

					var cacheKey = "HNtypeface" + typefaceNames[i];
					SKTypeface typeface = null;
					if (!_cache.TryGetValue(cacheKey, out typeface))
            		{
/* 
						var dbRow = _context.UploadFile.Where(f => f.Name == typefaceNames[i] + ".ttf").FirstOrDefault();
						if(dbRow != null){
							var strm = new MemoryStream(dbRow.Content);
							typeface = SKTypeface.FromStream( strm );
						} */
						typeface = SKTypeface.FromFile("Assets/Fonts/"+typefaceNames[i] + ".ttf");

						// Save data in cache.
						_cache.Set(cacheKey, typeface, cacheEntryOptions);
					}
					typefaces[i] = typeface;
				}              
                // Save data in cache.
                _cache.Set(typefacesCacheKey, typefaces, cacheEntryOptions);
            }
        }

		

		// Convert Stream to Byte array
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }


		public void AddPageInfo(SKCanvas pdfCanvas, int page, int totalPages, KersUser user, DateTime dt, string Ttl = "Monthly Expenses Report", string PageOrientation = "portrait"){
			
			
			var textTop = "Page " + page.ToString() + (totalPages == 0 ? "" : " of " + totalPages.ToString());
			var paint = getPaint(9.0f, 3, 0xFF000000, SKTextAlign.Right);
			var text = user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName + ", " + dt.ToString("MMMM yyyy") + ", " + Ttl;
			paint = getPaint(9.0f, 1, 0xFF000000, SKTextAlign.Right);
			if( PageOrientation == "portrait"){
				pdfCanvas.DrawText(textTop, 590, 20, paint);
				pdfCanvas.DrawText(text, 530, 20, paint);
				pdfCanvas.DrawText("Report Generated: ", 43, 770, getPaint(9.0f, 1));
				pdfCanvas.DrawText( DateTime.Now.ToString(), 125, 770, getPaint(9.0f));
			}else{
				pdfCanvas.DrawText(textTop, 750, 20, paint);
				pdfCanvas.DrawText(text, 690, 20, paint);
				pdfCanvas.DrawText("Report Generated: ", 43, 600, getPaint(9.0f, 1));
				pdfCanvas.DrawText( DateTime.Now.ToString(), 125, 600, getPaint(9.0f));
			}
			
		}

        public void SummaryInfo(SKCanvas pdfCanvas, int year, int month, KersUser user, string Ttl = "Monthly Expenses Report"){
            var date = new DateTime(year, month, 1);
            var text = date.ToString("MMMM yyyy");
            pdfCanvas.DrawText(text, 257, 80, getPaint(20.0f, 1, 0xFF000000));
            pdfCanvas.DrawText(Ttl, 257, 102, getPaint(20.0f, 3, 0xFF000000));
            text = user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName;
            pdfCanvas.DrawText(text, 43, 204, getPaint(18.0f, 1));
            text = user.ExtensionPosition.Title + ", " + user.RprtngProfile.PlanningUnit.Name;
            pdfCanvas.DrawText(text, 43, 222, getPaint(12.0f));
            pdfCanvas.DrawText("Summary", 43, 300, getPaint(20.0f));
        }

		public void SummaryLandscapeInfo(SKCanvas pdfCanvas, int year, int month, KersUser user, string Ttl = "Monthly Expenses Report", bool isOvernight = false, bool isPersonal = true){
			var date = new DateTime(year, month, 1);
			var text = date.ToString("MMMM yyyy");
			pdfCanvas.DrawText(text, 250, 75, getPaint(18.0f, 1, 0xFF000000));

			pdfCanvas.DrawText(isPersonal ? "Personal Vehicle" : "County Vehicle", 356, 86, getPaint(10.0f, 0, 0xFF000000, SKTextAlign.Right));

			pdfCanvas.DrawText(Ttl, 250, 102, getPaint(20.0f, 3, 0xFF000000));
			if( isOvernight ){
				text = "Overnight Trips";
			}else{
				text = "Day Trips";
			}
			pdfCanvas.DrawText(text, 250, 112, getPaint(9.0f));
			text = user.PersonalProfile.FirstName + " " + user.PersonalProfile.LastName;
			pdfCanvas.DrawText(text, 400, 75, getPaint(17.0f, 1));
			text = user.ExtensionPosition.Title;
			pdfCanvas.DrawText(text, 400, 90, getPaint(9.5f));
			text = user.RprtngProfile.PlanningUnit.Name;
			pdfCanvas.DrawText(text, 400, 102, getPaint(10.0f));
			if(user.PersonalProfile.OfficeAddress != null && user.PersonalProfile.OfficeAddress != "" ){
				text = user.RprtngProfile.PlanningUnit.Name;
				pdfCanvas.DrawText(text, 554, 75, getPaint(10.0f, 1));
				var addressLines = user.PersonalProfile.OfficeAddress.Split("\n");
				int initialY = 90;
				foreach( var line in addressLines){
					pdfCanvas.DrawText(line, 554, initialY, getPaint(10.0f));
					initialY += 12;
				}
				
			}else if(user.RprtngProfile.PlanningUnit.Address != null ){
				text = user.RprtngProfile.PlanningUnit.FullName;
				pdfCanvas.DrawText(text, 554, 75, getPaint(10.0f, 1));
				text = user.RprtngProfile.PlanningUnit.Address;
				pdfCanvas.DrawText(text, 554, 90, getPaint(10.0f));
				text = user.RprtngProfile.PlanningUnit.City + ", KY " + user.RprtngProfile.PlanningUnit.Zip;
				pdfCanvas.DrawText(text, 554, 102, getPaint(10.0f));
			}
			//pdfCanvas.DrawText("Summary", 43, 300, getPaint(20.0f));
		}


		public SKPaint getPaint(	float textSize = 64.0f, 
									int fontIndex = 0,
									UInt32 color = 0xFF000000, 
									SKTextAlign align = SKTextAlign.Left 
								){
			var paint = new SKPaint();
			paint.TextSize = textSize;
			paint.IsAntialias = true;
			paint.TextAlign = align;
			paint.Color = (SKColor)color; 


			paint.Typeface = this.typefaces[fontIndex];

			

			return paint;
		}


        


		public void AddUkLogo(SKCanvas canvas, int x = 50, int y = 50){
				
				var dbFile = _context.UploadFile.Where(f => f.Name == "UkCaLogo.svg").FirstOrDefault();
				
				var svg = new SkiaSharp.Extended.Svg.SKSvg();
				
				svg.Load(new MemoryStream(dbFile.Content));
				var matrix = SKMatrix.CreateTranslation(x, y);
				canvas.DrawPicture(svg.Picture, ref matrix);
		}

		public void AddUkCaLogo(SKCanvas canvas, int x = 50, int y = 50){
				
				byte[] bytes = Encoding.UTF8.GetBytes(this.CollegeLogoSvg);
				var svg = new SkiaSharp.Extended.Svg.SKSvg();
				svg.Load(new MemoryStream(bytes));
				var matrix = SKMatrix.Concat( SKMatrix.CreateTranslation(x, y), SKMatrix.CreateScale(0.22f, 0.22f));
				canvas.DrawPicture(svg.Picture, ref matrix);
		}


		public void addBitmap(SKCanvas pdfCanvas, string fileName = "RegSign.png", int tl = 315, int tr = 17, int bl = 431, int br = 75){
			var dbFile = _context.UploadFile.Where(f => f.Name == fileName).FirstOrDefault();
			if( dbFile != null){
				var PngStream = new MemoryStream(dbFile.Content);
				var webBitmap = SKBitmap.Decode(PngStream);
				SKRect rect = new SKRect(tl, tr, bl, br);
				pdfCanvas.DrawBitmap(webBitmap, rect);
			}
		}
		public SKDocumentPdfMetadata metadata(string Keywrds = "Kers, Expense Reporting, Expense", string Ttl = "Summary Expense Report", string Sbj = "Summary Expense Report" ){
			return new SKDocumentPdfMetadata
			{
				Author = "Ivelin Denev",
				Creation = DateTime.Now,
				Creator = "Kentucky Extension Reporting System",
				Keywords = Keywrds,
				Modified = DateTime.Now,
				Producer = "SkiaSharp",
				Subject = Sbj,
				Title = Ttl,
				RasterDpi = 300,
				EncodingQuality = 95
			};
		}


		public void saveFile(string path, string name, string type){
			
			var dbFile = _context.UploadFile.Where(f => f.Name == name).FirstOrDefault();

			if(dbFile == null){
				var stream = new FileStream(path, FileMode.Open);

				var upFile = new UploadFile();
				upFile.Created = DateTime.Now;
				upFile.Name = name;
				upFile.Updated = DateTime.Now;
				upFile.Type = type;
				upFile.Size = Int32.Parse(stream.Length.ToString());
				upFile.By = CurrentUser();
				byte[] content = ReadFully(stream);

				upFile.Content = content;
				_context.Add(upFile);
				_context.SaveChanges();
			}
		}

		protected string StripHTML(string htmlString){

            string pattern = @"<[^>]*(>|$)|&nbsp;|&#39;|&raquo;|&laquo;|&quot;";

            return Regex.Replace(htmlString, pattern, string.Empty);
        }


		private static String ReverseString(String str)
		{
			int word_length = 0;
			String result = "";
			if(str == null) return result;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == ' ')
				{
					result = " " + result;
					word_length = 0;
				}
				else
				{
					result = result.Insert(word_length, str[i].ToString());
					word_length++;
				}
			}
			return result;
		}

		public static List<string> SplitLineToMultiline(string input, int rowLength)
		{
			var result = new List<string>();

			StringBuilder line = new StringBuilder();

			Stack<string> stack = new Stack<string>(ReverseString(input).Split(' '));

			while (stack.Count > 0)
			{
				var word = stack.Pop();
				if (word.Length > rowLength)
				{
					string head = word.Substring(0, rowLength);
					string tail = word.Substring(rowLength);

					word = head;
					stack.Push(tail);
				}

				if (line.Length + word.Length > rowLength)
				{
					result.Add(line.ToString());
					line.Clear();
				}

				line.Append(word + " ");
			}

			result.Add(line.ToString());
			return result;
		}




		public void Log(   object obj, 
                            string objectType = "List<ExpenseSummary>",
                            string description = "Summary Expense Report Printed", 
                            string type = "Expense Reports",
                            string level = "Information"
                        ){
                             
            var log = new Log();
            log.Level = level;
            log.Time = DateTime.Now;
            log.User = this.CurrentUser();
            log.ObjectType = objectType;
            log.Object = JsonConvert.SerializeObject(obj,  
                                            new JsonSerializerSettings() {
                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                });
            log.Agent = Request.Headers["User-Agent"].ToString();
            log.Ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            log.Description = description;
            log.Type = type;
            this._context.Log.Add(log);
            this._context.SaveChanges();

        }

        public KersUser userByProfileId(string profileId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.personID == profileId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = userRepo.findByProfileID(profile.Id);
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }


        public KersUser userByLinkBlueId(string linkBlueId){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = this._context.KersUser.
                            Where( u => u.classicReportingProfileId == profile.Id).
                            Include(u => u.RprtngProfile).ThenInclude(r => r.PlanningUnit).
							Include( u => u.PersonalProfile).
                            Include(u => u.ExtensionPosition).
                            FirstOrDefault();
                if(user == null){
                    user = userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }

        public PlanningUnit CurrentPlanningUnit(){
            var u = this.CurrentUserId();
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == u).
                            FirstOrDefault();
            return  this._context.PlanningUnit.
                    Where( p=>p.Code == profile.planningUnitID).
                    FirstOrDefault();
        }

        public zEmpRptProfile profileByUser(KersUser user){
            var profile = mainContext.zEmpRptProfiles.
                            Where(p=> p.Id == user.classicReportingProfileId).
                            FirstOrDefault();
            
            return profile;
        }

        public KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }

        public zEmpRptProfile CurrentProfile(){
            var u = this.CurrentUserId();
            return mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == u).
                            FirstOrDefault();
        }

        public string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }


        
        public IActionResult Error()
        {
            return View();
        }

		public string CollegeLogoSvg = @"<?xml version='1.0' encoding='utf-8'?>
<!-- Generator: Adobe Illustrator 28.0.0, SVG Export Plug-In . SVG Version: 6.00 Build 0)  -->
<svg version='1.1' id='Layer_2' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px'
	 viewBox='0 0 762 227' style='enable-background:new 0 0 762 227;' xml:space='preserve'>
<style type='text/css'>
	.st0{fill:#231F20;}
</style>
<g>
	<g>
		<path class='st0' d='M226.89,102.04c0-9.1,5.24-16.05,14.22-16.05c5.4,0,9.51,2.56,11.86,6.7l-3.37,1.99
			c-1.67-3.13-4.43-5.16-8.45-5.16c-6.83,0-10.08,5.81-10.08,12.47c0,6.58,3.17,12.55,10.28,12.55c4.06,0,6.86-2.07,8.49-5.08
			l3.29,1.87c-2.32,4.1-6.42,6.74-11.9,6.74C232.13,118.08,226.89,111.14,226.89,102.04z'/>
		<path class='st0' d='M257.36,106.75c0-6.34,3.9-11.37,10.72-11.37c6.86,0,10.68,5.12,10.68,11.37c0,6.21-3.82,11.33-10.68,11.33
			C261.26,118.08,257.36,113.05,257.36,106.75z M261.26,106.75c0,4.31,2.28,8.04,6.78,8.04s6.82-3.74,6.82-8.04
			c0-4.35-2.32-8.08-6.82-8.08S261.26,102.4,261.26,106.75z'/>
		<path class='st0' d='M285.4,117.64v-31.2h3.86v31.2H285.4z'/>
		<path class='st0' d='M297.3,117.64v-31.2h3.86v31.2H297.3z'/>
		<path class='st0' d='M326.79,114.55c-1.95,2.27-4.75,3.53-8.25,3.53c-6.87,0-10.76-4.51-10.76-11.37c0-6.58,3.9-11.33,10.2-11.33
			c6.25,0,9.63,4.67,9.63,10.64c0,0.65-0.04,1.66-0.12,2.23h-15.76c0.41,4.14,2.8,6.54,6.86,6.54c2.4,0,4.23-0.89,5.53-2.64
			L326.79,114.55z M323.78,105.04c-0.24-3.57-2.03-6.42-5.89-6.42c-3.82,0-5.85,2.76-6.22,6.42H323.78z'/>
		<path class='st0' d='M353.16,115.24c0,6.95-4.47,10.28-11.05,10.28c-3.45,0-6.22-0.98-8.17-2.27l1.75-3.05
			c1.58,1.06,3.49,2.03,6.38,2.03c4.06,0,7.27-1.83,7.27-6.98v-1.87h-0.08c-1.66,2.6-4.31,3.9-7.27,3.9c-5.77,0-9.3-4.87-9.3-10.8
			c0-5.97,3.57-11.09,9.59-11.09c3.17,0,5.53,1.42,7.07,3.57h0.08l0.12-3.13h3.62V115.24z M349.42,106.26
			c0-4.18-2.32-7.51-6.42-7.51c-4.26,0-6.42,3.49-6.42,7.59c0,3.94,1.99,7.56,6.38,7.56C347.15,113.9,349.42,110.49,349.42,106.26z'
			/>
		<path class='st0' d='M378.79,114.55c-1.95,2.27-4.75,3.53-8.25,3.53c-6.87,0-10.76-4.51-10.76-11.37c0-6.58,3.9-11.33,10.2-11.33
			c6.25,0,9.63,4.67,9.63,10.64c0,0.65-0.04,1.66-0.12,2.23h-15.76c0.41,4.14,2.8,6.54,6.86,6.54c2.4,0,4.23-0.89,5.53-2.64
			L378.79,114.55z M375.79,105.04c-0.24-3.57-2.03-6.42-5.89-6.42c-3.82,0-5.85,2.76-6.22,6.42H375.79z'/>
		<path class='st0' d='M396.23,106.75c0-6.34,3.9-11.37,10.72-11.37c6.86,0,10.68,5.12,10.68,11.37c0,6.21-3.82,11.33-10.68,11.33
			C400.13,118.08,396.23,113.05,396.23,106.75z M400.13,106.75c0,4.31,2.28,8.04,6.78,8.04s6.82-3.74,6.82-8.04
			c0-4.35-2.32-8.08-6.82-8.08S400.13,102.4,400.13,106.75z'/>
		<path class='st0' d='M425.88,99.03h-4.35v-3.21h4.35v-3.66c0-4.02,2.4-6.17,6.54-6.17c0.98,0,1.79,0.12,2.56,0.33v3.21
			c-0.41-0.12-1.1-0.2-1.79-0.2c-2.52,0-3.45,1.02-3.45,3.33v3.17h5.24v3.21h-5.24v18.6h-3.86V99.03z'/>
		<path class='st0' d='M470.69,108.9H456.8l-3.33,8.73h-4.31l12.27-31.2h4.83l12.27,31.2h-4.47L470.69,108.9z M469.31,105.37
			l-5.56-14.5h-0.08l-5.53,14.5H469.31z'/>
		<path class='st0' d='M502.34,115.24c0,6.95-4.47,10.28-11.05,10.28c-3.45,0-6.22-0.98-8.17-2.27l1.75-3.05
			c1.58,1.06,3.49,2.03,6.38,2.03c4.06,0,7.27-1.83,7.27-6.98v-1.87h-0.08c-1.66,2.6-4.31,3.9-7.27,3.9c-5.77,0-9.3-4.87-9.3-10.8
			c0-5.97,3.57-11.09,9.59-11.09c3.17,0,5.53,1.42,7.07,3.57h0.08l0.12-3.13h3.62V115.24z M498.6,106.26c0-4.18-2.32-7.51-6.42-7.51
			c-4.26,0-6.42,3.49-6.42,7.59c0,3.94,1.99,7.56,6.38,7.56C496.33,113.9,498.6,110.49,498.6,106.26z'/>
		<path class='st0' d='M510.39,95.82h3.74l0.08,3.29h0.08c1.46-2.72,3.49-3.74,5.61-3.74c0.77,0,1.34,0.12,1.83,0.28v3.78
			c-0.37-0.12-1.14-0.28-2.03-0.28c-3.62,0-5.44,2.56-5.44,5.24v13.24h-3.86V95.82z'/>
		<path class='st0' d='M527.29,86.44h4.18v4.18h-4.18V86.44z M527.45,117.64V95.82h3.86v21.81H527.45z'/>
		<path class='st0' d='M537.97,106.83c0-6.46,3.86-11.46,10.56-11.46c4.14,0,7.07,1.99,8.73,5.12l-3.21,1.83
			c-1.1-2.11-2.84-3.65-5.61-3.65c-4.26,0-6.58,3.57-6.58,8.08c0,4.39,2.23,8.04,6.82,8.04c2.88,0,4.63-1.46,5.69-3.54l3.05,1.75
			c-1.54,3.05-4.63,5.08-8.85,5.08C541.87,118.08,537.97,113.25,537.97,106.83z'/>
		<path class='st0' d='M581.36,117.64h-3.78l-0.04-3.21h-0.08c-1.42,2.15-3.9,3.66-7.07,3.66c-4.71,0-7.68-3.05-7.68-7.64V95.82
			h3.86v13.85c0,2.84,1.58,5.08,4.96,5.08c3.66,0,5.97-2.52,5.97-5.93v-13h3.86V117.64z'/>
		<path class='st0' d='M589.41,117.64v-31.2h3.86v31.2H589.41z'/>
		<path class='st0' d='M598.51,95.82h4.35v-6.91h3.86v6.91h5.24v3.21h-5.24v11.94c0,2.6,0.69,3.78,3.53,3.78
			c0.73,0,1.26-0.08,1.71-0.16v3.13c-0.85,0.2-1.83,0.37-3.05,0.37c-4.15,0-6.05-2.27-6.05-6.13V99.03h-4.35V95.82z'/>
		<path class='st0' d='M636.41,117.64h-3.78l-0.04-3.21h-0.08c-1.42,2.15-3.9,3.66-7.07,3.66c-4.71,0-7.68-3.05-7.68-7.64V95.82
			h3.86v13.85c0,2.84,1.58,5.08,4.96,5.08c3.66,0,5.97-2.52,5.97-5.93v-13h3.86V117.64z'/>
		<path class='st0' d='M644.45,95.82h3.74l0.08,3.29h0.08c1.46-2.72,3.49-3.74,5.61-3.74c0.77,0,1.34,0.12,1.83,0.28v3.78
			c-0.37-0.12-1.14-0.28-2.03-0.28c-3.62,0-5.44,2.56-5.44,5.24v13.24h-3.86V95.82z'/>
		<path class='st0' d='M678.78,114.55c-1.95,2.27-4.75,3.53-8.25,3.53c-6.87,0-10.76-4.51-10.76-11.37c0-6.58,3.9-11.33,10.2-11.33
			c6.25,0,9.63,4.67,9.63,10.64c0,0.65-0.04,1.66-0.12,2.23h-15.76c0.41,4.14,2.8,6.54,6.86,6.54c2.4,0,4.23-0.89,5.53-2.64
			L678.78,114.55z M675.77,105.04c-0.24-3.57-2.03-6.42-5.89-6.42c-3.82,0-5.85,2.76-6.22,6.42H675.77z'/>
		<path class='st0' d='M686.83,111.99h4.15l-2.8,11.62h-3.05L686.83,111.99z'/>
		<path class='st0' d='M228.23,164.68v-31.2h19.99v3.57h-15.88v10.56h13.89v3.53h-13.89v13.53H228.23z'/>
		<path class='st0' d='M252.04,153.79c0-6.34,3.9-11.37,10.72-11.37c6.86,0,10.68,5.12,10.68,11.37c0,6.21-3.82,11.33-10.68,11.33
			C255.94,165.13,252.04,160.09,252.04,153.79z M255.94,153.79c0,4.31,2.28,8.04,6.78,8.04s6.82-3.74,6.82-8.04
			c0-4.35-2.32-8.08-6.82-8.08S255.94,149.45,255.94,153.79z'/>
		<path class='st0' d='M278.65,153.79c0-6.34,3.9-11.37,10.72-11.37c6.86,0,10.68,5.12,10.68,11.37c0,6.21-3.82,11.33-10.68,11.33
			C282.55,165.13,278.65,160.09,278.65,153.79z M282.55,153.79c0,4.31,2.28,8.04,6.78,8.04s6.82-3.74,6.82-8.04
			c0-4.35-2.32-8.08-6.82-8.08S282.55,149.45,282.55,153.79z'/>
		<path class='st0' d='M305.39,153.96c0-6.42,3.78-11.54,9.91-11.54c2.88,0,5.4,1.26,7.11,3.61h0.08v-12.55h3.86v31.2h-3.78
			l-0.04-3.53h-0.08c-1.83,2.64-4.43,3.98-7.35,3.98C308.96,165.13,305.39,160.05,305.39,153.96z M322.61,153.75
			c0-4.79-2.52-7.96-6.7-7.96c-4.55,0-6.62,3.86-6.62,8.04c0,4.06,2.07,7.92,6.5,7.92C320.09,161.76,322.61,158.38,322.61,153.75z'
			/>
		<path class='st0' d='M358.53,164.68l-0.08-3.17h-0.08c-1.22,2.15-3.45,3.61-6.58,3.61c-4.27,0-7.03-2.72-7.03-6.58
			c0-2.84,1.46-4.75,4.22-5.65c2.4-0.77,5.93-0.94,9.51-1.06v-1.95c0-2.23-1.54-4.22-4.83-4.22c-2.76,0-4.35,1.3-5.04,3.29
			l-3.21-1.54c1.26-3.29,4.39-5,8.37-5c5.2,0,8.53,2.72,8.53,7.47v14.79H358.53z M358.48,156.07v-1.26
			c-1.46,0.04-3.74,0.2-5.56,0.45c-2.48,0.33-4.31,1.01-4.31,3.25c0,2.03,1.54,3.37,4.06,3.37
			C356.21,161.88,358.48,159.2,358.48,156.07z'/>
		<path class='st0' d='M370.27,142.87H374l0.08,3.21h0.08c1.38-2.19,3.82-3.66,7.07-3.66c4.71,0,7.68,3.05,7.68,7.64v14.62h-3.86
			v-13.85c0-2.84-1.67-5.08-4.96-5.08c-3.65,0-5.97,2.52-5.97,5.93v13h-3.86V142.87z'/>
		<path class='st0' d='M395.58,153.96c0-6.42,3.78-11.54,9.91-11.54c2.88,0,5.4,1.26,7.11,3.61h0.08v-12.55h3.86v31.2h-3.78
			l-0.04-3.53h-0.08c-1.83,2.64-4.43,3.98-7.35,3.98C399.15,165.13,395.58,160.05,395.58,153.96z M412.8,153.75
			c0-4.79-2.52-7.96-6.7-7.96c-4.55,0-6.62,3.86-6.62,8.04c0,4.06,2.07,7.92,6.5,7.92C410.28,161.76,412.8,158.38,412.8,153.75z'/>
		<path class='st0' d='M436.49,164.68v-31.2h20.03v3.57h-15.96v10.15h13.97v3.54h-13.97v10.36h16.25v3.57H436.49z'/>
		<path class='st0' d='M462.98,142.87h3.74l0.08,3.21h0.08c1.38-2.19,3.82-3.66,7.07-3.66c4.71,0,7.68,3.05,7.68,7.64v14.62h-3.86
			v-13.85c0-2.84-1.67-5.08-4.96-5.08c-3.65,0-5.97,2.52-5.97,5.93v13h-3.86V142.87z'/>
		<path class='st0' d='M507.67,142.87l-8.61,21.81h-4.1l-8.57-21.81h4.22l6.42,17.39h0.08l6.5-17.39H507.67z'/>
		<path class='st0' d='M512.87,133.48h4.18v4.18h-4.18V133.48z M513.03,164.68v-21.81h3.86v21.81H513.03z'/>
		<path class='st0' d='M524.94,142.87h3.74l0.08,3.29h0.08c1.46-2.72,3.49-3.74,5.61-3.74c0.77,0,1.34,0.12,1.83,0.28v3.78
			c-0.37-0.12-1.14-0.28-2.03-0.28c-3.62,0-5.44,2.56-5.44,5.24v13.24h-3.86V142.87z'/>
		<path class='st0' d='M540.25,153.79c0-6.34,3.9-11.37,10.72-11.37c6.86,0,10.68,5.12,10.68,11.37c0,6.21-3.82,11.33-10.68,11.33
			C544.15,165.13,540.25,160.09,540.25,153.79z M544.15,153.79c0,4.31,2.28,8.04,6.78,8.04s6.82-3.74,6.82-8.04
			c0-4.35-2.32-8.08-6.82-8.08S544.15,149.45,544.15,153.79z'/>
		<path class='st0' d='M568.29,142.87h3.74l0.08,3.21h0.08c1.38-2.19,3.82-3.66,7.07-3.66c4.71,0,7.68,3.05,7.68,7.64v14.62h-3.86
			v-13.85c0-2.84-1.67-5.08-4.96-5.08c-3.65,0-5.97,2.52-5.97,5.93v13h-3.86V142.87z'/>
		<path class='st0' d='M598.79,145.79c1.34-2.07,3.41-3.37,6.42-3.37c3.13,0,5.48,1.42,6.58,3.74c1.58-2.56,4.1-3.74,7.07-3.74
			c4.39,0,7.27,2.76,7.27,6.91v15.35h-3.86v-14.46c0-2.52-1.46-4.47-4.51-4.47c-3.25,0-5.32,2.19-5.32,5.2v13.73h-3.86v-14.46
			c0-2.52-1.5-4.47-4.47-4.47c-3.25,0-5.36,2.19-5.36,5.2v13.73h-3.86v-21.81h3.74l0.08,2.93H598.79z'/>
		<path class='st0' d='M651.69,161.59c-1.95,2.27-4.75,3.53-8.25,3.53c-6.87,0-10.76-4.51-10.76-11.37c0-6.58,3.9-11.33,10.2-11.33
			c6.25,0,9.63,4.67,9.63,10.64c0,0.65-0.04,1.66-0.12,2.23h-15.76c0.41,4.14,2.8,6.54,6.86,6.54c2.4,0,4.23-0.89,5.53-2.64
			L651.69,161.59z M648.68,152.09c-0.24-3.57-2.03-6.42-5.89-6.42c-3.82,0-5.85,2.76-6.22,6.42H648.68z'/>
		<path class='st0' d='M658.96,142.87h3.74l0.08,3.21h0.08c1.38-2.19,3.82-3.66,7.07-3.66c4.71,0,7.68,3.05,7.68,7.64v14.62h-3.86
			v-13.85c0-2.84-1.67-5.08-4.96-5.08c-3.65,0-5.97,2.52-5.97,5.93v13h-3.86V142.87z'/>
		<path class='st0' d='M682.57,142.87h4.35v-6.91h3.86v6.91h5.24v3.21h-5.24v11.94c0,2.6,0.69,3.78,3.53,3.78
			c0.73,0,1.26-0.08,1.71-0.16v3.13c-0.85,0.2-1.83,0.37-3.05,0.37c-4.15,0-6.05-2.27-6.05-6.13v-12.92h-4.35V142.87z'/>
		<path class='st0' d='M243.78,187.09h3.14v16.13c0,5.39-3.69,8.85-9.81,8.85c-5.61,0-9.72-3.3-9.72-8.85v-16.13h3.24v15.97
			c0,3.72,2.54,6.22,6.61,6.22c4.07,0,6.54-2.37,6.54-6.22V187.09z'/>
		<path class='st0' d='M252.8,194.5h2.95l0.06,2.54h0.06c1.09-1.73,3.02-2.89,5.58-2.89c3.72,0,6.06,2.41,6.06,6.03v11.55h-3.05
			v-10.94c0-2.24-1.32-4.01-3.91-4.01c-2.89,0-4.71,1.99-4.71,4.68v10.26h-3.05V194.5z'/>
		<path class='st0' d='M273.04,187.09h3.3v3.3h-3.3V187.09z M273.16,211.72V194.5h3.05v17.22H273.16z'/>
		<path class='st0' d='M296.61,194.5l-6.8,17.22h-3.24l-6.77-17.22h3.33l5.07,13.73h0.06l5.13-13.73H296.61z'/>
		<path class='st0' d='M313.83,209.28c-1.54,1.8-3.75,2.79-6.51,2.79c-5.42,0-8.5-3.56-8.5-8.98c0-5.2,3.08-8.95,8.05-8.95
			c4.94,0,7.6,3.69,7.6,8.4c0,0.51-0.03,1.32-0.1,1.76h-12.44c0.32,3.27,2.21,5.16,5.42,5.16c1.89,0,3.34-0.71,4.36-2.08
			L313.83,209.28z M311.46,201.78c-0.19-2.82-1.6-5.07-4.65-5.07c-3.01,0-4.62,2.18-4.91,5.07H311.46z'/>
		<path class='st0' d='M318.93,194.5h2.95l0.06,2.6h0.06c1.15-2.15,2.76-2.95,4.42-2.95c0.61,0,1.06,0.1,1.45,0.23v2.98
			c-0.29-0.1-0.9-0.22-1.6-0.22c-2.85,0-4.3,2.02-4.3,4.14v10.46h-3.05V194.5z'/>
		<path class='st0' d='M329.9,209.48l1.86-2.08c1.51,1.44,3.4,2.12,5.45,2.12c2.73,0,4.04-0.99,4.04-2.47
			c0-1.64-1.73-2.02-4.62-2.63c-3.24-0.71-6.13-1.7-6.13-5.03c0-2.95,2.31-5.23,6.8-5.23c2.76,0,4.91,0.84,6.71,2.47l-1.83,2.08
			c-1.47-1.35-3.11-1.99-4.97-1.99c-2.41,0-3.62,1.03-3.62,2.34c0,1.6,1.54,1.96,4.52,2.6c3.18,0.71,6.22,1.6,6.22,5.04
			c0,3.14-2.5,5.39-7.22,5.39C334.07,212.07,331.7,211.18,329.9,209.48z'/>
		<path class='st0' d='M348.54,187.09h3.3v3.3h-3.3V187.09z M348.66,211.72V194.5h3.05v17.22H348.66z'/>
		<path class='st0' d='M355.21,194.5h3.43v-5.45h3.05v5.45h4.14v2.54h-4.14v9.43c0,2.05,0.55,2.98,2.79,2.98
			c0.58,0,1-0.06,1.35-0.13v2.47c-0.67,0.16-1.45,0.29-2.41,0.29c-3.27,0-4.78-1.8-4.78-4.84v-10.2h-3.43V194.5z'/>
		<path class='st0' d='M384.52,194.5l-7.31,18.54c-1.47,3.75-2.98,4.91-5.55,4.91c-0.9,0-1.57-0.16-2.08-0.35v-2.47
			c0.38,0.13,0.83,0.23,1.44,0.23c1.7,0,2.73-0.67,3.53-3.18l0.06-0.19l-6.9-17.48h3.33l5.07,13.73h0.06l5.13-13.73H384.52z'/>
		<path class='st0' d='M394.79,203.13c0-5,3.08-8.98,8.47-8.98c5.42,0,8.43,4.04,8.43,8.98c0,4.91-3.02,8.95-8.43,8.95
			C397.87,212.07,394.79,208.1,394.79,203.13z M397.87,203.13c0,3.4,1.8,6.35,5.36,6.35s5.39-2.95,5.39-6.35
			c0-3.43-1.83-6.38-5.39-6.38S397.87,199.69,397.87,203.13z'/>
		<path class='st0' d='M417.56,197.03h-3.43v-2.54h3.43v-2.89c0-3.18,1.89-4.88,5.16-4.88c0.77,0,1.41,0.1,2.02,0.26v2.54
			c-0.32-0.1-0.87-0.16-1.41-0.16c-1.99,0-2.73,0.8-2.73,2.63v2.5h4.14v2.54h-4.14v14.69h-3.05V197.03z'/>
		<path class='st0' d='M445.18,200.72h-4.65v11h-3.24v-24.63h3.24v10.84h4.71l6.61-10.84h3.69L448,199.24l7.86,12.48h-3.85
			L445.18,200.72z'/>
		<path class='st0' d='M472.15,209.28c-1.54,1.8-3.75,2.79-6.51,2.79c-5.42,0-8.5-3.56-8.5-8.98c0-5.2,3.08-8.95,8.05-8.95
			c4.94,0,7.6,3.69,7.6,8.4c0,0.51-0.03,1.32-0.1,1.76h-12.44c0.32,3.27,2.21,5.16,5.42,5.16c1.89,0,3.34-0.71,4.36-2.08
			L472.15,209.28z M469.78,201.78c-0.19-2.82-1.6-5.07-4.65-5.07c-3.01,0-4.62,2.18-4.91,5.07H469.78z'/>
		<path class='st0' d='M477.25,194.5h2.95l0.06,2.54h0.06c1.09-1.73,3.02-2.89,5.58-2.89c3.72,0,6.06,2.41,6.06,6.03v11.55h-3.05
			v-10.94c0-2.24-1.32-4.01-3.91-4.01c-2.89,0-4.71,1.99-4.71,4.68v10.26h-3.05V194.5z'/>
		<path class='st0' d='M495.25,194.5h3.43v-5.45h3.05v5.45h4.14v2.54h-4.14v9.43c0,2.05,0.55,2.98,2.79,2.98
			c0.58,0,1-0.06,1.35-0.13v2.47c-0.67,0.16-1.45,0.29-2.41,0.29c-3.27,0-4.78-1.8-4.78-4.84v-10.2h-3.43V194.5z'/>
		<path class='st0' d='M524.53,211.72h-2.98l-0.03-2.53h-0.06c-1.12,1.7-3.08,2.89-5.58,2.89c-3.72,0-6.06-2.41-6.06-6.03V194.5
			h3.05v10.94c0,2.24,1.25,4.01,3.91,4.01c2.89,0,4.71-1.99,4.71-4.68V194.5h3.05V211.72z'/>
		<path class='st0' d='M529.15,203.19c0-5.1,3.05-9.05,8.34-9.05c3.27,0,5.58,1.57,6.9,4.04l-2.54,1.44
			c-0.86-1.67-2.24-2.89-4.42-2.89c-3.37,0-5.2,2.82-5.2,6.38c0,3.46,1.76,6.35,5.39,6.35c2.28,0,3.66-1.15,4.49-2.79l2.41,1.38
			c-1.22,2.41-3.66,4.01-6.99,4.01C532.23,212.07,529.15,208.26,529.15,203.19z'/>
		<path class='st0' d='M553.62,204.02h-2.47v7.7h-3.05v-24.63h3.05v14.4h2.63l4.75-6.99h3.63l-5.87,8.28l6.16,8.95h-3.69
			L553.62,204.02z'/>
		<path class='st0' d='M580.66,194.5l-7.31,18.54c-1.47,3.75-2.98,4.91-5.55,4.91c-0.9,0-1.57-0.16-2.08-0.35v-2.47
			c0.38,0.13,0.83,0.23,1.44,0.23c1.7,0,2.73-0.67,3.53-3.18l0.06-0.19l-6.9-17.48h3.33l5.07,13.73h0.06l5.13-13.73H580.66z'/>
	</g>
	<polygon class='st0' points='61.72,84.88 51.22,84.88 51.22,28.91 61.72,28.91 61.72,14.91 12.74,14.91 12.74,28.91 23.23,28.91 
		23.23,84.88 37.23,98.88 61.72,98.88 	'/>
	<polygon class='st0' points='96.7,98.88 121.19,98.88 135.18,84.88 135.18,28.91 145.67,28.91 145.67,14.91 96.7,14.91 96.7,28.91 
		107.19,28.91 107.19,84.88 96.7,84.88 	'/>
	<polygon class='st0' points='146.55,111.99 177.16,81.38 191.15,81.38 191.15,67.39 138.68,67.39 138.68,81.38 143.93,81.38 
		122.94,102.37 93.2,102.37 93.2,81.38 103.7,81.38 103.7,67.39 54.72,67.39 54.72,81.38 65.21,81.38 65.21,144.35 54.72,144.35 
		54.72,158.35 103.7,158.35 103.7,144.35 93.2,144.35 93.2,123.36 122.94,123.36 143.92,144.35 138.68,144.35 138.68,158.35 
		191.15,158.35 191.15,144.35 178.91,144.35 	'/>
	<path class='st0' d='M194.65,161.85h-59.47v-20.99h0l-13.99-14H96.7v14h10.5v20.99H51.22v-20.99h10.5v-38.48H35.48L19.73,86.63
		V32.41h0H9.24V11.42h55.97v20.99h-10.5v31.48h48.98V32.41H93.2V11.42h55.98v20.99h-10.5v31.48h55.97v20.99h-15.74l-27.11,27.11
		l28.86,28.86h13.99V161.85z M182.41,137.36l-25.36-25.36l23.61-23.61h17.49l0-27.99l-55.97,0V35.9h10.5V7.92H89.7h0V35.9h0h10.49
		V60.4H58.21V35.9h10.5h0V7.92h0H5.74V35.9h10.5v52.48h0l0,0l17.49,17.49h24.49v31.49h-10.5v27.99h62.97v-27.99h-10.5v-7h19.24
		l12.24,12.25v22.74h66.47l0-27.99h0H182.41z'/>
	<path class='st0' d='M201.38,161.18c0-0.59,0.11-1.13,0.33-1.64c0.22-0.51,0.52-0.95,0.9-1.31c0.38-0.37,0.82-0.66,1.32-0.87
		c0.5-0.21,1.03-0.32,1.6-0.32c0.56,0,1.1,0.11,1.6,0.32s0.94,0.5,1.32,0.87c0.38,0.37,0.68,0.81,0.9,1.31
		c0.22,0.51,0.33,1.06,0.33,1.64c0,0.59-0.11,1.14-0.33,1.65c-0.22,0.51-0.52,0.94-0.9,1.31c-0.38,0.37-0.82,0.66-1.32,0.87
		c-0.5,0.21-1.03,0.32-1.6,0.32c-0.56,0-1.1-0.11-1.6-0.32c-0.5-0.21-0.94-0.5-1.32-0.87c-0.38-0.37-0.68-0.8-0.9-1.31
		C201.49,162.33,201.38,161.78,201.38,161.18z M202.12,161.18c0,0.5,0.09,0.97,0.27,1.4c0.18,0.43,0.42,0.8,0.72,1.12
		c0.3,0.32,0.67,0.56,1.08,0.75c0.42,0.18,0.86,0.27,1.34,0.27c0.48,0,0.92-0.09,1.33-0.27c0.41-0.18,0.77-0.43,1.08-0.75
		c0.31-0.32,0.55-0.69,0.73-1.12c0.18-0.43,0.27-0.9,0.27-1.4c0-0.5-0.09-0.96-0.27-1.4c-0.18-0.43-0.42-0.81-0.73-1.12
		c-0.31-0.32-0.67-0.56-1.08-0.75c-0.41-0.18-0.86-0.27-1.33-0.27c-0.47,0-0.92,0.09-1.34,0.27c-0.42,0.18-0.78,0.43-1.08,0.75
		c-0.3,0.32-0.55,0.69-0.72,1.12C202.21,160.22,202.12,160.69,202.12,161.18z M203.94,158.76h1.83c1.13,0,1.69,0.46,1.69,1.38
		c0,0.44-0.12,0.76-0.37,0.96c-0.25,0.2-0.55,0.33-0.91,0.36l1.39,2.14h-0.79l-1.31-2.08h-0.79v2.08h-0.74V158.76z M204.68,160.92
		h0.76c0.16,0,0.32-0.01,0.48-0.02c0.16-0.01,0.29-0.04,0.41-0.09c0.12-0.05,0.22-0.13,0.29-0.25c0.07-0.11,0.11-0.26,0.11-0.46
		c0-0.16-0.03-0.3-0.1-0.4c-0.07-0.1-0.15-0.18-0.26-0.23c-0.1-0.05-0.22-0.09-0.35-0.1c-0.13-0.01-0.25-0.02-0.38-0.02h-0.97
		V160.92z'/>
	<path class='st0' d='M580.43,207.63c0-0.59,0.11-1.13,0.33-1.64c0.22-0.51,0.52-0.95,0.9-1.31c0.38-0.37,0.82-0.66,1.32-0.87
		c0.5-0.21,1.03-0.32,1.6-0.32c0.56,0,1.1,0.11,1.6,0.32c0.5,0.21,0.94,0.5,1.32,0.87c0.38,0.37,0.68,0.81,0.9,1.31
		c0.22,0.51,0.33,1.06,0.33,1.64c0,0.59-0.11,1.14-0.33,1.65c-0.22,0.51-0.52,0.94-0.9,1.31c-0.38,0.37-0.82,0.66-1.32,0.87
		c-0.5,0.21-1.03,0.32-1.6,0.32c-0.56,0-1.1-0.11-1.6-0.32c-0.5-0.21-0.94-0.5-1.32-0.87c-0.38-0.37-0.68-0.8-0.9-1.31
		C580.54,208.77,580.43,208.22,580.43,207.63z M581.16,207.63c0,0.5,0.09,0.97,0.27,1.4c0.18,0.43,0.42,0.8,0.72,1.12
		c0.3,0.32,0.66,0.56,1.08,0.75c0.42,0.18,0.86,0.27,1.34,0.27c0.48,0,0.92-0.09,1.33-0.27c0.41-0.18,0.77-0.43,1.08-0.75
		c0.31-0.32,0.55-0.69,0.73-1.12c0.18-0.43,0.27-0.9,0.27-1.4c0-0.5-0.09-0.96-0.27-1.4c-0.18-0.43-0.42-0.81-0.73-1.12
		c-0.31-0.32-0.67-0.56-1.08-0.75c-0.41-0.18-0.86-0.27-1.33-0.27c-0.47,0-0.92,0.09-1.34,0.27c-0.42,0.18-0.78,0.43-1.08,0.75
		c-0.3,0.32-0.55,0.69-0.72,1.12C581.25,206.67,581.16,207.13,581.16,207.63z M582.99,205.2h1.83c1.13,0,1.69,0.46,1.69,1.38
		c0,0.44-0.12,0.76-0.37,0.96c-0.25,0.2-0.55,0.33-0.91,0.36l1.39,2.14h-0.79l-1.31-2.08h-0.79v2.08h-0.74V205.2z M583.73,207.36
		h0.76c0.16,0,0.32-0.01,0.48-0.02c0.16-0.01,0.29-0.04,0.41-0.09c0.12-0.05,0.22-0.13,0.29-0.25c0.07-0.11,0.11-0.26,0.11-0.46
		c0-0.16-0.03-0.3-0.1-0.4c-0.07-0.1-0.15-0.18-0.26-0.23c-0.1-0.05-0.22-0.09-0.35-0.1c-0.13-0.01-0.25-0.02-0.38-0.02h-0.97
		V207.36z'/>
	<g>
		<path class='st0' d='M296.12,62.99v1.52h-26.6v-1.52l8.84-1.61V12.2h-0.62l-20.89,52.31h-2.14L232.92,12.2h-0.63v48.38l7.05,2.41
			v1.52H222.3v-1.52l7.05-2.41V10.95l-7.05-1.79V7.65h18.75l3.48,9.2l14.28,35.26h0.45l13.93-35.26l3.3-9.2h19.64v1.52l-8.84,1.61
			v50.62L296.12,62.99z'/>
		<path class='st0' d='M331.93,59.42c0,2.59,2.14,3.39,7.5,2.86l0.18,0.98c-1.7,1.16-6.16,2.68-9.37,2.68
			c-3.39,0-5.18-1.88-5.45-6.61l-0.36-0.18c-4.11,4.11-8.66,6.78-13.48,6.78c-5.8,0-9.55-3.39-9.55-8.84
			c0-8.04,9.55-12.41,23.03-14.19v-6.7c0-5.98-3.84-9.1-9.19-9.1c-1.96,0-4.28,0.45-6.61,1.34c1.96,1.61,2.77,3.48,2.77,5.18
			c0,2.23-1.7,4.28-4.2,4.28c-2.68,0-4.28-2.05-4.28-4.37c0-5.62,9.28-10.36,17.76-10.36c7.86,0,11.25,4.2,11.25,12.23V59.42z
			 M324.43,45.5c-11.43,0-15.18,4.11-15.18,8.93c0,3.84,2.5,6.16,6.43,6.16c2.41,0,5.45-0.89,8.75-2.68V45.5z'/>
		<path class='st0' d='M367.19,33.09c-2.41,0-4.37-1.88-4.82-4.73c-3.3,0.27-6.69,2.86-7.94,5.8v27.58l6.16,1.61v1.16h-19.64v-1.16
			l5.89-1.61V30.77l-5.8-3.75v-0.8l12.5-3.21l0.36,0.27l-0.18,8.75l0.36,0.27c2.86-5.53,7.59-9.19,11.69-9.19
			c4.02,0,5.98,2.5,5.98,5.18C371.74,31.12,369.87,33.09,367.19,33.09z'/>
		<path class='st0' d='M388.62,65.22c-6.78,0-8.75-3.04-8.75-9.37V28.54h-6.25v-1.16l6.96-4.64l5.27-8.12h1.52V24.7h13.57v3.84
			h-13.57v24.28c0,3.21,0.27,5.27,2.14,6.52c1.25,0.71,3.48,1.07,6.6,1.07c1.43,0,3.13-0.09,5-0.18l0.09,0.89
			C397.28,63.53,392.82,65.22,388.62,65.22z'/>
		<path class='st0' d='M403.8,27.02v-0.8L416.83,23l0.36,0.27v38.48l5.98,1.61v1.16h-19.73v-1.16l6.25-1.61V30.77L403.8,27.02z
			 M413.71,4.61c2.77,0,4.82,2.05,4.82,4.82c0,2.86-2.32,5.45-5.36,5.45c-2.77,0-4.73-2.05-4.73-4.91
			C408.44,7.2,410.76,4.61,413.71,4.61z'/>
		<path class='st0' d='M466.65,61.74l5.89,1.61v1.16h-19.01v-1.16l5.62-1.61V39.34c0-6.78-2.41-10-8.39-10
			c-3.75,0-6.43,0.71-10,2.77v29.64l5.53,1.61v1.16h-19.01v-1.16l5.89-1.61V30.77l-5.8-3.75v-0.8l12.5-3.21l0.36,0.27l-0.18,7.14
			l0.36,0.18c5.36-5,10.45-7.5,15.26-7.5c8.04,0,10.98,6.07,10.98,12.41V61.74z'/>
		<path class='st0' d='M477.36,46.12v-5.09h21.07v5.09H477.36z'/>
		<path class='st0' d='M556.37,25.32c-5.89-11.78-12.14-16.43-20-16.43c-8.57,0-17.23,7.68-17.23,26.24
			c0,18.21,8.84,27.14,21.07,27.14c3.84,0,7.32-1.16,10.18-2.59V43.26l-7.68-1.61v-1.52h23.12v1.52l-6.52,1.61v21.96l-0.89,0.36
			l-6.6-4.28c-4.2,2.77-9.19,4.55-15.09,4.55c-16.25,0-27.85-11.96-27.85-29.19c0-16.52,12.94-30.35,30.53-30.35
			c4.55,0,9.46,0.98,12.85,2.77l3.93-3.03h0.98l0.62,19.1L556.37,25.32z'/>
		<path class='st0' d='M600.03,59.42c0,2.59,2.14,3.39,7.5,2.86l0.18,0.98c-1.7,1.16-6.16,2.68-9.37,2.68
			c-3.39,0-5.18-1.88-5.45-6.61l-0.36-0.18c-4.11,4.11-8.66,6.78-13.48,6.78c-5.8,0-9.55-3.39-9.55-8.84
			c0-8.04,9.55-12.41,23.03-14.19v-6.7c0-5.98-3.84-9.1-9.19-9.1c-1.96,0-4.28,0.45-6.61,1.34c1.96,1.61,2.77,3.48,2.77,5.18
			c0,2.23-1.7,4.28-4.2,4.28c-2.68,0-4.28-2.05-4.28-4.37c0-5.62,9.28-10.36,17.76-10.36c7.86,0,11.25,4.2,11.25,12.23V59.42z
			 M592.53,45.5c-11.43,0-15.18,4.11-15.18,8.93c0,3.84,2.5,6.16,6.43,6.16c2.41,0,5.45-0.89,8.75-2.68V45.5z'/>
		<path class='st0' d='M622.7,65.22c-6.78,0-8.75-3.04-8.75-9.37V28.54h-6.25v-1.16l6.96-4.64l5.27-8.12h1.52V24.7h13.57v3.84
			h-13.57v24.28c0,3.21,0.27,5.27,2.14,6.52c1.25,0.71,3.48,1.07,6.6,1.07c1.43,0,3.13-0.09,5-0.18l0.09,0.89
			C631.36,63.53,626.9,65.22,622.7,65.22z'/>
		<path class='st0' d='M651.09,65.22c-6.78,0-8.75-3.04-8.75-9.37V28.54h-6.25v-1.16l6.96-4.64l5.27-8.12h1.52V24.7h13.57v3.84
			h-13.57v24.28c0,3.21,0.27,5.27,2.14,6.52c1.25,0.71,3.48,1.07,6.6,1.07c1.43,0,3.13-0.09,5-0.18l0.09,0.89
			C659.75,63.53,655.29,65.22,651.09,65.22z'/>
		<path class='st0' d='M685.38,65.94c-12.59,0-19.46-9.19-19.46-21.42c0-11.43,7.68-21.42,20.71-21.42
			c12.77,0,19.37,8.93,19.37,21.16C706,55.67,698.5,65.94,685.38,65.94z M685.64,25.05c-6.16,0-11.07,6.34-11.07,19.73
			c0,13.03,5.45,19.19,11.69,19.19c6.61,0,11.07-6.34,11.07-19.73C697.34,31.21,692.34,25.05,685.64,25.05z'/>
		<path class='st0' d='M750.37,61.74l5.89,1.61v1.16h-19.01v-1.16l5.62-1.61V39.34c0-6.78-2.41-10-8.39-10
			c-3.75,0-6.43,0.71-10,2.77v29.64l5.53,1.61v1.16H711v-1.16l5.89-1.61V30.77l-5.8-3.75v-0.8l12.5-3.21l0.36,0.27l-0.18,7.14
			l0.36,0.18c5.36-5,10.44-7.5,15.26-7.5c8.03,0,10.98,6.07,10.98,12.41V61.74z'/>
	</g>
</g>
</svg>
";
		

        
    }
}
