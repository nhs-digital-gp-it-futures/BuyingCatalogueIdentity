using System;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Drivers;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Hooks
{
    [Binding]
    internal sealed class IntegrationHook
    {
        private readonly IObjectContainer objectContainer;

        public IntegrationHook(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
        }

        [BeforeScenario]
        public async Task BeforeScenarioAsync()
        {
            RegisterTestConfiguration();

            RegisterCustomValueRetrievers();

            await ResetDatabaseAsync();
        }

        [AfterScenario]
        public async Task CleanUpAsync()
        {
            await DeleteAllSentEmailsAsync();
        }

        public void RegisterTestConfiguration()
        {
            // ReSharper disable once StringLiteralTypo
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            objectContainer.RegisterInstanceAs<IConfiguration>(configurationBuilder);
        }

        private static void RegisterCustomValueRetrievers()
        {
            var valueRetrievers = Service.Instance.ValueRetrievers;

            valueRetrievers.Register(new NullStringValueRetriever());
            valueRetrievers.Register(new GenerateStringLengthValueRetriever());
        }

        private async Task ResetDatabaseAsync() =>
            await IntegrationDatabase.ResetAsync(objectContainer.Resolve<IConfiguration>());

        private async Task DeleteAllSentEmailsAsync()
        {
            var emailServerDriver = objectContainer.Resolve<EmailServerDriver>();
            await emailServerDriver.ClearAllEmailsAsync();
        }
    }
}
