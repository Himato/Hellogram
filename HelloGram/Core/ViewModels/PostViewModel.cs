using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using Microsoft.Ajax.Utilities;

namespace HelloGram.Core.ViewModels
{
    public class PostViewModel
    {
        public string Id { get; set; }

        public string UserImage { get; set; }

        public string AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string AuthorUsername { get; set; }

        public string AuthorImage { get; set; }

        public string Text { get; set; }

        public string Image { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public bool IsLogged { get; set; }

        public bool IsTrash { get; set; }

        public bool IsCategory { get; set; }

        public bool IsSaved { get; set; }

        public bool IsFollowing { get; set; }

        public bool IsReact { get; set; }

        public bool IsLike { get; set; }

        public bool IsSubscribed { get; set; }

        public string NumberOfLikes { get; set; }

        public string NumberOfDisLikes { get; set; }

        public string NumberOfComments { get; set; }

        public DateTime Date { get; set; }

        public CommentViewModel CommentViewModel { get; set; }

        public static PostViewModel Create(Post post, ApplicationUser user, bool isCategory = false)
        {
            var repos = new Repository();
            var logged = user.Id.Equals(post.ApplicationUserId);
            var reacted = repos.PostRepository.IsReacted(post.Id, user.Id);
            var likes = repos.PostRepository.GetNumberOfLikes(post.Id);
            var dislikes = repos.PostRepository.GetNumberOfDisLikes(post.Id);
            var comments = repos.PostRepository.GetNumberOfComments(post.Id);
            return new PostViewModel
            {
                IsLogged = logged,
                IsTrash = post.IsDeleted,
                IsCategory = isCategory,
                IsSaved = repos.PostRepository.IsSaved(post.Id, user.Id),
                IsFollowing = !logged && repos.ApplicationUserRepository.IsFollowing(user.Id, post.ApplicationUserId),
                Id = post.Id + "",
                UserImage = user.GetImage(),
                AuthorId = post.ApplicationUser?.Id,
                AuthorName = post.ApplicationUser?.Name,
                AuthorUsername = post.ApplicationUser?.UserName,
                AuthorImage = post.ApplicationUser.GetImage(),
                Text = post.Text,
                Image = post.Image,
                Date = post.Date,
                CategoryId = post.CategoryId,
                CategoryName = post.Category?.Name,
                IsReact = reacted,
                IsLike = reacted && repos.PostRepository.GetReact(post.Id, user.Id),
                IsSubscribed = logged || repos.PostRepository.IsSubscribed(post.Id, user.Id),
                NumberOfLikes = (likes >= 1000) ? $"{(likes / 1000.00):F1}K" : $"{likes}",
                NumberOfDisLikes = (dislikes >= 1000) ? $"{(dislikes / 1000.00):F1}K" : $"{dislikes}",
                CommentViewModel = CommentViewModel.Create(repos.PostRepository.GetComments(post.Id).FirstOrDefault(), user.Id),
                NumberOfComments = (comments >= 1000) ? $"{(comments / 1000.00):F1}K" : (comments == 0) ? "No" : $"{comments}"
            };
        }

        public string GetFormattedDate()
        {
            return Date.Humanize(DateTime.UtcNow);
        }
    }
}