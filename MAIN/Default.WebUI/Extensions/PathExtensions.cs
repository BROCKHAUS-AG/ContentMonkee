using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Default.WebUI.Extensions
{
    public static class PathExtensions
    {
        public static string GetImagePath(this string path)
        {
            var result = string.Empty;

            if (!string.IsNullOrWhiteSpace(path))
            {
                if (path.StartsWith("/Content/")|| path.StartsWith("http://") || path.StartsWith("https://"))
                    return path;
                result = "/file?cmd=file&target=" + path;
            }
            return result;
        }




    }
}