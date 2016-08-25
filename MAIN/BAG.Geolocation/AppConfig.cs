using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace BAG.Geolocation
{
    public class AppConfig
    {
        #region Singleton

        private static AppConfig _instance;

        public static AppConfig Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppConfig();
                }

                return _instance;
            }
        }

        #endregion

        public string this[string key]
        {
            get
            {
                return ConfigurationManager.AppSettings[key];
            }
        }

        public string GetOption(string name, string @default)
        {
            return this[name] ?? @default;
        }

        public static bool IsLocalUser()
        {
            var result = false;
            if (HttpContext.Current != null)
            {
                result = HttpContext.Current.Request.IsLocal;              
            }
            
            return result;
        }

        public static bool IsProductiveEnvironment
        {
            get { return bool.Parse(Current["BAG.ProductiveMode"]); }
        }

        public static string TestMail
        {
            get { return Current["BAG.TestMail"]; }
        }

        // Determine whether the website is deployed to the testserver.
        public static bool IsTestEnvironment()
        {
            string hostName = Dns.GetHostName();
            string testServer = ConfigurationManager.AppSettings["BAG.TestServerIp"];
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            return hostEntry.AddressList.Any(entry => entry.ToString() == testServer);
        }
    }
}
