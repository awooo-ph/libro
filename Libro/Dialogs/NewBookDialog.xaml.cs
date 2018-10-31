using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Libro.Annotations;
using Libro.Models;

namespace Libro.Dialogs
{
    /// <summary>
    /// Interaction logic for NewBookDialog.xaml
    /// </summary>
    public partial class NewBookDialog
    {
        public NewBookDialog()
        {
            InitializeComponent();
            DataContext = new NewBook();
        }
        
    }
}
