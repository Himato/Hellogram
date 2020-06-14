using System.Web;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace HelloGram.Hubs
{
    public class LoginHub : Hub
    {
        [Authorize]
        public void Register()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            Groups.Add(Context.ConnectionId, userId);
        }

        public static void ChangeState(string userId)
        {
            var loginHub = GlobalHost.ConnectionManager.GetHubContext<LoginHub>();

            var repos = new Repository();
            var chatUsers = repos.MessengerRepository.GetChatUsers(userId);
            var online = repos.MessengerRepository.GetIsOnline(userId);
            var lastLogin = repos.MessengerRepository.GetLastLogin(userId);
            var username = repos.ApplicationUserRepository.GetUserById(userId).UserName;

            foreach (var user in chatUsers)
            {
                loginHub.Clients.Group(user.Id).changeState(username, online, lastLogin);
            }
        }
    }
}