using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HelloGram.Startup))]
namespace HelloGram
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
