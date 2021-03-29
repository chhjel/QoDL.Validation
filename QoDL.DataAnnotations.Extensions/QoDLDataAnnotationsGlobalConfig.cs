using QoDL.DataAnnotations.Extensions.Models;
using System;

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

        internal static void ApplyDeveloperDetailsIfEnabled(ModelValidatedResult result, string devDetails)
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
