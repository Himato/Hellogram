using System.Web.Mvc;
using HelloGram.Core.ViewModels;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers
{
    [Authorize]
    public class MessengerController : Controller
    {
        private readonly Repository _repository;

        public MessengerController()
        {
            _repository = new Repository();
        }

        // GET: Messenger
        public ActionResult Index()
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(new MessengerViewModel(user));
        }

        [Route("~/Messenger/{username}")]
        public ActionResult Chat(string username)
        {
            var user = _repository.ApplicationUserRepository.GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                return HttpNotFound();
            }

            var client = _repository.ApplicationUserRepository.GetUserByUsername(username);

            if (client == null || client.Id.Equals(user.Id))
            {
                return View("Index", new MessengerViewModel(user));
            }

            return View(new ChatViewModel(user, client));
        }
    }
}