using System.Threading.Tasks;
using WeHackSecrets.Services.Models;

namespace WeHackSecrets.Services
{
    public interface IHttpClientProxy
    {
        Task<HttpClientProxyResponse> SendAsync(HttpClientProxyRequest proxyRequest);
    }
}