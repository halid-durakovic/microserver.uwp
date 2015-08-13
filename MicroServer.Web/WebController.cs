using System;

using MicroServer.Networking.Web.Mvc;

namespace MicroServer.Web
{
    public class WebController : WebSocketController
    {
        public IActionResult Session(ControllerContext context)
        {
            return EmptyResult();
        }
    }
}
