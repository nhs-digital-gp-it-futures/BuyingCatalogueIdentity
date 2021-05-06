using System;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OdsOrganisationBuilder
    {
        private Guid organisationId;
        private readonly string odsCode;
        private readonly string name;
        private readonly string primaryRoleId;
        private readonly Address address;
        private readonly bool isActive;
        private readonly bool isBuyerOrganisation;

        private OdsOrganisationBuilder(int index, bool isActiveBuyerOrganisation)
        {
            organisationId = Guid.NewGuid();
            name = $"Organisation {index}";
            odsCode = $"ODS {index}";
            primaryRoleId = $"ID {index}";
            address = null;
            isActive = isActiveBuyerOrganisation;
            isBuyerOrganisation = isActiveBuyerOrganisation;
        }

        internal static OdsOrganisationBuilder Create(int index, bool isActiveBuyerOrganisation = false)
        {
            return new(index, isActiveBuyerOrganisation);
        }

        internal OdsOrganisation Build()
        {
            return new()
            {
                OdsCode = odsCode,
                OrganisationId = organisationId,
                OrganisationName = name,
                PrimaryRoleId = primaryRoleId,
                Address = address,
                IsActive = isActive,
                IsBuyerOrganisation = isBuyerOrganisation,
            };
        }
    }
}
