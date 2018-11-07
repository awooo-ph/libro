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
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;

namespace Libro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            _navigationItems = new List<INavigationItem>()
            {
                new SubheaderNavigationItem(){Subheader = "BOOKS"},
                new FirstLevelNavigationItem(){Label = "MANAGE BOOKS",Icon = new PackIcon(){Kind = PackIconKind.Books}},
                new FirstLevelNavigationItem(){Label = "ADD NEW BOOK",Icon = new PackIcon(){Kind=PackIconKind.BookPlus}},
                new SubheaderNavigationItem(){Subheader = "STUDENTS"},
                new FirstLevelNavigationItem(){Label = "MANAGE STUDENTS",Icon = new PackIcon(){Kind=PackIconKind.AccountGroup}},
                new FirstLevelNavigationItem(){Label = "ADD NEW STUDENT",Icon = new PackIcon(){Kind=PackIconKind.AccountPlus}},
            };

            InitializeComponent();

            LoginDialogHost.Loaded += async (sender, args) =>
            {
                await Task.Delay(111);
                CheckAuthentication();
            };

            Navigation.DataContext = this;

        }

        private bool _isAuthenticating;
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            CheckAuthentication();
        }

        private async void CheckAuthentication()
        {
            if (!LoginDialogHost.IsLoaded) return;
            if (_isAuthenticating) return;
            
            if (!MainViewModel.Instance.IsAuthenticated)
            {
                _isAuthenticating = true;
                var login = await LoginDialog.Show();
                _isAuthenticating = false;
                MainViewModel.Instance.Login(login);
            }
        }

        private List<INavigationItem> _navigationItems;

        public List<INavigationItem> NagivationItems => _navigationItems;

        private void BorrowersTab_OnChecked(object sender, RoutedEventArgs e)
        {
            
        }

        private void BooksTab_OnClick(object sender, RoutedEventArgs e)
        {
            Libro.Properties.Resources.click_02.Play();
        }
    }
}
