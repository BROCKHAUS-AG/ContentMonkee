using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{
    public class WidgetList : Widget
    {
        public WidgetList()
        {
            Name = "List";
            WidgetDescription = "Special Type of List";
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }

        public override string GetContent()
        {
            return " "+Title + " " + Description+ " ";
        }
    }
}
