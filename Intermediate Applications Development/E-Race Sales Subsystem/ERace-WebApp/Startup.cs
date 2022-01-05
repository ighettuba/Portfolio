using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ERace_WebApp.Startup))]
namespace ERace_WebApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
