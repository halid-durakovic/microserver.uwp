using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web.Http;

using MicroServer.Logging;
using MicroServer.Networking.Web.Modules;
using MicroServer.Networking.Web.WebSocket;

namespace MicroServer.Networking.Web
{
    public sealed class HttpServer : IDisposable
    {

        #region Private Properties

        private static HttpServer _service;

        private readonly ILogger _logger = LogManager.GetLogger<HttpServer>();
        private readonly IModuleManager _moduleManager;

        private StreamSocketListener _listener;
        private int _servicePort = 0;
        private bool _isActive = false;
        private string _poweredBy = "MicroServer";

        #endregion Private Properties

        #region Public Properties

        /// <summary>
        ///   Gets or sets the port for receiving data
        /// </summary>
        public int ServicePort
        {
            get { return _servicePort; }
            set { _servicePort = value; }
        }

        /// <summary>
        ///   Gets or sets the "X-Powered-By" header
        /// </summary>
        public string PoweredBy
        {
            get { return _poweredBy; }
            set { _poweredBy = value; }
        }

        /// <summary>
        ///     Port that the server is listening on.
        /// </summary>
        /// <remarks>
        ///     You can use port <c>0</c> in <see cref="servicePort" /> to let the OS assign a port. This method will then give you the
        ///     assigned port.
        /// </remarks>
        public int ActivePort
        {
            get
            {
                if (_listener == null)
                    return -1;

                return (int.Parse(_listener.Information.LocalPort));
            }
        }

        /// <summary>
        ///   Gets or sets the socket listener active status.
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        /// <summary>
        ///     Gets current server.
        /// </summary>
        /// <remarks>
        /// Only valid when a request have been received and is being processed.
        /// </remarks>
        public static HttpServer Current
        {
            get { return _service; }
        }

        /// <summary>
        ///     You can fill this item with application specific information
        /// </summary>
        /// <remarks>
        ///     It will be supplied for every request in the <see cref="IHttpContext" />.
        /// </remarks>
        public KeyValuePair<string, object> ApplicationInfo { get; set; }

        #endregion Public Properties

        #region Constructors / Deconstructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpServer" /> class.
        /// </summary>
        public HttpServer(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
            _service = this;

            ApplicationInfo = new KeyValuePair<string, object>();

        }

        /// <summary>
        ///     Handles object cleanup
        /// </summary>
        public void Dispose()
        {

            if (IsActive)
                StopAsync();

            if (_listener != null)
                _listener = null;
        }

        #endregion  Constructors / Deconstructors

        #region Methods

        /// <summary>
        ///     Add a HTTP module
        /// </summary>
        /// <param name="module">Module to include</param>
        /// <remarks>Modules are executed in the order they are added.</remarks>
        public void Add(IHttpModule module)
        {
            _moduleManager.Add(module);
        }

        /// <summary>
        ///     Starts the service listener if it is in a stopped state.
        /// </summary>
        /// <param name="servicePort">The port used to listen on.</param>
        public async Task StartAsync(string servicePort)
        {
            try
            {
                _listener = new StreamSocketListener();

#pragma warning disable CS4014
                    await _listener.BindServiceNameAsync(servicePort);
#pragma warning restore CS4014

                _listener.ConnectionReceived += (sender, args) => OnClientConnected(sender, args);
                _logger.Info("Http listener Started on {0}", _listener.Information.LocalPort.ToString());

            }
            catch (Exception ex)
            {

                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    IsActive = false;
                    _logger.Error("Start listening failed with error: {0}" + ex.Message, ex);
                    ListenerError(_listener, ex);
                }

                ClientDisconnected(this, new ClientDisconnectedEventArgs(ex, SocketError.GetStatus(ex.HResult)));
            }

            IsActive = true;
        }

        /// <summary>
        ///     Stops the service listener if in started state.
        /// </summary>
        public async void StopAsync()
        {
            try
            {
                if (!IsActive)
                {
                    _logger.Info("Listener is not active and must be started before stopping");
                    return;
                }
                await _listener.CancelIOAsync();
                IsActive = false;
                _logger.Info("Stopped listening for requests on {0}", _listener.Information.LocalPort);

            }
            catch (Exception ex)
            {
                _logger.Error("Stopping listener Failed: {0}", ex.Message.ToString(), ex);
            }
        }

        /// <summary>
        ///     Restarts the service listener if in a started state.
        /// <param name="servicePort">The port used to listen on.</param>
        /// </summary>
        public async void RestartAsync(string servicePort)
        {
            StopAsync();
            await StartAsync(servicePort);
        }

        /// <summary>
        ///     A client has connected (nothing has been sent or received yet)
        /// </summary>
        /// <returns></returns>
        private async void OnClientConnected(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {

            if (args == null || args.Socket == null)
                return;

            _logger.Debug("Connection from {0}:{1} to {2}:{3} was opened",
                args.Socket.Information.RemoteHostName.DisplayName,
                args.Socket.Information.RemotePort,
                args.Socket.Information.LocalAddress.DisplayName,
                args.Socket.Information.LocalPort);

            _logger.Trace("Pipeline => HttpServer.OnClientConnected");

            ClientConnectedEventArgs connectedArgs = new ClientConnectedEventArgs(args);

            try
            {
                ClientConnected(sender, connectedArgs);

                if (connectedArgs.AllowConnect == false)
                {
                    if (connectedArgs.Response != null)
                    {
                        await connectedArgs.Response.FlushAsync();
                    }
                    _logger.Debug("Connection from {0}:{1} to {2}:{3} was denied access to connect",
                        args.Socket.Information.RemoteHostName.DisplayName,
                        args.Socket.Information.RemotePort,
                        args.Socket.Information.LocalAddress.DisplayName,
                        args.Socket.Information.LocalPort);
                }
                else
                {

                    OnMessageReceived(connectedArgs.Socket);
                }

            }
            catch (Exception ex)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    IsActive = false;
                    ListenerError(_listener, ex);

                    _logger.Error("Http request failed with error: {0}", ex.Message, ex);
                }
                else
                {
                    OnClientDisconnected(_listener, ex, SocketError.GetStatus(ex.HResult));
                }

                _logger.Debug("Connection from {0}:{1} to {2}:{3} was {4}",
                    args.Socket.Information.RemoteHostName.DisplayName,
                    args.Socket.Information.RemotePort,
                    args.Socket.Information.LocalAddress.DisplayName,
                    args.Socket.Information.LocalPort,
                    SocketError.GetStatus(ex.HResult).ToString());
            }
        }

        /// <summary>
        ///     A client has disconnected
        /// </summary>
        /// <param name="socket">Channel representing the client that disconnected</param>
        /// <param name="exception">
        ///     Exception which was used to detect disconnect (<c>SocketException</c> with status
        ///     <c>Success</c> is created for graceful disconnects)
        /// </param>
        private void OnClientDisconnected(StreamSocketListener sender, Exception exception, SocketErrorStatus socketErrorStatus)
        {
            _logger.Trace("Pipeline => HttpServer.OnClientDisconnected");

            ClientDisconnected(sender, new ClientDisconnectedEventArgs(exception, socketErrorStatus));
        }

        private async void OnMessageReceived(StreamSocket socket)
        {
            _logger.Trace("Pipeline => HttpServer.OnMessageReceived");
            _service = this;

            try
            {
                HttpContextContent socketContext = new HttpContextContent(socket);
                HttpContext context = await socketContext.ReadAsHttpContextAsync();

                context.Application = ApplicationInfo;
                context.Items = new KeyValuePair<string, object>();
                context.Response.Headers.Add("X-Powered-By", _poweredBy);

                _moduleManager.InvokeAsync(context, SendResponseAsync);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private async void SendResponseAsync(IAsyncModuleResult obj)
        {

            HttpContext context = (HttpContext)obj.Context;
            await SendResponseAsync(context.Socket, context.Response);

        }

        private async Task SendResponseAsync(StreamSocket socket, HttpResponseMessage response)
        {
            if (socket == null)
                return;

            _logger.Trace("Pipeline => HttpServer.SendResponseAsync");

            try
            {
                if (response == null)
                {
                    string errorPage = HtmlFactory.HttpResponse("utf-8", "Not Found", "Opps, page not found");
                    response.Content = new HttpStringContent(errorPage, UnicodeEncoding.Utf8, "text/html");
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = "Not Found";
                    response.Headers.Add("Connection", "close");
                }

                //if (response.Content == null)
                //    response.Content = new HttpStringContent(string.Empty);

                //ulong contentLength = await response.Content.BufferAllAsync();
                //if (contentLength == 0)
                //    return;

                using (DataWriter outputWriter = new DataWriter(socket.OutputStream))
                {
                    string htmlVersion;

                    switch (response.Version)
                    {
                        case HttpVersion.Http10:
                            htmlVersion = "HTTP/1.0";
                            break;
                        case HttpVersion.Http20:
                            htmlVersion = "HTTP/2.0";
                            break;
                        default:
                            htmlVersion = "HTTP/1.1";
                            break;
                    }

                    outputWriter.UnicodeEncoding = UnicodeEncoding.Utf8;

                    outputWriter.WriteString(string.Format("{0} {1} {2}\r\n", htmlVersion, (int)response.StatusCode, response.StatusCode));

                    foreach (var item in response.Headers)
                    {
                        outputWriter.WriteString(string.Format("{0}: {1}\r\n", item.Key, item.Value));
                    }

                    foreach (var item in response.Content.Headers)
                    {
                        outputWriter.WriteString(string.Format("{0}: {1}\r\n", item.Key, item.Value));
                    }

                    //string ret = "Set-Cookie: {0}={1}; Path={2}; Expires={3};\r\n";

                    outputWriter.WriteString("\r\n");

                    var bodyContent = await response.Content.ReadAsBufferAsync();
                    outputWriter.WriteBuffer(bodyContent);

                    await outputWriter.StoreAsync();
                    //await outputWriter.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Http request failed with error: {0}", ex.Message, ex);
            }
            finally
            {
                if (socket != null)
                    socket.Dispose();

                _logger.Debug("Connection from {0}:{1} to {2}:{3} was closed",
                socket.Information.RemoteHostName.DisplayName,
                socket.Information.RemotePort,
                socket.Information.LocalAddress.DisplayName,
                socket.Information.LocalPort);
            }
        }

        #endregion Methods

        #region Events


        /// <summary>
        ///     A client has connected (nothing have been sent or received yet)
        /// </summary>
        public event EventHandler<ClientConnectedEventArgs> ClientConnected = delegate { };

        /// <summary>
        ///     A client has disconnected
        /// </summary>
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected = delegate { };


        /// <summary>
        ///     A websocket client has connected (websocket handshake request is complete)
        /// </summary>
        public event EventHandler<WebSocketClientConnectEventArgs> WebSocketClientConnect = delegate { };

        /// <summary>
        ///     A websocket client has connected (websocket handshake response is complete)
        /// </summary>
        public event EventHandler<WebSocketClientConnectedEventArgs> WebSocketClientConnected = delegate { };

        /// <summary>
        ///     A websocket client has disconnected
        /// </summary>
        public event EventHandler<ClientDisconnectedEventArgs> WebSocketClientDisconnected = delegate { };

        /// <summary>
        ///     An internal error occured
        /// </summary>
        public event ErrorHandler ListenerError = delegate { };

        #endregion Events

    }
}