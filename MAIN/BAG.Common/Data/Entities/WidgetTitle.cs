using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{
    public class WidgetTitle : Widget
    {
        public WidgetTitle()
            : base()
        {
            Name = "Title";
            WidgetDescription = "Widget for special Title";
        }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public bool UseIcon { get; set; }
        public string IconName { get; set; }
        public bool UseFile { get; set; }
        public string FileName { get; set; }

        public override string GetContent()
        {
            return " "+Title + " " + SubTitle+ " ";
        }
    }
}
