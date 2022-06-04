using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebApiAutores.Validations
{
    public class EachWordCapAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            string valueCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToString().ToLower());

            if(valueCase != value.ToString())
                return new ValidationResult("Each word within the name must be capitalized");

            //var firstLetter = value.ToString()[0].ToString();

            //if(firstLetter != firstLetter.ToUpper())
            //    return new ValidationResult("First letter must be capitalized");

            return ValidationResult.Success;
        }
    }
}
