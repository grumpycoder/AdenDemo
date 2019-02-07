using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aden.Web.Helpers
{
    public static class ExceptionHelper
    {
        public static string ToBetterString(this Exception ex, string prepend = null)
        {
            var exceptionMessage = new StringBuilder();

            exceptionMessage.Append("<br />" + prepend + "Exception:" + ex.GetType());
            exceptionMessage.Append("<br />" + prepend + "Message:" + ex.Message);

            exceptionMessage.Append(GetOtherExceptionProperties(ex, "<br />" + prepend));

            exceptionMessage.Append("<br />" + prepend + "Source:" + ex.Source);
            exceptionMessage.Append("<br />" + prepend + "StackTrace:" + ex.StackTrace);

            exceptionMessage.Append(GetExceptionData("<br />" + prepend, ex));

            if (ex.InnerException != null)
                exceptionMessage.Append("<br />" + prepend + "InnerException: "
                    + ex.InnerException.ToBetterString(prepend + string.Concat(Enumerable.Repeat("&nbsp;", 2))));

            return exceptionMessage.ToString();
        }

        private static string GetExceptionData(string prependText, Exception exception)
        {
            var exData = new StringBuilder();
            foreach (var key in exception.Data.Keys.Cast<object>()
                .Where(key => exception.Data[key] != null))
            {
                exData.Append(prependText + $"DATA-{key}:{exception.Data[key]}");
            }

            return exData.ToString();
        }

        private static string GetOtherExceptionProperties(Exception exception, string s)
        {
            var allOtherProps = new StringBuilder();
            var exPropList = exception.GetType().GetProperties();

            var propertiesAlreadyHandled = new List<string>
            { "StackTrace", "Message", "InnerException", "Data", "HelpLink",
                "Source", "TargetSite" };

            foreach (var prop in exPropList
                .Where(prop => !propertiesAlreadyHandled.Contains(prop.Name)))
            {
                var propObject = exception.GetType().GetProperty(prop.Name)
                    .GetValue(exception, null);
                var propEnumerable = propObject as IEnumerable;

                if (propEnumerable == null || propObject is string)
                    allOtherProps.Append(s + String.Format("{0} : {1}",
                        prop.Name, propObject));
                else
                {
                    var enumerableSb = new StringBuilder();
                    foreach (var item in propEnumerable)
                    {
                        enumerableSb.Append(item + "|");
                    }
                    allOtherProps.Append(s + String.Format("{0} : {1}",
                        prop.Name, enumerableSb));
                }
            }

            return allOtherProps.ToString();
        }
    }
}
