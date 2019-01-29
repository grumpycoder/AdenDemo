using Humanizer;
using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using System.Web.Mvc;


namespace AdenDemo.Web.Helpers
{
    public static class HtmlHelpers
    {

        public static IHtmlString AssemblyVersion(this HtmlHelper helper)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return MvcHtmlString.Create(version);
        }

        public static IHtmlString RenderConfigurationValue(this HtmlHelper htmlHelper, string key)
        {
            var value = AppSettings.Get<string>(key);
            return new MvcHtmlString(value);
        }

        public static IHtmlString RenderMachineName(this HtmlHelper htmlHelper)
        {
            var value = System.Environment.MachineName;
            return new MvcHtmlString(value);
        }


        public static IHtmlString RenderDataSource(this HtmlHelper htmlHelper)
        {
            var connectionString = AppSettings.GetDatabaseString<string>(Constants.DatabaseContextName);
            var builder = new SqlConnectionStringBuilder { ConnectionString = connectionString };
            return new MvcHtmlString(builder.DataSource);
        }

        public static IHtmlString RenderDataName(this HtmlHelper htmlHelper)
        {
            var connectionString = AppSettings.GetDatabaseString<string>(Constants.DatabaseContextName);
            var builder = new SqlConnectionStringBuilder { ConnectionString = connectionString };
            return new MvcHtmlString(builder.InitialCatalog);
        }

        public static IHtmlString RenderApplicationName(this HtmlHelper htmlHelper)
        {
            var appInstance = htmlHelper.ViewContext.HttpContext.ApplicationInstance;

            var memberInfo = appInstance.GetType().BaseType;
            if (memberInfo != null)
            {
                var attr = memberInfo.Assembly.GetAssemblyAttribute<AssemblyTitleAttribute>();

                return new MvcHtmlString(attr.Title ?? "No Application Title");
            }
            return new MvcHtmlString("No Application Title");
        }

        public static IHtmlString RenderApplicationDescription(this HtmlHelper htmlHelper)
        {
            var appInstance = htmlHelper.ViewContext.HttpContext.ApplicationInstance;

            var memberInfo = appInstance.GetType().BaseType;
            if (memberInfo != null)
            {
                var attr = memberInfo.Assembly.GetAssemblyAttribute<AssemblyDescriptionAttribute>();

                return new MvcHtmlString(attr.Description ?? "No Application Description");
            }
            return new MvcHtmlString("No Application Description");
        }


        public static string MakeActiveClass(this UrlHelper urlHelper, string controller)
        {
            string result = "active";

            string controllerName = urlHelper.RequestContext.RouteData.Values["controller"].ToString();

            if (!controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
            {
                result = null;
            }

            return result;
        }

        public static MvcHtmlString NavigationLink(this HtmlHelper html, string linkText, string action, string controller, string icon = null, object routeValues = null, object css = null)
        {
            TagBuilder aTag = new TagBuilder("a");
            TagBuilder liTag = new TagBuilder("li");
            var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(css);
            string url = (routeValues == null) ?
                (new UrlHelper(html.ViewContext.RequestContext)).Action(action, controller)
                : (new UrlHelper(html.ViewContext.RequestContext)).Action(action, controller, routeValues);

            var imageIcon = string.Empty;
            if (icon != null)
            {
                imageIcon = $@"<i class='{icon} visible-sm'>&nbsp;</i>&nbsp;&nbsp;";
            }

            linkText = imageIcon + "<span class='hidden-sm'>" + linkText + "<span>";
            aTag.MergeAttribute("href", url);
            aTag.InnerHtml = linkText;

            aTag.MergeAttributes(htmlAttributes);

            if (action.ToLower() == html.ViewContext.RouteData.Values["action"].ToString().ToLower() && controller.ToLower() == html.ViewContext.RouteData.Values["controller"].ToString().ToLower())
                liTag.MergeAttribute("class", "active");

            liTag.InnerHtml = aTag.ToString(TagRenderMode.Normal);
            return new MvcHtmlString(liTag.ToString(TagRenderMode.Normal));
        }

        public static IHtmlString HumanizeTitleCase(this HtmlHelper htmlHelper, string key)
        {
            var value = key.Humanize(LetterCasing.Title);
            return new MvcHtmlString(value);
        }
    }

    enum Environment
    {
        Dev,
        Test,
        Stage,
        Production
    }

}
