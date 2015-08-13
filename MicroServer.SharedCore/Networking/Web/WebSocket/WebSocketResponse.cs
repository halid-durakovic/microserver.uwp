namespace MicroServer.Networking.Web.WebSocket
{
    /// <summary>
    /// WebSocket response
    /// </summary>
    public class WebSocketResponse : WebSocketMessage
    {
        internal WebSocketResponse(WebSocketFrame frame)
            : base(frame.Opcode, frame.Payload)
        {
        }
    }
}
