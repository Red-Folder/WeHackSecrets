using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeHackSecrets.Services.Models;

namespace WeHackSecrets.Services.Actions
{
    public class SecretsList : ISecretsList
    {
        private readonly SecretsListExtract _inner;

        public SecretsList(IHttpClientProxy client)
        {
            if (client == null) throw new ArgumentNullException("client");

            _inner = new SecretsListExtract(client, "Secrets");
        }

        public string GetTargetSecret(string secretKey)
        {
            return _inner.GetTargetSecret(secretKey);
        }
    }
}
