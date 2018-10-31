using System;
using System.Collections.Generic;
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
    /// Interaction logic for BookSelector.xaml
    /// </summary>
    public partial class BookSelector : INotifyPropertyChanged
    {
        public BookSelector()
        {
            InitializeComponent();
        }

        private string _searchKeyword="";

        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set
            {
                if(value == _searchKeyword)
                    return;
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
                if(_searchResult != null)
                    return _searchResult;
                _searchResult = new ListCollectionView(Book.Cache);
                _searchResult.Filter = Filter;
                return _searchResult;
            }
        }

        private bool Filter(object o)
        {
            var b = (Book)o;
            if(b.Barcode.ToLower().Contains(SearchKeyword))
                return true;
            if(b.Isbn.ToLower().Contains(SearchKeyword))
                return true;
            if(b.Isbn13.ToLower().Contains(SearchKeyword))
                return true;
            if(b.Issn.ToLower().Contains(SearchKeyword))
                return true;
            if(b.Title.ToLower().Contains(SearchKeyword))
                return true;
            if(b.Author.ToLower().Contains(SearchKeyword))
                return true;
            //if(b.Issn.ToLower().Contains(SearchKeyword))
              //  return true;
            //if(b.Issn.ToLower().Contains(SearchKeyword))
              //  return true;
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
