using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class TrendingViewModel : BaseViewModel
    {
        public List<PostViewModel> PostViewModels { get; set; }
        
        public TrendingViewModel(ApplicationUser user) : base(NavIndices.Trending, user)
        {
            var repos = new Repository();

            var posts = repos.PostRepository.GetTrendingPosts();
            PostViewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                PostViewModels.Add(PostViewModel.Create(post, user));
            }
        }
    }
}