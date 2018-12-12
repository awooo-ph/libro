using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Libro.Annotations;
using Libro.Dialogs;
using Libro.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace Libro.ViewModels
{
    class Books 
    {
        private Books()
        {
            
        }

        private ListCollectionView _booksView;

        public ListCollectionView BooksView
        {
            get
            {
                if (_booksView != null) return _booksView;
                _booksView = (ListCollectionView)CollectionViewSource.GetDefaultView(Book.Cache);
                return _booksView;
            }
        }

        private string _searchKeyword;

        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set
            {
                if (value == _searchKeyword) return;
                _searchKeyword = value;
                OnPropertyChanged();
                BooksView.Filter = o =>
                {
                    if (string.IsNullOrWhiteSpace(value)) return true;
                    if (!(o is Book bk)) return false;
                    if (bk.Title.ToLower().Contains(value.ToLower())) return true;
                    if (bk.Author.ToLower().Contains(value.ToLower())) return true;
                    if (bk.EqualsId(value)) return true;
                    if (bk.AccessionNumber == value) return true;
                    if (bk.Coauthors.ToLower().Contains(value.ToLower())) return true;
                    return false;
                };
            }
        }

        private static Books _instance;
        public static Books Instance => _instance ?? (_instance = new Books());

        private FrameworkElement _view;

        public FrameworkElement View
        {
            get
            {
                if (_view != null) return _view;
                _view = new Views.BooksView() { DataContext = this };
                return _view;
            }
        }

        private ICommand _addBookCommand;
        public ICommand AddBookCommand => _addBookCommand ?? (_addBookCommand = new DelegateCommand(async obj =>
        {
            var dlg = new NewBookDialog();
            var res = await DialogHost.Show(dlg, "RootDialog") as bool?;
            if (!(res ?? false)) return;
            var nb = ((NewBook)dlg.DataContext);
            nb.CancelRequest();

            var bk = nb.Book ?? new Book()
            {
                Isbn = nb.Isbn,
                AccessionNumber = nb.AccessionNumber,
                Condition = nb.Condition,
                DateReceived = nb.DateReceived,
                Fund = nb.Fund,
                Price = nb.Price,
            };
            if (!nb.IsBookFound)
            {
                bk.Title = nb.TemporaryBook.Title;
                bk.Author = nb.TemporaryBook.Author;
                bk.Barcode = nb.TemporaryBook.Barcode;
                bk.Section = nb.TemporaryBook.Section;
                bk.Subject = nb.TemporaryBook.Subject;
            }
            bk.Save();
        }));

        private ICommand _addBooksCommand;
        public ICommand AddBooksCommand => _addBooksCommand ?? (_addBooksCommand = new DelegateCommand(async obj =>
        {
            var dlg = new NewBooksDialog();
            var res = await DialogHost.Show(dlg, "RootDialog") as bool?;

            foreach (var newBook in dlg.ISBNs)
                newBook.CancelRequest();

            if (!(res ?? false)) return;

            foreach (var newBook in dlg.ISBNs)
                newBook.Book?.Save();
        }));

        private bool _multipleSelection;

        public bool MultipleSelection
        {
            get { return _multipleSelection; }
            set
            {
                if (value == _multipleSelection) return;
                _multipleSelection = value;
                OnPropertyChanged();
            }
        }

        private ICommand _selectCommand;
        public ICommand SelectBorrowerCommand => _selectCommand ?? (_selectCommand = new DelegateCommand(async obj =>
        {
            var dlg = new BorrowerSearch();
            var res = await DialogHost.Show(dlg, "RootDialog") as bool?;
            if (!(res ?? false))
                return;
            TakeoutViewModel.Barcode = (dlg.SearchResult.CurrentItem as Borrower)?.Barcode;
        }));
        
        public TakeoutViewModel TakeoutViewModel { get; } = new TakeoutViewModel(true);

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand<Book>(async book =>
        {
            if (book == null) return;
            var res = await MessageDialog.Show("CONFIRM DELETE", $"Are you sure you want to delete \"{book?.Title}\"?",
                "DELETE", "CANCEL", false);
            if (!(res ?? false)) return;
            book.Delete();
        },d=>d!=null));

        private ICommand _changeThumbnailCommand;

        public ICommand ChangeThumbnailCommand => _changeThumbnailCommand ?? (_changeThumbnailCommand = new DelegateCommand<Book>(ChangeThumbnail));

        private void ChangeThumbnail(Book obj)
        {
            var fd = new OpenFileDialog();
            fd.Multiselect = false;
            fd.CheckFileExists = true;
            fd.Filter = "Image Files (*.BMP; *.JPG; *.GIF; *.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            if (!(fd.ShowDialog() ?? false)) return;
            var p = Path.Combine(".", "Thumbnails");
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
            p = Path.Combine(p, $"{obj.Id}.pp");
            if (File.Exists(p)) File.Delete(p);
            try
            {
                File.Copy(fd.FileName, p);
                obj.Thumbnail = p;
            }
            catch (Exception)
            {
                //
            }
        }

        private Book _selectedBook;

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (Equals(value, _selectedBook)) return;
                _selectedBook = value;
                OnPropertyChanged();
                Takeouts.Refresh();
            }
        }

        private ListCollectionView _takeouts;

        public ListCollectionView Takeouts
        {
            get
            {
                if(_takeouts != null) return _takeouts;
                _takeouts = new ListCollectionView(Takeout.Cache);
                _takeouts.Filter = o =>
                {
                    if (!(o is Takeout t)) 
                        return false;
                    return t.BookId == SelectedBook?.Id;
                };
                return _takeouts;
            }
        }
        
        private ICommand _checkoutCommand;
        public ICommand CheckoutCommand => _checkoutCommand ?? (_checkoutCommand = new DelegateCommand(Checkout, CanTakeout));

        private bool CanTakeout(object obj)
        {
            return (SelectedBook != null && SelectedBook.Id > 0 && !SelectedBook.IsBorrowed) && (TakeoutViewModel.Borrower != null);
        }

        private void Checkout(object obj)
        {
            var tovm = TakeoutViewModel;
            var to = new Takeout()
            {
                UserId = 0,//Session.Current.User.Id,
                TakeOutNote = tovm.Note,
                TakeOutCondition = SelectedBook.Condition,
                BookId =SelectedBook.Id,
                BorrowerId = tovm.Borrower.Id
            };
            to.Save();
            SelectedBook.Update(nameof(Book.TakeoutId),to.Id);
            TakeoutViewModel.AddTakeout(to);
            OnPropertyChanged(nameof(SelectedBook));
            Messenger.Default.Broadcast(Messages.TakeoutsChanged);
        }

        private ICommand _returnBookCommand;

        public ICommand ReturnBookCommand
            => _returnBookCommand ?? (_returnBookCommand = new DelegateCommand<Takeout>(ReturnBook));

        private void ReturnBook(Takeout to)
        {
            to.IsReturned = true;
            if (string.IsNullOrWhiteSpace(to.ReturnCondition))
                to.ReturnCondition = SelectedBook.Condition;
            to.Save();
            SelectedBook.Update(nameof(Book.TakeoutId),0L);
            SelectedBook.Update(nameof(Book.Condition), to.ReturnCondition);
            TakeoutViewModel.RemoveTakeout(to);
            OnPropertyChanged(nameof(SelectedBook));
            Messenger.Default.Broadcast(Messages.TakeoutsChanged);
        }
        
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}