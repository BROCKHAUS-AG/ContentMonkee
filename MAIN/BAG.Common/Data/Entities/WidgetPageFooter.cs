using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{
    public class WidgetPageFooter : Widget
    {
        public WidgetPageFooter()
            : base()
        {
            Name = "Page Footer";
            WidgetDescription = "Widget for the Footer";
        }

        public string XingUrl { get; set; }
        public string BlogUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string MeinpraktikumUrl { get; set; }
        public string GitHubUrl { get; set; }

        public override string GetContent()
        {
            return " ";
        }
    }
}
