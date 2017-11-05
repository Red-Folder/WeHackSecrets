using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeHackSecrets.Services.Models
{
    public class HttpClientProxyRequest
    {
        public string Url { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Values { get; set; }
        public CookieCollection Cookies { get; set; }
    }
}
