using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kers.Models.Contexts;
using System.IO;

namespace Kers.Controllers
{
    [Route("[controller]")]
    public class FileUploadsController : Controller
    {


        KERScoreContext _context;

        public FileUploadsController(
            KERScoreContext _context
        ){
            this._context = _context;
        }

        [HttpGet("{filename}")]
        public IActionResult Index(string filename)
        {
            var file = _context.UploadFile.Where(u=>u.Name == filename).FirstOrDefault();
            if(file == null){
                return NotFound();
            }
            Stream stream = new MemoryStream(file.Content);
            return new FileStreamResult(stream, file.Type);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
