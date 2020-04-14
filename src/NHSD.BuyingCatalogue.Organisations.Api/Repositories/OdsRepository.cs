using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using NHSD.BuyingCatalogue.Organisations.Api.Extensions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    internal sealed class OdsRepository : IOdsRepository
    {
        private readonly string _buyerOrganisationRoleId;
        private readonly string _odsApiBaseUrl;

        public OdsRepository(string odsApiBaseUrl)
        {
            _buyerOrganisationRoleId = "RO98";
            _odsApiBaseUrl = odsApiBaseUrl;
        }

        public async Task<OdsOrganisation> GetBuyerOrganisationByOdsCode(string odsCode)
        {
            // instance of an ExpandoObject, containing values from the json response
            dynamic response = await _odsApiBaseUrl
                .AppendPathSegment("organisations")
                .AppendPathSegment(odsCode)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync();
            
            dynamic odsOrganisation = DynamicCast.GetPropertyOrDefault<dynamic>(response, "Organisation");

            return odsOrganisation is null ? null : new OdsOrganisation
            {
                OrganisationName = DynamicCast.GetPropertyOrDefault<string>(odsOrganisation, "Name"),
                OdsCode = odsCode,
                PrimaryRoleId = _buyerOrganisationRoleId,
                Address = ConvertDynamicObjectToAddress(odsOrganisation),
                IsActive = IsActive(odsOrganisation),
                IsBuyerOrganisation = IsBuyerOrganisation(odsOrganisation)
            };
        }

        private static bool IsActive(dynamic organisation)
        {
            return DynamicCast.GetPropertyOrDefault<string>(organisation, "Status").Equals("Active", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsBuyerOrganisation(dynamic organisation)
        {
            List<dynamic> roles = organisation.Roles.Role;
            return roles.Select(role => DynamicCast.GetPropertyOrDefault<string>(role, "id"))
                        .Any(id => id.Equals(_buyerOrganisationRoleId, StringComparison.OrdinalIgnoreCase));
        }

        private static Address ConvertDynamicObjectToAddress(dynamic organisation)
        {
            var extractProperty = new Func<string, string>(property => DynamicCast.GetPropertyOrDefault<string>(organisation.GeoLoc.Location, property));
    
            return new Address
            {
                Line1 = extractProperty("AddrLn1"),
                Line2 = extractProperty("AddrLn2"),
                Line3 = extractProperty("AddrLn3"),
                Line4 = extractProperty("AddrLn4"),
                Town = extractProperty("Town"),
                County = extractProperty("County"),
                Postcode = extractProperty("PostCode"),
                Country = extractProperty("Country"),
            };
        }
    }
}
