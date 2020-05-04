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
        private readonly ScenarioContext _context;
        private readonly DataProtectorTokenProvider<IdentityUser> _dataProtectionProvider;
        private readonly DirectoryInfo _keysDirectory = new DirectoryInfo("DP_Keys");
        private readonly IXmlRepository _keyRepository;
        private readonly Settings _settings;

        public PasswordResetSteps(ScenarioContext context, Settings settings)
        {
            _context = context;
            _settings = settings;

            _keyRepository = new FileSystemXmlRepository(_keysDirectory, NullLoggerFactory.Instance);
            var provider = DataProtectionProvider.Create(
                _keysDirectory,
                b => b.SetApplicationName(settings.DataProtectionAppName));

            _dataProtectionProvider = new DataProtectorTokenProvider<IdentityUser>(
                provider,
                null,
                new Logger<DataProtectorTokenProvider<IdentityUser>>(NullLoggerFactory.Instance));
        }

        private IEnumerable<DataProtectionKey> Keys =>
            _keyRepository.GetAllElements().Select(e => new DataProtectionKey(e));

        [When(@"the user with ID (\S*) has a valid password reset token")]
        public async Task WhenTheUserWithIdHasValidPasswordResetTokenAsync(string userId)
        {
            await SaveDbKeysToRepositoryAsync();

            var userEntity = new UserEntity { Id = userId };
            var userInDb = await userEntity.GetAsync(_settings.ConnectionString);

            var identityUser = new IdentityUser(userId)
            {
                Email = userInDb.EmailAddress,
                Id = userId,
                SecurityStamp = userInDb.SecurityStamp,
            };

            var userManager = new UserManager<IdentityUser>(
                new UserStore(identityUser),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            userManager.RegisterTokenProvider(TokenOptions.DefaultProvider, _dataProtectionProvider);

            _context.Set(identityUser);
            _context[ScenarioContextKeys.PasswordResetToken] = await userManager.GeneratePasswordResetTokenAsync(identityUser);

            await DataProtectionKeys.SaveToDbAsync(_settings.ConnectionString, Keys);
        }

        private async Task SaveDbKeysToRepositoryAsync()
        {
            var dbKeys = await DataProtectionKeys.GetFromDbAsync(_settings.ConnectionString);
            var newKeys = dbKeys.Except(Keys);

            foreach (var key in newKeys)
                _keyRepository.StoreElement(key.Element, key.FriendlyName);
        }
    }
}
