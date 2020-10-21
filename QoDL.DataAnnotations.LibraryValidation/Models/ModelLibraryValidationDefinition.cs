using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QoDL.DataAnnotations.LibraryValidation.Models
{
    /// <summary>
    /// Definition model for defined library validation.
    /// </summary>
    [Serializable]
    public class ModelLibraryValidationDefinition : Dictionary<string, List<PropertyLibraryValidationDefinition>>
    {
        /// <summary>Definition model for defined library validation.</summary>
        public ModelLibraryValidationDefinition() { }

        /// <summary>Definition model for defined library validation.</summary>
        public ModelLibraryValidationDefinition(int capacity)
            : base(capacity) { }

        /// <summary>Definition model for defined library validation.</summary>
        public ModelLibraryValidationDefinition(IEqualityComparer<string> comparer)
            : base(comparer) { }

        /// <summary>Definition model for defined library validation.</summary>
        public ModelLibraryValidationDefinition(IDictionary<string, List<PropertyLibraryValidationDefinition>> dictionary)
            : base(dictionary) { }

        /// <summary>Definition model for defined library validation.</summary>
        public ModelLibraryValidationDefinition(int capacity, IEqualityComparer<string> comparer)
            : base(capacity, comparer) { }

        /// <summary>Definition model for defined library validation.</summary>
        public ModelLibraryValidationDefinition(IDictionary<string, List<PropertyLibraryValidationDefinition>> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer) { }

        /// <summary>Definition model for defined library validation.</summary>
        protected ModelLibraryValidationDefinition(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Definition model for defined library validation.
    /// </summary>
    public class PropertyLibraryValidationDefinition
    {
        /// <summary>
        /// Enum validation type.
        /// </summary>
        public object Type { get; set; }

        /// <summary>
        /// If optional, the validation method will not be executed if value is empty.
        /// </summary>
        public bool Optional { get; set; }
    }
}
