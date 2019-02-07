using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Aden.Web.Helpers
{
    public static class EnumHelper<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes[0].ResourceType != null)
                return lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }


    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum val)
        {
            return val.GetType()
                       .GetMember(val.ToString())
                       .FirstOrDefault()
                       .GetCustomAttribute<DisplayAttribute>(false)
                       .Name
                   ?? val.ToString();
        }


        public static string GetShortName(this Enum val)
        {

            return val.GetAttribute<DisplayAttribute>().ShortName;

            //return val.GetType()
            //           .GetMember(val.ToString())
            //           .FirstOrDefault()
            //           .GetCustomAttribute<DisplayAttribute>(false)
            //           .ShortName
            //       ?? val.ToString();
        }

        public static string GetDescription(this Enum val)
        {

            return val.GetAttribute<DisplayAttribute>().Description;

            //return val.GetType()
            //           .GetMember(val.ToString())
            //           .FirstOrDefault()
            //           .GetCustomAttribute<DisplayAttribute>(false)
            //           .ShortName
            //       ?? val.ToString();
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttribute<TAttribute>();
        }

        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            T[] arr = (T[])Enum.GetValues(src.GetType());
            var j = Array.IndexOf<T>(arr, src) + 1;
            return (arr.Length == j) ? arr[0] : arr[j];
        }

        public static T Previous<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            T[] arr = (T[])Enum.GetValues(src.GetType());
            var j = Array.IndexOf<T>(arr, src) - 1;
            return (arr.Length == j) ? arr[0] : arr[j];
        }

    }
}
