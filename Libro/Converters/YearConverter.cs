using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Converters
{
    class YearConverter : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var s = value as string;
            if (s == null) return value;
            DateTime date;
            if (DateTime.TryParse(s, out date))
            {
                return date.Year;
            }
            return value;
        }
    }
}
