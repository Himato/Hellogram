using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using HelloGram.Core.Dtos;
using HelloGram.Core.Models;
using HelloGram.Hubs;

namespace HelloGram.Persistence.Repositories
{
    public class PostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public int GetNumberOfPosts(string userId)
        {
            return _context.Posts.Count(p => p.ApplicationUserId.Equals(userId) && !p.IsDeleted);
        }

        public IEnumerable<Post> GetHomePosts(string userId)
        {
            var applicationUserRepository = new ApplicationUserRepository(_context);
            var categoryRepository = new CategoryRepository(_context);
            var followings = applicationUserRepository.GetFollowings(userId);
            var subscriptions = categoryRepository.GetSubscriptions(userId);

            // f stands for followings - s stands for subscription - n stands for new
            /*
             * Algorithm goes as following for only 20 posts:-
             * f s n
             * f   n
             *   s n
             * f s 
             * f
             *   s
             */
            var fPosts = _context.Posts
                .Where(p => followings.Any(f => f.Id.Equals(p.ApplicationUserId)) && !p.IsDeleted)
                .Include(p => p.ApplicationUser)
                .Include(p => p.Category);

            var sPosts = _context.Posts
                .Where(p => subscriptions.Any(s => s.Id == p.CategoryId) && !p.IsDeleted)
                .Include(p => p.ApplicationUser)
                .Include(p => p.Category);

            // Better Solution in the future

            return sPosts.Union(fPosts).OrderByDescending(p => p.Date);
        }

        public IEnumerable<Post> GetTrendingPosts()
        {
            var last = DateTime.UtcNow.AddDays(-7);
            return _context.Reactions
                .GroupBy(r => r.PostId)
                .OrderByDescending(r => r.Count())
                .Select(t => t.FirstOrDefault())
                .Select(r => r.Post)
                .Where(p => p.Date > last)
                .OrderByDescending(p => p.Date)
                .Take(20)
                .Include(p => p.ApplicationUser)
                .Include(p => p.Category);
        }

        public IEnumerable<Post> GetPosts(string userId)
        {
            return _context.Posts.Where(p => p.ApplicationUserId.Equals(userId) && !p.IsDeleted)
                .Include(c => c.Category)
                .OrderByDescending(p => p.Date);
        }

        public IEnumerable<Post> GetPosts(string userId, bool isDeleted)
        {
            return _context.Posts.Where(p => p.ApplicationUserId.Equals(userId) && p.IsDeleted == isDeleted)
                .Include(a => a.ApplicationUser)
                .Include(c => c.Category)
                .OrderByDescending(p => p.Date);
        }

        public List<Post> GetCategoryPosts(int categoryId)
        {
            return _context.Posts.Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                    .Include(a => a.ApplicationUser)
                    .Include(c => c.Category)
                    .OrderByDescending(p => p.Date).ToList();
        }

        public Post GetPost(int id)
        {
            return _context.Posts.Include(a => a.ApplicationUser).Include(c => c.Category).FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        }
        public Post GetPostApi(int id)
        {
            return _context.Posts.Include(c => c.Category).FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        }

        public Post AddPost(string userId, PostDto postDto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name.Equals(postDto.Category));
            if (category == null)
            {
                throw new ArgumentException("Invalid category");
            }

            var post = new Post
            {
                Text = postDto.Text,
                Date = DateTime.UtcNow,
                CategoryId = category.Id,
                ApplicationUserId = userId,
                IsDeleted = false
            };
            _context.Posts.Add(post);

            return post;
        }

        public string AddImage(int id, string image)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                throw new ArgumentException("Post doesn't exist");
            }

            var old = post.Image;

            post.Image = image;

            _context.Entry(post).State = EntityState.Modified;

            return old;
        }

        public Post UpdatePost(int id, string userId, PostDto postDto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name.Equals(postDto.Category));
            if (category == null)
            {
                throw new ArgumentException("Invalid category");
            }

            var oldPost = GetPost(id);

            if (oldPost == null)
            {
                throw new ArgumentException("Entry Not Found");
            }

            if (!oldPost.ApplicationUserId.Equals(userId))
            {
                throw new AuthenticationException();
            }

            oldPost.Text = postDto.Text;
            oldPost.CategoryId = category.Id;

            _context.Entry(oldPost).State = EntityState.Modified;

            return oldPost;
        }

        public Post UpdatePost(int id, string userId)
        {
            var oldPost = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (oldPost == null)
            {
                throw new ArgumentException("Entry Not Found");
            }

            if (!oldPost.ApplicationUserId.Equals(userId))
            {
                throw new AuthenticationException();
            }

            oldPost.IsDeleted = false;

            _context.Entry(oldPost).State = EntityState.Modified;

            return oldPost;
        }

        public void DeletePost(int id, string userId)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                throw new ArgumentException("Entry Not Found");
            }

            if (!post.ApplicationUserId.Equals(userId))
            {
                throw new AuthenticationException();
            }

            if (post.IsDeleted)
            {
                _context.Posts.Remove(post);
            }
            else
            {
                post.IsDeleted = true;
                _context.Entry(post).State = EntityState.Modified;
            }
        }

        public void Save(int id, string userId)
        {
            var post = _context.Saves.FirstOrDefault(s =>
                s.ApplicationUserId.Equals(userId) && s.PostId == id);

            if (post == null)
            {
                var p = GetPost(id);
                if (p == null)
                {
                    throw new ArgumentException("Post doesn't exist");
                }

                if (p.ApplicationUserId.Equals(userId))
                {
                    throw new ArgumentException("You can't save your own post");
                }
                _context.Saves.Add(new Save
                {
                    ApplicationUserId = userId,
                    PostId = id
                });
            }
            else
            {
                _context.Saves.Remove(post);
            }
        }

        public IEnumerable<Post> GetSavedPosts(string userId)
        {
            return _context.Saves.Where(s => s.ApplicationUserId.Equals(userId))
                .Include(s => s.Post)
                .Select(s => s.Post)
                .Include(p => p.ApplicationUser)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Date);
        }

        public bool IsSaved(int id, string userId)
        {
            return _context.Saves.Any(s => s.ApplicationUserId.Equals(userId) && s.PostId == id);
        }

        public void React(int id, string userId, bool isLike)
        {
            var reaction = _context.Reactions.FirstOrDefault(s =>
                s.ApplicationUserId.Equals(userId) && s.PostId == id);

            if (reaction == null)
            {
                var p = GetPost(id);
                if (p == null)
                {
                    throw new ArgumentException("Post doesn't exist");
                }
                
                _context.Reactions.Add(new Reaction
                {
                    ApplicationUserId = userId,
                    PostId = id,
                    IsLike = isLike
                });
                new ApplicationUserRepository(_context).Notify(null, isLike ? NotificationType.Like : NotificationType.Dislike, userId, id);
            }
            else
            {
                if (reaction.IsLike == isLike)
                {
                    _context.Reactions.Remove(reaction);
                }
                else
                {
                    reaction.IsLike = isLike;
                    _context.Entry(reaction).State = EntityState.Modified;
                }
            }
        }

        public bool IsReacted(int id, string userId)
        {
            return _context.Reactions.Any(s => s.ApplicationUserId.Equals(userId) && s.PostId == id);
        }

        public bool GetReact(int id, string userId)
        {
            return _context.Reactions.First(s => s.ApplicationUserId.Equals(userId) && s.PostId == id).IsLike;
        }

        public int GetNumberOfLikes(int id)
        {
            return _context.Reactions.Count(s => s.PostId == id && s.IsLike);
        }

        public int GetNumberOfDisLikes(int id)
        {
            return _context.Reactions.Count(s => s.PostId == id && !s.IsLike);
        }

        public IEnumerable<Comment> GetComments(int id)
        {
            return _context.Comments.Where(c => c.PostId == id)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Post)
                .OrderByDescending(c => c.DateTime);
        }

        public int GetNumberOfComments(int id)
        {
            return _context.Comments.Count(c => c.PostId == id);
        }

        public Comment GetComment(int commentId)
        {
            return _context.Comments.Include(c => c.ApplicationUser).Include(c => c.Post).FirstOrDefault(c => c.Id == commentId);
        }

        public Comment AddComment(int id, string userId, string text)
        {
            var post = GetPost(id);

            if (post == null)
            {
                throw new ArgumentException("Post doesn't exist");
            }

            if (text.Length > 500)
            {
                throw new ArgumentException("Comment's text can't be more than 125 letters");
            }

            var comment = new Comment
            {
                PostId = id,
                ApplicationUserId = userId,
                ApplicationUser = new ApplicationUserRepository(_context).GetUserById(userId),
                Text = text.Trim(),
                DateTime = DateTime.UtcNow
            };
            _context.Comments.Add(comment);
            new ApplicationUserRepository(_context).Notify(null, NotificationType.Comment, userId, id);
            Subscribe(id, userId, true);

            return comment;
        }

        public void EditComment(int commentId, string userId, string text)
        {
            var comment = GetComment(commentId);

            if (comment == null || !comment.ApplicationUserId.Equals(userId))
            {
                throw new ArgumentException("Comment doesn't exist");
            }

            if (text.Length > 500)
            {
                throw new ArgumentException("Comment's text can't be more than 125 letters");
            }

            comment.Text = text;

            _context.Entry(comment).State = EntityState.Modified;
        }

        public void DeleteComment(int id, string userId)
        {
            var comment = _context.Comments
                .Include(c => c.Post)
                .FirstOrDefault(c => c.Id == id && (c.ApplicationUserId.Equals(userId) || c.Post.ApplicationUserId.Equals(userId)));

            if (comment == null)
            {
                throw new ArgumentException("Comment doesn't exist");
            }

            _context.Comments.Remove(comment);
        }

        public bool IsSubscribed(int id, string userId)
        {
            return _context.PostSubscriptions.Any(n => n.PostId == id && n.UserId.Equals(userId) && !n.IsOff);
        }

        public void Subscribe(int id, string userId, bool overwrite = false)
        {
            var post = GetPost(id);

            if (post == null)
            {
                throw new ArgumentException("Post doesn't exist");
            }

            var subscription =
                _context.PostSubscriptions.FirstOrDefault(n => n.PostId == post.Id && n.UserId.Equals(userId));

            if (subscription == null)
            {
                if (post.ApplicationUserId.Equals(userId))
                {
                    return;
                }

                _context.PostSubscriptions.Add(new PostSubscription
                {
                    PostId = id,
                    UserId = userId,
                    IsOff = false
                });
            }
            else
            {
                if (overwrite) return;

                subscription.IsOff = !subscription.IsOff;
                _context.Entry(subscription).State = EntityState.Modified;
            }
        }

        public IEnumerable<PostSubscription> GetPostSubscriptions(int id)
        {
            return _context.PostSubscriptions.Where(n => n.PostId == id).Include(n => n.User);
        }
    }
}