using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Libro.Converters
{
    class VisibilityRange : ConverterBase
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; } = double.MaxValue;

        protected override object Convert(object value, Type targetType, object parameter)
        {
            if ((double) value > Minimum && (double)value<Maximum)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
    }
}
