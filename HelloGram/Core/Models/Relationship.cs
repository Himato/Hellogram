using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Models
{
    public class Relationship
    {
        [Key]
        [Column(Order = 1)]
        public string FollowerId { get; set; }
        
        [Key]
        [Column(Order = 2)]
        public string FollowingId { get; set; }

        public ApplicationUser Follower { get; set; }

        public ApplicationUser Following { get; set; }
    }
}