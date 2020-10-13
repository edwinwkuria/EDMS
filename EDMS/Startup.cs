using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EDMS.Startup))]
namespace EDMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
