using System.Web.Mvc;
using System.Web.Routing;

namespace Aden.Web.Filters
{
    public class HandleUnauthorized : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //base.OnAuthorization(filterContext);

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Unauthorized" }));
            }

        }
    }
}
