using BAG.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Extensions
{
    /*
    routes.MapRoute(
          "SEOFriendlyRoute",
          "Content/{SEOUrl}"
         ).RouteHandler = new UrlRouteHandler();
     * http://www.4sln.com/Articles/seo-friendly-url-rewriting-in-mvc-by-keeping-references-in-database
    */
    public class UrlRouteHandler : System.Web.Mvc.MvcRouteHandler
    {
        //UnitOfWork unit = new UnitOfWork();
        //EF.TESTEntities con = new TESTEntities();
        protected override IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)

        {

            var SEOUrl = requestContext.RouteData.Values["SEOUrl"] as string;
            var controller = requestContext.RouteData.Values["Controller"] as string;
            var action = requestContext.RouteData.Values["Action"] as string;

            SEOUrl = SEOUrl == null ? null : SEOUrl.ToLower();

            //after Detail/
            //friendlyUrl = friendlyUrl.Substring(friendlyUrl.IndexOf('/') + 1);

            //URLRewrite urlrewrite = null;

            //if (!string.IsNullOrEmpty(friendlyUrl))
            //  urlrewrite = con.URLRewrites.Where(p => p.PageSEOUrl == friendlyUrl).FirstOrDefault();

            //UnitOfWork unit = new UnitOfWork();
            //if (unit.UserRepository.Get().Count() <= 1)
            //{
            //    requestContext.RouteData.Values["controller"] = "Installation";
            //    requestContext.RouteData.Values["action"] = "Index";
            //    return base.GetHttpHandler(requestContext);
            //}

            requestContext.RouteData.Values["controller"] = controller;
            if (action == null)
                requestContext.RouteData.Values["action"] = "Index";
            else
                requestContext.RouteData.Values["action"] = action;
            if (controller == "Home" && !string.IsNullOrEmpty(SEOUrl))
                requestContext.RouteData.Values["action"] = "Index";

            requestContext.RouteData.Values["urlrewrite"] = action;
            requestContext.RouteData.Values["name"] = SEOUrl;


            return base.GetHttpHandler(requestContext);
        }
    }
}