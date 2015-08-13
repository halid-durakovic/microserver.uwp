using System;
using System.Reflection;

namespace MicroServer.Resolver
{
    /// <summary>
    /// Use this get constructor info.
    /// </summary>
    public class FactoryResolver : IServiceResolver
    {
        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
