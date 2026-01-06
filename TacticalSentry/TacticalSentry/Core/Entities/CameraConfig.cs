using TacticalSentry.Core.Entities; // BaseEntity için

namespace TacticalSentry.Core.Entities
{
    public class CameraConfig : BaseEntity
    {
        public string Name { get; set; }

        public string StreamUrl { get; set; }

        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool IsInfraredEnabled { get; set; }

        public string GetFullUrl() => StreamUrl;
    }
}