using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "Post text can't exceed 1000 letters")]
        public string Text { get; set; }

        public DateTime Date { get; set; }

        public string Image { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public bool IsDeleted { get; set; }
    }
}