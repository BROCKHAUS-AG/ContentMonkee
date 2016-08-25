using BAG.Common.Data;
using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BAG.Common
{

    public class _Globals
    {
        public static _Globals Instance
        {
            get
            {
                var result = HttpContext.Current.Items["_Globals"] as _Globals;
                if (result == null)
                {
                    result = new _Globals();
                    HttpContext.Current.Items["_Globals"] = result;
                }
                return result;
            }
        }


        public static string ProductName { get { return "Default CMS"; } }

        public bool IsAuthenticated
        {
            get
            {
                return CurrentLoginUserId != Guid.Empty;
            }

        }

        public Guid CurrentLoginUserId
        {
            get
            {
                Guid result = Guid.Empty;
                if (HttpContext.Current.Session == null)
                {
                    if (HttpContext.Current.Items["CurrentLoginUserId"] != null)
                        result = (Guid)HttpContext.Current.Items["CurrentLoginUserId"];
                }
                else
                    if (HttpContext.Current.Session["CurrentLoginUserId"] != null)
                    result = (Guid)HttpContext.Current.Session["CurrentLoginUserId"];
                return result;
            }
            set
            {
                if (HttpContext.Current.Session == null)
                    HttpContext.Current.Items["CurrentLoginUserId"] = value;
                else
                    HttpContext.Current.Session["CurrentLoginUserId"] = value;
            }
        }
        public string CurrentLoginUserName
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session == null)
                {
                    if (HttpContext.Current.Items["CurrentLoginUserName"] != null)
                        result = (string)HttpContext.Current.Items["CurrentLoginUserName"];
                }
                else if (HttpContext.Current.Session["CurrentLoginUserName"] != null)
                    result = (string)HttpContext.Current.Session["CurrentLoginUserName"];
                return result;
            }
            set
            {
                if (HttpContext.Current.Session == null)
                    HttpContext.Current.Items["CurrentLoginUserName"] = value;
                else
                    HttpContext.Current.Session["CurrentLoginUserName"] = value;
            }
        }


        public string CurrentDisplayName
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session == null)
                {
                    if (HttpContext.Current.Items["CurrentDisplayName"] != null)
                        result = (string)HttpContext.Current.Items["CurrentDisplayName"];
                }
                else if (HttpContext.Current.Session["CurrentDisplayName"] != null)
                    result = (string)HttpContext.Current.Session["CurrentDisplayName"];
                return result;
            }
            set
            {
                if (HttpContext.Current.Session == null)
                    HttpContext.Current.Items["CurrentDisplayName"] = value;
                else
                    HttpContext.Current.Session["CurrentDisplayName"] = value;
            }
        }


        public bool EnableCookies
        {
            get
            {

                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("EnableCookies"))
                {
                    var cookie = HttpContext.Current.Request.Cookies["EnableCookies"];
                    var resultvalue = cookie.Value;
                    if (cookie.Value.Equals("False"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }

                return false;
            }
            set
            {
                var cookie = new HttpCookie("EnableCookies", value.ToString())
                {
                    Expires = DateTime.Now.AddDays(5)
                };
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }

        private Guid currentSiteSettingId = Guid.Empty;
        public Guid CurrentSiteSettingId
        {
            get
            {
                if (!EnableCookies)
                {
                    return currentSiteSettingId;
                }
                Guid result = currentSiteSettingId;
                if (HttpContext.Current.Session == null)
                {
                    if (HttpContext.Current.Items["CurrentSiteSettingId"] != null)
                        result = (Guid)HttpContext.Current.Items["CurrentSiteSettingId"];
                }
                else if (HttpContext.Current.Session["CurrentSiteSettingId"] != null)
                    result = (Guid)HttpContext.Current.Session["CurrentSiteSettingId"];

                return result;
            }
            private set
            {
                currentSiteSettingId = value;
                if (!EnableCookies)
                {
                    return;
                }
                if (HttpContext.Current.Session == null)
                    HttpContext.Current.Items["CurrentSiteSettingId"] = value;
                else
                    HttpContext.Current.Session["CurrentSiteSettingId"] = value;
            }
        }


        public Guid CurrentAccountId
        {
            get
            {
                Guid result = Guid.Empty;
                if (HttpContext.Current.Session == null)
                {
                    if (HttpContext.Current.Items["CurrentAccountId"] != null)
                        result = (Guid)HttpContext.Current.Items["CurrentAccountId"];
                }
                else if (HttpContext.Current.Session["CurrentAccountId"] != null)
                    result = (Guid)HttpContext.Current.Session["CurrentAccountId"];

                return result;
            }
            set
            {
                if (HttpContext.Current.Session == null)
                    HttpContext.Current.Items["CurrentAccountId"] = value;
                else
                    HttpContext.Current.Session["CurrentAccountId"] = value;
            }
        }

        public string CurrentLanguage
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session == null)
                {
                    if (HttpContext.Current.Items["CurrentLanguage"] != null)
                        result = (string)HttpContext.Current.Items["CurrentLanguage"];
                }
                else if (HttpContext.Current.Session["CurrentLanguage"] != null)
                    result = (string)HttpContext.Current.Session["CurrentLanguage"];

                if (string.IsNullOrEmpty(result))
                    return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(result);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(result);

                return result;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = "de";

                if (HttpContext.Current.Session == null)
                    HttpContext.Current.Items["CurrentLanguage"] = value;
                else
                    HttpContext.Current.Session["CurrentLanguage"] = value;

                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(value);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(value);
            }
        }

        User loginUser;
        public User GetLoginUser(UnitOfWork unit)
        {
            if (loginUser != null)
                return loginUser;

            if (CurrentLoginUserId != Guid.Empty)
                loginUser = unit.UserRepository.GetByID(CurrentLoginUserId);//unit.UserRepository.GetByID(CurrentLoginUserId);

            return loginUser;
        }

        public bool ChangingSiteSettingId
        {
            get
            {
                bool result = true;

                if (HttpContext.Current.Session == null)
                {
                    if (HttpContext.Current.Items["ChangingSiteSettingId"] != null)
                        result = (bool)HttpContext.Current.Items["ChangingSiteSettingId"];
                }
                else if (HttpContext.Current.Session["ChangingSiteSettingId"] != null)
                    result = (bool)HttpContext.Current.Session["ChangingSiteSettingId"];

                return result;
            }
            set
            {
                if (HttpContext.Current.Session == null)
                    HttpContext.Current.Items["ChangingSiteSettingId"] = value;
                else
                    HttpContext.Current.Session["ChangingSiteSettingId"] = value;
            }
        }

        public UnitOfWork ChangeSiteSettingId(Guid ssid, UnitOfWork old)
        {
            if (old != null)
            {
                old.Save();
            }
            _Globals.Instance.CurrentSiteSettingId = ssid;
            if (old != null)
            {
                return new UnitOfWork();
            }
            return null;
        }

    }
}
