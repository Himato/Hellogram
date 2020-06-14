using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;

namespace HelloGram.Core.ViewModels
{
    public class SavedPostsViewModel : BaseViewModel
    {
        public List<PostViewModel> PostViewModels { get; set; }

        public SavedPostsViewModel(ApplicationUser user, IEnumerable<Post> posts) : base(NavIndices.SavedPosts, user)
        {
            PostViewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                PostViewModels.Add(PostViewModel.Create(post, user));
            }
        }
    }
}