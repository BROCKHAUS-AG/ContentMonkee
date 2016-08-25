using BAG.Common;
using BAG.Common.Authorization;
using BAG.Common.Data;
using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Default.WebUI.Classes
{
    /// <summary>
    /// Gernierung der sitemap.xml.
    /// </summary>
    [WebAnonymousAttribute()]
    public class SitemapGenerator
    {
        private UnitOfWork unit = new UnitOfWork();
        public string getSitemap()
        {
            new AuthorizationRules().Anonymous();
            var domainname = HttpContext.Current.Request.Url.Authority;
            var domainscheme = HttpContext.Current.Request.Url.Scheme;
            var domain = domainscheme + "://" + domainname + "/";
            var sitesetting = unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            var sitemanagers = unit.SiteManagerRepository.Get(s => s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId && s.Release != null);
            var sites = new List<Site>();
            foreach (var sitemanager in sitemanagers)
            {
                sites.Add(sitemanager.Release);
            }

            XmlDocument sitemap = new XmlDocument();
            XmlDeclaration sitemapdec = sitemap.CreateXmlDeclaration("1.0", null, null);
            sitemapdec.Encoding = "UTF-8";

            XmlElement root = sitemap.CreateElement("urlset");
            root.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            sitemap.AppendChild(root);

            foreach (var site in sites)
            {
                site.OnLoad(unit);

                if (!site.Visible || site.SiteSettingId != _Globals.Instance.CurrentSiteSettingId)
                {
                    continue;
                }
                var wnUrl = "";

                XmlElement url = sitemap.CreateElement("url");

                XmlElement loc = sitemap.CreateElement("loc");
                XmlElement lastmod = sitemap.CreateElement("lastmod");
                XmlElement freq = sitemap.CreateElement("changefreq");
                XmlElement prio = sitemap.CreateElement("priority");

                loc.InnerText = domain + site.Url;
                lastmod.InnerText = site.LastChange.ToString("yyyy-MM-dd");
                freq.InnerText = "daily";
                prio.InnerText = site.Priority.ToString();

                url.AppendChild(loc);
                url.AppendChild(lastmod);
                url.AppendChild(freq);
                url.AppendChild(prio);

                root.AppendChild(url);
                foreach (var widgetpart in site.Widgets)
                {
                    var widget = widgetpart.Release;
                    if (widget.GetType().Name == "WidgetPageHeader" || widget.GetType().Name == "WidgetPageFooter")
                    {
                        continue;
                    }


                    var wUrl = widget.Url == null ? string.Empty : widget.Url;
                    var sUrl = string.IsNullOrWhiteSpace(site.Url) ? string.Empty : site.Url + "/";
                    if (site.WidgetNavigations.Contains(widget.Id))
                    {
                        wnUrl = "";
                    }

                    url = sitemap.CreateElement("url");
                    loc = sitemap.CreateElement("loc");
                    lastmod = sitemap.CreateElement("lastmod");
                    freq = sitemap.CreateElement("changefreq");
                    prio = sitemap.CreateElement("priority");

                    loc.InnerText = domain + sUrl + wnUrl + wUrl + "/";
                    lastmod.InnerText = site.LastChange.ToString("yyyy-MM-dd");
                    freq.InnerText = "daily";
                    prio.InnerText = widget.MetaPriority.ToString();

                    url.AppendChild(loc);
                    url.AppendChild(lastmod);
                    url.AppendChild(freq);
                    url.AppendChild(prio);

                    root.AppendChild(url);
                    if (site.WidgetNavigations.Contains(widget.Id))
                    {
                        wnUrl = wUrl + "/";
                    }


                }
            }
            XmlSerializer xs = new XmlSerializer(typeof(XmlDocument));
            var encoding = Encoding.GetEncoding("UTF-8");
            StringBuilder sb = new StringBuilder();
            using (StringWriterUtf8 sw = new StringWriterUtf8(sb))
            {
                var test = sw.Encoding;
                xs.Serialize(sw, sitemap);
                sw.Close();
            }
            
            return sb.ToString();
        }
    }
}