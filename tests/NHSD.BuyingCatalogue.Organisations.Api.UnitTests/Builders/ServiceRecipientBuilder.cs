using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientBuilder
    {
        private string _name;
        private string _odsCode;

        private ServiceRecipientBuilder(int index)
        {
            _name = $"Service {index}";
            _odsCode = $"ODS {index}";
        }

        internal static ServiceRecipientBuilder Create(int index)
        {
            return new ServiceRecipientBuilder(index);
        }

        internal ServiceRecipientBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal ServiceRecipientBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        internal ServiceRecipient Build()
        {
            return new ServiceRecipient {Name = _name, OrgId = _odsCode};
        }
    }
}
