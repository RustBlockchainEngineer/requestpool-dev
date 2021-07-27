using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Foundation.Core
{
    public class CustomRegularExpression: ValidationAttribute
    {
        public string Pattern { get; set; }
        public RegexOptions Options { get; set; }

        public CustomRegularExpression(string pattern):this(pattern, RegexOptions.ECMAScript)
        {

        }
        public CustomRegularExpression(string pattern, RegexOptions options)
        {
            Pattern = pattern;
            Options = options;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || Regex.IsMatch(value.ToString(), Pattern, Options))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }
    }
}