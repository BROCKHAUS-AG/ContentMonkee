using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class GlobalViewModel
    {
        public GlobalViewModel()
        {
            Results = new List<SearchResult>();
            Site = new BAG.Common.Data.Entities.Site();
        }
        public List<SearchResult> Results { get; set; }  
        public BAG.Common.Data.Entities.Site Site { get; set; }
        public string SEOSiteUrl { get; set; }
        public string SEODeepUrl { get; set; }
        public List<BAG.Common.Data.Entities.Site> Sites { get; set; }
        public BAG.Common.Data.Entities.SiteSetting SiteSetting { get; set; }

        public Guid RequestedWidgetId { get; set; }

        public bool IsPreview { get; set; }


        public Widget GetVersion(WidgetManager wm)
        {
            return this.IsPreview ? wm.PreRelease : wm.Release;
        }
        public Site GetVersion(SiteManager sm)
        {
            return this.IsPreview ? sm.PreRelease : sm.Release;
        }



    }
    public class SearchResult
    {
        public BAG.Common.Data.Entities.Site Site { get; set; }
        public int Quality { get; set; }
    }
}