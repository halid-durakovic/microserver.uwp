using System;
using Windows.Web.Http;

using MicroServer.Networking.Web.Mvc;

namespace MicroServer.Net.Http.Mvc.ActionResults
{
    public class HttpStatusCodeResult : ActionResult
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }

        public HttpStatusCodeResult(HttpStatusCode statusCode)
            : this(statusCode, string.Empty)
        {
        }

        public HttpStatusCodeResult(HttpStatusCode statusCode, string reasonPhrase)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
        }

        public override void ExecuteResult(IControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.HttpContext.Response.StatusCode = StatusCode;
            if (ReasonPhrase != string.Empty)
            {
                context.HttpContext.Response.ReasonPhrase = ReasonPhrase;
            }
        }
    }
}