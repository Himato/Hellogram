using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Models
{
    public class Status
    {
        public int Id { get; set; }

        public int NewUsers { get; set; }

        public int ActiveUsers { get; set; }

        public int Under18 { get; set; }

        public int Views { get; set; }

        public int Posts { get; set; }

        public DateTime LastTaken { get; set; }
    }
}