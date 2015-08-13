using System;

namespace MicroServer.Networking.Web
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceResolver
    {
        object Resolve(Type type);
    }
}