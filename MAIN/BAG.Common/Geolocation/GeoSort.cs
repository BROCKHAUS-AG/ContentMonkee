using System;
using System.Collections.Generic;
using System.Linq;
using BAG.Framework.Geolocation.Models;
using System.Diagnostics;

namespace BAG.Common.Geolocation
{
    /* GeoSorting mittles Haversine Algorithmus */
    /* Quelle: http://damien.dennehy.me/blog/2011/01/15/haversine-algorithm-in-csharp/ */
    public class GeoSort
    {
        private const double EARTH_RADIUS_KM = 6371;

        public static IEnumerable<ILocatable> SortByDistanceAsc(IEnumerable<ILocatable> locations, Coordinate location)
        {
            List<SortedLocateable> locateables = new List<SortedLocateable>();
            locations.ToList().ForEach(e =>
            {
                double distance = 0;
                if (e.Latitude == 0 && e.Longitude == 0 && !string.IsNullOrEmpty(e.Zip) && e.Zip.Length == 5)
                {
                    var coords = GeoInfo.GetUserPositionFromString(e.Zip);
                    if (coords != null)
                    {
                        e.Latitude = coords.Latitude;
                        e.Longitude = coords.Longitude;
                    }
                    else
                    {
                        Debug.WriteLine("Error in GeoSort> " + e.Zip + " coords not found");
                    }
                }

                distance = GetDistance(e, location);

                locateables.Add(new SortedLocateable()
                {
                    Distance = distance,
                    Locatable = e
                });
            });

            return locateables.OrderBy(l => l.Distance).Select(l => l.Locatable);
        }

        private static double GetDistance(ILocatable locatableObject, Coordinate location)
        {
            if (location == null)
                return -1;

            double dLat = ToRad(location.Latitude - locatableObject.Latitude);
            double dLon = ToRad(location.Longitude - locatableObject.Longitude);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(ToRad(locatableObject.Latitude)) * Math.Cos(ToRad(location.Latitude)) *
                       Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = EARTH_RADIUS_KM * c;
            return distance;
        }

        private static double ToRad(double input)
        {
            return input * (Math.PI / 180);
        }
    }

    public class SortedLocateable
    {
        public double Distance { get; set; }

        public ILocatable Locatable { get; set; }
    }

    //public class LocationItem : ILocatable
    //{
    //    public double Latitude
    //    {
    //        get;
    //        set;
    //    }

    //    public double Longitude
    //    {
    //        get;
    //        set;
    //    }

    //    public string Zip
    //    {
    //        get;
    //        set;
    //    }
    //}
}
