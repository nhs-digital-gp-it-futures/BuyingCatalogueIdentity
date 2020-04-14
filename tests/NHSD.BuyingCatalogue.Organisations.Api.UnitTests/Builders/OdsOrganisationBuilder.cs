using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OdsOrganisationBuilder
    {
        private string _odsCode;
        private string _name;
        private string _primaryRoleId;
        private Address _address;
        private bool _isActive;
        private bool _isBuyerOrganisation;

        private OdsOrganisationBuilder(int index, bool isActiveBuyerOrganisation)
        {
            _name = $"Organisation {index}";
            _odsCode = $"ODS {index}";
            _primaryRoleId = $"ID {index}";
            _address = null;
            _isActive = isActiveBuyerOrganisation;
            _isBuyerOrganisation = isActiveBuyerOrganisation;
        }

        internal static OdsOrganisationBuilder Create(int index, bool isActiveBuyerOrganisation = false)
        {
            return new OdsOrganisationBuilder(index, isActiveBuyerOrganisation);
        }

        internal OdsOrganisationBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        internal OdsOrganisationBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal OdsOrganisationBuilder WithPrimaryRoleId(string primaryRoleId)
        {
            _primaryRoleId = primaryRoleId;
            return this;
        }

        internal OdsOrganisationBuilder WithAddress(Address address)
        {
            _address = address;
            return this;
        }
        internal OdsOrganisationBuilder WithIsActive(bool isActive)
        {
            _isActive = isActive;
            return this;
        }

        internal OdsOrganisationBuilder WithIsBuyerOrganisation(bool isBuyerOrganisation)
        {
            _isBuyerOrganisation = isBuyerOrganisation;
            return this;
        }
        internal OdsOrganisation Build()
        {
            return new OdsOrganisation
            {
                OdsCode = _odsCode,
                OrganisationName = _name,
                PrimaryRoleId = _primaryRoleId,
                Address = _address,
                IsActive = _isActive,
                IsBuyerOrganisation = _isBuyerOrganisation
            };
        }
    }
}
