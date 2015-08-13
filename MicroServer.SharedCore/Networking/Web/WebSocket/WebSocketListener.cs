using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web.Http;

using MicroServer.Logging;
using MicroServer.Networking.Web.Modules;
using System.Threading;

namespace MicroServer.Networking.Web.WebSocket
{
    public sealed class WebSocketListener : IDisposable
    {

        #region Private Properties

        private readonly ILogger _logger = LogManager.GetLogger<WebSocketListener>();

        private NetworkAdapter _networkAdapter = null;
        private StreamSocketListener _listener;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private int _servicePort = 0;
        private bool _isActive = false;

        #endregion Private Properties

        #region Public Properties

        /// <summary>
        ///   Gets or sets the network adapter for receiving data
        /// </summary>
        public NetworkAdapter InterfaceAdapter
        {
            get { return _networkAdapter; }
            set { _networkAdapter = value; }
        }

        /// <summary>
        ///   Gets or sets the port for receiving data
        /// </summary>
        public int ServicePort
        {
            get { return _servicePort; }
            set { _servicePort = value; }
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

        #endregion Public Properties

        #region Constructors / Deconstructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebSocketListener" /> class.
        /// </summary>
        public WebSocketListener()
        {

        }

        /// <summary>
        /// Handles object cleanup for GC finalization.
        /// </summary>
        ~WebSocketListener()
        {
            Dispose(false);
        }

        /// <summary>
        /// Handles object cleanup.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handles object cleanup
        /// </summary>
        /// <param name="disposing">True if called from Dispose(); false if called from GC finalization.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (IsActive)
                    Stop();
            }

            if (_listener != null)
                _listener = null;
        }

        #endregion  Constructors / Deconstructors

        #region Methods

        /// <summary>
        ///  Starts the service listener if it is in a stopped state.
        /// </summary>
        /// <param name="servicePort">The port used to listen on.</param>
        public bool Start(string servicePort)
        {
            try
            {

                _listener = new StreamSocketListener();

#pragma warning disable CS4014
                if (InterfaceAdapter == null)
                {
                    _listener.BindServiceNameAsync(servicePort);
                }
                else
                {
                    _listener.BindServiceNameAsync(servicePort, SocketProtectionLevel.PlainSocket, InterfaceAdapter);
                }
#pragma warning restore CS4014

                _listener.ConnectionReceived += OnClientConnected;
                _logger.Info("Websocket listener Started on {0}", _listener.Information.LocalPort.ToString());

                //_logger.Info("Listener Started on {0}", ActivePort.ToString());

            }
            catch (Exception ex)
            {

                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    IsActive = false;
                    ListenerError(_listener, ex);
                    return false;
                }
                _logger.Error("Start listening failed with error: {0}" + ex.Message, ex);
            }

            IsActive = true;
            return true;
        }

        /// <summary>
        ///  Stops the service listener if in started state.
        /// </summary>
        public bool Stop()
        {
            try
            {
                if (!IsActive)
                    throw new InvalidOperationException("WebSocket listener is not active and must be started before stopping");

                IsActive = false;
                _logger.Info("WebSocket stopped listening for requests on {0}", _listener.Information.LocalPort);
                //_logger.Info("Stopped listening for requests on {0}", ActivePort.ToString());

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("WebSocket stopping listener Failed: {0}", ex.Message.ToString(), ex);
            }
            return false;
        }

        /// <summary>
        ///  Restarts the service listener if in a started state.
        /// <param name="servicePort">The port used to listen on.</param>
        /// </summary>
        public bool Restart(string servicePort)
        {
            Stop();
            return Start(servicePort);
        }

        /// <summary>
        ///     A client has connected (nothing have been sent or received yet)
        /// </summary>
        /// <returns></returns>
        private async void OnClientConnected(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {

            _logger.Debug("Connection from {0}:{1} to {2}:{3} was opened",
                args.Socket.Information.RemoteHostName.DisplayName,
                args.Socket.Information.RemotePort,
                args.Socket.Information.LocalAddress.DisplayName,
                args.Socket.Information.LocalPort);

            _logger.Trace("Pipeline => HttpServer.OnClientConnected");

            if (args == null || args.Socket == null)
                return;

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
                    //foreach (var item in httpMessage.Headers)
                    //{
                    //    _logger.Trace("Request Header => {0}:{1}", item.Key, item.Value);
                    //}
                    OnMessageReceived(connectedArgs.Socket);
                }

                //args.Socket.Dispose();
                //ClientDisconnected(sender, new ClientDisconnectedEventArgs(null, null));

                //_logger.Debug("Connection from {0}:{1} to {2}:{3} was closed",
                //    args.Socket.Information.RemoteHostName.DisplayName,
                //    args.Socket.Information.RemotePort,
                //    args.Socket.Information.LocalAddress.DisplayName,
                //    args.Socket.Information.LocalPort);

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
            _logger.Trace("Pipeline => WebSocketListener.OnClientDisconnected");

            ClientDisconnected(sender, new ClientDisconnectedEventArgs(exception, socketErrorStatus));
        }

        private async void OnMessageReceived(StreamSocket socket)
        {
            _logger.Trace("Pipeline => WebSocketListener.OnMessageReceived");

            try
            {
                HttpContextContent socketContext = new HttpContextContent(socket);
                HttpContext context = await socketContext.ReadAsHttpContextAsync();

            }
            catch (Exception ex)
            {
                _logger.Trace(ex.Message);
            }
        }

        private async void SendResponse(IAsyncModuleResult obj)
        {
            _logger.Trace("Pipeline => WebSocketListener.SendResponse");

            HttpContext context = (HttpContext)obj.Context;
            await SendResponseAsync(context.Socket, context.Response);

        }

        private async Task SendResponseAsync(StreamSocket socket, HttpResponseMessage response)
        {
            _logger.Trace("Pipeline => WebSocketListener.SendResponseAsync");

            try
            {
                if (response == null || response.Content == null )
                {
                    string errorPage = HtmlFactory.HttpResponse("utf-8", "Not Found", "Opps, page not found");
                    response.Content = new HttpStringContent(errorPage, UnicodeEncoding.Utf8, "text/html");
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = "Not Found";
                    response.Headers.Add("Connection", "close");
                }
     
                ulong contentLength = await response.Content.BufferAllAsync();
                if (contentLength == 0)
                    return;

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

                    //if (response.Header. != null)
                    //{
                    //    switch (response.RequestMessage)
                    //    {
                    //        case UnicodeEncoding.Utf16BE:
                    //            outputWriter.UnicodeEncoding = UnicodeEncoding.Utf16BE;
                    //            break;
                    //        case UnicodeEncoding.Utf16LE:
                    //            outputWriter.UnicodeEncoding = UnicodeEncoding.Utf16LE;
                    //            break;
                    //        default:
                    //            outputWriter.UnicodeEncoding = UnicodeEncoding.Utf8;
                    //            break;
                    //    }
                    //}
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
                }
            }
            catch (Exception ex)
            {
                _logger.Error("WebSocket request failed with error: {0}", ex.Message, ex);
            }
            finally
            {
                if (socket != null)
                socket.Dispose();

                _logger.Debug("WebSocket connection from {0}:{1} to {2}:{3} was closed",
                socket.Information.RemoteHostName.DisplayName,
                socket.Information.RemotePort,
                socket.Information.LocalAddress.DisplayName,
                socket.Information.LocalPort);
            }
        }

        #endregion Methods

        #region Events


        /// <summary>
        ///     A websocket client have connected (websocket handshake request is complete)
        /// </summary>
        public event EventHandler<WebSocketClientConnectEventArgs> WebSocketClientConnect = delegate { };

        /// <summary>
        ///     A websocket client have connected (websocket handshake response is complete)
        /// </summary>
        public event EventHandler<WebSocketClientConnectedEventArgs> WebSocketClientConnected = delegate { };

        /// <summary>
        ///     A websocket client have disconnected
        /// </summary>
        public event EventHandler<ClientDisconnectedEventArgs> WebSocketClientDisconnected = delegate { };



        /// <summary>
        ///     A client has connected (nothing have been sent or received yet)
        /// </summary>
        public event ClientConnectedEventHandler ClientConnected = delegate { };

        /// <summary>
        ///     A client has connected (nothing have been sent or received yet)
        /// </summary>
        public delegate void ClientConnectedEventHandler(StreamSocketListener sender, ClientConnectedEventArgs args);

        /// <summary>
        ///     A client has disconnected
        /// </summary>
        public event ClientDisconnectedEventHandler ClientDisconnected = delegate { };

        /// <summary>
        ///     A client has disconnected
        /// </summary>
        public delegate void ClientDisconnectedEventHandler(StreamSocketListener sender, ClientDisconnectedEventArgs args);

        /// <summary>
        ///     An internal error occured
        /// </summary>
        public event ErrorHandler ListenerError = delegate { };

        #endregion Events

    }
}