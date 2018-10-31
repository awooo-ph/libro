using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libro.Converters
{
    class IsNotCurrentUser : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            return false;// ((long) value) != Session.Current.User?.Id;
        }
    }
}
