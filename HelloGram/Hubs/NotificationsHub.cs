using System;
using System.Threading.Tasks;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace HelloGram.Hubs
{
    public class NotificationsHub : Hub
    {
        [Authorize]
        public void Register()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            Groups.Add(Context.ConnectionId, userId);
        }

        public static void Notify(string userId, ApplicationUser sender, Notification notification)
        {
            string buffer;

            var ownership = notification.OwnershipType == OwnershipType.Owner ? "your" : "a";

            switch (notification.NotificationType)
            {
                case NotificationType.Like:
                    buffer = $"liked {ownership} post.";
                    break;
                case NotificationType.Dislike:
                    buffer = $"disliked {ownership} post.";
                    break;
                case NotificationType.Comment:
                    buffer = $"commented on {ownership} post.";
                    break;
                case NotificationType.Follow:
                    buffer = "followed you.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(notification.NotificationType), notification.NotificationType, null);
            }
            
            var message = $"<div class=\"row mx-0 mb--2\"><div class=\"col-auto p-0\"><span class=\"avatar avatar-sm rounded-circle\"><img src=\"/Content/Images/Users/{sender.GetImage()}\"></span></div><div class=\"col p-0 ml-2\"><strong>{sender.Name}</strong> <span class=\"ml-1\">{buffer}</span></div></div>";
            var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
            notificationHub.Clients.Group(userId).notify(message, new Repository().ApplicationUserRepository.GetNumberOfUnreadNotifications(userId) + 1);
        }
    }
}