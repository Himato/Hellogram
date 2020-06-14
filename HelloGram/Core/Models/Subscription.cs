using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Models
{
    public class Subscription
    {
        [Key]
        [Column(Order = 1)]
        public string ApplicationUserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CategoryId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public Category Category { get; set; }
    }
}