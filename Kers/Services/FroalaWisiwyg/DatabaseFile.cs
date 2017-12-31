using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;


namespace Kers.Services.FroalaWisiwyg
{
    /// <summary>
    /// File functionality.
    /// </summary>
    public static class DatabaseFile
    {
        /// <summary>
        /// Content type string used in http multipart.
        /// </summary>
        public static string MultipartContentType = "multipart/form-data";

        /// <summary>
        /// File default options.
        /// </summary>
        public static FileOptions defaultOptions = new FileOptions();

        /// <summary>
        /// Check http request content type.
        /// </summary>
        /// <returns>true if content type is multipart.</returns>
        public static bool CheckContentType(HttpContext httpContext)
        {       
            bool isMultipart = httpContext.Request.ContentType.StartsWith(MultipartContentType);

            return isMultipart;
        }

        /// <summary>
        /// Uploads a file to disk.
        /// </summary>
        /// <param name="httpContext">The HttpContext object containing information about the request.</param>
        /// <param name="fileRoute">Server route where the file will be uploaded. This route must be public to be accesed by the editor.</param>
        /// <param name="options">File options.</param>
        /// <returns>Object with link.</returns>
        public static UploadFile Upload(HttpContext httpContext, KERScoreContext context, KersUser user, FileOptions options = null)
        {
            // Use default file options.
            if (options == null)
            {
                options = defaultOptions;
            }

            if (!CheckContentType(httpContext))
            {
                throw new Exception("Invalid contentType. It must be " + MultipartContentType);
            }

            var httpRequest = httpContext.Request;

            int filesCount = 0;

            filesCount = httpRequest.Form.Files.Count;


            if (filesCount == 0)
            {
                throw new Exception("No file found");
            }

            // Get HTTP posted file based on the fieldname. 

            var file = httpRequest.Form.Files.GetFile(options.Fieldname);


            if (file == null)
            {
                throw new Exception("Fieldname is not correct. It must be: " + options.Fieldname);
            }

            // Generate Random name.
            string extension = Utils.GetFileExtension(file.FileName);
            string name = Utils.GenerateUniqueString() + "." + extension;
            Stream stream;

            stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;
            // Save file to disk.
            var upFile = Save(stream, context, name, file.ContentType, file.Length, user, options);


            // Make sure it is compatible with ASP.NET Core.
            return upFile ;
        }








        public static UploadFile UploadStream(Stream stream, HttpContext httpContext, KERScoreContext context, KersUser user, FileOptions options = null)
        {
            // Use default file options.
            if (options == null)
            {
                options = defaultOptions;
            }

            if (!CheckContentType(httpContext))
            {
                throw new Exception("Invalid contentType. It must be " + MultipartContentType);
            }

            var httpRequest = httpContext.Request;

            int filesCount = 0;

            filesCount = httpRequest.Form.Files.Count;


            if (filesCount == 0)
            {
                throw new Exception("No file found");
            }

            // Get HTTP posted file based on the fieldname. 

            var file = httpRequest.Form.Files.GetFile(options.Fieldname);


            if (file == null)
            {
                throw new Exception("Fieldname is not correct. It must be: " + options.Fieldname);
            }

            // Generate Random name.
            string extension = Utils.GetFileExtension(file.FileName);
            string name = Utils.GenerateUniqueString() + "." + extension;

            stream.Position = 0;
            // Save file to disk.
            var upFile = Save(stream, context, name, file.ContentType, file.Length, user, options);


            // Make sure it is compatible with ASP.NET Core.
            return upFile ;
        }










        /// <summary>
        /// Get absolute server path.
        /// </summary>
        /// <param name="path">Relative path.</param>
        /// <returns>Absolute path.</returns>
        public static String GetAbsoluteServerPath(string path) 
        {
            return path;

        }

        /// <summary>
        /// Save an input file stream to disk.
        /// </summary>
        /// <param name="fileStream">Input file stream</param>
        /// <param name="link">Server file path.</param>
        /// <param name="options">File options.</param>
        public static UploadFile Save(Stream fileStream, KERScoreContext context, String name, String type, long size, KersUser user, FileOptions options)
        {
            
            var upFile = new UploadFile();
            upFile.Created = DateTime.Now;
            upFile.Name = name;
            upFile.Updated = DateTime.Now;
            upFile.Type = type;
            upFile.Size = Int32.Parse(size.ToString());
            upFile.By = user;
            byte[] content = ReadFully(fileStream);


            upFile.Content = content;
            context.Add(upFile);
            context.SaveChanges();
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

        /// <summary>
        /// Delete a file from disk.
        /// </summary>
        /// <param name="src">Server file path.</param>
        public static void Delete(string filePath)
        {

            filePath = "wwwroot/" + filePath;

            // Will throw an exception if an error occurs.
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
