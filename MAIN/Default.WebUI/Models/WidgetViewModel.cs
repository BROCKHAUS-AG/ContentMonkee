using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class WidgetViewModel
    {
        public string SocialUrl { get; set; }
        public Widget Widget { get; set; }
        public bool IsNavPoint { get; set; }
        public Guid SiteId { get; set; }        
    }
}