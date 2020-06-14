using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public List<PostViewModel> PostViewModels { get; set; }

        public ProfileViewModel(ApplicationUser user, NavIndices index = NavIndices.Profile, bool isTrash = false) : base(index, user, true)
        {
            var posts = new Repository().PostRepository.GetPosts(user.Id, isTrash);

            PostViewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                PostViewModels.Add(PostViewModel.Create(post, user));
            }
        }
    }
}