using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace BAG.Common.Sitemap
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class UrlSet
    {
        private readonly List<SiteUrl> urls = new List<SiteUrl>();
        [XmlElement("url")]
        public List<SiteUrl> Urls { get { return urls; } }


        public void CreateItem(string loc, string priority = null)
        {
            var url = new SiteUrl();
            url.Location = loc;
            url.ChangeFrequency = "weekly";
            url.Priority = string.IsNullOrEmpty(priority) ? "1.00" : priority;
            urls.Add(url);
        }

        public void WriteResponse(HttpResponse response)
        {
            response.Clear();
            response.ContentType = "text/xml";
            response.ContentEncoding = System.Text.Encoding.UTF8;
            response.Write(this.ToXmlString());
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public string ToXmlString()
        {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.Encoding = new UTF8Encoding(false);
            using (StringWriter str = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(str, settings))
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                //ns.Add(_newsSiteMapPrefix, _newsSiteMapSchema);

                XmlSerializer xser = new XmlSerializer(typeof(UrlSet));
                xser.Serialize(writer, this, ns);
                return str.ToString();
            }

        }
        public override string ToString()
        {
            return ToXmlString();
        }
    }

}
