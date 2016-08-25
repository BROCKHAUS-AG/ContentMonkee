using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class SiteVerifyPasswordModel
    {
        public Guid SiteSettingId { get; set; }
        public Guid SiteId { get; set; }
    }
}