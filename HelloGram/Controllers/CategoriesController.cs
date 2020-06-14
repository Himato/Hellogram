using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HelloGram.Core.ViewModels;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly Repository _repository;

        public CategoriesController()
        {
            _repository = new Repository();
        }

        // GET: Categories
        public ActionResult Index()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(new CategoriesViewModel(user));
        }

        [Route("~/Category/{id}")]
        public ActionResult Category(int id)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(new CategoryViewModel(user, id));
        }
    }
}