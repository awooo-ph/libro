using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Converters
{
    class ToUpperCase : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            return value?.ToString().ToUpper();
        }
    }
}
