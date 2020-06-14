using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class BaseViewModel
    {
        public enum NavIndices
        {
            Home,
            Trending,
            Categories,
            Followers,
            Followings,
            SavedPosts,
            Notifications,
            Messages,
            Profile,
            Trash,
            Settings,
            Nothing
        }

        public int Index { get; set; }

        public UserViewModel UserViewModel { get; set; }

        public IEnumerable<string> Categories { get; set; }

        public BaseViewModel(NavIndices index, ApplicationUser user = null, bool loadProfileData = false)
        {
            var repos = new Repository();

            Index = (int) index;
            if (user != null)
            {
                UserViewModel = UserViewModel.Create(user, user.Id, loadProfileData);
            }
            Categories = repos.CategoryRepository.GetCategories();
        }
    }
}