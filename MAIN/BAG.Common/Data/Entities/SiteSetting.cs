using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    public class SiteSetting : Base
    {
        public SiteSetting()
            : base()
        {

        }

        public string Author { get; set; }
        [AllowHtml]
        public string CookieDescription { get; set; }

        public string Favicon { get; set; }

        /// <summary>
        /// 302 redirect by 404
        /// </summary>
        public string Fallback { get; set; }

        /// <summary>
        /// www.brockhaus-ag.de,*
        /// </summary>
        public string Bindings { get; set; }
        /// <summary>
        /// Main Domain for all subdomain
        /// </summary>
        public string MainDomain { get; set; }

        public bool IsDefault { get; set; }
        public bool IsPortalEnabled { get; set; }
        public string Name { get; set; }

        //public string Controller { get; set; }
        //public string Action { get; set; }
        public string Type { get; set; }
        public string Template { get; set; }

        public List<Redirection> Redirections { get; set; }
        public List<Guid> SiteNavigations { get; set; }
        public List<Guid> SiteIds { get; set; }
        public string[] WidgetsList { get; set; }
        public string Robots { get; set; }
        public bool HideWidgetsToolbar { get; set; }

        public string SiteStructureType { get; set; }
        public string SiteStructureName { get; set; }
        public string SiteStructureUrl { get; set; }
        public string SiteStructureLogo { get; set; }
        public string SiteStructureStreetAddress { get; set; }
        public string SiteStructureAddressLocality { get; set; }
        public string SiteStructureAddressRegion { get; set; }
        public string SiteStructurePostalCode { get; set; }
        public string SiteStructureAddressCountry { get; set; }
        public string SiteStructureGeoLatitude { get; set; }
        public string SiteStructureGeoLongitude { get; set; }
        public string SiteStructureTelephone { get; set; }
        public string SiteStructureContactType { get; set; }
        public string SiteStructureOpens { get; set; }
        public string SiteStructureCloses { get; set; }

        public int ImagePhoneHight { get; set; }
        public int ImagePhoneWidth { get; set; }
        public int ImageTabletHight { get; set; }
        public int ImageTabletWidth { get; set; }
        public int ImageDesktopHight { get; set; }
        public int ImageDesktopWidth { get; set; }
        public string LogoImage { get; set; }
        public bool IpAnonymization { get; set; }
        public string GoogleAnalyticTrackingId { get; set; }
        public string TrackingCode { get; set; }
        public string FontsCSS { get; set; }

        public override List<BaseEntity> RemapAllGuids(Func<Guid, bool, Guid> map)
        {
            base.RemapAllGuids(map);
            SiteIds = SiteIds.Select(s => map(s, false)).ToList();
            SiteNavigations = SiteNavigations.Select(s => map(s, false)).ToList();
            return null;
        }

    }
}
