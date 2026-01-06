using System.Collections.Generic;
using OpenCvSharp;
using TacticalSentry.Core.Entities;

namespace TacticalSentry.Core.Interfaces
{
    public interface ITargetDetector
    {
        List<DetectedTarget> AnalyzeFrame(Mat frame);

        void LoadModel(string modelPath);
    }
}