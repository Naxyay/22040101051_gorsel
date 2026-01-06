using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TacticalSentry.Core.Entities;
using TacticalSentry.Core.Enums;      
using TacticalSentry.Core.Interfaces;
using TacticalSentry.Domain.Rules;
using TacticalSentry.Infrastructure.Services;
using TacticalSentry.Presentation.Utilities;
using TacticalSentry.Presentation.Views;

namespace TacticalSentry.Presentation.ViewModels
{
    public class CameraSource
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class DashboardViewModel : ObservableObject
    {
        private readonly IVideoSource _camera;
        private readonly ITargetDetector _detector;
        private readonly IEvidenceLocker _evidenceLocker;
        private readonly IAlertSystem _alerter;
        private readonly EngagementRuleEngine _rules;
        private readonly ThreatClassifier _classifier;

        private readonly IMissionLogger _missionLogger;

        private BitmapSource _currentImage;
        private string _status = "SİSTEM HAZIR";
        private bool _isRunning;
        private string _selectedCameraUrl;

        private DateTime _lastCaptureTime = DateTime.MinValue;
        private readonly TimeSpan _captureCooldown = TimeSpan.FromSeconds(3);

        public ObservableCollection<CameraSource> CameraList { get; set; }

        public string SelectedCameraUrl
        {
            get => _selectedCameraUrl;
            set { _selectedCameraUrl = value; OnPropertyChanged(); }
        }

        public BitmapSource CurrentImage
        {
            get => _currentImage;
            set { _currentImage = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public ICommand StartCommand { get; }
        public ICommand OpenFolderCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public DashboardViewModel()
        {
            _camera = new IpCameraService();
            _evidenceLocker = new SecureDiskStorage();
            _alerter = new WindowsSoundAlerter();
            _rules = new EngagementRuleEngine();
            _classifier = new ThreatClassifier();

            _missionLogger = new DbMissionLogger();

            try
            {
                _detector = new YoloDetectorService("last.onnx");
            }
            catch
            {
                StatusMessage = "HATA: AI MODELİ BULUNAMADI!";
            }

            CameraList = new ObservableCollection<CameraSource>
            {
                new CameraSource { Name = "Yerel: Telefon (IP Webcam)", Url = "http://192.168.1.35:8080/video" },
                new CameraSource { Name = "Yerel: Bilgisayar Webcam", Url = "0" },
                new CameraSource { Name = "MOBESE - Aşık Veysel Parkı-1", Url = "https://cdn-f10.galata.ai/5934de99076d4387aaa69c4966840e65/tracks-v1/mono.m3u8?token=R01XTk9uVHlxVDRpSzBVZ0UrbkMxdlhRcWpUamRrWHF1OG12UUpNUU1XST0%3D" }
            };

            if (CameraList.Count > 0) SelectedCameraUrl = CameraList[0].Url;

            StartCommand = new RelayCommand(o => StartMission());
            OpenFolderCommand = new RelayCommand(o => OpenEvidenceFolder());

            OpenSettingsCommand = new RelayCommand(o => {
                var settingsWin = new SettingsWindow();
                settingsWin.ShowDialog();
            });

            StopCommand = new RelayCommand(o => {
                _camera?.Disconnect();
                Environment.Exit(0);
            });
        }

        private async void StartMission()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _camera.Disconnect();
                await Task.Delay(500);
            }

            StatusMessage = "BAĞLANILIYOR...";

            try { _camera.Connect(SelectedCameraUrl); }
            catch { StatusMessage = "BAĞLANTI HATASI!"; return; }

            if (!_camera.IsConnected)
            {
                StatusMessage = "KAMERA AÇILAMADI";
                return;
            }

            _isRunning = true;
            StatusMessage = "TARAMA AKTİF";

            await Task.Run(() =>
            {
                while (_isRunning && _camera.IsConnected)
                {
                    using var frame = _camera.GrabFrame();
                    if (frame.Empty()) continue;

                    var targets = _detector.AnalyzeFrame(frame);
                    bool threatDetected = false;

                    foreach (var t in targets)
                    {
                        t.Classification = _classifier.Classify(t.Label);

                        if (_rules.ShouldEngage(t))
                        {
                            threatDetected = true;

                            if ((DateTime.Now - _lastCaptureTime) > _captureCooldown)
                            {
                                _evidenceLocker.SecureSave(frame, t.Label);

                                _alerter.TriggerAudioAlert(t.Label);

                                _missionLogger.Log(t.Label, LogSeverity.CriticalAlert);

                                _lastCaptureTime = DateTime.Now;

                                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                                    StatusMessage = $"⚠️ TESPİT EDİLDİ: {t.Label.ToUpper()}";
                                });
                            }
                        }
                    }

                    if (!threatDetected && (DateTime.Now - _lastCaptureTime).TotalSeconds > 3)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => StatusMessage = "BÖLGE GÜVENLİ");
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(() => CurrentImage = ImageHelper.ToBitmapSource(frame));
                }
            });
        }

        private void OpenEvidenceFolder()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Evidence");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            try { Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true, Verb = "open" }); } catch { }
        }
    }
}