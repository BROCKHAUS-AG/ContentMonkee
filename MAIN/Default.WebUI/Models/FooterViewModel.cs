using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Default.WebUI.Models
{
    public class FooterViewModel : WidgetViewModel
    {
        public List<Site> Sites { get; set; }
        public Site Site { get; set; }
        public SiteSetting SiteSetting { get; set; }
        public bool IsDesignMode { get; set; }

        public bool IsPreview { get; set; }

        public List<String> GetWidgetsUrls()
        {
            List<String> widgetUrls = new List<string>();
            String parentUrl = string.Empty;
            foreach (WidgetManager wm in Site.Widgets)
            {
                Widget w = wm.Release;
                string url = "/" + Site.Url + "/";
                if (Site.WidgetNavigations.Contains(w.Id))
                {
                    parentUrl = w.Url;
                }
                else
                {
                    url += parentUrl + "/";
                }
                url += w.Url + "/";
                widgetUrls.Add(Regex.Replace("/" + url + "/", @"[/]+", "/"));
            }
            return widgetUrls;
        }

        public List<String> GetSitesUrls()
        {
            return Sites.Select(s => ("/" + s.Url + "/")).ToList();
        }
    }
}