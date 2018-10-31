using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Converters
{
    class IsNullToBoolean : ConverterBase
    {
        public Type Type { get; set; }

        

        protected override object Convert(object value, Type targetType, object parameter)
        {
            var t = value?.GetType();
            return (value!=null && t == Type);
        }
    }
}
