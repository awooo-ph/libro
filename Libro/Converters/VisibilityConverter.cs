using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Libro.Converters
{
    class VisibilityConverter:ConverterBase
    {
        public object Value { get; set; }

        public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (value?.ToString() == Value?.ToString())
                return Visibility.Visible;
            return FalseVisibility;
        }
    }
}
