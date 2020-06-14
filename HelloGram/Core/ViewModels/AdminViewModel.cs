using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using HelloGram.Persistence.Repositories;

namespace HelloGram.Core.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {

        public StatusRepository.Status UsersStatus { get; set; }

        public StatusRepository.Status NewUsersStatus { get; set; }

        public StatusRepository.Status ActiveUsersStatus { get; set; }

        public StatusRepository.Status Under18Status { get; set; }

        public StatusRepository.Status ViewsStatus { get; set; }

        public StatusRepository.Status SubscriptionStatus { get; set; }

        public StatusRepository.Status PostsStatus { get; set; }

        public StatusRepository.Status ReactionsStatus { get; set; }

        public IEnumerable<UserViewModel> UserViewModels { get; set; }

        public AdminViewModel(ApplicationUser user, string query = null) : base(NavIndices.Nothing, user)
        {
            var repo = new Repository(true);

            UsersStatus = repo.StatusRepository.GetUsersStatus();
            NewUsersStatus = repo.StatusRepository.GetNewUsersStatus();
            ActiveUsersStatus = repo.StatusRepository.GetActiveUsersStatus();
            Under18Status = repo.StatusRepository.GetUnder18Status();
            ViewsStatus = repo.StatusRepository.GetViewsStatus();
            PostsStatus = repo.StatusRepository.GetPostsStatus();
            SubscriptionStatus = repo.StatusRepository.GetSubscriptionsStatus();
            ReactionsStatus = repo.StatusRepository.GetReactionsStatus();

            UserViewModels = repo.ApplicationUserRepository.GetAllUsers(100, query)
                .Select(u => UserViewModel.Create(u, user.Id, true, true));
        }
    }
}