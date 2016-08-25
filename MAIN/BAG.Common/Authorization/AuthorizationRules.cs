using BAG.Common.Data;
using BAG.Common.Data.Cryptography;
using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace BAG.Common.Authorization
{
    public class AuthorizationRules
    {
        string LoginPage = "~/account";


        public bool CheckCookie(bool redirect = false)
        {
            if (!_Globals.Instance.IsAuthenticated)
            {
                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("user"))
                {
                    var cookie = HttpContext.Current.Request.Cookies["user"];
                    var splitResult = cookie.Value.Split(':');
                    if (splitResult.Length == 2)
                    {
                        var user = splitResult[0];
                        var password = splitResult[1];
                        if (LoginByCookie(user, password))
                        {
                            return true;
                        }
                    }
                }
                if (redirect)
                    HttpContext.Current.Response.Redirect(LoginPage);
                return false;
            }
            return true;
        }


        public bool LoginByForm(string username, string password)
        {
            UnitOfWork unit = new UnitOfWork();
            IEnumerable<User> users = null;
            if (username.Contains("@"))
            {
                users = unit.UserRepository.Get(u => string.Compare(u.Email, username, true) == 0);
            }
            else
            {
                users = unit.UserRepository.Get(u => string.Compare(u.FirstName + "." + u.LastName, username, true) == 0 ||
                                                     string.Compare(u.UserName, username, true) == 0);
            }

            if (!string.IsNullOrEmpty(password) && users.Count() == 1)
            {
                var unitUser = users.FirstOrDefault();
                if (unitUser != null)
                {
                    if (unitUser.Password.CompareTo(Hash.ComputeHash(password)) == 0)
                    {
                        Login(unitUser);
                        unitUser.LastLogin = DateTime.Now;
                        unit.UserRepository.Update(unitUser);
                        unit.Save();
                        return true;
                    }
                }
            }
            return false;
        }

        bool LoginByCookie(string username, string passwordHash)
        {
            UnitOfWork unit = new UnitOfWork();
            IEnumerable<User> users = null;
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(passwordHash))
            {
                users = unit.UserRepository.Get(u => String.Compare(u.Email, username, true) == 0 &&
                                                     u.Password == passwordHash);
            }
            var unitUser = users.FirstOrDefault();
            if (unitUser != null)
            {
                Login(unitUser);
                unitUser.LastLogin = DateTime.Now;
                unit.UserRepository.Update(unitUser);
                unit.Save();
                return true;
            }

            return false;
        }

        void Login(User unitUser)
        {
            if (unitUser == null)
                return;
            // Create generic identity.
            var identity = new GenericIdentity(unitUser.UserName);
            identity.AddClaim(new System.Security.Claims.Claim("userId", unitUser.Id.ToString("N")));
            // Create generic principal.
            var principal = new GenericPrincipal(identity, new string[] { "User" });
            HttpContext.Current.User = principal;

            _Globals.Instance.CurrentLoginUserId = unitUser.Id;
            _Globals.Instance.CurrentLoginUserName = unitUser.UserName;
            _Globals.Instance.CurrentDisplayName = unitUser.DisplayName;
            _Globals.Instance.CurrentAccountId = unitUser.OwnerId;
            _Globals.Instance.CurrentLanguage = unitUser.Language;
            _Globals.Instance.EnableCookies = true;
            new AuthorizationRules().SetSiteSettingIdFromBinding(HttpContext.Current.Request.Url);

            var expires = DateTime.Now.AddDays(5);
            var user = new HttpCookie("user")
            {
                Expires = expires,
                Value = unitUser.Email + ":" + unitUser.Password
            };
            HttpContext.Current.Response.Cookies.Add(user);
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
            _Globals.Instance.CurrentLoginUserId = Guid.Empty;
            _Globals.Instance.CurrentAccountId = Guid.Empty;
            new AuthorizationRules().SetSiteSettingIdFromBinding(HttpContext.Current.Request.Url);
            if (HttpContext.Current.Request.Cookies["user"] != null)
            {
                var user = new HttpCookie("user")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = null
                };
                HttpContext.Current.Response.Cookies.Set(user);
            }
        }

        public void Anonymous()
        {
            if (_Globals.Instance.CurrentLoginUserId != Guid.Empty)
            {
                _Globals.Instance.EnableCookies = true;
            }

            if (_Globals.Instance.ChangingSiteSettingId)
            {
                SetSiteSettingIdFromBinding(HttpContext.Current.Request.Url);
                _Globals.Instance.ChangingSiteSettingId = false;
            }

            if (!_Globals.Instance.EnableCookies)
            {
                SetSiteSettingIdFromBinding(HttpContext.Current.Request.Url);
            }
        }


        public void SetSiteSettingIdFromBinding(Uri requestUrl)
        {
            _Globals.Instance.ChangeSiteSettingId(GetSiteSettingIdFromBinding(requestUrl), null);
        }

        public Guid GetSiteSettingIdFromBinding(Uri requestUrl)
        {
            GenericRepository<SiteSetting> siteSettings = (new UnitOfWork()).SiteSettingRepository;

            var siteSettingMatches = (new[] { new { SSID = Guid.Empty, SpecificCoefficient = 0f } }).ToList(); // only for type signature
            siteSettingMatches.Clear();

            string host = requestUrl.Host.ToLower().Trim();
            foreach (SiteSetting siteSetting in siteSettings.Get())
            {
                IEnumerable<string> domains = siteSetting.Bindings.Split(',').ToList();
                domains = domains.Union(siteSetting.MainDomain.Split(','));
                domains = domains.Select(d => d.Replace("http://", string.Empty).Replace("https://", string.Empty));
                domains = domains.Where(d => !string.IsNullOrWhiteSpace(d));
                foreach (string domainOriginal in domains)
                {
                    string domain = Regex.Replace(domainOriginal.ToLower().Trim(), @"[*]+", "*");
                    bool hasFirstStar = domain.Length > 0 && domain.First() == '*';
                    bool hasLastStar = domain.Length > 0 && domain.Last() == '*';
                    IEnumerable<string> partials = domain.Split('*').Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => "(" + p + ")");
                    string term = string.Join("(.)*", partials);
                    term = hasFirstStar ? "(.)*" + term : "^" + term;
                    term = hasLastStar ? term + "(.)*" : term + "$";
                    term = Regex.Replace(term, @"(\(\.\)\*)+", "(.)*");

                    if (Regex.IsMatch(host, term))
                    {
                        int domainLength = Regex.Replace(domainOriginal.Trim(), @"[*]+", string.Empty).Length;
                        float coefficient = ((float)domainLength) / host.Length;
                        siteSettingMatches.Add(new { SSID = siteSetting.Id, SpecificCoefficient = coefficient });
                    }
                }
            }
            {
                SiteSetting siteSetting;
                if ((siteSetting = siteSettings.Find(s => s.IsDefault)) != null)
                {
                    siteSettingMatches.Add(new { SSID = siteSetting.Id, SpecificCoefficient = 0f });
                }
                else if (siteSettings.Get().Count() > 0 && (siteSetting = siteSettings.Get().First()) != null)
                {
                    siteSettingMatches.Add(new { SSID = siteSetting.Id, SpecificCoefficient = 0f });
                }
            }

            if (siteSettingMatches.Count <= 0)
            {
                return Guid.Empty;
            }            
            return siteSettingMatches.OrderBy(ssm => ssm.SpecificCoefficient).Last().SSID;
        }

        // 24.08.2016 - Michel jakob - new Binding
        //public void FindSiteSettingId()
        //{
        //    UnitOfWork unit = new UnitOfWork();
        //    bool fromdomain = false;
        //    bool frombinding = false;
        //    var sitesettings = unit.SiteSettingRepository.Get().ToList();
        //    if (sitesettings.Count != 0)
        //    {

        //        foreach (var sitesetting in sitesettings)
        //        {

        //            if (!fromdomain)
        //            {
        //                fromdomain = findbyMainDomain(sitesetting, true);
        //                if (!frombinding)
        //                {
        //                    frombinding = findbyBinding(sitesetting, true);
        //                }
        //            }
        //        }
        //        if (!fromdomain && !frombinding && unit.SiteSettingRepository.Find(s => s.IsDefault) != null)
        //        {
        //            unit = _Globals.Instance.ChangeSiteSettingId(unit.SiteSettingRepository.Find(s => s.IsDefault).Id, unit);
        //        }
        //        else if (!fromdomain && !frombinding && unit.SiteSettingRepository.Find(s => s.IsDefault) == null)
        //        {
        //            unit = _Globals.Instance.ChangeSiteSettingId(unit.SiteSettingRepository.Get().First().Id, unit);
        //        }
        //    }

        //}

        //public bool findbyBinding(SiteSetting sitesetting, bool withtoset)
        //{
        //    var quality = 0;
        //    var domain = HttpContext.Current.Request.Url.Host;
        //    var binding = sitesetting.Bindings.Trim();
        //    if (binding.Contains(','))
        //    {
        //        var bindarr = binding.Split(',');
        //        foreach (var bindpart in bindarr)
        //        {
        //            quality = comparisonQuality(domain, bindpart.Trim());
        //        }
        //    }
        //    else
        //    {
        //        quality = comparisonQuality(domain, binding);
        //    }
        //    if (0 < quality)
        //    {
        //        if (withtoset)
        //        {
        //            _Globals.Instance.ChangeSiteSettingId(sitesetting.Id, null);
        //        }
        //        return true;
        //    }

        //    return false;
        //}

        //public bool findbyMainDomain(SiteSetting sitesetting, bool withtoset)
        //{
        //    var domain = HttpContext.Current.Request.Url.Host;
        //    string maindomain = "";
        //    if (!string.IsNullOrEmpty(sitesetting.MainDomain))
        //    {
        //        maindomain = sitesetting.MainDomain.Trim();
        //    }
        //    if (domain.Equals(maindomain))
        //    {
        //        if (withtoset)
        //        {
        //            _Globals.Instance.ChangeSiteSettingId(sitesetting.Id, null);
        //        }
        //        return true;
        //    }

        //    return false;
        //}

        private int comparisonQuality(string domain, string compare)
        {
            var count = 0;
            if (compare.First().Equals('*') || compare.Last().Equals('*'))
            {
                var comparearr = compare.Trim().Split('.');
                foreach (var part in comparearr)
                {
                    if (domain.Contains(part))
                    {
                        count++;
                    }
                }
            }
            else
            {
                if (domain.Equals(compare))
                {
                    count = 10;
                }
                else if (compare.Contains(domain))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
