using System;

using Windows.Web.Http;
using Windows.Networking.Sockets;

namespace MicroServer.Networking.Web.WebSocket
{
    /// <summary>
    /// WebSocket Connect Event which includes the handshake request
    /// </summary>
    public class WebSocketClientConnectEventArgs : ClientConnectedEventArgs
    {
        /// <summary>
        /// Create a new instance of <see cref="WebSocketClientConnectEventArgs"/>
        /// </summary>
        /// <param name="args">Channel args that connected</param>
        /// <param name="request">Request that we received</param>
        public WebSocketClientConnectEventArgs(StreamSocketListenerConnectionReceivedEventArgs args, HttpRequestMessage request)
            : base(args)
        {
            if (request == null) throw new ArgumentNullException("args");
            Request = request;
        }

        /// <summary>
        /// WebSocket handshake request
        /// </summary>
        public HttpRequestMessage Request { get; private set; }

    }
}
