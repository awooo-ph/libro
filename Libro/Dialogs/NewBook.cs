using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Libro.Annotations;
using Libro.Google;
using Libro.Models;

namespace Libro.Dialogs
{
    public class NewBook : INotifyPropertyChanged, IDataErrorInfo
    {
        public NewBook() : this(null)
        {
        }

        public NewBook(ObservableCollection<NewBook> parentList)
        {
            _parentList = parentList;
            if(_parentList==null)
                AccessionNumber = Book.GetNextNumber().ToString();
            else
            {
                if (parentList.Any())
                {
                    var an = ToLong(parentList.OrderByDescending(x => ToLong(x.AccessionNumber)).FirstOrDefault()?.AccessionNumber);
                    AccessionNumber = (++an).ToString();
                }
                else
                {
                    AccessionNumber = Book.GetNextNumber().ToString();
                }
            }
        }

        private long ToLong(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            long.TryParse(s, out var l);
            return l;
        }

        private ObservableCollection<NewBook> _parentList;

        private string _isbn;

        public string Isbn
        {
            get { return _isbn; }
            set
            {
                if(value == _isbn)
                    return;
                _isbn = value;
                OnPropertyChanged();
                SearchOnline(null);
            }
        }

        private ICommand _searchOnlineCommand;

        public ICommand SearchOnlineCommand
            => _searchOnlineCommand ?? (_searchOnlineCommand = new DelegateCommand(SearchOnline, CanSearch));

        private bool CanSearch(object obj)
        {
            if(obj != null)
                return true;
            return !string.IsNullOrEmpty(Isbn);
        }

        public bool UseISSN { get; set; }

        private string _accessionNumber;

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set
            {
                if (value == _accessionNumber) return;
                _accessionNumber = value;
                OnPropertyChanged();
                
            }
        }
        
        private string _callNumber;

        public string CallNumber
        {
            get { return _callNumber; }
            set
            {
                if (value == _callNumber) return;
                _callNumber = value;
                OnPropertyChanged();
            }
        }
        
        private CancellationTokenSource _tokenSource;
        private Task _searchTask;

        private GoogleBooks _google = new GoogleBooks();

        private void SearchOnline(object obj)
        {
            if(obj != null)
                Isbn = obj as string;

            IsSearching = false;
            IsBookFound = false;
            IsSearchingComplete = false;

            var searchStart = DateTime.Now;
            
            _tokenSource?.Cancel();
            
            if(string.IsNullOrEmpty(Isbn))
                return;

            _tokenSource = new CancellationTokenSource();
            Task.Delay(400, _tokenSource.Token).ContinueWith(res =>
            {
                if (res.IsCanceled) return;
                if (!res.IsCompleted) return;
                IsSearching = true;
                IsBookFound = false;
                IsSearchingComplete = false;
                var token = _tokenSource.Token;
                Task.Factory.StartNew(() =>
                {
                    var book = _google.GetByIsbn(Isbn, true, UseISSN);
                    if(token.IsCancellationRequested)
                    {
                        IsSearchingComplete = false;
                        IsSearching = false;
                        IsBookFound = false;
                        return;
                    }

                    IsSearchingComplete = true;
                    IsSearching = false;
                    if(book != null)
                    {
                        Book = book;
                        //Book.AccessionNumber = AccessionNumber;
                        //Book.CallNumber = CallNumber;
                        //Book.Coauthors = Condition;
                        //Book.DateReceived = DateReceived;
                      //  Book.Fund = Fund;
                    //    Book.Price = Price;
                        SearchResult = "";
                        IsBookFound = true;
                    } else
                    {
                        SearchResult = "UNABLE TO GET BOOK DETAILS";
                        IsBookFound = false;
                        Book = null;
                    }
                }, _tokenSource.Token);
                
            });
        }

        private string _searchResult;

        public string SearchResult
        {
            get { return _searchResult; }
            private set
            {
                if(value == _searchResult)
                    return;
                _searchResult = value;
                OnPropertyChanged();
            }
        }

        private bool _isBookFound = false;

        public bool IsBookFound
        {
            get { return _isBookFound; }
            private set
            {
                if(value == _isBookFound)
                    return;
                _isBookFound = value;
                OnPropertyChanged();
            }
        }

        private bool _isSearching;

        public bool IsSearching
        {
            get { return _isSearching; }
            private set
            {
                if(value == _isSearching)
                    return;
                _isSearching = value;
                OnPropertyChanged();
            }
        }

        private Book _temporaryBook = new Book();

        public Book TemporaryBook
        {
            get { return _temporaryBook; }
            set
            {
                if (Equals(value, _temporaryBook)) return;
                _temporaryBook = value;
                OnPropertyChanged();
            }
        }

        private Book _book;

        public Book Book
        {
            get { return _book; }
            set
            {
                if(Equals(value, _book))
                    return;
                _book = value;
                OnPropertyChanged();
                if(value!=null) TemporaryBook = value;
            }
        }

        private bool _isSearchingComplete;

        public bool IsSearchingComplete
        {
            get { return _isSearchingComplete; }
            private set
            {
                if(value == _isSearchingComplete)
                    return;
                _isSearchingComplete = value;
                OnPropertyChanged();
            }
        }

        public bool IsValid
        {
            get
            {
                if(string.IsNullOrEmpty(Isbn))
                    return false;
                //if(!IsSearchingComplete)
                  //  return false;
                //if(!IsBookFound)
                  //  return false;
                if(string.IsNullOrEmpty(AccessionNumber))
                    return false;
                return Book.Cache.All(x => x.AccessionNumber != AccessionNumber);
            }
        }

        private string _condition = "NEW";

        public string Condition
        {
            get { return _condition; }
            set
            {
                if (value == _condition) return;
                _condition = value;
                OnPropertyChanged();
            }
        }

        private DateTime _dateReceived = DateTime.Now;

        public DateTime DateReceived
        {
            get { return _dateReceived; }
            set
            {
                if (value.Equals(_dateReceived)) return;
                _dateReceived = value;
                OnPropertyChanged();
            }
        }

        private double _price;

        public double Price
        {
            get { return _price; }
            set
            {
                if (value.Equals(_price)) return;
                _price = value;
                OnPropertyChanged();
            }
        }

        private string _fund;

        public string Fund
        {
            get { return _fund; }
            set
            {
                if (value == _fund) return;
                _fund = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsValid"));

            if (Book == null) return;
            Book.AccessionNumber = AccessionNumber;
            Book.Fund = Fund;
            Book.Price = Price;
            Book.DateReceived = DateReceived;
            Book.Condition = Condition;
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(AccessionNumber):
                    {
                        if (string.IsNullOrWhiteSpace(AccessionNumber) && !string.IsNullOrWhiteSpace(Isbn))
                            return "Accession Number is Required.";
                        if (!string.IsNullOrWhiteSpace(AccessionNumber) &&
                            Book.Cache.Any(x => x.AccessionNumber == AccessionNumber))
                            return "Accession Number already exists.";
                        if (_parentList != null &&
                            _parentList.Any(x => !string.IsNullOrWhiteSpace(AccessionNumber) && x.AccessionNumber == AccessionNumber && x != this))
                            return "Another book has the same Accession Number.";
                        break;
                    }

                    case nameof(Isbn):
                    {
                        if (_parentList!=null && string.IsNullOrWhiteSpace(Isbn)) return "ISBN is required.";
                        break;
                    }
                }
                return null;
            }
        }

        string IDataErrorInfo.Error
        {
            get { return null; }
        }
        

        public void CancelRequest()
        {
            _google.CancelRequest();
            _tokenSource?.Cancel();
        }
    }
}
