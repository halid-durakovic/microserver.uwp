using System;

using MicroServer.Networking.Web.Mvc;

namespace MicroServer.Web
{
    public class HomeController : Controller
    {
        public IActionResult Index(ControllerContext context)
        {
            string response = "<!doctype html><html><head><title>Hello, world!</title>" +
                "<style>body { background-color: #111 }" +
                "h1 { font-size:3cm; text-align: center; color: white;}</style></head>" +
                "<body><h1>" + DateTime.Now.ToString() + "</h1></body></html>\r\n";

            return ContentResult(response, "text/html");
        }
    }
}
