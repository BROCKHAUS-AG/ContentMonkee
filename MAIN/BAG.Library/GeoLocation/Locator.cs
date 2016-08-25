namespace BAG.Framework.Geolocation
{
    using System.Collections.Generic;

    using BAG.Framework.Geolocation.Models;

    public class Locator<T>
        where T : class, ILocationService, new()
    {
        private readonly ILocationService locationService;

        public Locator()
        {
            this.locationService = new T();
        }

        public bool IsExcludedExpression
        {
            get
            {
                return this.locationService.IsExcludedExpression;
            }
        }

        public Coordinate GetCoordinateFromStringExpression(string zip)
        {
            return this.locationService.GetCoordinateFromStringExpression(zip);
        }

        public string GetServiceName()
        {
            return typeof(T).Name;
        }
    }
}