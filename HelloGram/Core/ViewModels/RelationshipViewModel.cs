using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class RelationshipViewModel : BaseViewModel
    {
        public string Title { get; set; }

        public List<UserViewModel> UserViewModels { get; set; }

        public RelationshipViewModel(NavIndices index, ApplicationUser user, string q = null) : base(index, user)
        {
            UserViewModels = new List<UserViewModel>();
            Title = index.ToString();

            var repos = new Repository();

            if (index == NavIndices.Followers)
            {
                var users = repos.ApplicationUserRepository.GetFollowers(user.Id);

                foreach (var applicationUser in users)
                {
                    UserViewModels.Add(UserViewModel.Create(applicationUser, user.Id, true));
                }
            }
            else if (index == NavIndices.Followings)
            {
                var users = repos.ApplicationUserRepository.GetFollowings(user.Id);

                foreach (var applicationUser in users)
                {
                    UserViewModels.Add(UserViewModel.Create(applicationUser, user.Id, true));
                }
            }
            else
            {
                Title = "People";
                var users = repos.ApplicationUserRepository.GetUsersByName(user.Id, q);

                foreach (var applicationUser in users)
                {
                    UserViewModels.Add(UserViewModel.Create(applicationUser, user.Id, true));
                }
            }
        }
    }
}