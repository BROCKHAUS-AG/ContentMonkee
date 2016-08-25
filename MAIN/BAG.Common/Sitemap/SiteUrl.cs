using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BAG.Common.Sitemap
{
    public class SiteUrl
    {
        [XmlElement("loc")]
        public string Location { get; set; }
        [XmlElement("priority")]
        public string Priority { get; set; }
        [XmlElement("news", Namespace = "http://www.google.com/schemas/sitemap-news/0.9")]
        public string News { get; set; }
        [XmlElement("lastmod")]
        public DateTime? LastModified { get; set; }
        [XmlElement("changefreq")]
        public string ChangeFrequency { get; set; }

        public bool ShouldSerializeLastModified() { return LastModified.HasValue; }
    }
}
