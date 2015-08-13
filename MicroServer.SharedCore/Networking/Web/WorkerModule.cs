using System;
using System.Collections;
using System.Reflection;

using MicroServer.Logging;
using MicroServer.Networking.Web.Modules;
using MicroServer.Networking.Web.Mvc.Controllers;
using MicroServer.Networking.Web.Mvc;
using System.Collections.Generic;

namespace MicroServer.Networking.Web
{
    public class WorkerModule : IWorkerModule
    {
        private readonly ILogger _logger = LogManager.GetLogger<WorkerModule>();

        public WorkerModule()
        {

        }

        #region IWorkerModule Members

        /// <summary>
        /// Invoked before anything else.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.
        /// <para>If you are going to handle the request, implement <see cref="IWorkerModule"/> and do it in the <see cref="IWorkerModule.HandleRequest"/> method.</para>
        /// </remarks>
        public void BeginRequest(IHttpContext context)
        {
            _logger.Trace("Pipeline => WorkerModule.BeginRequest");
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
            _logger.Trace("Pipeline => WorkerModule.HandleRequestAsync");
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
            _logger.Trace("Pipeline => WorkerModule.HandleRequest");
            return ModuleResult.Continue;
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
            _logger.Trace("Pipeline => WorkerModule.EndRequest");
        }

        #endregion IWorkerModule Members
    }
}
