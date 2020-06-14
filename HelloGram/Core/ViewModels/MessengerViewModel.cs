using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using Microsoft.VisualBasic.ApplicationServices;

namespace HelloGram.Core.ViewModels
{
    public class MessengerViewModel
    {
        public UserViewModel UserViewModel { get; set; }

        public List<MemberViewModel> ChatsMembers { get; set; }

        public List<MemberViewModel> PeopleMembers { get; set; }

        public bool HasChats { get; set; }

        public MessengerSettingsViewModel SettingsViewModel { get; set; }

        public MessengerViewModel(ApplicationUser user)
        {
            UserViewModel = UserViewModel.Create(user, user.Id);

            var repos = new Repository();

            var followings = repos.ApplicationUserRepository.GetFollowings(user.Id).ToList();

            var chatUsers = repos.MessengerRepository.GetChatUsers(user.Id).ToList();

            PeopleMembers = followings.Select(f => MemberViewModel.Create(MemberViewModel.Tabs.People, user.Id, f)).ToList();

            ChatsMembers = chatUsers.Select(c => MemberViewModel.Create(MemberViewModel.Tabs.Chats, user.Id, c)).OrderByDescending(m => m.LastMessage.SentDateTime).ToList();

            SettingsViewModel = MessengerSettingsViewModel.Create(user);

            HasChats = ChatsMembers.Any(c => c.NumberOfUnReadMessages > 0);
        }
    }
}