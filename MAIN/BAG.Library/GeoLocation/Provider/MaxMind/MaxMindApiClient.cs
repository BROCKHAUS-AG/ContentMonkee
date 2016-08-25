using System.Collections.Generic;

namespace BAG.Framework.Geolocation.Provider.MaxMind
{
    using System.Configuration;

    using BAG.Framework.Geolocation.Models;
    using BAG.Framework.Geolocation.Provider.MaxMind.Internal;
    using Newtonsoft.Json.Linq;

    public class MaxMindApiClient : ILocationService
    {
        private const string DatabaseConfigKey = "BAG.Framework.Geolocation.MaxMindDBPath";

        private static Reader reader;

        private readonly List<string> excludedExpressions;

        public bool IsExcludedExpression { get; set; }

        public MaxMindApiClient()
        {
            this.excludedExpressions = new List<string>
                                           {
                                               "::1",
                                               "localhost",
                                               "127.0.0.1",
                                               "10.",
                                               "172.16.",
                                               "192.168."
                                           };
        }

        private bool IsExcludedQueryString(string expression)
        {
            if (expression == null)
            {
                return true;
            }

            foreach (string excludedExpression in this.excludedExpressions)
            {
                if (excludedExpression.Equals(expression) || expression.StartsWith(excludedExpression))
                {
                    return true;
                }
            }

            return false;
        }


        public Coordinate GetCoordinateFromStringExpression(string zip)
        {
            Coordinate defaultCoordinate = new Coordinate
                {
                    Latitude = 51.529,
                    Longitude = 7.4802,
                };


            if (reader == null && ConfigurationManager.AppSettings[DatabaseConfigKey] != null)
            {
                reader = new Reader(ConfigurationManager.AppSettings[DatabaseConfigKey]);
            }

            if (this.IsExcludedQueryString(zip))
            {
                this.IsExcludedExpression = true;
                return defaultCoordinate;
            }

            Coordinate resultCoordinate = defaultCoordinate;
            JToken result = reader.Find(zip);

            if (result != null)
            {
                JObject location = result.Value<JObject>("location");
                resultCoordinate.Latitude = location.Value<double>("latitude");
                resultCoordinate.Longitude = location.Value<double>("longitude");
            }         

            return resultCoordinate;
        }
    }
}
