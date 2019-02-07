using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(Aden.Web.Startup))]
namespace Aden.Web
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieDomain = "alsde.edu",
                CookieName = "Aden.ApplicationCookie"
            });

            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

        }
    }
}
