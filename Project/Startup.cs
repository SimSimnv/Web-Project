using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebProject.Startup))]
namespace WebProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
