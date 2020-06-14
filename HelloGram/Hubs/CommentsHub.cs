using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace HelloGram.Hubs
{
    public class CommentsHub : Hub
    {
        [Authorize]
        public void Register(string postId)
        {
            Groups.Add(Context.ConnectionId, postId);
        }

        public static void Notify(int postId, int commentId)
        {
            var commentsHub = GlobalHost.ConnectionManager.GetHubContext<CommentsHub>();
            commentsHub.Clients.Group(postId + "").notify(commentId);
        }
    }
}