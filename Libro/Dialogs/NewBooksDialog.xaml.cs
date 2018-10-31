using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Libro.Annotations;
using Libro.Models;

namespace Libro.Dialogs
{
    /// <summary>
    /// Interaction logic for NewBooksDialog.xaml
    /// </summary>
    public partial class NewBooksDialog : INotifyPropertyChanged
    {
        public NewBooksDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private ObservableCollection<NewBook> _isbns;

        public ObservableCollection<NewBook> ISBNs
        {
            get
            {
                if (_isbns != null) return _isbns;
                _isbns = new ObservableCollection<NewBook>();
                AddISBN(null);
                AddISBN(null);
                
                return _isbns;
            }
        }

        private ICommand _addISBNCommand;
        public ICommand AddISBNCommand => _addISBNCommand ?? (_addISBNCommand = new DelegateCommand<NewBook>(AddISBN));

        private void AddISBN(NewBook obj)
        {
            if (obj!=null && ISBNs.LastOrDefault() != obj) return;
            var isbn = new NewBook(_isbns);
            isbn.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(IsValid)) return;
                OnPropertyChanged(nameof(IsValid));
            };
            ISBNs.Add(isbn);
        }
        
        public bool IsValid
        {
            get
            {
                if (!ISBNs.Any(x => x.IsValid))
                    return false;
                return !ISBNs.Any(x=>x.IsBookFound && !x.Book.IsValid);
            }
        }
        
      
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsValid"));
        }
    }
}
