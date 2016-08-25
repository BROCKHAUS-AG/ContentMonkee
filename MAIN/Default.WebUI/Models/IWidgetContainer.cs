using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Default.WebUI.Models
{
    public interface IWidgetContainer
    {
        Guid SiteId { get;}
        List<Widget> Widgets { get; set; }
    }
}
