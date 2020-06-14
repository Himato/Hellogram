using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HelloGram.Core.Models;

namespace HelloGram.Persistence.Repositories
{
    public class CategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<string> GetCategories()
        {
            return _context.Categories.Select(c => c.Name).OrderBy(c => c);
        }

        public List<Category> GetCategoryList()
        {
            return _context.Categories.OrderBy(c => c.Name).ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public int GetNumberOfPosts(int id)
        {
            return _context.Posts.Count(p => p.CategoryId == id && !p.IsDeleted);
        }

        public int GetNumberOfSubscriptions(int id)
        {
            return _context.Subscriptions.Count(s => s.CategoryId == id);
        }

        public bool GetIsSubscribed(string userId, int categoryId)
        {
            return _context.Subscriptions.Any(s => s.ApplicationUserId.Equals(userId) && s.CategoryId == categoryId);
        }

        public void Subscribe(int id, string userId)
        {
            var sub = _context.Subscriptions.FirstOrDefault(s =>
                s.ApplicationUserId.Equals(userId) && s.CategoryId == id);

            if (sub == null)
            {
                var category = GetCategory(id);
                if (category == null)
                {
                    throw new ArgumentException("Category doesn't exist");
                }

                _context.Subscriptions.Add(new Subscription
                {
                    ApplicationUserId = userId,
                    CategoryId = id
                });
            }
            else
            {
                _context.Subscriptions.Remove(sub);
            }
        }

        public IEnumerable<Category> GetSubscriptions(string userId)
        {
            return _context.Subscriptions.Where(s => s.ApplicationUserId.Equals(userId))
                .Include(s => s.Category).Select(s => s.Category);
        }
    }
}