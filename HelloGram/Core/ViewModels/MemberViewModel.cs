using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class MemberViewModel
    {
        public enum Tabs
        {
            People,
            Chats
        }

        public string Username { get; set; }

        public string Image { get; set; }

        public string Name { get; set; }

        public bool IsOnline { get; set; }

        public string LastLogin { get; set; }

        public MessageViewModel LastMessage { get; set; }

        public int NumberOfUnReadMessages { get; set; }

        public Tabs Type { get; set; }

        public static MemberViewModel Create(Tabs tab, string userId, ApplicationUser client)
        {
            var repos = new Repository();
            
            if (tab == Tabs.Chats)
            {
                var lastMessage = repos.MessengerRepository.GetMessages(userId, client.Id).Last();
                var str = MessageViewModel.Create(userId, lastMessage);
                str.Content = (str.IsMine ? "You: " : "") + (str.IsFile ? "Sent an attachment" : str.Content);

                return new MemberViewModel
                {
                    Username = client.UserName,
                    Image = client.GetImage(),
                    Name = client.Name,
                    IsOnline = repos.MessengerRepository.GetIsOnline(client.Id),
                    LastLogin = null,
                    LastMessage = str,
                    NumberOfUnReadMessages = repos.MessengerRepository.GetUnreadMessages(userId, client.Id).Count(),
                    Type = Tabs.Chats
                };
            }

            var online = repos.MessengerRepository.GetIsOnline(client.Id);

            return new MemberViewModel
            {
                Username = client.UserName,
                Image = client.GetImage(),
                Name = client.Name,
                IsOnline = online,
                LastLogin = !online ? repos.MessengerRepository.GetLastLogin(client.Id) : "Online",
                LastMessage = null,
                NumberOfUnReadMessages = 0,
                Type = tab
            };
        }
    }
}