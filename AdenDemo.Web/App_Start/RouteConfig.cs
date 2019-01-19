using System.Web.Mvc;
using System.Web.Routing;

namespace AdenDemo.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute
                (
                name: "Review",
                url: "review/{datayear}/{filenumber}",
                defaults: new { controller = "Home", action = "Review", datayear = UrlParameter.Optional, filenumber = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
