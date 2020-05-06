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
        public LadderController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    IKersUserRepository userRepo,
                    IHostingEnvironment hostingEnvironment
            ):base(mainContext, context, userRepo){
                _context = context;
                _hostingEnvironment = hostingEnvironment;
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


        [HttpPost("addladder")]
        [Authorize]
        public IActionResult AddLadderApplication( [FromBody] LadderApplication LadderApplication){
            if(LadderApplication != null){
                
                this.context.Add(LadderApplication);
                this.context.SaveChanges();
                return new OkObjectResult(LadderApplication);
            }else{
                this.Log( LadderApplication,"LadderApplication", "Error in adding LadderApplication attempt.", "LadderApplication", "Error");
                return new StatusCodeResult(500);
            }
        }



        [HttpPut("updateladder/{id}")]
        [Authorize]
        public IActionResult UpdateLadderApplication( int id, [FromBody] LadderApplication LadderApplication){
           


            if(LadderApplication != null ){
                if(LadderApplication.KersUserId == 0){
                    var user = this.CurrentUser();
                    LadderApplication.KersUserId = user.Id;
                }
                
                this.Log(LadderApplication,"LadderApplication", "LadderApplication Updated.");
                
                return new OkObjectResult(LadderApplication);
            }else{
                this.Log( LadderApplication ,"LadderApplication", "Not Found LadderApplication in an update attempt.", "LadderApplication", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("deleteladder/{id}")]
        [Authorize]
        public IActionResult DeleteLadderApplication( int id ){
            //var entity = context.LadderApplication.Find(id);
            
            /* 
            if(entity != null){
                
                context.ExtensionEvent.Remove(entity);
                context.SaveChanges();
                
                this.Log(entity,"ExtensionEvent", "ExtensionEvent Removed.");

                return new OkResult();
            }else{
                this.Log( id ,"ExtensionEvent", "Not Found ExtensionEvent in a delete attempt.", "ExtensionEvent", "Error");
                return new StatusCodeResult(500);
            } */
            return new OkResult();
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
