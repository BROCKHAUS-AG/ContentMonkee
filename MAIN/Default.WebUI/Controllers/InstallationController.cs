using BAG.Common.Data;
using BAG.Common.Data.Entities;
using Default.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Default.WebUI.Controllers
{
    public class InstallationController : Controller
    {
        // GET: Installation
        public ActionResult Index()
        {
            UnitOfWork unit = new UnitOfWork();
            if (unit.UserRepository.Get().Count() > 0)
            {
                return RedirectToAction("index", "home");
            }
            return View(new InstallationViewModel());
        }

        [HttpPost]
        public JsonResult Install(InstallationViewModel model)
        {
            UnitOfWork unit = new UnitOfWork();
            if (unit.UserRepository.Get().Count() > 0)
            {
                return Json("Access denied!");
            }

            User user = new User();
            user.Email = model.User.Mail;
            user.SetPassword(model.User.Password);
            user.FirstName = model.User.FirstName;
            user.LastName = model.User.LastName;
            user.IsAdmin = true;
            user.UserName = model.User.UserName;
            unit.UserRepository.Insert(user);

            IEnumerable<SiteSetting> siteSettings = unit.SiteSettingRepository.Get().Where(ss => ss.Id != Guid.Empty);
            SiteSetting siteSetting = siteSettings.Count() == 1 ? siteSettings.First() : null;
            
            if (siteSetting!=null)
            {
                siteSetting.Name = model.Settings.Name;
                siteSetting.MainDomain = model.Settings.Domain;
                siteSetting.Bindings = "localhost, " + model.Settings.Domain;
                siteSetting.Author = user.FirstName + " " + user.LastName;                
            }

            unit.Save();
                        
            FormCollection formWidget = new FormCollection();
            formWidget["email"] = model.User.Mail;
            formWidget["password"] = model.User.Password;

            AccountController ac = new AccountController();
            RedirectToRouteResult result = (RedirectToRouteResult)(ac.Login(formWidget));

            return Json("Success");
        }
    }
}