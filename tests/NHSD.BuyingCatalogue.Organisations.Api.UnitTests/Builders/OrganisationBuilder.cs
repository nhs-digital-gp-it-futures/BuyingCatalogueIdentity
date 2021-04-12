using System;
using System.Collections.Generic;
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
        private RelatedOrganisation relatedOrganisation;

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

        internal OrganisationBuilder WithRelatedOrganisation(Guid relatedOrganisationId)
        {
            relatedOrganisation = new()
            {
                OrganisationId = organisationId,
                RelatedOrganisationId = relatedOrganisationId,
                Organisation = Build(),
                ChildOrganisation = BuildRelatedOrganisation(relatedOrganisationId),
            };
            return this;
        }

        internal Organisation Build()
        {
            Organisation org = new()
            {
                OrganisationId = organisationId,
                Name = name,
                OdsCode = odsCode,
                PrimaryRoleId = primaryRoleId,
                CatalogueAgreementSigned = catalogueAgreementSigned,
                Address = address,
                LastUpdated = lastUpdated,
            };

            if (relatedOrganisation != null)
            {
                org.RelatedOrganisations.Add(relatedOrganisation);
            }

            return org;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "This Doesn't Actually need to be at the top")]
        internal static Organisation BuildRelatedOrganisation(Guid index)
        {
            return new()
            {
                OrganisationId = index,
                Name = $"Organisation {index}",
                OdsCode = $"ODS {index}",
                PrimaryRoleId = $"ID {index}",
                CatalogueAgreementSigned = true,
                Address = null,
                LastUpdated = DateTime.UtcNow,
            };
        }
    }
}
