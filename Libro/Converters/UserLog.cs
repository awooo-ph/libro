using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libro.Models;

namespace Libro.Converters
{
    class UserLog : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var usr = value as User;
            if (usr == null) return null;
            return Takeout.GetByUser(usr.Id);
        }
    }
}
