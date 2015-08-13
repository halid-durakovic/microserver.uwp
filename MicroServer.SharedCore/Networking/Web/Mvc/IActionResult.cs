
namespace MicroServer.Networking.Web.Mvc
{
    public interface IActionResult
    {
        void ExecuteResult(IControllerContext context);
    }
}
