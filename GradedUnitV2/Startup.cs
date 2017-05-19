using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GradedUnitV2.Startup))]
namespace GradedUnitV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
