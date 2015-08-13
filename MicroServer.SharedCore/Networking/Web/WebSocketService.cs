using System;

using MicroServer.Logging;

namespace MicroServer.Networking.Web
{
    class WebSocketService : IWebSocketService
    {
        private readonly ILogger _logger = LogManager.GetLogger<WebSocketService>();
        private readonly string _uriEndPoint;

        public WebSocketService(string uriEndPoint)
        {
            if (uriEndPoint == null) throw new ArgumentNullException("uriEndPoint");
            _uriEndPoint = uriEndPoint;

            OnOpen = () => { };
            OnClose = () => { };
            OnMessage = x => { };
            OnBinary = x => { };
            OnPing = x => { };
            OnPong = x => { };
            OnError = x => { };
        }

        public Action OnOpen { get; set; }

        public Action OnClose { get; set; }

        public Action<string> OnMessage { get; set; }

        public Action<byte[]> OnBinary { get; set; }

        public Action<byte[]> OnPing { get; set; }

        public Action<byte[]> OnPong { get; set; }

        public Action<Exception> OnError { get; set; }
   }
}
