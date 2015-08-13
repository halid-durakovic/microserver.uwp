using System;

namespace MicroServer.Networking.Web.Mvc
{
    public interface IController
    {
        void OnActionExecuting(ActionExecutingContext context);
        void OnActionExecuted(ActionExecutedContext context);
        void OnException(ExceptionContext context);
        void SetContext(IControllerContext context);
        ActionResult TriggerOnException(Exception ex);
    }
}
