using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Handler
{
    /// <summary>
    /// Summary description for FontsHandler
    /// </summary>
    public class FontsHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string path = HttpContext.Current.Request.Url.AbsolutePath;
            string[] patharr = path.Split('/');
            string font = patharr.Last();


            if (patharr[1].Equals("admin"))
            {
                context.Response.WriteFile("~/fonts/" + font);
            }

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