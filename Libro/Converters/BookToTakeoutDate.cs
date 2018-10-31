using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Libro.Models;

namespace Libro.Converters
{
    class BookToTakeoutDate : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var bo = value as Book;
            if (bo == null) return Binding.DoNothing;
            var to = Takeout.GetById(bo.TakeoutId);
            if(to == null)
                return Binding.DoNothing;
            return to.TakeoutDate;
        }
    }
}
