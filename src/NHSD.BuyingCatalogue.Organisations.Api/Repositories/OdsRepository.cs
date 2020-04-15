using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    internal sealed class OdsRepository : IOdsRepository
    {
        private readonly OdsSettings _settings;

        public OdsRepository(OdsSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<OdsOrganisation> GetBuyerOrganisationByOdsCodeAsync(string odsCode)
        {
            var response = await _settings.ApiBaseUrl
                .AppendPathSegment("organisations")
                .AppendPathSegment(odsCode)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync<OdsResponse>();

            var odsOrganisation = response.Organisation;

            return odsOrganisation is null ? null : new OdsOrganisation
            {
                OrganisationName = odsOrganisation.Name,
                OdsCode = odsCode,
                PrimaryRoleId = GetPrimaryRoleId(odsOrganisation),
                Address = OdsResponseAddressToAddress(odsOrganisation.GeoLoc.Location),
                IsActive = IsActive(odsOrganisation),
                IsBuyerOrganisation = IsBuyerOrganisation(odsOrganisation)
            };
        }

        private static bool IsActive(OdsResponseOrganisation organisation)
        {
            return organisation.Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsBuyerOrganisation(OdsResponseOrganisation organisation)
        {
            return _settings.BuyerOrganisationRoleIds.Contains(GetPrimaryRoleId(organisation));
        }

        private static string GetPrimaryRoleId(OdsResponseOrganisation organisation)
        {
            return organisation.Roles.Role.Where(r => r.primaryRole).Select(r => r.id).FirstOrDefault();
        }

        private static Address OdsResponseAddressToAddress(OdsResponseAddress odsAddress)
        {
            return new Address
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

        private class OdsResponse
        {
            public OdsResponseOrganisation Organisation { get; set; }
        }

        private class OdsResponseOrganisation
        {
            public string Name { get; set; }
            public string Status { get; set; }
            public GeoLoc GeoLoc { get; set; }
            public OdsResponseRoles Roles { get; set; }
        }

        private class GeoLoc
        {
            public OdsResponseAddress Location { get; set; }
        }

        private class OdsResponseAddress
        {
            public string AddrLn1 { get; set; }
            public string AddrLn2 { get; set; }
            public string AddrLn3 { get; set; }
            public string AddrLn4 { get; set; }
            public string Town { get; set; }
            public string County { get; set; }
            public string PostCode { get; set; }
            public string Country { get; set; }
        }

        private class OdsResponseRoles
        {
            public List<OdsResponseRole> Role { get; set; }
        }

        private class OdsResponseRole
        {
            public string id { get; set; }
            public bool primaryRole { get; set; }
        }
    }
}
