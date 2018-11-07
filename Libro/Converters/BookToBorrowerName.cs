using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Libro.Models;

namespace Libro.Converters
{
    class BookToBorrowerName : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (!(value is long id)) return Binding.DoNothing;
            if (id == 0) return "[UNKNOWN]";
            //var bk = value as Book;
            //if (bk == null) return Binding.DoNothing;
            var to = Takeout.GetById(id);
            if (to == null) return "[UNKNOWN]";
            var borrower = Borrower.GetById(to.BorrowerId);
            if (borrower == null) return "[UNKNOWN]";
            return borrower.Fullname.ToUpper();
        }
    }
}
