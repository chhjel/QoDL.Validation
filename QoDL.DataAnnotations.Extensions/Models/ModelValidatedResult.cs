using QoDL.DataAnnotations.Extensions.Extensions;
using System.Collections.Generic;

namespace QoDL.DataAnnotations.Extensions.Models
{
    /// <summary>
    /// Type that is serialized in <see cref="ModelStateDictionaryExtensions.CreateJsonResult"/>
    /// and <see cref="ModelStateDictionaryExtensions.CreateJsonResultSuccess"/> extension methods.
    /// </summary>
    public class ModelValidatedResult
    {
        /// <summary>
        /// Everything went as expected?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Optional error that isn't bound to any model property.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Errors per property name.
        /// </summary>
        public Dictionary<string, string> ModelErrors { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Any custom flags.
        /// </summary>
        public HashSet<string> Flags { get; set; } = new HashSet<string>();
    }

    /// <summary>
    /// Type that is serialized in <see cref="ModelStateDictionaryExtensions.CreateJsonResult"/>
    /// and <see cref="ModelStateDictionaryExtensions.CreateJsonResultSuccess"/> extension methods.
    /// </summary>
    public class ModelValidatedResult<TData> : ModelValidatedResult
    {
        /// <summary>
        /// Any extra data.
        /// </summary>
        public TData Data { get; set; }
    }
}
