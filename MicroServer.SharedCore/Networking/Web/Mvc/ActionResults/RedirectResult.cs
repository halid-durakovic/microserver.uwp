using System;

using Windows.Web.Http;
using Windows.Storage.Streams;

namespace MicroServer.Networking.Web.Mvc.ActionResults
{
    /// <summary>
    /// Redirect to another url or controller/action.
    /// </summary>
    public class RedirectResult : ActionResult
    {
        private string _url;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectResult"/> class.
        /// </summary>
        /// <param name="url">Uri to redirect to.</param>
        /// <remarks>
        /// Include "http(s)://" in Uri to redirect to another site.
        /// </remarks>
        public RedirectResult(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("url");
            }

            _url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectResult"/> class.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        public RedirectResult(string controllerName, string actionName)
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("controllerName");
            }

            if (string.IsNullOrEmpty(actionName))
            {
                throw new ArgumentException("actionName");
            }

            _url = "/" + controllerName + "/" + actionName;
        }

        /// <summary>
        /// Execute the response result.
        /// </summary>
        /// <param name="context">HTTP controller context</param>
        /// <remarks>Invoked by <see cref="ControllerFactory"/> to process the response.</remarks>
        public override void ExecuteResult(IControllerContext context)
        {
            if (string.IsNullOrEmpty(_url))
            {
                string body = @"<!DOCTYPE html><html><head><META http-equiv='refresh' content='0;URL=" + _url + @"'</head><body></body></html>";
                context.HttpContext.Response.Content = new HttpStringContent(body, UnicodeEncoding.Utf8);
            }
        }
    }
}