using System;

using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Storage.Streams;


namespace MicroServer.Networking.Web.Mvc.ActionResults
{ 
    /// <summary>
    /// Send binary content to the client. 
    /// </summary>
    public class ContentResult : ActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResult"/> class.
        /// </summary>
        /// <param name="content">The body content.</param>
        /// <param name="contentType">The content type header.</param>
        public ContentResult(IHttpContent content, HttpMediaTypeHeaderValue contentType)
        {
            if (content == null)
            {
                throw new NullReferenceException("content");
            }

            if (contentType == null)
            {
                throw new ArgumentException("contentType");
            }

            Content = content;
            ContentType = contentType;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResult"/> class.
        /// </summary>
        /// <param name="buffer">The body content.</param>
        /// <param name="contentType">The content type header.</param>
        public ContentResult(IBuffer buffer, HttpMediaTypeHeaderValue contentType)
        {
            if (buffer.Length == 0)
            {
                throw new ArgumentOutOfRangeException("buffer");
            }

            if (contentType == null)
            {
                throw new ArgumentException("contentType");
            }
            
            Content = new HttpBufferContent(buffer);
            ContentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResult"/> class.
        /// </summary>
        /// <param name="content">The body content.</param>
        /// <param name="contentType">The content type header.</param>
        public ContentResult(string content, HttpMediaTypeHeaderValue contentType)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("content");
            }

            if (contentType == null)
            {
                throw new ArgumentException("contentType");
            }
            
            Content = new  HttpStringContent(content, UnicodeEncoding.Utf8, ContentType.MediaType);
            ContentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResult"/> class.
        /// </summary>
        /// <param name="content">The body content.</param>
        /// <param name="contentType">The content type header.</param>
        public ContentResult(string content, string contentType)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("content");
            }

            if (contentType == null)
            {
                throw new ArgumentException("contentType");
            }

            Content = new HttpStringContent(content, UnicodeEncoding.Utf8, contentType);
            ContentType = HttpMediaTypeHeaderValue.Parse(contentType);
        }

        /// <summary>
        /// Gets stream to send
        /// </summary>
        public IHttpContent Content { get; private set; }

        /// <summary>
        /// Gets or sets content type.
        /// </summary>
        public HttpMediaTypeHeaderValue ContentType { get; set; }

        /// <summary>
        /// Execute the response result.
        /// </summary>
        /// <param name="context">HTTP controller context</param>
        /// <remarks>Invoked by <see cref="ControllerFactory"/> to process the response.</remarks>
        public override void ExecuteResult(IControllerContext context)
        {    
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (ContentType != null && Content != null)
            {
                context.HttpContext.Response.Content = Content;
            }
        }
    }
}