using System.Collections.Generic;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Messages;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users
{
    public sealed class CreateBuyerResponseViewModel
    {
        public string UserId { get; set; }

        public IEnumerable<ErrorMessageViewModel> Errors { get; set; }
    }
}
