using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class NotificationsViewModel : BaseViewModel
    {
        public List<NotificationViewModel> NotificationViewModels { get; set; }

        public NotificationsViewModel(ApplicationUser user) : base(NavIndices.Notifications, user)
        {
            var repos = new Repository();

            NotificationViewModels = new List<NotificationViewModel>();

            var notifications = repos.ApplicationUserRepository.GetNotifications(user.Id).ToList();

            {
                var likes = notifications.Where(c => c.NotificationType == NotificationType.Like).GroupBy(n => n.PostId).Select(n => n.ToList()).ToList();
                foreach (var n in likes.Select(like => like.GroupBy(n => n.IsRead).Select(n => n.ToList()).ToList()).SelectMany(temp => temp))
                {
                    NotificationViewModels.Add(NotificationViewModel.Create(n.GroupBy(x => x.SenderId).Select(z => z.FirstOrDefault()).ToList()));
                }
            }

            {
                var dislikes = notifications.Where(c => c.NotificationType == NotificationType.Dislike).GroupBy(n => n.PostId).Select(n => n.ToList()).ToList();
                foreach (var n in dislikes.Select(dislike => dislike.GroupBy(n => n.IsRead).Select(n => n.ToList()).ToList()).SelectMany(temp => temp))
                {
                    NotificationViewModels.Add(NotificationViewModel.Create(n.GroupBy(x => x.SenderId).Select(z => z.FirstOrDefault()).ToList()));
                }
            }

            {
                var comments = notifications.Where(c => c.NotificationType == NotificationType.Comment).GroupBy(n => n.PostId).Select(n => n.ToList()).ToList();
                foreach (var n in comments.Select(comment => comment.GroupBy(n => n.IsRead).Select(n => n.ToList()).ToList()).SelectMany(temp => temp))
                {
                    NotificationViewModels.Add(NotificationViewModel.Create(n.GroupBy(x => x.SenderId).Select(z => z.FirstOrDefault()).ToList()));
                }
            }

            {
                var followings = notifications.Where(c => c.NotificationType == NotificationType.Follow).GroupBy(n => n.PostId).Select(n => n.ToList()).ToList();
                foreach (var n in followings.Select(following => following.GroupBy(n => n.IsRead).Select(n => n.ToList()).ToList()).SelectMany(temp => temp))
                {
                    NotificationViewModels.Add(NotificationViewModel.Create(n.GroupBy(x => x.SenderId).Select(z => z.FirstOrDefault()).ToList()));
                }
            }

            NotificationViewModels = NotificationViewModels.OrderByDescending(n => n.Date).ToList();
        }
    }
}