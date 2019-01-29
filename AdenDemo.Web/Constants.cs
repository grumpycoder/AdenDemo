using AdenDemo.Web.Helpers;

namespace AdenDemo.Web
{
    public static class Constants
    {

        public static string Environment => AppSettings.Get<string>("ASPNET_ENV");
        public static string DatabaseContextName => "AdenContext";
        public static string AimBaseUrl => "aim.alsde.edu";

        public static string ReplyAddress = AppSettings.Get<string>("ReplyAddress");

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

    }
}
