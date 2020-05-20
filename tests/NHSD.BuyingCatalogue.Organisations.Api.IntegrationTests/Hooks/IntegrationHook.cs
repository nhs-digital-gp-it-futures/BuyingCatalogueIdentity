using System;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Hooks
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
        public async Task CleanUpAsync()
        {
            await DeleteAllOdsEndpointMappings();
        }

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

        private async Task DeleteAllOdsEndpointMappings()
        {
            var odsApi = _objectContainer.Resolve<OdsApiSteps>();
            await odsApi.ClearMappings();
        }
    }
}
