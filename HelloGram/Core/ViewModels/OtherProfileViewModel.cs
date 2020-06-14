using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class OtherProfileViewModel : BaseViewModel
    {
        public UserViewModel OtherUserViewModel { get; set; }

        public List<PostViewModel> PostViewModels { get; set; }

        public OtherProfileViewModel(ApplicationUser user, ApplicationUser otherUser) : base(NavIndices.Nothing, user)
        {
            OtherUserViewModel = UserViewModel.Create(otherUser, user.Id, true);

            var repos = new Repository();
            var posts = repos.PostRepository.GetPosts(otherUser.Id);

            PostViewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                post.ApplicationUser = otherUser;
                PostViewModels.Add(PostViewModel.Create(post, user));
            }
        }
    }
}