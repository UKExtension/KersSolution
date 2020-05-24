using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Kers.Services;
using Kers.Services.FroalaWisiwyg;

namespace Kers.Controllers
{
    [Route("api/[controller]")]
    public class LadderController : BaseController
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        KERScoreContext _context;
        IFiscalYearRepository fiscalYearRepo;
        public LadderController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IHostingEnvironment hostingEnvironment,
                    IFiscalYearRepository fiscalYearRepo
            ):base(mainContext, context, userRepo){
                _context = context;
                _hostingEnvironment = hostingEnvironment;
                this.fiscalYearRepo = fiscalYearRepo;
        }




        [HttpGet("levels")]
        [Authorize]
        public async Task<IActionResult> Levels(){
            var levels = context.LadderLevel.OrderBy( o => o.Order);
            return new OkObjectResult(await levels.ToListAsync());
        }
        [HttpGet("educationlevels")]
        [Authorize]
        public async Task<IActionResult> EducationLevels(){
            var levels = context.LadderEducationLevel.OrderBy( o => o.Order);
            return new OkObjectResult(await levels.ToListAsync());
        }

        [HttpGet("getStage/{StageId}")]
        [Authorize]
        public async Task<IActionResult> GetStage(int StageId){
            var stage = context.LadderStage.Where(a => a.Id == StageId)
                            .Include( a => a.LadderStageRoles).ThenInclude( r => r.zEmpRoleType)
                            .FirstOrDefaultAsync();
            return new OkObjectResult(await stage);
        }

        [HttpGet("GetApplicationsForReview/{StageId}")]
        [Authorize]
        public async Task<IActionResult> GetApplicationsForReview(int StageId){
            var apps = context.LadderApplication.Where(a => a.LastStageId == StageId)
                            .Include( a => a.KersUser).ThenInclude( u => u.RprtngProfile).ThenInclude( p => p.PlanningUnit)
                            .ToListAsync();
            var appls = await apps;
            foreach( var app in appls) app.KersUser.RprtngProfile.PlanningUnit.GeoFeature = null;
            return new OkObjectResult(appls);
        }


        [HttpPost("review/{Approved}")]
        [Authorize]
        public IActionResult ReviewLadderApplication( Boolean Approved, [FromBody] LadderApplicationStage ApplStage){
            var application = context.LadderApplication.Where( a => a.Id == ApplStage.LadderApplicationId)
                                .Include( a => a.Stages).ThenInclude( s => s.LadderStage)
                                .Include( a => a.KersUser)
                                .Include( a => a.LadderLevel)
                                .FirstOrDefault();


            var LastStage = application.Stages.OrderBy( a => a.Created ).Last();
            LastStage.KersUser = this.CurrentUser();
            LastStage.Reviewed = DateTime.Now;
            LastStage.Note = ApplStage.Note;
            if( Approved ){
                var Next = this.NextStage(LastStage.LadderStage.Id);
                if( Next == null ){
                    application.Approved = true;
                    var Approval = new LadderKersUserLevel();
                    Approval.LadderApplication = application;
                    Approval.KersUser = application.KersUser;
                    Approval.Created = DateTime.Now;
                    Approval.LadderLevel = application.LadderLevel;
                    context.Add(Approval);
                }else{
                    var NextStage = new LadderApplicationStage();
                    NextStage.LadderStage = Next;
                    NextStage.Created = DateTime.Now;
                    application.Stages.Add( NextStage );
                    application.LastStageId = Next.Id;
                }
            }else{
                var Previous = this.PreviousStage( LastStage.LadderStage.Id );
                if( Previous == null ){
                    application.Draft = true;
                    application.LastStageId = null;
                }else{
                    var NextStage = new LadderApplicationStage();
                    NextStage.LadderStage = Previous;
                    NextStage.Created = DateTime.Now;
                    application.Stages.Add( NextStage );
                    application.LastStageId = Previous.Id;
                }
            }
            this.context.SaveChanges();
            return new OkObjectResult(application);
        }

        private LadderStage NextStage( int StageId){
            var current = context.LadderStage.Find(StageId);
            if( current != null){
                var nxt = context.LadderStage.Where( a => a.Order > current.Order).OrderBy( a => a.Order ).FirstOrDefault();
                return nxt;
            }
            return null;
        }

        [HttpGet("nextstage/{StageId}")]
        [Authorize]
        public IActionResult NextStageAction(int StageId){
            var n = NextStage(StageId);
            return new OkObjectResult(n);
        }

        [HttpGet("previousstage/{StageId}")]
        [Authorize]
        public IActionResult PreviousStageAction(int StageId){
            return new OkObjectResult(PreviousStage(StageId));
        }

        private LadderStage PreviousStage( int StageId){
            var current = context.LadderStage.Find(StageId);
            if( current != null){
                var prv = context.LadderStage.Where( a => a.Order < current.Order).OrderByDescending( a => a.Order ).FirstOrDefault();
                return prv;
            }
            return null;
        }

        [HttpGet("applicationsByUser/{UserId?}")]
        [Authorize]
        public async Task<IActionResult> ApplicationsByUser(int UserId = 0){
            if(UserId == 0){
                var user = this.CurrentUser();
                UserId = user.Id;
            }
            var appilcations = context.LadderApplication.Where( a => a.KersUserId == UserId).OrderBy( o => o.Created);
            return new OkObjectResult(await appilcations.ToListAsync());
        }
        [HttpGet("application/{Id}")]
        [Authorize]
        public async Task<IActionResult> Application(int Id){
           
            var appilcation = await  context.LadderApplication
                                            .Where( a => a.Id == Id)
                                            .Include( a => a.KersUser).ThenInclude( u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit)
                                            .Include( a => a.LadderEducationLevel)
                                            .Include( a => a.LadderLevel)
                                            .Include( a => a.Ratings)
                                            .Include( a => a.Images).ThenInclude( a => a.UploadImage)
                                            .Include( a => a.Stages).ThenInclude( s => s.LadderStage)
                                            .Include( a => a.Stages).ThenInclude( s => s.KersUser).ThenInclude( u => u.RprtngProfile)
                                            .FirstOrDefaultAsync();
            appilcation.KersUser.RprtngProfile.PlanningUnit.GeoFeature = "";
            return new OkObjectResult(appilcation);
        }

        [HttpGet("applicationByUserByFiscalYear/{UserId?}/{fy?}")]
        [Authorize]
        public async Task<IActionResult> ApplicationsByUserByFiscalYear(int UserId = 0, string fy="0"){
            FiscalYear fiscalYear;
            if(fy != "0"){
                fiscalYear = fiscalYearRepo.byName(fy, FiscalYearType.ServiceLog);
            }else{
                fiscalYear = fiscalYearRepo.currentFiscalYear(FiscalYearType.ServiceLog);
            }
            if(UserId == 0){
                var user = this.CurrentUser();
                UserId = user.Id;
            }
            var appilcations = context.LadderApplication
                            .Where( a => a.KersUserId == UserId && a.Created <= fiscalYear.End && a.Created >= fiscalYear.Start)
                            .Include( a => a.Ratings)
                            .Include( a => a.Images).ThenInclude( i => i.UploadImage)
                            .OrderBy( o => o.Created);
            var appl = await appilcations.FirstOrDefaultAsync();
            return new OkObjectResult(appl);
        }


        [HttpPost("addladder")]
        [Authorize]
        public IActionResult AddLadderApplication( [FromBody] LadderApplication LadderApplication){
            if(LadderApplication != null){
                LadderApplication.Created = DateTime.Now;
                LadderApplication.LastUpdated = DateTime.Now;
                if(!LadderApplication.Draft){
                    var FirstStage = context.LadderStage.OrderBy( s => s.Order).FirstOrDefault();
                    var FirstApplicationStage = new LadderApplicationStage();
                    FirstApplicationStage.LadderStage = FirstStage;
                    FirstApplicationStage.Created = DateTime.Now;
                    LadderApplication.Stages = new List<LadderApplicationStage>();
                    LadderApplication.Stages.Add( FirstApplicationStage );
                    LadderApplication.LastStage = FirstStage;
                }
                this.context.Add(LadderApplication);
                this.context.SaveChanges();
                foreach( var e in LadderApplication.Images ){
                    e.UploadImage = context.UploadImage.Where( i => i.Id == e.UploadImageId).FirstOrDefault();
                }
                return new OkObjectResult(LadderApplication);
            }else{
                this.Log( LadderApplication,"LadderApplication", "Error in adding LadderApplication attempt.", "LadderApplication", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("updateladder/{id}")]
        [Authorize]
        public IActionResult UpdateLadderApplication( int id, [FromBody] LadderApplication LadderApplication){
           
            var entity = this.context.LadderApplication.Where( a => a.Id == id)
                            .Include( a => a.Images)
                            .Include( a => a.Ratings)
                            .Include( a => a.Stages)
                            .FirstOrDefault();

            if(LadderApplication != null ){
                entity.LadderLevelId = LadderApplication.LadderLevelId;
                entity.PositionNumber = LadderApplication.PositionNumber;
                entity.ProgramOfStudy = LadderApplication.ProgramOfStudy;
                entity.Evidence = LadderApplication.Evidence;
                entity.NumberOfYears = LadderApplication.NumberOfYears;
                entity.LastPromotion = LadderApplication.LastPromotion;
                entity.StartDate = LadderApplication.StartDate;
                entity.LadderEducationLevelId = LadderApplication.LadderEducationLevelId;
                entity.Draft = LadderApplication.Draft;
                if(!LadderApplication.Draft){
                    var FirstStage = context.LadderStage.OrderBy( s => s.Order).FirstOrDefault();
                    var FirstApplicationStage = new LadderApplicationStage();
                    FirstApplicationStage.LadderStage = FirstStage;
                    entity.Stages.Add( FirstApplicationStage );
                    entity.LastStage = FirstStage;
                }
                entity.Ratings = LadderApplication.Ratings;
                entity.Images = LadderApplication.Images;
                entity.LastUpdated = DateTime.Now;
                this.context.SaveChanges();
                this.Log(LadderApplication,"LadderApplication", "LadderApplication Updated.");
                foreach( var e in entity.Images ){
                    e.UploadImage = context.UploadImage.Where( i => i.Id == e.UploadImageId).FirstOrDefault();
                }
                return new OkObjectResult(entity);
            }else{
                this.Log( LadderApplication ,"LadderApplication", "Not Found LadderApplication in an update attempt.", "LadderApplication", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("deleteladder/{id}")]
        [Authorize]
        public IActionResult DeleteLadderApplication( int id ){
            var entity = context.LadderApplication
                            .Where( a => a.Id == id)
                            .Include( a => a.Ratings)
                            .Include( a => a.Images).ThenInclude( i => i.UploadImage).ThenInclude( m => m.UploadFile)
                            .FirstOrDefault();
            
            if(entity != null){
                foreach( var img in entity.Images){
                    context.Remove( img.UploadImage.UploadFile);
                    context.Remove( img.UploadImage);
                }
                
                context.LadderApplication.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"LadderApplication", "LadderApplication Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"LadderApplication", "Not Found LadderApplication in a delete attempt.", "LadderApplication", "Error");
                return new StatusCodeResult(500);
            } 
        }

        


        [HttpDelete("deleteimage/{id}")]
        [Authorize]
        public IActionResult DeleteLadderImage( int id ){
            var entity = context.UploadImage.Find(id);
 
            if(entity != null){
                
                

                var LadderImage = context.LadderImage.Where( i => i.UploadImageId == id).FirstOrDefault();
                if( LadderImage != null) context.LadderImage.Remove(LadderImage);
                var file = context.UploadFile.Find(entity.UploadFileId);
                if(file != null ) context.UploadFile.Remove(file);
                
                context.UploadImage.Remove(entity);

                context.SaveChanges();
                
                this.Log(entity,"LadderImage", "LadderImage Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"LadderImage", "Not Found LadderImage in a delete attempt.", "LadderImage", "Error");
                return new StatusCodeResult(500);
            } 
        }


        [HttpPost("UploadFiles/{userId}")]
        public ActionResult Post(int userId, List<IFormFile> files)
        {

            var result = new FileUploadResult();
            // Get the file from the POST request
            var theFile = HttpContext.Request.Form.Files.GetFile("file");

            // Get the server path, wwwroot
            string webRootPath = _hostingEnvironment.WebRootPath;

            // Building the path to the uploads directory
            var fileRoute = Path.Combine(webRootPath, "uploads");

            // Get the mime type
            var mimeType = HttpContext.Request.Form.Files.GetFile("file").ContentType;

            // Get File Extension
            string extension = System.IO.Path.GetExtension(theFile.FileName);

            // Generate Random name.
            string name = Guid.NewGuid().ToString().Substring(0, 8) + extension;

            // Build the full path inclunding the file name
            string link = Path.Combine(fileRoute, name);

            // Create directory if it does not exist.
            FileInfo dir = new FileInfo(fileRoute);
            dir.Directory.Create();

            // Basic validation on mime types and file extension
            string[] imageMimetypes = { "image/jpeg", "image/pjpeg", "image/x-png", "image/png", "image/svg+xml" };
            string[] imageExt = { ".jpeg", ".jpg", ".png" };

           
            if (
                    Array.IndexOf(imageMimetypes, mimeType) >= 0 
                    && 
                    Array.IndexOf(imageExt, extension) >= 0
            ){
                Stream stream;

                stream = new MemoryStream();
                theFile.CopyTo(stream);



                var imService = new ImageProcessingService();

                ImageProcessingService.ResizeParams resizeParams = new ImageProcessingService.ResizeParams();
                resizeParams.h = 1200;
                resizeParams.w = 1200;
                resizeParams.hasParams = true;
                resizeParams.autorotate = true;
                resizeParams.quality = 100;
                resizeParams.format = theFile.ContentType == "image/png"? "png":"jpg";
                resizeParams.mode = "max";

                stream.Flush(); 
                stream.Position = 0;

                var imge = imService.Restrict(stream,resizeParams,new DateTime());

                UploadFile upResult = SaveFileToDatebase(imge.AsStream(), name, theFile.ContentType, theFile.Length, userId );
                UploadImage im = new UploadImage();
                im.UploadFile = upResult;
                im.Name = upResult.Name;
                context.Add(im);
                context.SaveChanges();
                result.Success = true;
                result.Message = "Yor Document was Successfully Uploaded.";
                result.FileId = upResult.Id;
                result.ImageId = im.Id;
                result.FileName = im.Name;
            }else{
                result.Success = false;
                result.Message = "Not Supported File Format. Accepted are Only JPG or PNG images.";
            }
            return new OkObjectResult(result);
         
        }


        public class FileUploadResult{
            public Boolean Success;
            public string Message;
            public int FileId;
            public int ImageId;
            public string FileName;
        }


        public UploadFile SaveFileToDatebase(Stream fileStream, String name, String type, long size, int userId)
        {
            
            var upFile = new UploadFile();
            upFile.Created = DateTime.Now;
            upFile.Name = name;
            upFile.Updated = DateTime.Now;
            upFile.Type = type;
            upFile.Size = Int32.Parse(size.ToString());
            upFile.ById = userId;
            byte[] content = ReadFully(fileStream);


            upFile.Content = content;
            this._context.Add(upFile);
            this._context.SaveChanges();
            return upFile;

        }

        // Convert Stream to Byte array
        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }








       
        public IActionResult Error()
        {
            return View();
        }


    }
    
}
