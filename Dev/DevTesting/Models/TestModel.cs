using DevTesting.__Dev;

namespace DevTesting.Models
{
    public class TestModel
    {
        [LibraryValidation(LibraryValidationMethod.Required)]
        [LibraryValidation(LibraryValidationMethod.PhoneNumber)]
        public string SomeRequiredString { get; set; }

        [LibraryValidation(LibraryValidationMethod.Required)]
        public int SomeRequiredInt { get; set; }

        [LibraryValidation(LibraryValidationMethod.Required)]
        public bool SomeRequiredBool { get; set; }

        [LibraryValidation(LibraryValidationMethod.Required)]
        public System.DateTime SomeRequiredDate { get; set; }

        [LibraryValidation(LibraryValidationMethod.PhoneNumber)]
        public string Phone { get; set; }

        [LibraryValidation(LibraryValidationMethod.PhoneNumber, optional: true)]
        public string OptionalPhone { get; set; }

        public string AnotherThingWithoutValidation { get; set; }
    }
}