using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Libro
{
    class MainViewModel:ViewModelBase
    {
        private MainViewModel(){}

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

        public bool Login(NetworkCredential login)
        {
            IsAuthenticated = true;
            return true;
        }

        public static ICommand ShowBorrowerCommand { get; }
    }
}
