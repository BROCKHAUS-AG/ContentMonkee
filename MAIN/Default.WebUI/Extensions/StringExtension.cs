using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Extensions
{
    public static class StringExtension
    {
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            StringComparison comp = StringComparison.OrdinalIgnoreCase;
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string ToVirtualPath(this string stringValue)
        {
            return "~" + stringValue.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], string.Empty).Replace("\\", "/");
        }

        public static bool IsNullOrEmpty(this string val)
        {
            return val == null || val == String.Empty;
        }
    }
}