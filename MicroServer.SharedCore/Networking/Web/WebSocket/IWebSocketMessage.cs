using System.IO;

namespace MicroServer.Networking.Web.WebSocket
{
    /// <summary>
    /// Interface for WebSocket messages
    /// </summary>
    public interface IWebSocketMessage
    {
        /// <summary>
        /// Type of message
        /// </summary>
        WebSocketOpcode Opcode { get; set; }

        /// <summary>
        /// Message payload
        /// </summary>
        Stream Payload { get; set; }
    }
}
