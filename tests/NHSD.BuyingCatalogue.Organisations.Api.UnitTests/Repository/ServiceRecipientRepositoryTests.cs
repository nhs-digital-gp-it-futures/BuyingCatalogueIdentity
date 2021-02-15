using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Repository
{
    [TestFixture]
    public sealed class ServiceRecipientRepositoryTests
    {
        private const string OdsCode = "XYZ";

        [Test]
        public async Task GetServiceRecipientsByParentOdsCode_SinglePage_ReturnsOrganisation()
        {
            var context = ServiceRecipientTestContext.Setup();
            context.Settings.GetChildOrganisationSearchLimit = 2;
            var childOrg = new ServiceRecipient { Name = "Organisation 1", PrimaryRoleId = "RO177", OrgId = "ABC" };
            var json = CreatePageJson(childOrg);
            context.Http.RespondWith(status: 200, body: json);

            var response = await context.Repository.GetServiceRecipientsByParentOdsCode(OdsCode);
            response.Should().BeEquivalentTo(childOrg);
        }

        [Test]
        public async Task GetServiceRecipientsByParentOdsCode_MultiplePage_ReturnsAllOrganisations()
        {
            var context = ServiceRecipientTestContext.Setup();
            context.Settings.GetChildOrganisationSearchLimit = 1;
            var childOne = new ServiceRecipient { Name = "Organisation 1", PrimaryRoleId = "RO177", OrgId = "ABC" };
            var childTwo = new ServiceRecipient { Name = "Organisation 2", PrimaryRoleId = "RO177", OrgId = "ABD" };
            var jsonPageOne = CreatePageJson(childOne);
            var jsonPageTwo = CreatePageJson(childTwo);
            var jsonPageThree = CreatePageJson();

            context.Http.RespondWith(status: 200, body: jsonPageOne);
            context.Http.RespondWith(status: 200, body: jsonPageTwo);
            context.Http.RespondWith(status: 200, body: jsonPageThree);

            var response = await context.Repository.GetServiceRecipientsByParentOdsCode(OdsCode);
            response.Should().BeEquivalentTo(childOne, childTwo);
        }

        [Test]
        public async Task GetServiceRecipientsByParentOdsCode_SinglePageDifferentRoleIds_ReturnsOnlyMatching()
        {
            var context = ServiceRecipientTestContext.Setup();
            context.Settings.GetChildOrganisationSearchLimit = 3;
            var childOne = new ServiceRecipient { Name = "Organisation 1", PrimaryRoleId = "RO177", OrgId = "ABC" };
            var childTwo = new ServiceRecipient { Name = "Organisation 2", PrimaryRoleId = "RO178", OrgId = "ABD" };
            var jsonPageOne = CreatePageJson(childOne, childTwo);

            context.Http.RespondWith(status: 200, body: jsonPageOne);

            var response = await context.Repository.GetServiceRecipientsByParentOdsCode(OdsCode);
            response.Should().BeEquivalentTo(childOne);
        }

        [Test]
        public async Task GetServiceRecipientsByParentOdsCode_NoOrganisations_ReturnsEmptyList()
        {
            var context = ServiceRecipientTestContext.Setup();
            context.Http.RespondWith(status: 200, body: CreatePageJson());
            var response = await context.Repository.GetServiceRecipientsByParentOdsCode(OdsCode);
            response.Should().BeEmpty();
        }

        [Test]
        public void Ctor_NullSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => { new ServiceRecipientRepository(null); });
        }

        private static string CreatePageJson(params ServiceRecipient[] serviceRecipients)
        {
            var recipientJson = serviceRecipients.Select(x => JsonSerializer.Serialize(x));
            var json = string.Join(',', recipientJson);

            return $@"{{""Organisations"": [{json}]}}";
        }

        private sealed class ServiceRecipientTestContext
        {
            private ServiceRecipientTestContext()
            {
                Settings = new OdsSettings
                {
                    ApiBaseUrl = "https://fakeodsserver.net/ORD/2-0-0",
                    GetChildOrganisationSearchLimit = 1,
                    GpPracticeRoleId = "RO177"
                };
                Repository = new ServiceRecipientRepository(Settings);
                Http = new HttpTest();
            }

            public OdsSettings Settings { get; set; }

            public ServiceRecipientRepository Repository { get; set; }

            public HttpTest Http { get; set; }

            public static ServiceRecipientTestContext Setup()
            {
                return new ServiceRecipientTestContext();
            }
        }
    }
}
