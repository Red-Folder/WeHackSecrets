using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHackSecrets.Services;
using WeHackSecrets.Services.Actions;
using WeHackSecrets.Services.Models;
using Xunit;

namespace WeHackSecrets.Tests.Unit.Services.Actions
{
    public class SecretShareTests
    {
        [Fact]
        public void ConstructorFailsIfNullClient()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SecretShare(null, 1)
            );

            Assert.Contains("client", ex.Message);
        }

        [Fact]
        public void GetTargetSecret()
        {
            var responseContent = new StringBuilder();
            responseContent.AppendLine("<html>");
            responseContent.AppendLine("<body>");
            responseContent.AppendLine("<div class=\"panel-heading\">   MySecret   </div>");
            responseContent.AppendLine("<div class=\"panel-body\">  TEST1234   </div>");
            responseContent.AppendLine("</body>");
            responseContent.AppendLine("</html>");

            var response = new HttpClientProxyResponse
            {
                StatusCode = HttpStatusCode.OK,
                Contents = responseContent.ToString()
            };

            var client = new Mock<IHttpClientProxy>();
            client.Setup(x => x.SendAsync(It.IsAny<HttpClientProxyRequest>()))
                .Returns(Task.FromResult(response));

            var sut = new SecretShare(client.Object, 1);
            var result = sut.GetTargetSecret("MySecret");

            Assert.Equal("TEST1234", result);
        }
    }
}
