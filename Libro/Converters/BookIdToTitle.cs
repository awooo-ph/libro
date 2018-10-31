using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libro.Models;

namespace Libro.Converters
{
    class BookIdToTitle : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var id = (long) value;
            return Book.GetById(id)?.Title;
        }
    }
}
