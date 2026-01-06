using System.Windows;
using TacticalSentry.Presentation.Views;

namespace TacticalSentry
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
        }
    }
}