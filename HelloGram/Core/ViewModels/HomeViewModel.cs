using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public List<PostViewModel> PostViewModels { get; set; }

        public HomeViewModel(ApplicationUser user) : base(NavIndices.Home, user)
        {
            var repos = new Repository();

            var posts = repos.PostRepository.GetHomePosts(user.Id).Take(10);
            PostViewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                PostViewModels.Add(PostViewModel.Create(post, user));
            }
        }
    }
}