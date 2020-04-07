using System;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Drivers;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Hooks
{
    [Binding]
    public sealed class IntegrationHook
    {
        private readonly IObjectContainer _objectContainer;

        public IntegrationHook(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
        }

        [BeforeScenario]
        public async Task BeforeScenarioAsync()
        {
            RegisterTestConfiguration();

            RegisterCustomValueRetrievers();

            await ResetDatabaseAsync();
        }

        [AfterScenario]
        public async Task CleanUpAsync() => await DeleteAllSentEmailsAsync();

        public void RegisterTestConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            _objectContainer.RegisterInstanceAs<IConfiguration>(configurationBuilder);
        }

        private static void RegisterCustomValueRetrievers()
        {
            var valueRetrievers = Service.Instance.ValueRetrievers;

            valueRetrievers.Register(new NullStringValueRetriever());
            valueRetrievers.Register(new GenerateStringLengthValueRetriever());
        }

        private async Task ResetDatabaseAsync() => 
            await IntegrationDatabase.ResetAsync(_objectContainer.Resolve<IConfiguration>());

        private async Task DeleteAllSentEmailsAsync()
        {
            var emailServerDriver = _objectContainer.Resolve<EmailServerDriver>();
            await emailServerDriver.ClearAllEmailsAsync();
        }
    }
}
