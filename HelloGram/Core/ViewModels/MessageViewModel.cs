using System;
using System.Collections.Generic;
using System.Linq;
using HelloGram.Persistence;
using Message = HelloGram.Core.Models.Message;

namespace HelloGram.Core.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        public string SenderUsername { get; set; }

        public string SenderImage { get; set; }

        public string SenderName { get; set; }

        public string Content { get; set; }

        public string Attachment { get; set; }

        public string AttachmentSize { get; set; }

        public bool IsMine { get; set; }

        public bool IsFile { get; set; }

        public bool IsImage { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime SentDateTime { get; set; }

        public bool Seen { get; set; }

        public string ToolTip { get; set; }

        public MessageViewModel Reply { get; set; }

        public static MessageViewModel Create(string userId, Message message)
        {
            if (message == null)
            {
                return null;
            }

            var mine = message.SenderId.Equals(userId);
            string tooltip;

            if (mine)
            {
                tooltip = $"Sent in {message.SentDateTime.ToLongDateString()} at {message.SentDateTime.ToShortTimeString()}";
                tooltip += message.IsRead
                    ? $", Seen in {message.SeenDateTime?.ToLongDateString()} at {message.SeenDateTime?.ToShortTimeString()}"
                    : "";
            }
            else
            {
                tooltip = $"Sent in {message.SentDateTime.ToLongDateString()} at {message.SentDateTime.ToShortTimeString()}";
            }

            return new MessageViewModel
            {
                Id = message.Id,
                SenderUsername = message.Sender.UserName,
                SenderImage = message.Sender.GetImage(),
                SenderName = message.Sender.Name,
                Content = message.Content,
                Attachment = message.Attachment,
                AttachmentSize = message.AttachmentSize,
                IsMine = mine,
                IsFile = message.IsFile,
                IsImage = message.IsImage,
                IsDeleted = message.IsDeleted,
                SentDateTime = message.SentDateTime,
                Seen = message.IsRead && mine,
                ToolTip = tooltip,
                Reply = Create(userId, message.Reply)
            };
        }

        public string GetFormattedSentDate()
        {
            return SentDateTime.ToShortTimeString();
        }
    }
}