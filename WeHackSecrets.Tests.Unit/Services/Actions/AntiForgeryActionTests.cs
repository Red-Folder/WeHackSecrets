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
    public class AntiForgeryActionTests
    {
        [Fact]
        public void ConstructorFailsIfNullClient()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new AntiForgeryAction(null)
            );

            Assert.Contains("client", ex.Message);
        }

        [Fact]
        public void GetToken()
        {
            var responseContent = new StringBuilder();
            responseContent.AppendLine("<html>");
            responseContent.AppendLine("<body>");
            responseContent.AppendLine("<form>");
            responseContent.AppendLine("<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"ThisIsATestToken\"/>");
            responseContent.AppendLine("</form>");
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

            var sut = new AntiForgeryAction(client.Object);
            var result = sut.GetToken("");

            Assert.Equal("ThisIsATestToken", result);
        }

        [Fact]
        public void GetTokenWithNoNameThrowsArguementException()
        {
            var client = new Mock<IHttpClientProxy>();

            var sut = new AntiForgeryAction(client.Object);

            var ex = Assert.Throws<ArgumentNullException>(() =>
                sut.GetToken(null)
            );

            Assert.Contains("relativePath", ex.Message);
        }
    }
}
