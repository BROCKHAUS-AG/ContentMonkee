using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class WidgetWrapperViewModel
    {
        public WidgetWrapperViewModel(Widget widget, string prefix = null)
        {
            this.Prefix = prefix;
            this.Widget = widget;
        }
        public string Prefix { get; set; }
        public Widget Widget { get; set; }
        public bool IsDesignMode { get; set; }
        
    }
}