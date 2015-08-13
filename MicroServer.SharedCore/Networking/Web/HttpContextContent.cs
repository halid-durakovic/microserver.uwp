using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace MicroServer.Networking.Web
{
    public sealed class HttpContextContent
    {
        private HttpContext _context;

        public HttpContextContent(StreamSocket socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException("socket");
            }

            _context = new HttpContext();
            _context.Socket = socket;
            _context.SocketID = Guid.NewGuid();

        }

        public async Task<HttpContext> ReadAsHttpContextAsync()
        {
            try
            {
                await PrepareHttpContextAsync();
            }
            catch
            {

            }

            return _context;
        }

        private async Task PrepareHttpContextAsync2()
        {

            HttpRequestMessage requestMessage = new HttpRequestMessage();

            HttpStreamContent content = new HttpStreamContent(_context.Socket.InputStream);
            await content.BufferAllAsync();

            _context.Response.RequestMessage = requestMessage;

        }



        private async Task PrepareHttpContextAsync()
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            //IHttpContent requestContent;

            //  TODO:  Find better way to handle this read
            //string request = await ReadRequest(_context.Socket);

            HttpStreamContent content = new HttpStreamContent(_context.Socket.InputStream);
            await content.BufferAllAsync();
            string request = await content.ReadAsStringAsync();

            //string request;
            //ulong contentLength;
            //using (IInputStream input = _context.Socket.InputStream)
            //{
            //    HttpStreamContent content = new HttpStreamContent(input);
            //    request = await content.ReadAsStringAsync();
            //}

            if (string.IsNullOrWhiteSpace(request))
            {
                return;
            }

            string[] requestParts = request.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //set method
            var requestLine = requestParts[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (requestLine.Length > 0 ? requestLine[0] : null)
            {
                case "OPTIONS":
                    requestMessage.Method = HttpMethod.Options;
                    break;
                case "GET":
                    requestMessage.Method = HttpMethod.Get;
                    break;
                case "HEAD":
                    requestMessage.Method = HttpMethod.Head;
                    break;
                case "POST":
                    requestMessage.Method = HttpMethod.Post;
                    break;
                case "PUT":
                    requestMessage.Method = HttpMethod.Put;
                    break;
                case "DELETE":
                    requestMessage.Method = HttpMethod.Delete;
                    break;
                case "PATCH":
                    requestMessage.Method = HttpMethod.Patch;
                    break;
                default: break;
            }

            //set URL
            var uri = new Uri(requestLine[1], UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri($"http://{_context.Socket.Information.LocalAddress}:{_context.Socket.Information.LocalPort}"), uri);

            if (uri.IsWellFormedOriginalString())
            {
                requestMessage.RequestUri = uri;
                _context.RouteUri = uri;
            }
            else
            {
                return;
            }

            // set version
            switch (requestLine[2])
            {
                case "HTTP/1.0":
                    _context.Response.Version = HttpVersion.Http10;
                    break;
                case "HTTP/1.1":
                    _context.Response.Version = HttpVersion.Http11;
                    break;
                case "HTTP/2.0":
                    _context.Response.Version = HttpVersion.Http20;
                    break;
                default:
                    _context.Response.Version = HttpVersion.None;
                    break;
            }

            // parse headers
            for (int i = 1; i < requestParts.Count(); i++)
            {
                var line = requestParts[i];
                if (string.IsNullOrWhiteSpace(line))
                    break;

                var header = line.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var headerName = header.Length > 0 ? header[0].Trim() : null;
                var headerValue = header.Length > 1 ? header[1].Trim() : null;

                if (headerName != null && headerValue != null)
                {
                    //Debug.WriteLine("{0}:{1}", headerName, headerValue);

                    switch (headerName.Trim().ToUpper())
                    {
                        //case "HOST":
                        //    requestMessage.Headers.Host = new HostName(headerName.TrimStart('/').TrimEnd(':'));
                        //    break;

                        //case "CONTENT-TYPE":
                        //    HttpMediaTypeHeaderValue contentType;
                        //    if (HttpMediaTypeHeaderValue.TryParse(headerValue, out contentType))
                        //        requestContent.Headers.ContentType = contentType;

                        //    break;

                        //case "CONTENT-LENGTH":
                        //    if (ulong.TryParse(headerValue, out contentLength))
                        //        requestContent.Headers.ContentLength = contentLength;
                        //    break;

                        //case "CONTENT-RANGE":
                        //    HttpContentRangeHeaderValue contentRange;
                        //    if (HttpContentRangeHeaderValue.TryParse(headerValue, out contentRange))
                        //        requestContent.Headers.ContentRange = contentRange;
                        //    break;


                        //case "CONTENT-DISPOSITION":
                        //    HttpContentDispositionHeaderValue contentDisposition;
                        //    if (HttpContentDispositionHeaderValue.TryParse(headerValue, out contentDisposition))
                        //        requestContent.Headers.ContentDisposition = contentDisposition;
                        //    break;

                        //case "CONTENT-LOCATION":
                        //    Uri contentLocation;
                        //    if (Uri.TryCreate(headerValue, UriKind.RelativeOrAbsolute, out contentLocation))
                        //        requestMessage.Content.Headers.ContentLocation = contentLocation;
                        //    break;

                        //case "CONTENT-MD5":
                        //    //IBuffer contentMD5 = headerValue;
                        //    //requestMessage.Content.Headers.ContentMD5 = contentMD5;
                        //    break;

                        //case "CONTENT-LASTMODIFIED":
                        //    DateTimeOffset lastModified;
                        //    if (DateTimeOffset.TryParse(headerValue, out lastModified))
                        //        requestContent.Headers.LastModified = lastModified;
                        //    break;

                        //case "CONTENT-EXPIRES":
                        //    DateTimeOffset expires;
                        //    if (DateTimeOffset.TryParse(headerValue, out expires))
                        //        requestContent.Headers.Expires = expires;
                        //    break;

                        case "COOKIE":
                            HttpCookiePairHeaderValue cookie;
                            if (HttpCookiePairHeaderValue.TryParse(headerValue, out cookie))
                                requestMessage.Headers.Cookie.Add(cookie);
                            break;

                        default:
                            requestMessage.Headers.Add(headerName, headerValue);
                            break;
                    }
                }

                requestMessage.Content = new HttpStringContent(requestParts[requestParts.Count() - 1].ToString());
                await requestMessage.Content.BufferAllAsync();

            }
            _context.Response.RequestMessage = requestMessage;
        }

        private static async Task<string> ReadRequest(StreamSocket socket)
        {
            uint BufferSize = 8192;

            var httpStreamContent = new HttpStreamContent(socket.InputStream);

            var stringContent = await httpStreamContent.ReadAsInputStreamAsync();

            var request = new StringBuilder();
            using (var input = stringContent)
            {
                var data = new byte[BufferSize];
                var buffer = data.AsBuffer();
                var dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }
            return request.ToString();
        }
    }
}
