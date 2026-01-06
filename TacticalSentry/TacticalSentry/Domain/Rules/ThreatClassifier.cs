using TacticalSentry.Core.Enums;

namespace TacticalSentry.Domain.Rules
{
    public class ThreatClassifier
    {
        public ThreatType Classify(string label)
        {
            if (string.IsNullOrEmpty(label)) return ThreatType.Unknown;

            label = label.ToLower();

            if (label.Contains("knife") ||
                label.Contains("gun") ||
                label.Contains("rifle") ||
                label.Contains("pistol") ||   
                label.Contains("grenade") ||  
                label.Contains("missile"))    
            {
                return ThreatType.HostileInput;
            }

            if (label.Contains("tank") || label.Contains("vehicle"))
                return ThreatType.Vehicle;

            return ThreatType.Unknown;
        }
    }
}