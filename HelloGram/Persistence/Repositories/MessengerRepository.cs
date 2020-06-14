using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HelloGram.Core.Dtos;
using HelloGram.Core.Models;
using HelloGram.Core.ViewModels;
using HelloGram.Hubs;

namespace HelloGram.Persistence.Repositories
{
    public class MessengerRepository
    {
        private readonly ApplicationDbContext _context;

        public MessengerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Preference GetPreference(string userId)
        {
            return _context.Preferences.FirstOrDefault(p => p.ApplicationUserId.Equals(userId));
        }

        public void UpdatePreference(string userId, PreferenceDto preferenceDto)
        {
            var preference = GetPreference(userId);

            if (preference == null)
            {
                preference = new Preference
                {
                    NotificationSound = preferenceDto.NotificationSound,
                    DarkTheme = preferenceDto.DarkTheme,
                    ActiveState = preferenceDto.ActiveState,
                    ApplicationUserId = userId
                };

                _context.Preferences.Add(preference);
            }
            else
            {
                preference.NotificationSound = preferenceDto.NotificationSound;
                preference.DarkTheme = preferenceDto.DarkTheme;
                preference.ActiveState = preferenceDto.ActiveState;

                _context.Entry(preference).State = EntityState.Modified;
            }
        }

        public bool GetIsOnline(string userId)
        {
            var pref = GetPreference(userId);

            if (pref != null && !pref.ActiveState)
            {
                return false;
            }

            var login = new ApplicationUserRepository(_context).GetLogin(userId);
            if (login == null)
            {
                return false;
            }

            return !login.IsFinished;
        }

        public string GetLastLogin(string userId)
        {
            var pref = GetPreference(userId);

            if (pref != null && !pref.ActiveState) return null;

            var date = new ApplicationUserRepository(_context).GetLogin(userId)?.DateTime;
            return $"Last seen in {date?.ToString("dd MMM yyyy")} at {date?.ToShortTimeString()}";
        }

        public IEnumerable<ApplicationUser> GetChatUsers(string userId)
        {
            return _context.Messages
                .Where(m => m.ReceiverId.Equals(userId) || m.SenderId.Equals(userId))
                .Include(m => m.Sender).Include(m => m.Receiver)
                .Select(m => m.ReceiverId.Equals(userId) ? m.Sender : m.Receiver).Distinct();
        }

        private IEnumerable<Message> GetSentMessage(string userId, string clientId)
        {
            return _context.Messages.Where(m => m.SenderId.Equals(userId) && m.ReceiverId.Equals(clientId)).Include(m => m.Sender).Include(m => m.Reply);
        }

        private IEnumerable<Message> GetReceivedMessage(string userId, string clientId)
        {
            return _context.Messages.Where(m => m.ReceiverId.Equals(userId) && m.SenderId.Equals(clientId))
                .Include(m => m.Sender).Include(m => m.Reply);
        }

        public IEnumerable<Message> GetMessages(string userId, string username)
        {
            return GetSentMessage(userId, username)
                .Union(GetReceivedMessage(userId, username)).OrderBy(m => m.SentDateTime);
        }
        
        public IEnumerable<Message> GetUnreadMessages(string userId, string clientId)
        {
            return GetReceivedMessage(userId, clientId).Where(m => !m.IsRead);
        }

        public bool HasUnreadMessages(string userId)
        {
            return _context.Messages.Any(m => m.ReceiverId.Equals(userId) && !m.IsRead);
        }

        public Message GetMessage(string userId, int messageId)
        {
            return _context.Messages.Include(m => m.Sender).Include(m => m.Reply)
                .FirstOrDefault(m => m.Id == messageId && (m.ReceiverId.Equals(userId) || m.SenderId.Equals(userId)));
        }

        public Message UploadMessage(string userId, string receiverId, MessageDto messageDto)
        {
            if (receiverId.Equals(userId))
            {
                throw new ArgumentException("You can't send to yourself");
            }

            var message = new Message
            {
                Content = messageDto.Content.Trim(),
                IsFile = false,
                IsImage = false,
                IsDeleted = false,
                IsRead = false,
                SentDateTime = DateTime.UtcNow,
                SenderId = userId,
                ReceiverId = receiverId,
                ReplyId = messageDto.ReplyId
            };

            _context.Messages.Add(message);
            return message;
        }

        public Message UploadAttachment(string userId, string receiverId, string content, string attachment,
            string size, bool isImage)
        {
            if (receiverId.Equals(userId))
            {
                throw new ArgumentException("You can't send to yourself");
            }

            var message = new Message
            {
                Content = content,
                Attachment = attachment,
                AttachmentSize = size,
                IsFile = true,
                IsImage = isImage,
                IsDeleted = false,
                IsRead = false,
                SentDateTime = DateTime.UtcNow,
                SenderId = userId,
                ReceiverId = receiverId
            };

            _context.Messages.Add(message);
            return message;
        }

        public void Send(string userId, string receiverUsername, string ids)
        {
            var repos = new ApplicationUserRepository(_context);
            var clientId = repos.GetUserByUsername(receiverUsername).Id;
            MessengerHub.Receive(repos.GetUserById(userId).UserName, clientId, ids);
        }

        public List<string> MarkAsRead(string userId, string clientId)
        {
            var messages = GetUnreadMessages(userId, clientId);
            var list = new List<string>();

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.SeenDateTime = DateTime.UtcNow;
                _context.Entry(message).State = EntityState.Modified;

                list.Add($"{message.Id}#Sent in {message.SentDateTime.ToLongDateString()} at {message.SentDateTime.ToShortTimeString()}, Seen in {message.SeenDateTime?.ToLongDateString()} at {message.SeenDateTime?.ToShortTimeString()}");
            }

            return list;
        }
    }
}