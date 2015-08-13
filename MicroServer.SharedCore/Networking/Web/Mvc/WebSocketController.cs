using System;
using Windows.Web.Http;
using System.Collections.Generic;
using MicroServer.Networking.Web.WebSocket;

namespace MicroServer.Networking.Web.Mvc
{
    /// <summary>
    /// WebSocket controller
    /// </summary>
    public class WebSocketController : ControllerBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketController"/> class.
        /// </summary>
        protected WebSocketController()
        {

        }

        public virtual void OnClose()
        {
            throw new NotImplementedException();
        }

        public virtual void OnError()
        {
            throw new NotImplementedException();
        }

        public virtual void OnMessage(string message)
        {
            throw new NotImplementedException();
        }

        public virtual void OnMessage(byte[] message)
        {
            throw new NotImplementedException();
        }

        public virtual void OnOpen()
        {
            throw new NotImplementedException();
        }

        public virtual void OnPing()
        {
            throw new NotImplementedException();
        }

        public virtual void OnPong()
        {
            throw new NotImplementedException();
        }

        #region  ControllerFactory  Members

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //TODO: fix this hack
            filterContext.HttpContext.Response.Content = new HttpStringContent(" ");

            if (filterContext.HttpContext.RouteUri == null)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Bad Request");
                return;
            }

            if (this.Request.Headers.Contains(new KeyValuePair<string, string>("Upgrade", "Websocket")) &&
                this.Request.Headers.Contains(new KeyValuePair<string, string>("Connection", "Upgrade")))
            {

                string webSocketVersion;
                this.Request.Headers.TryGetValue("Sec-WebSocket-Version", out webSocketVersion);
                if (webSocketVersion != "13")
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid Websocket Version");
                    return;
                }

                string webSocketKey;
                if (this.Request.Headers.TryGetValue("Sec-WebSocket-Key", out webSocketKey))
                {

                    filterContext.HttpContext.Response.Headers.Add("Upgrade", "Websocket");
                    filterContext.HttpContext.Response.Headers.Add("Sec-WebSocket-Accept", WebSocketUtils.HashWebSocketKey(webSocketKey));
                    filterContext.HttpContext.Response.Headers.Add("Sec-WebSocket-Location", "ws://localhost:8181/websession");

                    string webSocketOrigin;
                    this.Request.Headers.TryGetValue("Sec-WebSocket-Origin", out webSocketOrigin);
                    if (webSocketOrigin != null)
                        filterContext.HttpContext.Response.Headers.Add("Sec-WebSocket-Origin", webSocketOrigin);

                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.SwitchingProtocols, "WebSocket Protocol Handshake");

                    base.OnActionExecuting(filterContext);
                }

            }

        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public override void OnException(ExceptionContext filterContext)
        {

        }

        #endregion  ControllerFactory  Members
    }
}
