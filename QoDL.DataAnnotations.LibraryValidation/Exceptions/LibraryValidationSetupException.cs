using System;
using System.Runtime.Serialization;

namespace QoDL.DataAnnotations.LibraryValidation.Exceptions
{
    /// <summary>
    /// Thrown when you have done something wrong in your code.
    /// </summary>
    [Serializable]
    public class LibraryValidationSetupException : Exception
    {
        /// <summary>
        /// Thrown when you have done something wrong in your code.
        /// </summary>
        public LibraryValidationSetupException() { }

        /// <summary>
        /// Create a new <see cref="LibraryValidationSetupException"/>.
        /// </summary>
        public LibraryValidationSetupException(string message) : base(message) { }

        /// <summary>
        /// Create a new <see cref="LibraryValidationSetupException"/>.
        /// </summary>
        public LibraryValidationSetupException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Create a new <see cref="LibraryValidationSetupException"/>.
        /// </summary>
        protected LibraryValidationSetupException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
