using System.Media;
using TacticalSentry.Core.Interfaces;

namespace TacticalSentry.Infrastructure.Services
{
    public class WindowsSoundAlerter : IAlertSystem
    {
        public void TriggerVisualAlert() { }

        public void TriggerAudioAlert(string threatType)
        {
            SystemSounds.Exclamation.Play();
        }
    }
}