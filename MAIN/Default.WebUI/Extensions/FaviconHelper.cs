using BAG.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Default.WebUI.Extensions
{
    public class FaviconHelper
    {
        public FaviconHelper(HttpServerUtilityBase _server, string _favicon)
        {
            Server = _server;
            OriginFaviconPathRelative = _favicon;
            ContentPath = Server.MapPath("~/Content");
            OriginFaviconPath = ContentPath + OriginFaviconPathRelative.Replace("/Content/tcSitesettingID", "/"+ _Globals.Instance.CurrentSiteSettingId.ToString());
            OriginFaviconFileName = Path.GetFileName(OriginFaviconPath);
            NewPath = ContentPath + "/Favicons/" + OriginFaviconFileName;
            if (!OriginFaviconPath.Contains(".svg"))
            {
                OriginFavicon = new Bitmap(OriginFaviconPath);

                CreatePaths();

                foreach (FaviconSize fs in FaviconSizeStorage.Sizes)
                {
                    CreateFavicon(fs.Name, fs.Size, "png");
                }

                CreateFavicon("favicon", 16, "ico");
            }
        }

        public HttpServerUtilityBase Server { get; set; }

        public string OriginFaviconPathRelative { get; set; }

        private string OriginFaviconPath { get; set; }

        private string OriginFaviconFileName { get; set; }

        private string ContentPath { get; set; }

        private string NewPath { get; set; }

        private Bitmap OriginFavicon { get; set; }

        private void CreatePaths()
        {
            if (!Directory.Exists(ContentPath + "/Favicons"))
            {
                Directory.CreateDirectory(ContentPath + "/Favicons");
            }

            if (!Directory.Exists(NewPath))
            {
                Directory.CreateDirectory(NewPath);
            }
        }

        private void CreateFavicon(string name, int size, string format)
        {
            Bitmap img = new Bitmap(OriginFavicon, new Size(size, size));

            string NewImagePath = NewPath + "/" + name + "." + format;

            if (File.Exists(NewImagePath))
            {
                File.Delete(NewImagePath);
            }

            if (format == "png")
            {
                img.Save(NewImagePath, ImageFormat.Png);

                img.Dispose();
            } else if(format == "ico")
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    img.Save(memoryStream, ImageFormat.Png);

                    FileStream filestream = new FileStream(NewImagePath, FileMode.Create);

                    BinaryWriter iconWriter = new BinaryWriter(filestream);
                    if (filestream != null && iconWriter != null)
                    {
                        iconWriter.Write((byte)0);
                        iconWriter.Write((byte)0);
                        iconWriter.Write((short)1);
                        iconWriter.Write((short)1);
                        iconWriter.Write((byte)size);
                        iconWriter.Write((byte)size);
                        iconWriter.Write((byte)0);
                        iconWriter.Write((byte)0);
                        iconWriter.Write((short)0);
                        iconWriter.Write((short)32);
                        iconWriter.Write((int)memoryStream.Length);
                        iconWriter.Write((int)(6 + 16));
                        iconWriter.Write(memoryStream.ToArray());
                        iconWriter.Flush();
                    }
                }

                img.Dispose();
            }
        }
    }

    static class FaviconSizeStorage
    {
        public static List<FaviconSize> Sizes = new List<FaviconSize>();

        static FaviconSizeStorage()
        {
            Sizes.Add(new FaviconSize("ms-icon-310x310", 310));
            Sizes.Add(new FaviconSize("apple-icon", 192));
            Sizes.Add(new FaviconSize("apple-icon-precomposed", 192));
            Sizes.Add(new FaviconSize("android-icon-192x192", 192));
            Sizes.Add(new FaviconSize("apple-icon-180x180", 180));
            Sizes.Add(new FaviconSize("apple-icon-152x152", 152));
            Sizes.Add(new FaviconSize("ms-icon-150x150", 150));
            Sizes.Add(new FaviconSize("android-icon-144x144", 144));
            Sizes.Add(new FaviconSize("apple-icon-144x144", 144));
            Sizes.Add(new FaviconSize("ms-icon-144x144", 144));
            Sizes.Add(new FaviconSize("apple-icon-120x120", 120));
            Sizes.Add(new FaviconSize("apple-icon-114x114", 114));
            Sizes.Add(new FaviconSize("android-icon-96x96", 96));
            Sizes.Add(new FaviconSize("favicon-96x96", 96));
            Sizes.Add(new FaviconSize("apple-icon-76x76", 76));
            Sizes.Add(new FaviconSize("android-icon-72x72", 72));
            Sizes.Add(new FaviconSize("apple-icon-72x72", 72));
            Sizes.Add(new FaviconSize("ms-icon-70x70", 70));
            Sizes.Add(new FaviconSize("apple-icon-60x60", 60));
            Sizes.Add(new FaviconSize("apple-icon-57x57", 57));
            Sizes.Add(new FaviconSize("android-icon-48x48", 48));
            Sizes.Add(new FaviconSize("android-icon-36x36", 36));
            Sizes.Add(new FaviconSize("favicon-32x32", 32));
            Sizes.Add(new FaviconSize("favicon-16x16", 16));
        }
    }

    class FaviconSize
    {
        public FaviconSize(string _name, int _s)
        {
            Name = _name;
            Size = _s;
        }

        public string Name { get; set; }

        public int Size { get; set; }
    }
}