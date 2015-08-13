using System;
using Windows.Web.Http;
using MicroServer.Networking.Web.Mvc.ActionResults;

namespace MicroServer.Networking.Web.Mvc
{
    /// <summary>
    /// Base controller
    /// </summary>
    public abstract class ControllerBase : IController
    {
        private IControllerContext _context;

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
        /// Gets HTTP request
        /// </summary>
        public HttpRequestMessage Request { get { return _context.HttpContext.Response.RequestMessage; } }

        /// <summary>
        /// Gets HTTP response
        /// </summary>
        public HttpResponseMessage Response { get { return _context.HttpContext.Response; } }

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

        public virtual EmptyResult EmptyResult()
        {
            return new EmptyResult();
        }

        #region ActionResults  Members
        public virtual ActionResult TriggerOnException(Exception ex)
        {
            return null;
        }

        #endregion ActionResults  Members

        #region  ControllerFactory  Members

        public virtual void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
        }
        public virtual void OnActionExecuted(ActionExecutedContext filterContext)
        {
          
        }

        public virtual void OnException(ExceptionContext filterContext)
        {

        }

        #endregion  ControllerFactory  Members
    }
}
