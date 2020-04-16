using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public interface IPasswordResetCallback
    {
        Uri GetPasswordResetCallback(PasswordResetToken token);
    }
}
