#if NETCORE
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;

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
        public bool AllowGet { get; set; }

        /// <summary></summary>
        public Encoding ContentEncoding { get; set; } = Encoding.UTF8;

        /// <summary></summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// Json result type used by QoDL namespace.
        /// </summary>
        public QoDLJsonContentResult(object data, bool allowGet, int? statusCode = null)
        {
            Data = data;
            AllowGet = allowGet;
            StatusCode = statusCode;
        }

        /// <summary></summary>
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (!AllowGet
                && string.Equals(context.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request.");
            }

            var response = context.HttpContext.Response;
            response.ContentType = ContentType;
            if (StatusCode != null)
            {
                response.StatusCode = StatusCode.Value;
            }

            var jsonResult = new JsonResult(Data, QoDLDataAnnotationsGlobalConfig.DefaultSerializerSettings)
            {
                StatusCode = StatusCode,
                ContentType = ContentType
            };

            await jsonResult.ExecuteResultAsync(context);
        }
    }
}
#endif
