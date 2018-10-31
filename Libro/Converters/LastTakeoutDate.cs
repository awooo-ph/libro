using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Libro.Models;

namespace Libro.Converters
{
    class LastTakeoutDate : ConverterBase
    {
        public string Format { get; set; } = "d";

        protected override object Convert(object value, Type targetType, object parameter)
        {
            var bk = value as Book;
            if (bk == null) return Binding.DoNothing;
            var to = Takeout.GetById(bk.TakeoutId);
            if (to == null) return "NO RECORD";
            return to.TakeoutDate.ToString(Format);
        }
    }
}
