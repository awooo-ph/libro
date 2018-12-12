using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Libro.Dialogs;
using Libro.Properties;
using Libro.ViewModels;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using Settings = Libro.ViewModels.Settings;

namespace Libro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LoginDialogHost.Loaded += async (sender, args) =>
            {
                await Task.Delay(111);
                CheckAuthentication();
            };

        }

        private bool _isAuthenticating;
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            CheckAuthentication();
        }

        private async void CheckAuthentication()
        {
            if (!Settings.Instance.EnableSecurity) return;
            if (!LoginDialogHost.IsLoaded) return;
            if (_isAuthenticating) return;
            
            if (!MainViewModel.Instance.IsAuthenticated)
            {
                _isAuthenticating = true;
                var login = await LoginDialog.Show();
                _isAuthenticating = false;
                MainViewModel.Instance.Login(login);
                CheckAuthentication();
            }
        }

        private void DashboardTab_OnChecked(object sender, RoutedEventArgs e)
        {
            Dashboard.Instance.Refresh();
        }
    }
}
