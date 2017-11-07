using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeHackSecrets.Services.Models;

namespace WeHackSecrets.Services.Actions
{
    public class CreateSecretAction : ICreateSecretAction
    {
        private readonly IHttpClientProxy _client;
        private readonly IAntiForgeryAction _antiForgeryAction;
        private readonly string _relativePath = "Secrets";

        public CreateSecretAction(IHttpClientProxy client, IAntiForgeryAction antiForgeryAction)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (antiForgeryAction == null) throw new ArgumentNullException("antiForgeryAction");

            _client = client;
            _antiForgeryAction = antiForgeryAction;
        }

        public void Create(string key, string value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            var antiForgeryToken = _antiForgeryAction.GetToken(_relativePath);

            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Key", key),
                new KeyValuePair<string, string>("Value", value)
            };

            if (!string.IsNullOrEmpty(antiForgeryToken))
            {
                values.Add(new KeyValuePair<string, string>("__RequestVerificationToken", antiForgeryToken));
            }

            // Login as hacker - get session cookie
            var newSecretRequest = new HttpClientProxyRequest
            {
                Url = _relativePath,
                HttpMethod = HttpMethod.Post,
                Values = values
                //Cookies = antiForgeryResponse.Cookies
            };

            var newSecretResponse = _client.SendAsync(newSecretRequest).Result;
        }

    }
}
