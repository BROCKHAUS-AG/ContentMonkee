namespace BAG.Framework.Geolocation
{
    using System.Collections.Generic;

    using BAG.Framework.Geolocation.Models;

    public interface ILocationService
    {
        bool IsExcludedExpression { get; set; }

        Coordinate GetCoordinateFromStringExpression(string zip);
    }
}
