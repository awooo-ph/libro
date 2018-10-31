using System;

namespace Libro.Converters
{
    class DateToAgeConverter : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var dob =(DateTime) value;
            if (dob == DateTime.MinValue) return "DATE OF  BIRTH";

            var today = DateTime.Today;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dob.Year * 100 + dob.Month) * 100 + dob.Day;

            return $"DATE OF BIRTH (Age: {((a - b) / 10000)} Years)";
        }
    }
}
