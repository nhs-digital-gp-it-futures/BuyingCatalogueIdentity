using System;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class LoginViewModelBuilder
    {
        private string _emailAddress;
        private string _password;
        private Uri _returnUrl;
        private string _disabledError;

        private LoginViewModelBuilder()
        {
            _returnUrl = new Uri("https://postman-echo.com/get", UriKind.Absolute);
        }

        internal static LoginViewModelBuilder Create()
        {
            return new LoginViewModelBuilder();
        }

        internal LoginViewModelBuilder WithEmailAddress(string emailAddress)
        {
            _emailAddress = emailAddress;
            return this;
        }

        internal LoginViewModelBuilder WithPassword(string password)
        {
            _password = password;
            return this;
        }

        internal LoginViewModelBuilder WithReturnUrl(Uri returnUrl)
        {
            _returnUrl = returnUrl;
            return this;
        }        
        
        internal LoginViewModelBuilder WithDisabledError(string disabledError)
        {
            _disabledError = disabledError;
            return this;
        }

        internal LoginViewModel Build()
        {
            return new LoginViewModel
            {
                EmailAddress = _emailAddress,
                Password = _password,
                ReturnUrl = _returnUrl,
                DisabledError = _disabledError
            };
        }
    }
}
