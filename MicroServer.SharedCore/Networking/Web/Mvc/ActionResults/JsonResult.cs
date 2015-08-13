using System;
using System.IO;

using Windows.Data.Json;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Storage.Streams;

namespace MicroServer.Networking.Web.Mvc.ActionResults
{
    /// <summary>
    /// Send JSON formated content to the client. 
    /// </summary>
    public class JsonResult : ActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResult"/> class.
        /// </summary>
        /// <param name="jdom">The JSON object body content.</param>
        public JsonResult(JsonObject jdom)
        {
            Content = jdom;
            ContentType = HttpMediaTypeHeaderValue.Parse("application/json");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResult"/> class.
        /// </summary>
        /// <param name="jdom">The JSON object body content.</param>
        /// <param name="contentType">The content type header.</param>
        public JsonResult(JsonObject jdom, HttpMediaTypeHeaderValue contentType)
        {
            Content = jdom;
            ContentType = contentType;
        }

        /// <summary>
        /// Gets content type.
        /// </summary>
        public HttpMediaTypeHeaderValue ContentType { get; set; }

        /// <summary>
        /// Gets JSON data object.
        /// </summary>
        public JsonObject Content { get; set; }

        /// <summary>
        /// Execute the response result.
        /// </summary>
        /// <param name="context">HTTP controller context</param>
        /// <remarks>Invoked by <see cref="ControllerFactory"/> to process the response.</remarks>
        public override void ExecuteResult(IControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (ContentType != null && Content != null)
            {
                context.HttpContext.Response.Content = 
                    new HttpStringContent(Content.Stringify(), UnicodeEncoding.Utf8, ContentType.MediaType);
            }
        }
    }
}
