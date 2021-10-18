#if NETCORE
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace QoDL.DataAnnotations.Extensions.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HtmlHelper"/>.
    /// </summary>
    public static class HtmlHelperExtensions
    {
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
        public static HtmlString GetModelErrorsDictionaryAsJson(this HtmlHelper html)
        {
            var data = html.ViewData.ModelState.GetModelErrorsAsDictionary() ?? new Dictionary<string, string>();
            string json = SerializeJson(data);
            return new HtmlString(json);
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
