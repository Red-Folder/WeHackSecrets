using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeHackSecrets.Services.Models;

namespace WeHackSecrets.Services.Actions
{
    public class SecretsListExtract
    {
        private readonly IHttpClientProxy _client;
        private readonly string _relativePath = "Secrets";

        public SecretsListExtract(IHttpClientProxy client, string relativePath)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (relativePath == null) throw new ArgumentNullException("relativePath");

            _client = client;
            _relativePath = relativePath;
        }

        public string GetTargetSecret(string secretKey)
        {
            // Get the contents of the Secrets
            var secretsRequest = new HttpClientProxyRequest
            {
                Url = _relativePath,
                HttpMethod = HttpMethod.Get
            };

            var secretsResponse = _client.SendAsync(secretsRequest).Result;

            return GetSecret(secretsResponse.Contents, secretKey);
        }

        private string GetSecret(string content, string secretKey)
        {
            // Remove escape characters
            var cleanContent = content.Replace("\r", string.Empty).Replace("\n", string.Empty);

            // Define the panel pattern
            var panelPattern = new Regex("<div class=\"panel-heading\">([^<]*)</div>[^<]*<div class=\"panel-body\">([^<]*)", RegexOptions.Singleline);
            var panels = panelPattern.Matches(cleanContent);

            // Loop through the found panels to find the secret
            foreach (Match panel in panels)
            {
                if (panel.Success && panel.Groups != null && panel.Groups.Count == 3)
                {
                    var key = panel.Groups[1].Value.Trim();
                    var value = panel.Groups[2].Value.Trim();

                    if (key == secretKey)
                    {
                        return value;
                    }
                }
            }

            return null;
        }
    }
}
