using System;
using System.ComponentModel.DataAnnotations;

namespace HelloGram.Core.Models
{
    public class Login
    {
        [Key]
        public string ApplicationUserId { get; set; }

        public string ConnectionId { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsFinished { get; set; }

        public int Count { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}