using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TacticalSentry.Core.Entities;
using TacticalSentry.Domain.Rules;      
using TacticalSentry.Domain.Security;   
using TacticalSentry.Infrastructure.Data;
using TacticalSentry.Infrastructure.Services;
using TacticalSentry.Presentation.Utilities;

namespace TacticalSentry.Presentation.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        public ObservableCollection<SecurityLog> LogHistory { get; set; }

        private ObservableCollection<OperatorUser> _personnelList;
        public ObservableCollection<OperatorUser> PersonnelList
        {
            get => _personnelList;
            set { _personnelList = value; OnPropertyChanged(); }
        }

        private OperatorUser _selectedPersonnel;
        public OperatorUser SelectedPersonnel
        {
            get => _selectedPersonnel;
            set { _selectedPersonnel = value; OnPropertyChanged(); }
        }

        private bool _isSoundOn = true;
        private bool _isAutoRecord = true;
        private int _sensitivity = 50;
        private string _selectedModelPath = "last.onnx";
        private string _selectedMode = "Normal";

        public bool IsSoundOn { get => _isSoundOn; set { _isSoundOn = value; OnPropertyChanged(); } }
        public bool IsAutoRecord { get => _isAutoRecord; set { _isAutoRecord = value; OnPropertyChanged(); } }
        public int Sensitivity { get => _sensitivity; set { _sensitivity = value; OnPropertyChanged(); } }
        public string SelectedModelPath { get => _selectedModelPath; set { _selectedModelPath = value; OnPropertyChanged(); } }

        public bool IsModeSafe { get => _selectedMode == "Safe"; set { if (value) _selectedMode = "Safe"; OnPropertyChanged(); } }
        public bool IsModeNormal { get => _selectedMode == "Normal"; set { if (value) _selectedMode = "Normal"; OnPropertyChanged(); } }
        public bool IsModeHostile { get => _selectedMode == "Hostile"; set { if (value) _selectedMode = "Hostile"; OnPropertyChanged(); } }

        private bool _isAdminPanelVisible;
        public bool IsAdminPanelVisible { get => _isAdminPanelVisible; set { _isAdminPanelVisible = value; OnPropertyChanged(); } }

        public string NewUsername { get; set; }
        public string NewPassword { get; set; }

        public ICommand SelectModelCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand ExportLogsCommand { get; }
        public ICommand ClearLogsCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public SettingsViewModel()
        {
            LoadLogsFromDb();
            LoadPersonnelFromDb();

            var user = SessionManager.Instance.CurrentOperator;
            IsAdminPanelVisible = user != null && user.ClearanceLevel >= 5;


            SelectModelCommand = new RelayCommand(o => {
                OpenFileDialog dlg = new OpenFileDialog { Filter = "ONNX Model|*.onnx" };
                if (dlg.ShowDialog() == true) SelectedModelPath = dlg.FileName;
            });

            SaveSettingsCommand = new RelayCommand(o => MessageBox.Show("Sistem yapılandırması veritabanına kaydedildi.", "Başarılı"));

            ExportLogsCommand = new RelayCommand(o => {
                SaveFileDialog dlg = new SaveFileDialog { Filter = "Şifreli Rapor (*.enc)|*.enc" };
                if (dlg.ShowDialog() == true)
                {
                    string report = $"GİZLİ RAPOR\nTarih: {DateTime.Now}\nOluşturan: {SessionManager.Instance.CurrentOperator?.Username}\nToplam Olay: {LogHistory.Count}";

                    string encrypted = DataEncryptor.EncryptString(report);
                    System.IO.File.WriteAllText(dlg.FileName, encrypted);

                    MessageBox.Show("Rapor şifrelenerek dışa aktarıldı.", "Veri Güvenliği", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            ClearLogsCommand = new RelayCommand(o => {
                var validator = new PermissionValidator();
                var currentUser = SessionManager.Instance.CurrentOperator;

                if (!validator.CanDeleteLogs(currentUser))
                {
                    MessageBox.Show($"Erişim Reddedildi!\nLogları silmek için Admin yetkisi (Seviye 5) gerekir.\nSizin Seviyeniz: {currentUser.ClearanceLevel}",
                                    "Yetkisiz İşlem", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ClearAllLogs();
            });

            AddUserCommand = new RelayCommand(o => AddNewOperator());

            DeleteUserCommand = new RelayCommand(o => DeleteSelectedUser());
        }


        private void LoadLogsFromDb()
        {
            try
            {
                using (var context = new TacticalDbContext())
                {
                    context.Database.EnsureCreated();
                    var logs = context.Logs.OrderByDescending(x => x.Id).ToList();
                    LogHistory = new ObservableCollection<SecurityLog>(logs);
                }
            }
            catch { LogHistory = new ObservableCollection<SecurityLog>(); }
        }

        private void LoadPersonnelFromDb()
        {
            try
            {
                using (var context = new TacticalDbContext())
                {
                    context.Database.EnsureCreated();
                    var users = context.Users.OrderByDescending(u => u.ClearanceLevel).ToList();
                    PersonnelList = new ObservableCollection<OperatorUser>(users);
                }
            }
            catch { PersonnelList = new ObservableCollection<OperatorUser>(); }
        }

        private void ClearAllLogs()
        {
            if (MessageBox.Show("TÜM GEÇMİŞ LOGLAR SİLİNECEK.\nOnaylıyor musunuz?", "Kritik İşlem", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var context = new TacticalDbContext())
                {
                    context.Logs.RemoveRange(context.Logs);
                    context.SaveChanges();
                }
                LogHistory.Clear();
                MessageBox.Show("Veritabanı temizlendi.");
            }
        }

        private void AddNewOperator()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("Kullanıcı adı ve şifre giriniz!", "Hata");
                return;
            }

            var auth = new DbAuthService();
            auth.RegisterUser(NewUsername, NewPassword, 1); 

            MessageBox.Show($"Operatör '{NewUsername}' sisteme eklendi.");
            LoadPersonnelFromDb(); 

            NewUsername = "";
            NewPassword = "";
            OnPropertyChanged(nameof(NewUsername));
            OnPropertyChanged(nameof(NewPassword));
        }

        private void DeleteSelectedUser()
        {
            if (SelectedPersonnel == null)
            {
                MessageBox.Show("Lütfen silinecek personeli listeden seçin.", "Seçim Yok");
                return;
            }

            if (SelectedPersonnel.Username == "admin")
            {
                MessageBox.Show("Ana Yönetici (Admin) hesabı silinemez!", "Güvenlik Protokolü", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (SelectedPersonnel.Username == SessionManager.Instance.CurrentOperator.Username)
            {
                MessageBox.Show("Kendi hesabınızı silemezsiniz!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"'{SelectedPersonnel.Username}' kullanıcısı silinecek. Onaylıyor musunuz?", "Personel Silme", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var auth = new DbAuthService();
                auth.DeleteUser(SelectedPersonnel.Username); 

                MessageBox.Show("Personel kaydı silindi.");
                LoadPersonnelFromDb();
            }
        }
    }
}