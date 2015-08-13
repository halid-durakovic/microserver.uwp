using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServer.Networking.Web
{
    public class HtmlFactory
    {
        public static string HttpResponse(string charset, string title, string body)
        {
            string lang = "en";
            string htmlPage = string.Format(
                @"<!DOCTYPE html>" +
                @"<html lang='{0}'>" +
                @"  <head>" +
                @"      <meta charset='{1}'>" +
                @"      <title>{2}</title >" +
                @"  </head>" +
                @"  <body>{3}</body>" +
                @"</html>", lang, charset, title, body
                );
            return htmlPage;
        }
    }
}
