using BAG.Common.Data;
using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Default.WebUI.Controllers
{
    public class ResetController : Controller
    {
        [Obsolete]
        private UnitOfWork unit = null;
        private UnitOfWork Unit
        {
            get
            {
                if (unit == null)
                {
                    this.unit = new UnitOfWork();
                }
                return this.unit;
            }
            set
            {
                this.unit = Unit;
            }
        }
        private static readonly object LockObject = new object();

        // GET: Reset
        public ActionResult Seed()
        {
            //string data = System.IO.File.ReadAllText(Server.MapPath("~/content/et.json"));
            //JObject entities = JsonConvert.DeserializeObject<JObject>(data);

            //var users = unit.UserRepository.Get();
            //if (!users.Any())
            //{
            //    var user = new User();
            //    user.UserName = "pmizel";
            //    user.FirstName = "Paul";
            //    user.LastName = "Mizel";
            //    user.Email = "pmizel@brockhaus-ag.de";
            //    user.SetPassword("letmein");
            //    unit.UserRepository.Insert(user);
            //    user = new User();
            //    user.UserName = "mprzybilla";
            //    user.FirstName = "Marcus";
            //    user.LastName = "Przybilla";
            //    user.Email = "mprzybilla@brockhaus-ag.de";
            //    user.SetPassword("letmein");
            //    unit.UserRepository.Insert(user);
            //    unit.Save();
            //}



            //var carpenters = unit.ManufactoryRepository.Get();
            //if (!carpenters.Any())
            //{
            //    foreach (var carpenter in entities["Manufactories"])
            //    {
            //        var title = carpenter["Title"].ToString();
            //        var fax = carpenter["Fax"].ToString();
            //        var homepage = carpenter["Homepage"].ToString();
            //        var latitudeStriung = carpenter["Address"]["Latitude"].ToString();
            //        var longitudeString = carpenter["Address"]["Longitude"].ToString();
            //        var mail = carpenter["Email"].ToString();
            //        var telephone = carpenter["Telephone"].ToString();
            //        var url = carpenter["UrlName"].ToString();
            //        var cite = carpenter["Cite"].ToString();
            //        var citedPerson = carpenter["CitedPerson"].ToString();

            //        var newCarpenter = new Manufactory
            //        {
            //            Title = title,
            //            //Address = carpenter["Address"]["Street"].ToString() +
            //            //          carpenter["Address"]["Zip"].ToString() +
            //            //          carpenter["Address"]["City"].ToString(),
            //            Street = carpenter["Address"]["Street"].ToString(),
            //            Zip = carpenter["Address"]["Zip"].ToString(),
            //            City = carpenter["Address"]["City"].ToString(),
            //            Archived = false,
            //            ImageFactory = string.Empty,
            //            Fax = fax,
            //            Homepage = homepage,
            //            LatitudeString = latitudeStriung,
            //            LongitudeString = longitudeString,
            //            Mail = mail,
            //            Telephone = telephone,
            //            ImageWorker = string.Empty,
            //            Url = url,
            //            Cite = cite,
            //            CitedPerson = citedPerson,

            //        };

            //        unit.ManufactoryRepository.Insert(newCarpenter);
            //    }

            //    unit.Save();
            //}

            //var sites = unit.SiteRepository.Get();
            //if (!sites.Any())
            //{
            //    var site = new Site();
            //    site.Name = "impressum";
            //    site.Url = "impressum";

            //    Widget widget = null;
            //    widget = new WidgetContent()
            //    {
            //        Title = "Impressum",
            //        Content = "Beschreibung <br/> <b>"
            //    };
            //    unit.WidgetRepository.Insert(widget);
            //    site.WidgetIds.Add(widget.Id);
            //    unit.SiteRepository.Insert(site);
            //    site = new Site();
            //    site.Name = "datenschutz";
            //    site.Url = "datenschutz";
            //    unit.SiteRepository.Insert(site);

            //    site = new Site();
            //    site.Name = "Startseite";
            //    site.Title = "Edition Tischler";
            //    site.Url = "";

            //    widget = new WidgetCarusel()
            //    {
            //        Name = "Startseite Header Carusel"
            //    };
            //    unit.WidgetRepository.Insert(widget);
            //    site.WidgetIds.Add(widget.Id);

            //    widget = new WidgetEditionCarusel()
            //    {
            //        Name = "Startseite Produkt Carusel"
            //    };
            //    unit.WidgetRepository.Insert(widget);
            //    site.WidgetIds.Add(widget.Id);

            //    widget = new WidgetImageText()
            //    {
            //        Name = "Startseite Text / Bild DIE MANUFAKTUREN"
            //    };
            //    unit.WidgetRepository.Insert(widget);
            //    site.WidgetIds.Add(widget.Id);

            //    widget = new WidgetContent()
            //    {
            //        Name = "Startseite FERTIGUNG"
            //    };
            //    unit.WidgetRepository.Insert(widget);
            //    site.WidgetIds.Add(widget.Id);


            //    widget = new WidgetImageText()
            //    {
            //        Name = "Startseite Bild / Text Qualität & Service"
            //    };
            //    unit.WidgetRepository.Insert(widget);
            //    site.WidgetIds.Add(widget.Id);

            //    unit.SiteRepository.Insert(site);


            //    unit.Save();
            //}

            //var products = unit.EditionRepository.Get();
            //if (!products.Any())
            //{
            //    // Editionen
            //    foreach (var edition in entities["Editions"])
            //    {
            //        var newProduct = new Edition
            //        {
            //            Title = edition["Title"].ToString(),
            //            AdditionalInformation = (edition["AdditionalInformation"] ?? string.Empty).ToString(),
            //            Cite = (edition["Cite"] ?? string.Empty).ToString(),
            //            CitedPerson = (edition["CitePerson"] ?? string.Empty).ToString(),
            //            Designer = edition["Designer"].ToString(),
            //            MetaTitle = edition["SeoTitle"].ToString(),
            //            MetaDescription = edition["SeoDescription"].ToString(),
            //            MetaKeywords = edition["SeoKeywords"].ToString(),
            //            Description = (edition["Description"] ?? string.Empty).ToString(),
            //            Price = edition["Price"].ToString().ToDecimal(-666m),
            //            Url = edition["UrlName"].ToString(),
            //            Notes = edition["Notes"].ToString(),
            //            Type = ProductType.Edition
            //        };

            //        foreach (var story in edition["ProductStories"])
            //        {
            //            ProductStory newStory = new ProductStory
            //            {
            //                Title = story["Title"].ToString(),
            //                Story = story["Story"].ToString()
            //            };

            //            newProduct.ProductStories.Add(newStory);
            //        }

            //        unit.EditionRepository.Insert(newProduct);
            //    }

            //    // Accessories
            //    foreach (var accessory in entities["Accessories"])
            //    {
            //        var newProduct = new Edition
            //        {
            //            Title = accessory["Title"].ToString(),
            //            AdditionalInformation = (accessory["AdditionalInformation"] ?? string.Empty).ToString(),
            //            Cite = (accessory["Cite"] ?? string.Empty).ToString(),
            //            CitedPerson = (accessory["CitePerson"] ?? string.Empty).ToString(),
            //            Designer = accessory["Designer"].ToString(),
            //            MetaTitle = accessory["SeoTitle"].ToString(),
            //            MetaDescription = accessory["SeoDescription"].ToString(),
            //            MetaKeywords = accessory["SeoKeywords"].ToString(),
            //            Description = (accessory["Description"] ?? string.Empty).ToString(),
            //            Price = accessory["Price"].ToString().ToDecimal(-666m),
            //            Url = accessory["UrlName"].ToString(),
            //            Notes = accessory["Notes"].ToString(),
            //            Type = ProductType.Accessory
            //        };

            //        foreach (var story in accessory["ProductStories"])
            //        {
            //            ProductStory newStory = new ProductStory
            //            {
            //                Title = story["Title"].ToString(),
            //                Story = story["Story"].ToString()
            //            };

            //            newProduct.ProductStories.Add(newStory);
            //        }

            //        unit.EditionRepository.Insert(newProduct);
            //    }

            //    unit.Save();
            //}

            //unit.Save();

            return View();
        }

        public ActionResult Cache()
        {
            return View();
        }





        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, true);
                }
            }
        }


        public ActionResult CleanUp()
        {
            lock (LockObject)
            {



                if (ConfigurationManager.AppSettings["BAG.Backup.PassKey"] == null &&
                    string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["BAG.Backup.PassKey"]))
                {
                    return HttpNotFound();
                }
                string passKey = ConfigurationManager.AppSettings["BAG.Backup.PassKey"];
                string key = Request.QueryString["passkey"];
                if (passKey != key)
                {
                    return HttpNotFound();
                }

                string tmpPath = HttpContext.Server.MapPath("~/Temp/");
                if (Directory.Exists(tmpPath))
                {
                    Directory.Delete(tmpPath, true);
                }
                return View();
            }
        }

        public ActionResult Backup()
        {
            lock (LockObject)
            {
                if (ConfigurationManager.AppSettings["BAG.Backup.Key"] == null &&
                    string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["BAG.Backup.Key"]))
                {
                    return HttpNotFound();
                }
                string passKey = ConfigurationManager.AppSettings["BAG.Backup.Key"];

                //reset/backup?key=geheim&exclude=content,app_data&autocleanup=false
                string key = Request.QueryString["key"];
                string exclude = Request.QueryString["exclude"];
                bool auto_cleanup = Request.QueryString["autocleanup"] == "true";
                if (passKey != key)
                {
                    return HttpNotFound();
                }

                string dir = HttpContext.Server.MapPath("~/App_Data/");
                var tmpPathName = DateTime.Now.ToString("yyyyMMdd");
                var fileName = tmpPathName + ".zip";
                //tmpPathName = tmpPathName + "_" + Guid.NewGuid();                

                string path = HttpContext.Server.MapPath("~/Temp/" + fileName);
                string tmpPath = HttpContext.Server.MapPath("~/Temp/" + tmpPathName);

                if (Directory.Exists(tmpPath))
                {
                    Directory.Delete(tmpPath, true);
                }

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                System.Threading.Thread.Sleep(100);

                Directory.CreateDirectory(Path.GetDirectoryName(path));
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath + "\\App_Data\\"));
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath + "\\Views\\"));
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath + "\\Views\\Widgets\\"));
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath + "\\Views\\Shared\\"));
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath + "\\Content\\"));

                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                if (string.IsNullOrEmpty(exclude) || !exclude.Contains("app_data"))
                {
                    FileInfo[] files = dirInfo.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        if (file.Extension.ToLower() == ".xml")
                        {
                            string tempDatapath = Path.Combine(tmpPath + "\\App_Data\\", file.Name);
                            file.CopyTo(tempDatapath, false);
                        }
                    }
                }

                //List<string> excludeList = new List<string>();
                if (string.IsNullOrEmpty(exclude) || !exclude.Contains("content"))
                    DirectoryCopy(HttpContext.Server.MapPath("~/Content/"), tmpPath + "\\Content\\", true);
                if (string.IsNullOrEmpty(exclude) || !exclude.Contains("widgets"))
                    DirectoryCopy(HttpContext.Server.MapPath("~/Views/Widgets/"), tmpPath + "\\Views\\Widgets\\", true);
                if (string.IsNullOrEmpty(exclude) || !exclude.Contains("shared"))
                    DirectoryCopy(HttpContext.Server.MapPath("~/Views/Shared/"), tmpPath + "\\Views\\Shared\\", true);

                ZipFile.CreateFromDirectory(tmpPath, path);
                if (Directory.Exists(tmpPath))
                {
                    if (auto_cleanup)
                    {
                        Directory.Delete(tmpPath, true);
                    }
                }
                System.Threading.Thread.Sleep(100);


                FileStream readFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                return File(readFile, "application/x-zip-compressed", Path.GetFileName(path));

            }


        }



    }

    public static class ConversionExtensions
    {
        public static decimal ToDecimal(this string str, decimal defaultDecimal)
        {
            decimal value = 0m;
            return decimal.TryParse(str, out value) ? value : defaultDecimal;
        }
    }
}