using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Hubs;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.Owin;

namespace HelloGram.Persistence.Repositories
{
    public class ApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ApplicationUser> GetAllUsers(int limit, string query = null)
        {
            return query != null ? _context.Users.Where(u => u.Name.Contains(query)).Take(limit) : _context.Users.Take(limit);
        }

        public ApplicationUser GetUserById(string id)
        {
            return _context.Users.FirstOrDefault(u => u.Id.Equals(id));
        }

        public ApplicationUser GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.UserName.Equals(username));
        }

        public IEnumerable<ApplicationUser> GetUsersByName(string userId, string name)
        {
            var followings = GetFollowings(userId).Where(u => u.Name.Contains(name)).ToList();
            if (followings.Any())
            {
                return followings;
            }

            return _context.Users.Where(u => u.Name.Contains(name) && !u.Id.Equals(userId));
        }

        public Login GetLogin(string userId)
        {
            return _context.Logins.FirstOrDefault(l => l.ApplicationUserId.Equals(userId));
        }

        public void Login(string userId, string connectionId)
        {
            var login = GetLogin(userId);
            
            if (login == null)
            {
                _context.Logins.Add(new Login
                {
                    ApplicationUserId = userId,
                    DateTime = DateTime.UtcNow,
                    ConnectionId = connectionId,
                    Count = 1,
                    IsFinished = false
                });
            }
            else
            {
                login.DateTime = DateTime.UtcNow;
                login.ConnectionId = connectionId;
                login.Count += 1;
                login.IsFinished = false;

                _context.Entry(login).State = EntityState.Modified;
            }
        }

        public string Logout(string connectionId)
        {
            var login = _context.Logins.FirstOrDefault(l => l.ConnectionId.Equals(connectionId));

            if (login == null)
            {
                throw new ArgumentException("Login not found");
            }

            login.DateTime = DateTime.UtcNow;
            login.IsFinished = true;

            _context.Entry(login).State = EntityState.Modified;

            return login.ApplicationUserId;
        }

        public int GetNumberOfFollowers(string userId)
        {
            return _context.Relationships.Count(r => r.FollowingId.Equals(userId));
        }

        public int GetNumberOfFollowings(string userId)
        {
            return _context.Relationships.Count(r => r.FollowerId.Equals(userId));
        }

        public IEnumerable<ApplicationUser> GetFollowers(string userId)
        {
            return _context.Relationships.Where(r => r.FollowingId.Equals(userId)).Select(r => r.Follower).OrderBy(f => f.Name);
        }

        public IEnumerable<ApplicationUser> GetFollowings(string userId)
        {
            return _context.Relationships.Where(r => r.FollowerId.Equals(userId)).Select(r => r.Following).OrderBy(f => f.Name);
        }
        
        public void Follow(string userId, string otherUserId)
        {
            var follow = _context.Relationships.FirstOrDefault(s =>
                s.FollowerId.Equals(userId) && s.FollowingId.Equals(otherUserId));

            if (follow == null)
            {
                if (userId.Equals(otherUserId))
                {
                    throw new ArgumentException("You can't follow yourself.");
                }

                _context.Relationships.Add(new Relationship
                {
                    FollowerId = userId,
                    FollowingId = otherUserId
                });
                Notify(otherUserId, NotificationType.Follow, userId, null);
            }
            else
            {
                _context.Relationships.Remove(follow);
            }
        }

        public bool IsFollowing(string userId, string otherUserId)
        {
            return _context.Relationships.Any(s => s.FollowerId.Equals(userId) && s.FollowingId.Equals(otherUserId));
        }

        public string UploadImage(string userId, string filename)
        {
            var user = GetUserById(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var image = user.Image;

            user.Image = filename;
            _context.Entry(user).State = EntityState.Modified;

            return image;
        }

        public IEnumerable<Notification> GetNotifications(string userId)
        {
            return _context.Notifications
                .Where(n => n.ReceiverId.Equals(userId))
                .OrderByDescending(n => n.DateTime)
                .Include(n => n.Sender)
                .Include(n => n.Receiver)
                .Include(n => n.Post.ApplicationUser);
        }

        public int GetNumberOfUnreadNotifications(string userId)
        {
            return _context.Notifications
                .Where(n => n.ReceiverId.Equals(userId) && !n.IsRead)
                .GroupBy(n => n.NotificationType).Select(n => n.GroupBy(c => c.PostId).ToList())
                .Count();
        }

        public void MarkNotificationAsRead(string userId, int? id)
        {
            IEnumerable<Notification> notifications;
            if (id == null)
            {
                notifications = _context.Notifications.Where(n => !n.IsRead && n.ReceiverId.Equals(userId));
            }
            else
            {
                var notification = _context.Notifications.FirstOrDefault(n => n.Id == id && n.ReceiverId.Equals(userId));

                if (notification == null)
                {
                    throw new ArgumentException("Notification doesn't exist");
                }

                notifications = _context.Notifications.Where(n => !n.IsRead && n.ReceiverId.Equals(userId) && n.NotificationType == notification.NotificationType && n.PostId == notification.PostId);
            }

            foreach (var n in notifications)
            {
                n.IsRead = true;
                _context.Entry(n).State = EntityState.Modified;
            }
        }

        public void Notify(string receiverId, NotificationType type, string senderId, int? postId)
        {
            var sender = GetUserById(senderId);

            if (sender == null)
            {
                throw new ArgumentException("Sender doesn't exist");
            }

            if (postId != null)
            {
                var post = new PostRepository(_context).GetPost((int) postId);

                if (post == null)
                {
                    throw new ArgumentException("Post doesn't exist");
                }

                var notification = _context.Notifications.
                    FirstOrDefault(n => n.ReceiverId.Equals(post.ApplicationUserId) && n.SenderId.Equals(sender.Id) && n.NotificationType == type && n.PostId == postId);

                if (!post.ApplicationUserId.Equals(sender.Id) && (notification == null || type == NotificationType.Comment))
                {
                    var newNotification = new Notification
                    {
                        DateTime = DateTime.UtcNow,
                        IsRead = false,
                        PostId = post.Id,
                        SenderId = sender.Id,
                        ReceiverId = post.ApplicationUserId,
                        NotificationType = type,
                        OwnershipType = OwnershipType.Owner
                    };
                    _context.Notifications.Add(newNotification);

                    NotificationsHub.Notify(post.ApplicationUserId, sender, newNotification);
                }
                NotifyOthers(type, senderId, (int)postId);
            }

            if (!receiverId.IsNullOrWhiteSpace())
            {
                var receiver = GetUserById(receiverId);

                if (receiver == null)
                {
                    throw new ArgumentException("Receiver doesn't exist");
                }

                var notification = _context.Notifications.FirstOrDefault(n => n.ReceiverId.Equals(receiver.Id) && n.SenderId.Equals(sender.Id) && n.NotificationType == type);

                if (notification == null)
                {
                    var newNotification = new Notification
                    {
                        DateTime = DateTime.UtcNow,
                        IsRead = false,
                        SenderId = sender.Id,
                        ReceiverId = receiver.Id,
                        NotificationType = NotificationType.Follow,
                        OwnershipType = OwnershipType.Owner
                    };
                    _context.Notifications.Add(newNotification);

                    NotificationsHub.Notify(receiver.Id, sender, newNotification);
                }
            }
        }

        public void NotifyOthers(NotificationType type, string senderId, int postId)
        {
            var sender = GetUserById(senderId);

            if (sender == null)
            {
                throw new ArgumentException("Sender doesn't exist");
            }

            var subscribers = new PostRepository(_context).GetPostSubscriptions(postId).ToList();

            foreach (var subscriber in subscribers)
            {
                if (subscriber.IsOff)
                {
                    continue;
                }

                var notification = _context.Notifications
                    .FirstOrDefault(n => n.ReceiverId.Equals(subscriber.UserId) && n.SenderId.Equals(sender.Id) && n.NotificationType == type && n.PostId == postId);

                var post = new PostRepository(_context).GetPost(postId);

                if (post == null)
                {
                    throw new ArgumentException("Post doesn't exist");
                }

                if (subscriber.UserId.Equals(sender.Id))
                {
                    continue;
                }

                if (notification == null || type == NotificationType.Comment)
                {
                    var newNotification = new Notification
                    {
                        DateTime = DateTime.UtcNow,
                        IsRead = false,
                        PostId = post.Id,
                        SenderId = sender.Id,
                        ReceiverId = subscriber.UserId,
                        NotificationType = type,
                        OwnershipType = OwnershipType.Other
                    };
                    _context.Notifications.Add(newNotification);

                    NotificationsHub.Notify(subscriber.UserId, sender, newNotification);
                }
            }
        }
    }
}