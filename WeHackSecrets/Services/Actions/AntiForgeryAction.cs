using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeHackSecrets.Services.Models;

namespace WeHackSecrets.Services.Actions
{
    public class AntiForgeryAction : IAntiForgeryAction
    {
        private readonly IHttpClientProxy _client;

        public AntiForgeryAction(IHttpClientProxy client)
        {
            if (client == null) throw new ArgumentNullException("client");

            _client = client;
        }

        public string GetToken(string relativePath)
        {
            // Get the anti-forgery token for login
            var antiForgeryRequest = new HttpClientProxyRequest
            {
                Url = relativePath,
                HttpMethod = HttpMethod.Get
            };
            var antiForgeryResponse = _client.SendAsync(antiForgeryRequest).Result;

            return GetTokenFromContent(antiForgeryResponse.Contents);
        }

        private string GetTokenFromContent(string content)
        {
            // <input name="__RequestVerificationToken" type="hidden" value="CfDJ8JIQbnKXGVBOq5Ol3OwTSwb70NghdjkTWWkiCgFC5kG5VmAlu8-RN920o0MiE8YJzf2IwWW4_wWcCd3a2qqi3JXrOyixeL_2ZSVi2cPXpPwtdlOaK9z7JuICBwQftxH7ZJt4u3pEBiH3E0sy6D2AWa4" />
            var pattern = new Regex("<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]*)", RegexOptions.Singleline);

            var match = pattern.Match(content);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
