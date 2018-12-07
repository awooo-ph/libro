using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Libro.Annotations;
using Libro.Models;

namespace Libro.ViewModels
{
    class TakeoutViewModel : INotifyPropertyChanged
    {
        private readonly SynchronizationContext context;
        public TakeoutViewModel(bool autoSearch)
        {
            AutoSearch = autoSearch;
            context = SynchronizationContext.Current;
            Messenger.Default.AddListener(Messages.TakeoutsChanged,RefreshHistory);
        }

        private string _barcode;

        public string Barcode
        {
            get { return _barcode; }
            set
            {
                if (value == _barcode) return;
                _barcode = value;
                OnPropertyChanged();
                if(AutoSearch)
                    SearchBarcode(value);
            }
        }

        private ICommand _escapeCommand;
        public ICommand EscapeCommand => _escapeCommand ?? (_escapeCommand = new DelegateCommand(CloseScreen));

        private void CloseScreen(object obj = null)
        {
            if (BookCode.Length > 0)
            {
                BookCode = "";
            }
            else if (Barcode.Length > 0)
            {
                BookCart.Clear();
                BookCode = "";
                Barcode = "";
                Borrower = null;
            }
            else
                Messenger.Default.Broadcast(Messages.HOME_CloseScreen);
        }

        public bool AutoSearch { get; set; }

        private ICommand _searchBorrowerCommand;

        public ICommand SearchBorrowerCommand
            => _searchBorrowerCommand ?? (_searchBorrowerCommand = new DelegateCommand(SearchBorrower));

        private void SearchBorrower(object obj)
        {
            SearchBarcode(Barcode);
        }

        private string _bookCode;

        public string BookCode
        {
            get { return _bookCode; }
            set
            {
                if (value == _bookCode) return;
                _bookCode = value;
                OnPropertyChanged();
                SearchBook(value);
            }
        }

        private ICommand _addBookCommand;
        public ICommand AddBookCommand => _addBookCommand ?? (_addBookCommand = new DelegateCommand(AddBook,CanAddBook));

        private bool CanAddBook(object obj)
        {
            return !SelectedBook?.IsBorrowed??true;
        }

        private void AddBook(object obj)
        {
            if(SelectedBook.IsBorrowed) return;
            BookCart.Add(SelectedBook);
            BookCode = "";
        }

        private readonly ObservableCollection<Book> _bookCart = new ObservableCollection<Book>();
        public ObservableCollection<Book> BookCart
        {
            get { return _bookCart; }
        }

        private void SearchBook(string code)
        {
            SelectedBook = null;
            SelectedBook = Book.Cache.FirstOrDefault(x => (x.Barcode==code || x.AccessionNumber == code) && !BookCart.Contains(x));
        }

        public bool BookFound => SelectedBook != null;

        private Book _selectedBook;

        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                if (Equals(value, _selectedBook)) return;
                _selectedBook = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BookFound));
            }
        }


        private void SearchBarcode(string code)
        {
            Borrower = Borrower.Cache.FirstOrDefault(x => x.Barcode == code || x.SchoolId == code);
        }

        private Borrower _borrower;

        public Borrower Borrower
        {
            get { return _borrower; }
            private set
            {
                if (Equals(value, _borrower)) return;
                _borrower = value;
                OnPropertyChanged();
                _bookTakeouts = null;
                _takeouts = null;
                OnPropertyChanged(nameof(Takeouts));
               RefreshHistory();
            }
        }

        private void RefreshHistory()
        {
            History.Clear();

            Task.Factory.StartNew(() =>
            {
                var to = Takeout.GetByBorrower(Borrower?.Id);
                to.ForEach(x => context.Post(d => History.Add((Takeout)d), x));
            });
        }

        private string _note;

        public string Note
        {
            get { return _note; }
            set
            {
                if (value == _note) return;
                _note = value;
                OnPropertyChanged();
            }
        }

        private string _condition;

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


        private ListCollectionView _takeouts;

        public ListCollectionView Takeouts
        {
            get
            {
                if(_takeouts != null)
                    return _takeouts;
                _takeouts = (ListCollectionView)CollectionViewSource.GetDefaultView(BookTakeouts);
                return _takeouts;
            }
        }

        private readonly ObservableCollection<Takeout> _history = new ObservableCollection<Takeout>();

        public ObservableCollection<Takeout> History
        {
            get { return _history; }
        }


        private ObservableCollection<Takeout> _bookTakeouts;

        private ObservableCollection<Takeout> BookTakeouts
        {
            get
            {
                if(_bookTakeouts != null)
                    return _bookTakeouts;
                _bookTakeouts = new ObservableCollection<Takeout>(Takeout.GetUnreturnedByBorrower(Borrower?.Id));
                return _bookTakeouts;
            }
        }

        private ICommand _changeBorrowerCommand;

        public ICommand ChangeBorrowerCommand
            => _changeBorrowerCommand ?? (_changeBorrowerCommand = new DelegateCommand(ChangeBorrower));

        private ICommand _clearBooksCommand;

        public ICommand ClearBooksCommand
            => _clearBooksCommand ?? (_clearBooksCommand = new DelegateCommand(ClearBooks,CanClearBooks));

        private bool CanClearBooks(object obj)
        {
            return BookCart.Count > 0;
        }

        private void ClearBooks(object obj)
        {
            BookCart.Clear();
        }

        private ICommand _takeoutCommand;
        public ICommand TakeoutCommand => _takeoutCommand ?? (_takeoutCommand = new DelegateCommand(TakeoutBooks, CanTakeout));

        private bool CanTakeout(object obj)
        {
            return BookCart.Count > 0;
        }

        private void TakeoutBooks(object obj)
        {
            foreach (var book in BookCart)
            {
                var to = new Takeout()
                {
                    UserId = 0,//Session.Current.User.Id,
                    BookId = book.Id,
                    BorrowerId = Borrower.Id,
                    TakeOutCondition = book.Condition,
                };
                to.Save();
                book.Update(nameof(Book.TakeoutId),to.Id);
            }
            BookCart.Clear();
            BookCode = "";
            Barcode = "";
            Borrower = null;
            Messenger.Default.Broadcast(Messages.TakeoutsChanged);
        }

        private void ChangeBorrower(object obj)
        {
            Barcode = "";
            Borrower = null;
        }

        private ICommand _removeBookCommand;

        public ICommand RemoveBookCommand
            => _removeBookCommand ?? (_removeBookCommand = new DelegateCommand<Book>(RemoveBook));

        private void RemoveBook(Book obj)
        {
            BookCart.Remove(obj);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddTakeout(Takeout to)
        {
            BookTakeouts.Add(to);
        }

        public void RemoveTakeout(Takeout to)
        {
            BookTakeouts.Remove(to);
        }
    }
}
