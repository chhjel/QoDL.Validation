#if NETFULL
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using System.Web.Mvc;

namespace QoDL.DataAnnotations.Extensions.Models
{
    /// <summary>
    /// Json result type used by QoDL namespace.
    /// </summary>
    public class QoDLJsonContentResult : ActionResult
    {
        /// <summary>
        /// Any custom data.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Status code if returned.
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary></summary>
        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        /// <summary></summary>
        public Encoding ContentEncoding { get; set; } = Encoding.UTF8;

        /// <summary></summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// Json result type used by QoDL namespace.
        /// </summary>
        public QoDLJsonContentResult(object data, JsonRequestBehavior jsonRequestBehavior, int? statusCode = null)
        {
            Data = data;
            JsonRequestBehavior = jsonRequestBehavior;
            StatusCode = statusCode;
        }

        /// <summary></summary>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet
                && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request.");
            }

            var response = context.HttpContext.Response;

            response.ContentType = ContentType;
            response.ContentEncoding = ContentEncoding;
            if (StatusCode != null)
            {
                response.StatusCode = StatusCode.Value;
            }

            if (Data == null)
            {
                response.Write("null");
            }

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            response.Write(JsonConvert.SerializeObject(Data, Formatting.Indented, jsonSerializerSettings));
        }
    }
}
#endif
