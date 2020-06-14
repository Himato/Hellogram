﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Models
{
    public class Save
    {
        [Key]
        [Column(Order = 1)]
        public int PostId { get; set; }

        [Key]
        [Column(Order = 2)]
        public string ApplicationUserId { get; set; }

        public Post Post { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}