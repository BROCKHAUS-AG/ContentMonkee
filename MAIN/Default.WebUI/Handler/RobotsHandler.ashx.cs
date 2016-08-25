using BAG.Common;
using BAG.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Default.WebUI.Handler
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für RobotsHandler
    /// </summary>
    public class RobotsHandler : IHttpHandler, IReadOnlySessionState
    {
        private UnitOfWork unit = new UnitOfWork();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "Text/unformatiert";
            context.Response.Write(getRobots());
        }

        private string getRobots()
        {
            var domainhost = HttpContext.Current.Request.Url.Authority;
            var domainscheme = HttpContext.Current.Request.Url.Scheme;
            var sitemaplink = domainscheme + "://" + domainhost + "/sitemap.xml";
            var sitesetting = unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            var robots = sitesetting.Robots;
            robots = robots + Environment.NewLine + "Sitemap: " + sitemaplink;

            return robots;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}