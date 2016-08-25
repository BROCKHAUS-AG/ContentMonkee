using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace BAG.Common.Sitemap
{

    public class SiteMapResult : ActionResult
    {
        public UrlSet UrlSet { get; set; }

        public SiteMapResult(UrlSet set)
        {
            UrlSet = set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.Encoding = new UTF8Encoding(false);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //ns.Add(_newsSiteMapPrefix, _newsSiteMapSchema);
            XmlSerializer xser = new XmlSerializer(typeof(UrlSet));
            //xser.Serialize(writer, this, ns);
            xser.Serialize(context.HttpContext.Response.OutputStream, UrlSet, ns);
        }
    }

}
