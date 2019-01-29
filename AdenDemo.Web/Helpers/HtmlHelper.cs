using Humanizer;
using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Claims;
using System.Text;
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

        public static IHtmlString RenderStatusBarColor(this HtmlHelper htmlHelper)
        {
            var cssClass = GetCssClass();

            return MvcHtmlString.Create(cssClass);
        }

        public static IHtmlString UserNavBarMenuList(this HtmlHelper htmlHelper, string position)
        {
            var identity = ((ClaimsIdentity)HttpContext.Current.User.Identity);
            var sb = new StringBuilder();
            var cssClass = GetCssClass();

            sb.AppendFormat("<ul class='nav navbar-nav pull-{0} {1}'>", position, cssClass);

            if (!identity.IsAuthenticated)
            {
                //TODO: Create return to aim link
                sb.AppendFormat("<li><a href='{0}aim/ApplicationInventory.aspx'><i class='fa fa-home'></i> My Applications</a></li>", Constants.AimBaseUrl);
                sb.Append("</ul>");
                return MvcHtmlString.Create(sb.ToString());
            }

            sb.Append(@"<li class='dropdown'>");
            sb.AppendFormat(
                @"<a href='#' class='dropdown-toggle' data-toggle='dropdown' role='button' aria-haspopup='true' aria-expanded='false'><i class='fa fa-user'></i>&nbsp;{0}<span class='caret'></span></a>",
                identity.Name);


            sb.Append("<ul class='dropdown-menu'>");

            sb.AppendFormat("<li><a href='{0}aim/ApplicationInventory.aspx'><i class='fa fa-home'></i> My Applications</a></li>", Constants.AimBaseUrl);
            sb.AppendFormat("<li><a href='{0}aim/UserProfile.aspx'><i class='fa fa-book'></i> User Profile</a></li>", Constants.AimBaseUrl);
            sb.AppendFormat("<li><a href='{0}aim/EdDirPositions.aspx'><i class='fa fa-university'></i> EdDir Positions</a></li>", Constants.AimBaseUrl);
            sb.Append("<li role='separator' class='divider'></li>");


            if (identity.HasClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "IdemAppUsers"))
            {
                sb.Append("<li class='dropdown-header'>AIM Groups and Users</li>");
                sb.AppendFormat("<li><a href='{0}aim/admin/RolesAndUsers.aspx'><i class='fa fa-group'></i> Groups and Users</a></li>", Constants.AimBaseUrl);
                sb.AppendFormat("<li><a href='{0}aim/admin/UserMaintenance.aspx'><i class='fa fa-user'></i> User Maintenance</a></li>", Constants.AimBaseUrl);
                sb.AppendFormat("<li><a href='{0}aim/alsde/AppMembership.aspx'><i class='fa fa-heartbeat'></i> App Members</a></li>", Constants.AimBaseUrl);
                sb.Append("<li role='separator' class='divider'></li>");
            }

            if (identity.HasClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "IdemAppAdministrators"))
            {
                sb.Append("<li class='dropdown-header'>AIM Administration</li>");
                sb.AppendFormat(
                    "<li><a href='{0}aim/admin/EditMessages.aspx'><i class='fa fa-comment'></i> Messages</a></li>",
                    Constants.AimBaseUrl);
                sb.AppendFormat(
                    "<li><a href='{0}aim/admin/WebsitesandApplications.aspx'><i class='fa fa-sitemap'></i> Websites and Applications</a></li>",
                    Constants.AimBaseUrl);
                sb.AppendFormat(
                    "<li><a href='{0}aim/admin/Groups.aspx''><i class='fa fa-cogs'></i> Group/Subgroup Maintenance</a></li>",
                    Constants.AimBaseUrl);
                sb.AppendFormat(
                    "<li><a href='{0}aim/alsde/LoadGroups.aspx'><i class='fa fa-cogs'></i> Load Groups</a></li>",
                    Constants.AimBaseUrl);
                sb.Append("<li role='separator' class='divider'></li>");
            }

            //if (LoginHelper.CurrentUser.ImpersonateEmailAddress != HttpContext.Current.User.Identity.Name)
            //{
            //    sb.AppendFormat(
            //        "<li><a href='{0}aim/Index.aspx?Impersonate=0'><i class='fa fa-stop'></i> Stop Impersonating</a></li>",
            //        Constants.AimBaseUrl);
            //    sb.Append("<li role='separator' class='divider'></li>");
            //}

            sb.Append("<li><a href='/account/signout'><i class='fa fa-sign-out'></i> Logout</a></li>");
            sb.Append("</ul>");

            sb.Append("<li><a href='/account/signout'><i class='fa fa-sign-out'></i> Logout</a></li>");

            sb.Append("</ul>");

            return MvcHtmlString.Create(sb.ToString());
        }

        private static string GetCssClass()
        {
            string cssClass;
            Enum.TryParse(Constants.Environment, out Environment env);

            //var env = (Environment)Enum.Parse(typeof(Environment), Constants.Environment);

            switch (env)
            {
                case Environment.Dev:
                    cssClass = "bg-orange";
                    break;
                case Environment.Test:
                    cssClass = "bg-blue";
                    break;
                case Environment.Stage:
                    cssClass = "bg-yellow";
                    break;
                case Environment.Production:
                    cssClass = "bg-white";
                    break;
                default:
                    cssClass = "bg-orange";
                    break;
            }

            return cssClass;
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
