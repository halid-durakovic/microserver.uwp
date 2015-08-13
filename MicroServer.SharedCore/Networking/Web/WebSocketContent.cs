using System;
using System.Threading.Tasks;

using MicroServer.Networking.Web.WebSocket;
using Windows.Web.Http;


namespace MicroServer.Networking.Web
{
    public class WebSocketContent
    {
        private IWebSocketMessage _message;
        private IHttpContent _content;

        public WebSocketContent(IWebSocketMessage message, WebSocketOpcode opcode)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message == null) throw new ArgumentNullException("opcode");

            _message = new WebSocketMessage(opcode);
        }

        public async Task<IHttpContent> ReadAsWebSocketMessageAsync()
        {
            try
            {
                await PrepareHttpContextAsync();
            }
            catch
            {

            }

            return _content;
        }

        private async Task PrepareHttpContextAsync()
        {


        }
    }
}
