using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Dtos
{
    public class PreferenceDto
    {
        public bool NotificationSound { get; set; }

        public bool DarkTheme { get; set; }

        public bool ActiveState { get; set; }
    }
}