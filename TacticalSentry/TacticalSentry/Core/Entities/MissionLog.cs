using TacticalSentry.Core.Enums;

namespace TacticalSentry.Core.Entities
{
    public class MissionLog : BaseEntity
    {
        public string EventCode { get; set; }
        public string Message { get; set; }
        public LogSeverity Severity { get; set; }
    }
}