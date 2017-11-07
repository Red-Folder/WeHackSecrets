using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHackSecrets.Services;
using WeHackSecrets.Services.Actions;
using WeHackSecrets.Services.Models;
using Xunit;

namespace WeHackSecrets.Tests.Unit.Services.Actions
{
    public class CreateSecretActionTests
    {
        [Fact]
        public void ConstructorFailsIfNullClient()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new CreateSecretAction(null, null)
            );

            Assert.Contains("client", ex.Message);
        }

        [Fact]
        public void ConstructorFailsIfNullAntiForgeryAction()
        {
            var client = new Mock<IHttpClientProxy>();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                new CreateSecretAction(client.Object, null)
            );

            Assert.Contains("antiForgeryAction", ex.Message);
        }

        [Fact]
        public void CreatesSecret()
        {
            var client = new Mock<IHttpClientProxy>();
            var response = new HttpClientProxyResponse
            {
                StatusCode = HttpStatusCode.OK
            };

            client.Setup(x => x.SendAsync(It.Is<HttpClientProxyRequest>(y =>
                y.Values.Any(z => z.Key == "Key" && z.Value == "SecretKey") &&
                y.Values.Any(z => z.Key == "Value" && z.Value == "SecretValue"))))
                .Returns(Task.FromResult(response));

            var antiForgery = new Mock<IAntiForgeryAction>();

            var sut = new CreateSecretAction(client.Object, antiForgery.Object);

            sut.Create("SecretKey", "SecretValue");

            client.VerifyAll();
        }

        [Fact]
        public void CreateWithNoKeyThrowArguementException()
        {
            var client = new Mock<IHttpClientProxy>();
            var antiForgery = new Mock<IAntiForgeryAction>();

            var sut = new CreateSecretAction(client.Object, antiForgery.Object);

            var ex = Assert.Throws<ArgumentNullException>(() =>
                sut.Create(null, "SecretValue")
            );

            Assert.Contains("key", ex.Message);
        }

        [Fact]
        public void CreateWithNoValueThrowArguementException()
        {
            var client = new Mock<IHttpClientProxy>();
            var antiForgery = new Mock<IAntiForgeryAction>();

            var sut = new CreateSecretAction(client.Object, antiForgery.Object);

            var ex = Assert.Throws<ArgumentNullException>(() =>
                sut.Create("SecretKey", null)
            );

            Assert.Contains("value", ex.Message);
        }

        [Fact]
        public void CreatesSecretWithAntiForgery()
        {
            var client = new Mock<IHttpClientProxy>();
            var response = new HttpClientProxyResponse
            {
                StatusCode = HttpStatusCode.OK
            };

            client.Setup(x => x.SendAsync(It.Is<HttpClientProxyRequest>(y =>
                y.Values.Any(z => z.Key == "Key" && z.Value == "SecretKey") &&
                y.Values.Any(z => z.Key == "Value" && z.Value == "SecretValue") &&
                y.Values.Any(z => z.Key == "__RequestVerificationToken" && z.Value == "Token1234"))))
                .Returns(Task.FromResult(response));

            var antiForgery = new Mock<IAntiForgeryAction>();
            antiForgery.Setup(x => x.GetToken(It.IsAny<string>())).Returns("Token1234");

            var sut = new CreateSecretAction(client.Object, antiForgery.Object);

            sut.Create("SecretKey", "SecretValue");

            client.VerifyAll();

        }
    }
}
