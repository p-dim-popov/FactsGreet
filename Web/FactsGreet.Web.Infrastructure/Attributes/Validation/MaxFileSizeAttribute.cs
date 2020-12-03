namespace FactsGreet.Web.Infrastructure.Attributes.Validation
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long maxBytes;

        public MaxFileSizeAttribute(long maxBytes)
        {
            this.maxBytes = maxBytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is IFormFile file))
            {
                return ValidationResult.Success;
            }

            if (file.Length > this.maxBytes)
            {
                return new ValidationResult($"Maximum file size is {this.maxBytes / 1024:f0}kb");
            }

            return ValidationResult.Success;
        }
    }
}
