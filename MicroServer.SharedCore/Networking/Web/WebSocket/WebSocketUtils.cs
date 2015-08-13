using System;
using Windows.Web.Http;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MicroServer.Networking.Web.WebSocket
{
    internal class WebSocketUtils
    {
        private const string _guid = "4E59686F-1505-4053-94F3-4B1217B53C7D";

        /// <summary>
        /// Hashes the client WebSocket key for the server
        /// </summary>
        /// <param name="webSocketKey"></param>
        /// <returns></returns>
        public static string HashWebSocketKey(string webSocketKey)
        {
            if (webSocketKey == null) throw new ArgumentNullException("webSocketKey");

            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(webSocketKey + _guid, BinaryStringEncoding.Utf8);

            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);

            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            String strHashBase64 = CryptographicBuffer.EncodeToBase64String(buffHash);

            return strHashBase64;
        }

        /// <summary>
        /// Creates a new random WebSocket key for the client
        /// </summary>
        /// <returns></returns>
        public static string CreateWebSocketKey()
        {
            var key = new byte[16];
            new Random().NextBytes(key);
            return Convert.ToBase64String(key);
        }

        /// <summary>
        /// Check if http message is a valid WebSocket upgrade request
        /// </summary>
        /// <param name="httpRequestMessage">message to check</param>
        /// <returns>true if message is a valid WebSocket upgrade request</returns>
        public static bool IsWebSocketUpgrade(HttpRequestMessage httpRequestMessage)
        {
            return httpRequestMessage != null &&
                (httpRequestMessage.Headers["Connection"] ?? string.Empty).IndexOf("upgrade", StringComparison.OrdinalIgnoreCase) != -1 &&
                (httpRequestMessage.Headers["Upgrade"] ?? string.Empty).IndexOf("websocket", StringComparison.OrdinalIgnoreCase) != -1;
        }

        /// <summary>
        /// Creates a new radom masking key
        /// </summary>
        /// <returns>masking key</returns>
        public static byte[] CreateMaskingKey()
        {
            var key = new byte[4];
            new Random().NextBytes(key);
            return key;
        }

        /// <summary>
        /// Helper function to convert a byte array to a short using big endian
        /// </summary>
        /// <param name="value">byte array</param>
        /// <returns></returns>
        public static ushort ToBigEndianUInt16(byte[] value)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(value);
            return BitConverter.ToUInt16(value, 0);
        }

        /// <summary>
        /// Helper function to convert a byte array to a long using big endian
        /// </summary>
        /// <param name="value">byte array</param>
        /// <returns></returns>
        public static ulong ToBigEndianUInt64(byte[] value)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(value);
            return BitConverter.ToUInt64(value, 0);
        }

        /// <summary>
        /// Helper function to convert a short to a byte array using big endian
        /// </summary>
        /// <param name="value">short</param>
        /// <returns></returns>
        public static byte[] GetBigEndianBytes(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Helper function to convert a long to a byte array using big endian
        /// </summary>
        /// <param name="value">long</param>
        /// <returns></returns>
        public static byte[] GetBigEndianBytes(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

    }
}
