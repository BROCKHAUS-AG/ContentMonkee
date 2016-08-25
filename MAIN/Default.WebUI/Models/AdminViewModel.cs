using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BAG.Common.Data.Entities;
using BAG.Common;
using System.Xml.Serialization;

namespace Default.WebUI.Models
{
    public class AdminViewModel : IWidgetContainer, ISiteSelection
    {
        public AdminViewModel()
        {
            Sites = new List<Site>();
            Users = new List<User>();
            Widgets = new List<Widget>();
            SiteSettings = new List<SiteSetting>();
            Layouts = new List<System.IO.FileInfo>();

            Redirections = new List<Redirection>();
            NewWidgets = new List<Widget>();

            var type = typeof(Widget);
            var attributes = type.GetCustomAttributes(typeof(XmlIncludeAttribute), true).ToList();
            attributes.ForEach(a =>
            {
                var t = ((XmlIncludeAttribute)a).Type;
                NewWidgets.Add((Widget)Activator.CreateInstance(t));
            }
            );
            NewWidgets = NewWidgets.OrderBy(o => o.Name).ToList();
        }
        public List<Site> Sites { get; set; }
        public List<SiteManager> SiteManagers { get; set; }
        public List<User> Users { get; set; }
        public User CurrentUser { get; set; }
        public List<Widget> Widgets { get; set; }
        public List<Widget> NewWidgets { get; set; }
        public List<SiteSetting> SiteSettings { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Redirection> Redirections { get; set; }
        public Widget CurrentWidget { get; set; }
        public SiteSetting CurrentSiteSetting { get; set; }
        public Guid CurrentBindingId{ get; set; }
        public Employee CurrentEmployee { get; set; }
        public List<System.IO.FileInfo> Layouts { get; set; }
        public string LayoutName { get; set; }
        public string LayoutContent { get; set; }
        public string[] Templates { get; set; }
        public Redirection Redirection { get; set; }
        public List<WidgetManager> WidgetManagers { get; set; }

        public LayoutEditorModel SiteLayoutEditor { get; set; }

        public object Model { get; set; }

        public Guid SiteId
        {
            get { return Guid.Empty; }
        }

        public bool IsLoggedInAdmin { get; set; }

        public bool IsCurrentUserLoggedIn
        {
            get
            {
                if (CurrentUser == null)
                    return false;

                return CurrentUser.Id == _Globals.Instance.CurrentLoginUserId;
            }
        }
    }
}