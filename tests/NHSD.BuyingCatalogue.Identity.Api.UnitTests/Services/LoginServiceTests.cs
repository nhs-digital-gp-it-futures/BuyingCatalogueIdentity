﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class LoginServiceTests
    {
        [Test]
        public async Task SignInAsync_FailedSignIn_NullContext_ReturnsExpectedResult()
        {
            using var loginService = new LoginServiceBuilder()
                .WithSignInResult(IdentitySignInResult.Failed)
                .Build();

            var result = await loginService.SignInAsync("user", "pass", null);

            Assert.NotNull(result);
            result.IsSuccessful.Should().BeFalse();
            result.IsTrustedReturnUrl.Should().BeFalse();
            result.LoginHint.Should().BeNull();
        }

        [Test]
        public async Task SignInAsync_FailedSignIn_WithContext_ReturnsExpectedResult()
        {
            const string loginHint = "Hint";

            var authorizationRequest = new AuthorizationRequest { LoginHint = loginHint };
            using var loginService = new LoginServiceBuilder()
                .WithAuthorizationContextResult(authorizationRequest)
                .WithSignInResult(IdentitySignInResult.Failed)
                .Build();

            var result = await loginService.SignInAsync("user", "pass", null);

            Assert.NotNull(result);
            result.IsSuccessful.Should().BeFalse();
            result.IsTrustedReturnUrl.Should().BeFalse();
            result.LoginHint.Should().Be(loginHint);
        }

        [Test]
        public async Task SignInAsync_FailedSignInWithContext_RaisesLoginFailureEvent()
        {
            const string clientId = "ClientId";
            const string username = "UncleBob@email.com";

            int eventCount = 0;
            UserLoginFailureEvent raisedEvent = null;

            void EventCallback(UserLoginFailureEvent evt)
            {
                eventCount++;
                raisedEvent = evt;
            }

            using var loginService = new LoginServiceBuilder()
                .WithEventServiceCallback<UserLoginFailureEvent>(EventCallback)
                .WithAuthorizationContextResult(new AuthorizationRequest { ClientId = clientId })
                .WithSignInResult(IdentitySignInResult.Failed)
                .Build();

            await loginService.SignInAsync(username, "pass", null);

            eventCount.Should().Be(1);
            Assert.NotNull(raisedEvent);
            raisedEvent.ClientId.Should().Be(clientId);
            raisedEvent.Message.Should().Be(LoginService.EventMessage);
            raisedEvent.Username.Should().Be(username);
        }

        [Test]
        public async Task SignInAsync_FailedSignInWithNullContext_RaisesLoginFailureEvent()
        {
            const string username = "UncleBob@email.com";

            int eventCount = 0;
            UserLoginFailureEvent raisedEvent = null;

            void EventCallback(UserLoginFailureEvent evt)
            {
                eventCount++;
                raisedEvent = evt;
            }

            using var loginService = new LoginServiceBuilder()
                .WithEventServiceCallback<UserLoginFailureEvent>(EventCallback)
                .WithSignInResult(IdentitySignInResult.Failed)
                .Build();

            await loginService.SignInAsync(username, "pass", null);

            eventCount.Should().Be(1);
            Assert.NotNull(raisedEvent);
            raisedEvent.ClientId.Should().BeNull();
            raisedEvent.Message.Should().Be(LoginService.EventMessage);
            raisedEvent.Username.Should().Be(username);
        }

        [Test]
        [TestCase(null, "Password")]
        [TestCase("", "Password")]
        [TestCase("\t", "Password")]
        [TestCase("User", null)]
        [TestCase("User", "")]
        [TestCase("User", "\t")]
        public async Task SignInAsync_MissingCredentials_ReturnsExpectedResult(string username, string password)
        {
            using var loginService = new LoginServiceBuilder().Build();

            var result = await loginService.SignInAsync(username, password, null);

            Assert.NotNull(result);
            result.IsSuccessful.Should().BeFalse();
        }

        [Test]
        public async Task SignInAsync_SuccessfulSignIn_NullContext_ReturnsExpectedResult()
        {
            using var loginService = new LoginServiceBuilder()
                .WithSignInResult(IdentitySignInResult.Success)
                .Build();

            var result = await loginService.SignInAsync("user", "pass", null);

            Assert.NotNull(result);
            result.IsSuccessful.Should().BeTrue();
            result.IsTrustedReturnUrl.Should().BeFalse();
            result.LoginHint.Should().BeNull();
        }

        [Test]
        public async Task SignInAsync_SuccessfulSignIn_WithContext_ReturnsExpectedResult()
        {
            using var loginService = new LoginServiceBuilder()
                .WithAuthorizationContextResult(new AuthorizationRequest())
                .WithSignInResult(IdentitySignInResult.Success)
                .Build();

            var result = await loginService.SignInAsync("user", "pass", null);

            Assert.NotNull(result);
            result.IsSuccessful.Should().BeTrue();
            result.IsTrustedReturnUrl.Should().BeTrue();
            result.LoginHint.Should().BeNull();
        }

        [Test]
        public async Task SignInAsync_SuccessfulSignInWithContext_RaisesLoginSuccessEvent()
        {
            int eventCount = 0;
            UserLoginSuccessEvent raisedEvent = null;

            void EventCallback(UserLoginSuccessEvent evt)
            {
                eventCount++;
                raisedEvent = evt;
            }

            const string clientId = "ClientId";
            const string userId = "UserId";
            const string username = "UncleBob@email.com";

            using var loginService = new LoginServiceBuilder()
                .WithEventServiceCallback<UserLoginSuccessEvent>(EventCallback)
                .WithAuthorizationContextResult(new AuthorizationRequest { ClientId = clientId })
                .WithSignInResult(IdentitySignInResult.Success)
                .WithFindUserResult(ApplicationUserBuilder
                    .Create()
                    .WithUserId(userId)
                    .WithUsername(username)
                    .Build())
                .Build();

            await loginService.SignInAsync("user", "pass", new Uri("~/", UriKind.Relative));

            eventCount.Should().Be(1);
            Assert.NotNull(raisedEvent);
            raisedEvent.ClientId.Should().Be(clientId);
            raisedEvent.DisplayName.Should().Be(username);
            raisedEvent.Message.Should().BeNull();
            raisedEvent.SubjectId.Should().Be(userId);
            raisedEvent.Username.Should().Be(username);
        }

        [Test]
        public async Task SignInAsync_SuccessfulSignInWithNullContext_RaisesLoginSuccessEvent()
        {
            int eventCount = 0;
            UserLoginSuccessEvent raisedEvent = null;

            void EventCallback(UserLoginSuccessEvent evt)
            {
                eventCount++;
                raisedEvent = evt;
            }

            const string userId = "UserId";
            const string username = "UncleBob@email.com";

            using var loginService = new LoginServiceBuilder()
                .WithEventServiceCallback<UserLoginSuccessEvent>(EventCallback)
                .WithSignInResult(IdentitySignInResult.Success)
                .WithFindUserResult(ApplicationUserBuilder
                    .Create()
                    .WithUserId(userId)
                    .WithUsername(username)
                    .Build())
                .Build();

            await loginService.SignInAsync("user", "pass", new Uri("~/", UriKind.Relative));

            eventCount.Should().Be(1);
            Assert.NotNull(raisedEvent);
            raisedEvent.ClientId.Should().BeNull();
            raisedEvent.DisplayName.Should().Be(username);
            raisedEvent.Message.Should().BeNull();
            raisedEvent.SubjectId.Should().Be(userId);
            raisedEvent.Username.Should().Be(username);
        }
    }
}
