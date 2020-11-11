namespace FactsGreet.Web.Infrastructure.ValidationAttributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    using Microsoft.AspNetCore.Http;

    public class GeneralImageAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is IFormFile image))
            {
                return ValidationResult.Success;
            }

            if (!Regex.IsMatch(image.ContentType, @"image/(png|gif|jpeg)"))
            {
                return new ValidationResult("Content type not supported. Supported types are: png, gif, jpeg");
            }

            return ValidationResult.Success;
        }
    }
}