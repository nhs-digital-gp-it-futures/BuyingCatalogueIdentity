using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientBuilder
    {
        private readonly string name;
        private readonly string odsCode;

        private ServiceRecipientBuilder(int index)
        {
            name = $"Service {index}";
            odsCode = $"ODS {index}";
        }

        internal static ServiceRecipientBuilder Create(int index)
        {
            return new(index);
        }

        internal ServiceRecipient Build()
        {
            return new() { Name = name, OrgId = odsCode };
        }
    }
}
