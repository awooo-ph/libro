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

            Navigation.DataContext = this;
            DataContext = MainViewModel.Instance;
        }

        private bool _isAuthenticating;
        protected override async void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!DialogHost.IsLoaded) return;
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

    }
}
