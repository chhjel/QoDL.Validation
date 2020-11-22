#if NETFULL
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using System.Web.Mvc;

namespace QoDL.DataAnnotations.Extensions.Models
{
    /// <summary>
    /// Serialized json result.
    /// </summary>
    internal class JsonContentResult : ActionResult
    {
        public object Data { get; set; }
        public int? StatusCode { get; set; }
        public JsonRequestBehavior JsonRequestBehavior { get; set; }
        public Encoding ContentEncoding { get; set; } = Encoding.UTF8;
        public string ContentType { get; set; } = "application/json";

        internal JsonContentResult(object data, JsonRequestBehavior jsonRequestBehavior, int? statusCode = null)
        {
            Data = data;
            JsonRequestBehavior = jsonRequestBehavior;
            StatusCode = statusCode;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet
                && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
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
