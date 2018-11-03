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
