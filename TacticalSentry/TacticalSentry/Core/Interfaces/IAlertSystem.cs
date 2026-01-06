namespace TacticalSentry.Core.Interfaces
{
    public interface IAlertSystem
    {
        void TriggerVisualAlert();
        void TriggerAudioAlert(string threatType);
    }
}