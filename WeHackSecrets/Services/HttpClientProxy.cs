using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeHackSecrets.Services.Models;

namespace WeHackSecrets.Services
{
    public class HttpClientProxy : IHttpClientProxy
    {
        private static HttpClient _client;
        private static CookieContainer _cookies;

        public HttpClientProxy()
        {
            _cookies = new CookieContainer();
            var handler = new HttpClientHandler();
            handler.CookieContainer = _cookies;

            _client = new HttpClient(handler);

            _client.BaseAddress = new Uri("https://localhost:44353/");
        }

        public async Task<HttpClientProxyResponse> SendAsync(HttpClientProxyRequest proxyRequest)
        {
            var httpRequest = new HttpRequestMessage(proxyRequest.HttpMethod, proxyRequest.Url);

            if (proxyRequest.Values != null)
            {
                httpRequest.Content = new FormUrlEncodedContent(proxyRequest.Values);
            }

            if (proxyRequest.Cookies != null)
            {
                // TODO
                // Do I need to add this cookie back in or does it remeber for the current session?
                //_cookies.Add(proxyRequest.Cookies);
            }

            var httpResponse = await _client.SendAsync(httpRequest);

            var proxyResponse = new HttpClientProxyResponse
            {
                StatusCode = httpResponse.StatusCode,
                Contents = httpResponse.Content == null ? "" : await httpResponse.Content.ReadAsStringAsync(),
                Cookies = _cookies.GetCookies(new Uri(_client.BaseAddress, proxyRequest.Url))
            };

            //var responseCookies = _cookies.GetCookies().Cast<Cookie>();

            return proxyResponse;
        }

        public async void Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/secrets");
            request.Headers.Add("Cookie", ".AspNetCore.Identity.Application=CfDJ8JIQbnKXGVBOq5Ol3OwTSwZ66ZVMf9sz2rJ-oVnav0-Ib8NUMNhQORaZTWMqsn1KzfaPeBHoDruoY7W54F46VfjRMM2XJIBwzGpiRGt5KcEj0IhB_QbnNtMmd7CUPVwg-T1o36nmJFRQxkWm7VZ7tsdsGxmiK1AWdzmVYvR5f4eKC-yamZAYLIUoghQubKAJt6AHOl4rWIK0qsRnZx7Bm0pxC1QuJ6vPlrDIbSxcK6o2tTwjgkgQ8inx7006QoVqn2gSQeXlCLUlo6EzpkK_zoO2KAnFvdCLFmh84vJi6BIUd0HtI73gL-jWOYCn7ucxONUz-I_IiVPZZYLdXfYPsZQM3UE8kxkBajxTlDcXRCZbmkRpgbvQhMJAirCDy0ik_CCsm415M3XOIUhmOBHnaLYwgFuG8KaQKTipY7oyTC4_mD_tjbpeMbrxEWIdYnG9-MhzkXBagLPkV9PeDgLLB9hf76nNlGQDR71WEbGGKu5cB2J8WHFwjd8RRAHKfyIDPlQotbj1n4KxFiTbptjf_Y7ze6DJ4I-pwQEpddIk8GcR4CIEX18Sm9pl2lB9Cbz5fqH_kUJ6d5tvglh5n4p-GslkmUqkuscEFMFkmJYx3zUyXZDViqFSd5vbxvTohPb9teqLUkM1mDsGhSBV5FRZJto_kuRgDtYwC_et3S1pxppRua9pVR5vGUPkCb5JddLDLA");

            var result = await _client.SendAsync(request);

            var content = await result.Content.ReadAsStringAsync();

            var value = GetSecretFromPanel(content);
        }

        private string GetSecretFromPanel(string content)
        {
            // Remove escape characters
            var cleanContent = content.Replace("\r", string.Empty).Replace("\n", string.Empty);

            // Define the panel pattern
            var panelPattern = new Regex("<div class=\"panel-heading\">([^<]*)</div>[^<]*<div class=\"panel-body\">([^<]*)", RegexOptions.Singleline);
            var panels = panelPattern.Matches(cleanContent);

            // Loop through the found panels to find the secret
            foreach(Match panel in panels)
            {
                if (panel.Success && panel.Groups != null && panel.Groups.Count == 3)
                {
                    var key = panel.Groups[1].Value.Trim();
                    var value = panel.Groups[2].Value.Trim();

                    if (key == "MySecret")
                    {
                        return value;
                    }
                }
            }

            return null;
        }
    }
}
