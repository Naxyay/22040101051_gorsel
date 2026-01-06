using TacticalSentry.Core.Entities;
using TacticalSentry.Core.Enums;

namespace TacticalSentry.Domain.Rules
{
    public class EngagementRuleEngine
    {
        public bool ShouldEngage(DetectedTarget target)
        {
            if (target.ConfidenceScore < 0.60f) return false;

            if (target.Classification == ThreatType.Unknown && target.ConfidenceScore > 0.80f)
                return true;

            if (target.Classification == ThreatType.HostileInput || target.Classification == ThreatType.Vehicle) 
                return true;

            return false;
        }
    }
}