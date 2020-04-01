using System.Collections.Generic;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Messages;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users
{
    public sealed class CreateBuyerResponseViewModel
    {
        public IEnumerable<ErrorMessageViewModel> Errors { get; set; }
    }
}
