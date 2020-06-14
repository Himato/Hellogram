using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;

namespace HelloGram.Core.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public bool IsSubscribed { get; set; }

        public string NumberOfSubscriptions { get; set; }

        public string NumberOfPosts { get; set; }

        public List<PostViewModel> PostViewModels { get; set; }

        public CategoryViewModel(ApplicationUser user, Category category) : base(NavIndices.Categories, user)
        {
            Id = category.Id;
            Name = category.Name;
            Image = category.Image;
            Description = category.Description;

            var repos = new Repository();
            IsSubscribed = repos.CategoryRepository.GetIsSubscribed(user.Id, category.Id);

            var subscriptions = repos.CategoryRepository.GetNumberOfSubscriptions(category.Id);
            NumberOfSubscriptions = (subscriptions >= 1000) ? $"{subscriptions / 1000.00:F1}K" : $"{subscriptions}";

            var posts = repos.CategoryRepository.GetNumberOfPosts(category.Id);
            NumberOfPosts = (posts >= 1000) ? $"{posts / 1000.00:F1}K" : $"{posts}";
        }

        public CategoryViewModel(ApplicationUser user, int id) : base(NavIndices.Categories, user)
        {
            var repos = new Repository();
            var category = repos.CategoryRepository.GetCategory(id);

            if (category == null)
            {
                throw new ArgumentException("Not Found");
            }

            Name = category.Name;

            var posts = repos.PostRepository.GetCategoryPosts(id);
            PostViewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                PostViewModels.Add(PostViewModel.Create(post, user, true));
            }
        }
    }
}