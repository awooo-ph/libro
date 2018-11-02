using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Converters
{
    class PercentageConverter:ConverterBase
    {
        public double Percentage { get; set; } = 1;

        public PercentageConverter(double percent)
        {
            Percentage = percent;
        }
        public  PercentageConverter(){}

        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (!(value is double d)) return 0;
            return d * Percentage;
        }
    }
}
