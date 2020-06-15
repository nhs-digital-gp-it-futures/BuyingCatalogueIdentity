using System;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OrganisationBuilder
    {
        private Guid _organisationId;
        private string _name;
        private string _odsCode;
        private string _primaryRoleId;
        private bool _catalogueAgreementSigned;
        private Address _address;
        private readonly DateTime _lastUpdated;

        private OrganisationBuilder(int index)
        {
            _organisationId = Guid.NewGuid();
            _name = $"Organisation {index}";
            _odsCode = $"ODS {index}";
            _primaryRoleId = $"ID {index}";
            _catalogueAgreementSigned = true;
            _address = null;
            _lastUpdated = DateTime.UtcNow;
        }

        internal static OrganisationBuilder Create(int index)
        {
            return new OrganisationBuilder(index);
        }

        internal OrganisationBuilder WithOrganisationId(Guid organisationId)
        {
            _organisationId = organisationId;
            return this;
        }

        internal OrganisationBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal OrganisationBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        internal OrganisationBuilder WithPrimaryRoleId(string primaryRoleId)
        {
            _primaryRoleId = primaryRoleId;
            return this;
        }

        internal OrganisationBuilder WithCatalogueAgreementSigned(bool isSigned)
        {
            _catalogueAgreementSigned = isSigned;
            return this;
        }

        internal OrganisationBuilder WithAddress(Address address)
        {
            _address = address;
            return this;
        }

        internal Organisation Build()
        {
            return new Organisation
            {
                OrganisationId = _organisationId,
                Name = _name,
                OdsCode = _odsCode,
                PrimaryRoleId = _primaryRoleId,
                CatalogueAgreementSigned = _catalogueAgreementSigned,
                Address = _address,
                LastUpdated = _lastUpdated
            };
        }
    }
}
