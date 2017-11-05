using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeHackSecrets.Services.Models;

namespace WeHackSecrets.Services.Actions
{
    public class LoginAction : ILoginAction
    {
        private readonly IHttpClientProxy _client;
        private readonly IAntiForgeryAction _antiForgeryAction;
        private readonly string _relativePath = "Account/Login";

        public LoginAction(IHttpClientProxy client, IAntiForgeryAction antiForgeryAction)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (antiForgeryAction == null) throw new ArgumentNullException("antiForgeryAction");

            _client = client;
            _antiForgeryAction = antiForgeryAction;
        }

        public void Login(string user, string password)
        {
            var antiForgeryToken = _antiForgeryAction.GetToken(_relativePath);

            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Username", user),
                new KeyValuePair<string, string>("Password", password)
            };

            if (!string.IsNullOrEmpty(antiForgeryToken))
            {
                values.Add(new KeyValuePair<string, string>("__RequestVerificationToken", antiForgeryToken));
            }

            // Login as hacker - get session cookie
            var loginRequest = new HttpClientProxyRequest
            {
                Url = _relativePath,
                HttpMethod = HttpMethod.Post,
                Values = values
                //Cookies = antiForgeryResponse.Cookies
            };

            var loginResponse = _client.SendAsync(loginRequest).Result;
        }
    }
}
