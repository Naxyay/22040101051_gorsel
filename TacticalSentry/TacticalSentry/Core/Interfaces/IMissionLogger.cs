using TacticalSentry.Core.Enums;

namespace TacticalSentry.Core.Interfaces
{
    public interface IMissionLogger
    {
        void Log(string message, LogSeverity severity);
    }
}