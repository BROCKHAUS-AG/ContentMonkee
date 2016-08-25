using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class WidgetsViewModel
    {
        public IEnumerable<Widget> Widgets { get; set; }
        public int HasPrinted { get; set; }
        public string SEOUrl { get; set; }
        public IEnumerable<Guid> NavigationPoints { get; set; }
    }
}