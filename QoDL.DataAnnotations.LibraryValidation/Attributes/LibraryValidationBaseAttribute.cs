using QoDL.DataAnnotations.LibraryValidation.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace QoDL.DataAnnotations.LibraryValidation.Attributes
{
    /// <summary>
    /// Base library validation attribute to inherit from.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class LibraryValidationBaseAttribute : ValidationAttribute
    {
        /// <summary>
        /// Ignores null-values.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Your type of class containing static validation methods decorated with <see cref="LibraryValidationMethodAttribute"/>.
        /// </summary>
        public Type ValidationLibraryClassType { get; set; }

        /// <summary>
        /// Your enum containing types of validation methods.
        /// </summary>
        public object ValidationTypeEnum { get; set; }

        /// <summary>
        /// Cache of what validation methods to invoke per enum value.
        /// </summary>
        private static Dictionary<Enum, List<MethodInfo>> _validationMethodsCache;
        private static readonly object _validationMethodsCacheLock = new object();

        /// <summary>
        /// Base library validation attribute to inherit from.
        /// </summary>
        /// <param name="validationLibraryClassType">Your type of class containing static validation methods decorated with <see cref="LibraryValidationMethodAttribute"/>.</param>
        /// <param name="validationTypeEnum">Your enum containing types of validation methods.</param>
        /// <param name="optional">If true, any null values will not be sent to validators.</param>
        protected LibraryValidationBaseAttribute(Type validationLibraryClassType, object validationTypeEnum, bool optional = false)
        {
            ValidationLibraryClassType = validationLibraryClassType;
            ValidationTypeEnum = validationTypeEnum;
            Optional = optional;
        }

        /// <summary>
        /// Invokes validation methods.
        /// </summary>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Abort if optional and no value was given.
            if (Optional && value == null)
            {
                return ValidationResult.Success;
            }

            // Ensure validation method cache
            lock (_validationMethodsCacheLock)
            {
                if (_validationMethodsCache == null)
                {
                    BuildValidationMethodCache();
                }
            }

            // Verify that we actually have have an enum value
            if (!(ValidationTypeEnum is Enum validationTypeEnum))
            {
                throw new LibraryValidationSetupException(
                    $"Property '{validationContext.MemberName}' on '{validationContext.ObjectType.Name}' has a wrong attribute parameter. " +
                    $"The '{nameof(ValidationTypeEnum)}' parameter must be set to an instance of an enum.");
            }

            // Verify that any validation method for this type exists
            if (!_validationMethodsCache.ContainsKey(validationTypeEnum))
            {
                throw new LibraryValidationSetupException(
                    $"Property '{validationContext.MemberName}' on '{validationContext.ObjectType.Name}' is using a validation of " +
                    $"type '{ValidationTypeEnum}' with no matching found validation method. " +
                    $"Decorate a method in '{ValidationLibraryClassType.FullName}' with the '{nameof(LibraryValidationMethodAttribute)}'-attribute " +
                    $"to create the validation method.");
            }

            var memberType = validationContext.ObjectInstance?.GetType()
                ?.GetMember(validationContext.MemberName)
                ?.Select(x => (x as FieldInfo)?.FieldType ?? (x as PropertyInfo)?.PropertyType)
                ?.FirstOrDefault();

            var matchingMethodData = new List<MethodWithParameters>();
            foreach (var method in _validationMethodsCache[validationTypeEnum])
            {
                // Build method parameters array
                var methodParameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                Type[] inputTypes = new Type[methodParameterTypes.Length];
                object[] parameters = new object[methodParameterTypes.Length];

                for (int i=0; i<methodParameterTypes.Length; i++)
                {
                    var parameterType = methodParameterTypes[i];
                    inputTypes[i] = parameterType;

                    if (parameterType == typeof(ValidationContext))
                    {
                        parameters[i] = validationContext;
                    }
                    else if (parameterType == typeof(Type))
                    {
                        parameters[i] = memberType;
                    }
                    else
                    {
                        parameters[i] = value;
                        inputTypes[i] = memberType;
                    }
                }

                // Validate
                var parameterTypesAreValid = true;
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterType = methodParameterTypes[i];
                    var parameter = parameters[i];
                    var inputType = inputTypes[i];

                    if (parameter != null && !parameterType.IsAssignableFrom(inputType))
                    {
                        parameterTypesAreValid = false;
                        break;
                    }
                }
                if (!parameterTypesAreValid)
                {
                    continue;
                }

                matchingMethodData.Add(new MethodWithParameters
                {
                    Method = method,
                    Parameters = parameters.ToArray()
                });
            }

            if (!matchingMethodData.Any())
            {
                throw new LibraryValidationSetupException(
                    $"Property '{validationContext.MemberName}' on '{validationContext.ObjectType.Name}' is using a validation of " +
                    $"type '{ValidationTypeEnum}' with no matching found validation method that takes a parameter " +
                    $"of type '{value?.GetType()?.Name ?? "<null>"}'. " +
                    $"Decorate a method in '{ValidationLibraryClassType.FullName}' with " +
                    $"the '{nameof(LibraryValidationMethodAttribute)}'-attribute to create the validation method.");
            }

            // Invoke all matching validation methods
            foreach (var methodData in matchingMethodData)
            {
                var method = methodData.Method;
                var parameters = methodData.Parameters;

                // If the method returns a string, its the errormessage
                var validationResult = method.Invoke(null, parameters) as string;
                if (validationResult != null)
                {
                    return new ValidationResult(validationResult, new[] { validationContext.MemberName });
                }
            }

            // All validations passed
            return ValidationResult.Success;
        }

        private class MethodWithParameters
        {
            public MethodInfo Method { get; set; }
            public object[] Parameters { get; set; }
        }

        /// <summary>
        /// Build a cache of what validation methods to invoke per enum value.
        /// <para>Also validates the validation methods themselves.</para>
        /// </summary>
        /// <exception cref="LibraryValidationSetupException"></exception>
        private void BuildValidationMethodCache()
        {
            // Find validation methods on library type
            var validationMethods = ValidationLibraryClassType.GetMethods()
                .Select(x => new {
                    Method = x,
                    Attributes = x.GetCustomAttributes<LibraryValidationMethodAttribute>()
                })
                .Where(x => x.Attributes.Any());

#pragma warning disable S2696 // Instance members should not write to "static" fields
            _validationMethodsCache = new Dictionary<Enum, List<MethodInfo>>();
#pragma warning restore S2696 // Instance members should not write to "static" fields

            foreach (var methodData in validationMethods)
            {
                foreach (var attribute in methodData.Attributes)
                {
                    // Verify that method is public static
                    if (!methodData.Method.IsStatic || !methodData.Method.IsPublic)
                    {
                        throw new LibraryValidationSetupException(
                            $"Method '{ValidationLibraryClassType.FullName}.{methodData.Method.Name}' must be public static.");
                    }
                    // Verify that the return type is string
                    else if (methodData.Method.ReturnType != typeof(string))
                    {
                        throw new LibraryValidationSetupException(
                            $"Method '{ValidationLibraryClassType.FullName}.{methodData.Method.Name}' must have a string return type.");
                    }

                    // Verify parameters
                    var hasValidationContextParameter = methodData.Method.GetParameters()
                        .Any(x => x.ParameterType == typeof(ValidationContext));
                    var hasTypeParameter = methodData.Method.GetParameters()
                        .Any(x => x.ParameterType == typeof(Type));
                    if (methodData.Method.GetParameters().Length == 0
                        || methodData.Method.GetParameters().Length > 3
                        || (methodData.Method.GetParameters().Length == 2 && !hasValidationContextParameter && !hasTypeParameter)
                        || (methodData.Method.GetParameters().Length == 3 && (!hasValidationContextParameter || !hasTypeParameter))
                    )
                    {
                        throw new LibraryValidationSetupException(
                            $"Method '{ValidationLibraryClassType.FullName}.{methodData.Method.Name}' must contain a parameter of a matching type for the value to be validated, and optionally ValidationContext and/or a Type for the value.");
                    }
                    else
                    {
                        // Verify that the attribute points to an enum value
                        if (!(attribute.ValidationTypeEnum is Enum validationTypeEnum))
                        {
                            throw new LibraryValidationSetupException(
                                $"Method '{ValidationLibraryClassType.FullName}.{methodData.Method.Name}' has a wrong attribute parameter. " +
                                $"The '{nameof(LibraryValidationMethodAttribute.ValidationTypeEnum)}' parameter must be set to an instance of an enum.");
                        }

                        // Add method to cache
                        if (!_validationMethodsCache.ContainsKey(validationTypeEnum))
                        {
                            _validationMethodsCache[validationTypeEnum] = new List<MethodInfo>();
                        }
                        _validationMethodsCache[validationTypeEnum].Add(methodData.Method);
                    }
                }
            }
        }
    }
}
