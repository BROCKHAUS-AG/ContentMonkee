namespace BAG.Framework.Geolocation.Provider.Google
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    //using System.Web.Helpers;

    using BAG.Framework.Geolocation.Models;
    using BAG.Framework.Geolocation.Provider.Google.Models;
    using Newtonsoft.Json;

    public class MapsApiClient : ILocationService
    {
        public const string CountrySearchQueryExtension = " Deutschland";
        public const string MapsApiServiceBaseAddress = "http://maps.googleapis.com/maps/api/geocode/json?address=";
        public const string DirectionsServiceBaseAddress = "http://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&sensor=false";

        public const double EarthMeanRadius = 6372.8;

        private readonly WebClient webClient;

        public MapsApiClient()
        {
            this.webClient = new WebClient();
        }

        private string CreateDirectionsQueryUri(Coordinate origin, Coordinate destination)
        {
            return string.Format(
                DirectionsServiceBaseAddress,
                Uri.EscapeDataString(origin.ToString()),
                Uri.EscapeDataString(destination.ToString()));
        }

        public bool IsExcludedExpression { get; set; }

        public Coordinate GetCoordinateFromStringExpression(string zip)
        {
            MapsApiResult resultObject = this.Query(zip);
            Coordinate coordinate = null;

            if (resultObject != null && resultObject.status == "OK")
            {
                var firstOrDefault = resultObject.results
                    .FirstOrDefault(obj => this.ContainsGermanResultObject(obj.address_components));

                if (firstOrDefault != null)
                {
                    Location currentLocation = firstOrDefault
                                                   .geometry
                                                   .location ?? new Location();
                    coordinate = new Coordinate()
                                     {
                                         Latitude = currentLocation.lat,
                                         Longitude = currentLocation.lng
                                     };
                }
            }

            return coordinate;
        }

        private MapsApiResult Query(string query)
        {
            MapsApiResult queryResultObject=null;
            try
            {
                if (!query.Contains(CountrySearchQueryExtension))
                {
                    query = string.Format("{0}{1}", query, CountrySearchQueryExtension);
                }

                query = Uri.EscapeDataString(query);

                string apiQueryAddress = this.CreateQueryString(query);
                string queryResult = this.webClient.DownloadString(apiQueryAddress);

                queryResultObject = JsonConvert.DeserializeObject<MapsApiResult>(queryResult);
                //Json.Decode<MapsApiResult>(queryResult);
            }
            catch (Exception)
            {
                queryResultObject = null;
            }
            return queryResultObject;
        }

        private string CreateQueryString(string query)
        {
            return string.Format("{0}{1}", MapsApiServiceBaseAddress, query);
        }

        private bool ContainsGermanResultObject(IEnumerable<AddressComponent> components)
        {
            return components.FirstOrDefault(c => c.short_name == "DE") != null;
        }
    }
}