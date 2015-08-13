using System;

using Windows.Web.Http;

using MicroServer.Logging;
using MicroServer.Networking.Web.Modules;
using MicroServer.Networking.Web.WebSocket;
using System.Collections.Generic;

namespace MicroServer.Networking.Web
{
    /// <summary>
    /// Will serve static files
    /// </summary>
    /// <example>
    /// <code>
    /// // One of the available file services.
    /// var diskFiles = new StorageService("/public/", @"C:\www\public\");
    /// var module = new FileModule(diskFiles);
    /// 
    /// // the module manager is added to the HttpServer.
    /// var moduleManager = new ModuleManager();
    /// moduleManager.Add(module);
    /// </code>
    /// </example>
    public class WebSocketModule : IWorkerModule
    {
        private readonly ILogger _logger = LogManager.GetLogger<WebSocketModule>();
        private readonly string _uriEndPoint;
        private readonly string _port;

        public WebSocketModule(string uriEndPoint, string port)
        {
            if (uriEndPoint == null) throw new ArgumentNullException("uriEndPoint");
            if (port == null) throw new ArgumentNullException("port");
            _uriEndPoint = uriEndPoint;
            _port = port;
        }

        #region IWorkerModule Members

            /// <summary>
            /// Invoked before anything else
            /// </summary>
            /// <param name="context">HTTP context</param>
            /// <remarks>
            /// <para>The first method that is exeucted in the pipeline.</para>
            /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.
            /// <para>If you are going to handle the request, implement <see cref="IWorkerModule"/> and do it in the <see cref="IWorkerModule.HandleRequest"/> method.</para>
            /// </remarks>
        public void BeginRequest(IHttpContext context)
        {
            _logger.Trace("Pipeline => WebSocketModule.BeginRequest");
        }

        /// <summary>
        /// Handle the request asynchronously.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="callback">callback</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing except <see cref="IHttpModule.EndRequest"/>.</returns>
        /// <remarks>Invoked in turn for all modules unless you return <see cref="ModuleResult.Stop"/>.</remarks>
        public void HandleRequestAsync(IHttpContext context, Action<IAsyncModuleResult> callback)
        {
            // just invoke the callback synchronously.
            callback(new AsyncModuleResult(context, HandleRequest(context)));
        }

        /// <summary>
        /// Handle the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing except <see cref="IHttpModule.EndRequest"/>.</returns>
        /// <remarks>Invoked in turn for all modules unless you return <see cref="ModuleResult.Stop"/>.</remarks>
        public ModuleResult HandleRequest(IHttpContext context)
        {
            if (context.RouteUri == null)
            {
                context.Response.StatusCode = HttpStatusCode.BadRequest;
                context.Response.ReasonPhrase = "Bad Request";
                return ModuleResult.Stop;
            }

            // only handle GET requests
            if (context.Response.RequestMessage.Method != HttpMethod.Get)
                return ModuleResult.Continue;

            if (context.RouteUri.AbsolutePath == _uriEndPoint)
            {
                HttpRequestMessage httpMessage = context.Response.RequestMessage;

                if (httpMessage.Headers.Contains(new KeyValuePair<string, string>("Upgrade", "Websocket")) &&
                    httpMessage.Headers.Contains(new KeyValuePair<string, string>("Connection", "Upgrade")))
                {

                    string webSocketVersion;
                    httpMessage.Headers.TryGetValue("Sec-WebSocket-Version", out webSocketVersion);
                    if (webSocketVersion != "13")
                    {
                        context.Response.StatusCode = HttpStatusCode.BadRequest;
                        context.Response.ReasonPhrase = "Invalid Websocket Version";
                        return ModuleResult.Stop;
                    }

                    string webSocketKey;
                    if (httpMessage.Headers.TryGetValue("Sec-WebSocket-Key", out webSocketKey))
                    {
                        context.Response.StatusCode = HttpStatusCode.SwitchingProtocols;
                        context.Response.ReasonPhrase = "WebSocket Protocol Handshake";
                        context.Response.Headers.Add("Upgrade", "Websocket");
                        context.Response.Headers.Add("Sec-WebSocket-Accept", WebSocketUtils.HashWebSocketKey(webSocketKey));
                        context.Response.Headers.Add("Sec-WebSocket-Location", "ws://localhost:8181/websession");

                        string webSocketOrigin;
                        httpMessage.Headers.TryGetValue("Sec-WebSocket-Origin", out webSocketOrigin);
                        if (webSocketOrigin != null)
                            context.Response.Headers.Add("Sec-WebSocket-Origin", webSocketOrigin);

                        context.Response.Content = new HttpStringContent("Switching Protocols");



                        //IWebSocketMessage webSocketMessage = msg as IWebSocketMessage;
                        //if (webSocketMessage != null)
                        //{
                        //    // standard message responses handled by listener
                        //    switch (webSocketMessage.Opcode)
                        //    {
                        //        case WebSocketOpcode.Ping:
                        //            source.Send(new WebSocketMessage(WebSocketOpcode.Pong, webSocketMessage.Payload));
                        //            return;
                        //        case WebSocketOpcode.Close:
                        //            source.Send(new WebSocketMessage(WebSocketOpcode.Close));
                        //            source.Close();

                        //            WebSocketClientDisconnected(this,
                        //                new ClientDisconnectedEventArgs(source, new Exception("WebSocket closed")));
                        //            return;
                        //    }

                        //    _webSocketMessageReceived(source, webSocketMessage);
                        //    return;























                            return ModuleResult.Stop;
                    }
                    else
                    {
                        context.Response.StatusCode = HttpStatusCode.BadRequest;
                        context.Response.ReasonPhrase = "Bad Request";
                        return ModuleResult.Stop;
                    }
                }
            }
            return ModuleResult.Continue;
        }

        /// <summary>
        /// End request is typically used for post processing. The response should already contain everything required.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The last method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void EndRequest(IHttpContext context)
        {
             _logger.Trace("Pipeline => WebSocketModule.EndRequest");
        }
        #endregion
    }
}