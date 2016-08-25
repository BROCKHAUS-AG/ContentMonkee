using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{

    public class WidgetMap : Widget
    {
        public WidgetMap()
        {
            Name = "Map";
            WidgetDescription = "Widget for Google Map";
        }

        public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Zoom { get; set; }
        public string Content { get; set; }
        public bool ControlZoom { get; set; }
        public bool ControlScale { get; set; }
        public bool Scrollwheel { get; set; }

        public override string GetContent()
        {
            return " "+Content+ " "+Location+ " ";
        }

    }
}
