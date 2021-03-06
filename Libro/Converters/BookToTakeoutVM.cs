﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Libro.Models;

namespace Libro.Converters
{
    class BookToTakeoutVM:ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var bk = value as Book;
            if (bk == null) return Binding.DoNothing;
            var to = Takeout.GetById(bk.TakeoutId);
            return to;
        }
    }
}
