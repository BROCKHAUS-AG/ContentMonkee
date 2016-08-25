using BAG.Common.Data.Entities;
using System.Collections.Generic;

namespace Default.WebUI.Models
{
    public interface ISiteSelection
    {
        List<BAG.Common.Data.Entities.SiteSetting> SiteSettings { get; set; }
        SiteSetting CurrentSiteSetting { get; set; }
    }
}