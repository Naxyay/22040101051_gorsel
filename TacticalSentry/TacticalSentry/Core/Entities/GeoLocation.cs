namespace TacticalSentry.Core.Entities
{
    public class GeoLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public override string ToString() => $"LAT:{Latitude:F4} LON:{Longitude:F4}";
    }
}