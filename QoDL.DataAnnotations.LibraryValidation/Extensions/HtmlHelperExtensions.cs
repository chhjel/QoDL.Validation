#if NETFULL
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QoDL.DataAnnotations.LibraryValidation.Models;
using QoDL.DataAnnotations.LibraryValidation.Utils;
using System.Web.Mvc;

namespace QoDL.DataAnnotations.LibraryValidation.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HtmlHelper"/>.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Get a definition of the validation on the current model.
        /// </summary>
        public static ModelLibraryValidationDefinition GetModelLibraryValidationDefinition(this HtmlHelper html)
        {
            var model = html?.ViewData?.Model;
            if (model == null)
            {
                return new ModelLibraryValidationDefinition();
            }

            return LibraryValidationDefinitionDefinitionUtils.GetModelLibraryValidationDefinition(model.GetType());
        }

        /// <summary>
        /// Get a definition of the validation on the current model.
        /// </summary>
        public static MvcHtmlString GetModelLibraryValidationDefinitionAsJson(this HtmlHelper html)
        {
            var model = html?.ViewData?.Model;
            ModelLibraryValidationDefinition definition;

            if (model == null)
            {
                definition = new ModelLibraryValidationDefinition();
            }
            else
            {
                definition = LibraryValidationDefinitionDefinitionUtils.GetModelLibraryValidationDefinition(model.GetType());
            }

            string json = SerializeJson(definition);
            return new MvcHtmlString(json);
        }

        private static string SerializeJson(object data)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var json = JsonConvert.SerializeObject(data, Formatting.None, jsonSerializerSettings);
            return json;
        }
    }
}
#endif
