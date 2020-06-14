using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloGram.Core.Dtos
{
    public class MessageDto
    {
        public int? ReplyId { get; set; }

        public string Content { get; set; }
    }
}