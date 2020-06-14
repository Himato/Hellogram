using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using Microsoft.Ajax.Utilities;

namespace HelloGram.Core.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string UserImage { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public bool Has { get; set; }

        public bool IsOwner { get; set; }

        public string Error { get; set; }

        public static CommentViewModel Create(Comment comment, string userId)
        {
            if (comment == null)
            {
                return null;
            }

            return new CommentViewModel
            {
                Id = comment.Id,
                Username = comment.ApplicationUser?.UserName,
                Name = comment.ApplicationUser?.Name,
                UserImage = comment.ApplicationUser.GetImage(),
                Text = comment.Text,
                Date = comment.DateTime,
                Has = comment.ApplicationUserId.Equals(userId) || comment.Post.ApplicationUserId.Equals(userId),
                IsOwner = comment.ApplicationUserId.Equals(userId)
            };
        }

        public string GetFormattedDate()
        {
            return Date.Humanize(DateTime.UtcNow);
        }
    }
}