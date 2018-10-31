using System.Net;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace Libro.Dialogs
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog
    {
        private LoginDialog()
        {
            InitializeComponent();
        }

        public static async Task<NetworkCredential> Show()
        {
            var dlg = new LoginDialog();
            var res = await DialogHost.Show(dlg, "RootDialog");
            if (res == null) return null;
            return new NetworkCredential(dlg.Username.Text,dlg.Password.SecurePassword);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown(0);
        }
    }
}
