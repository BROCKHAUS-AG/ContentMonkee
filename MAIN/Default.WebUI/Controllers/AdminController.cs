using BAG.Common;
using BAG.Common.Authorization;
using BAG.Common.Data;
using Default.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Default.WebUI.Extensions;
using BAG.Common.Data.Entities;
using System.IO;
using BAG.Common.Geolocation;
using BAG.Framework.Geolocation.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Default.WebUI.Classes;
using System.Xml.Serialization;
using System.Drawing;
using System.Configuration;

namespace Default.WebUI.Controllers
{

    [WebAuthorizeAttribute()]
    public class AdminController : Controller
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

        public ActionResult Index()
        {
            var model = new AdminViewModel();

            var widgetmanagers = Unit.WidgetManagerRepository.Get(w => w.SitesettingId == _Globals.Instance.CurrentSiteSettingId).ToList();
            var sitemanagers = Unit.SiteManagerRepository.Get(s => s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId).ToList();
            var widgets = new List<Widget>();
            var sites = new List<Site>();
            widgetmanagers.ForEach(w => widgets.Add(w.PreRelease));
            sitemanagers.ForEach(s => sites.Add(s.PreRelease));

            model.Users = Unit.UserRepository.Get().ToList();
            model.Widgets = widgets;
            if (Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId) != null)
            {
                model.Redirections = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId).Redirections;
                model.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            }
            model.Sites = sites;
            foreach (var site in model.Sites)
            {
                site.OnLoad(Unit);
            }
            model.SiteSettings = Unit.SiteSettingRepository.Get().ToList();
            model.SiteManagers = sitemanagers;
            model.WidgetManagers = widgetmanagers;

            return View(model);
        }

        public ActionResult EmployeeEdit(Guid? id)
        {
            Employee item;
            bool isLoggedInAdmin = _Globals.Instance.GetLoginUser(Unit).IsAdmin;
            var model = new AdminViewModel();
            if (id == null)
            {
                item = new Employee();
            }
            else
            {
                item = Unit.EmployeeRepository.GetByID(id);
            }

            if (item != null)
            {
                model.IsLoggedInAdmin = isLoggedInAdmin;
                model.CurrentEmployee = item;
                model.Users = Unit.UserRepository.Get().ToList();
            }
            model.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);

            return View(model);
        }

        [HttpPost]
        public ActionResult EmployeeEdit(Guid? id, FormCollection form)
        {
            bool isLoggedInAdmin = _Globals.Instance.GetLoginUser(Unit).IsAdmin;
            if (!isLoggedInAdmin)
            {
                return RedirectToAction("Data");
            }
            Employee item = Unit.EmployeeRepository.GetByID(id);

            bool doInsert = false;
            if (item == null)
            {
                item = new Employee();
                doInsert = true;
            }
            item.Gender = form.GetEnum("Gender", Gender.Male);
            item.FirstName = form.GetString("FirstName", string.Empty);
            item.LastName = form.GetString("LastName", string.Empty);
            item.Position = form.GetString("Position", string.Empty);
            item.Email = form.GetString("Email", string.Empty);
            item.EmailAlias = form.GetString("EmailAlias", string.Empty);
            item.Telephone = form.GetString("Telephone", string.Empty);
            item.Fax = form.GetString("Fax", string.Empty);
            item.EnterDate = form.GetDate("EnterDate", DateTime.Now);
            item.ExitDate = form.GetDate("ExitDate", DateTime.Now);
            item.Birthday = form.GetDate("Birthday", DateTime.Now);
            item.ProfilImagePath = form.GetString("ProfilImagePath", string.Empty);
            item.UserId = form.GetGuid("UserId", Guid.Empty);
            item.GoogleProfileUrl = form.GetString("GoogleProfilUrl", string.Empty);
            item.XprncProfileUrl = form.GetString("XprncProfilUrl", string.Empty);
            item.XingProfileUrl = form.GetString("XingProfilUrl", string.Empty);
            item.FacebookProfileUrl = form.GetString("FacebookProfilUrl", string.Empty);
            item.VCardUrl = form.GetString("VCardUrl", string.Empty);
            item.LinkedInProfileUrl = form.GetString("LinkedInProfileUrl", string.Empty);

            if (doInsert)
            {
                item.Created = DateTime.Now;
                Unit.EmployeeRepository.Insert(item);
            }
            else
            {
                item.Updated = DateTime.Now;
                Unit.EmployeeRepository.Update(item);
            }
            Unit.Save();

            return RedirectToAction("Data");
        }

        public ActionResult EmployeeDelete(Guid id)
        {
            Employee employee = Unit.EmployeeRepository.GetByID(id);

            if (employee != null)
            {
                Unit.EmployeeRepository.Delete(employee);
                Unit.Save();
            }

            return RedirectToAction("Data");
        }

        private void ChangeSiteSetting(Guid ssid)
        {
            this.Unit = _Globals.Instance.ChangeSiteSettingId(ssid, this.Unit);
            _Globals.Instance.ChangingSiteSettingId = false;
        }

        [HttpPost]
        public ActionResult SelectSiteSetting(FormCollection form)
        {
            Guid selectedId = form.GetGuid("selectCurrentSiteSetting", Guid.Empty);
            ChangeSiteSetting(selectedId);
            var returnUrl = form.GetString("returnUrl");
            return Redirect(returnUrl);
        }

        public ActionResult SiteSettings()
        {
            var rules = new AuthorizationRules();
            var widgetmanagers = Unit.WidgetManagerRepository.Get().ToList();
            var sitemanagers = Unit.SiteManagerRepository.Get().ToList();
            var widgetlist = new List<Widget>();
            var sites = new List<Site>();
            // 24.08.2016 - Michel jakob - new Binding
            //bool findbinding = true;
            widgetmanagers.ForEach(w => widgetlist.Add(w.PreRelease));
            sitemanagers.ForEach(s => sites.Add(s.PreRelease));

            var model = new AdminViewModel();
            model.SiteSettings = Unit.SiteSettingRepository.Get().ToList();

            model.CurrentBindingId = rules.GetSiteSettingIdFromBinding(HttpContext.Request.Url);

            // 24.08.2016 - Michel jakob - new Binding
            //foreach (var sitesetting in model.SiteSettings)
            //{
            //    if (findbinding)
            //    {
            //        var bydomain = rules.findbyMainDomain(sitesetting, false);
            //        var bybinding = rules.findbyBinding(sitesetting, false);
            //        if (bydomain)
            //        {
            //            model.CurrentBindingId = sitesetting.Id;
            //            findbinding = false;
            //        }
            //        else if (bybinding)
            //        {
            //            model.CurrentBindingId = sitesetting.Id;
            //            findbinding = false;
            //        }
            //    }

            //}
            model.CurrentUser = _Globals.Instance.GetLoginUser(Unit);
            model.Users = Unit.UserRepository.Get().ToList();
            model.Widgets = widgetlist;
            model.Sites = sites;
            foreach (var site in model.Sites)
            {
                site.OnLoad(Unit);
            }
            return View(model);
        }

        public ActionResult SiteSettingEdit(Guid? id)
        {
            SiteSetting item;
            var model = new AdminViewModel();

            model.Templates = Directory.GetDirectories(Server.MapPath("~/Views/Template/")).Where(f => !f.EndsWith("Widgets")).ToArray();

            if (id == null)
            {
                item = new SiteSetting();
            }
            else
            {
                item = Unit.SiteSettingRepository.GetByID(id);
                ChangeSiteSetting(id.Value);
            }

            if (item != null)
            {
                model.CurrentUser = _Globals.Instance.GetLoginUser(Unit);
                model.CurrentSiteSetting = item;
            }

            return View(model);
        }

        [ConfigurationPropertyAttribute("requestValidationMode", DefaultValue = "2.0")]
        public Version RequestValidationMode { get; set; }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SiteSettingEdit(Guid? id, FormCollection form)
        {
            DirectoryManagement dirMan;
            bool doInsert = false;
            string fileNameOld, sourcePath, destpath, contentsourcePath, contentdestpath, templateOld;

            SiteSetting item = Unit.SiteSettingRepository.GetByID(id);
            if (item == null)
            {
                item = new SiteSetting();
                doInsert = true;
            }

            fileNameOld = item.Bindings;
            if (item.Template == null)
            {
                templateOld = "null";
                item.Template = form.GetString("Template", string.Empty);
            }
            else
            {
                templateOld = item.Template;
            }

            var widgetslist = form.GetValues("WidgetsType");

            item.Name = form.GetString("Name", string.Empty);
            item.Bindings = form.GetString("Bindings", string.Empty);
            item.Robots = form.GetString("Robots", string.Empty);

            if (!templateOld.Equals(item.Template))
            {
                dirMan = new DirectoryManagement(item.Bindings, fileNameOld, item.Id);

                sourcePath = Server.MapPath("~/Views/Template/" + item.Template + "/");
                destpath = Server.MapPath("~/Views/" + item.Id + "/");

                contentsourcePath = Server.MapPath("~/Content/Template/" + item.Template + "/");
                contentdestpath = Server.MapPath("~/Content/" + item.Id + "/");

                dirMan.CreateFolderAndFile(Server.MapPath("~/Content/" + item.Id));
                dirMan.CreateFolderAndFile(Server.MapPath("~/Views/" + item.Id));

                if (!item.Template.Equals(""))
                {
                    dirMan.Copy(sourcePath, destpath, true);
                    dirMan.Copy(contentsourcePath, contentdestpath, true);
                }
                var widgetsource = Server.MapPath("~/Views/Template/Widgets/");
                var widgetdest = Server.MapPath("~/Views/" + item.Id + "/Widgets/");
                if (widgetslist != null)
                {
                    item.WidgetsList = widgetslist;
                    Directory.CreateDirectory(widgetdest);
                    foreach (var widget in widgetslist)
                    {
                        System.IO.File.Copy(widgetsource + widget + ".cshtml", widgetdest + widget + ".cshtml");
                    }

                }
                else
                {
                    dirMan.Copy(widgetsource, widgetdest, true);
                }

                var filepath = Server.MapPath("~/robots.txt");
                using (StreamReader sr = new StreamReader(filepath))
                {
                    var line = sr.ReadToEnd();
                    item.Robots = line;
                }
            }

            //checkIsDefault
            item.IsDefault = form.GetBool("IsDefault");
            if (item.IsDefault)
            {
                List<SiteSetting> listOfSiteSettings = Unit.SiteSettingRepository.Get().ToList();

                foreach (SiteSetting site in listOfSiteSettings)
                {
                    if (site.Id != item.Id)
                    {
                        site.IsDefault = false;
                        Unit.SiteSettingRepository.Update(site);
                    }
                }
            }

            item.Fallback = form.GetString("Fallback", string.Empty);
            item.CookieDescription = form.GetString("CookieDescription", string.Empty);
            item.MainDomain = form.GetString("MainDomain", string.Empty);
            item.Author = form.GetString("Author", string.Empty);
            item.Type = form.GetString("Type", string.Empty);
            item.OwnerId = form.GetGuid("OwnerId", Guid.Empty);
            item.IsPortalEnabled = form.GetBool("IsPortalEnabled");
            item.Archived = form.GetBool("Archived");
            item.HideWidgetsToolbar = form.GetBool("HideWidgetsToolbar");

            item.SiteStructureType = form.GetString("SiteStructureType");
            item.SiteStructureName = form.GetString("SiteStructureName");
            item.SiteStructureUrl = form.GetString("SiteStructureUrl");
            item.SiteStructureLogo = form.GetString("SiteStructureLogo");
            item.SiteStructureStreetAddress = form.GetString("SiteStructureStreetAddress");
            item.SiteStructureAddressLocality = form.GetString("SiteStructureAddressLocality");
            item.SiteStructureAddressRegion = form.GetString("SiteStructureAddressRegion");
            item.SiteStructurePostalCode = form.GetString("SiteStructurePostalCode");
            item.SiteStructureAddressCountry = form.GetString("SiteStructureAddressCountry");
            item.SiteStructureGeoLatitude = form.GetString("SiteStructureGeoLatitude");
            item.SiteStructureGeoLongitude = form.GetString("SiteStructureGeoLongitude");
            item.SiteStructureTelephone = form.GetString("SiteStructureTelephone");
            item.SiteStructureContactType = form.GetString("SiteStructureContactType");
            item.SiteStructureOpens = form.GetString("SiteStructureOpens");
            item.SiteStructureCloses = form.GetString("SiteStructureCloses");

            item.ImagePhoneHight = form.GetInt("ImagePhoneHight");
            item.ImagePhoneWidth = form.GetInt("ImagePhoneWidth");
            item.ImageTabletHight = form.GetInt("ImageTabletHight");
            item.ImageTabletWidth = form.GetInt("ImageTabletWidth");
            item.ImageDesktopHight = form.GetInt("ImageDesktopHight");
            item.ImageDesktopWidth = form.GetInt("ImageDesktopWidth");

            item.LogoImage = form.GetString("LogoImage");
            item.FontsCSS = form.GetString("FontsCSS");


            item.GoogleAnalyticTrackingId = form.GetString("GoogleAnalyticTrackingId");
            item.IpAnonymization = form.GetBool("IpAnonymization");
            item.TrackingCode = form.GetString("TrackingCode");

            if (!form.GetString("favicon").IsNullOrEmpty() && item.Favicon != form.GetString("favicon"))
            {
                item.Favicon = form.GetString("favicon");

                new FaviconHelper(Server, item.Favicon);
            }
            else
            {
                item.Favicon = form.GetString("favicon");
            }

            if (doInsert)
            {
                item.Created = DateTime.Now;
                Unit.SiteSettingRepository.Insert(item);
            }
            else
            {
                item.Updated = DateTime.Now;
                Unit.SiteSettingRepository.Update(item);
            }

            Unit.Save();
            this.ChangeSiteSetting(item.Id);
            Unit = new UnitOfWork();
            if (fileNameOld == null)
            {
                ChangeSiteSetting(item.Id);
                FormCollection formSite = new FormCollection();
                formSite["name"] = "Home";
                formSite["sitetype"] = string.Empty;
                formSite["url"] = string.Empty;
                formSite["startsite"] = "true";
                formSite["siteSettingId"] = item.Id.ToString();

                RedirectToRouteResult result = (RedirectToRouteResult)SiteAdd(formSite);

            }

            return RedirectToAction("SiteSettings");
        }

        public ActionResult SiteSettingDelete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction("SiteSettings");
            }
            if (id == _Globals.Instance.CurrentSiteSettingId)
            {
                ChangeSiteSetting(Guid.Empty);
            }

            DirectoryManagement dirMan;
            SiteSetting siteSetting = Unit.SiteSettingRepository.GetByID(id);

            if (siteSetting == null)
            {
                return RedirectToAction("SiteSettings");
            }

            Unit.SiteSettingRepository.Delete(siteSetting);
            Unit.Save();


            if (siteSetting.Template.Equals(""))
            {
                dirMan = new DirectoryManagement();
                dirMan.Move(Server.MapPath("~/Views/" + siteSetting.Id), Server.MapPath("~/Trash/Views/" + siteSetting.Id));
                dirMan.Move(Server.MapPath("~/Content/" + siteSetting.Id), Server.MapPath("~/Trash/Content/" + siteSetting.Id));
                dirMan.Move(Server.MapPath("~/App_Data/" + siteSetting.Id), Server.MapPath("~/Trash/App_Data/" + siteSetting.Id));
            }
            else
            {
                createTrashFolder("Views", siteSetting.Id.ToString());
                createTrashFolder("Content", siteSetting.Id.ToString());
                createTrashFolder("App_Data", siteSetting.Id.ToString());
                try
                {
                    deleteFolder("Views", siteSetting.Id.ToString());
                    deleteFolder("Content", siteSetting.Id.ToString());
                    deleteFolder("App_Data", siteSetting.Id.ToString());
                }
                catch (Exception)
                {

                }
            }
            return RedirectToAction("SiteSettings");
        }

        private void deleteFolder(string contenFolder, string folder)
        {
            if (Directory.Exists(Server.MapPath("~/" + contenFolder + "/" + folder)))
            {
                var subFolders = Directory.GetDirectories(Server.MapPath("~/" + contenFolder + "/" + folder));
                foreach (var subFolder in subFolders)
                {
                    deleteFolder(contenFolder, folder + "/" + Path.GetFileName(subFolder));
                }
                if (!Directory.EnumerateFiles(Server.MapPath("~/" + contenFolder + "/" + folder)).Any())
                {
                    string[] filelist = Directory.GetFiles(Server.MapPath("~/" + contenFolder + "/" + folder));
                    foreach (string f in filelist)
                    {
                        System.IO.File.Delete(f);
                    }
                }
                Directory.Delete(Server.MapPath("~/" + contenFolder + "/" + folder));
            }
        }

        private void copyTrashFile(string oldDirectory, string newDirectory)
        {
            var subFiles = Directory.GetFiles(oldDirectory);
            foreach (var subFile in subFiles)
            {
                var fileName = Path.GetFileName(subFile);
                System.IO.File.Move(oldDirectory + "\\" + fileName, newDirectory + "\\" + fileName);

            }
        }

        private void createTrashFolder(string contenFolder, string folder)
        {
            if (Directory.Exists(Server.MapPath("~/" + contenFolder + "/" + folder)))
            {
                Directory.CreateDirectory(Server.MapPath("~/Trash/" + contenFolder + "/" + folder));
                var subFolders = Directory.GetDirectories(Server.MapPath("~/" + contenFolder + "/" + folder));
                foreach (var subFolder in subFolders)
                {
                    createTrashFolder(contenFolder, folder + "/" + Path.GetFileName(subFolder));
                }
                copyTrashFile(Server.MapPath("~/" + contenFolder + "/" + folder), Server.MapPath("~/Trash/" + contenFolder + "/" + folder));
            }
        }

        public ActionResult Help()
        {
            var model = new AdminViewModel();
            return View(model);
        }

        public ActionResult Contents()
        {
            AdminViewModel model = new AdminViewModel();
            model.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            model.SiteSettings = Unit.SiteSettingRepository.Get().ToList();

            return View(model);
        }

        public ActionResult Data()
        {
            var model = new DataViewModel();
            model.TabKey = Request.QueryString["tab"];
            model.Employees = Unit.EmployeeRepository.Get().ToList();
            model.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            model.SiteSettings = Unit.SiteSettingRepository.Get().ToList();

            ISiteSelection siteSelection = new AdminViewModel();
            siteSelection.SiteSettings = Unit.SiteSettingRepository.Get().ToList();
            siteSelection.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            model.SiteSelectionViewModel = siteSelection;


            return View(model);
        }

        public ActionResult WidgetMove(Guid id, Guid siteid, string direction)
        {
            WidgetMoveBase(id, siteid, direction);
            return RedirectToAction("ContentSite", "Home");
        }

        public void WidgetMoveBase(Guid id, Guid siteid, string direction)
        {
            var sitemanager = Unit.SiteManagerRepository.GetByID(siteid);
            var site = sitemanager.PreRelease;
            var awidget = site.WidgetIds.ToArray();
            var countwidget = site.WidgetIds.Count();
            countwidget--;
            var indexold = Array.LastIndexOf(awidget, id);
            switch (direction)
            {
                case "first":
                    {
                        if (indexold != 0)
                        {
                            var newpos = 0;
                            var valuenewpos = awidget[newpos];
                            awidget[newpos] = id;
                            for (int run = 1; run <= indexold; run++)
                            {
                                var valuecurrent = awidget[run];
                                awidget[run] = valuenewpos;
                                valuenewpos = valuecurrent;
                            }
                            site.WidgetIds = awidget.ToList<Guid>();
                            sitemanager.PreRelease = site;
                            Unit.SiteManagerRepository.Update(sitemanager);
                            Unit.Save();
                        }

                        break;
                    }
                case "up":
                    {
                        if (indexold != 0)
                        {
                            var newpos = indexold - 1;
                            var valuenewpos = awidget[newpos];
                            awidget[newpos] = id;
                            awidget[indexold] = valuenewpos;
                            site.WidgetIds = awidget.ToList<Guid>();
                            sitemanager.PreRelease = site;
                            Unit.SiteManagerRepository.Update(sitemanager);
                            Unit.Save();
                        }

                        break;
                    }
                case "down":
                    {
                        if (indexold < countwidget)
                        {
                            var newpos = indexold + 1;
                            var valuenewpos = awidget[newpos];
                            awidget[newpos] = id;
                            awidget[indexold] = valuenewpos;
                            site.WidgetIds = awidget.ToList<Guid>();
                            sitemanager.PreRelease = site;
                            Unit.SiteManagerRepository.Update(sitemanager);
                            Unit.Save();
                        }

                        break;
                    }
                case "last":
                    {
                        if (indexold < countwidget)
                        {
                            var newpos = countwidget;
                            var valuenewpos = awidget[newpos];
                            awidget[newpos] = id;
                            for (int run = countwidget - 1; run >= indexold; run--)
                            {
                                var valuecurrent = awidget[run];
                                awidget[run] = valuenewpos;
                                valuenewpos = valuecurrent;
                            }
                            site.WidgetIds = awidget.ToList<Guid>();
                            sitemanager.PreRelease = site;
                            Unit.SiteManagerRepository.Update(sitemanager);
                            Unit.Save();
                        }

                        break;
                    }
                default:
                    break;
            }
        }
        public ActionResult WidgetEditDyn(Guid id, Guid? siteid, Guid? widgetid)
        {
            return View(WidgetEditForm(id, siteid, widgetid));
        }
        public ActionResult WidgetEdit(Guid id, Guid? siteid, Guid? widgetid)
        {
            return View(WidgetEditForm(id, siteid, widgetid));
        }
        public AdminWidgetViewModel WidgetEditForm(Guid id, Guid? siteid, Guid? widgetid)
        {
            var siteidIsEmpty = (siteid == null);
            var widgetidIsEmpty = (widgetid == null);
            var model = new AdminWidgetViewModel();
            var widgetlist = new List<Widget>();
            var widgetmanagers = Unit.WidgetManagerRepository.Get(w => w.SitesettingId == _Globals.Instance.CurrentSiteSettingId).ToList();
            var sitemanagers = Unit.SiteManagerRepository.Get().ToList();
            var sites = new List<Site>();
            widgetmanagers.ForEach(w => widgetlist.Add(w.PreRelease));
            sitemanagers.ForEach(s => sites.Add(s.PreRelease));

            model.Users = Unit.UserRepository.Get().ToList();
            model.Sites = sites;
            model.Sites.ForEach(s => s.OnLoad(Unit));

            model.Widgets = widgetlist;
            model.Widgets.ForEach(w => w.OnLoad(Unit));
            model.CurrentWidget = Unit.WidgetManagerRepository.GetByID(id).PreRelease;
            model.WidgetsList = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId).WidgetsList;
            model.Employees = Unit.EmployeeRepository.Get().ToList();
            model.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            model.WidgetManagers = widgetmanagers;
            if (!widgetidIsEmpty)
            {
                model.CurrentWidgetId = widgetid.Value;
            }
            if (!siteidIsEmpty)
            {
                model.CurrentSiteId = siteid.Value;
                model.Sites.Find(s => s.Id == siteid);
                foreach (var navigation in model.Sites.Find(s => s.Id == siteid).WidgetNavigations)
                {
                    if (navigation == id)
                    {
                        model.IsNavigation = true;
                    }

                }
            }
            model.SetSEOModel(model.CurrentWidget);
            return model;
        }

        [HttpPost]
        public string CreateUniqueWidgetUrl(Guid id, string name, bool asjson = true)
        {
            string newurl = name.Replace(" ", "-")
                                .Replace("ä", "ae")
                                .Replace("ö", "oe")
                                .Replace("ü", "ue")
                                .Replace("?", "")
                                .Replace(":", "")
                                .Replace("/", "")
                                .Replace("#", "")
                                .Replace("[", "")
                                .Replace("]", "")
                                .Replace("@", "at")
                                .Replace("$", "")
                                .Replace("&", "und")
                                .Replace("'", "")
                                .Replace("(", "")
                                .Replace(")", "")
                                .Replace("*", "")
                                .Replace("+", "")
                                .Replace(",", "")
                                .Replace(";", "")
                                .Replace("=", "")
                                .Replace("\"", "")
                                .ToLower();
            string append = "";
            bool unique = true;
            int i = 0;
            string[] controllers = { "Admin", "Account", "Reset", "File", "Ajax", "Search", "QRCode", "SendContact", "Content" };
            foreach (var controller in controllers)
            {
                if (newurl.Equals(controller.ToLower()))
                {
                    unique = false;
                    append = "-" + i.ToString();
                    i++;
                }
            }

            while (Unit.WidgetManagerRepository.Get(w => w.PreRelease.Id != id && w.PreRelease.Url != null && w.PreRelease.Url.ToLower() == newurl + append && w.SitesettingId == _Globals.Instance.CurrentSiteSettingId).Any())
            {
                unique = false;
                append = "-" + i.ToString();
                i++;
            }

            if (asjson)
            {
                return "{\"url\": \"" + newurl + append + "\", \"unique\": " + unique.ToString().ToLower() + "}";
            }
            else
            {
                return newurl + append;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult WidgetEditAutoSave(Guid id, FormCollection form)
        {
            WidgetEdit(id, form);
            Widget widget = Unit.WidgetManagerRepository.GetByID(id).PreRelease;
            return Content(widget.Updated.ToString());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult WidgetEdit(Guid id, FormCollection form)
        {
            var itemmanager = Unit.WidgetManagerRepository.Get(w => w.PreRelease.Id == id).First();
            var item = itemmanager.PreRelease;
            var siteId = form.GetGuid("currentsiteid", Guid.Empty);
            var widgetId = form.GetGuid("currentwidgetid", Guid.Empty);
            var inmenu = form.GetBool("inmenu");
            var widgetids = form.GetString("itemsstring");
            item.Updated = DateTime.Now;
            if (!(siteId == Guid.Empty))
            {
                var sitemanageritem = Unit.SiteManagerRepository.GetByID(siteId);
                var siteitem = sitemanageritem.PreRelease;
                if (inmenu)
                {
                    if (!siteitem.WidgetNavigations.Contains(item.Id))
                    {
                        siteitem.WidgetNavigations.Add(item.Id);
                    }
                }
                else
                {
                    siteitem.WidgetNavigations.Remove(item.Id);
                }
                Unit.SiteManagerRepository.Update(sitemanageritem);
            }

            var type = item.GetType();
            var props = type.GetProperties();
            var keys = form.AllKeys.ToList();
            var name = string.Empty;
            bool wasVisible = item.Visible;
            foreach (var prop in props)
            {
                if (prop.CanWrite)
                {
                    switch (prop.PropertyType.Name)
                    {
                        case "String":
                            name = prop.Name.ToLower();
                            if (keys.Contains(name))
                            {
                                var value = form.GetString(name);
                                prop.SetValue(item, value);
                            }
                            break;
                        case "Boolean":
                            name = prop.Name.ToLower();
                            prop.SetValue(item, false);
                            if (keys.Contains(name))
                            {
                                var value = form.GetBool(name);
                                prop.SetValue(item, value);
                            }
                            break;
                        case "Int32":
                            name = prop.Name.ToLower();
                            if (keys.Contains(name))
                            {
                                var value = form.GetInt(name);
                                prop.SetValue(item, value);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            item.Visible = wasVisible;
            if (item.Type == "WidgetContact")
            {
                var widget = item as WidgetContact;
                widget.UseVCard = form.GetBool("usevcard");
                widget.UserId = form.GetGuid("userid", Guid.Empty);
                widget.SmtpPort = form.GetInt("smtpport");
            }

            if (item.Type == "WidgetVCard")
            {
                var widget = item as WidgetVCard;
                widget.UserId = form.GetGuid("userid", Guid.Empty);
                widget.FlyerFileUrl = form.GetString("FlyerFileUrl", string.Empty);
                widget.FlyerText = form.GetString("FlyerText", string.Empty);
                widget.ReferencesFileUrl = form.GetString("ReferencesFileUrl", string.Empty);
                widget.ReferencesText = form.GetString("ReferencesText", string.Empty);
            }

            if (item.Type == "WidgetComposite")
            {
                var widget = item as WidgetComposite;
                widget.Horizontal = form.GetBool("orientation");
                if (!string.IsNullOrEmpty(widgetids))
                {
                    widget.WidgetIds = form.GetGuidList("itemsstring");
                }
            }

            item.SocialMedia = new SocialMedia();
            item.SocialMedia.Facebook_Like = form.GetBool("socialmedia.facebook_like", false);
            item.SocialMedia.Facebook_Share = form.GetBool("socialmedia.facebook_share", false);
            item.SocialMedia.Xing_Like = form.GetBool("socialmedia.xing_like", false);
            item.SocialMedia.Xing_Share = form.GetBool("socialmedia.xing_share", false);
            item.SocialMedia.LinkedIn_Like = form.GetBool("socialmedia.linkedin_like", false);
            item.SocialMedia.LinkedIn_Share = form.GetBool("socialmedia.linkedin_share", false);
            item.SocialMedia.Googleplus_share = form.GetBool("socialmedia.googleplus_share", false);
            item.SocialMedia.Reddit_share = form.GetBool("socialmedia.reddit_share", false);
            item.SocialMedia.Twitter_share = form.GetBool("socialmedia.twitter_share", false);
            item.SocialMedia.Email_share = form.GetBool("socialmedia.email_share", false);
            item.Url = CreateUniqueWidgetUrl(item.Id, item.Name, false);
            item.MetaPriority = form.GetDouble("metapriority");

            var originalname = item.Type + ".cshtml";
            var partialname = item.Partial;
            var path = Server.MapPath("~/Views/Widgets/" + item.Partial);
            var viewspath = Server.MapPath("~/Views/" + _Globals.Instance.CurrentSiteSettingId + "/Widgets/" + item.Partial);
            if (System.IO.File.Exists(viewspath))
            {
                path = viewspath;
            }
            var html = form.GetString("widgetfilecontent");
            html = Server.HtmlDecode(html);
            var old_html = System.IO.File.ReadAllText(path);
            if (old_html != html)
            {
                System.IO.File.WriteAllText(path, html, System.Text.UTF8Encoding.UTF8);
            }

            Unit.WidgetManagerRepository.Update(itemmanager);

            Unit.Save();
            if (siteId != Guid.Empty && widgetId != Guid.Empty)
            {
                return RedirectToAction("WidgetEdit", new { id = widgetId, siteid = siteId });
            }
            else if (!(siteId == Guid.Empty))
            {
                return RedirectToAction("SiteEdit", new { id = siteId });
            }
            else if (!(widgetId == Guid.Empty))
            {
                return RedirectToAction("WidgetEdit", new { id = widgetId });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult WidgetReset(FormCollection form)
        {
            var id = form.GetGuid("id", Guid.Empty);
            var widget = Unit.WidgetManagerRepository.GetByID(id);
            widget.PreRelease.Partial = widget.PreRelease.Type + ".cshtml";
            Unit.WidgetManagerRepository.Update(widget);
            widget.Updated = DateTime.Now;
            Unit.Save();
            return RedirectToAction("WidgetEdit", new { id = widget.Id });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult WidgetAdd(FormCollection form)
        {

            var widgetmanager = new WidgetManager();

            var id = form.GetGuid("id", Guid.Empty);
            var siteId = form.GetGuid("siteId", Guid.Empty);

            var widgetType = form.GetString("widgetType");
            var widgetTypeId = form.GetGuid("widgetType", Guid.Empty);

            var sitemanager = Unit.SiteManagerRepository.GetByID(siteId);

            Site site = null;
            if (sitemanager != null)
            {
                site = sitemanager.PreRelease;
                site.Updated = DateTime.Now;
            }

            Widget widget = widgetmanager.PreRelease;

            bool isNew = false;
            if (widgetTypeId != Guid.Empty)
            {
                widgetmanager = Unit.WidgetManagerRepository.GetByID(widgetTypeId);
                widget = widgetmanager.PreRelease;
            }
            else
            {
                var widgetTypeInstance = Type.GetType("BAG.Common.Data.Entities." + widgetType + ",BAG.Common");
                widget = (Widget)Activator.CreateInstance(widgetTypeInstance);
                widget.Name = form.GetString("name");
                widget.SitesettingId = _Globals.Instance.CurrentSiteSettingId;
                widgetmanager.SitesettingId = _Globals.Instance.CurrentSiteSettingId;
                widget.Id = widgetmanager.Id;
                widget.Url = CreateUniqueWidgetUrl(widget.Id, widget.Name, false);
                widgetmanager.PreRelease = widget;
                Unit.WidgetManagerRepository.Insert(widgetmanager);
                isNew = true;
            }

            widget.Updated = DateTime.Now;
            widget.Visible = true;

            if (site != null)
            {
                site.WidgetIds.Add(widgetmanager.Id);
                sitemanager.PreRelease = site;
                Unit.SiteManagerRepository.Update(sitemanager);
            }
            Unit.Save();

            if (isNew)
                return RedirectToAction("WidgetEdit", new { id = widgetmanager.Id, siteid = siteId });
            else
                return RedirectToAction("SiteEdit", new { id = siteId });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult WidgetCompositeAdd(FormCollection form)
        {
            var id = form.GetGuid("id", Guid.Empty);
            var widgetId = form.GetGuid("widgetId", Guid.Empty);
            var siteId = form.GetGuid("siteId", Guid.Empty);
            if (siteId != Guid.Empty)
            {
                var sitemanager = Unit.SiteManagerRepository.GetByID(siteId);
                sitemanager.PreRelease.Updated = DateTime.Now;
            }

            var widgetType = form.GetString("widgetType");
            var widgetTypeId = form.GetGuid("widgetType", Guid.Empty);

            var widgetmanagercomposite = Unit.WidgetManagerRepository.GetByID(widgetId);
            var widgetcomposite = widgetmanagercomposite.PreRelease;
            var widgetbase = widgetcomposite as WidgetComposite;
            var widgetmanager = new WidgetManager();
            Widget widget = null;

            bool isNew = false;
            if (widgetTypeId != Guid.Empty)
            {
                widgetmanager = Unit.WidgetManagerRepository.GetByID(widgetTypeId);
                widget = widgetmanager.PreRelease;
            }
            else
            {
                var widgetTypeInstance = Type.GetType("BAG.Common.Data.Entities." + widgetType + ",BAG.Common");
                widget = (Widget)Activator.CreateInstance(widgetTypeInstance);
                widget.Name = form.GetString("name");
                widget.SitesettingId = _Globals.Instance.CurrentSiteSettingId;
                widgetmanager.SitesettingId = _Globals.Instance.CurrentSiteSettingId;
                widget.Id = widgetmanager.Id;
                widget.Url = CreateUniqueWidgetUrl(widget.Id, widget.Name, false);
                widgetmanager.PreRelease = widget;
                Unit.WidgetManagerRepository.Insert(widgetmanager);
                isNew = true;
            }
            widget.Updated = DateTime.Now;
            if (widgetbase != null)
            {
                widgetbase.WidgetIds.Add(widget.Id);
                widgetmanagercomposite.PreRelease = widgetbase;
                Unit.WidgetManagerRepository.Update(widgetmanagercomposite);
            }
            Unit.Save();

            if (isNew)
            {
                if (siteId != Guid.Empty)
                {
                    return RedirectToAction("WidgetEdit", new { id = widget.Id, widgetid = widgetId, siteid = siteId });
                }
                else
                {
                    return RedirectToAction("WidgetEdit", new { id = widget.Id, widgetid = widgetId });
                }
            }
            else
            {
                if (siteId != Guid.Empty)
                {
                    return RedirectToAction("WidgetEdit", new { id = widgetId, siteid = siteId });
                }
                else
                {
                    return RedirectToAction("WidgetEdit", new { id = widgetId });
                }
            }
        }
        public bool WidgetDeleteFront(Guid id, Guid siteid)
        {
            var sitemanager = Unit.SiteManagerRepository.GetByID(siteid);
            if (sitemanager.PreRelease.WidgetIds.Contains(id))
            {
                sitemanager.PreRelease.WidgetIds.Remove(id);
                sitemanager.PreRelease.Updated = DateTime.Now;
                Unit.SiteManagerRepository.Update(sitemanager);
                Unit.Save();
                return true;
            }
            return false;
        }

        [HttpPost]
        public ActionResult WidgetDelete(FormCollection form)
        {
            var id = form.GetGuid("id", Guid.Empty);
            var siteid = form.GetGuid("siteid", Guid.Empty);
            var compositeid = form.GetGuid("compositeid", Guid.Empty);
            SiteManager sitemanager = null;
            Site site = null;

            var widgetmanager = Unit.WidgetManagerRepository.GetByID(id);
            if (siteid != Guid.Empty)
            {
                sitemanager = Unit.SiteManagerRepository.GetByID(siteid);
                site = sitemanager.PreRelease;
            }

            var widget = widgetmanager.PreRelease;

            var compositewidgetmanager = Unit.WidgetManagerRepository.GetByID(compositeid);
            var widgets = new List<Widget>();
            var widgetmanagers = Unit.WidgetManagerRepository.Get(w => w.SitesettingId == _Globals.Instance.CurrentSiteSettingId).ToList();
            if (compositewidgetmanager != null)
            {
                var composite = compositewidgetmanager.PreRelease as WidgetComposite;
                if (composite.WidgetIds.Contains(id))
                {
                    composite.WidgetIds.Remove(id);
                    compositewidgetmanager.Updated = DateTime.Now;
                    Unit.WidgetManagerRepository.Update(compositewidgetmanager);
                    Unit.Save();
                }
                return RedirectToAction("WidgetEdit", new { id = compositeid, siteid = siteid });
            }
            else if (site != null)
            {
                //remove from one site
                if (site.WidgetIds.Contains(id))
                {
                    site.WidgetIds.Remove(id);
                    sitemanager.PreRelease.Updated = DateTime.Now;
                    Unit.SiteManagerRepository.Update(sitemanager);
                    Unit.Save();
                }
                return RedirectToAction("SiteEdit", new { id = siteid });
            }
            else if (widget != null)
            {
                widget.OnLoad(Unit);
                //delete and remove from all sites
                widget.Sites.ForEach(s =>
                {
                    if (s.WidgetIds.Contains(id))
                    {
                        var sitemanagercurent = Unit.SiteManagerRepository.GetByID(s.Id);
                        sitemanagercurent.PreRelease.WidgetIds.Remove(id);

                        if (sitemanagercurent.Release != null)
                        {
                            sitemanagercurent.Release.WidgetIds.Remove(id);
                            sitemanagercurent.Release.WidgetNavigations.Remove(id);
                        }
                        Unit.SiteManagerRepository.Update(sitemanagercurent);
                    }
                });
                //Remove from all WidgetComposite
                widgetmanagers.ForEach(w =>
                {
                    if (w.PreRelease.Type == "WidgetComposite")
                    {
                        var cw = w.PreRelease as WidgetComposite;

                        cw.WidgetIds.Remove(id);

                        if (w.Release != null)
                        {
                            var cwr = w.Release as WidgetComposite;
                            cwr.WidgetIds.Remove(id);
                        }
                        Unit.WidgetManagerRepository.Update(w);
                    }
                });
                Unit.WidgetManagerRepository.Delete(widgetmanager);
                Unit.Save();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        public ActionResult WidgetImage(string type)
        {
            var dir = Server.MapPath("~/App_Themes/admin/img/widgets/");
            var path = Path.Combine(dir, type.Replace(".", "") + ".svg");
            if (!System.IO.File.Exists(path))
            {
                path = Path.Combine(dir, "default.svg");
            }
            return base.File(path, "image/svg");
        }

        public void WidgetToggleVisibility(Guid id)
        {
            WidgetManager wm = Unit.WidgetManagerRepository.GetByID(id);
            Widget release = wm.Release;
            Widget preRelease = wm.PreRelease;
            preRelease.Visible = !preRelease.Visible;
            if (release != null)
            {
                release.Visible = preRelease.Visible;
            }
            Unit.Save();
        }
        public ActionResult WidgetCopy(Guid id)
        {
            WidgetManager wm = Unit.WidgetManagerRepository.GetByID(id);
            WidgetManager wmc = null;
            string widgetManagerXml = string.Empty;


            XmlSerializer xmlSerializer = new XmlSerializer(wm.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, wm);
                widgetManagerXml = textWriter.ToString();
            }

            using (TextReader reader = new StringReader(widgetManagerXml))
            {
                wmc = (WidgetManager)xmlSerializer.Deserialize(reader);
            }

            wmc.Id = Guid.NewGuid();
            foreach (Widget w in (new Widget[] { wmc.Release, wmc.PreRelease }))
            {
                if (w == null)
                {
                    continue;
                }
                w.Name = w.Name + " Copy";
                w.Id = wmc.Id;
            }


            Unit.WidgetManagerRepository.Insert(wmc);
            Unit.Save();
            return RedirectToAction("WidgetEdit", new { id = wmc.Id });

        }
        public ActionResult WidgetPublish(Guid id, Guid siteid, Guid? widgetid)
        {
            var widgetmanager = Unit.WidgetManagerRepository.GetByID(id);
            widgetmanager.Publish();
            Unit.WidgetManagerRepository.Update(widgetmanager);

            if (widgetmanager.PreRelease.Type == "WidgetComposite")
            {
                var widgetcomposite = widgetmanager.PreRelease as WidgetComposite;
                foreach (var subwidgets in widgetcomposite.WidgetIds)
                {
                    var subwidget = Unit.WidgetManagerRepository.GetByID(subwidgets);
                    subwidget.Publish();
                    Unit.WidgetManagerRepository.Update(subwidget);
                }
            }


            Unit.Save();


            if (widgetid != null && siteid != Guid.Empty)
            {
                return RedirectToAction("WidgetEdit", new { id = widgetid.Value, siteid = siteid });
            }
            else if (widgetid != null)
            {
                return RedirectToAction("WidgetEdit", new { id = widgetid.Value });
            }

            if (siteid != Guid.Empty)
            {
                return RedirectToAction("SiteEdit", new { id = siteid });
            }


            return RedirectToAction("Index");


        }

        public ActionResult WidgetReset(Guid id, Guid siteid, Guid? widgetid)
        {
            var widgetmanager = Unit.WidgetManagerRepository.GetByID(id);
            widgetmanager.Reset();
            Unit.WidgetManagerRepository.Update(widgetmanager);
            Unit.Save();


            if (widgetid != null && siteid != Guid.Empty)
            {
                return RedirectToAction("WidgetEdit", new { id = widgetid.Value, siteid = siteid });
            }
            else if (widgetid != null)
            {
                return RedirectToAction("WidgetEdit", new { id = widgetid.Value });
            }

            if (siteid != Guid.Empty)
            {
                return RedirectToAction("SiteEdit", new { id = siteid });
            }


            return RedirectToAction("Index");


        }
        [HttpPost]
        public string CreateUniqueSiteUrl(string name, bool asjson = true)
        {
            string newurl = name.Replace(" ", "-")
                                .Replace("ä", "ae")
                                .Replace("ö", "oe")
                                .Replace("ü", "ue")
                                .Replace("?", "")
                                .Replace(":", "")
                                .Replace("/", "")
                                .Replace("#", "")
                                .Replace("[", "")
                                .Replace("]", "")
                                .Replace("@", "at")
                                .Replace("$", "")
                                .Replace("&", "und")
                                .Replace("'", "")
                                .Replace("(", "")
                                .Replace(")", "")
                                .Replace("*", "")
                                .Replace("+", "")
                                .Replace(",", "")
                                .Replace(";", "")
                                .Replace("=", "")
                                .Replace("\"", "")
                                .ToLower();
            string append = "";
            bool unique = true;
            int i = 0;

            string[] controllers = { "Admin", "Account", "Reset", "File", "Ajax", "Search", "QRCode", "SendContact", "Content" };
            foreach (var controller in controllers)
            {
                if (newurl.Equals(controller.ToLower()))
                {
                    unique = false;
                    append = "-" + i.ToString();
                    i++;
                }
            }

            while (Unit.SiteManagerRepository.Get(s => s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId && s.PreRelease.Url != null && s.PreRelease.Url.ToLower() == newurl + append).Any())
            {
                unique = false;
                append = "-" + i.ToString();
                i++;
            }

            if (asjson)
            {
                return "{\"url\": \"" + newurl + append + "\", \"unique\": " + unique.ToString().ToLower() + "}";
            }
            else
            {
                return newurl + append;
            }
        }

        public ActionResult SiteEdit(Guid id)
        {
            var sitemanager = Unit.SiteManagerRepository.GetByID(id);
            var site = sitemanager.PreRelease;
            site.OnLoad(Unit);
            var widgets = new List<Widget>();
            var widgetmanagers = Unit.WidgetManagerRepository.Get(w => w.SitesettingId == _Globals.Instance.CurrentSiteSettingId).ToList();
            widgetmanagers.ForEach(w => widgets.Add(w.PreRelease));
            widgets.ForEach(w => w.OnLoad(Unit));
            var model = new SiteViewModel();
            model.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            model.Site = site;
            model.Widgets = widgets;
            model.WidgetsList = model.CurrentSiteSetting.WidgetsList;
            model.IsDistinct = sitemanager.IsDistinct;

            foreach (var navigation in model.CurrentSiteSetting.SiteNavigations)
            {
                if (navigation == id)
                {
                    model.IsNavigation = true;
                    break;
                }
            }

            model.SetSEOModel(model.Site, model.Widgets);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SiteEdit(Guid id, FormCollection form)
        {
            var sitemanager = Unit.SiteManagerRepository.GetByID(id);
            var site = sitemanager.PreRelease;
            site.OnLoad(Unit);
            var widgets = new List<Widget>();
            var widgetmanagers = Unit.WidgetManagerRepository.Get().ToList();
            widgetmanagers.ForEach(w => widgets.Add(w.PreRelease));
            widgets.ForEach(w => w.OnLoad(Unit));
            var inmenu = form.GetBool("inmenu");
            var sitesetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            if (inmenu)
            {
                if (!sitesetting.SiteNavigations.Contains(site.Id))
                {
                    sitesetting.SiteNavigations.Add(site.Id);
                }
            }
            else
            {
                sitesetting.SiteNavigations.Remove(site.Id);
            }
            Unit.SiteSettingRepository.Update(sitesetting);

            site.Updated = DateTime.Now;

            site.Name = form.GetString("name");
            site.Url = form.GetString("url");
            site.UrlPrefix = form.GetString("urlprefix");
            site.Css = form.GetString("css");
            site.JavaScript = form.GetString("javascript");

            site.Title = form.GetString("title");
            site.Description = form.GetString("description");
            site.Keywords = form.GetString("keywords");
            site.Author = form.GetString("author");
            site.Visible = form.GetBool("visible");
            site.HeaderImage = form.GetString("headerimage");
            site.Priority = form.GetDouble("priority");
            site.LastChange = DateTime.Now;
            site.ChangeFaviconOnTabChange = form.GetBool("changefaviconontabchange");
            site.ChangeTitleOnTabChange = form.GetBool("changetitleontabchange");
            site.AnimateAltTitle = form.GetBool("animatealttitle");
            site.AltTitle = form.GetString("alttitle");
            site.TabChangeTimeout = form.GetString("tabchangetimeout");
            site.AnimationSpeed = form.GetString("animationspeed");

            site.HasPassword = form.GetBool("haspassword");
            string newPassword = form.GetString("sitepassword");

            if (site.HasPassword && !string.IsNullOrEmpty(newPassword))
            {
                site.SetPassword(newPassword);
            }

            if (!form.GetString("favicon").IsNullOrEmpty() && site.Favicon != form.GetString("favicon"))
            {
                site.Favicon = form.GetString("favicon");

                new FaviconHelper(Server, site.Favicon);
            }
            else
            {
                site.Favicon = form.GetString("favicon");
            }

            if (!form.GetString("altfavicon").IsNullOrEmpty() && site.AltFavicon != form.GetString("altfavicon"))
            {
                site.AltFavicon = form.GetString("altfavicon");

                new FaviconHelper(Server, site.AltFavicon);
            }
            else
            {
                site.AltFavicon = form.GetString("altfavicon");
            }

            var itemsstring = form.GetString("itemsstring");
            if (!string.IsNullOrEmpty(itemsstring))
            { site.WidgetIds = form.GetGuidList("itemsstring"); }

            site.SiteSettingId = _Globals.Instance.CurrentSiteSettingId;
            sitemanager.PreRelease = site;
            Unit.SiteManagerRepository.Update(sitemanager);
            Unit.Save();

            return RedirectToAction("SiteEdit", new { id = id });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IndexEdit(FormCollection form)
        {

            if (string.IsNullOrEmpty(form["siteorderstring"]))
            {
                return RedirectToAction("Index");
            }
            List<Guid> newOrder = form.GetGuidList("siteorderstring");

            Guid currentSetting = _Globals.Instance.CurrentSiteSettingId;
            SiteSetting setting = Unit.SiteSettingRepository.GetByID(currentSetting);

            var extractedSiteManagers = Unit.SiteManagerRepository.Get(s => s.SiteSettingId == currentSetting);
            Unit.SiteManagerRepository.Delete(extractedSiteManagers);

            var orderedSites = from id in newOrder
                               join sm in extractedSiteManagers on id equals sm.Id
                               select sm;

            Unit.SiteManagerRepository.Insert(orderedSites);

            Unit.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SiteAdd(FormCollection form)
        {
            var startsite = form.GetBool("startsite");
            var sitemanager = new SiteManager();
            var site = new Site();
            site.Name = form.GetString("name", "defaultName");
            site.Type = form.GetString("sitetype", "content");
            var url = string.Empty;
            var urlprefix = string.Empty;
            var siteSettingId = form.GetGuid("siteSettingId", _Globals.Instance.CurrentSiteSettingId);
            if (url.IsNullOrEmpty())
            {
                if (!startsite)
                {
                    url = CreateUniqueSiteUrl(site.Name, false);
                }
            }
            if (startsite)
            {
                var secondstartsite = Unit.SiteManagerRepository.Find(s => s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId && s.PreRelease.Url == "");

                if (secondstartsite != null)
                {
                    url = CreateUniqueSiteUrl(site.Name, false);
                }

            }
            site.SiteSettingId = siteSettingId;

            site.Url = url;
            site.UrlPrefix = urlprefix;
            site.LastChange = DateTime.Now;
            site.Id = sitemanager.Id;
            site.OnSave(Unit);
            sitemanager.PreRelease = site;
            sitemanager.SiteSettingId = siteSettingId;

            Unit.SiteManagerRepository.Insert(sitemanager);
            Unit.Save();

            /// Automisch Widget erstellen
            FormCollection formWidget = new FormCollection();
            formWidget["siteId"] = site.Id.ToString();
            formWidget["name"] = "MyWidget";
            formWidget["title"] = "Welcome";
            formWidget["widgetType"] = "WidgetContent";

            RedirectToRouteResult result = (RedirectToRouteResult)WidgetAdd(formWidget);

            WidgetContent widget = (WidgetContent)Unit.WidgetManagerRepository.Find(w => w.Id == (Guid)result.RouteValues["id"]).PreRelease;
            widget.Content = "First example Widget";

            Unit.Save();

            return RedirectToAction("SiteEdit", new { site.Id });
        }

        [HttpPost]
        public ActionResult SiteDelete(FormCollection form)
        {

            var id = form.GetGuid("id", Guid.Empty);

            var sitemanager = Unit.SiteManagerRepository.GetByID(id);
            var sitesetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            if (sitemanager != null)
            {
                Unit.SiteManagerRepository.Delete(sitemanager);
                sitesetting.SiteNavigations.Remove(id);
                Unit.Save();
            }

            return RedirectToAction("Index");
        }

        public ActionResult SitePublish(Guid id)
        {

            var sitemanager = Unit.SiteManagerRepository.GetByID(id);
            sitemanager.Publish();
            Unit.SiteManagerRepository.Update(sitemanager);
            var widgetmanager = Unit.WidgetManagerRepository.Get(w => w.SitesettingId == _Globals.Instance.CurrentSiteSettingId).ToList();
            foreach (var widget in widgetmanager)
            {
                if (sitemanager.PreRelease.WidgetIds.Contains(widget.Id))
                {
                    widget.Publish();
                    Unit.WidgetManagerRepository.Update(widget);
                    if (widget.PreRelease.Type == "WidgetComposite")
                    {
                        var widgetcomposite = widget.PreRelease as WidgetComposite;
                        foreach (var subwidgets in widgetcomposite.WidgetIds)
                        {
                            var subwidget = Unit.WidgetManagerRepository.GetByID(subwidgets);
                            subwidget.Publish();
                            Unit.WidgetManagerRepository.Update(subwidget);
                        }
                    }
                }
            }
            Unit.Save();

            return RedirectToAction("Index");
        }

        public ActionResult Settings()
        {
            var model = new AdminViewModel();
            model.CurrentUser = _Globals.Instance.GetLoginUser(Unit);
            model.CurrentUser.Password = string.Empty;
            model.Users = Unit.UserRepository.Get().ToList();
            return View(model);
        }

        public ActionResult Layouts(string name, string selectedTab, string alert)
        {
            var model = new AdminViewModel();

            var path = Server.MapPath("~/Views/Shared");
            var dir = new DirectoryInfo(path);
            var sitemanagers = Unit.SiteManagerRepository.Get(f => f.SiteSettingId == _Globals.Instance.CurrentSiteSettingId).ToList();
            var sites = new List<Site>();
            sitemanagers.ForEach(s => sites.Add(s.PreRelease));
            model.Layouts = dir.GetFiles().ToList();
            model.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            model.Sites = sites;
            model.SiteSettings = Unit.SiteSettingRepository.Get().ToList();

            var websitePath = Server.MapPath("~/Views/" + _Globals.Instance.CurrentSiteSettingId);
            if (!System.IO.File.Exists(websitePath))
            {
                Directory.CreateDirectory(websitePath);
            }
            var sharedPath = Server.MapPath("~/Views/Shared");
            var homePath = Server.MapPath("~/Views/Home");
            var widgetsPath = Server.MapPath("~/Views/Widgets");

            var webConfigPath = Server.MapPath("~/Web.config");


            var siteModel = new LayoutEditorModel(websitePath, sharedPath, homePath, widgetsPath);
            siteModel.ConfigFiles.Insert(webConfigPath);
            siteModel.CurrentTabName = selectedTab;
            siteModel.Alert = alert;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(selectedTab))
            {
                var fullPhysicalFilePath = Server.MapPath(name);
                if (System.IO.File.Exists(fullPhysicalFilePath))
                {
                    siteModel.OpenFilePath = name;
                    siteModel.OpenFileContent = System.IO.File.ReadAllText(fullPhysicalFilePath);
                }
            }

            model.SiteLayoutEditor = siteModel;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Layouts(string name, FormCollection form)
        {
            var layoutname = form.GetString("layoutname");
            var layoutcontent = form.GetString("layoutcontent");
            var physicalPath = Server.MapPath(layoutname);
            var tab = form.GetString("tab");

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.WriteAllText(physicalPath, layoutcontent, System.Text.UTF8Encoding.UTF8);
            }

            return RedirectToAction("Layouts", new { name = layoutname, selectedTab = tab, alert = "File saved!" });
        }

        public ActionResult UserEdit(Guid? id)
        {
            bool isLoggedInAdmin = _Globals.Instance.GetLoginUser(Unit).IsAdmin;
            var model = new AdminViewModel();

            User item;
            if (id == null)
            {
                item = new User();
                item.IsAdmin = true;
            }
            else
            {
                item = Unit.UserRepository.GetByID(id);

            }
            if (item != null)
            {
                item.Password = string.Empty;
                model.IsLoggedInAdmin = isLoggedInAdmin;
                model.CurrentUser = item;
                model.Users.Add(item);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UserEdit(Guid id, FormCollection form)
        {
            bool isLoggedInAdmin = _Globals.Instance.GetLoginUser(Unit).IsAdmin;
            if (!isLoggedInAdmin)
            {
                return RedirectToAction("SiteSettings");
            }
            bool doInsert = false;
            User item = Unit.UserRepository.GetByID(id);
            if (item == null)
            {
                item = new User();
                doInsert = true;

            }

            item.UserName = form.GetString("username");
            item.Email = form.GetString("email");
            item.FirstName = form.GetString("firstname");
            item.LastName = form.GetString("lastname");
            item.IsAdmin = form.GetBool("isadmin");
            string newPassword = form.GetString("password");
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                item.SetPassword(newPassword);
            }

            if (doInsert)
            {
                Unit.UserRepository.Insert(item);
            }
            else
            {
                Unit.UserRepository.Update(item);
            }

            Unit.Save();

            return RedirectToAction("SiteSettings");
        }

        public ActionResult UserDelete(FormCollection form)
        {

            Guid id = form.GetGuid("id", Guid.Empty);
            if (_Globals.Instance.CurrentLoginUserId == id)
                return RedirectToAction("SiteSettings");
            if (!_Globals.Instance.GetLoginUser(Unit).IsAdmin)
            {
                return RedirectToAction("SiteSettings");
            }


            User user = Unit.UserRepository.GetByID(id);
            if (user != null)
            {
                Unit.UserRepository.Delete(user);
                Unit.Save();

            }

            return RedirectToAction("SiteSettings");
        }
        public ActionResult RedirectionEdit(Guid? id)
        {

            var model = new AdminViewModel();
            model.CurrentSiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);

            List<Redirection> redirections = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId).Redirections;
            Redirection redirection = null;
            if (id != null)
            {
                redirection = redirections.Find(s => s.Id == id);
            }
            if (redirection == null)
            {
                redirection = new Redirection();
            }

            model.Redirection = redirection;

            return View(model);

        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RedirectionEdit(Guid? id, FormCollection form)
        {
            string newUrl = form["newSEOUrl"];
            string oldUrl = form["oldSEOUrl"];


            List<Redirection> redirections = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId).Redirections;
            Redirection redirection = redirections.Find(s => s.Id == id);

            if (redirection == null)
            {
                redirection = new Redirection();
                redirection.Id = Guid.NewGuid();
                redirections.Add(redirection);
            }

            redirection.newSEOUrl = newUrl;
            redirection.oldSEOUrl = oldUrl;

            Unit.Save();

            return RedirectToAction("Index");
        }

        public ActionResult RedirectionDelete(Guid? id)
        {
            Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId).Redirections.RemoveAll(s => s.Id == id);
            Unit.Save();

            return RedirectToAction("Index");
        }

        public ActionResult CreateBackup(Guid id, FormCollection form)
        {
            var name = form.GetString("name");
            ErrorCode ec = BackupController.Store(Unit, name, id);

            if (ec == ErrorCode.RUNNING)
            {
                return RedirectToAction("ViewBackups");
            }
            return Content("Sorry, something went wrong!");
        }

        public ActionResult ViewBackups()
        {
            var model = new BackupViewModel();
            string path = Server.MapPath("~/App_Data/Backup/");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            model.Backups = StoreProgress.GetAllUnsuccessfulStoreTaskNames(Directory.GetDirectories(path).ToArray().Select(s => Path.GetFileName(s)));

            return View(model);
        }

        public ActionResult RestoreBackup(FormCollection form)
        {
            var name = form.GetString("name");

            Guid ssid = Guid.Empty;
            ErrorCode ec = ErrorCode.SUCCESS;
            if ((ec = BackupController.Restore(Unit, name, out ssid)) != ErrorCode.SUCCESS)
            {
                return Content("Sorry, something went wrong!");
            }
            FormCollection formSCSS = new FormCollection();
            formSCSS.Add("selectCurrentSiteSetting", ssid.ToString());
            formSCSS.Add("returnUrl", "/admin");
            return SelectSiteSetting(formSCSS);
        }

        public ActionResult DownloadBackup(string name)
        {
            FileStream file;
            string filename;
            ErrorCode ec = ErrorCode.UNKNOWN;
            if (!(ec = BackupController.Download(name, out file, out filename)).HasFailure())
            {
                var result = File(file, "application/x-zip-compressed", filename);
                return result;
            }
            if (file != null)
            {
                file.Close();
            }
            return Content(ec.Message());
        }

        public ActionResult GetState(string name)
        {

            string styleClass = string.Empty;
            ErrorCode ec = StoreProgress.Get(name);

            if (ec == ErrorCode.NULL)
            {
                return Content("<div class='success'> Fertig </div>");
            }

            if (ec == ErrorCode.RUNNING)
            {
                return Content("<div class='progress'>" + ec.Message() + " </div>");
            }

            return Content("<div class='failure'>" + ec.Message() + " </div>");

        }

        public ActionResult DeleteBackup(string name)
        {
            BackupController.Delete(name);
            return RedirectToAction("ViewBackups");
        }

        public ActionResult DownloadLog()
        {
            string path = Server.MapPath("~/App_Data/perflog.log");
            if (System.IO.File.Exists(path))
            {
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                var result = File(file, "application/text", "perflog.log");
                return result;
            }
            Stream emptyfile = FileStream.Null;
            var emptyresult = File(emptyfile, "application/text", "perflog.log");
            if (emptyfile != null)
            {
                emptyfile.Close();
            }
            return emptyresult;
        }

    }
}