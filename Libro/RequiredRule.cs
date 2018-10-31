using System.Globalization;
using System.Windows.Controls;

namespace Libro.Validation
{
    class RequiredRule:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(string.IsNullOrEmpty(value as string)) return new ValidationResult(false,"This cannot be empty.");
            return ValidationResult.ValidResult;
        }
    }
}
