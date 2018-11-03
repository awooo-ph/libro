﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Libro.Models;

namespace Libro
{
    partial class MainViewModel:ViewModelBase
    {
        private MainViewModel()
        {
            Initialize();
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
            }
        }

        private ICommand _toggleNotifications;

        public ICommand ToggleNotifications => _toggleNotifications ?? (_toggleNotifications = new DelegateCommand(d =>
        {
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
            IsAuthenticated = true;
            return true;
        }

        public static ICommand ShowBorrowerCommand { get; }
    }
}
