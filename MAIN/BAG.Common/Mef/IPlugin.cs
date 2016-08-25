using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Mef
{
    public interface IPlugin
    {
        string Name { get; set; }

        Version Version { get; set; }

        void Install(string landingPageName);
        void UnInstall(string landingPageName);
    }
}
