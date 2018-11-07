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
using MaterialDesignThemes.Wpf;

namespace Libro.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog 
    {
        public MessageDialog()
        {
            InitializeComponent();
        }

        public static async Task<bool> Show(string title, string message="")
        {
            var dlg = new MessageDialog();
            dlg.Message.Text = message;
            dlg.Title.Text = title;
            var response = await DialogHost.Show(dlg, "RootDialog");
            return (response as bool?) ?? false;
        }

        public static async Task<bool> Show(string title, string message, string positive, string negative=null)
        {
            var dlg = new MessageDialog();
            dlg.Message.Text = message;
            dlg.Title.Text = title;

            dlg.Positive.Content = positive;
            dlg.Negative.Content = negative;
            if (string.IsNullOrEmpty(negative))
            {
                dlg.Negative.IsCancel = false;
                dlg.Negative.Visibility = Visibility.Collapsed;
                dlg.Positive.IsCancel = true;
            }

            var response = await DialogHost.Show(dlg, "RootDialog");
            return (response as bool?) ?? false;
        }

        public static async Task<bool?> Show(string title, string message, string positive, string negative, bool showCancel)
        {
            var dlg = new MessageDialog();
            dlg.Message.Text = message;
            dlg.Title.Text = title;

            dlg.Positive.Content = positive;
            dlg.Negative.Content = negative;

            if (showCancel)
            {
                dlg.Negative.IsCancel = false;
                dlg.Cancel.IsCancel = true;
                dlg.Cancel.Visibility = Visibility.Visible;
            }

            var response = await DialogHost.Show(dlg, "RootDialog");
            return response as bool?;
        }
    }
}
