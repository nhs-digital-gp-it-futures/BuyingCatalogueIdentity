using System;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class LoginViewModelBuilder
    {
        private string emailAddress;
        private string password;
        private Uri returnUrl;

        private LoginViewModelBuilder()
        {
            returnUrl = new Uri("https://postman-echo.com/get", UriKind.Absolute);
        }

        internal static LoginViewModelBuilder Create()
        {
            return new();
        }

        internal LoginViewModelBuilder WithEmailAddress(string address)
        {
            emailAddress = address;
            return this;
        }

        internal LoginViewModelBuilder WithPassword(string pass)
        {
            password = pass;
            return this;
        }

        internal LoginViewModelBuilder WithReturnUrl(Uri url)
        {
            returnUrl = url;
            return this;
        }

        internal LoginViewModel Build()
        {
            return new()
            {
                EmailAddress = emailAddress,
                Password = password,
                ReturnUrl = returnUrl,
            };
        }
    }
}
