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
