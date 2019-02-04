using AdenDemo.Web.Helpers;

namespace AdenDemo.Web
{
    public static class Constants
    {
        public const string ApplicationName = "Aden";
        public const string LayerName = "WebApp";
        public const string WebApiLayerName = "WebApi";

        public static string TpaAccessKey => AppSettings.Get<string>("TPA_AccessKey");
        public static string DatabaseContextName => "AdenContext";
        public static string Environment => AppSettings.Get<string>("ASPNET_ENV");
        public static string AimApplicationViewKey => AppSettings.Get<string>("ALSDE_AIM_ApplicationViewKey");
        public static string WebServiceUrl => AppSettings.Get<string>("WebServiceUrl");
        public static string AimUrl => AppSettings.Get<string>("AimUrl");

        public static string LogoutUrl = AppSettings.Get<string>("LogoutUrl");


        public static string AimBaseUrl => "aim.alsde.edu";

        public static string ReplyAddress = AppSettings.Get<string>("ReplyAddress");


        public static int CommandTimeout => AppSettings.Get<int>("CommandTimeout");
        public static string GlobalAdministrators => AppSettings.Get<string>("GlobalAdministratorsGroupName");

        public const string FileSpecificationAdministratorGroup = "AdenAppSpecificationAdministrators";


        //Email variables
        public const string TaskIcon = "tasklist";
        public const string CancelledIcon = "event-declined";
        public const string AcceptIcon = "check-file";
        public const string ApprovalIcon = "approval";
        public const string SubmitIcon = "submit-progress";
        public const string ErrorIcon = "error";
        public const string SuccessIcon = "good-quality";
        public const string WorkItemTemplatePath = @"D:\source\AdenDemo\AdenDemo.Web\Templates\WorkItemEmailTemplate.cshtml";
        public const string SubmissionTemplatePath = @"D:\source\AdenDemo\AdenDemo.Web\Templates\WorkItemSubmissionTemplate.cshtml";
        public const string CancelTemplatePath = @"D:\source\AdenDemo\AdenDemo.Web\Templates\WorkItemCancelTemplate.cshtml";
        public const string UserRequestTemplatePath = @"D:\source\AdenDemo\AdenDemo.Web\Templates\UserRequestEmailTemplate.cshtml";

    }
}
