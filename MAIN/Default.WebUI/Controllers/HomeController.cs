using BAG.Common.Data;
using Default.WebUI.Models;
using System.Web.Mvc;
using System.Linq;
using BAG.Common.Authorization;
using BAG.Common;
using System.Collections.Generic;
using BAG.Common.Data.Entities;
using System;
using System.Web;
using System.Text.RegularExpressions;

namespace Default.WebUI.Controllers
{
    [WebAnonymousAttribute()]
    public class HomeController : Controller
    {
        [Obsolete]
        private UnitOfWork unit = null;
        private UnitOfWork Unit
        {
            get
            {
                if (unit == null)
                {
                    this.unit = new UnitOfWork();
                }
                return this.unit;
            }
            set
            {
                this.unit = Unit;
            }
        }


        private UrlMapper UrlMapper = null;

        public ActionResult Error(string errorcode = "defaultErrorCode")
        {
            return File(Server.MapPath("~/Views/" + errorcode + ".html"), "text/html");
        }

        //START
        public ActionResult Index(string SEOUrl = "/")
        {
            SEOUrl = Regex.Replace("/" + SEOUrl.ToLower() + "/", @"[/]+", "/");
            GlobalViewModel model = null;
            try
            {
                model = getModel(SEOUrl);
            }
            catch (Exception)
            {

                try
                {
                    ActionResult actionResult = null;
                    if ((actionResult = RedirectIfRequired(SEOUrl)) != null)
                    {
                        return actionResult;
                    }
                }
                catch (Exception)
                {
                    if (SEOUrl == "/")
                    {
                        return new HttpNotFoundResult("Sorry, page not found!");
                    }
                    return this.Index("/");
                }
            }
            {
                ActionResult actionResult = null;
                if ((actionResult = RedirectIfSiteSecured(UrlMapper.Site)) != null)
                {
                    return actionResult;
                }
            }
            
            return View(model);
        }

        private GlobalViewModel getModel(string SEOUrl)
        {
            var model = new GlobalViewModel();

            Uri baseUri = Url.RequestContext.HttpContext.Request.Url;
            string baseUrl = baseUri.LocalPath.Length <= 1 ? baseUri.AbsoluteUri : (baseUri.AbsoluteUri.Replace(baseUri.LocalPath, "") + "/");
            UrlMapper = new UrlMapper(Unit, new Uri(baseUrl), SEOUrl);

            if (!UrlMapper.IsValide)
            {
                throw new Exception();
            }

            model.SiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);

            FillModelWithSiteInfo(model);
            FillModelWithNavInfo(model, UrlMapper.Site.Url);

            // 

            if (UrlMapper.Widget != null && UrlMapper.Widget.GetType().Name == "WidgetPageHeader")
            {
                WidgetPageHeader wHead = (UrlMapper.Widget as WidgetPageHeader);
                var ws = UrlMapper.Site.Widgets.Where(w => w.Id != wHead.Id);
                wHead.FirstNonHeaderUrl = ws.Count() == 0 ? "/" : UrlMapper.GetVersion(ws.First()).Url;
                wHead.FirstNonHeaderUrl = Regex.Replace("/" + UrlMapper.SiteUrl + "/" + wHead.FirstNonHeaderUrl + "/", @"[/]+", "/");


            }
            model.IsPreview = UrlMapper.IsPreview;

            return model;
        }


        private void FillModelWithNavInfo(GlobalViewModel model, string SEOUrl)
        {
            model.Sites = Unit.SiteManagerRepository.Get(s => s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId).Where(sm => UrlMapper.GetVersion(sm) != null).Select(sm => UrlMapper.GetVersion(sm)).ToList();
            foreach (var site in model.Sites)
            {
                site.OnLoad(Unit);
            }
        }

        private void FillModelWithSiteInfo(GlobalViewModel model)
        {
            model.Site = UrlMapper.Site;
            model.SEOSiteUrl = model.Site.Url;
            model.SEOSiteUrl = UrlMapper.SiteUrl;
            model.SEODeepUrl = UrlMapper.DeepUrl;
            model.RequestedWidgetId = UrlMapper.Widget == null ? Guid.Empty : UrlMapper.Widget.Id;
        }

        private ActionResult RedirectIfRequired(string SEOUrl)
        {
            IEnumerable<string> requestPaths = SEOUrl.Split('/').Where(s => !string.IsNullOrWhiteSpace(s));
            if (requestPaths.Count() <= 0)
            {
                return null;
            }
            string requestPath = requestPaths.First();

            List<Redirection> redirections = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId).Redirections;
            var newUrls = from redirection in redirections
                          where !string.IsNullOrEmpty(redirection.oldSEOUrl) &&
                                 !string.IsNullOrEmpty(redirection.newSEOUrl) &&
                                 Regex.Replace(redirection.oldSEOUrl, @"[/]+", string.Empty).Trim().Equals(requestPath)
                          select redirection.newSEOUrl;

            if (newUrls == null || newUrls.Count() <= 0)
            {
                if (!UrlMapper.IsValide)
                {
                    throw new Exception();
                }
                if (UrlMapper.Widget != null && UrlMapper.Widget.GetType().Name.Equals("WidgetPageHeader"))
                {
                    return Redirect(Regex.Replace("/" + UrlMapper.SiteUrl, @"[/]+", "/") + HttpContext.Request.Url.Query);
                }
                if (!UrlMapper.DeepUrl.Equals(SEOUrl))
                {
                    return Redirect(UrlMapper.DeepUrl + HttpContext.Request.Url.Query);
                }
                return null;
            }
            return Redirect(newUrls.First() + HttpContext.Request.Url.Query);
        }

        private ActionResult RedirectIfSiteSecured(Site s)
        {

            if (!s.HasPassword || s.SitePassword == AccountController.GetSitePasswordFromCookie(HttpContext, _Globals.Instance.CurrentSiteSettingId, s.Id))
            {
                return null;
            }

            return Redirect("~/Account/SiteVerifyPassword/" + s.Id.ToString());
        }

        // infiniteContent
        public ActionResult GetWidgetByFullUrl(string SEOUrl = null)
        {
            SEOUrl = Regex.Replace("/" + SEOUrl.ToLower() + "/", @"[/]+", "/");

            GlobalViewModel homeModel = null;
            try
            {
                homeModel = this.getModel(SEOUrl);
            }
            catch (Exception)
            {
                return Content(string.Empty);
            }



            ActionResult actionResult = null;
            if ((actionResult = RedirectIfSiteSecured(UrlMapper.Site)) != null)
            {
                throw new Exception("access denied");
            }

            WidgetViewModel widgetModel = new WidgetViewModel();
            widgetModel.Widget = UrlMapper.Widget == null ? UrlMapper.GetVersion(homeModel.Site.Widgets.Where(wm => UrlMapper.GetVersion(wm) != null).First()) : UrlMapper.Widget;
            widgetModel.IsNavPoint = homeModel.Site.WidgetNavigations.Contains(widgetModel.Widget.Id);
            widgetModel.SiteId = homeModel.Site.Id;

            widgetModel.SocialUrl = UrlMapper.FlatUrl;

            return PartialView("_Widget", widgetModel);
        }



    }

    class UrlMapper : Uri
    {

        private UnitOfWork Unit = null;
        public Widget Widget { get; private set; }
        public Site Site { get; private set; }

        public string FlatUrl
        {
            get
            {
                return GetFlatUrl();
            }
        }

        public string DeepUrl
        {
            get { return GetDeepUrl(); }
        }

        public string SiteUrl { get; private set; }
        public string WidgetUrl { get; private set; }

        public bool IsValide { get; private set; }
        public string FullBinding { get; private set; }

        public bool IsPreview { get; private set; }

        public Dictionary<string, string> QueryMap { get; }

        public UrlMapper(UnitOfWork unit, Uri baseUri, string relativeUrl) : base(baseUri, relativeUrl)
        {
            QueryMap = new Dictionary<string, string>();
            SetQuerMap();
            SetPreviewGetUrl(relativeUrl);
            Unit = unit;
            FullBinding = this.AbsoluteUri.Replace(this.LocalPath, "");
            Site = null;
            Widget = null;
            WidgetUrl = string.Empty;
            SiteUrl = string.Empty;
            IsValide = true;
            EvaluateUrl(relativeUrl);


            if (this.Widget != null && !this.Widget.Visible)
            {
                this.IsValide = false;
            }

            SiteUrl = "";
            WidgetUrl = "";
            if (Site != null && !string.IsNullOrWhiteSpace(Site.Url))
            {
                SiteUrl = ("/" + Site.Url + "/").Replace("//", "/");
            }
            if (Widget != null && !string.IsNullOrWhiteSpace(Widget.Url))
            {
                WidgetUrl = ("/" + Widget.Url + "/").Replace("//", "/");
            }
        }

        private void SetQuerMap()
        {
            string query = HttpContext.Current.Request.Url.Query;
            string[] queries = query.Replace("?", "").Split('&');
            string[] kv = null;
            string key = null;
            string value = null;
            foreach (string q in queries)
            {
                kv = q.Split('=');
                if (kv.Count() == 0)
                {
                    continue;
                }
                key = kv[0];
                value = kv.Count() > 1 ? kv[1] : null;
                QueryMap.Add(key, value);
            }
        }

        private void SetPreviewGetUrl(string url)
        {
            string value;
            this.IsPreview = QueryMap.TryGetValue("preview", out value);
        }

        private void EvaluateUrl(string relativeUrl)
        {
            string[] urls = relativeUrl.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (urls.Count() <= 0)
            {
                Site = GetSiteFromUrl(null);
                if (Site == null)
                {
                    IsValide = false;
                }
                return;
            }

            Site = GetSiteFromUrl(null);

            Widget = GetWidgetFromUrl(Site, urls[0]);

            if (Widget == null)
            {
                Site = GetSiteFromUrl(urls[0]);
                urls = urls.Skip(1).ToArray();
                if (Site == null)
                {
                    IsValide = false;
                    return;
                }
            }

            foreach (string url in urls)
            {
                Widget w = GetWidgetFromUrl(Site, url);
                if (w != null)
                {
                    Widget = w;
                }
                else
                {
                    if (Widget == null)
                    {
                        IsValide = false;
                        return;
                    }
                    return;
                }
            }
        }

        private Site GetSiteFromUrl(string url)
        {
            url = string.IsNullOrWhiteSpace(url) ? string.Empty : url;
            SiteManager sitemanager = Unit.SiteManagerRepository.Find(s => GetVersion(s) != null && GetVersion(s).Url == url && s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId);

            if (sitemanager != null)
            {
                GetVersion(sitemanager).OnLoad(Unit);
                return GetVersion(sitemanager);
            }
            return null;
        }
        private Widget GetWidgetFromUrl(Site site, string url)
        {
            if (site == null)
            {
                return null;
            }
            url = string.IsNullOrWhiteSpace(url) ? string.Empty : url;
            WidgetManager widgetManager = null;

            widgetManager = site.Widgets.Find(wm => GetVersion(wm) != null && GetVersion(wm).Url == url);


            if (widgetManager != null)
            {
                GetVersion(widgetManager).OnLoad(Unit);
                return GetVersion(widgetManager);
            }
            return null;
        }
        private string GetFlatUrl()
        {
            return ("/" + SiteUrl + WidgetUrl + "/").Replace("//", "/");
        }


        private string GetDeepUrl()
        {
            if (!IsValide)
            {
                return "/";
            }
            if (Widget == null || Site == null || Site.WidgetNavigations.Contains(Widget.Id))
            {
                return FlatUrl;
            }
            Widget parent = null;
            foreach (WidgetManager wm in Site.Widgets)
            {
                Widget w = GetVersion(wm);
                if (w == null)
                {
                    continue;
                }

                if (w.Id == Widget.Id)
                {
                    break;
                }
                if (Site.WidgetNavigations.Contains(w.Id))
                {
                    parent = w;
                }
            }
            if (parent == null)
            {
                return FlatUrl;
            }
            return Regex.Replace("/" + SiteUrl + "/" + parent.Url + "/" + WidgetUrl + "/", @"[/]+", "/");
        }
        public Widget GetVersion(WidgetManager wm)
        {
            return this.IsPreview ? wm.PreRelease : wm.Release;
        }
        public Site GetVersion(SiteManager sm)
        {
            return this.IsPreview ? sm.PreRelease : sm.Release;
        }

    }
}