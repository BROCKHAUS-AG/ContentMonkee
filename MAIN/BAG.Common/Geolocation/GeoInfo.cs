using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BAG.Framework.Geolocation;
using BAG.Framework.Geolocation.Models;
using BAG.Framework.Geolocation.Provider.Google;
using BAG.Framework.Geolocation.Provider.MaxMind;
using System.Diagnostics;
using System.Web.Configuration;

namespace BAG.Common.Geolocation
{
    public static class GeoInfo
    {
        public const string VisitorLocationKey = "EXACT_VISITOR_LOCATION";

        public static Coordinate UserPosition
        {
            get
            {
                Coordinate coordinate = null;

                if (HttpContext.Current.Session[VisitorLocationKey] != null)
                {
                    coordinate = HttpContext.Current.Session[VisitorLocationKey] as Coordinate;
                }
                else
                {
                    coordinate = GetUserPositionByIp();
                }

                return coordinate;
            }
        }

        private static Coordinate GetUserPositionByIp()
        {
            Locator<MaxMindApiClient> locator = new Locator<MaxMindApiClient>();
            return locator.GetCoordinateFromStringExpression(HttpContext.Current.Request.UserHostAddress);
        }

        public static Coordinate GetUserPositionFromString(string expression)
        {
            Coordinate result = new Coordinate();
            if (WebConfigurationManager.AppSettings["BAG.Framework.Geolocation.Enabled"] != "false")
            {
                Locator<MapsApiClient> locator = new Locator<MapsApiClient>();
                result = locator.GetCoordinateFromStringExpression(expression);

                if (WebConfigurationManager.AppSettings["BAG.Framework.Geolocation.Google.EnableFallback"] != "false")
                {
                    //if not found, try next.
                    if (result == null) // not found, try to search next zip code.
                    {
                        Debug.WriteLine("ZipCode not found " + expression);
                        int zip;
                        if (int.TryParse(expression, out zip))
                        {
                            int max_counter = 1000;

                            if (zip > 1000 && zip < 99999)
                                do
                                {
                                    max_counter--;
                                    zip++;
                                    result = locator.GetCoordinateFromStringExpression(zip.ToString("D5"));
                                }
                                while (result == null && max_counter >= 0);

                            if (result != null)
                                Debug.WriteLine("ZipCode not fount " + expression + " next found at " + zip.ToString("D5"));
                        }
                    }
                }
            }
            return result;
        }
    }
}
