using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeHackSecrets.Services.Actions
{
    public class SecretShare : ISecretsList
    {
        private readonly SecretsListExtract _inner;

        public SecretShare(IHttpClientProxy client, int id)
        {
            if (client == null) throw new ArgumentNullException("client");

            _inner = new SecretsListExtract(client, $"Secrets/PublicShare/{id}");
        }

        public string GetTargetSecret(string secretKey)
        {
            return _inner.GetTargetSecret(secretKey);
        }

    }
}
