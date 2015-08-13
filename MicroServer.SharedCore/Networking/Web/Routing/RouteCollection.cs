using System.Collections.Generic;
using System.Text.RegularExpressions;

using MicroServer.Logging;


namespace MicroServer.Networking.Web.Routing
{
    /// <summary>
    /// Collection of route rules
    /// </summary>
    public class RouteCollection 
    {
        private readonly ILogger _logger = LogManager.GetLogger<RouteCollection>();
        private readonly List<string> _ingoredRoutes = new List<string>();
        private readonly List<MappedRoute> _mappedRoutes = new List<MappedRoute>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteCollection"/> class.
        /// </summary>
        public RouteCollection()
        {

        }

        /// <summary>
        /// Gets the ignored routes.
        /// </summary>
        public List<string> IngoredRoutes
        {
            get { return _ingoredRoutes; }
        }


        /// <summary>
        /// Gets the mapped routes.
        /// </summary>
        public List<MappedRoute> MappedRoutes
        {
            get { return _mappedRoutes; }
        }

        /// <summary>
        /// Add ignored routes.
        /// </summary>
        public void IgnoreRoute(string regex)
        {
            _ingoredRoutes.Add(regex);
        }

        /// <summary>
        /// Add mapped routes.
        /// </summary>
        public void MapRoute(string name, string regex, DefaultRoute defaults)
        {
            MappedRoute mappedRoute = new MappedRoute();
            mappedRoute.name = name;
            mappedRoute.regex = regex;
            mappedRoute.defaults = defaults;

            _mappedRoutes.Add(mappedRoute);
        }

        /// <summary>
        /// Match the route and apply the context
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The match results for ignored and mapped routes (<c>null</c> if not existing)</returns>
        public MatchResult Match(IHttpContext context)
        {
            Regex regex;
            Match match;

            _logger.Info("Match result for ignored and mapped routes:");

            // Process ignored routes
            foreach (string pattern in _ingoredRoutes)
            {
                regex = new Regex(pattern, RegexOptions.IgnoreCase);

                //match = regex.Match(context.Request.Uri.AbsolutePath);
                match = regex.Match(context.Response.RequestMessage.RequestUri.AbsolutePath);
                if (match.Success)
                {
                    //_logger.Info("  route pattern:'" + pattern +
                    //    "' regex:'" + pattern +
                    //    "' => uri:'" + context.Response.RequestMessage.RequestUri.AbsolutePath +
                    //    "' = true");

                    _logger.Info("  route pattern:'{0}' regex:'{1}' => uri:'{2}' = true",
                        pattern,
                        pattern,
                        context.Response.RequestMessage.RequestUri.AbsolutePath);

                    return null;
                }
                else
                {
                    //_logger.Info("  route pattern:'" + pattern +
                    //    "' regex:'" + pattern +
                    //    "' => uri:'" + context.Response.RequestMessage.RequestUri.AbsolutePath +
                    //    "' = false");

                    _logger.Info("  route pattern:'{0}' regex:'{1}' => uri:'{2}' = false",
                        pattern,
                        pattern,
                        context.Response.RequestMessage.RequestUri.AbsolutePath);
                }
            }

            // Process mapped routes
            MappedRoute route;

            foreach (MappedRoute item in _mappedRoutes)
            {
                route = (MappedRoute)item;
                regex = new Regex(route.regex);

                match = regex.Match(context.Response.RequestMessage.RequestUri.AbsolutePath);
                if (match.Success)
                {
                    //_logger.Info("  route name:'" + route.name +
                    //        "' regex:'" + route.regex +
                    //        "' => uri:'" + context.Response.RequestMessage.RequestUri.AbsolutePath +
                    //        "' = true");

                    _logger.Info("  route name:'{0}' regex:'{1}' => uri:'{2}' = true",
                       route.name,
                       route.regex,
                       context.Response.RequestMessage.RequestUri.AbsolutePath);

                    return new MatchResult(match, route);
                }
                else
                {
                    //_logger.Info("  route name:'" + route.name +
                    //        "' regex:'" + route.regex +
                    //        "' => uri:'" + context.Response.RequestMessage.RequestUri.AbsolutePath +
                    //        "' = false");

                    _logger.Info("  route name:'{0}' regex:'{1}' => uri:'{2}' = false",
                       route.name,
                       route.regex,
                       context.Response.RequestMessage.RequestUri.AbsolutePath);
                }
            }
            return null;
        }
    }
}
