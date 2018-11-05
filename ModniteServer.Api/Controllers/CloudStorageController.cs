using Serilog;
using System.IO;
using System.Linq;

namespace ModniteServer.API.Controllers
{
    public sealed class CloudStorageController : Controller
    {
        /// <summary>
        /// Gets the list of system files to download from the cloud storage service.
        /// </summary>
        [Route("GET", "/fortnite/api/cloudstorage/system")]
        public void GetSystemFileList()
        {
            if (!Authorize()) { }

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write("[]");

            /* 
             * [
             *   {
             *     "uniqueFileName": "<some hashed filename>",
             *     "filename": "DefaultRuntimeOptions.ini",
             *     "hash": "<hash>"
             *     "hash256": "<hash>"
             *     "length": <bytes>
             *     "contentType": "application/octet-stream"
             *     "uploaded": "<ISO date>"
             *     "storageType": "S3"
             *     "doNotCache": bool
             * ]
             */
        }

        [Route("GET", "/fortnite/api/cloudstorage/user/*")]
        public void GetUserFileList()
        {
            // TODO: Implement cloud storage system
            if (!Authorize()) { }

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write("[]");
        }

        [Route("GET", "/fortnite/api/cloudstorage/user/*/*")]
        public void GetUserFile()
        {
            if (!Authorize()) { }

            string accountId = Request.Url.Segments.Reverse().Skip(1).Take(1).Single();
            string file = Request.Url.Segments.Last();

            if (file.ToLowerInvariant() == "clientsettings.sav")
            {
                // For now, we're giving the default settings. Eventually, we'll be storing user settings.
                Response.StatusCode = 200;
                Response.ContentType = "application/octet-stream";
                Response.Write(File.ReadAllBytes("Assets/ClientSettings.Sav"));
            }
            else
            {
                Response.StatusCode = 404;
            }

            Log.Information($"{accountId} downloaded {file} {{AccountId}}{{File}}", accountId, file);
        }

        // todo
        [Route("PUT", "/fortnite/api/cloudstorage/user/*/*")]
        public void SaveUserFile()
        {
            if (!Authorize()) { }

            string accountId = Request.Url.Segments.Reverse().Skip(1).Take(1).Single();
            string file = Request.Url.Segments.Last();

            Response.StatusCode = 204;
        }
    }
}