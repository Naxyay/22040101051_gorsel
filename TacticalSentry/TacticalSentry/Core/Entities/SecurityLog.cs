using System;

namespace TacticalSentry.Core.Entities
{
    public class SecurityLog
    {
        public int Id { get; set; }
        public string ThreatType { get; set; } 
        public string DetectedTime { get; set; }
        public string Status { get; set; } 
        public int Confidence { get; set; } 
    }
}