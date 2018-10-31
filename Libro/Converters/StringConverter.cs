using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Converters
{
    class StringConverter : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var s = (string) value;
            return s?.Replace(Environment.NewLine, " ");
        }
    }
}
