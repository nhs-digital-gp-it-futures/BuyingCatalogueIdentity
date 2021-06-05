using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using JetBrains.Annotations;
using LazyCache;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    internal sealed class OdsRepository : IOdsRepository
    {
        private readonly OdsSettings settings;
        private readonly IAppCache appCache;

        public OdsRepository(OdsSettings settings, IAppCache appCache)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.appCache = appCache;
        }

        public async Task<OdsOrganisation> GetBuyerOrganisationByOdsCodeAsync(string odsCode)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
                throw new ArgumentException($"A valid {nameof(odsCode)} is required for this call");

            var response = await appCache.GetOrAddAsync(
                odsCode,
                () => settings.ApiBaseUrl
                    .AppendPathSegment("organisations")
                    .AppendPathSegment(odsCode)
                    .AllowHttpStatus("3xx,4xx")
                    .GetJsonAsync<OdsResponse>());

            var odsOrganisation = response?.Organisation;

            return odsOrganisation is null ? null : new OdsOrganisation
            {
                OrganisationName = odsOrganisation.Name,
                OdsCode = odsCode,
                PrimaryRoleId = GetPrimaryRoleId(odsOrganisation),
                Address = OdsResponseAddressToAddress(odsOrganisation.GeoLoc.Location),
                IsActive = IsActive(odsOrganisation),
                IsBuyerOrganisation = IsBuyerOrganisation(odsOrganisation),
            };
        }

        private static bool IsActive(OdsResponseOrganisation organisation)
        {
            return organisation.Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetPrimaryRoleId(OdsResponseOrganisation organisation)
        {
            return organisation.Roles.Role.FirstOrDefault(r => r.primaryRole)?.id;
        }

        private static Address OdsResponseAddressToAddress(OdsResponseAddress odsAddress)
        {
            return new()
            {
                Line1 = odsAddress.AddrLn1,
                Line2 = odsAddress.AddrLn2,
                Line3 = odsAddress.AddrLn3,
                Line4 = odsAddress.AddrLn4,
                Town = odsAddress.Town,
                County = odsAddress.County,
                Postcode = odsAddress.PostCode,
                Country = odsAddress.Country,
            };
        }

        private bool IsBuyerOrganisation(OdsResponseOrganisation organisation)
        {
            return settings.BuyerOrganisationRoleIds.Contains(GetPrimaryRoleId(organisation));
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OdsResponse
        {
            public OdsResponseOrganisation Organisation { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OdsResponseOrganisation
        {
            public string Name { get; init; }

            public string Status { get; init; }

            public GeoLoc GeoLoc { get; init; }

            public OdsResponseRoles Roles { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class GeoLoc
        {
            public OdsResponseAddress Location { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OdsResponseAddress
        {
            // ReSharper disable once IdentifierTypo
            public string AddrLn1 { get; init; }

            // ReSharper disable once IdentifierTypo
            public string AddrLn2 { get; init; }

            // ReSharper disable once IdentifierTypo
            public string AddrLn3 { get; init; }

            // ReSharper disable once IdentifierTypo
            public string AddrLn4 { get; init; }

            public string Town { get; init; }

            public string County { get; init; }

            public string PostCode { get; init; }

            public string Country { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OdsResponseRoles
        {
            public List<OdsResponseRole> Role { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "ODS naming")]
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "ODS naming")]
        private sealed class OdsResponseRole
        {
            public string id { get; init; }

            public bool primaryRole { get; init; }
        }
    }
}
