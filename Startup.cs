using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WallaRSSMVC.Startup))]
namespace WallaRSSMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
          //  ConfigureAuth(app);
        }
    }
}
