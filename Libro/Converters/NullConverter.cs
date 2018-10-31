using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Libro.Converters
{
    class NullConverter : ConverterBase
    {
        public string NullValue { get; set; }

        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (value is DateTime && ((DateTime) value) == DateTime.MinValue) return NullValue;
            if (value == null) return NullValue;
            return Binding.DoNothing;
        }
    }
}
