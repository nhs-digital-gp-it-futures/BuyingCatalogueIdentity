using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Testing;
using LazyCache;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Repository
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OdsRepositoryTests
    {
        private const string OdsCode = "XYZ";

        private const string ValidResponseBody = @"{""Organisation"": "
            + @"{""Name"": ""SOUTH EAST - H&J COMMISSIONING HUB"", "
            + @"""Date"": [{""Type"": ""Operational"", ""Start"": ""2019-10-25""}, "
            + @"{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], "
            + @"""OrgId"": {""root"": ""2.16.840.1.113883.2.1.3.2.4.18.48"", ""assigningAuthorityName"": ""HSCIC"", ""extension"": ""97T""}, "
            + @"""Status"": ""Active"", ""LastChangeDate"": ""2020-04-01"", ""orgRecordClass"": ""RC1"", "
            + @"""GeoLoc"": {""Location"": {""AddrLn1"": ""C/O NHS ENGLAND"", ""AddrLn2"": ""1W09, 1ST FLOOR, QUARRY HOUSE"", "
            + @"""AddrLn3"": ""QUARRY HILL"", ""Town"": ""LEEDS"", ""PostCode"": ""LS2 7UA"", ""Country"": ""ENGLAND""}}, "
            + @"""Roles"": {""Role"": [{""id"": ""RO218"", ""uniqueRoleId"": 391223, "
            + @"""Date"": [{""Type"": ""Operational"", ""Start"": ""2019-10-25""}, "
            + @"{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Status"": ""Active""}, "
            + @"{""id"": ""RO98"", ""uniqueRoleId"": 386574, ""primaryRole"": true, "
            + @"""Date"": [{""Type"": ""Operational"", ""Start"": ""2019-10-25""},"
            + @" {""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Status"": ""Active""}]}, "
            + @"""Rels"": {""Rel"": [{""Date"": [{""Type"": ""Operational"", ""Start"": ""2019-10-25""}, "
            + @"{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Status"": ""Active"", "
            + @"""Target"": {""OrgId"": {""root"": ""2.16.840.1.113883.2.1.3.2.4.18.48"", ""assigningAuthorityName"": ""HSCIC"", ""extension"": ""Y59""}, "
            + @"""PrimaryRoleId"": {""id"": ""RO209"", ""uniqueRoleId"": 299360}}, ""id"": ""RE5"", ""uniqueRelId"": 619596}]}, "
            + @"""Succs"": {""Succ"": [{""uniqueSuccId"": 37762, ""Date"": [{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Type"": ""Predecessor"", "
            + @"""Target"": {""OrgId"": {""root"": ""2.16.840.1.113883.2.1.3.2.4.18.48"", ""assigningAuthorityName"": ""HSCIC"", ""extension"": ""14W""}, "
            + @"""PrimaryRoleId"": {""id"": ""RO98"", ""uniqueRoleId"": 296831}}}, {""uniqueSuccId"": 37761, "
            + @"""Date"": [{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Type"": ""Predecessor"", "
            + @"""Target"": {""OrgId"": {""root"": ""2.16.840.1.113883.2.1.3.2.4.18.48"", ""assigningAuthorityName"": ""HSCIC"", ""extension"": ""14V""}, "
            + @"""PrimaryRoleId"": {""id"": ""RO98"", ""uniqueRoleId"": 296829}}}]}}}";

        [Test]
        public static async Task GetBuyerOrganisationByOdsCode_WithValidResponse_Returns_BuyerOrganisation()
        {
            var context = OdsRepositoryTestContext.Setup();
            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: ValidResponseBody);

            var expected = new OdsOrganisation
            {
                OdsCode = OdsCode,
                OrganisationName = "SOUTH EAST - H&J COMMISSIONING HUB",
                PrimaryRoleId = "RO98",
                Address = new Address
                {
                    Line1 = "C/O NHS ENGLAND",
                    Line2 = "1W09, 1ST FLOOR, QUARRY HOUSE",
                    Line3 = "QUARRY HILL",
                    Town = "LEEDS",
                    Postcode = "LS2 7UA",
                    Country = "ENGLAND",
                },
                IsActive = true,
                IsBuyerOrganisation = true,
            };
            var actual = await context.OdsRepository.GetBuyerOrganisationByOdsCodeAsync(OdsCode);

            actual.Should().BeOfType<OdsOrganisation>();
            actual.Should().NotBeNull();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task GetBuyerOrganisationByOdsCode_WithNotFoundResponseFromOdsApi_Returns_Null()
        {
            const string odsCode = "ods-code-839";
            var context = OdsRepositoryTestContext.Setup();
            var url = $"{context.OdsSettings.ApiBaseUrl}/organisations/{odsCode}";
            var cachingService = new CachingService();
            cachingService.CacheProvider.Remove(odsCode);

            var repository = new OdsRepository(context.OdsSettings, cachingService);
            using var httpTest = new HttpTest();
            httpTest
                .ForCallsTo(url)
                .RespondWithJson(new { ErrorCode = 404, ErrorText = "Not Found." }, 404);

            var result = await repository.GetBuyerOrganisationByOdsCodeAsync(odsCode);

            httpTest.ShouldHaveCalled(url)
                .WithVerb(HttpMethod.Get)
                .Times(1);
            result.Should().BeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void GetBuyerOrganisationByOdsCode_InvalidOdsCode_ThrowsException(string invalid)
        {
            var context = OdsRepositoryTestContext.Setup();
            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 500);

            Assert.ThrowsAsync<ArgumentException>(
                async () => await context.OdsRepository.GetBuyerOrganisationByOdsCodeAsync(invalid))
                .Message.Should().Be($"A valid odsCode is required for this call");
        }

        [Test]
        public static void GetBuyerOrganisationByOdsCode_WithInternalServerErrorResponseFromOdsApi_Throws()
        {
            var context = OdsRepositoryTestContext.Setup();
            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 500);

            Assert.ThrowsAsync<FlurlHttpException>(async () => await context.OdsRepository.GetBuyerOrganisationByOdsCodeAsync("invalid"));
        }

        [Test]
        [Ignore("Fails intermittently on server")]
        public static async Task GetBuyerOrganisationByOdsCode_CallsOdsApi_OnceForMultipleCalls()
        {
            var baseUrl = "https://fakeodsserver.net/ORD/2-0-0";
            var repository = new OdsRepository(
                new OdsSettings
                {
                    ApiBaseUrl = baseUrl,
                    BuyerOrganisationRoleIds = new[] { "RO98", "RO177", "RO213", "RO272" },
                },
                new CachingService());
            var url = $"{baseUrl}/organisations/{OdsCode}";
            using var httpTest = new HttpTest();
            httpTest
                .ForCallsTo(url)
                .RespondWith(status: 200, body: ValidResponseBody);
            if (httpTest.GetType()
                .GetField("_calls", BindingFlags.Instance | BindingFlags.NonPublic)
                ?
                .GetValue(httpTest) is ConcurrentQueue<FlurlCall> calls)
            {
                calls.Clear();
            }

            await repository.GetBuyerOrganisationByOdsCodeAsync(OdsCode);
            await repository.GetBuyerOrganisationByOdsCodeAsync(OdsCode);
            await repository.GetBuyerOrganisationByOdsCodeAsync(OdsCode);

            httpTest.ShouldHaveCalled(url)
                .WithVerb(HttpMethod.Get)
                .Times(1);
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public static void Constructor_IOdsRepository_OdsSettings_NullSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new OdsRepository(null, new CachingService()));
        }
    }
}
