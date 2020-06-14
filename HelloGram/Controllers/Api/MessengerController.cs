using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HelloGram.Core.Dtos;
using HelloGram.Hubs;
using HelloGram.Persistence;
using HelloGram.Persistence.Exceptions;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers.Api
{
    [Authorize]
    public class MessengerController : ApiHelper
    {
        [HttpPut]
        [Route("~/api/messenger/preferences")]
        public IHttpActionResult UpdatePreferences(PreferenceDto preferenceDto)
        {
            return Put(userId => Repository.MessengerRepository.UpdatePreference(userId, preferenceDto));
        }

        [HttpPut]
        [Route("~/api/messenger/mark_read")]
        public IHttpActionResult MarkAsSeen(string username)
        {
            var clientId = Repository.ApplicationUserRepository.GetUserByUsername(username).Id;
            var list = new List<string>();
            var output = Put(userId => { list = Repository.MessengerRepository.MarkAsRead(userId, clientId); });

            MessengerHub.ChangeSeen(clientId, list.ToArray());

            return output;
        }

        [HttpPost]
        [Route("~/api/messenger/messages")]
        public IHttpActionResult UploadMessage(string username, MessageDto messageDto)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var output = Repository.MessengerRepository.UploadMessage(userId,
                    Repository.ApplicationUserRepository.GetUserByUsername(username).Id, messageDto);
                Repository.Complete();

                Repository.MessengerRepository.Send(userId, username, output.Id + "");

                return Ok(output.Id);
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
            catch (DbEntityValidationException exception)
            {
                return BadRequest(DbEntityValidationExceptionHandler.GetExceptionMessage(exception));
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost]
        [Route("~/api/messenger/files")]
        public Task<HttpResponseMessage> UploadAttachment(string username)
        {
            // Check if the request contains multipart/form-data
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //Get the path of folder where we want to upload all files.
            var rootPath = HttpContext.Current.Server.MapPath("~/Content/Messenger/Attachments");
            var provider = new MultipartFileStreamProvider(rootPath);

            // Read the form data.
            //If any error(Cancelled or any fault) occurred during file read, return internal server error
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith(t =>
                {
                    var ids = new List<string>();
                    foreach (var fileData in provider.FileData)
                    {
                        try
                        {
                            var info = new FileInfo(fileData.LocalFileName);

                            if (info.Length > 25 * 1024 * 1024)
                            {
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File is too large. The maximum size is 25 MB.");
                            }

                            //Replace / from file name
                            var name = fileData.Headers.ContentDisposition.FileName.Replace("\"", "");

                            //Create New file name using GUID to prevent duplicate file name
                            var newFileName = Guid.NewGuid() + Path.GetExtension(name);
                            //Move file from current location to target folder.
                            var path = Path.Combine(rootPath, newFileName);
                            File.Move(fileData.LocalFileName, path);
                            var image = true;
                            try
                            {
                                var imgInput = System.Drawing.Image.FromFile(path);
                                var gInput = Graphics.FromImage(imgInput);
                                var thisFormat = imgInput.RawFormat;
                            }
                            catch
                            {
                                image = false;
                            }

                            var size = info.Length > 1024 * 1024
                                ? $"{info.Length / 1024.00 / 1024.00:F1} MB"
                                : $"{info.Length / 1024.00:F2} KB";

                            var message = Repository.MessengerRepository.UploadAttachment(User.Identity.GetUserId(),
                                Repository.ApplicationUserRepository.GetUserByUsername(username).Id, name, newFileName, size, image);
                            Repository.Complete();

                            ids.Add(message.Id + "");
                        }
                        catch
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to load your image.");
                        }
                    }

                    var output = ids.Aggregate("", (current, id) => current + id + "#");
                    Repository.MessengerRepository.Send(User.Identity.GetUserId(), username, output);
                    return Request.CreateResponse(HttpStatusCode.Created, output);
                });
            return task;
        }
    }
}
