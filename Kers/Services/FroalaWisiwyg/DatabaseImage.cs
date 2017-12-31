using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Services;

namespace Kers.Services.FroalaWisiwyg
{
    /// <summary>
    /// Image functionality.
    /// </summary>
    public static class DatabaseImage
    {
        /// <summary>
        /// Content type string used in http multipart.
        /// </summary>

        public static ImageOptions defaultOptions = new ImageOptions();

        /// <summary>
        /// Uploads an image to disk.
        /// </summary>
        /// <param name="httpContext">The HttpContext object containing information about the request.</param>
        /// <param name="fileRoute">Server route where the file will be uploaded. This route must be public to be accesed by the editor.</param>
        /// <param name="options">File options.</param>
        /// <returns>Object with link.</returns>
        public static UploadImage Upload (HttpContext httpContext, KERScoreContext context, KersUser user, FileOptions options = null)
        {
            if (options == null)
            {
                options = defaultOptions;
            }


            var file = httpContext.Request.Form.Files.GetFile(options.Fieldname);
            if (file == null)
            {
                throw new Exception("Fieldname is not correct. It must be: " + options.Fieldname);
            }

            Stream stream;

            stream = new MemoryStream();
            file.CopyTo(stream);



            var imService = new ImageProcessingService();

            ImageProcessingService.ResizeParams resizeParams = new ImageProcessingService.ResizeParams();
            resizeParams.h = 1200;
            resizeParams.w = 1200;
            resizeParams.hasParams = true;
            resizeParams.autorotate = true;
            resizeParams.quality = 100;
            resizeParams.format = file.ContentType == "image/png"? "png":"jpg";
            resizeParams.mode = "max";

            stream.Flush(); 
            stream.Position = 0;
            

            var imge = imService.Restrict(stream,resizeParams,new DateTime());






            UploadFile upResult = DatabaseFile.UploadStream(imge.AsStream(), httpContext, context, user, options);
            
            UploadImage im = new UploadImage();
            im.UploadFile = upResult;
            context.Add(im);
            context.SaveChanges();



            return im;
        }


        /// <summary>
        /// Delete an image from disk.
        /// </summary>
        /// <param name="src">Server image path.</param>
        public static void Delete(string filePath)
        {
            DatabaseFile.Delete(filePath);
        }

        /// <summary>
        /// List images from disk.
        /// </summary>
        /// <param name="folderPath">Server folder path.</param>
        /// <param name="thumbPath">Optional. Server thumb path.</param>
        /// <returns></returns>
        public static List<object> List(string folderPath, string thumbPath = null)
        {
            // Use thumbPath as folderPath.
            if (thumbPath == null)
            {
                thumbPath = folderPath;
            }

            // Array of image objects to return.
            List<object> response = new List<object>();

            string absolutePath = DatabaseFile.GetAbsoluteServerPath(folderPath);

            string[] imageMimetypes = ImageValidation.AllowedImageMimetypesDefault;

            // Check if path exists.
            if (!System.IO.Directory.Exists(absolutePath))
            {
                throw new Exception("Images folder does not exist!");
            }

            string[] fileEntries = System.IO.Directory.GetFiles(absolutePath);

            // Add images.
            foreach (string filePath in fileEntries)
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                if (System.IO.File.Exists(filePath))
                {   
                    string mimeType;
                    new FileExtensionContentTypeProvider().TryGetContentType(filePath, out mimeType);
                    if (mimeType == null) {
                        mimeType = "application/octet-stream";
                    }
     

                    if (Array.IndexOf(imageMimetypes, mimeType) >= 0)
                    {
                        response.Add(new
                        {
                            url = folderPath.Replace("wwwroot/", "") + fileName,
                            thumb = thumbPath.Replace("wwwroot/", "") + fileName,
                            name = fileName
                        });
                    }
                }
            }
            return response;
        }
    }
}
