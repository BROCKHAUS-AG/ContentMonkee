using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{
    public class WidgetContent : Widget
    {
        public WidgetContent()
        {
            Name = "Content";
            WidgetDescription = "Widget for content, image and tables";
        }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Class { get; set; }
        public override string GetContent()
        {
            return " "+Content+ " ";
        }
    }

}
