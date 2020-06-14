using System.Data.Entity;
using HelloGram.Core.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelloGram.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Save> Saves { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PostSubscription> PostSubscriptions { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}