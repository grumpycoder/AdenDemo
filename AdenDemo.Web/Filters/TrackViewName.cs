using Alsde.Extensions;
using System;
using System.Web.Mvc;

namespace Aden.Web.Filters
{
    public class TrackViewName : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewResultBase view = filterContext.Result as ViewResultBase;
            if (view != null)
            {
                string viewName = view.ViewName;
                // If we did not explicitly specify the view name in View() method,
                // it will be same as the action name. So let's get that.
                if (String.IsNullOrEmpty(viewName))
                {
                    viewName = filterContext.ActionDescriptor.ActionName;
                }
                view.ViewBag.CurrentView = viewName.ToTitleCase();
            }
        }
    }
}
