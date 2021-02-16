using System;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OrganisationBuilder
    {
        private readonly DateTime lastUpdated;

        private Guid organisationId;
        private string name;
        private string odsCode;
        private string primaryRoleId;
        private bool catalogueAgreementSigned;
        private Address address;

        private OrganisationBuilder(int index)
        {
            organisationId = Guid.NewGuid();
            name = $"Organisation {index}";
            odsCode = $"ODS {index}";
            primaryRoleId = $"ID {index}";
            catalogueAgreementSigned = true;
            address = null;
            lastUpdated = DateTime.UtcNow;
        }

        internal static OrganisationBuilder Create(int index)
        {
            return new(index);
        }

        internal OrganisationBuilder WithOrganisationId(Guid id)
        {
            organisationId = id;
            return this;
        }

        internal OrganisationBuilder WithName(string value)
        {
            name = value;
            return this;
        }

        internal OrganisationBuilder WithOdsCode(string code)
        {
            odsCode = code;
            return this;
        }

        internal OrganisationBuilder WithPrimaryRoleId(string id)
        {
            primaryRoleId = id;
            return this;
        }

        internal OrganisationBuilder WithCatalogueAgreementSigned(bool isSigned)
        {
            catalogueAgreementSigned = isSigned;
            return this;
        }

        internal OrganisationBuilder WithAddress(Address value)
        {
            address = value;
            return this;
        }

        internal Organisation Build()
        {
            return new()
            {
                OrganisationId = organisationId,
                Name = name,
                OdsCode = odsCode,
                PrimaryRoleId = primaryRoleId,
                CatalogueAgreementSigned = catalogueAgreementSigned,
                Address = address,
                LastUpdated = lastUpdated,
            };
        }
    }
}
