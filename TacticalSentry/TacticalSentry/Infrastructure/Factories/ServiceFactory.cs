using TacticalSentry.Core.Interfaces;
using TacticalSentry.Infrastructure.Services;

namespace TacticalSentry.Infrastructure.Factories
{
    public static class ServiceFactory
    {
        public static IVideoSource CreateCamera() => new IpCameraService();
        public static ITargetDetector CreateDetector() => new YoloDetectorService();
    }
}