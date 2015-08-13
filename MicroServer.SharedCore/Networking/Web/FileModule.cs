using System;
using System.Globalization;
using System.IO;
using System.Text;

using Windows.Web.Http;

using MicroServer.Networking.Web.Modules;
using MicroServer.Networking.Web.Files;
using Windows.Web.Http.Headers;

namespace MicroServer.Networking.Web
{
    /// <summary>
    /// Will serve static files
    /// </summary>
    /// <example>
    /// <code>
    /// // One of the available file services.
    /// var diskFiles = new StorageService("/public/", @"C:\www\public\");
    /// var module = new FileModule(diskFiles);
    /// 
    /// // the module manager is added to the HttpServer.
    /// var moduleManager = new ModuleManager();
    /// moduleManager.Add(module);
    /// </code>
    /// </example>
    public class FileModule : IWorkerModule
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileModule" /> class.
        /// </summary>
        /// <param name="fileService">The file service.</param>
        public FileModule(IFileService fileService)
        {
            if (fileService == null) throw new ArgumentNullException("fileService");
            _fileService = fileService;
        }

        /// <summary>
        /// Gets or sets if we should allow file listing
        /// </summary>
        public bool AllowFileListing { get; set; }

        #region IWorkerModule Members

        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is exeucted in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.
        /// <para>If you are going to handle the request, implement <see cref="IWorkerModule"/> and do it in the <see cref="IWorkerModule.HandleRequest"/> method.</para>
        /// </remarks>
        public void BeginRequest(IHttpContext context)
        {
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
        }

        /// <summary>
        /// Handle the request asynchronously.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="callback">callback</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing except <see cref="IHttpModule.EndRequest"/>.</returns>
        /// <remarks>Invoked in turn for all modules unless you return <see cref="ModuleResult.Stop"/>.</remarks>
        public void HandleRequestAsync(IHttpContext context, Action<IAsyncModuleResult> callback)
        {
            // just invoke the callback synchronously.
            callback(new AsyncModuleResult(context, HandleRequest(context)));
        }

        /// <summary>
        /// Handle the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing except <see cref="IHttpModule.EndRequest"/>.</returns>
        /// <remarks>Invoked in turn for all modules unless you return <see cref="ModuleResult.Stop"/>.</remarks>
        public ModuleResult HandleRequest(IHttpContext context)
        {
            // only handle GET and HEAD
            if (context.Response.RequestMessage.Method != HttpMethod.Get
                && context.Response.RequestMessage.Method != HttpMethod.Head)
                return ModuleResult.Continue;

            var date = context.Response.RequestMessage.Headers.IfModifiedSince.GetValueOrDefault(DateTime.MinValue).UtcDateTime;

            FileContext fileContext = new FileContext(context.RouteUri, date);

            _fileService.GetFileAsync(fileContext);
            //var test = await _fileService.GetFile(fileContext);


            if (!fileContext.IsFound)
                return ModuleResult.Continue;

            if (!fileContext.IsModified)
            {
                context.Response.StatusCode = HttpStatusCode.NotModified;
                context.Response.ReasonPhrase = "Was last modified " + fileContext.LastModified.ToString("R");
                return ModuleResult.Stop;
            }

            var mimeType = MimeTypeProvider.Instance.Get(fileContext.File.Name);
            if (mimeType == null)
            {
                context.Response.StatusCode = HttpStatusCode.UnsupportedMediaType;
                context.Response.ReasonPhrase = string.Format("File type '{0}' is not supported.",
                                                                   Path.GetExtension(fileContext.File.Name));
                return ModuleResult.Stop;
            }
            context.Response.Content = new HttpStreamContent(fileContext.FileContent);
            context.Response.Content.Headers.Add("Last-Modified", fileContext.LastModified.ToString("R"));
            //context.Response.Headers.Add("Accept-Ranges", "bytes");
            context.Response.Content.Headers.Add("Content-Disposition", "inline;filename=\"" + Path.GetFileName(fileContext.File.Name) + "\"");
            context.Response.Content.Headers.ContentType = HttpMediaTypeHeaderValue.Parse(mimeType);
            //context.Response.Content.Headers.ContentLength = (ulong)fileContext.FileContent.Length;


            // do not include a body when the client only want's to get content information.
            if (context.Response.RequestMessage.Method == HttpMethod.Head && context.Response.Content != null)
            {
                context.Response.Content.Dispose();
                context.Response.Content = null;
            }
            return ModuleResult.Stop;
        }

        #endregion
    }
}