using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Libro.Annotations;
using Libro.Dialogs;
using Libro.Models;
using Libro.Views;
using MaterialDesignThemes.Wpf;

namespace Libro.ViewModels
{
    class Students 
    {
        private bool _showStudents;

        public bool ShowStudents
        {
            get { return _showStudents; }
            set
            {
                if(value == _showStudents)
                    return;
                _showStudents = value;
                if(!value && !ShowFaculty)
                    ShowAll = true;
                OnPropertyChanged();
                BorrowersView.Filter = FilterBorrower;
            }
        }

        private bool _showFaculty;

        public bool ShowFaculty
        {
            get { return _showFaculty; }
            set
            {
                if(value == _showFaculty)
                    return;
                _showFaculty = value;
                if(!value && !ShowStudents)
                    ShowAll = true;
                OnPropertyChanged();
                BorrowersView.Filter = FilterBorrower;
            }
        }

        private bool _showAll = true;

        public bool ShowAll
        {
            get { return _showAll; }
            set
            {
                if(value == _showAll)
                    return;
                if(!value && !(ShowFaculty || ShowStudents))
                    value = true;
                _showAll = value;
                OnPropertyChanged();
                BorrowersView.Filter = FilterBorrower;
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
                BorrowersView.Filter = FilterBorrower;
            }
        }

        private ListCollectionView _borrowersView;

        public ListCollectionView BorrowersView
        {
            get
            {
                if (_borrowersView != null) return _borrowersView;
                _borrowersView = new ListCollectionView(Borrower.Cache);
                _borrowersView.CurrentChanged += (sender, args) =>
                {
                    TakeoutsView.Refresh();
                };
                _borrowersView.Filter = FilterBorrower;
                return _borrowersView;
            }
        }

        private bool FilterBorrower(object o)
        {
            var b = (Borrower) o;
            if(!(ShowAll || (ShowStudents && b.IsStudent) || (ShowFaculty && !b.IsStudent))) return false;
            if (string.IsNullOrEmpty(SearchKeyword)) return true;
            if (b.Firstname.ToLower().Contains(SearchKeyword.ToLower())) return true;
            if(b.Lastname.ToLower().Contains(SearchKeyword.ToLower()))
                return true;
            if(b.Course.ToLower().Contains(SearchKeyword.ToLower()))
                return true;
            if(b.SchoolId.ToLower().Contains(SearchKeyword.ToLower()))
                return true;
            if(b.ContactNumber.ToLower().Contains(SearchKeyword.ToLower()))
                return true;
            return false;
        }

        //private Borrower _selectedItem;

        //public Borrower SelectedItem
        //{
        //    get { return _selectedItem; }
        //    set
        //    {
        //        if (Equals(value, _selectedItem)) return;
        //        _selectedItem = value;
        //        OnPropertyChanged();
        //        _takeouts = null;
        //        _transactionsView = null;
        //        OnPropertyChanged(nameof(Takeouts));
        //        OnPropertyChanged(nameof(TakeoutsView));
        //    }
        //}

        //private ObservableCollection<Takeout> _takeouts;

        //private ObservableCollection<Takeout> Takeouts
        //{
        //    get
        //    {
        //        if (_takeouts != null) return _takeouts;
        //        _takeouts = new ObservableCollection<Takeout>(Takeout.GetByBorrower(SelectedItem?.Id));
        //        return _takeouts;
        //    }
        //}

        private ListCollectionView _transactionsView;

        public ListCollectionView TakeoutsView
        {
            get
            {
                if (_transactionsView != null) return _transactionsView;
                _transactionsView = (ListCollectionView) CollectionViewSource.GetDefaultView(Takeout.Cache);
                _transactionsView.Filter = FilterTransactions;
                return _transactionsView;
            }
        }

        private bool FilterTransactions(object o)
        {
            if (!(o is Takeout to)) return false;
            if (!(BorrowersView.CurrentItem is Borrower b)) return false;
            return to.BorrowerId == b.Id;
        }
        
        private ICommand _deleteCommand;

        public ICommand DeleteCommand
            => _deleteCommand ?? (_deleteCommand = new DelegateCommand<Borrower>(DeleteBorrower));

        private async void DeleteBorrower(Borrower obj)
        {
            //var mv = Application.Current.MainWindow as MetroWindow;
            //if (mv == null) return;
            //var res = await mv.ShowMessageAsync("Delete " + obj.Fullname + "?",
            //        $"You are about to delete {obj.Fullname}. This action can not be undone.",
            //        MessageDialogStyle.AffirmativeAndNegative,
            //        new MetroDialogSettings() {
            //            AffirmativeButtonText = "DELETE",
            //            NegativeButtonText = "CANCEL",
            //            ColorScheme = MetroDialogColorScheme.Accented
            //        });
            //if (res == MessageDialogResult.Negative) return;
           // Borrowers.Remove(obj);
            obj.Delete();
        }

        private static Students _students;

        public static Students Instance
        {
            get
            {
                if (_students != null) return _students;
                _students = new Students();
                return _students;
            }
        }

     //   private SynchronizationContext context;
        private Students()
        {
           // context = SynchronizationContext.Current;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private FrameworkElement _view;

        public FrameworkElement View
        {
            get
            {
                if (_view != null) return _view;
                _view = new StudentsView() {DataContext = this};
                return _view;
            }
            set
            {
                if (Equals(value, _view)) return;
                _view = value;
                OnPropertyChanged();
            }
        }

        public string Title { get; set; } = "Borrowers";

        private ICommand _addNewCommand;
        public ICommand AddNewCommand => _addNewCommand ?? (_addNewCommand = new DelegateCommand(AddNew));

        private async void AddNew(object obj)
        {
            var borrower = new Borrower() {IsStudent = ShowStudents};
            
            var dlg = new BorrowerEditor("","ADD") {DataContext = borrower};
            var res = await DialogHost.Show(dlg, "RootDialog") as bool?;
            if (res ?? false) borrower.Save();
        }

        private ICommand _editCommand;
        public ICommand EditCommand => _editCommand ?? (_editCommand = new DelegateCommand<Borrower>(Edit));

        private async void Edit(Borrower item)
        {
            var brw = new Borrower()
            {
                Id = item.Id,
                Firstname = item.Firstname,
                SchoolId = item.SchoolId,
                Lastname = item.Lastname,
                IsStudent = item.IsStudent,
                ContactNumber = item.ContactNumber,
                Course = item.Course,
                Barcode = item.Barcode
            };
            var dlg = new BorrowerEditor("", "SAVE") { DataContext = brw };
            var res = await DialogHost.Show(dlg, "RootDialog") as bool?;

            if(!res ?? false)
                return;

            item.Firstname = brw.Firstname;
            item.Lastname = brw.Lastname;
            item.ContactNumber = brw.ContactNumber;
            item.Course = brw.Course;
            item.IsStudent = brw.IsStudent;
            item.SchoolId = brw.SchoolId;
            item.Barcode = brw.Barcode;
            item.Save();
           
        }
    }
}
