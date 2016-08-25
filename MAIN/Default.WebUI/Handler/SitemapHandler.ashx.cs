using BAG.Common;
using BAG.Common.Data;
using BAG.Common.Sitemap;
using Default.WebUI.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Default.WebUI.Handler
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für SitemapHandler
    /// </summary>
    public class SitemapHandler : IHttpHandler, IReadOnlySessionState
    {
        private UnitOfWork unit = new UnitOfWork();
        public void ProcessRequest(HttpContext context)
        {
            var sitemap = new SitemapGenerator(); 
            
            context.Response.ContentType = "Text/xml";
            context.Response.Write(sitemap.getSitemap());
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}