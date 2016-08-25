using System;
using System.IO;
using System.Web.Mvc;
using BAG.Framework.ElFinder;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using Default.WebUI.Classes;

namespace Default.WebUI.Controllers
{
    public class FileController : Controller
    {


        private Connector _connector;

        public Connector Connector
        {
            get
            {
                if (_connector == null)
                {
                    var path = new ContentPath();
                    FileSystemDriver driver = new FileSystemDriver();
                    DirectoryInfo thumbsStorage = new DirectoryInfo(Server.MapPath("~/Content"));
                    //driver.AddRoot(new Root(new DirectoryInfo(@"C:\Program Files"))
                    //{
                    //    IsLocked = true,
                    //    IsReadOnly = true,
                    //    IsShowOnly = true,
                    //    //ThumbnailsStorage = thumbsStorage,
                    //    //ThumbnailsUrl = "Thumbnails/"
                    //});
                    driver.AddRoot(new Root(new DirectoryInfo(path.getPath()), "http://" + Request.Url.Authority + "/Content/")
                    {
                        Alias = "Bibliothek",
                        StartPath = new DirectoryInfo(path.getPath()),
                        ThumbnailsStorage = thumbsStorage,
                        MaxUploadSizeInMb = 1,

                        ThumbnailsUrl = "http://" + Request.Url.Authority + "/thumbnails/"
                    });
                    _connector = new Connector(driver);
                }
                return _connector;
            }
        }

        public virtual ActionResult Index(string folder, string subFolder)
        {
            try
            {
                //    FileSystemDriver driver = new FileSystemDriver();

                //    var root = new Root(
                //            new DirectoryInfo(Server.MapPath("~/Content/" + folder)),
                //            "http://" + Request.Url.Authority + "/Content/" + folder)
                //    {
                //        IsReadOnly = false,
                //        Alias = "File",
                //        MaxUploadSizeInKb = 10000,IsShowOnly = false, IsLocked = false

                //    };

                //    if (!string.IsNullOrEmpty(subFolder))
                //    {
                //        root.StartPath = new DirectoryInfo(Server.MapPath("~/Content/" + folder + "/" + subFolder));
                //    }

                //    driver.AddRoot(root);

                //    var connector = new Connector(driver);

                //    return connector.Process(this.HttpContext.Request);
                return Connector.Process(this.HttpContext.Request);

            }
            catch (InvalidCastException)
            {
                return HttpNotFound();
            }
        }

        public virtual ActionResult SelectFile(string target)
        {

            return Json(Connector.GetFileByHash(target).FullName);

        }


        public ActionResult Thumbs(string tmb)
        {
            return Connector.GetThumbnail(Request, Response, tmb);
        }

        public ActionResult Thumbnails(string id)
        {
            return Connector.GetThumbnail(Request, Response, id);
        }
        
    }
}