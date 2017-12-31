using System;
using Microsoft.AspNetCore.Mvc;
using Kers.Models.Contexts;
using System.Security.Claims;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Kers.Models.Abstract;

namespace Kers.Controllers
{
    public class FroalaApiController : Controller
    {

        KERScoreContext _context;
        KERSmainContext _mainContext;
        IKersUserRepository _userRepo;
    

        public FroalaApiController(
            KERScoreContext _context,
            KERSmainContext _mainContext,
            IKersUserRepository _userRepo
        ){
            this._context = _context;
            this._mainContext = _mainContext;
            this._userRepo = _userRepo;
        }


        public IActionResult UploadImage()
        {

            //To do ----------
            // Use /Services/ImpageProcessingService.cs to rotate image if needed
            
            try
            {  
                var hContext = HttpContext;
                var profile = hContext.Request.Form["profileId"];
                var user = _context.KersUser.Find(Int32.Parse(profile));
                var im = Kers.Services.FroalaWisiwyg.DatabaseImage.Upload(HttpContext, _context, user);
                return Json(new { link = "fileuploads/" + im.UploadFile.Name, imageId = im.Id } );
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadFile () {
            
            object response;
            try
            {  
                var hContext = HttpContext;
                var profile = hContext.Request.Form["profileId"];
                var user = _userRepo.findByProfileID(Int32.Parse(profile));
                response = Kers.Services.FroalaWisiwyg.DatabaseFile.Upload(HttpContext, _context, user);
                return Json(response);
            }
            catch (Exception e)
            {
               return Json(e);
            }
        }

        public IActionResult LoadImages()
        {
            string uploadPath = "wwwroot/uploads/";

            try
            {  
                return Json(Kers.Services.FroalaWisiwyg.DatabaseImage.List(uploadPath));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadImageResize()
        {
            //MagickGeometry resizeGeometry = new MagickGeometry(300, 300);
            //resizeGeometry.IgnoreAspectRatio = true;

            Kers.Services.FroalaWisiwyg.ImageOptions options = new Kers.Services.FroalaWisiwyg.ImageOptions
            {
                //ResizeGeometry = resizeGeometry
            };

            try
            {
                var hContext = HttpContext;
                var profile = hContext.Request.Form["profileId"];
                var user = _userRepo.findByProfileID(Int32.Parse(profile));

                return Json(Kers.Services.FroalaWisiwyg.DatabaseImage.Upload(hContext, _context, user, options));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadImageValidation ()
        {
            
            Func<string, string, bool> validationFunction = (filePath, mimeType) => {
/*
                MagickImageInfo info = new MagickImageInfo(filePath);

                if (info.Width != info.Height)
                {
                    return false;
                }
 */
                return true;
            };

            Kers.Services.FroalaWisiwyg.ImageOptions options = new Kers.Services.FroalaWisiwyg.ImageOptions
            {
                Fieldname = "myImage",
                Validation = new Kers.Services.FroalaWisiwyg.ImageValidation(validationFunction)
            };

            try
            {
                var hContext = HttpContext;
                var profile = hContext.Request.Form["profileId"];
                var user = _userRepo.findByProfileID(Int32.Parse(profile));
                return Json(Kers.Services.FroalaWisiwyg.DatabaseImage.Upload(HttpContext, _context, user, options));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadFileValidation ()
        {
            
            Func<string, string, bool> validationFunction = (filePath, mimeType) => {

                long size = new System.IO.FileInfo(filePath).Length;
                if (size > 10 * 1024 * 1024)
                {
                    return false;
                }

                return true;
            };

            Kers.Services.FroalaWisiwyg.FileOptions options = new Kers.Services.FroalaWisiwyg.FileOptions
            {
                Fieldname = "myFile",
                Validation = new Kers.Services.FroalaWisiwyg.FileValidation(validationFunction)
            };

            try
            {
                var hContext = HttpContext;
                var profile = hContext.Request.Form["profileId"];
                var user = _userRepo.findByProfileID(Int32.Parse(profile));
                return Json(Kers.Services.FroalaWisiwyg.DatabaseImage.Upload(HttpContext, _context, user, options));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult DeleteFile ()
        {
            try
            {
                Kers.Services.FroalaWisiwyg.DatabaseFile.Delete(HttpContext.Request.Form["src"]);
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult DeleteImage()
        {
            try
            {
                Kers.Services.FroalaWisiwyg.DatabaseImage.Delete(HttpContext.Request.Form["src"]);
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }
/*
        private KersUser userByLinkBlueId(string linkBlueId){
            var profile = _mainContext.zEmpRptProfiles.
                            Where(p=> p.linkBlueID == linkBlueId).
                            FirstOrDefault();
            KersUser user = null;
            if(profile != null){
                user = _userRepo.findByProfileID(profile.Id);
                if(user == null){
                    user = _userRepo.createUserFromProfile(profile);
                }
            }
            return user;
        }


        private KersUser CurrentUser(){
            var u = this.CurrentUserId();
            return this.userByLinkBlueId(u);
        }


        private string CurrentUserId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

 */
        public IActionResult Error()
        {
            return View();
        }
    }
}
