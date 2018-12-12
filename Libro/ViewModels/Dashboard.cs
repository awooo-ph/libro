using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libro.Models;
using LiveCharts;
using LiveCharts.Configurations;

namespace Libro.ViewModels
{
    class Dashboard:ViewModelBase
    {
        private static Dashboard _instance;
        public static Dashboard Instance => _instance ?? (_instance = new Dashboard());
        
        public List<TopBook> _topTitles;

        public List<TopBook> TopTitles
        {
            get
            {
                if (_topTitles != null) return _topTitles;
                _topTitles = new List<TopBook>(Book.GetTopTitle().Select(d=>new TopBook(){Title = d.Title,Count=d.Usage}));
                return _topTitles;
            }
        }

        private List<TopBook> _TopBorrowers;

        public List<TopBook> TopBorrowers
        {
            get
            {
                if (_TopBorrowers != null) return _TopBorrowers;
                _TopBorrowers = new List<TopBook>(Borrower.GetTopList().Select(d => new TopBook() { Title = d.Title, Count = d.Usage }));
                return _TopBorrowers;
            }
        }

        public long BookCount => Book.Cache.Count;
        public long UnreturnedBooks => Takeout.Cache.Count(x => !x.IsReturned);
        public double UnpaidPenalty => Takeout.Cache.Where(x => !x.Paid).Sum(x => x.Penalty);

        public ChartValues<long> Elementary { get; set; } = new ChartValues<long>();
        public ChartValues<long> HighSchool { get; set; } = new ChartValues<long>();
        public ChartValues<long> College { get; set; } = new ChartValues<long>();
        public ChartValues<long> Faculty { get; set; } = new ChartValues<long>();

        public void Refresh()
        {
            OnPropertyChanged();
        }

        private Dashboard()
        {
            foreach (var usage in DailyUsage.Cache)
            {
                Elementary.Add(usage.Elementary);
                HighSchool.Add(usage.HighSchool);
                College.Add(usage.College);
                Faculty.Add(usage.Faculty);
            }
            
            //DateFormatter = d =>
            //{
            //    return _Dates[(int) d].Date.ToString("d");
            //}; 
            //TotalUsage = d => { return Elementary[(int) d].ToString(); };
        }
        
        //public Func<double,string> TotalUsage { get; set; }
        public DateTime InitialDate { get; set; } = DailyUsage.Cache.FirstOrDefault()?.Date.Date??DateTime.Now.Date;
    }

    public class BorrowCount
    {
        public DateTime DateTime { get; set; }
        public long Value { get; set; }
    }

    public class TopBook
    {
        public string Title { get; set; }
        public long Count { get; set; }
    }
}
