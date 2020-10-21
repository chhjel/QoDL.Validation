using QoDL.DataAnnotations.LibraryValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace DevTesting.__Dev
{
    public static class ValidationLibrary
    {
        [LibraryValidationMethod(LibraryValidationMethod.Required)]
        public static string ValidateRequiredObj(object input, Type inputType, ValidationContext validationContext)
        {
            var valid = true;
            if (inputType == typeof(DateTime) && input == default)
            {
                valid = false;
            }
            else if (string.IsNullOrWhiteSpace(input?.ToString()))
            {
                valid = false;
            }

            return valid ? null : string.Format(Dictionary.Validation.GenericRequired, validationContext.DisplayName);
        }

        [LibraryValidationMethod(LibraryValidationMethod.Required)]
        public static string ValidateRequiredInt(int input)
        {
            return input == 0 ? "Example where 0 is not a valid integer" : null;
        }

        [LibraryValidationMethod(LibraryValidationMethod.PhoneNumber)]
        public static string ValidatePhoneNumber(string input, ValidationContext validationContext)
        {
            var valid = true;
            if (input == null || !int.TryParse(input, out int number))
            {
                valid = false;
            }
            else if (number.ToString().Length != 8)
            {
                valid = false;
            }

            return valid ? null : string.Format(Dictionary.Validation.PhoneInvalid, validationContext.DisplayName);
        }
    }
}