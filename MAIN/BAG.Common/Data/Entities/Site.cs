using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    public class Site : Base, ILazyEntity
    {
        public Site()
            : base()
        {
            Widgets = new List<WidgetManager>();
            WidgetIds = new List<Guid>();
            WidgetNavigations = new List<Guid>();
            Visible = true;
            Lang = "de";
            Priority = 1;
            ChangeFaviconOnTabChange = false;
            ChangeTitleOnTabChange = false;
            AnimateAltTitle = false;
            TabChangeTimeout = "5";
            AnimationSpeed = "10";
        }

        public string Lang { get; set; }

        public Guid SiteSettingId { get; set; }




        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Keywords { get; set; }


        public string Favicon { get; set; }
        public string TabChangeTimeout { get; set; }
        public bool ChangeFaviconOnTabChange { get; set; }
        public string AltFavicon { get; set; }
        public bool ChangeTitleOnTabChange { get; set; }
        public bool AnimateAltTitle { get; set; }
        public string AnimationSpeed { get; set; }
        public string AltTitle { get; set; }
        public bool HasPassword { get; set; }
        public string SitePassword { get; set; }
        /// <summary>
        /// 0.1...1.0
        /// </summary>
        public double Priority { get; set; }
        /// <summary>
        /// always
        ///         hourly
        ///         daily
        /// weekly
        /// monthly
        /// yearly
        /// never
        /// </summary>
        public string ChangeFreqerty { get; set; }

        //public bool IsSitemapVisible { get; set; }
        //public bool IsSearchVisible { get; set; }


        private string url = string.Empty;
        public string Url
        {
            get { return url; }
            set
            {
                if (url == null)
                {
                    url = null;
                }
                url = value.ToLower();
            }
        }
        public string UrlFull { get { return UrlPrefix + Url; } }
        string urlprefix = string.Empty;
        public string UrlPrefix
        {
            get { return urlprefix; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    if (!value.EndsWith("/"))
                        value = value + "/";
                urlprefix = value;
            }
        }

        public string Name { get; set; }

        //public string Controller { get; set; }
        //public string Action { get; set; }
        public string Type { get; set; }

        public string Css { get; set; }
        public string JavaScript { get; set; }

        public bool Visible { get; set; }

        public List<Guid> WidgetIds { get; set; }

        public string HeaderImage { get; set; }
        public List<Guid> WidgetNavigations { get; set; }
        public DateTime LastChange { get; set; }

        [NotMapped]
        [XmlIgnore]
        public List<WidgetManager> Widgets { get; set; }


        [NotMapped]
        [XmlIgnore]
        public Boolean Loaded { get; private set; }
        public void SetPassword(string pHash)
        {
            SitePassword = Cryptography.Hash.ComputeHash(pHash);
        }

        public void OnLoad(UnitOfWork unit)
        {
            if (Loaded)
            {
                return;
            }
            foreach (var id in WidgetIds)
            {
                var widget = unit.WidgetManagerRepository.GetByID(id);
                if (widget != null)
                {
                    Widgets.Add(widget);
                }

            }
            Widgets.ForEach(w => w.OnLoad(unit));
            Loaded = true;
        }

        public void OnSave(UnitOfWork unit)
        {
            WidgetIds = Widgets.Select(w => w.Id).ToList();
        }

        public override List<BaseEntity> RemapAllGuids(Func<Guid, bool, Guid> map)
        {
            base.RemapAllGuids(map);
            WidgetNavigations = WidgetNavigations.Select(wn => map(wn, false)).ToList();
            WidgetIds = WidgetIds.Select(w => map(w, false)).ToList();
            SiteSettingId = map(SiteSettingId, false);

            return null;
        }
    }
}
