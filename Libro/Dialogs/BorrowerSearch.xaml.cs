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
    /// Interaction logic for BorrowerSearch.xaml
    /// </summary>
    public partial class BorrowerSearch : INotifyPropertyChanged
    {
        public BorrowerSearch()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string _searchKeyword="";

        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set
            {
                if (value == _searchKeyword) return;
                _searchKeyword = value;
                OnPropertyChanged();
                SearchResult.Filter = Filter;
            }
        }

        private ListCollectionView _searchResult;

        public ListCollectionView SearchResult
        {
            get
            {
                if (_searchResult != null) return _searchResult;
                _searchResult = new ListCollectionView(Borrower.Cache);
                _searchResult.Filter = Filter;
                return _searchResult;
            }
        }

        private bool Filter(object o)
        {
            var b = (Borrower) o;
            if(b.Firstname.ToLower().Contains(SearchKeyword)) return true;
            if(b.Lastname.ToLower().Contains(SearchKeyword))
                return true;
            if(b.Barcode.ToLower().Contains(SearchKeyword))
                return true;
            if(b.SchoolId.ToLower().Contains(SearchKeyword))
                return true;
            return false;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
