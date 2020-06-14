using System;
using System.ComponentModel.DataAnnotations;

namespace HelloGram.Core.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public string Attachment { get; set; }

        public string AttachmentSize { get; set; }

        public int? ReplyId { get; set; }
        
        public bool IsFile { get; set; }

        public bool IsImage { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsRead { get; set; }

        public DateTime SentDateTime { get; set; }

        public DateTime? SeenDateTime { get; set; }
        
        public string SenderId { get; set; }

        public string ReceiverId { get; set; }
        
        public Message Reply { get; set; }

        public ApplicationUser Sender { get; set; }

        public ApplicationUser Receiver { get; set; }
    }
}