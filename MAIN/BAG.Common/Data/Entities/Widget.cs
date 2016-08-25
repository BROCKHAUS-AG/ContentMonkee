using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    [XmlInclude(typeof(WidgetContent))]
    [XmlInclude(typeof(WidgetCarusel))]
    [XmlInclude(typeof(WidgetComposite))]
    [XmlInclude(typeof(WidgetMap))]
    [XmlInclude(typeof(WidgetContact))]
    [XmlInclude(typeof(WidgetVCard))]
    [XmlInclude(typeof(WidgetTitle))]
    [XmlInclude(typeof(WidgetList))]
    [XmlInclude(typeof(WidgetPageHeader))]
    [XmlInclude(typeof(WidgetPageFooter))]
    [XmlInclude(typeof(WidgetGrid))]
    public abstract class Widget : Base, ILazyEntity
    {
        public Widget()
        {
            var name = this.GetType().Name;
            this.Name = name;
            this.Type = name;
            this.Partial = name + ".cshtml";
          

            Visible = true;
            Sites = new List<Site>();
        }



        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaAuthor { get; set; }
        public string MetaKeywords { get; set; }
        public double MetaPriority { get; set; }

        public Boolean Visible { get; set; }

        public string Name { get; set; }


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

        public string Type { get; set; }
        public virtual string Partial { get; set; }
        public SocialMedia SocialMedia { get; set; }
        public Guid SitesettingId { get; set; }
        [XmlIgnore]
        [NotMapped]
        public List<Site> Sites { get; set; }

        [XmlIgnore]
        [NotMapped]
        public string WidgetDescription { get; set; }

        [NotMapped]
        public bool IsDefaultPartial
        {
            get
            {
                return Type + ".cshtml" == Partial;
            }
        }


        [NotMapped]
        [XmlIgnore]
        public Boolean Loaded { get; private set; }

        public virtual void OnLoad(UnitOfWork unit)
        {
            if (Loaded)
            {
                return;
            }
            var sitemanagers = unit.SiteManagerRepository.Get(s => s.PreRelease.WidgetIds.Contains(this.Id)).ToList();
            var sites = new List<Site>();
            sitemanagers.ForEach(s => sites.Add(s.PreRelease));
            Sites = sites;
            Loaded = true;
        }

        public virtual void OnSave(UnitOfWork unit)
        {

        }
        public virtual bool IsDistinct()
        {
            return false;
        }

        public abstract string GetContent();
        public string GetContentExceptTags()
        {
            var content = GetContent();
            if (content == null)
            {
                return null;
            }
            var result = Regex.Replace(content, "<.*?>", " ");
            result = Regex.Replace(result, @"\s+", " ");
            return result;
        }

        public override List<BaseEntity> RemapAllGuids(Func<Guid, bool, Guid> map)
        {
            base.RemapAllGuids(map);
            SitesettingId = map(SitesettingId, false);
            return null;
        }
    }
}
