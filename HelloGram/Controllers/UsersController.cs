using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HelloGram.Core.ViewModels;
using HelloGram.Persistence;
using HelloGram.Persistence.Exceptions;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers
{
    [System.Web.Mvc.Authorize]
    public class UsersController : Controller
    {
        private readonly Repository _repository;

        public UsersController()
        {
            _repository = new Repository();
        }
        
        [Route("~/Profile")]
        public ActionResult UserProfile()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(new ProfileViewModel(user));
        }
        
        [Route("~/Users/{username}")]
        public ActionResult UsersProfile(string username)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            if (user.UserName.Equals(username))
            {
                return RedirectToAction("UserProfile");
            }

            var otherUser = _repository.ApplicationUserRepository.GetUserByUsername(username);

            if (otherUser == null)
            {
                return HttpNotFound();
            }

            return View("OtherProfile", new OtherProfileViewModel(user, otherUser));
        }
        
        [Route("~/Trash")]
        public ActionResult UserTrash()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View("UserProfile", new ProfileViewModel(user, BaseViewModel.NavIndices.Trash, true));
        }
        
        [Route("~/Users/{username}/{postId}")]
        public ActionResult UserPost(string username, int postId)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            var post = _repository.PostRepository.GetPost(postId);

            if (post == null || !post.ApplicationUser.UserName.Equals(username))
            {
                return HttpNotFound();
            }

            return View(new UserPostViewModel(user, post));
        }
        
        [Route("~/Saved")]
        public ActionResult SavedPosts()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(new SavedPostsViewModel(user, _repository.PostRepository.GetSavedPosts(user.Id)));
        }
        
        [Route("~/Followings")]
        public ActionResult Followings()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View("Relationships", new RelationshipViewModel(BaseViewModel.NavIndices.Followings, user));
        }

        [Route("~/Followers")]
        public ActionResult Followers()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View("Relationships", new RelationshipViewModel(BaseViewModel.NavIndices.Followers, user));
        }
    }
}