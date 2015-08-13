using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using MicroServer.Logging;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using System.Diagnostics;

using MicroServer.Reflection;

namespace MicroServer.Networking.Web.Mvc.Controllers
{
    public class ControllerIndexer
    {
        private readonly ILogger _logger = LogManager.GetLogger<RouterModule>();

        private List<ControllerMapping> _controllers = new List<ControllerMapping>();

        public IEnumerable Controllers
        {
            get { return _controllers; }
        }


        public void Find()
        {
            //List<Assembly> assemblies = new List<Assembly>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //TODO:  Fix static listing
            //assemblies.Add(Assembly.Load(new AssemblyName("MicroServer.SharedCore")));
            //assemblies.Add(Assembly.Load(new AssemblyName("MicroServer.Web")));

            Type controllerType = typeof(Controller);

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.ExportedTypes)
                {
                    if (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface || type.GetTypeInfo().IsNotPublic)
                        continue;

                    if (type.GetTypeInfo().IsSubclassOf(typeof(Controller)) || 
                            type.GetTypeInfo().IsSubclassOf(typeof(WebSocketController)))
                        MapController(type);
                }
            }
        }

        private void MapController(Type type)
        {
            ControllerMapping mapping = new ControllerMapping(type);
            mapping.Uri = type.Name.Replace("Controller", string.Empty);

            foreach (MethodInfo method in type.GetRuntimeMethods())
            {
                if (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsNotPublic)
                    continue;

                if (method.ReturnType.Equals(typeof(IActionResult)))
                    mapping.Add(method);
            }
            _controllers.Add(mapping);
        }
    }
}
