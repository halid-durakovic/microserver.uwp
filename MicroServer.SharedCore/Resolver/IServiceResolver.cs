using System;

namespace MicroServer.Resolver
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceResolver
    {
        object Resolve(Type type);
    }
}