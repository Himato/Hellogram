using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class MessengerSettingsViewModel
    {
        public bool NotificationSound { get; set; }

        public bool DarkTheme { get; set; }

        public bool ActiveState { get; set; }

        public static MessengerSettingsViewModel Create(ApplicationUser user)
        {
            var repos = new Repository();
            var pref = repos.MessengerRepository.GetPreference(user.Id);
            if (pref == null)
            {
                return new MessengerSettingsViewModel
                {
                    NotificationSound = true,
                    DarkTheme = false,
                    ActiveState = true,
                };
            }

            return new MessengerSettingsViewModel
            {
                NotificationSound = pref.NotificationSound,
                DarkTheme = pref.DarkTheme,
                ActiveState = pref.ActiveState
            };
        }
    }
}