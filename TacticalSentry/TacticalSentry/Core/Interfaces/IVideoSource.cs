using OpenCvSharp;

namespace TacticalSentry.Core.Interfaces
{
    public interface IVideoSource
    {
        void Connect(string url);
        void Disconnect();
        Mat GrabFrame();
        bool IsConnected { get; }
    }
}