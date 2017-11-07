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
    public class LoginActionTests
    {
        [Fact]
        public void ConstructorFailsIfNullClient()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new LoginAction(null, null)
            );

            Assert.Contains("client", ex.Message);
        }

        [Fact]
        public void ConstructorFailsIfNullAntiForgeryAction()
        {
            var client = new Mock<IHttpClientProxy>();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                new LoginAction(client.Object, null)
            );

            Assert.Contains("antiForgeryAction", ex.Message);
        }

        [Fact]
        public async void LoginSuccessful()
        {
            var client = new Mock<IHttpClientProxy>();
            var response = new HttpClientProxyResponse
            {
                StatusCode = HttpStatusCode.OK,
                Contents = "<html><body><h1>Hello World</h2></body></html>"
            };

            client.Setup(x => x.SendAsync(It.Is<HttpClientProxyRequest>(y =>
                y.Values.Any(z => z.Key == "Username" && z.Value == "Username") &&
                y.Values.Any(z => z.Key == "Password" && z.Value == "Password"))))
                .Returns(Task.FromResult(response));

            var antiForgery = new Mock<IAntiForgeryAction>();

            var sut = new LoginAction(client.Object, antiForgery.Object);

            await sut.LoginAsync("Username", "Password");

            Assert.True(sut.Successful);
            client.VerifyAll();
        }

        [Fact]
        public async void LoginSuccessfulWithAntiForgery()
        {
            var client = new Mock<IHttpClientProxy>();
            var response = new HttpClientProxyResponse
            {
                StatusCode = HttpStatusCode.OK,
                Contents = "<html><body><h1>Hello World</h2></body></html>"
            };

            client.Setup(x => x.SendAsync(It.Is<HttpClientProxyRequest>(y =>
                y.Values.Any(z => z.Key == "Username" && z.Value == "Username") &&
                y.Values.Any(z => z.Key == "Password" && z.Value == "Password") &&
                y.Values.Any(z => z.Key == "__RequestVerificationToken" && z.Value == "Token1234"))))
                .Returns(Task.FromResult(response));

            var antiForgery = new Mock<IAntiForgeryAction>();
            antiForgery.Setup(x => x.GetToken(It.IsAny<string>())).Returns("Token1234");

            var sut = new LoginAction(client.Object, antiForgery.Object);

            await sut.LoginAsync("Username", "Password");

            Assert.True(sut.Successful);
            client.VerifyAll();
        }

        [Fact]
        public async void LoginWithNoUsernameThrowArguementException()
        {
            var client = new Mock<IHttpClientProxy>();
            var antiForgery = new Mock<IAntiForgeryAction>();

            var sut = new LoginAction(client.Object, antiForgery.Object);

            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                sut.LoginAsync(null, "Password")
            );

            Assert.Contains("user", ex.Message);
        }

        [Fact]
        public async void LoginWithNoPasswordThrowArguementException()
        {
            var client = new Mock<IHttpClientProxy>();
            var antiForgery = new Mock<IAntiForgeryAction>();

            var sut = new LoginAction(client.Object, antiForgery.Object);

            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                sut.LoginAsync("Username", null)
            );

            Assert.Contains("password", ex.Message);
        }

        [Fact]
        public async void LoginFailure()
        {
            var client = new Mock<IHttpClientProxy>();
            var response = new HttpClientProxyResponse
            {
                StatusCode = HttpStatusCode.OK,
                Contents = "<html><body><h1>Hello Hacker -> Invalid login attempt.</h2></body></html>"
            };

            client.Setup(x => x.SendAsync(It.Is<HttpClientProxyRequest>(y =>
                y.Values.Any(z => z.Key == "Username" && z.Value == "Username") &&
                y.Values.Any(z => z.Key == "Password" && z.Value == "Password"))))
                .Returns(Task.FromResult(response));

            var antiForgery = new Mock<IAntiForgeryAction>();

            var sut = new LoginAction(client.Object, antiForgery.Object);

            await sut.LoginAsync("Username", "Password");

            Assert.False(sut.Successful);
            client.VerifyAll();
        }

        [Fact]
        public async void LoginFailureWithAntiForgery()
        {
            var client = new Mock<IHttpClientProxy>();
            var response = new HttpClientProxyResponse
            {
                StatusCode = HttpStatusCode.OK,
                Contents = "<html><body><h1>Hello Hacker -> Invalid login attempt.</h2></body></html>"
            };

            client.Setup(x => x.SendAsync(It.Is<HttpClientProxyRequest>(y =>
                y.Values.Any(z => z.Key == "Username" && z.Value == "Username") &&
                y.Values.Any(z => z.Key == "Password" && z.Value == "Password") &&
                y.Values.Any(z => z.Key == "__RequestVerificationToken" && z.Value == "Token1234"))))
                .Returns(Task.FromResult(response));

            var antiForgery = new Mock<IAntiForgeryAction>();
            antiForgery.Setup(x => x.GetToken(It.IsAny<string>())).Returns("Token1234");

            var sut = new LoginAction(client.Object, antiForgery.Object);

            await sut.LoginAsync("Username", "Password");

            Assert.False(sut.Successful);
            client.VerifyAll();
        }

    }
}
