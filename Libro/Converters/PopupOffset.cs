using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Converters
{
    class PopupOffset:ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (!(value is double d)) return 0;
            return -(d / 2);
        }
    }
}
