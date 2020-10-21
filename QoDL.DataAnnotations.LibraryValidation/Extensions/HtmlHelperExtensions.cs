#if NETFULL
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QoDL.DataAnnotations.LibraryValidation.Models;
using QoDL.DataAnnotations.LibraryValidation.Utils;
using System.Collections.Generic;
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

        /// <summary>
        /// Get a dictionary of all errors in the model.
        /// </summary>
        public static Dictionary<string, string> GetModelErrorsDictionary(this HtmlHelper html)
        {
            return html.ViewData.ModelState.GetModelErrorsAsDictionary();
        }

        /// <summary>
        /// Get a dictionary of all errors in the model as json.
        /// </summary>
        public static MvcHtmlString GetModelErrorsDictionaryAsJson(this HtmlHelper html)
        {
            var data = html.ViewData.ModelState.GetModelErrorsAsDictionary() ?? new Dictionary<string, string>();
            string json = SerializeJson(data);
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
