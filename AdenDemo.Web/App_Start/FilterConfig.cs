using System.Web.Mvc;
using Alsde.Mvc.Logging.Attributes;

namespace Aden.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new TrackPerformanceAttribute(Constants.ApplicationName,
                Constants.LayerName));
        }
    }
}
