using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloGram.Core.ViewModels
{
    public class PostBufferViewModel
    {
        public int Count { get; set; }

        public bool IsDeleted { get; set; }

        public PostBufferViewModel(int count, bool isDeleted = false)
        {
            Count = count;
            IsDeleted = isDeleted;
        }
    }
}