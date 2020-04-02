using BoDi;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests
{
    [Binding]
    public sealed class RegisterDependencies
    {
        [BeforeScenario]
        public void Register(IObjectContainer objectContainer)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            objectContainer.RegisterInstanceAs<IConfiguration>(config);
        }
    }
}
