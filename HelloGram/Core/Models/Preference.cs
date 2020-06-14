using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Models
{
    public class Preference
    {
        public int Id { get; set; }

        public bool NotificationSound { get; set; }

        public bool DarkTheme { get; set; }

        public bool ActiveState { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}