using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Services;
using SkiaSharp;

namespace Kers.Controllers
{
    [Route("[controller]")]
    public class ImageController : Controller
    {
        KERScoreContext _context;

        public ImageController(
            KERScoreContext _context
        ){
            this._context = _context;
        }



        [HttpGet("{width}/{height}/{filename}")]
        public IActionResult ReSize(int width, int height, string filename)
        {
            var file = _context.
                            UploadImage.
                            Where(u=>u.UploadFile.Name == filename).
                            Include(u=>u.UploadFile).
                            FirstOrDefault();
            if(file == null || file.UploadFile == null){
                return NotFound();
            }
            Stream input = new MemoryStream(file.UploadFile.Content);

            const int quality = 75;


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
                        .Resize(new SKImageInfo(newWidth, newHeight), SKBitmapResizeMethod.Lanczos3))
                    {
                        using (var image = SKImage.FromBitmap(resized))
                        {
                            return new FileStreamResult(image.Encode(SKEncodedImageFormat.Jpeg, quality).AsStream(), file.UploadFile.Type);
                        }
                        
                    }

                }
            }           
        }

        [HttpGet("{size}/{filename}")]
        public IActionResult Size(int size, string filename)
        {
            var file = _context.
                            UploadImage.
                            Where(u=>u.UploadFile.Name == filename).
                            Include(u=>u.UploadFile).
                            FirstOrDefault();
            if(file == null || file.UploadFile == null){
                return NotFound();
            }
            Stream input = new MemoryStream(file.UploadFile.Content);


            
            const int quality = 75;
            int width, height;

            using (var inputStream = new SKManagedStream(input))
            {
                using (var original = SKBitmap.Decode(inputStream))
                {
                    if (original.Width > original.Height)
                    {
                        width = size;
                        height = original.Height * size / original.Width;
                    }
                    else
                    {
                        width = original.Width * size / original.Height;
                        height = size;
                    }

                    using (var resized = original
                        .Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3))
                    {
                        using (var image = SKImage.FromBitmap(resized))
                        {
                             return new FileStreamResult(image.Encode(SKEncodedImageFormat.Jpeg, quality).AsStream(), file.UploadFile.Type);
                        }
                        
                    }
                }
            }           
        }
        
        [HttpGet("{filename}")]
        public IActionResult Index(string filename)
        {
            var file = _context.
                            UploadImage.
                            Where(u=>u.UploadFile.Name == filename).
                            Include(u=>u.UploadFile).
                            FirstOrDefault();
            if(file == null || file.UploadFile == null){
                return NotFound();
            }
            Stream stream = new MemoryStream(file.UploadFile.Content);
            return new FileStreamResult(stream, file.UploadFile.Type);
        }



        [HttpGet]
        [Route("{mode}/{w}/{h}/{filename}", Name="ProcessImage")]
        public IActionResult Process(string mode, int w, int h, string filename)
        {
            var file = _context.
                            UploadImage.
                            Where(u=>u.UploadFile.Name == filename).
                            Include(u=>u.UploadFile).
                            FirstOrDefault();
            if(file == null || file.UploadFile == null){
                return NotFound();
            }
            Stream stream = new MemoryStream(file.UploadFile.Content);

            var srv = new ImageProcessingService();


            ImageProcessingService.ResizeParams resizeParams = new ImageProcessingService.ResizeParams();
            resizeParams.h = h;
            resizeParams.w = w;
            resizeParams.hasParams = true;
            resizeParams.autorotate = true;
            resizeParams.quality = 100;
            resizeParams.format = file.UploadFile.Type == "image/png"? "png":"jpg";
            resizeParams.mode = mode;

            var im = srv.GetImageData(stream,resizeParams,new DateTime());
            

            return new FileStreamResult(im.AsStream(), file.UploadFile.Type);
        }



        [HttpGet("id/{id}")]
        public IActionResult FileName(int id)
        {
            var im = _context.UploadImage.
                        Where(u=>u.Id == id).
                        Include(u=>u.UploadFile).
                        FirstOrDefault();
            if(im == null || im.UploadFile == null){
                return NotFound();
            }
            return new OkObjectResult( new {filename = im.UploadFile.Name} );
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
