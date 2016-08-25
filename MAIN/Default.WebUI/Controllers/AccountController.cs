using BAG.Common;
using BAG.Common.Authorization;
using BAG.Common.Data;
using BAG.Common.Data.Entities;
using Default.WebUI.Extensions;
using Default.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Default.WebUI.Controllers
{
    [WebAnonymousAttribute()]
    public class AccountController : Controller
    {
        // GET: Login
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            UnitOfWork unit = new UnitOfWork(); ;
            var email = form["email"];
            var password = form["password"];

            if (string.IsNullOrEmpty(email))
                return Json("error-user");

            email = email.ToLowerInvariant();
            if (new AuthorizationRules().LoginByForm(email, password))
            {
                return RedirectToAction("Index", "Admin", new { username = _Globals.Instance.CurrentLoginUserName });
            }
            ModelState.AddModelError("login", "Es konnte kein Benutzer mit diesen Zugangsdaten gefunden werden. Bitte versuchen Sie es nochmal.");
            return View();
        }


        public ActionResult Logout()
        {
            new AuthorizationRules().Logout();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SetCookie()
        {

            _Globals.Instance.EnableCookies = true;
            new AuthorizationRules().SetSiteSettingIdFromBinding(HttpContext.Request.Url);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SiteVerifyPassword(string sid)
        {
            if (string.IsNullOrEmpty(sid))
            {
                return HttpNotFound();
            }
            if (!_Globals.Instance.EnableCookies)
            {
                SetCookie();
            }
            SiteVerifyPasswordModel model = new SiteVerifyPasswordModel();
            model.SiteId = Guid.Parse(sid);
            model.SiteSettingId = _Globals.Instance.CurrentSiteSettingId;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SiteLogin(FormCollection form)
        {

            string pw = form.GetString("pw", string.Empty);
            string md5 = BAG.Common.Data.Cryptography.Hash.ComputeHash(pw);

            Guid sid = form.GetGuid("sid", Guid.Empty);
            Guid ssid = form.GetGuid("ssid", Guid.Empty);

            UnitOfWork unit = new UnitOfWork();

            HttpCookie cookie = new HttpCookie("SPW_" + ssid.ToString() + "_" + sid.ToString(), md5)
            {
                Expires = DateTime.Now.AddDays(1)
            };
            HttpContext.Response.Cookies.Set(cookie);
            Site site = unit.SiteManagerRepository.Find(sm => sm.Id == sid).Release;
            string url = Regex.Replace("/" + site.Url, @"[/]+", "/");
            return Redirect("~/" + url);
        }

        internal static string GetSitePasswordFromCookie(HttpContextBase hcb, Guid ssid, Guid sid)
        {
            string cookieName = "SPW_" + ssid.ToString() + "_" + sid.ToString();
            if (hcb.Request.Cookies.AllKeys.Contains(cookieName))
            {
                return hcb.Request.Cookies[cookieName].Value;
            }
            return string.Empty;
        }
    }
}