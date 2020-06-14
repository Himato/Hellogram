using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Models
{
    public enum NotificationType
    {
        Like,
        Dislike,
        Comment,
        Follow
    }

    public enum OwnershipType
    {
        Owner,
        Other
    }
    public class Notification
    {
        public int Id { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public int? PostId { get; set; }

        public NotificationType NotificationType { get; set; }

        public OwnershipType OwnershipType { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsRead { get; set; }

        public ApplicationUser Sender { get; set; }

        public ApplicationUser Receiver { get; set; }

        public Post Post { get; set; }
    }
}