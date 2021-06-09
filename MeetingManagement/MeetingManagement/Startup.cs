using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MeetingManagement.Startup))]
namespace MeetingManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
