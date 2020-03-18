using System;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class OrganisationBuilder
    {
        private readonly Guid _organisationId;

        private readonly string _name;

        private readonly string _odsCode;

        private readonly string _primaryRoleId;

        private bool _catalogueAgreementSigned;

        private Address _address;

        private readonly DateTime _lastUpdated;

        public OrganisationBuilder(int index)
        {
            _organisationId = Guid.NewGuid();
            _name = $"Organisation {index}";
            _odsCode = $"ODS {index}";
            _primaryRoleId = $"ID {index}";
            _catalogueAgreementSigned = true;
            _address = null;
            _lastUpdated = DateTime.UtcNow;
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

        internal static OrganisationBuilder Create(int index)
        {
            return new OrganisationBuilder(index);
        }

        internal Organisation Build()
        {
            return new Organisation()
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
