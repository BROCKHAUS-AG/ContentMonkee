using Default.WebUI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Default.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("sitemap.xml");
            routes.IgnoreRoute("robots.txt");
            routes.IgnoreRoute("*.png");
            routes.IgnoreRoute("*.jpg");
            routes.IgnoreRoute("*.gif");
            routes.IgnoreRoute("*.jpeg");
            routes.IgnoreRoute("*.tiff");
            routes.IgnoreRoute("*.tff");
            routes.IgnoreRoute("*.woff");
            routes.IgnoreRoute("admin/font/{*pathInfo}");
            routes.IgnoreRoute("admin/images/{*pathInfo}");
            routes.IgnoreRoute("img/{*pathInfo}");
            routes.IgnoreRoute("content/{*pathInfo}");

            routes.MapRoute(
                       name: "errorcode",
                        url: "error/{errorcode}",
                        defaults: new { controller = "Home", action = "Error", errorcode = UrlParameter.Optional }
               );
            routes.MapRoute(
                       name: "SiteVerification",
                        url: "Account/SiteVerifyPassword/{sid}",
                        defaults: new { controller = "Account", action = "SiteVerifyPassword", sid = UrlParameter.Optional }
               );

            // für infinite scroll
            routes.MapRoute(
                       name: "widgetList",
                       url: "widgetList/{hasPrinted}/{amount}/{SEOUrl}",
                       defaults: new { controller = "Home", action = "GetWidgets", SEOUrl = UrlParameter.Optional, hasPrinted = UrlParameter.Optional, amount = UrlParameter.Optional }
               );
            routes.MapRoute(
                       name: "getWidgetByFullUrl",
                       url: "getWidgetByFullUrl/{*SEOUrl}",
                       defaults: new { controller = "Home", action = "GetWidgetByFullUrl", SEOUrl = UrlParameter.Optional}
               );
            

            (new string[] { "Admin", "Account", "Reset", "File", "Ajax", "Search" ,"QRCode", "SendContact" }).ToList().ForEach(name =>
           {
               routes.MapRoute(
                       name: name,
                       url: name + "/{action}/{id}",
                       defaults: new { controller = name, action = "Index", id = UrlParameter.Optional }
               );
           });

            routes.MapRoute(null, "Thumbnails/{tmb}", new { controller = "File", action = "Thumbs", tmb = UrlParameter.Optional });
            //  routes.MapRoute(null, "admin/thumbnails/{tmb}", new { controller = "File", action = "Thumbs", tmb = UrlParameter.Optional });
                       

            //Home Controller
            routes.MapRoute(
              name: "Default",
              url: "{*SEOUrl}",
              defaults: new { controller = "Home", SEOUrl = UrlParameter.Optional}
              ).RouteHandler = new UrlRouteHandler();

            routes.LowercaseUrls = true;
        }
    }
}
