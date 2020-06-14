using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class CategoriesViewModel : BaseViewModel
    {
        public List<CategoryViewModel> CategoryList { get; set; }

        public CategoriesViewModel(ApplicationUser user) : base(NavIndices.Categories, user)
        {
            CategoryList = new List<CategoryViewModel>();

            var categories = new Repository().CategoryRepository.GetCategoryList();
            foreach (var category in categories)
            {
                CategoryList.Add(new CategoryViewModel(user, category));
            }
        }
    }
}