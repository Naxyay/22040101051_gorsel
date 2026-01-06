using OpenCvSharp;
using TacticalSentry.Core.Interfaces;

namespace TacticalSentry.Infrastructure.Services
{
    public class IpCameraService : IVideoSource
    {
        private VideoCapture _capture;

        public bool IsConnected => _capture != null && _capture.IsOpened();

        public void Connect(string url)
        {
            if (url == "0") _capture = new VideoCapture(0);
            else _capture = new VideoCapture(url);

            if (IsConnected)
            {
                _capture.Set(VideoCaptureProperties.BufferSize, 1);
            }
        }

        public void Disconnect()
        {
            _capture?.Release();
            _capture?.Dispose();
            _capture = null;
        }

        public Mat GrabFrame()
        {
            var frame = new Mat();
            if (IsConnected) _capture.Read(frame);
            return frame;
        }
    }
}