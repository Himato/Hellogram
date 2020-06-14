using HelloGram.Core.Models;
using HelloGram.Persistence.Repositories;
using Microsoft.Ajax.Utilities;

namespace HelloGram.Persistence
{
    public static class RepositoryExtensionMethods
    {
        public static string GetImage(this ApplicationUser user)
        {
            return !user.Image.IsNullOrWhiteSpace() ? user.Image : "default.png";
        }
    }

    public class Repository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository ApplicationUserRepository { get; }

        public CategoryRepository CategoryRepository { get; }

        public PostRepository PostRepository { get; }

        public MessengerRepository MessengerRepository { get; }

        public StatusRepository StatusRepository { get; set; }

        public Repository(bool isAdmin = false)
        {
            _context = new ApplicationDbContext();
            ApplicationUserRepository = new ApplicationUserRepository(_context);

            if (isAdmin)
            {
                StatusRepository = new StatusRepository(_context);
            }
            else
            {
                CategoryRepository = new CategoryRepository(_context);
                PostRepository = new PostRepository(_context);
                MessengerRepository = new MessengerRepository(_context);
            }
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}