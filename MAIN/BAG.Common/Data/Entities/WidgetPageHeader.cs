using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    public class WidgetPageHeader : Widget
    {
        public WidgetPageHeader()
        {
            EnableBackground = true;
            EnableAnimations = true;
            AnimationSpeed = "10";
            Intensity = "100";
            BaseColor = "#EBEBEB";
            UseCustomImage = false;
            CustomImage = "";
            Name = "Page Header";
            WidgetDescription = "Widget for the PageHeader";
        }

        public string Title { get; set; }
        public string Description { get; set; }

        public bool EnableBackground { get; set; }

        public bool EnableAnimations { get; set; }

        public string AnimationSpeed { get; set; }

        public string Intensity { get; set; }

        public string BaseColor { get; set; }

        public bool UseCustomImage { get; set; }

        public string CustomImage { get; set; }

        [XmlIgnore]
        public String FirstNonHeaderUrl { get; set; }

        public override string GetContent()
        {
            return " "+Title + " " + Description+ " ";
        }
    }
}
