using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public int PostId { get; set; }
        
        public string ApplicationUserId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Comment's text has two be at least 2 letters")]
        [MaxLength(500, ErrorMessage = "Comment's text can't be more than 500 letters")]
        public string Text { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        public Post Post { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}