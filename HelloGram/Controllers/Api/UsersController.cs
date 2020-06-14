using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers.Api
{
    [Authorize]
    public class UsersController : ApiHelper
    {
        [HttpPut]
        [Route("~/api/followings")]
        public IHttpActionResult Follow(string id)
        {
            return Put(userId => Repository.ApplicationUserRepository.Follow(userId, id));
        }

        [HttpPut]
        [Route("~/api/subscriptions")]
        public IHttpActionResult Subscribe(int id)
        {
            return Put(userId => Repository.CategoryRepository.Subscribe(id, userId));
        }

        [HttpPut]
        [Route("~/api/mark_read")]
        public IHttpActionResult MarkNotificationAsRead(int? id)
        {
            return Put(userId => Repository.ApplicationUserRepository.MarkNotificationAsRead(userId, id));
        }

        [HttpPut]
        [Route("~/Users/Images")]
        public Task<HttpResponseMessage> Upload()
        {
            // Check if the request contains multipart/form-data
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            
            //Get the path of folder where we want to upload all files.
            var rootPath = HttpContext.Current.Server.MapPath("~/Content/Images/Users");
            var provider = new MultipartFileStreamProvider(rootPath);

            // Read the form data.
            //If any error(Cancelled or any fault) occurred during file read, return internal server error
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith(t =>
                {
                    foreach (var fileData in provider.FileData)
                    {
                        try
                        {
                            var info = new FileInfo(fileData.LocalFileName);

                            if (info.Length > 5 * 1024 * 1024)
                            {
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Image is too large. The maximum size is 5 MB.");
                            }

                            //Replace / from file name
                            var name = fileData.Headers.ContentDisposition.FileName.Replace("\"", "");

                            //Create New file name using GUID to prevent duplicate file name
                            var newFileName = Guid.NewGuid() + Path.GetExtension(name);
                            //Move file from current location to target folder.
                            var path = Path.Combine(rootPath, newFileName);
                            File.Move(fileData.LocalFileName, path);

                            try
                            {
                                var imgInput = System.Drawing.Image.FromFile(path);
                                var gInput = Graphics.FromImage(imgInput);
                                var thisFormat = imgInput.RawFormat;
                            }
                            catch
                            {
                                File.Delete(path);
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to load your image.");
                            }

                            var repository = new Repository();
                            var oldImage = repository.ApplicationUserRepository.UploadImage(User.Identity.GetUserId(), newFileName);
                            repository.Complete();
                            try
                            {
                                File.Delete(Path.Combine(rootPath, oldImage));
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        catch
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to load your image.");
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.Created);
                });
            return task;
        }
    }
}
