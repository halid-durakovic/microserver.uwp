using Windows.Web.Http;

namespace MicroServer.Networking.Web.WebSocket
{
    /// <summary>
    /// WebSocket request includes the initial handshake request
    /// </summary>
    public class WebSocketRequest : WebSocketMessage
    {

        private readonly HttpRequestMessage _handshake;
        private readonly WebSocketFrame _frame;

        internal WebSocketRequest(HttpRequestMessage handshake, WebSocketFrame frame)
            : base(frame.Opcode, frame.Payload)
        {
            _handshake = handshake;
            _frame = frame;
        }

        /// <summary>
        /// Cookies of the handshake request
        /// </summary>
        public HttpRequestMessage Handshake
        {
            get
            {
                return _handshake;
            }
        }

    }
}
