using TacticalSentry.Core.Enums;

namespace TacticalSentry.Core.Entities
{
    public class DetectedTarget : BaseEntity
    {
        public string Label { get; set; }
        public float ConfidenceScore { get; set; }
        public ThreatType Classification { get; set; }
        public GeoLocation Location { get; set; }
        public string EvidenceImagePath { get; set; } 
    }
}