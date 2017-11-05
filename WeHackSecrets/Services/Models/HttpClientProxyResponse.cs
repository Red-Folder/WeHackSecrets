using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WeHackSecrets.Services.Models
{
    public class HttpClientProxyResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public CookieCollection Cookies { get; set; }

        public string Contents { get; set; }
    }
}
