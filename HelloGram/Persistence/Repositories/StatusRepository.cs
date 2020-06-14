using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloGram.Persistence.Repositories
{
    public class StatusRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly int _lastNumberOfNewUsers = 0;

        private readonly int _lastNumberOfActiveUsers = 0;

        private readonly int _lastNumberOfAge18 = 0;

        private readonly int _lastNumberOfViews = 0;

        private readonly int _lastNumberOfPosts = 0;

        private readonly DateTime _lastMonthDateTime = DateTime.UtcNow.AddMonths(-1);

        public StatusRepository(ApplicationDbContext context)
        {
            _context = context;

            var lastStatus = _context.Statuses.OrderByDescending(m => m.LastTaken).FirstOrDefault();

            if (lastStatus != null)
            {
                _lastNumberOfNewUsers = lastStatus.NewUsers;
                _lastNumberOfActiveUsers = lastStatus.ActiveUsers;
                _lastNumberOfPosts = lastStatus.Posts;

                if (lastStatus.LastTaken > DateTime.UtcNow.AddMonths(-1)) return;

                var newStatus = new Core.Models.Status
                {
                    NewUsers = GetNewUsersStatus().Number,
                    ActiveUsers = GetActiveUsersStatus().Number,
                    Under18 = GetUnder18Status().Number,
                    Views = GetViewsStatus().Number,
                    Posts = GetPostsStatus().Number,
                    LastTaken = DateTime.UtcNow
                };

                _context.Statuses.Add(newStatus);
            }
            else
            {
                lastStatus = new Core.Models.Status
                {
                    NewUsers = _lastNumberOfNewUsers,
                    ActiveUsers = _lastNumberOfActiveUsers,
                    Under18 = _lastNumberOfAge18,
                    Views = _lastNumberOfViews,
                    Posts = _lastNumberOfPosts,
                    LastTaken = DateTime.UtcNow
                };

                _context.Statuses.Add(lastStatus);
            }

            _context.SaveChanges();
        }

        public Status GetUsersStatus()
        {
            var newNumber = _context.Users.Count();

            return new Status(newNumber);
        }

        public Status GetNewUsersStatus()
        {
            var newNumber = _context.Users.Count(m => m.CreatedAt > _lastMonthDateTime);

            return new Status(newNumber, _lastNumberOfNewUsers);
        }

        public Status GetActiveUsersStatus()
        {
            var newNumber = _context.Logins.Count(m => m.Count > 1 && m.DateTime > _lastMonthDateTime);

            return new Status(newNumber, _lastNumberOfActiveUsers);
        }

        public Status GetUnder18Status()
        {
            var age18 = DateTime.UtcNow.AddYears(-18);

            var newNumber = _context.Users.Count(m => m.Age > age18);

            return new Status(newNumber, _lastNumberOfAge18);
        }

        public Status GetViewsStatus()
        {
            var list = _context.Logins.Select(l => l.Count);

            var newNumber = list.Sum();

            return new Status(newNumber, _lastNumberOfViews);
        }

        public Status GetPostsStatus()
        {
            var newNumber = _context.Posts.Count(p => p.Date >= _lastMonthDateTime);

            return new Status(newNumber, _lastNumberOfPosts);
        }

        public Status GetSubscriptionsStatus()
        {
            var newNumber = _context.Subscriptions.Count();

            return new Status(newNumber);
        }

        public Status GetReactionsStatus()
        {
            var newNumber = _context.Reactions.Count();

            return new Status(newNumber);
        }

        public class Status
        {
            public int Number { get; set; }

            public double Percent { get; }

            public bool IsTotal { get; set; }

            public int State { get; }

            public Status(int newNumber, int? oldNumber = null)
            {
                Number = newNumber;

                if (oldNumber == null)
                {
                    Percent = State = 0;
                    IsTotal = true;
                }
                else
                {
                    Percent = oldNumber != 0 ? (double)Math.Abs(newNumber - oldNumber.Value) / oldNumber.Value : newNumber;

                    State = newNumber - oldNumber.Value;
                    IsTotal = false;
                }
            }
        }
    }
}