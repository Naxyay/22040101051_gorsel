using OpenCvSharp;

namespace TacticalSentry.Core.Interfaces
{
    public interface IEvidenceLocker
    {
        string SecureSave(Mat frame, string threatLabel);
        void ArchiveOldEvidence(int daysToKeep);
    }
}