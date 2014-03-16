using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Veis.WebInterface.Infrastructure;

namespace Veis.WebInterface.Controllers
{
    public class Controller
    {
        private const string ErrorPage = "errorpage";

        protected Hashtable FormulateResponse(string strOut, int responseCode = 404)
        {
            var responsedata = new Hashtable();
            responsedata["int_response_code"] = responseCode;
            responsedata["content_type"] = "text/html";
            responsedata["keepalive"] = false;
            responsedata["str_response_string"] = strOut;
            return responsedata;
        }

        public Hashtable ShowErrorPage(params string[] messages)
        {
            var errorModel = messages.ToList();
            var errorPage = PageBuilder.Current.Transform(ErrorPage, errorModel);
            return FormulateResponse(errorPage, 404);
        }
    }
}
