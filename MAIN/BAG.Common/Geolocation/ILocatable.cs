namespace BAG.Common.Geolocation
{
    public interface ILocatable
    {
        double Latitude { get; set; }

        double Longitude { get; set; }

        string Zip { get; }
    }
}
