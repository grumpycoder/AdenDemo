using System;
using System.Web.Http.Filters;

namespace Aden.Web.Filters
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TraceExceptionLogger : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Alsde.Mvc.Logging.Helpers.LogWebError(Constants.ApplicationName, Constants.WebApiLayerName, context.Exception);
        }
    }
}
