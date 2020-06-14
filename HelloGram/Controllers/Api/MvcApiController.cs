using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using HelloGram.Core.Dtos;
using HelloGram.Core.Models;
using HelloGram.Core.ViewModels;
using HelloGram.Hubs;
using HelloGram.Persistence;
using HelloGram.Persistence.Exceptions;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers.Api
{
    [Authorize]
    public class MvcApiController : Controller
    {
        private readonly Repository _repository;

        public MvcApiController()
        {
            _repository = new Repository();
        }

        [HttpGet]
        [Route("~/mvc/posts/home")]
        public ActionResult GetHomePosts(string loaded)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            var loadedPosts = new List<Post>();
            try
            {
                var ids = loaded.Split('_');
                foreach (var id in ids)
                {
                    try
                    {
                        var temp = Convert.ToInt32(id);
                        loadedPosts.Add(_repository.PostRepository.GetPost(temp));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignored
            }

            var posts = _repository.PostRepository.GetHomePosts(user.Id).Except(loadedPosts).Take(10);
            var list = posts.Select(post => PostViewModel.Create(post, user)).ToList();

            return PartialView("_PostList", list);
        }

        [HttpGet]
        [Route("~/mvc/posts/comments")]
        public ActionResult GetComments(int postId)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            var comments = _repository.PostRepository.GetComments(postId);
            var list = comments.Select(c => CommentViewModel.Create(c, user.Id)).ToList();

            return PartialView("_CommentsList", list);
        }

        [HttpGet]
        [Route("~/mvc/posts/comment")]
        public ActionResult GetComment(int commentId)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Comment", CommentViewModel.Create(_repository.PostRepository.GetComment(commentId), user.Id));
        }

        [HttpPost]
        [Route("~/mvc/posts/comments")]
        public ActionResult AddComment(CommentDto commentDto)
        {
            var list = new List<CommentViewModel>();
            try
            {
                var userId = User.Identity.GetUserId();
                var comment = _repository.PostRepository.AddComment(commentDto.PostId, userId, commentDto.Text);
                list.Add(CommentViewModel.Create(comment, userId));
                _repository.Complete();
                list[0].Id = comment.Id;
                CommentsHub.Notify(commentDto.PostId, comment.Id);
            }
            catch (DbEntityValidationException exception)
            {
                list[0].Error = DbEntityValidationExceptionHandler.GetExceptionMessage(exception);
                return PartialView("_CommentsList", list);
            }
            catch (Exception exception)
            {
                list[0].Error = exception.Message;
                return PartialView("_CommentsList", list);
            }

            return PartialView("_CommentsList", list);
        }

        [HttpGet]
        [Route("~/mvc/messenger")]
        public ActionResult GetMessage(string messageIds)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            var ids = messageIds.Split('#');

            var model = new List<MessageViewModel>();

            foreach (var id in ids)
            {
                try
                {
                    var message = _repository.MessengerRepository.GetMessage(user.Id, Convert.ToInt32(id));

                    if (message == null)
                    {
                        continue;
                    }

                    model.Add(MessageViewModel.Create(user.Id, message));
                }
                catch
                {
                    // ignored
                }
            }

            if (model.Any())
            {
                return PartialView("~/Views/Messenger/Message.cshtml", model);
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Route("~/mvc/messenger/files")]
        public ActionResult GetFiles(string messageIds)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            var ids = messageIds.Split('#');

            var model = new List<FileViewModel>();

            foreach (var id in ids)
            {
                try
                {
                    var message = _repository.MessengerRepository.GetMessage(user.Id, Convert.ToInt32(id));

                    if (message == null)
                    {
                        continue;
                    }

                    model.Add(FileViewModel.Create(message));
                }
                catch
                {
                    // ignored
                }
            }

            if (model.Any())
            {
                return PartialView("~/Views/Messenger/File.cshtml", model);
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Route("~/mvc/messenger/members")]
        public ActionResult GetMember(string username)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            var sender = _repository.ApplicationUserRepository.GetUserByUsername(username);

            if (sender == null)
            {
                return HttpNotFound();
            }

            return PartialView("~/Views/Messenger/Member.cshtml", MemberViewModel.Create(MemberViewModel.Tabs.Chats, user.Id, sender));
        }

        [HttpGet]
        [Route("~/mvc/messenger/search")]
        public ActionResult GetMembers(string q)
        {
            var userId = User.Identity.GetUserId();
            if (q.IsNullOrWhiteSpace())
            {
                return PartialView("~/Views/Messenger/PeopleList.cshtml", _repository.ApplicationUserRepository.GetFollowings(userId)
                    .Select(x => MemberViewModel.Create(MemberViewModel.Tabs.People, userId, x)));
            }

            return PartialView("~/Views/Messenger/PeopleList.cshtml", _repository.ApplicationUserRepository.GetUsersByName(userId, q)
                .Select(x => MemberViewModel.Create(MemberViewModel.Tabs.People, userId, x)));
        }
    }
}