using System;
using Windows.Networking.Sockets;

namespace MicroServer.Networking.Web
{
    /// <summary>
    ///     Event arguments for <see cref="SocketListener.ClientDisconnected" />.
    /// </summary>
    public class ClientDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientDisconnectedEventArgs"/> class.
        /// </summary>
        /// <param name="socketErrorStatus">The socket error status that was caught.</param>
        /// <param name="exception">The exception that was caught.</param>
        public ClientDisconnectedEventArgs(Exception exception, SocketErrorStatus socketErrorStatus)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            Socket = null;
            SocketErrorStatus = SocketErrorStatus;
            Exception = exception;
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ClientDisconnectedEventArgs"/> class.
        ///// </summary>
        ///// <param name="socket">The channel that disconnected.</param>
        ///// <param name="exception">The exception that was caught.</param>
        //public ClientDisconnectedEventArgs(
        //    StreamSocketListenerConnectionReceivedEventArgs args,
        //    SocketErrorStatus socketErrorStatus,
        //    Exception exception)
        //{
        //    if (args == null) throw new ArgumentNullException("args");
        //    if (exception == null) throw new ArgumentNullException("exception");


        //    Socket = args.Socket;
        //    SocketErrorStatus = SocketErrorStatus;
        //    Exception = exception;
        //}

        /// <summary>
        /// Channel that was disconnected
        /// </summary>
        public StreamSocket Socket { get; private set; }

        /// <summary>
        /// Channel that was disconnected
        /// </summary>
        public SocketErrorStatus SocketErrorStatus { get; private set; }

        /// <summary>
        /// Exception that was caught (is SocketException if the connection failed or if the remote end point disconnected).
        /// </summary>
        /// <remarks>
        /// <c>SocketException</c> with status <c>Success</c> is created for graceful disconnects.
        /// </remarks>
        public Exception Exception { get; private set; }

    }
}