using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HelloGram.Core.Dtos;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers.Api
{
    [Authorize]
    public class PostsController : ApiHelper
    {
        [HttpPut]
        [Route("~/api/posts/subscribe")]
        public IHttpActionResult Subscribe(int id)
        {
            return Put(userId => Repository.PostRepository.Subscribe(id, userId));
        }

        [HttpPut]
        [Route("~/api/posts/saves")]
        public IHttpActionResult Save(int id)
        {
            return Put(userId => Repository.PostRepository.Save(id, userId));
        }

        [HttpPut]
        [Route("~/api/posts/react/{id}")]
        public IHttpActionResult React(int id, bool isLike)
        {
            return Put(userId => Repository.PostRepository.React(id, userId, isLike));
        }

        [HttpPut]
        [Route("~/api/posts/comments")]
        public IHttpActionResult EditComment(int id, string text)
        {
            return Put(userId => Repository.PostRepository.EditComment(id, userId, text));
        }

        [HttpDelete]
        [Route("~/api/posts/comments")]
        public IHttpActionResult DeleteComment(int id)
        {
            return Delete(userId => Repository.PostRepository.DeleteComment(id, userId));
        }

        [HttpGet]
        public IHttpActionResult GetPost(int id)
        {
            var post = Repository.PostRepository.GetPostApi(id);

            if (post == null || !post.ApplicationUserId.Equals(User.Identity.GetUserId()))
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpGet]
        public IHttpActionResult GetPosts(bool deleted = false)
        {
            return Ok(Repository.PostRepository.GetPosts(User.Identity.GetUserId(), deleted));
        }

        [HttpPut]
        public IHttpActionResult EditPost(int id, PostDto postDto)
        {
            return Put(userId => Repository.PostRepository.UpdatePost(id, userId, postDto));
        }

        [HttpPut]
        public IHttpActionResult UpdatePost(int id, bool deleted)
        {
            return Put(userId => Repository.PostRepository.UpdatePost(id, userId));
        }

        [HttpPost]
        public IHttpActionResult AddPost(PostDto postDto)
        {
            return Post(userId => Repository.PostRepository.AddPost(userId, postDto));
        }

        [HttpDelete]
        public IHttpActionResult DeletePost(int id)
        {
            return Delete(userId => Repository.PostRepository.DeletePost(id, userId));
        }

        [HttpPost]
        public Task<HttpResponseMessage> Upload(int id)
        {
            var repository = new Repository();

            // Check if the request contains multipart/form-data
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //Get the path of folder where we want to upload all files.
            var rootPath = HttpContext.Current.Server.MapPath("~/Content/Images/Posts");
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
                            //Replace / from file name
                            var file = fileData.Headers.ContentDisposition.FileName;

                            if (file == null)
                            {
                                var old = repository.PostRepository.AddImage(id, null);
                                repository.Complete();
                                try
                                {
                                    File.Delete(Path.Combine(rootPath, old));
                                }
                                catch
                                {
                                    // ignored
                                }

                                return Request.CreateResponse(HttpStatusCode.Created);
                            }

                            var name = file.Replace("\"", "");

                            //Create New file name using GUID to prevent duplicate file name
                            var newFileName = Guid.NewGuid() + Path.GetExtension(name);
                            //Move file from current location to target folder.
                            var path = Path.Combine(rootPath, newFileName);
                            File.Move(fileData.LocalFileName, path);

                            try
                            {
                                var imgInput = Image.FromFile(path);
                                var gInput = Graphics.FromImage(imgInput);
                                var thisFormat = imgInput.RawFormat;
                            }
                            catch
                            {
                                File.Delete(path);
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to load your image.");
                            }

                            var oldImage = repository.PostRepository.AddImage(id, newFileName);
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
