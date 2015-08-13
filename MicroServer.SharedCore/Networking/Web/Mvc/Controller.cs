using System;

using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Storage.Streams;

using MicroServer.Networking.Web.Mvc.Views;
using MicroServer.Networking.Web.Mvc.ActionResults;
using MicroServer.Networking.Web.Files;
using Windows.Data.Json;
using System.Collections.Generic;

namespace MicroServer.Networking.Web.Mvc
{
    /// <summary>
    /// MVC controller.
    /// </summary>
    public abstract class Controller : IController
    {
        private IControllerContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        protected Controller()
        {

        }

        /// <summary>
        /// Gets name of requested action.
        /// </summary>
        public string ActionName { get { return _context.ActionName; } }

        /// <summary>
        /// Gets controller uri
        /// </summary>
        /// <remarks>
        /// Can be "/controllerName/" or "/section/controllerName/" depending on the <see cref="ControllerUriAttribute"/>.
        /// </remarks>
        public string ControllerUri { get { return _context.ControllerUri; } }

        /// <summary>
        /// Gets or sets name of controller.
        /// </summary>
        /// <remarks>
        /// Can be "controllerName" or "section/controllerName" depending on the <see cref="ControllerUriAttribute"/>.
        /// </remarks>
        public string ControllerName { get { return _context.ControllerUri.TrimStart('/').TrimEnd('/'); } }

        /// <summary>
        /// Gets HTTP request
        /// </summary>
        public HttpRequestMessage Request { get { return _context.HttpContext.Response.RequestMessage; } }

        /// <summary>
        /// Gets HTTP response
        /// </summary>
        public HttpResponseMessage Response { get { return _context.HttpContext.Response; } }

        /// <summary>
        /// Gets request parameters
        /// </summary>
        /// <remarks>A merged collection of Uri and Form parameters</remarks>
        public IDictionary<string, object>  Properties
        {
            get { return _context.HttpContext.Response.RequestMessage.Properties; }
        }

        /// <summary>
        /// Gets form parameters
        /// </summary>
        /// <remarks>Form parameters</remarks>
        //public IParameterCollection Form
        //{
        //    get { return _context.HttpContext.Request.Form; }
        //}

        /// <summary>
        /// Invoke a method in another controller.
        /// </summary>
        /// <param name="controllerName">Name of controller.</param>
        /// <param name="action">Action to invoke</param>
        /// <param name="arguments">Parameters used by the controller.</param>
        /// <returns></returns>
        ////public static object Invoke(string controllerName, string actionName, params object[] arguments)
        //{
        //    return    Current.Invoke(controllerName, actionName, arguments);

        //}

        /// <summary>
        /// Gets or sets id
        /// </summary>
        public string Id
        {
            get
            {
                var prefixLength = _context.ControllerUri.Length + ActionName.Length + 1; //1= last slash

                if (_context.HttpContext.RouteUri.AbsolutePath.Length <= prefixLength)
                    return string.Empty;

                var myUri = _context.HttpContext.RouteUri.AbsolutePath.Substring(prefixLength);
                int pos = myUri.IndexOf("/");

                return pos == -1 ? myUri : myUri.Substring(0, pos);
            }
        }

        /// <summary>
        /// Sets controller context 
        /// </summary>
        /// <remarks>
        /// Context contains information about the current request.
        /// </remarks>
        public void SetContext(IControllerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// View data used when rendering a view.
        /// </summary>
        public IViewData ViewData
        {
            get { return _context.ViewData; }
            set { _context.ViewData = value; }
        }

        #region ActionResults Members

        //public virtual ViewResult View()
        //{
        //    return new ViewResult();
        //}

        public virtual ContentResult ContentResult(IHttpContent content, HttpMediaTypeHeaderValue contentType)
        {
            return new ContentResult(content, contentType);
        }

        public virtual ContentResult ContentResult(IBuffer buffer, HttpMediaTypeHeaderValue contentType)
        {
            return new ContentResult(buffer, contentType);
        }

        public virtual ContentResult ContentResult(string content, HttpMediaTypeHeaderValue contentType)
        {
            return new ContentResult(content, contentType);
        }

        public virtual ContentResult ContentResult(string content, string contentType)
        {
            return new ContentResult(content, contentType);
        }

        public virtual EmptyResult EmptyResult()
        {
            return new EmptyResult();
        }

        //public virtual FileResult FileResult(string rootFilePath, string fileNamePath)
        //{
        //    return new FileResult(rootFilePath, fileNamePath);
        //}

        //public virtual FileResult FileResult(string rootUri, string rootFilePath, string fileNamePath)
        //{
        //    return new FileResult(rootUri, rootFilePath, fileNamePath);
        //}

        //public virtual FileResult FileResult(IFileService fileService, string fullFilePath)
        //{
        //    return new FileResult(fileService, fullFilePath);
        //}

        public virtual JsonResult JsonResult(JsonObject jdom)
        {
            return new JsonResult(jdom);
        }

        public virtual JsonResult JsonResult(JsonObject jdom, HttpMediaTypeHeaderValue contentType)
        {
            return new JsonResult(jdom, contentType);
        }

        public virtual RedirectResult RedirectResult(string url)
        {
            return new RedirectResult(url);
        }

        public virtual RedirectResult RedirectResult(string controllerName, string actionName)
        {
            return new RedirectResult(controllerName, actionName);
        }

        public virtual ActionResult TriggerOnException(Exception ex)
        {
            return null;
        }

        #endregion ActionResults  Members

        #region  ControllerFactory  Members

        public virtual void OnActionExecuting(ActionExecutingContext context)
        {

        }
        public virtual void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public virtual void OnException(ExceptionContext context)
        {

        }

        #endregion  ControllerFactory  Members
    }
}