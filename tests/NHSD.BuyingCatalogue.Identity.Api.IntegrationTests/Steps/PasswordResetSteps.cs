using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class PasswordResetSteps
    {
        private readonly ScenarioContext context;
        private readonly DataProtectorTokenProvider<IdentityUser> dataProtectionProvider;
        private readonly DirectoryInfo keysDirectory = new("DP_Keys");
        private readonly IXmlRepository keyRepository;
        private readonly Settings settings;

        public PasswordResetSteps(ScenarioContext context, Settings settings)
        {
            this.context = context;
            this.settings = settings;

            keyRepository = new FileSystemXmlRepository(keysDirectory, NullLoggerFactory.Instance);
            var provider = DataProtectionProvider.Create(
                keysDirectory,
                b => b.SetApplicationName(settings.DataProtectionAppName));

            dataProtectionProvider = new DataProtectorTokenProvider<IdentityUser>(
                provider,
                null,
                new Logger<DataProtectorTokenProvider<IdentityUser>>(NullLoggerFactory.Instance));
        }

        private IEnumerable<DataProtectionKey> Keys =>
            keyRepository.GetAllElements().Select(e => new DataProtectionKey(e));

        [When(@"the user with ID (\S*) has an expired password reset token")]
        public async Task WhenTheUserWithIdHasExpiredPasswordResetTokenAsync(string userId)
        {
            await WhenTheUserWithIdHasValidPasswordResetTokenAsync(userId);
            var userEntity = new UserEntity { Id = userId, SecurityStamp = Guid.NewGuid().ToString() };
            await userEntity.UpdateSecurityStamp(settings.ConnectionString);
        }

        [When(@"the user with ID (\S*) has a valid password reset token")]
        public async Task WhenTheUserWithIdHasValidPasswordResetTokenAsync(string userId)
        {
            await SaveDbKeysToRepositoryAsync();

            var userEntity = new UserEntity { Id = userId };
            var userInDb = await userEntity.GetAsync(settings.ConnectionString);

            var identityUser = new IdentityUser(userId)
            {
                Email = userInDb.EmailAddress,
                Id = userId,
                SecurityStamp = userInDb.SecurityStamp,
            };

            using var userStore = new UserStore(identityUser);
            using var userManager = new UserManager<IdentityUser>(
                userStore,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            userManager.RegisterTokenProvider(TokenOptions.DefaultProvider, dataProtectionProvider);

            context.Set(identityUser);
            context[ScenarioContextKeys.PasswordResetToken] = await userManager.GeneratePasswordResetTokenAsync(identityUser);

            await DataProtectionKeys.SaveToDbAsync(settings.ConnectionString, Keys);
        }

        private async Task SaveDbKeysToRepositoryAsync()
        {
            var dbKeys = await DataProtectionKeys.GetFromDbAsync(settings.ConnectionString);
            var newKeys = dbKeys.Except(Keys);

            foreach (var key in newKeys)
                keyRepository.StoreElement(key.Element, key.FriendlyName);
        }
    }
}
