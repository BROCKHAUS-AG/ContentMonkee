using BAG.Common;
using BAG.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Default.WebUI.Classes
{
    public class ContentPath
    {
        private UnitOfWork unit = new UnitOfWork();
        public string getPath()
        {
            var userdate = unit.UserRepository.GetByID(_Globals.Instance.CurrentLoginUserId);

            if (userdate.IsAdmin)
            {
                var path = HttpContext.Current.Server.MapPath("~/Content/" + _Globals.Instance.CurrentSiteSettingId);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }else
            {
                var path = HttpContext.Current.Server.MapPath("~/Content/" + _Globals.Instance.CurrentSiteSettingId );
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }

        }
    }
}