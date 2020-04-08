using System.Collections.Generic;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Messages;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public sealed class CreateOrganisationResponseViewModel
    {
        public string OrganisationId { get; set; }

        public IEnumerable<ErrorMessageViewModel> Errors { get; set; }
    }
}
