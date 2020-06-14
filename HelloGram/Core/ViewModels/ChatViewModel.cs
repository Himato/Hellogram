using System;
using System.Collections.Generic;
using System.Linq;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using Newtonsoft.Json.Linq;

namespace HelloGram.Core.ViewModels
{
    public class ChatViewModel : MessengerViewModel
    {
        public UserViewModel ReceiverUserViewModel { get; set; }

        public bool IsOnline { get; set; }
        
        public string LastLogin { get; set; }

        public List<List<MessageViewModel>> MessageViewModels { get; set; }

        public IEnumerable<FileViewModel> FileViewModels { get; set; }

        public ChatViewModel(ApplicationUser user, ApplicationUser client) : base(user)
        {
            var repos = new Repository();
            
            IsOnline = repos.MessengerRepository.GetIsOnline(client.Id);
            LastLogin = !IsOnline ? repos.MessengerRepository.GetLastLogin(client.Id) : "Online";
            ReceiverUserViewModel = UserViewModel.Create(client, user.Id);

            MessageViewModels = new List<List<MessageViewModel>>();
            var messages = repos.MessengerRepository.GetMessages(user.Id, client.Id).ToList();

            for (var i = 0; i < messages.Count;)
            {
                var list = new List<MessageViewModel>();
                var mine = messages[i].SenderId.Equals(user.Id);
                for (var j = i; j < messages.Count; j++, i++)
                {
                    if (messages[j].SenderId.Equals(user.Id) == mine)
                    {
                        list.Add(MessageViewModel.Create(user.Id, messages[j]));
                    }
                    else
                    {
                        break;
                    }
                }

                MessageViewModels.Add(list);
            }

            FileViewModels = messages.Where(x => x.IsFile && !x.IsImage).OrderByDescending(x => x.SentDateTime).Select(FileViewModel.Create);
        }
    }
}