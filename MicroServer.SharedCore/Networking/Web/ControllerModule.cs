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
    public class ControllerModule : IWorkerModule
    {

        private readonly ILogger _logger = LogManager.GetLogger<ControllerModule>();
        private readonly ControllerFactory _controllerFactory = new ControllerFactory();

        public ControllerModule()
        {
            _controllerFactory.Load();

            _logger.Info("Controller Url(s) Loaded:");
            foreach (KeyValuePair<string, ControllerMapping> controller in _controllerFactory.Controllers)
            {
                _logger.Info("  route url: /" + controller.Key.ToString());
            }
        }

        /// <summary>
        /// Key is complete uri to action
        /// </summary>
        private List<string> _controllers = new List<string>();

        public List<string> Controllers
        {
            get { return _controllers; }
        }

        #region IWorkerModule Members

        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.
        /// <para>If you are going to handle the request, implement <see cref="IWorkerModule"/> and do it in the <see cref="IWorkerModule.HandleRequest"/> method.</para>
        /// </remarks>
        public void BeginRequest(IHttpContext context)
        {
            _logger.Trace("Pipeline => ControllerModule.BeginRequest");
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
            _logger.Trace("Pipeline => ControllerModule.HandleRequestAsync");
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
            _logger.Trace("Pipeline => ControllerModule.HandleRequest");

            ControllerMapping mapping;

            if (context.RouteUri == null)
            {
                context.Response.StatusCode = Windows.Web.Http.HttpStatusCode.BadRequest;
                context.Response.ReasonPhrase = "Bad Request";
                return ModuleResult.Stop;
            }

            string uri = context.RouteUri.AbsolutePath.TrimStart('/').TrimEnd('/').ToLower();

            if (_controllerFactory.TryMapping(uri, out mapping))
            {
                string actionName = uri.Substring(uri.LastIndexOf('/') + 1);

                MethodInfo action = mapping.FindAction(actionName);
                ActionResult result = (ActionResult)_controllerFactory.Invoke(mapping.ControllerType, action, context);
                return ModuleResult.Continue;
            }

            context.Response.StatusCode = Windows.Web.Http.HttpStatusCode.NotFound;
            context.Response.ReasonPhrase = "Page not found";
            return ModuleResult.Stop;
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
            _logger.Trace("Pipeline => ControllerModule.EndRequest");
        }

        #endregion IWorkerModule Members
    }
}
