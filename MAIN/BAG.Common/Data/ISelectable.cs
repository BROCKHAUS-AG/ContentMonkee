using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data
{
    public interface ISelectable
    {
        Guid Id { get; set; }

        string Title { get; set; }

        string Image { get; set; }

        bool IsSelected { get; set; }
    }
}
