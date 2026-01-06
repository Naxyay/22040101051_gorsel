using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using TacticalSentry.Core.Entities;
using TacticalSentry.Core.Enums;
using TacticalSentry.Core.Interfaces;

namespace TacticalSentry.Infrastructure.Services
{
    public class YoloDetectorService : ITargetDetector, IDisposable
    {
        private InferenceSession _session;
        private readonly string[] _labels;

        private const float CONFIDENCE_THRESHOLD = 0.20f;
        private const int MODEL_INPUT_SIZE = 640;

        public YoloDetectorService(string modelPath = "last.onnx")
        {
            _labels = new string[]
            {
                "Grenade",   
                "Knife",     
                "Missile",   
                "Pistol",   
                "Rifle"    
            };
            LoadModel(modelPath);
        }

        public void LoadModel(string modelPath)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, modelPath);
                var options = new SessionOptions();
                try { options.AppendExecutionProvider_CUDA(0); } catch { }
                _session = new InferenceSession(fullPath, options);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        public List<DetectedTarget> AnalyzeFrame(Mat frame)
        {
            var results = new List<DetectedTarget>();
            if (_session == null || frame.Empty()) return results;

            using var resized = new Mat();
            Cv2.Resize(frame, resized, new Size(MODEL_INPUT_SIZE, MODEL_INPUT_SIZE));
            var input = new DenseTensor<float>(new[] { 1, 3, MODEL_INPUT_SIZE, MODEL_INPUT_SIZE });

            for (int y = 0; y < MODEL_INPUT_SIZE; y++)
            {
                for (int x = 0; x < MODEL_INPUT_SIZE; x++)
                {
                    var pixel = resized.At<Vec3b>(y, x);
                    input[0, 0, y, x] = pixel.Item2 / 255f;
                    input[0, 1, y, x] = pixel.Item1 / 255f;
                    input[0, 2, y, x] = pixel.Item0 / 255f;
                }
            }

            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(_session.InputMetadata.Keys.First(), input) };
            using var output = _session.Run(inputs);
            var outputTensor = output.First().AsTensor<float>();

            int dimensions = outputTensor.Dimensions[1];
            int rows = outputTensor.Dimensions[2];

            for (int i = 0; i < rows; i++)
            {
                float maxScore = 0;
                int classId = -1;

                for (int c = 4; c < dimensions; c++)
                {
                    float score = outputTensor[0, c, i];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        classId = c - 4;
                    }
                }

                if (maxScore > CONFIDENCE_THRESHOLD)
                {
                    string labelName = (classId >= 0 && classId < _labels.Length) ? _labels[classId] : $"ID-{classId}";

                    Debug.WriteLine($"[TESPİT] {labelName} (%{maxScore * 100:F0})");

                    results.Add(new DetectedTarget
                    {
                        Label = labelName,
                        ConfidenceScore = maxScore,
                        Classification = ThreatType.HostileInput 
                    });
                }
            }

            if (results.Count > 0)
                return new List<DetectedTarget> { results.OrderByDescending(x => x.ConfidenceScore).First() };

            return results;
        }

        public void Dispose() => _session?.Dispose();
    }
}