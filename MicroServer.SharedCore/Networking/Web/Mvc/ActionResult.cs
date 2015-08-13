using System;

namespace MicroServer.Networking.Web.Mvc
{
    public abstract class ActionResult : IActionResult
    {
        public abstract void ExecuteResult(IControllerContext context);
    }
}
