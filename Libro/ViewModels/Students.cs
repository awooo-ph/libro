using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        //private bool _showExpired;

        //public bool ShowExpired
        //{
        //    get => _showExpired;
        //    set
        //    {
        //        if (value == _showExpired) return;
        //        _showExpired = value;
        //        OnPropertyChanged(nameof(ShowExpired));
        //        BorrowersView.Filter = FilterBorrower;
        //    }
        //}

        //private bool _ShowUnpaid;

        //public bool ShowUnpaid
        //{
        //    get => _ShowUnpaid;
        //    set
        //    {
        //        if (value == _ShowUnpaid) return;
        //        _ShowUnpaid = value;
        //        OnPropertyChanged(nameof(ShowUnpaid));
        //        BorrowersView.Filter = FilterBorrower;
        //    }
        //}



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
            if (b == null) return false;
            if(!ShowAll && (ShowFaculty && b.IsStudent || ShowStudents && !b.IsStudent)) return false;

            //if (ShowUnpaid && Takeout.GetUnpaidByBorrower(b.Id).Count==0) return false;
            //if (ShowExpired && Takeout.GetUnreturnedByBorrower(b.Id).Count == 0) return false;

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
            => _deleteCommand ?? (_deleteCommand = new DelegateCommand<Borrower>(async obj =>
            {
                var res = await MessageDialog.Show($"Confirm Delete {obj.Fullname}?",
                    $"Deleting borrowers can not be undone. Click DELETE to permanently delete {obj.Fullname}.",
                    "DELETE", "CANCEL");
                if (res) obj.Delete();
            }));
        
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

        private Students()
        {
           
        }
        
        private static ICommand _payCommand;
        public static ICommand PayCommand => _payCommand ?? (_payCommand = new DelegateCommand<Takeout>(async to =>
        {
            if (to == null) return;
            if (!to.IsReturned)
            {
                await MessageDialog.Show("The book is not yet returned.",
                    $"{to.Borrower.Fullname} has not returned the book {to.Book.Title} yet.",
                    "OKAY");
                return;
            }
            to.Update(nameof(to.Paid),true);
        }, to=>to!=null));

        private static ICommand _returnCommand;
        public static ICommand ReturnCommand => _returnCommand ?? (_returnCommand = new DelegateCommand<Takeout>(async to =>
        {
            if (to == null) return;
            if (to.HasPenalty)
            {
                var res = await MessageDialog.Show($"Has {to.Borrower.Firstname} paid the {to.Penalty:#,##0.00} penalty?",
                    $"{to.Borrower.Fullname} has not returned the book {to.Book.Title} on time. He/she must pay {to.Penalty:#,##0.00} as penalty.\nClick YES if he/she has paid the full amount.",
                    "YES", "NO", true);
                if (res == null) return;
                if((bool)res) to.Update(nameof(to.Paid),true);    
            }

            to.IsReturned = true;
            to.Returned = DateTime.Now;
            to.Save();
        }, to=>to!=null));

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
        public ICommand AddNewCommand => _addNewCommand ?? (_addNewCommand = new DelegateCommand(async d =>
        {
            var borrower = new Borrower() { DepartmentType = Departments.College };
            var dlg = new BorrowerEditor("", "ADD") { DataContext = borrower };

            if (await DialogHost.Show(dlg, "RootDialog") is bool res && res) borrower.Save();
        }));


        private ICommand _editCommand;
        public ICommand EditCommand => _editCommand ?? (_editCommand = new DelegateCommand<Borrower>(async item =>
        {
            var brw = new Borrower()
            {
                Id = item.Id,
                Firstname = item.Firstname,
                SchoolId = item.SchoolId,
                Lastname = item.Lastname,
                DepartmentType = item.DepartmentType,
                ContactNumber = item.ContactNumber,
                Course = item.Course,
                Barcode = item.Barcode
            };
            var dlg = new BorrowerEditor("", "SAVE") { DataContext = brw };
            var res = await DialogHost.Show(dlg, "RootDialog") as bool?;

            if (!(res ?? false)) return;

            item.Firstname = brw.Firstname;
            item.Lastname = brw.Lastname;
            item.ContactNumber = brw.ContactNumber;
            item.Course = brw.Course;
            item.DepartmentType = brw.DepartmentType;
            item.SchoolId = brw.SchoolId;
            item.Barcode = brw.Barcode;
            item.Save();
        }));
        
        public void ShowBorrower(Borrower borrower)
        {
            //ShowUnpaid = false;
            //ShowExpired = false;
            ShowAll = true;
            BorrowersView.MoveCurrentTo(borrower);
        }
    }
}
