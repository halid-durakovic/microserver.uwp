using System;

namespace MicroServer.Networking.Web
{
    interface IWebSocketService
    {
        Action<byte[]> OnBinary { get; set; }
        Action OnClose { get; set; }
        Action<Exception> OnError { get; set; }
        Action<string> OnMessage { get; set; }
        Action OnOpen { get; set; }
        Action<byte[]> OnPing { get; set; }
        Action<byte[]> OnPong { get; set; }
    }
}