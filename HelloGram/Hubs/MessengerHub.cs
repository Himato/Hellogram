using System.Threading.Tasks;
using System.Web;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace HelloGram.Hubs
{
    public class MessengerHub : Hub
    {
        [Authorize]
        public void Register()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            Groups.Add(Context.ConnectionId, userId);

            var repos = new Repository();
            repos.ApplicationUserRepository.Login(userId, Context.ConnectionId);
            repos.Complete();

            LoginHub.ChangeState(userId);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var repos = new Repository();
            var userId = repos.ApplicationUserRepository.Logout(Context.ConnectionId);
            repos.Complete();

            Groups.Remove(Context.ConnectionId, userId);

            LoginHub.ChangeState(userId);

            return base.OnDisconnected(stopCalled);
        }

        public static void Receive(string username, string receiverId, string ids)
        {
            var messengerHub = GlobalHost.ConnectionManager.GetHubContext<MessengerHub>();
            messengerHub.Clients.Group(receiverId).receive(username, ids);
            messengerHub.Clients.Group(receiverId).notify();
        }

        public static void ChangeSeen(string receiverId, string[] list)
        {
            var messengerHub = GlobalHost.ConnectionManager.GetHubContext<MessengerHub>();
            messengerHub.Clients.Group(receiverId).changeSeen(list);
        }
    }
}