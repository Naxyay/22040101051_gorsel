using System;
using System.IO;
using OpenCvSharp;
using TacticalSentry.Core.Interfaces;

namespace TacticalSentry.Infrastructure.Services
{
    public class SecureDiskStorage : IEvidenceLocker
    {
        private readonly string _basePath;

        public SecureDiskStorage()
        {
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Evidence");
            if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
        }

        public string SecureSave(Mat frame, string threatLabel)
        {
            try
            {
                string filename = $"{DateTime.Now:yyyyMMdd_HHmmss}_{threatLabel.Replace(" ", "_")}.jpg";
                string fullPath = Path.Combine(_basePath, filename);
                frame.SaveImage(fullPath);
                return fullPath;
            }
            catch { return null; }
        }

        public void ArchiveOldEvidence(int daysToKeep) { }
    }
}