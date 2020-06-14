using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelloGram.Core.ViewModels;
using HelloGram.Migrations;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly Repository _repository;

        public AdminController()
        {
            _repository = new Repository();
        }

        public ActionResult Index(string q = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return HttpNotFound();
            }

            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            var username = ConfigurationManager.AppSettings["admin-username"];
            var email = ConfigurationManager.AppSettings["admin-email"];

            if (!user.UserName.Equals(username) || !user.Email.Equals(email))
            {
                return HttpNotFound();
            }

            return View(new AdminViewModel(user, q));
        }
    }
}