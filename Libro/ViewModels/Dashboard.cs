using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libro.Models;

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

        public long BookCount => Book.Cache.Count;
        public long UnreturnedBooks => Takeout.Cache.Count(x => !x.IsReturned);
        public double UnpaidPenalty => Takeout.Cache.Where(x => !x.Paid).Sum(x => x.Penalty);

        public void Refresh()
        {
            OnPropertyChanged();
        }
    }

    public class TopBook
    {
        public string Title { get; set; }
        public long Count { get; set; }
    }
}
