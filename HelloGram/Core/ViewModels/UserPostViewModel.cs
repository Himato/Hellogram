using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;

namespace HelloGram.Core.ViewModels
{
    public class UserPostViewModel : BaseViewModel
    {
        public PostViewModel PostViewModel { get; set; }

        public UserPostViewModel(ApplicationUser user, Post post) : base(NavIndices.Home, user)
        {
            PostViewModel = PostViewModel.Create(post, user);
        }
    }
}