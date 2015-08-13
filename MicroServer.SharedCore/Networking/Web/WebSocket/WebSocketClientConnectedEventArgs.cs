using System;
using Windows.Networking.Sockets;
using Windows.Web.Http;

namespace MicroServer.Networking.Web.WebSocket
{
    /// <summary>
    /// WebSocket Connected Event which includes the handshake request and response
    /// </summary>
    public class WebSocketClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new isntance of <see cref="WebSocketClientConnectedEventArgs"/>
        /// </summary>
        /// <param name="socket">Socket used for transfers</param>
        /// <param name="request">Request (should contain the upgrade request)</param>
        /// <param name="response">Response (should include the upgrade confirmation)</param>
        public WebSocketClientConnectedEventArgs(StreamSocket socket, HttpRequestMessage request, HttpResponseMessage response)
        {
            Socket = socket;
            Request = request;
            Response = response;
        }

        /// <summary>
        ///     Gets socket for the connected client end point.
        /// </summary>
        public StreamSocket Socket { get; private set; }

        /// <summary>
        /// WebSocket handshake request
        /// </summary>
        public HttpRequestMessage Request { get; private set; }

        /// <summary>
        /// WebSocket handshake response
        /// </summary>
        public HttpResponseMessage Response { get; private set; }

    }
}
