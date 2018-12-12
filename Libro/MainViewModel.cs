using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Libro.Models;
using Libro.Properties;
using Libro.ViewModels;
using MaterialDesignThemes.Wpf;
using Settings = Libro.ViewModels.Settings;

namespace Libro
{
    partial class MainViewModel:ViewModelBase
    {
        private Timer _expirationTimer;
        private Timer _penaltyUpdater;
        private MainViewModel()
        {
            Initialize();

            _expirationTimer = new Timer(Settings.Instance.ExpirationTimer*1000);
            _penaltyUpdater = new Timer(Settings.Instance.PenaltyInterval*1000);

            _expirationTimer.AutoReset = true;
            _penaltyUpdater.AutoReset = true;

            _expirationTimer.Elapsed += (sender, args) => CheckExpiration();

            _penaltyUpdater.Elapsed += (sender, args) => CheckPenalty(); 

            Messenger.Default.AddListener(Libro.Messages.SettingsChanged, () =>
                {
                    _expirationTimer.Interval = Settings.Instance.ExpirationTimer * 1000;
                });

            _expirationTimer.Start();
            _penaltyUpdater.Start();
        }

        private void CheckPenalty()
        {
            var expired = Takeout.Cache.Where(x => !x.IsReturned && IsExpired(x.TakeoutDate)).ToList();
            foreach (var takeout in expired)
            {
                var penalty = 0.0;
                if (Settings.Instance.PenaltyInterval == 0)
                    penalty = Settings.Instance.Penalty;
                else if (Settings.Instance.PenaltyInterval == 1)
                    penalty = ((long)Math.Ceiling((DateTime.Now - takeout.TakeoutDate)
                        .Add(TimeSpan.FromHours(-Settings.Instance.MaximumTakeoutHours))
                        .TotalHours));
                else if (Settings.Instance.PenaltyInterval == 2)
                    penalty = ((long)Math.Ceiling((DateTime.Now - takeout.TakeoutDate)
                                  .Add(TimeSpan.FromHours(-Settings.Instance.MaximumTakeoutHours))
                                  .TotalDays)) * Settings.Instance.Penalty;

                if (penalty < 0) penalty = 0;
                takeout.Update(nameof(takeout.Penalty), penalty);
            }
        }

        private bool _checking;
        private void CheckExpiration()
        {
            if (_checking) return;
            _checking = true;
            
            var expired = Takeout.Cache.Where(x => !x.IsReturned && IsExpired(x.TakeoutDate)).ToList();
            foreach (var takeout in expired)
            {
                var notification = Notification.Cache.FirstOrDefault(x =>
                    x.NotificationType == Notification.NotificationTypes.TakeoutExpired &&
                    x.RecordId == takeout.Id);
                if(notification!=null) continue;
                var borrower = Borrower.GetById(takeout.BorrowerId);
                var book = Book.GetById(takeout.BookId);
                notification = new Notification()
                {
                    Thumbnail = book.Thumbnail,
                    RecordId = takeout.Id, 
                    NotificationType = Notification.NotificationTypes.TakeoutExpired,
                    Title = "BOOK NOT RETURNED",
                    Message = $"{borrower.Fullname} did not return the book {book.Title} borrowed last {takeout.TakeoutDate.ToShortDateString()}."
                };
                App.Current.Dispatcher.Invoke(() =>notification.Save());
                Messages.Enqueue(notification);
                Resources.chime_glass_note_hi.Play();

            }

            _checking = false;
        }
        
        private bool IsExpired(DateTime date)
        {
            return (DateTime.Now - date).TotalHours > Settings.Instance.MaximumTakeoutHours;
        }
        
        private SnackbarMessageQueue _messages;
        public SnackbarMessageQueue Messages
        {
            get
            {
                if (_messages != null) return _messages;
                _messages = new SnackbarMessageQueue(TimeSpan.FromSeconds(7));
                return _messages;
            }
        }

        partial void Initialize();
        public string AppTitle { get; private set; }
        public string AppIcon { get; private set; }
        
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

        private bool _IsAuthenticated;

        public bool IsAuthenticated
        {
            get => _IsAuthenticated;
            private set
            {
                if (value == _IsAuthenticated) return;
                _IsAuthenticated = value;
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }

        
        
        private bool _ShowNotifications;

        public bool ShowNotifications
        {
            get => _ShowNotifications;
            set
            {
                if (value == _ShowNotifications) return;
                _ShowNotifications = value;
                OnPropertyChanged(nameof(ShowNotifications));
                if (value)
                {
                    foreach (var notification in Notification.Cache)
                    {
                        notification.Update(nameof(Notification.Read),true);
                    }

                    UnreadNotifications = null;
                }
            }
        }

        private ICommand _toggleNotifications;

        public ICommand ToggleNotifications => _toggleNotifications ?? (_toggleNotifications = new DelegateCommand(d =>
        {
            if (Notification.Cache.Count == 0) return;
            ShowNotifications = !ShowNotifications;
        }));

        
        private ListCollectionView _notifications;

        public ListCollectionView Notifications
        {
            get
            {
                if (_notifications != null) return _notifications;
                _notifications = (ListCollectionView) CollectionViewSource.GetDefaultView(Notification.Cache);
                Notification.Cache.CollectionChanged += (sender, args) =>
                {
                    if (Notification.Cache.Any(x=>!x.Read))
                        UnreadNotifications = Notification.Cache.Count(x=>!x.Read);
                    else
                        UnreadNotifications = null;
                };
                if (Notification.Cache.Any(x=>!x.Read))
                    UnreadNotifications = Notification.Cache.Count(x=>!x.Read);
                else
                    UnreadNotifications = null;
                _notifications.LiveFilteringProperties.Add(nameof(Notification.Read));
                _notifications.IsLiveFiltering = true;
                _notifications.SortDescriptions.Add(new SortDescription(nameof(Notification.Created), ListSortDirection.Descending));
                return _notifications;
            }
        }
        
        private long? _UnreadNotifications;
        
        public long? UnreadNotifications
        {
            get => _UnreadNotifications;
            set
            {
                if (value == _UnreadNotifications) return;
                _UnreadNotifications = value;
                OnPropertyChanged(nameof(UnreadNotifications));
            }
        }

        public bool Login(NetworkCredential login)
        {
            if (login.UserName.ToLower() != Settings.Instance.Username) return false;
            if (string.IsNullOrEmpty(Settings.Instance.Password))
                Settings.Instance.Password = login.Password;
            if (login.Password != Settings.Instance.Password || string.IsNullOrEmpty(login.Password)) return false;
            IsAuthenticated = true;
            return true;
        }

        private ICommand _showBorrowerCommand;

        public ICommand ShowBorrowerCommand =>
            _showBorrowerCommand ?? (_showBorrowerCommand = new DelegateCommand<Notification>(d =>
            {
                ShowNotifications = false;
                ((MainWindow) App.Current.MainWindow).BorrowersTab.IsChecked = true;
                Students.Instance.ShowBorrower(d.Borrower);
                d.Update(nameof(d.Read),true);
                d.Delete(true);
            }));
    }
}
