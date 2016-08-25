using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class WidgetContainer : IWidgetContainer
    {
        Guid siteId;
        public List<Guid> Widgetnavigation { get; set; }

        public Guid SiteId
        {
            get { return siteId; }
        }

        public WidgetContainer(IEnumerable<WidgetManager> wms)
        {
            siteId = Guid.Empty;
            WidgetManagers = wms.ToList();
            Widgets = WidgetManagers.Select(wm => wm.PreRelease).ToList();
        }

        public WidgetContainer(Site site)
        {
            if (site == null)
            {
                return;
            }
            siteId = site.Id;
            Widgetnavigation = site.WidgetNavigations;
            var widgetList = new List<Widget>();
            foreach (var widgetpart in site.Widgets)
            {
                widgetList.Add(widgetpart.PreRelease);
            }
            WidgetManagers = site.Widgets;
            Widgets = widgetList;
        }
        public WidgetContainer(IEnumerable<Site> sites)
        {
            if (sites == null)
            {
                return;
            }
            siteId = Guid.Empty;

            IEnumerable<WidgetManager> wmList = new List<WidgetManager>();
            foreach (Site site in sites)
            {
                wmList = wmList.Union(site.Widgets);
            }
            WidgetManagers = wmList.ToList();
            Widgets = WidgetManagers.Select(wm => wm.PreRelease).ToList();
        }

        public List<WidgetManager> WidgetManagers { get; set; }

        public List<BAG.Common.Data.Entities.Widget> Widgets
        {
            get;
            set;
        }
    }
}