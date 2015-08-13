using Windows.Web.Http;

using MicroServer.Logging;
using MicroServer.Networking.Web.Modules;

namespace MicroServer.Networking.Web
{
    /// <summary>
    /// Reports errors to different sources.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// var module = new ErrorModule();
    /// ]]>
    /// </code>
    /// </example>
    public sealed class ErrorModule : IHttpModule
    {

        private readonly ILogger _logger = LogManager.GetLogger<ControllerModule>();

        #region IHttpModule Members

        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is exeucted in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void BeginRequest(IHttpContext context)
        {
            _logger.Trace("Pipeline => ErrorModule.BeginRequest");
        }

        /// <summary>
        /// End request is typically used for post processing. The response should already contain everything required.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The last method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void EndRequest(IHttpContext context)
        {
            _logger.Trace("Pipeline => ErrorModule.EndRequest");

            if (context.LastException != null)
            {
                string errorPage = HtmlFactory.HttpResponse("utf-8", "Exeption Error", string.Format("Opps, fail with exception: {0}", context.LastException));

                //string errorPage = string.Format("<!DOCTYPE html><html><body>Opps, fail with exception: {0}</html></body>", context.LastException);
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Response.ReasonPhrase = "Exception Error";
                context.Response.Content = new HttpStringContent(errorPage, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/html");
            }
            if (context.Response.RequestMessage == null)
            {
                string errorPage = HtmlFactory.HttpResponse("utf-8", "Not Found", "Opps, page not found");
                context.Response.StatusCode = HttpStatusCode.NotFound;
                context.Response.ReasonPhrase = "Not Found";
                context.Response.Content = new HttpStringContent(errorPage, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/html");
            }
        }

        #endregion
        }
}