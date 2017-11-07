using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeHackSecrets.Services;
using WeHackSecrets.Services.Actions;
using Xunit;

namespace WeHackSecrets.Tests.Unit.Services
{
    public class SqlInjectionCopySecretOnSecretSaveTests
    {
        [Fact]
        public void ConstructorWithNoHackerUserThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SqlInjectionCopySecretOnSecretSave(null, null, null, null, null, null)
            );

            Assert.Contains("hackerUser", ex.Message);
        }

        [Fact]
        public void ConstructorWithNoTargetUserThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SqlInjectionCopySecretOnSecretSave("HackerUser", null, null, null, null, null)
            );

            Assert.Contains("targetUser", ex.Message);
        }

        [Fact]
        public void ConstructorWithNoTargetKeyThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SqlInjectionCopySecretOnSecretSave("HackerUser", "TargetUser", null, null, null, null)
            );

            Assert.Contains("targetKey", ex.Message);
        }

        [Fact]
        public void ConstructorWithNoLoginActionThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SqlInjectionCopySecretOnSecretSave("HackerUser", "TargetUser", "TargetKey", null, null, null)
            );

            Assert.Contains("loginAction", ex.Message);
        }

        [Fact]
        public void ConstructorWithNoCreateSecretActionThrowsArgumentException()
        {
            var login = new Mock<ILoginAction>();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SqlInjectionCopySecretOnSecretSave("HackerUser", "TargetUser", "TargetKey", login.Object, null, null)
            );

            Assert.Contains("createSecretAction", ex.Message);
        }

        [Fact]
        public void ConstructorWithNoSecretsListThrowsArgumentException()
        {
            var login = new Mock<ILoginAction>();
            var createSecret = new Mock<ICreateSecretAction>();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                new SqlInjectionCopySecretOnSecretSave("HackerUser", "TargetUser", "TargetKey", login.Object, createSecret.Object, null)
            );

            Assert.Contains("secretsList", ex.Message);
        }

        [Fact]
        public void SuccessfulFalseOnConstruction()
        {
            var login = new Mock<ILoginAction>();
            var createSecret = new Mock<ICreateSecretAction>();
            var secretsList = new Mock<ISecretsList>();

            var sut = new SqlInjectionCopySecretOnSecretSave("HackerUser", "TargetUser", "TargetKey", login.Object, createSecret.Object, secretsList.Object);

            Assert.False(sut.Successful);
        }

        [Fact]
        public void SecretValueEmptyOnConstruction()
        {
            var login = new Mock<ILoginAction>();
            var createSecret = new Mock<ICreateSecretAction>();
            var secretsList = new Mock<ISecretsList>();

            var sut = new SqlInjectionCopySecretOnSecretSave("HackerUser", "TargetUser", "TargetKey", login.Object, createSecret.Object, secretsList.Object);

            Assert.Empty(sut.SecretValue);
        }

        [Fact]
        public void SuccessfulExploit()
        {
            var login = new Mock<ILoginAction>();
            login.Setup(x => x.LoginAsync("HackerUser", "Test1234!")).Returns(Task.CompletedTask);
            var createSecret = new Mock<ICreateSecretAction>();
            createSecret.Setup(x => x.Create("HackerKey", It.IsAny<string>()));
            var secretsList = new Mock<ISecretsList>();
            secretsList.Setup(x => x.GetTargetSecret("TargetKey")).Returns("Secret1234");

            var sut = new SqlInjectionCopySecretOnSecretSave("HackerUser", "TargetUser", "TargetKey", login.Object, createSecret.Object, secretsList.Object);

            sut.Exploit();
            login.VerifyAll();
            createSecret.VerifyAll();
            secretsList.VerifyAll();
            Assert.True(sut.Successful);
            Assert.Equal("Secret1234", sut.SecretValue);
        }


        // TODO - handle failure & exceptions
    }
}
