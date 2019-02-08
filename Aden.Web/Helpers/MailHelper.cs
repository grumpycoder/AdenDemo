using System.Configuration;
using System.IO;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

namespace Aden.Web.Helpers
{
    public static class MailHelper
    {
        static bool? _isUsingPickupDirectory;

        /// <summary>
        /// Gets a value to indicate if the default SMTP Delivery
        /// method is SpecifiedPickupDirectory
        /// </summary>
        public static bool IsUsingPickupDirectory
        {
            get
            {
                if (!_isUsingPickupDirectory.HasValue)
                {
                    Configuration config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
                    MailSettingsSectionGroup mail = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
                    _isUsingPickupDirectory = mail.Smtp.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory;
                }
                return _isUsingPickupDirectory.Value;
            }
        }

        /// <summary>
        /// Sets the default PickupDirectoryLocation for the SmtpClient.
        /// </summary>
        /// <remarks>
        /// This method should be called to set the PickupDirectoryLocation
        /// for the SmtpClient at runtime (Application_Start)
        ///
        /// Reflection is used to set the private variable located in the
        /// internal class for the SmtpClient's mail configuration:
        /// System.Net.Mail.SmtpClient.MailConfiguration.Smtp.SpecifiedPickupDirectory.PickupDirectoryLocation
        ///
        /// The folder must exist.
        /// </remarks>
        /// <param name="path"></param>
        public static void SetPickupDirectoryLocation(string path)
        {
            BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo prop;
            object mailConfiguration, smtp, specifiedPickupDirectory;

            // get static internal property: MailConfiguration
            prop = typeof(SmtpClient).GetProperty("MailConfiguration", BindingFlags.Static | BindingFlags.NonPublic);
            mailConfiguration = prop.GetValue(null, null);

            // get internal property: Smtp
            prop = mailConfiguration.GetType().GetProperty("Smtp", instanceFlags);
            smtp = prop.GetValue(mailConfiguration, null);

            // get internal property: SpecifiedPickupDirectory
            prop = smtp.GetType().GetProperty("SpecifiedPickupDirectory", instanceFlags);
            specifiedPickupDirectory = prop.GetValue(smtp, null);

            // get private field: pickupDirectoryLocation, then set it to the supplied path
            FieldInfo field = specifiedPickupDirectory.GetType().GetField("pickupDirectoryLocation", instanceFlags);
            field.SetValue(specifiedPickupDirectory, path);
        }

        /// <summary>
        /// Sets the default PickupDirectoryLocation for the SmtpClient
        /// to the relative path from the current web root.
        /// </summary>
        /// <param name="path">Relative path to the web root</param>
        public static void SetRelativePickupDirectoryLocation(string path)
        {
            SetPickupDirectoryLocation(HttpRuntime.AppDomainAppPath, path);
        }

        /// <summary>
        /// Sets the default PickupDirectoryLocation for the SmtpClient.
        /// </summary>
        /// <remarks>
        /// This is a shortcut for passing in two paths, which are then
        /// combined to set the pickup directory.
        /// </remarks>
        /// <param name="path1">Base path</param>
        /// <param name="path3">Relative path to be combined with </param>
        public static void SetPickupDirectoryLocation(string path1, string path3)
        {
            SetPickupDirectoryLocation(Path.Combine(path1, path3));
        }
    }
}
