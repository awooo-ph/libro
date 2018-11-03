using System.Windows.Input;
using Jot;
using Jot.DefaultInitializer;
using Jot.Storage;
using Jot.Triggers;

namespace Libro.ViewModels
{
    class Settings:ViewModelBase
    {
        public static StateTracker Tracker { get; } = new StateTracker(new JsonFileStoreFactory(), new DesktopPersistTrigger());

        private Settings()
        {
            Tracker.Configure(this)
                .Apply();
        }

        private static Settings _instance;
        public static Settings Instance => _instance ?? (_instance = new Settings());

        private bool _ShowGeneralSettings = true;
        [Trackable]
        public bool ShowGeneralSettings
        {
            get => _ShowGeneralSettings;
            set
            {
                if (value == _ShowGeneralSettings) return;
                _ShowGeneralSettings = value;
                OnPropertyChanged(nameof(ShowGeneralSettings));
            }
        }

        private bool _ShowSettings;

        public bool ShowSettings
        {
            get => _ShowSettings;
            set
            {
                if (value == _ShowSettings) return;
                _ShowSettings = value;
                OnPropertyChanged(nameof(ShowSettings));
            }
        }

        
        private ICommand _toggleSettings;

        public ICommand ToggleSettings => _toggleSettings ?? (_toggleSettings = new DelegateCommand(d =>
        {
            ShowSettings = !ShowSettings;
        }));

        private long _MaximumTakeoutHours = 24;
        [Trackable]
        public long MaximumTakeoutHours
        {
            get => _MaximumTakeoutHours;
            set
            {
                if (value == _MaximumTakeoutHours) return;
                _MaximumTakeoutHours = value;
                OnPropertyChanged(nameof(MaximumTakeoutHours));
            }
        }

        private long _Penalty;
        [Trackable]
        public long Penalty
        {
            get => _Penalty;
            set
            {
                if (value == _Penalty) return;
                _Penalty = value;
                OnPropertyChanged(nameof(Penalty));
            }
        }

        private string _WebApi;
        [Trackable]
        public string WebApi
        {
            get => _WebApi;
            set
            {
                if (value == _WebApi) return;
                _WebApi = value;
                OnPropertyChanged(nameof(WebApi));
            }
        }

        private string _ModemPort;
        [Trackable]
        public string ModemPort
        {
            get => _ModemPort;
            set
            {
                if (value == _ModemPort) return;
                _ModemPort = value;
                OnPropertyChanged(nameof(ModemPort));
            }
        }

        private string _CommandPath;
        [Trackable]
        public string CommandPath
        {
            get => _CommandPath;
            set
            {
                if (value == _CommandPath) return;
                _CommandPath = value;
                OnPropertyChanged(nameof(CommandPath));
            }
        }

        private string _CommandArguments;
        [Trackable]
        public string CommandArguments
        {
            get => _CommandArguments;
            set
            {
                if (value == _CommandArguments) return;
                _CommandArguments = value;
                OnPropertyChanged(nameof(CommandArguments));
            }
        }



        private int _SmsIntegrations;
        [Trackable]
        public int SmsIntegration
        {
            get => _SmsIntegrations;
            set
            {
                if (value == _SmsIntegrations) return;
                _SmsIntegrations = value;
                OnPropertyChanged(nameof(SmsIntegration));
            }
        }
        

        private bool _SendSms;
        [Trackable]
        public bool SendSms
        {
            get => _SendSms;
            set
            {
                if (value == _SendSms) return;
                _SendSms = value;
                OnPropertyChanged(nameof(SendSms));
            }
        }



        public enum PenaltyIntervals
        {
            Once,
            Hourly,
            Daily
        }

        private int _PenaltyInterval=0;
        [Trackable]
        public int PenaltyInterval
        {
            get => _PenaltyInterval;
            set
            {
                if (value == _PenaltyInterval) return;
                _PenaltyInterval = value;
                OnPropertyChanged(nameof(PenaltyInterval));
            }
        }

        private ICommand _togglePenaltyInterval;

        public ICommand TogglePenaltyIntervalCommand =>
            _togglePenaltyInterval ?? (_togglePenaltyInterval = new DelegateCommand(
                d =>
                {
                    if (PenaltyInterval == 2)
                    {
                        PenaltyInterval = 0;
                    }
                    else
                    {
                        PenaltyInterval++;
                    }
                }));
    }
}
