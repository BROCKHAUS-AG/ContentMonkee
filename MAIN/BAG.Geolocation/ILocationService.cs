namespace BAG.Geolocation
{
    using System.Collections.Generic;

    using BAG.Geolocation.Models;

    public interface ILocationService
    {
        bool IsExcludedExpression { get; set; }

        Coordinate GetCoordinateFromStringExpression(string zip);
    }
}
