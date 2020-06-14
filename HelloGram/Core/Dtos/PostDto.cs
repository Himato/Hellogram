using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Dtos
{
    public class PostDto
    {
        [Required]
        [MaxLength(1000, ErrorMessage = "Post text can't exceed 1000 letters")]
        public string Text { get; set; }

        [Required]
        public string Category { get; set; }
    }
}