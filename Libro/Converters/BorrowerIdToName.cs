using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libro.Models;

namespace Libro.Converters
{
    class BorrowerIdToName : ConverterBase
    {
        public bool UpperCase { get; set; } = false;

        protected override object Convert(object value, Type targetType, object parameter)
        {
            var id = (long) value;
            var n = Borrower.GetById(id)?.Fullname;
            if (UpperCase) n = n.ToUpper();
            return n;
        }
    }
}
