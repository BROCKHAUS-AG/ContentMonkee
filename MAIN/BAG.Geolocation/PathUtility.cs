using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace BAG.Geolocation
{
    public static class PathUtility
    {
        private const string UtilConfigKey = "BAG.PathUtility.ApplicationLocation";
        private const string UtilConfigKeyLocal = "BAG.PathUtility.ApplicationLocation.Local";

        private static string GetAssemblyLocation()
        {
            return new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
        }

        public static string Sanitize(this string path)
        {
            return Regex.Replace(path, @"[^\u0000-\u007F]", string.Empty);
        }

        public static string ToPhysicalPath(this string virtualPath)
        {
            string basePath = null;
            if (AppConfig.IsLocalUser())
            {
                basePath = ConfigurationManager.AppSettings[UtilConfigKeyLocal];
            }
            else
            {
                basePath = ConfigurationManager.AppSettings[UtilConfigKey];
            }
             
            if (basePath == null && HttpContext.Current != null)
            {
                basePath = HttpContext.Current.Server.MapPath("~");
            }
            if(basePath == null)
            {
                basePath = GetAssemblyLocation();
            }
            string path = virtualPath;

            if (virtualPath.Contains("~"))
            {
                path = path.Replace("~", basePath);
            }

            return path;
        }
    }
}
