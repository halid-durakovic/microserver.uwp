using System;
using System.Collections.Generic;
using System.Security.Principal;
using Windows.Networking.Sockets;
using Windows.Web.Http;

namespace MicroServer.Networking.Web
{
    /// <summary>
    /// Request context
    /// </summary>
    public sealed class HttpContext : IHttpContext
    {
        private readonly LinkedList<Action<IHttpContext>> _callbacks = new LinkedList<Action<IHttpContext>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContext" /> class.
        /// </summary>
        public HttpContext()
        {
            Response = new HttpResponseMessage();
            Items = new KeyValuePair<string, object>();
            Application = new KeyValuePair<string, object>();
            Session = new KeyValuePair<string, object>();
        }

        #region IHttpContext Members


        /// <summary>
        /// Channel Id connnected
        /// </summary>
        public Guid SocketID { get; set; }

        /// <summary>
        /// Channel connected
        /// </summary>
        public StreamSocket Socket { get; set; }

        /// <summary>
        /// Response to send back
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Gets information stored for the route.
        /// </summary>
        /// <remarks>For instance used to convert the URI into parameters.</remarks>
        /// <seealso cref="RouterModule"/>
        public Uri RouteUri { get; set; }

        /// <summary>
        /// Can be used to store items through this request
        /// </summary>
        public KeyValuePair<string, object> Items { get; set; }

        /// <summary>
        /// Used to store items for the entire application.
        /// </summary>
        public KeyValuePair<string, object> Application { get; set; }

        /// <summary>
        /// USed to store items for the current session (if a session has been started)
        /// </summary>
        /// <remarks>Will be null if a session has not been started.</remarks>
        public KeyValuePair<string, object> Session { get; set; }

        /// <summary>
        /// All exceptions will be logged by the system, but we generally do only keep track of the last one.
        /// </summary>
        public Exception LastException { get; set; }

        /// <summary>
        /// Gets or sets currently logged in user.
        /// </summary>
        public IPrincipal User { get; set; }

        /// <summary>
        /// Register a callback for the request disposal (i.e. the reply have been sent back and everything is cleaned up)
        /// </summary>
        /// <param name="callback">Callback to invoke</param>
        public void RegisterForDisposal(Action<IHttpContext> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            _callbacks.AddLast(callback);  
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var callback in _callbacks)
            {
                callback(this);
            }
            Socket.Dispose();
            Response.Dispose();
            _callbacks.Clear();
        }

        #endregion
    }
}