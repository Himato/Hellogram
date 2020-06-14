using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using Microsoft.Ajax.Utilities;

namespace HelloGram.Core.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }

        public string Image { get; set; }

        public string Name { get; set; }

        public NotificationType Type { get; set; }

        public string OwnershipType { get; set; }

        public string Post { get; set; }

        public DateTime Date { get; set; }

        public bool IsRead { get; set; }

        public static NotificationViewModel Create(List<Notification> notifications)
        {
            string name;
            if (notifications.Count > 3)
            {
                name = $"{notifications[0].Sender.Name}, {notifications[1].Sender.Name} and {notifications.Count - 2} others";
            }
            else if (notifications.Count >= 2)
            {
                name = notifications.Count == 2
                    ? $"{notifications[0].Sender.Name} and {notifications[1].Sender.Name}"
                    : $"{notifications[0].Sender.Name} and 2 others";
            }
            else if (notifications.Count == 1)
            {
                name = $"{notifications[0].Sender.Name}";
            }
            else
            {
                return null;
            }

            return new NotificationViewModel
            {
                Id = notifications[0].Id,
                Image = notifications[0].Sender.GetImage(),
                Name = name,
                Type = notifications[0].NotificationType,
                OwnershipType = notifications[0].OwnershipType == Models.OwnershipType.Owner ? "your" : "a",
                Post = notifications[0].Post != null ? $"/Users/{notifications[0].Post.ApplicationUser.UserName}/{notifications[0].Post.Id}" : "",
                Date = notifications[0].DateTime,
                IsRead = notifications[0].IsRead
            };
        }

        public string GetFormattedDate()
        {
            return Date.Humanize(DateTime.UtcNow);
        }
    }
}