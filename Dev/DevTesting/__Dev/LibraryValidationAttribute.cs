using QoDL.DataAnnotations.LibraryValidation.Attributes;

namespace DevTesting.__Dev
{
    public class LibraryValidationAttribute : LibraryValidationBaseAttribute
    {
        public LibraryValidationAttribute(LibraryValidationMethod method, bool optional = false)
            : base(typeof(ValidationLibrary), method, optional) { }
    }
}