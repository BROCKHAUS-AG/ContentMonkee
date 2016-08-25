using BAG.Common;
using BAG.Common.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;

namespace Default.WebUI.Handler
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler, IReadOnlySessionState
    {

        private UnitOfWork unit = new UnitOfWork();
        public void ProcessRequest(HttpContext context)
        {
            var path = HttpContext.Current.Request.Url.AbsolutePath;
            var patharr = path.Split('.');
            var type = patharr.Last();

            string[] adminimgarr = path.Split('/');
            string adminimgfirst = adminimgarr[1];

            if (adminimgfirst.Equals("img") && adminimgarr.Count() < 4)
            {
                context.Response.ContentType = "image/" + type;
                if (File.Exists(HttpContext.Current.Server.MapPath("~/app_themes/admin/lib/elfile/img/" + adminimgarr.Last())))
                {
                    context.Response.WriteFile("~/app_themes/admin/lib/elfile/img/" + adminimgarr.Last());
                }     
            }
            else if (adminimgfirst.Equals("admin"))
            {
                context.Response.ContentType = "image/" + type;
                context.Response.WriteFile("~/app_themes/admin/css/images/" + adminimgarr.Last());
            }

            var imagepath = ControlFormats(path);
            if (File.Exists(HttpContext.Current.Server.MapPath("~" + imagepath)))
            {

                context.Response.ContentType = "image/" + type;
                context.Response.WriteFile("~" + imagepath);
            }
        }

        private string ControlFormats(string path)
        {
            var sitesetting = unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            var height = 0;
            var width = 0;
            var originpath = path;
            if (path.Contains("-phone."))
            {
                height = sitesetting.ImagePhoneHight;
                width = sitesetting.ImagePhoneWidth;
                originpath = path.Replace("-phone.", ".");
            }
            else if (path.Contains("-tablet."))
            {
                height = sitesetting.ImageTabletHight;
                width = sitesetting.ImageTabletWidth;
                originpath = path.Replace("-tablet.", ".");
            }
            if (path.Contains("-desktop."))
            {
                height = sitesetting.ImageDesktopHight;
                width = sitesetting.ImageDesktopWidth;
                originpath = path.Replace("-desktop.", ".");
            }

            if (width <= 0 || height <= 0)
            {
                return originpath;
            }
            else
            {
                return GetImage(originpath, path, height, width);
            }

        }

        private string GetImage(string originpath, string path, int height, int width)
        {
            try
            {
                Image origin = Image.FromFile(HostingEnvironment.MapPath(originpath));
                Bitmap newBitmap = new Bitmap(origin, new Size(width, height));
                newBitmap.Save(HostingEnvironment.MapPath(path));
                return path;
            }
            catch
            {
                throw new HttpException(404, "Not found");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}