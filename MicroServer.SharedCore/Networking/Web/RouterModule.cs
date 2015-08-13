using System;
using System.Reflection;
using System.Collections.Generic;

using Windows.Web.Http;

using MicroServer.Logging;
using MicroServer.Resolver;
using MicroServer.Reflection;
using MicroServer.Networking.Web.Routing;
using MicroServer.Networking.Web.Modules;


namespace MicroServer.Networking.Web
{
    /// <summary>
    /// Used to route the request based on <see cref="Regex"/> patterns.
    /// </summary>
    /// <remarks>Rewrites the request URI based on the <see cref="RouteConfig"/>.</remarks>
    public class RouterModule : IRoutingModule
    {
        private readonly ILogger _logger = LogManager.GetLogger<RouterModule>();
        private readonly RouteCollection _routes = new RouteCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="RouterModule"/> class.
        /// </summary>
        public RouterModule()
        {
            Invoke(_routes);
            LogRoutes();
        }

        /// <summary>
        /// Log the loaded routes.
        /// </summary>
        private void LogRoutes()
        {
            _logger.Info("Ignored routes loaded from RouteConfig (HttpApplication) class:");
            foreach (string pattern in _routes.IngoredRoutes)
            {
                _logger.Info("  route regex: '{0}'", pattern);
            }

            _logger.Info("Mapped routes loaded from RouteConfig (HttpApplication) class:");
            foreach (MappedRoute items in _routes.MappedRoutes)
            {
                MappedRoute item = items;
                DefaultRoute route = item.defaults;

                _logger.Info("  route name:'{0}' regex:'{1}' => default: controller='{2}' action='{3}' id='{4}'",
                    item.name, item.regex, route.controller, route.action, route.id);
            }
        }

        /// <summary>
        /// Invoke the <see cref="RegisterRoutes"/> method in <see cref="RouteConfig"/>.
        /// </summary>
        /// <param name="routes">Routes.</param>
        private void Invoke(RouteCollection routes)
        {
            Type configType = typeof(HttpApplication);

            Type routeType = Find(configType);

            if (routeType != null)
            {
                var routeConfig = (HttpApplication)ServiceResolver.Current.Resolve(routeType);

                if (routes != null)
                    routeConfig.RegisterRoutes(routes);
            }
        }

        /// <summary>
        /// Find the type in the current domain assemblies.
        /// </summary>
        /// <param name="configType">Routes</param>
        private Type Find(Type configType)
        {
            //List<Assembly> assemblies = new List<Assembly>();
            //assemblies.Add(Assembly.Load(new AssemblyName("MicroServer.SharedCore")));
            //assemblies.Add(Assembly.Load(new AssemblyName("MicroServer.Web")));

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.ExportedTypes)
                {
                    //_logger.Trace("Assembly => {0}", type.ToString());
                    if (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface)
                        continue;

                    if (type.GetTypeInfo().IsSubclassOf(configType))
                        return type;
                }
            }

            return null;
        }

        #region IRoutingModule Members

        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void BeginRequest(IHttpContext context)
        {
            _logger.Trace("Pipeline => RouteModule.BeginRequest");
        }

        /// <summary>
        /// Route the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing including <see cref="IHttpModule.EndRequest"/>.</returns>
        /// <remarks>Simply change the request URI to something else.</remarks>
        public ModuleResult Route(IHttpContext context)
        {
            _logger.Trace("Pipeline => RouteModule.BeginRequest");

            MatchResult results = _routes.Match(context);

            if (results != null)
            {
                if (results.MatchStatus.Success)
                {
                    DefaultRoute defaultRoute = results.MappedRoute.defaults;

                    string path = String.Empty;

                    if (!string.IsNullOrEmpty(defaultRoute.controller))
                        path += string.Concat("/", defaultRoute.controller);

                    if (!string.IsNullOrEmpty(defaultRoute.action))
                        path += string.Concat("/", defaultRoute.action);

                    if (!string.IsNullOrEmpty(defaultRoute.id))
                        path += string.Concat("?id=", defaultRoute.id);

                    if (string.IsNullOrEmpty(path))
                        path = context.Response.RequestMessage.RequestUri.AbsolutePath;

                    Uri url = context.Response.RequestMessage.RequestUri;
                    string port = string.Empty;

                    if (url.Port != 80)
                        port = string.Concat(";", url.Port);

                    context.RouteUri = new Uri(
                        string.Concat(url.Scheme, "://", url.Host, port, path));

                }
                return ModuleResult.Continue;
            }

            context.Response.StatusCode = HttpStatusCode.Forbidden;
            context.Response.ReasonPhrase = "Forbidden";
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
            _logger.Trace("Pipeline => RouteModule.EndRequest");
        }

        #endregion
    }
}