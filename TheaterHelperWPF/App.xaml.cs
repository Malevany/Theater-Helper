using System.Configuration;
using System.Data;
using System.Windows;
using TheaterHelperWPF.ViewModel;

namespace TheaterHelperWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow win = new MainWindow();
            win.DataContext = new MainWindowViewModel();
            win.Show();
        }
    }
}
