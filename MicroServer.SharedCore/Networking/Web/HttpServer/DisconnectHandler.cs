using System;

using Windows.Networking.Sockets;

namespace MicroServer.Networking.Web
{
    /// <summary>
    /// <see cref="SocketChannel"/> was disconnected
    /// </summary>
    /// <param name="sender">Channel which got disconnected</param>
    /// <param name="exception">Exception  (<c>SocketException</c> for TCP/IP errors)</param>
    /// <seealso cref="SocketChannel"/>
    public delegate void DisconnectHandler(StreamSocketListener sender, Exception exception);
}