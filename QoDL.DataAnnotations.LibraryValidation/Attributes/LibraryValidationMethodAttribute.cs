using System;
using System.ComponentModel.DataAnnotations;

namespace QoDL.DataAnnotations.LibraryValidation.Attributes
{
    /// <summary>
    /// Decorate validation methods with this attribute.
    /// <para>Method must be public static.</para>
    /// <para>Method must have a string return type. When any non-null string is returned it becomes the validation error.</para>
    /// <para>Method must have a parameter of a matching type for the value to be validated.</para>
    /// <para>The method can optionally include a <see cref="ValidationContext"/> parameter.</para>
    /// <para>The method can optionally include a <see cref="Type"/> parameter that will be the input type.</para>
    /// <para>All matching validation methods will be invoked, stopping at the first error.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LibraryValidationMethodAttribute : Attribute
    {
        /// <summary>
        /// Your enum containing types of validation methods.
        /// </summary>
        public object ValidationTypeEnum { get; set; }

        /// <summary>
        /// Decorate validation methods with this attribute.
        /// <para>Method must be public static.</para>
        /// <para>Method must have a string return type. When any non-null string is returned it becomes the validation error.</para>
        /// <para>Method must have a parameter of a matching type for the value to be validated.</para>
        /// <para>The method can optionally include a <see cref="ValidationContext"/> parameter.</para>
        /// <para>The method can optionally include a <see cref="Type"/> parameter that will be the input type.</para>
        /// <para>All matching validation methods will be invoked, stopping at the first error.</para>
        /// </summary>
        /// <param name="validationTypeEnum">Your enum containing types of validation methods.</param>
        public LibraryValidationMethodAttribute(object validationTypeEnum)
        {
            ValidationTypeEnum = validationTypeEnum;
        }
    }
}
