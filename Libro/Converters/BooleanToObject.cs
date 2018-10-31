using System;

namespace Libro.Converters
{
    class BooleanToObject:ConverterBase
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        
        public BooleanToObject(object trueValue, object falseValue=null)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        protected override object Convert(object value, Type targetType, object parameter)
        {
            if((bool) value) return TrueValue;
            return FalseValue;

        }
    }
}
