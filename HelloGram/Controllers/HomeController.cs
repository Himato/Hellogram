using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HelloGram.Core;
using HelloGram.Core.Dtos;
using HelloGram.Core.Models;
using HelloGram.Core.ViewModels;
using HelloGram.Hubs;
using HelloGram.Persistence;
using HelloGram.Persistence.Exceptions;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers
{
    public class HomeController : Controller
    {
        private readonly Repository _repository;

        public HomeController()
        {
            _repository = new Repository();
        }

        [Authorize]
        public ActionResult Index()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new HomeViewModel(user);

            if (!model.PostViewModels.Any())
            {
                return RedirectToAction("Index", "Categories");
            }

            return View(model);
        }

        [Route("~/About")]
        public ActionResult About()
        {
            try
            {
                var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());
                ViewBag.Name = user.Name;
            }
            catch
            {
                // ignored
            }

            return View();
        }

        [Route("~/Policy")]
        public ActionResult Policy()
        {
            try
            {
                var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());
                ViewBag.Name = user.Name;
            }
            catch
            {
                // ignored
            }

            return View();
        }

        [Route("~/Terms")]
        public ActionResult Terms()
        {
            try
            {
                var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());
                ViewBag.Name = user.Name;
            }
            catch
            {
                // ignored
            }

            return View();
        }

        [Authorize]
        [Route("~/Trending")]
        public ActionResult Trending()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(new TrendingViewModel(user));
        }

        [Authorize]
        [Route("~/Notifications")]
        public ActionResult Notifications()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(new NotificationsViewModel(user));
        }

        [Authorize]
        [Route("~/Search")]
        public ActionResult Search(string q)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View("~/Views/Users/Relationships.cshtml", new RelationshipViewModel(BaseViewModel.NavIndices.Nothing, user, q));
        }
    }
}