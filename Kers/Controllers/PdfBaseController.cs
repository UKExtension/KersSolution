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
    public class PdfBaseController : Controller
    {

        public KERScoreContext _context;
        public KERSmainContext mainContext;
        public IKersUserRepository userRepo;
        public IMemoryCache _cache;

		const int width = 612;
		const int height = 792;

		string[] typefaceNames = {	"HelveticaNeue", "HelveticaNeue-Bold", 
									"HelveticaNeue-CondensedBold", "HelveticaNeue-Light"
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
			
			
			var textTop = "Page " + page.ToString() + " of " + totalPages.ToString();
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
				text = user.RprtngProfile.PlanningUnit.FullName;
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
				var matrix = SKMatrix.MakeTranslation(x, y);
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

        
    }
}
