using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Libro.Annotations;

namespace Libro
{
    class DateTimeHelper : INotifyPropertyChanged
    {
        private static DateTimeHelper _instance;
        public static DateTimeHelper Instance => _instance ?? (_instance = new DateTimeHelper());

        private SynchronizationContext context;
        private DateTimeHelper()
        {
            context = SynchronizationContext.Current;
            BindTime();
        }
        
        private Task tick;
        
        private void BindTime()
        {
            if (tick != null) return;
            tick = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    context.Post(d =>
                    {
                        DateTime = DateTime.Now;
                    },null);
                    
                    await Task.Delay(1000);
                }
                
            });
        }

        private DateTime _dateTime;

        public DateTime DateTime
        {
            get { return _dateTime; }
            private set
            {
                if (value.Equals(_dateTime)) return;
                _dateTime = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
