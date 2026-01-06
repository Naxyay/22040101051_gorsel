using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace TacticalSentry.Presentation.Utilities
{
    public static class ImageHelper
    {
        public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(Mat frame)
        {
            return frame.ToBitmapSource();
        }
    }
}