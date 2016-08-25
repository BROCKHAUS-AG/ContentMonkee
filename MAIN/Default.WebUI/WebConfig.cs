using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Default.WebUI
{
  public static class WebConfig
  {
    static WebConfig()
    {
      Domain = WebConfigurationManager.AppSettings["BAG.Domain"];
      IsProductiveEnvironment = bool.Parse(WebConfigurationManager.AppSettings["BAG.ProductiveMode"]);
    }

    public static string Domain { get; private set; }

    public static bool IsProductiveEnvironment { get; private set; }

  }
}