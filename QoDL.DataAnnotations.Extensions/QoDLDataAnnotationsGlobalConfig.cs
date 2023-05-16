using QoDL.DataAnnotations.Extensions.Models;
using System;
#if NETFRAMEWORK
using Newtonsoft.Json;
#endif

namespace QoDL.DataAnnotations.Extensions
{
    /// <summary>
    /// Global config for <c>QoDL.DataAnnotations</c>.
    /// </summary>
    public static class QoDLDataAnnotationsGlobalConfig
    {
        /// <summary>
        /// If false or null, any dev details will be discarded.
        /// </summary>
        public static Func<bool> EnableDeveloperDetails { get; set; }

        /// <summary>
        /// When developer details are given and <see cref="EnableDeveloperDetails"/> is enabled, this action will be executed.
        /// <para>By default the developer details are appended to the error message for unsuccessful results, and in flags for successful results.</para>
        /// </summary>
        public static Action<ModelValidatedResult, string> DeveloperDetailsAction { get; set; } = DefaultDeveloperDetailsAction;

        /// <summary>
        /// This callback is invoked after result model is created by any extension method.
        /// <para>String parameter is the devDetails if any.</para>
        /// <para>Returned value will be serialized to json and returned.</para>
        /// </summary>
        public static Func<ModelValidatedResult, string, ModelValidatedResult> ResultPostProcessAction { get; set; }

#if NETFRAMEWORK
        /// <summary>
        /// Default serializer settings used for Json results.
        /// </summary>
        public static JsonSerializerSettings DefaultSerializerSettings { get; set; }

#elif NETCORE
        /// <summary>
        /// Default serializer settings used for Json results.
        /// </summary>
        public static System.Text.Json.JsonSerializerOptions DefaultSerializerSettings { get; set; }
#endif

        /// <summary>
        /// This method is invoked after result model is created by any extension method.
        /// </summary>
        public static ModelValidatedResult PostProcessResult(ModelValidatedResult result, string devDetails)
        {
            ApplyDeveloperDetailsIfEnabled(result, devDetails);
            return ResultPostProcessAction?.Invoke(result, devDetails) ?? result;
        }

        /// <summary>
        /// Apply the given dev details if its enabled.
        /// </summary>
        public static void ApplyDeveloperDetailsIfEnabled(ModelValidatedResult result, string devDetails)
        {
            if (string.IsNullOrWhiteSpace(devDetails) || EnableDeveloperDetails?.Invoke() != true)
            {
                return;
            }

            DeveloperDetailsAction?.Invoke(result, devDetails);
        }

        private static void DefaultDeveloperDetailsAction(ModelValidatedResult result, string devDetails)
        {
            if (string.IsNullOrWhiteSpace(devDetails) || result == null)
            {
                return;
            }

            if (result.Success)
            {
                result.Flags.Add($"Developer details: {devDetails}");
            }
            else
            {
                result.Error += $"\n\n Developer details: {devDetails}";
            }
        }
    }
}
