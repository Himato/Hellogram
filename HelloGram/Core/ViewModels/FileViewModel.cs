using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;

namespace HelloGram.Core.ViewModels
{
    public class FileViewModel
    {
        public string Content { get; set; }

        public string Attachment { get; set; }

        public string AttachmentSize { get; set; }

        public static FileViewModel Create(Message message)
        {
            if (message != null && message.IsFile && !message.IsImage)
            {
                return new FileViewModel
                {
                    Content = message.Content,
                    Attachment = message.Attachment,
                    AttachmentSize = message.AttachmentSize
                };
            }

            return null;
        }
    }
}