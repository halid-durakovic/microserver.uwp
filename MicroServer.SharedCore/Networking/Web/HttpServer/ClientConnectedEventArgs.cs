using System;

using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace MicroServer.Networking.Web
{
    /// <summary>
    ///     Used by <see cref="SocketListener.OnClientConnected" />.
    /// </summary>
    public class ClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientConnectedEventArgs" /> class.
        /// </summary>
        /// <param name="arg">The channel args.</param>
        public ClientConnectedEventArgs(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            if (args == null) throw new ArgumentNullException("args");
            
            AllowConnect = true;
            Socket = args.Socket;
       
        }

        /// <summary>
        ///     Gets socket for the connected end point.
        /// </summary>
        public StreamSocket Socket { get; private set; }

        /// <summary>
        ///     Response (only if the client may not connect)
        /// </summary>
        public IOutputStream Response { get; set; }

        /// <summary>
        ///     Determines if the client may connect.
        /// </summary>
        public bool AllowConnect { get; set; }

        /// <summary>
        ///     Cancel connection, will make the listener close it.
        /// </summary>
        public void CancelConnection()
        {
            AllowConnect = false;
        }

        /// <summary>
        ///     Close the listener, but send a response (you are yourself responsible of encoding it to a message)
        /// </summary>
        /// <param name="response">Stream with encoded message (which can be sent as-is).</param>
        public void CancelConnection(IOutputStream response)
        {
            if (response == null) throw new ArgumentNullException("response");
                AllowConnect = false;
        }
    }
}