using System.Windows;
using TacticalSentry.Infrastructure.Services;

namespace TacticalSentry.Presentation.Views
{
    public partial class LoginWindow : Window
    {
        private readonly DbAuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new DbAuthService();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (_authService.Login(TxtUser.Text, TxtPass.Password))
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Giriş Yetkisi Yok!", "HATA", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}